using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using FFXIV_GameSense.Properties;
using FFXIV_GameSense.UI;
using Microsoft.AspNetCore.SignalR.Client;
using NAudio.Wave;
using Splat;
using XIVAPI;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x02000035 RID: 53
	internal class FFXIVHunts : IDisposable
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00006CD0 File Offset: 0x00004ED0
		// (set) Token: 0x0600018D RID: 397 RVA: 0x00006CD7 File Offset: 0x00004ED7
		internal static HttpClient Http { get; private set; } = new HttpClient();

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00006CDF File Offset: 0x00004EDF
		// (set) Token: 0x0600018F RID: 399 RVA: 0x00006CE6 File Offset: 0x00004EE6
		internal static bool Joined { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000190 RID: 400 RVA: 0x00006CEE File Offset: 0x00004EEE
		// (set) Token: 0x06000191 RID: 401 RVA: 0x00006CF5 File Offset: 0x00004EF5
		internal static bool Joining { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000192 RID: 402 RVA: 0x00006D00 File Offset: 0x00004F00
		private static World CurrentWorld
		{
			get
			{
				World world;
				if (!FFXIVHunts.Worlds.TryGetValue(FFXIVHunts.LastJoinedWorldID, out world) && !FFXIVHunts.Worlds.TryGetValue(Program.mem.GetCurrentWorldId(), out world))
				{
					return null;
				}
				return world;
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00006D40 File Offset: 0x00004F40
		internal async Task LeaveGroup()
		{
			if (FFXIVHunts.Joined)
			{
				await this.LeaveDCZone().ConfigureAwait(false);
				await FFXIVHunts.hubConnection.Connection.InvokeAsync("LeaveGroup", FFXIVHunts.LastJoinedWorldID, default(CancellationToken)).ConfigureAwait(false);
				LogHost.Default.Info("Left " + GameResources.GetWorldName(FFXIVHunts.LastJoinedWorldID));
				FFXIVHunts.Joined = false;
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00006D85 File Offset: 0x00004F85
		internal FFXIVHunts(Window1 pw1)
		{
			this.w1 = pw1;
			FFXIVHunts.CreateWorldsOnDC(GameResources.GetWorld(Program.mem.GetHomeWorldId()).GetWorldsOnSameDataCenter());
			this.CreateConnection();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00006DB4 File Offset: 0x00004FB4
		private static void CreateWorldsOnDC(IEnumerable<World> worldsOnCurrentDC)
		{
			foreach (World worldNotYetCreated in from x in worldsOnCurrentDC
			where !FFXIVHunts.Worlds.ContainsKey(x.ID)
			select x)
			{
				FFXIVHunts.Worlds.Add(worldNotYetCreated.ID, new World(worldNotYetCreated.ID));
				foreach (KeyValuePair<ushort, HuntRank> kvp in Hunt.RankMap)
				{
					if (worldNotYetCreated.ID > 100 && worldNotYetCreated.ID < 2000 && (kvp.Key > 8000 || FFXIVHunts.GetZoneId(kvp.Key) == 156 || FFXIVHunts.GetZoneId(kvp.Key) == 152))
					{
						for (byte i = 1; i <= 3; i += 1)
						{
							FFXIVHunts.Worlds[worldNotYetCreated.ID].Hunts.Add(new Hunt(kvp.Key, worldNotYetCreated.ID)
							{
								Instance = i
							});
						}
					}
					else
					{
						FFXIVHunts.Worlds[worldNotYetCreated.ID].Hunts.Add(new Hunt(kvp.Key, worldNotYetCreated.ID));
					}
				}
			}
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00006F54 File Offset: 0x00005154
		private void CreateConnection()
		{
			if (FFXIVHunts.hubConnection == null)
			{
				FFXIVHunts.hubConnection = new HuntsHubConnection();
				this.RegisterHubMethods();
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00006F70 File Offset: 0x00005170
		private void RegisterHubMethods()
		{
			FFXIVHunts.hubConnection.Connection.On("ReceiveHunt", delegate(Hunt hunt)
			{
				this.ReceiveHunt(hunt);
			});
			FFXIVHunts.hubConnection.Connection.On("ReceiveFATE", delegate(FATEReport fate)
			{
				this.ReceiveFATE(fate);
			});
			FFXIVHunts.hubConnection.Connection.On("DCInstanceMatch", delegate(DataCenterInstanceMatchInfo instance)
			{
				FFXIVHunts.DCInstanceMatch(instance);
			});
			FFXIVHunts.hubConnection.Connection.On("ConnectedCount", delegate(int connectedCount)
			{
				this.w1.HuntConnectionTextBlock.Dispatcher.Invoke(delegate()
				{
					string worldName = GameResources.GetWorldName(Program.mem.GetCurrentWorldId());
					IEnumerable<string> enumerable = string.Format(CultureInfo.CurrentCulture, Resources.FormConnectedToCount, worldName, connectedCount - 1).SplitAndKeep(worldName, StringComparison.Ordinal);
					this.w1.HuntConnectionTextBlock.Inlines.Clear();
					Hyperlink worldLink = new Hyperlink(new Run(worldName))
					{
						NavigateUri = new Uri("https://xivhunt.net/" + worldName)
					};
					worldLink.RequestNavigate += LogInForm.Link_RequestNavigate;
					foreach (string s in enumerable)
					{
						if (s.Equals(worldName, StringComparison.Ordinal))
						{
							this.w1.HuntConnectionTextBlock.Inlines.Add(worldLink);
						}
						else
						{
							this.w1.HuntConnectionTextBlock.Inlines.Add(s);
						}
					}
				});
			});
			FFXIVHunts.hubConnection.Connection.Closed += this.Connection_Closed;
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00007030 File Offset: 0x00005230
		private static void DCInstanceMatch(DataCenterInstanceMatchInfo instance)
		{
			string s = string.Format(CultureInfo.CurrentCulture, Resources.DCInstanceMatch, Program.AssemblyName.Name, (FFXIVHunts.ServerTimeUtc - instance.StartTime).TotalMinutes.ToString("F0", CultureInfo.CurrentCulture), string.Format("{0}DCInstance/{1}", "https://xivhunt.net/", instance.ID));
			LogHost.Default.Info("DCInstanceMatch: " + s);
			FFXIVHunts.DCInstance = instance;
			ChatMessage cm = new ChatMessage(s);
			Program.mem.WriteChatMessage(cm);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x000070C9 File Offset: 0x000052C9
		private void ReceiveFATE(FATEReport fate)
		{
			if (this.PutInChat(fate) && Settings.Default.FlashTaskbarIconOnHuntAndFATEs)
			{
				NativeMethods.FlashTaskbarIcon(Program.mem.Process);
			}
		}

		// Token: 0x0600019A RID: 410 RVA: 0x000070F0 File Offset: 0x000052F0
		private void ReceiveHunt(Hunt hunt)
		{
			LogHost.Default.Debug(string.Format(CultureInfo.CurrentCulture, "[{0}] Report received: {1}", GameResources.GetWorldName(hunt.WorldId), hunt.Name));
			if (this.PutInChat(hunt) && Settings.Default.FlashTaskbarIconOnHuntAndFATEs)
			{
				if (hunt.LastAlive)
				{
					NativeMethods.FlashTaskbarIcon(Program.mem.Process, 45u, true);
					return;
				}
				NativeMethods.StopFlashWindowEx(Program.mem.Process);
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00007168 File Offset: 0x00005368
		private Task Connection_Closed(Exception arg)
		{
			FFXIVHunts.Joined = (FFXIVHunts.Joining = false);
			return Task.CompletedTask;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000717C File Offset: 0x0000537C
		internal HuntRank HuntRankFor(ushort HuntID)
		{
			HuntRank hr;
			if (Hunt.TryGetHuntRank(HuntID, out hr))
			{
				return hr;
			}
			throw new ArgumentException("Unknown hunt", "HuntID");
		}

		// Token: 0x0600019D RID: 413 RVA: 0x000071A4 File Offset: 0x000053A4
		internal async Task Connect()
		{
			this.CreateConnection();
			ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = FFXIVHunts.hubConnection.Connect(this.w1).ConfigureAwait(false).GetAwaiter();
			if (!configuredTaskAwaiter.IsCompleted)
			{
				await configuredTaskAwaiter;
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
				configuredTaskAwaiter = configuredTaskAwaiter2;
				configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
			}
			if (configuredTaskAwaiter.GetResult())
			{
				this.RegisterHubMethods();
			}
			if (!FFXIVHunts.Joined && FFXIVHunts.hubConnection.Connected)
			{
				await this.JoinServerGroup().ConfigureAwait(false);
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000071EC File Offset: 0x000053EC
		private bool PutInChat(FATEReport fate)
		{
			World world;
			if (!FFXIVHunts.Worlds.TryGetValue(fate.WorldId, out world))
			{
				return false;
			}
			int idx = world.FATEs.IndexOf(fate);
			if (idx == -1)
			{
				return false;
			}
			FATEReport FATEReport = world.FATEs[idx];
			FATEReport.State = fate.State;
			FATEReport.StartTimeEpoch = fate.StartTimeEpoch;
			FATEReport.Duration = fate.Duration;
			FATEReport.Progress = fate.Progress;
			bool skipAnnounce = (Settings.Default.NoAnnouncementsInContent && Program.mem.GetCurrentContentFinderCondition() > 0) || (Math.Abs(fate.TimeRemaining.TotalHours) < 3.0 && fate.TimeRemaining.TotalMinutes < (double)Settings.Default.FATEMinimumMinutesRemaining) || ((fate.State == FATEState.Preparation) ? (FATEReport.lastPutInChat > Program.mem.GetServerUtcTime().AddMinutes(-10.0)) : (Math.Abs((int)(fate.Progress - FATEReport.LastReportedProgress)) < (int)Settings.Default.FATEMinimumPercentInterval && Settings.Default.FATEMinimumPercentInterval > 0));
			if (FFXIVHunts.FateNotifyCheck(FATEReport.ID) && FATEReport.lastPutInChat < Program.mem.GetServerUtcTime().AddMinutes((double)(-(double)Settings.Default.FATEInterval)) && !fate.HasEnded && !skipAnnounce)
			{
				string postpend;
				if (fate.State == FATEState.Preparation)
				{
					postpend = Resources.PreparationState;
				}
				else if (Math.Abs(fate.TimeRemaining.TotalHours) > 3.0)
				{
					postpend = fate.Progress.ToString() + "%";
				}
				else
				{
					postpend = string.Format(CultureInfo.CurrentCulture, Resources.FATEPrcTimeRemaining, fate.Progress, (int)fate.TimeRemaining.TotalMinutes, fate.TimeRemaining.Seconds.ToString("D2", CultureInfo.CreateSpecificCulture(Settings.Default.LanguageCI)));
				}
				if (fate.Instance > 0)
				{
					postpend += string.Format(CultureInfo.CurrentCulture, " " + Resources.InstanceSpecifier, fate.Instance);
				}
				ChatMessage cm = ChatMessage.MakePosChatMessage(string.Format(CultureInfo.CurrentCulture, FFXIVHunts.GetWorldPrepend(fate.WorldId) + Resources.FATEMsg, FATEReport.Name(false)), fate.ZoneID, fate.PosX, fate.PosY, " " + postpend, 0);
				Program.mem.WriteChatMessage(cm);
				this.CheckAndPlaySound(HuntRank.FATE);
				FATEReport.lastPutInChat = Program.mem.GetServerUtcTime();
				FATEReport.LastReportedProgress = fate.Progress;
				return true;
			}
			return false;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000074CD File Offset: 0x000056CD
		private static string GetWorldPrepend(ushort wid)
		{
			if (Settings.Default.NotificationsFromOtherWorlds)
			{
				return "[" + GameResources.GetWorldName(wid) + "] ";
			}
			return string.Empty;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x000074F8 File Offset: 0x000056F8
		internal async Task QueryHunt(ushort id)
		{
			if (FFXIVHunts.hubConnection.Connected && FFXIVHunts.Joined)
			{
				IOrderedEnumerable<Hunt> results = from x in await FFXIVHunts.hubConnection.Connection.InvokeAsync("QueryHunt", id, default(CancellationToken)).ConfigureAwait(false)
				orderby x.LastReported
				select x;
				foreach (Hunt result in results)
				{
					TimeSpan timeSinceLastReport = FFXIVHunts.ServerTimeUtc.Subtract(result.LastReported);
					if (timeSinceLastReport < TimeSpan.Zero)
					{
						timeSinceLastReport = TimeSpan.Zero;
					}
					ChatMessage cm = new ChatMessage();
					double TotalHours = Math.Floor(timeSinceLastReport.TotalHours);
					if (!result.LastAlive)
					{
						cm.MessageString = FFXIVHunts.GetWorldPrepend(result.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.LKIHuntKilled, result.Name);
						if (Resources.LKIHuntKilled.Contains("<time>"))
						{
							ChatMessage chatMessage = cm;
							chatMessage.MessageString += cm.MessageString.Replace("<time>", string.Format(CultureInfo.CurrentCulture, Resources.LKIHoursMinutes, TotalHours, timeSinceLastReport.Minutes));
						}
						else if (timeSinceLastReport.TotalDays > 90.0)
						{
							cm.MessageString = FFXIVHunts.GetWorldPrepend(result.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.LKIHuntNotReported, result.Name);
						}
						else if (timeSinceLastReport.TotalHours > 72.0)
						{
							ChatMessage chatMessage2 = cm;
							chatMessage2.MessageString += string.Format(CultureInfo.CurrentCulture, Resources.LKIHours, TotalHours);
						}
						else if (timeSinceLastReport.TotalHours < 1.0)
						{
							ChatMessage chatMessage3 = cm;
							chatMessage3.MessageString += string.Format(CultureInfo.CurrentCulture, Resources.LKIMinutes, Math.Floor(timeSinceLastReport.TotalMinutes));
						}
						else
						{
							ChatMessage chatMessage4 = cm;
							chatMessage4.MessageString += string.Format(CultureInfo.CurrentCulture, Resources.LKIHoursMinutes, TotalHours, timeSinceLastReport.Minutes);
						}
					}
					else
					{
						ushort zid = FFXIVHunts.GetZoneId(result.Id);
						cm = ChatMessage.MakePosChatMessage(FFXIVHunts.GetWorldPrepend(result.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.LKILastSeenAt, result.Name), zid, result.LastX, result.LastY, string.Format(CultureInfo.CurrentCulture, Resources.LKIHoursMinutes, TotalHours, timeSinceLastReport.Minutes), 0);
					}
					if (result.Instance > 0)
					{
						cm.PostpendToMessage(string.Format(CultureInfo.CurrentCulture, " " + Resources.InstanceSpecifier, result.Instance));
					}
					await Program.mem.WriteChatMessage(cm).ConfigureAwait(false);
				}
				IEnumerator<Hunt> enumerator = null;
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00007540 File Offset: 0x00005740
		internal async Task LastKnownInfoForFATE(ushort id)
		{
			if (FFXIVHunts.hubConnection.Connected && FFXIVHunts.Joined)
			{
				FATEReport[] results = await FFXIVHunts.hubConnection.Connection.InvokeAsync("QueryFATE", id, default(CancellationToken)).ConfigureAwait(false);
				foreach (FATEReport result in results)
				{
					TimeSpan timeSinceLastReport = FFXIVHunts.ServerTimeUtc.Subtract(result.LastReported);
					if (timeSinceLastReport < TimeSpan.Zero)
					{
						timeSinceLastReport = TimeSpan.Zero;
					}
					ChatMessage cm = new ChatMessage();
					if (timeSinceLastReport.TotalDays > 90.0)
					{
						cm.MessageString = FFXIVHunts.GetWorldPrepend(result.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.LKIHuntNotReported, result.Name(false));
					}
					else if (timeSinceLastReport.TotalHours > 100.0)
					{
						cm.MessageString = FFXIVHunts.GetWorldPrepend(result.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.LKIFATEDays, result.Name(false), Convert.ToUInt32(timeSinceLastReport.TotalDays));
					}
					else
					{
						cm = ChatMessage.MakePosChatMessage(FFXIVHunts.GetWorldPrepend(result.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.LKIFATE, result.Name(false), Math.Floor(timeSinceLastReport.TotalHours), timeSinceLastReport.Minutes), result.ZoneID, result.PosX, result.PosY, "", 0);
					}
					if (result.Instance > 0)
					{
						cm.PostpendToMessage(string.Format(CultureInfo.CurrentCulture, " " + Resources.InstanceSpecifier, result.Instance));
					}
					await Program.mem.WriteChatMessage(cm).ConfigureAwait(false);
				}
				FATEReport[] array = null;
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00007588 File Offset: 0x00005788
		internal Task<Item> QueryItem(string itemsearch)
		{
			return FFXIVHunts.hubConnection.Connection.InvokeAsync("QueryItem", itemsearch, Thread.CurrentThread.CurrentUICulture.Name, default(CancellationToken));
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000075C4 File Offset: 0x000057C4
		internal async void Check(FFXIVMemory mem)
		{
			FFXIVHunts.<>c__DisplayClass44_0 CS$<>8__locals1 = new FFXIVHunts.<>c__DisplayClass44_0();
			if (!FFXIVHunts.hubConnection.Connected)
			{
				await this.Connect();
			}
			if (FFXIVHunts.hubConnection.Connected)
			{
				ushort currentWorldID = mem.GetCurrentWorldId();
				if ((FFXIVHunts.LastJoinedWorldID != currentWorldID && FFXIVHunts.Joined && !FFXIVHunts.Joining) || !FFXIVHunts.Joined)
				{
					await this.LeaveGroup();
					await this.JoinServerGroup();
				}
				FFXIVHunts.ServerTimeUtc = mem.GetServerUtcTime();
				CS$<>8__locals1.thisZone = mem.GetZoneId();
				if (CS$<>8__locals1.thisZone != FFXIVHunts.lastZone && Settings.Default.OncePerHunt && Settings.Default.ForgetOnZoneChange)
				{
					FFXIVHunts.HuntsPutInChat.Clear();
				}
				if (Array.IndexOf<uint>(FFXIVHunts.DCZones, (uint)CS$<>8__locals1.thisZone) > -1 && Array.IndexOf<uint>(FFXIVHunts.DCZones, (uint)FFXIVHunts.lastZone) == -1 && FFXIVHunts.Joined)
				{
					TaskAwaiter<DateTime> taskAwaiter = this.JoinDCZone(CS$<>8__locals1.thisZone).GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<DateTime> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<DateTime>);
					}
					FFXIVHunts.LastShoutChatSync = taskAwaiter.GetResult();
				}
				else if (Array.IndexOf<uint>(FFXIVHunts.DCZones, (uint)FFXIVHunts.lastZone) > -1 && Array.IndexOf<uint>(FFXIVHunts.DCZones, (uint)CS$<>8__locals1.thisZone) == -1)
				{
					await this.LeaveDCZone();
				}
				FFXIVHunts.lastZone = CS$<>8__locals1.thisZone;
				IEnumerable<Monster> source = mem.Combatants.OfType<Monster>();
				Func<Monster, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__3) == null)
				{
					FFXIVHunts.<>c__DisplayClass44_0 CS$<>8__locals2 = CS$<>8__locals1;
					Func<Monster, bool> func = (Monster c) => FFXIVHunts.CurrentWorld.Hunts.Exists((Hunt h) => h.Id == c.BNpcNameID && FFXIVHunts.GetZoneId(c.BNpcNameID) == CS$<>8__locals1.thisZone);
					CS$<>8__locals2.<>9__3 = func;
					predicate = func;
				}
				foreach (Monster c2 in source.Where(predicate))
				{
					this.ReportHunt(c2);
				}
				if (Array.IndexOf<uint>(FFXIVHunts.DCZones, (uint)CS$<>8__locals1.thisZone) > -1)
				{
					DateTime lastShoutChatSync = FFXIVHunts.LastShoutChatSync;
					await this.ReportDCShoutChat((from x in mem.ReadChatLogBackwards(1000, (ChatMessage x) => x.Channel == ChatChannel.Shout && !string.IsNullOrWhiteSpace(x.Sender.Name), (ChatMessage x) => x.Timestamp <= FFXIVHunts.LastShoutChatSync)
					orderby x.Timestamp descending
					select x).Take(10));
				}
				IEnumerable<FATE> fateList = mem.GetFateList();
				Func<FATE, bool> predicate2;
				if ((predicate2 = CS$<>8__locals1.<>9__5) == null)
				{
					FFXIVHunts.<>c__DisplayClass44_0 CS$<>8__locals3 = CS$<>8__locals1;
					Func<FATE, bool> func2 = (FATE f) => f.ZoneID == CS$<>8__locals1.thisZone;
					CS$<>8__locals3.<>9__5 = func2;
					predicate2 = func2;
				}
				foreach (FATE f2 in fateList.Where(predicate2))
				{
					this.ReportFate(f2);
					if (f2.IsDataCenterShared() && this.PutInChat(new FATEReport(f2)
					{
						WorldId = mem.GetHomeWorldId()
					}) && Settings.Default.FlashTaskbarIconOnHuntAndFATEs)
					{
						NativeMethods.FlashTaskbarIcon(mem.Process);
					}
				}
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00007608 File Offset: 0x00005808
		private async Task LeaveDCZone()
		{
			try
			{
				if (FFXIVHunts.hubConnection != null && FFXIVHunts.hubConnection.Connected && FFXIVHunts.Joined)
				{
					await FFXIVHunts.hubConnection.Connection.InvokeAsync("LeaveDCZone", default(CancellationToken)).ConfigureAwait(false);
				}
			}
			catch (Exception e)
			{
				LogHost.Default.WarnException("LeaveDCZone", e);
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00007648 File Offset: 0x00005848
		private async Task<DateTime> JoinDCZone(ushort zoneid)
		{
			try
			{
				if (FFXIVHunts.hubConnection.Connected && FFXIVHunts.Joined)
				{
					HubConnection connection = FFXIVHunts.hubConnection.Connection;
					string methodName = "JoinDCZone";
					object arg = zoneid;
					DataCenterInstanceMatchInfo dcinstance = FFXIVHunts.DCInstance;
					uint? num;
					if (dcinstance == null || dcinstance.ID <= 0u)
					{
						num = new uint?(0u);
					}
					else
					{
						DataCenterInstanceMatchInfo dcinstance2 = FFXIVHunts.DCInstance;
						num = ((dcinstance2 != null) ? new uint?(dcinstance2.ID) : null);
					}
					return await connection.InvokeAsync(methodName, arg, num, default(CancellationToken)).ConfigureAwait(false);
				}
			}
			catch (Exception e)
			{
				LogHost.Default.WarnException("JoinDCZone", e);
			}
			return DateTime.MaxValue;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00007690 File Offset: 0x00005890
		private async Task ReportDCShoutChat(IEnumerable<ChatMessage> recentShoutChat)
		{
			if (recentShoutChat.Any<ChatMessage>() && FFXIVHunts.hubConnection.Connected && FFXIVHunts.Joined)
			{
				try
				{
					await FFXIVHunts.hubConnection.Connection.InvokeAsync("ReportDCShoutChat", recentShoutChat, default(CancellationToken)).ConfigureAwait(false);
					FFXIVHunts.LastShoutChatSync = recentShoutChat.Max((ChatMessage x) => x.Timestamp);
				}
				catch (Exception e)
				{
					LogHost.Default.WarnException("ReportDCShoutChat", e);
				}
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000076D8 File Offset: 0x000058D8
		internal Task RandomPositionForBNpc(ushort bnpcid)
		{
			FFXIVHunts.<RandomPositionForBNpc>d__48 <RandomPositionForBNpc>d__;
			<RandomPositionForBNpc>d__.bnpcid = bnpcid;
			<RandomPositionForBNpc>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RandomPositionForBNpc>d__.<>1__state = -1;
			AsyncTaskMethodBuilder <>t__builder = <RandomPositionForBNpc>d__.<>t__builder;
			<>t__builder.Start<FFXIVHunts.<RandomPositionForBNpc>d__48>(ref <RandomPositionForBNpc>d__);
			return <RandomPositionForBNpc>d__.<>t__builder.Task;
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00007720 File Offset: 0x00005920
		private async Task ReportFate(FATE f)
		{
			int idx = FFXIVHunts.CurrentWorld.FATEs.FindIndex((FATEReport h) => h.ID == f.ID);
			if (idx >= 0 && (!(FFXIVHunts.CurrentWorld.FATEs[idx].LastReported > FFXIVHunts.ServerTimeUtc.AddSeconds(-5.0)) || (FFXIVHunts.CurrentWorld.FATEs[idx].Progress != 100 && f.Progress > 99)))
			{
				FATEReport FATEReport = FFXIVHunts.CurrentWorld.FATEs[idx];
				FATEReport.LastReported = FFXIVHunts.ServerTimeUtc;
				FATEReport.Progress = f.Progress;
				FATEReport.PosX = f.PosX;
				FATEReport.PosZ = f.PosZ;
				FATEReport.PosY = f.PosY;
				FATEReport.ZoneID = f.ZoneID;
				FATEReport.Duration = f.Duration;
				FATEReport.StartTimeEpoch = f.StartTimeEpoch;
				FATEReport.State = f.State;
				FATEReport.ZoneID = f.ZoneID;
				if (FATEReport.ID > 1425 || f.ZoneID == 156 || f.ZoneID == 152)
				{
					FATEReport.Instance = Program.mem.GetZoneInstance();
				}
				try
				{
					if (FFXIVHunts.hubConnection.Connected && FFXIVHunts.Joined)
					{
						await FFXIVHunts.hubConnection.Connection.InvokeAsync("ReportFate", FATEReport, default(CancellationToken)).ConfigureAwait(false);
					}
				}
				catch (Exception e)
				{
					LogHost.Default.WarnException("ReportFate", e);
				}
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00007765 File Offset: 0x00005965
		private static bool FateNotifyCheck(ushort id)
		{
			FATEInfo fateinfo = GameResources.GetFATEInfo(id);
			id = GameResources.GetFateId((fateinfo != null) ? fateinfo.Name : null, false);
			return Settings.Default.FATEs.Contains(id);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00007794 File Offset: 0x00005994
		private async Task JoinServerGroup()
		{
			if ((!FFXIVHunts.Joined || !FFXIVHunts.hubConnection.Connected) && !FFXIVHunts.Joining)
			{
				FFXIVHunts.Joining = true;
				this.w1.HuntConnectionTextBlock.Dispatcher.Invoke<string>(() => this.w1.HuntConnectionTextBlock.Text = Resources.FormReadingSID);
				ushort cwid = Program.mem.GetCurrentWorldId();
				ushort hwid = Program.mem.GetHomeWorldId();
				if (FFXIVHunts.Reporter != null)
				{
					FFXIVHunts.Reporter.Dispose();
				}
				if (!FFXIVHunts.Worlds.ContainsKey(cwid))
				{
					FFXIVHunts.CreateWorldsOnDC(GameResources.GetWorld(hwid).GetWorldsOnSameDataCenter());
				}
				FFXIVHunts.Reporter = new Reporter(hwid, cwid, Program.mem.GetSelfCombatant().Name, FFXIVHunts.Worlds[cwid].Hunts.AsReadOnly(), FFXIVHunts.hubConnection.Connection);
				LogHost.Default.Info("Joining " + GameResources.GetWorldName(cwid));
				JoinGroupResult result = await FFXIVHunts.hubConnection.Connection.InvokeAsync("JoinGroup", FFXIVHunts.Reporter, default(CancellationToken)).ConfigureAwait(false);
				if (result == JoinGroupResult.Denied)
				{
					this.w1.HuntConnectionTextBlock.Dispatcher.Invoke(delegate()
					{
						this.w1.HuntConnectionTextBlock.Inlines.Clear();
						foreach (string s in string.Format(CultureInfo.CurrentCulture, Resources.FormFailedToJoin, FFXIVHunts.Reporter.NameAndHomeWorld ?? "").SplitAndKeep("XIVHunt.net", StringComparison.Ordinal))
						{
							if (s.Equals("XIVHunt.net", StringComparison.Ordinal))
							{
								Hyperlink link = new Hyperlink(new Run("XIVHunt.net"))
								{
									NavigateUri = new Uri("https://xivhunt.net/Manage/VerifiedCharacters")
								};
								link.RequestNavigate += LogInForm.Link_RequestNavigate;
								this.w1.HuntConnectionTextBlock.Inlines.Add(link);
							}
							else
							{
								this.w1.HuntConnectionTextBlock.Inlines.Add(s);
							}
						}
					});
				}
				else if (result == JoinGroupResult.Locked)
				{
					this.w1.HuntConnectionTextBlock.Dispatcher.Invoke<string>(() => this.w1.HuntConnectionTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Resources.FormJoinLocked, Program.AssemblyName.Name));
				}
				FFXIVHunts.Joining = false;
				FFXIVHunts.Joined = true;
				FFXIVHunts.LastJoinedWorldID = cwid;
				ushort zid = Program.mem.GetZoneId();
				if (Array.IndexOf<uint>(FFXIVHunts.DCZones, (uint)zid) > -1)
				{
					await this.JoinDCZone(zid).ConfigureAwait(false);
				}
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x000077DC File Offset: 0x000059DC
		private bool PutInChat(Hunt hunt)
		{
			World w;
			if ((Settings.Default.NoAnnouncementsInContent && Program.mem.GetCurrentContentFinderCondition() > 0) || FFXIVHunts.CurrentWorld == null || !FFXIVHunts.Worlds.TryGetValue(hunt.WorldId, out w))
			{
				return false;
			}
			int idx = w.Hunts.IndexOf(hunt);
			if (idx < 0)
			{
				return false;
			}
			ChatMessage cm = new ChatMessage();
			if (Settings.Default.OncePerHunt ? (!FFXIVHunts.HuntsPutInChat.Contains(hunt.OccurrenceID)) : (w.Hunts[idx].lastPutInChat < Program.mem.GetServerUtcTime().AddMinutes((double)(-(double)Settings.Default.HuntInterval)) && hunt.LastAlive))
			{
				cm = ChatMessage.MakePosChatMessage(FFXIVHunts.GetWorldPrepend(hunt.WorldId) + string.Format(CultureInfo.CurrentCulture, Resources.HuntMsg, hunt.Rank.ToString(), hunt.Name) + ((hunt.Instance > 0) ? string.Format(CultureInfo.CurrentCulture, " " + Resources.InstanceSpecifier, hunt.Instance) : string.Empty), FFXIVHunts.GetZoneId(hunt.Id), hunt.LastX, hunt.LastY, "", 0);
				if (cm != null)
				{
					Program.mem.WriteChatMessage(cm);
					this.CheckAndPlaySound(hunt.Rank);
					w.Hunts[idx] = hunt;
					FFXIVHunts.HuntsPutInChat.Add(hunt.OccurrenceID);
					w.Hunts[idx].lastPutInChat = Program.mem.GetServerUtcTime();
					return true;
				}
			}
			else if (w.Hunts[idx].lastReportedDead < FFXIVHunts.ServerTimeUtc.AddSeconds(-12.0) && !hunt.LastAlive)
			{
				cm.MessageString = string.Format(CultureInfo.CurrentCulture, FFXIVHunts.GetWorldPrepend(hunt.WorldId) + Resources.HuntMsgKilled, hunt.Rank.ToString(), hunt.Name) + ((hunt.Instance > 0) ? string.Format(CultureInfo.CurrentCulture, " " + Resources.InstanceSpecifier, hunt.Instance) : string.Empty);
				if (cm != null)
				{
					Program.mem.WriteChatMessage(cm);
					w.Hunts[idx] = hunt;
					w.Hunts[idx].lastReportedDead = Program.mem.GetServerUtcTime();
					return true;
				}
			}
			return false;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00007A74 File Offset: 0x00005C74
		internal static ushort GetZoneId(ushort huntId)
		{
			foreach (KeyValuePair<ushort, HashSet<ushort>> i in FFXIVHunts.MapHunts)
			{
				if (i.Value.Contains(huntId))
				{
					return i.Key;
				}
			}
			return 0;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00007ADC File Offset: 0x00005CDC
		private void CheckAndPlaySound(HuntRank r)
		{
			try
			{
				AudioFileReader audioFile;
				if (this.w1.sounds.TryGetValue(r, out audioFile))
				{
					SoundPlayer.Play(audioFile);
				}
			}
			catch (Exception ex)
			{
				LogHost.Default.ErrorException("CheckAndPlaySound", ex);
			}
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00007B2C File Offset: 0x00005D2C
		private async Task ReportHunt(Monster c)
		{
			int idx = FFXIVHunts.CurrentWorld.Hunts.FindIndex((Hunt h) => h.Id == c.BNpcNameID);
			if (!(FFXIVHunts.CurrentWorld.Hunts[idx].LastReported > FFXIVHunts.ServerTimeUtc.AddSeconds(-5.0)) || c.CurrentHP <= 0u)
			{
				Hunt hunt = FFXIVHunts.CurrentWorld.Hunts[idx];
				hunt.LastReported = FFXIVHunts.ServerTimeUtc;
				hunt.LastX = c.PosX;
				hunt.LastY = c.PosY;
				hunt.OccurrenceID = c.ID;
				hunt.LastAlive = (c.CurrentHP > 0u);
				if (FFXIVHunts.GetZoneId(hunt.Id) == 156 || hunt.Id > 8000 || FFXIVHunts.GetZoneId(hunt.Id) == 152)
				{
					hunt.Instance = Program.mem.GetZoneInstance();
				}
				try
				{
					if (FFXIVHunts.Joined)
					{
						await FFXIVHunts.hubConnection.Connection.InvokeAsync("ReportHunt", hunt, default(CancellationToken)).ConfigureAwait(false);
					}
				}
				catch (Exception e)
				{
					LogHost.Default.WarnException("ReportHunt", e);
				}
			}
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00007B74 File Offset: 0x00005D74
		public void Dispose()
		{
			this.LeaveGroup();
			Reporter reporter = FFXIVHunts.Reporter;
			if (reporter != null)
			{
				reporter.Dispose();
			}
			FFXIVHunts.Reporter = null;
			try
			{
				FFXIVHunts.hubConnection.Connection.DisposeAsync();
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x040000FC RID: 252
		internal static readonly Dictionary<ushort, HashSet<ushort>> MapHunts = new Dictionary<ushort, HashSet<ushort>>
		{
			{
				134,
				new HashSet<ushort>
				{
					2928,
					2945,
					2962
				}
			},
			{
				135,
				new HashSet<ushort>
				{
					2929,
					2946,
					2963
				}
			},
			{
				137,
				new HashSet<ushort>
				{
					2930,
					2947,
					2964
				}
			},
			{
				138,
				new HashSet<ushort>
				{
					2931,
					2948,
					2965
				}
			},
			{
				139,
				new HashSet<ushort>
				{
					2932,
					2949,
					2966
				}
			},
			{
				140,
				new HashSet<ushort>
				{
					2923,
					2940,
					2957
				}
			},
			{
				141,
				new HashSet<ushort>
				{
					2924,
					2941,
					2958
				}
			},
			{
				145,
				new HashSet<ushort>
				{
					2925,
					2942,
					2959
				}
			},
			{
				146,
				new HashSet<ushort>
				{
					2926,
					2943,
					2960
				}
			},
			{
				147,
				new HashSet<ushort>
				{
					2927,
					2944,
					2961
				}
			},
			{
				148,
				new HashSet<ushort>
				{
					2919,
					2936,
					2953
				}
			},
			{
				152,
				new HashSet<ushort>
				{
					2920,
					2937,
					2954
				}
			},
			{
				153,
				new HashSet<ushort>
				{
					2921,
					2938,
					2955
				}
			},
			{
				154,
				new HashSet<ushort>
				{
					2922,
					2939,
					2956
				}
			},
			{
				155,
				new HashSet<ushort>
				{
					2934,
					2951,
					2968
				}
			},
			{
				156,
				new HashSet<ushort>
				{
					2935,
					2952,
					2969
				}
			},
			{
				180,
				new HashSet<ushort>
				{
					2933,
					2950,
					2967
				}
			},
			{
				397,
				new HashSet<ushort>
				{
					4350,
					4351,
					4362,
					4363,
					4374
				}
			},
			{
				398,
				new HashSet<ushort>
				{
					4352,
					4353,
					4364,
					4365,
					4375
				}
			},
			{
				399,
				new HashSet<ushort>
				{
					4354,
					4355,
					4366,
					4367,
					4376
				}
			},
			{
				400,
				new HashSet<ushort>
				{
					4356,
					4357,
					4368,
					4369,
					4377
				}
			},
			{
				401,
				new HashSet<ushort>
				{
					4358,
					4359,
					4370,
					4371,
					4378
				}
			},
			{
				402,
				new HashSet<ushort>
				{
					4360,
					4361,
					4372,
					4373,
					4380
				}
			},
			{
				612,
				new HashSet<ushort>
				{
					6008,
					6009,
					5990,
					5991,
					5987
				}
			},
			{
				620,
				new HashSet<ushort>
				{
					6010,
					6011,
					5992,
					5993,
					5988
				}
			},
			{
				621,
				new HashSet<ushort>
				{
					6012,
					6013,
					5994,
					5995,
					5989
				}
			},
			{
				613,
				new HashSet<ushort>
				{
					6002,
					6003,
					5996,
					5997,
					5984
				}
			},
			{
				614,
				new HashSet<ushort>
				{
					6004,
					6005,
					5998,
					5999,
					5985
				}
			},
			{
				622,
				new HashSet<ushort>
				{
					6006,
					6007,
					6000,
					6001,
					5986
				}
			},
			{
				813,
				new HashSet<ushort>
				{
					8905,
					8906,
					8907,
					8908,
					8909
				}
			},
			{
				814,
				new HashSet<ushort>
				{
					8910,
					8911,
					8912,
					8913,
					8914
				}
			},
			{
				815,
				new HashSet<ushort>
				{
					8900,
					8901,
					8902,
					8903,
					8904
				}
			},
			{
				816,
				new HashSet<ushort>
				{
					8653,
					8654,
					8655,
					8656,
					8657
				}
			},
			{
				817,
				new HashSet<ushort>
				{
					8890,
					8891,
					8892,
					8893,
					8894
				}
			},
			{
				818,
				new HashSet<ushort>
				{
					8895,
					8896,
					8897,
					8898,
					8899
				}
			}
		};

		// Token: 0x040000FD RID: 253
		internal static Dictionary<ushort, World> Worlds = new Dictionary<ushort, World>();

		// Token: 0x040000FE RID: 254
		private static readonly HashSet<uint> HuntsPutInChat = new HashSet<uint>();

		// Token: 0x040000FF RID: 255
		private static readonly uint[] DCZones = new uint[]
		{
			630u,
			656u,
			732u,
			763u,
			795u,
			827u
		};

		// Token: 0x04000100 RID: 256
		private static HuntsHubConnection hubConnection;

		// Token: 0x04000104 RID: 260
		private static volatile ushort LastJoinedWorldID = 0;

		// Token: 0x04000105 RID: 261
		private static volatile ushort lastZone;

		// Token: 0x04000106 RID: 262
		internal const string baseUrl = "https://xivhunt.net/";

		// Token: 0x04000107 RID: 263
		internal const string VerifiedCharactersUrl = "https://xivhunt.net/Manage/VerifiedCharacters";

		// Token: 0x04000108 RID: 264
		private static DateTime ServerTimeUtc;

		// Token: 0x04000109 RID: 265
		private static DateTime LastShoutChatSync;

		// Token: 0x0400010A RID: 266
		private static DataCenterInstanceMatchInfo DCInstance;

		// Token: 0x0400010B RID: 267
		private readonly Window1 w1;

		// Token: 0x0400010C RID: 268
		private static Reporter Reporter;
	}
}

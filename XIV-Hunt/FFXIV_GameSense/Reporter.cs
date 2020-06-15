using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FFXIV_GameSense.Properties;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Splat;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x0200004A RID: 74
	internal class Reporter
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0000AE44 File Offset: 0x00009044
		// (set) Token: 0x0600020A RID: 522 RVA: 0x0000AE4C File Offset: 0x0000904C
		public ushort HomeWorldID { get; private set; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000AE55 File Offset: 0x00009055
		// (set) Token: 0x0600020C RID: 524 RVA: 0x0000AE5D File Offset: 0x0000905D
		public ushort CurrentWorldID { get; private set; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x0600020D RID: 525 RVA: 0x0000AE66 File Offset: 0x00009066
		// (set) Token: 0x0600020E RID: 526 RVA: 0x0000AE6E File Offset: 0x0000906E
		public string Name { get; private set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000AE77 File Offset: 0x00009077
		// (set) Token: 0x06000210 RID: 528 RVA: 0x0000AE7F File Offset: 0x0000907F
		public Version Version { get; private set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000AE88 File Offset: 0x00009088
		[JsonIgnore]
		public string NameAndHomeWorld
		{
			get
			{
				return this.Name + " (" + GameResources.GetWorldName(this.HomeWorldID) + ")";
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000AEAA File Offset: 0x000090AA
		public ObservableHashSet<ushort> SubscribedHunts
		{
			get
			{
				return Settings.Default.Hunts;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000AEB6 File Offset: 0x000090B6
		public ObservableHashSet<ushort> SubscribedFATEs
		{
			get
			{
				return Settings.Default.FATEs;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000AEC2 File Offset: 0x000090C2
		public bool SubscribedToOtherWorlds
		{
			get
			{
				return Settings.Default.NotificationsFromOtherWorlds;
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000AED0 File Offset: 0x000090D0
		public Reporter(ushort hwid, ushort cwid, string name, IReadOnlyList<Hunt> hunts, HubConnection connection)
		{
			this.HomeWorldID = hwid;
			this.CurrentWorldID = cwid;
			this.Name = name;
			this.Version = Program.AssemblyName.Version;
			this.Hunts = hunts;
			this.hubConnection = connection;
			this.RefreshSubscribedHuntIDs();
			Settings.Default.PropertyChanged += this.ReporterSettingsPropertyChanged;
			this.SubscribedHunts.CollectionChanged += this.SubscribedHunt_CollectionChanged;
			this.SubscribedFATEs.CollectionChanged += this.SubscribedFATEs_CollectionChanged;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000AF64 File Offset: 0x00009164
		private void ReporterSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!(e.PropertyName == "NotificationsFromOtherWorlds"))
			{
				if (!e.PropertyName.StartsWith("notify", StringComparison.OrdinalIgnoreCase) || e.PropertyName.Length != 7)
				{
					if (!e.PropertyName.All((char x) => char.IsUpper(x)) || e.PropertyName.Length >= 5)
					{
						return;
					}
				}
				this.RefreshSubscribedHuntIDs();
				return;
			}
			this.hubConnection.SendAsync("SetSubscribedToOtherWorlds", Settings.Default.NotificationsFromOtherWorlds, default(CancellationToken));
			if (Settings.Default.NotificationsFromOtherWorlds)
			{
				LogHost.Default.Info("[Reporter] Subscribed to other worlds on this datacenter");
				return;
			}
			LogHost.Default.Info("[Reporter] Unsubscribed from other worlds");
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000B03C File Offset: 0x0000923C
		private void SubscribedHunt_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				HubConnection hubConnection = this.hubConnection;
				string methodName = "SetSubscription";
				string t = "Hunt";
				NotifyCollectionChangedAction action = e.Action;
				IList newItems = e.NewItems;
				hubConnection.SendAsync(methodName, new SubscriptionUpdate(t, action, (newItems != null && newItems.Count > 0) ? e.NewItems : e.OldItems), default(CancellationToken));
			}
			catch (Exception ex)
			{
				LogHost.Default.WarnException("Could not syncronize settings", ex);
			}
			this.LogInfo("SubscribedHunt_CollectionChanged", e);
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000B0CC File Offset: 0x000092CC
		private void SubscribedFATEs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			try
			{
				HubConnection hubConnection = this.hubConnection;
				string methodName = "SetSubscription";
				string t = "FATEReport";
				NotifyCollectionChangedAction action = e.Action;
				IList newItems = e.NewItems;
				hubConnection.SendAsync(methodName, new SubscriptionUpdate(t, action, (newItems != null && newItems.Count > 0) ? e.NewItems : e.OldItems), default(CancellationToken));
			}
			catch (Exception ex)
			{
				LogHost.Default.WarnException("Could not syncronize settings", ex);
			}
			this.LogInfo("SubscribedFATEs_CollectionChanged", e);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000B15C File Offset: 0x0000935C
		private void LogInfo(string methodName, NotifyCollectionChangedEventArgs e)
		{
			string info = methodName + ":" + e.Action.ToString() + Environment.NewLine;
			foreach (object i in (e.NewItems ?? new List<object>()))
			{
				info = info + "+" + ((i != null) ? i.ToString() : null) + (((e.NewItems.IndexOf(i) + 1) % 3 == 0) ? Environment.NewLine : " ");
			}
			foreach (object j in (e.OldItems ?? new List<object>()))
			{
				info = info + "-" + ((j != null) ? j.ToString() : null) + (((e.OldItems.IndexOf(j) + 1) % 3 == 0) ? Environment.NewLine : " ");
			}
			LogHost.Default.Info(info);
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000B2A0 File Offset: 0x000094A0
		private void RefreshSubscribedHuntIDs()
		{
			HashSet<ushort> newSubscribedHuntIDsHS = new HashSet<ushort>();
			if (Settings.Default.notifyS)
			{
				newSubscribedHuntIDsHS.AddRange(from x in this.Hunts
				where x.Rank == HuntRank.S && ((x.IsARR && Settings.Default.SARR) || (x.IsHW && Settings.Default.SHW) || (x.IsSB && Settings.Default.SSB) || (x.IsSHB && Settings.Default.SSHB))
				select x.Id);
			}
			if (Settings.Default.notifyA)
			{
				newSubscribedHuntIDsHS.AddRange(from x in this.Hunts
				where x.Rank == HuntRank.A && ((x.IsARR && Settings.Default.AARR) || (x.IsHW && Settings.Default.AHW) || (x.IsSB && Settings.Default.ASB) || (x.IsSHB && Settings.Default.ASHB))
				select x.Id);
			}
			if (Settings.Default.notifyB)
			{
				newSubscribedHuntIDsHS.AddRange(from x in this.Hunts
				where x.Rank == HuntRank.B && ((x.IsARR && Settings.Default.BARR) || (x.IsHW && Settings.Default.BHW) || (x.IsSB && Settings.Default.BSB) || (x.IsSHB && Settings.Default.BSHB))
				select x.Id);
			}
			this.SubscribedHunts.RemoveRange(this.SubscribedHunts.Except(newSubscribedHuntIDsHS));
			this.SubscribedHunts.AddRange(newSubscribedHuntIDsHS);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00002E6C File Offset: 0x0000106C
		protected virtual void Dispose(bool disposing)
		{
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000B3FC File Offset: 0x000095FC
		public void Dispose()
		{
			Settings.Default.PropertyChanged -= this.ReporterSettingsPropertyChanged;
			this.SubscribedHunts.CollectionChanged -= this.SubscribedHunt_CollectionChanged;
			this.SubscribedFATEs.CollectionChanged -= this.SubscribedFATEs_CollectionChanged;
			this.hubConnection = null;
			this.Hunts = null;
			this.Dispose(true);
		}

		// Token: 0x0400016E RID: 366
		[JsonIgnore]
		private IReadOnlyList<Hunt> Hunts;

		// Token: 0x0400016F RID: 367
		[JsonIgnore]
		private HubConnection hubConnection;
	}
}

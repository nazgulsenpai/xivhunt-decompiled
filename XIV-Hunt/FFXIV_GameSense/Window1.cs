using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Threading;
using FFXIV_GameSense.IPC;
using FFXIV_GameSense.MML;
using FFXIV_GameSense.Properties;
using FFXIV_GameSense.UI;
using FFXIV_GameSense.UI.Controls;
using Microsoft.Win32;
using NAudio.Wave;
using Splat;
using XIVDB;

namespace FFXIV_GameSense
{
	public class Window1 : Window, IDisposable, IComponentConnector
	{
		public Window1()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.LanguageCI);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.LanguageCI);
			this.InitializeComponent();
			Logger logger = new Logger(Window1.LogView);
			Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger), null);
			base.Title = string.Concat(new string[]
			{
				Program.AssemblyName.Name,
				" ",
				Program.AssemblyName.Version.ToString(3),
				" - ",
				(Environment.Is64BitProcess ? 64 : 32).ToString(),
				"-Bit"
			});
			this.vm = new ViewModel();
			base.Closed += this.MenuForm_FormClosed;
			this.dispatcherTimer1s = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1.0)
			};
			this.dispatcherTimer1s.Tick += this.DispatcherTimer1s_Tick;
			this.dispatcherTimer1s.Start();
			this.CheckSoundStartup();
			this.trayIcon = new NotifyIcon
			{
				Icon = FFXIV_GameSense.Properties.Resources.enemy,
				Visible = false,
				Text = base.Title
			};
			this.trayIcon.Click += delegate(object sender, EventArgs args)
			{
				base.Show();
				base.Visibility = Visibility.Visible;
				base.WindowState = WindowState.Normal;
				this.trayIcon.Visible = false;
			};
			base.DataContext = this.vm;
			if (Settings.Default.StartMinimized)
			{
				base.WindowState = WindowState.Minimized;
				if (Settings.Default.MinimizeToTray)
				{
					this.HideWindowAndShowTrayIcon();
					base.OnStateChanged(EventArgs.Empty);
				}
			}
		}

		protected override void OnStateChanged(EventArgs e)
		{
			this.HideWindowAndShowTrayIcon();
			base.OnStateChanged(e);
		}

		private void HideWindowAndShowTrayIcon()
		{
			if (base.WindowState == WindowState.Minimized && Settings.Default.MinimizeToTray)
			{
				this.trayIcon.Visible = true;
				base.Visibility = Visibility.Hidden;
				base.Hide();
			}
		}

		private void CheckSoundStartup()
		{
			string[] slist = new string[]
			{
				Settings.Default.SBell,
				Settings.Default.ABell,
				Settings.Default.BBell,
				Settings.Default.FATEBell
			};
			for (int i = 0; i < slist.Length; i++)
			{
				if (!string.IsNullOrEmpty(slist[i]) && !slist[i].Equals(FFXIV_GameSense.Properties.Resources.NoSoundAlert, StringComparison.OrdinalIgnoreCase) && File.Exists(slist[i]))
				{
					switch (i)
					{
					case 0:
						this.SetAlarmSound(this.SBell, slist[0]);
						break;
					case 1:
						this.SetAlarmSound(this.ABell, slist[1]);
						break;
					case 2:
						this.SetAlarmSound(this.BBell, slist[2]);
						break;
					case 3:
						this.SetAlarmSound(this.FATEBell, slist[3]);
						break;
					}
				}
				else
				{
					switch (i)
					{
					case 0:
						this.UnsetAlarmSound(this.SBell);
						break;
					case 1:
						this.UnsetAlarmSound(this.ABell);
						break;
					case 2:
						this.UnsetAlarmSound(this.BBell);
						break;
					case 3:
						this.UnsetAlarmSound(this.FATEBell);
						break;
					}
				}
			}
		}

		private void DispatcherTimer1s_Tick(object sender, EventArgs e)
		{
			if (this.ProcessComboBox.SelectedIndex < 0)
			{
				this.ProcessComboBox.SelectedIndex = 0;
			}
			int? psv = (int?)this.ProcessComboBox.SelectedValue;
			this.vm.Refresh();
			int? num = psv;
			int num2 = 0;
			if (num.GetValueOrDefault() > num2 & num != null)
			{
				this.ProcessComboBox.SelectedValue = psv;
			}
			if (this.ProcessComboBox.Items.Count > 1)
			{
				this.ProcessComboBox.IsEnabled = true;
			}
			else
			{
				this.ProcessComboBox.IsEnabled = false;
			}
			try
			{
				if (!this.AnyProblems())
				{
					this.HuntAndCFCheck();
					this.dispatcherTimer1s.Interval = TimeSpan.FromSeconds(1.0);
				}
			}
			catch (Exception ex)
			{
				if (ex is MemoryScanException && this.dispatcherTimer1s.Interval.TotalSeconds < 5.0)
				{
					this.dispatcherTimer1s.Interval = TimeSpan.FromSeconds(this.dispatcherTimer1s.Interval.TotalSeconds + 1.0);
				}
				if (ex is MemoryScanException)
				{
					LogHost.Default.InfoException("MemoryScanException", ex);
				}
				else if (ex is Win32Exception)
				{
					LogHost.Default.ErrorException("Could not read process memory. Possibly lacking privileges.", ex);
					this.HuntConnectionTextBlock.Text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.FormFailAttachNoAdmin, Program.AssemblyName.Name);
				}
				else
				{
					LogHost.Default.WarnException("Unknown exception", ex);
				}
			}
		}

		private bool AnyProblems()
		{
			if (Program.mem != null && Program.mem.ValidateProcess() && Program.mem.GetSelfCombatant() != null)
			{
				int id = Program.mem.Process.Id;
				int? num = (int?)this.ProcessComboBox.SelectedValue;
				if (id == num.GetValueOrDefault() & num != null)
				{
					if (this.hunts == null && Program.mem != null && Program.mem.ValidateProcess())
					{
						this.hunts = new FFXIVHunts(this);
					}
					this.MainTabControl.IsEnabled = (this.HuntNotifyGroupBox.IsEnabled = true);
					return false;
				}
			}
			this.HuntConnectionTextBlock.Text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.FormNoProcess, "ffxiv_dx11.exe");
			if (this.ProcessComboBox.SelectedValue != null && FFXIVProcessHelper.GetFFXIVProcess((int)this.ProcessComboBox.SelectedValue) != null)
			{
				if (Program.mem != null)
				{
					Program.mem.OnNewCommand -= this.ProcessChatCommand;
					Program.mem.Dispose();
					FFXIVHunts ffxivhunts = this.hunts;
					if (ffxivhunts != null)
					{
						ffxivhunts.LeaveGroup();
					}
				}
				Program.mem = null;
				Program.mem = new FFXIVMemory(FFXIVProcessHelper.GetFFXIVProcess((int)this.ProcessComboBox.SelectedValue));
				Program.mem.OnNewCommand += this.ProcessChatCommand;
				PersistentNamedPipeServer.Restart();
			}
			FFXIVHunts ffxivhunts2 = this.hunts;
			if (ffxivhunts2 != null)
			{
				ffxivhunts2.LeaveGroup();
			}
			this.MainTabControl.IsEnabled = (this.HuntNotifyGroupBox.IsEnabled = false);
			return true;
		}

		internal void ProcessChatCommand(object sender, CommandEventArgs e)
		{
			Window1.<>c__DisplayClass16_0 CS$<>8__locals1 = new Window1.<>c__DisplayClass16_0();
			CS$<>8__locals1.e = e;
			CS$<>8__locals1.<>4__this = this;
			IFullLogger @default = LogHost.Default;
			string str = "[ProcessChatCommand] New command: ";
			CommandEventArgs e2 = CS$<>8__locals1.e;
			@default.Info(str + ((e2 != null) ? e2.ToString() : null));
			CS$<>8__locals1.mem = ((sender is FFXIVMemory) ? (sender as FFXIVMemory) : Program.mem);
			if (CS$<>8__locals1.mem == null)
			{
				return;
			}
			if (CS$<>8__locals1.e.Command == Command.Hunt && this.hunts != null)
			{
				Window1.<>c__DisplayClass16_1 CS$<>8__locals2 = new Window1.<>c__DisplayClass16_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				Tuple<ushort, ushort, float, float> hi;
				if (GameResources.TryGetDailyHuntInfo(CS$<>8__locals2.CS$<>8__locals1.e.Parameter, out hi))
				{
					CS$<>8__locals2.CS$<>8__locals1.mem.WriteChatMessage(ChatMessage.MakePosChatMessage(string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.LKICanBeFoundAt, GameResources.GetEnemyName((uint)hi.Item1, true)), hi.Item2, hi.Item3, hi.Item4, "", 0));
					return;
				}
				if (FFXIVHunts.Worlds.First<KeyValuePair<ushort, World>>().Value.Hunts.Exists((Hunt x) => x.Name.Equals(CS$<>8__locals2.CS$<>8__locals1.e.Parameter, StringComparison.OrdinalIgnoreCase)))
				{
					this.hunts.QueryHunt(FFXIVHunts.Worlds.First<KeyValuePair<ushort, World>>().Value.Hunts.First((Hunt x) => x.Name.Equals(CS$<>8__locals2.CS$<>8__locals1.e.Parameter, StringComparison.OrdinalIgnoreCase)).Id);
					return;
				}
				ushort bnpcid;
				if (GameResources.GetEnemyId(CS$<>8__locals2.CS$<>8__locals1.e.Parameter, out bnpcid))
				{
					this.hunts.RandomPositionForBNpc(bnpcid);
					return;
				}
				CS$<>8__locals2.fid = GameResources.GetFateId(CS$<>8__locals2.CS$<>8__locals1.e.Parameter, true);
				if (CS$<>8__locals2.fid > 0)
				{
					this.hunts.LastKnownInfoForFATE(CS$<>8__locals2.fid);
					if (Settings.Default.TrackFATEAfterQuery)
					{
						this.vm.FATEEntries.SingleOrDefault((FATEListViewItem x) => x.ID == CS$<>8__locals2.fid).Announce = true;
						return;
					}
				}
				else
				{
					Window1.<>c__DisplayClass16_2 CS$<>8__locals3 = new Window1.<>c__DisplayClass16_2();
					CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
					if (Enum.TryParse<HuntRank>(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.e.Parameter.Split(' ', StringSplitOptions.None).Last<string>(), out CS$<>8__locals3.hr) && CS$<>8__locals3.hr != HuntRank.FATE)
					{
						string parameter = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.e.Parameter;
						int length = parameter.Length;
						int num = 0;
						int length2 = length - 2 - num;
						ushort ZoneID;
						if (GameResources.TryGetZoneID(parameter.Substring(num, length2).Trim(), out ZoneID) && FFXIVHunts.MapHunts.ContainsKey(ZoneID))
						{
							IEnumerable<ushort> source = FFXIVHunts.MapHunts[ZoneID];
							Func<ushort, bool> predicate;
							if ((predicate = CS$<>8__locals3.<>9__3) == null)
							{
								predicate = (CS$<>8__locals3.<>9__3 = ((ushort x) => CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.<>4__this.hunts.HuntRankFor(x) == CS$<>8__locals3.hr));
							}
							using (IEnumerator<ushort> enumerator = source.Where(predicate).GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									ushort hid = enumerator.Current;
									this.hunts.QueryHunt(hid);
								}
								return;
							}
						}
					}
					if (!string.IsNullOrWhiteSpace(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.e.Parameter))
					{
						Window1.<>c__DisplayClass16_3 CS$<>8__locals4 = new Window1.<>c__DisplayClass16_3();
						CS$<>8__locals4.CS$<>8__locals3 = CS$<>8__locals3;
						CS$<>8__locals4.pwords = CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.e.Parameter.Split(' ', StringSplitOptions.None);
						CS$<>8__locals4.hqprefer = CS$<>8__locals4.pwords.Last<string>().Equals("HQ", StringComparison.OrdinalIgnoreCase);
						Task.Run(delegate()
						{
							Window1.<>c__DisplayClass16_3.<<ProcessChatCommand>b__4>d <<ProcessChatCommand>b__4>d;
							<<ProcessChatCommand>b__4>d.<>4__this = CS$<>8__locals4;
							<<ProcessChatCommand>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
							<<ProcessChatCommand>b__4>d.<>1__state = -1;
							AsyncTaskMethodBuilder <>t__builder = <<ProcessChatCommand>b__4>d.<>t__builder;
							<>t__builder.Start<Window1.<>c__DisplayClass16_3.<<ProcessChatCommand>b__4>d>(ref <<ProcessChatCommand>b__4>d);
							return <<ProcessChatCommand>b__4>d.<>t__builder.Task;
						});
						return;
					}
				}
			}
			else if (CS$<>8__locals1.e.Command == Command.Perform)
			{
				if (!Directory.Exists(Settings.Default.PerformDirectory))
				{
					LogHost.Default.Error(FFXIV_GameSense.Properties.Resources.PerformDirectoryNotExists);
					CS$<>8__locals1.mem.WriteChatMessage(new ChatMessage(FFXIV_GameSense.Properties.Resources.PerformDirectoryNotExists));
					return;
				}
				string nametxt = CS$<>8__locals1.e.Parameter;
				if (!nametxt.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
				{
					nametxt += ".txt";
				}
				string namemml = CS$<>8__locals1.e.Parameter;
				if (!namemml.EndsWith(".mml", StringComparison.OrdinalIgnoreCase))
				{
					namemml += ".mml";
				}
				string pathnametxt = Path.Combine(Settings.Default.PerformDirectory, nametxt);
				string pathnamemml = Path.Combine(Settings.Default.PerformDirectory, namemml);
				if (File.Exists(pathnametxt))
				{
					this.StopPerformance();
					Performance p = new Performance(string.Join(",", File.ReadAllLines(pathnametxt)));
					if (p.Sheet.Count > 0)
					{
						CS$<>8__locals1.mem.PlayPerformance(p, this.cts.Token);
						return;
					}
					this.TryMML(pathnametxt);
					return;
				}
				else
				{
					if (File.Exists(pathnamemml))
					{
						this.StopPerformance();
						this.TryMML(pathnamemml);
						return;
					}
					LogHost.Default.Error(string.Concat(new string[]
					{
						"Neither of these files were found:",
						Environment.NewLine,
						pathnametxt,
						Environment.NewLine,
						pathnamemml
					}));
					return;
				}
			}
			else
			{
				if (CS$<>8__locals1.e.Command == Command.PerformStop && this.cts != null)
				{
					this.cts.Cancel();
					return;
				}
				if (CS$<>8__locals1.e.Command == Command.Flag)
				{
					string optionalZone = string.Concat<char>(CS$<>8__locals1.e.Parameter.TakeWhile(new Func<char, bool>(Window1.<ProcessChatCommand>g__optionalZonePartPredicate|16_5))).Trim();
					ushort mid = 0;
					string[] coords = string.Concat<char>(CS$<>8__locals1.e.Parameter.SkipWhile(new Func<char, bool>(Window1.<ProcessChatCommand>g__optionalZonePartPredicate|16_5))).Split(',', StringSplitOptions.None);
					float xR;
					float yR;
					if (coords.Length > 1 && float.TryParse(coords[0], out xR) && float.TryParse(coords[1], out yR))
					{
						ushort zid;
						if (!GameResources.TryGetZoneID(optionalZone, out zid))
						{
							zid = Program.mem.GetZoneId();
							mid = Program.mem.GetMapId();
						}
						float x2 = Entity.GetCoordFromReadable(xR, zid, mid);
						float y = Entity.GetCoordFromReadable(yR, zid, mid);
						ChatMessage cm = ChatMessage.MakePosChatMessage(string.Empty, zid, x2, y, string.Empty, mid);
						CS$<>8__locals1.mem.WriteChatMessage(cm);
					}
				}
			}
		}

		private void HuntAndCFCheck()
		{
			if (this.hunts != null)
			{
				this.hunts.Check(Program.mem);
				ContentFinder cf = Program.mem.GetContentFinder();
				if (Settings.Default.FlashTaskbarIconOnDFPop && cf.State == ContentFinderState.Popped && !Window1.IconIsFlashing)
				{
					NativeMethods.FlashTaskbarIcon(Program.mem.Process, 45u, false);
					Window1.IconIsFlashing = true;
				}
				if ((!Settings.Default.notifyDutyRoulette || cf.State != ContentFinderState.Popped || !cf.IsDutyRouletteQueued()) && cf.State != ContentFinderState.Popped)
				{
					if (Window1.IconIsFlashing)
					{
						NativeMethods.StopFlashWindowEx(Program.mem.Process);
					}
					Window1.IconIsFlashing = false;
				}
			}
		}

		private void StopPerformance()
		{
			if (this.cts != null && !this.cts.IsCancellationRequested)
			{
				this.cts.Cancel();
			}
			this.cts = new CancellationTokenSource();
		}

		private void TryMML(string pathname)
		{
			ImplementedPlayer mml = new ImplementedPlayer();
			string[] mmls = File.ReadAllLines(pathname);
			for (int i = 0; i < mmls.Length; i++)
			{
				mmls[i] = mmls[i].RemoveLineComments();
			}
			string fmml = string.Join(string.Empty, mmls).RemoveBlockComments();
			mml.Load(fmml);
			Program.mem.PlayMML(mml, this.cts.Token);
		}

		private void MenuForm_FormClosed(object sender, EventArgs e)
		{
			this.OverlayView.Dispose();
			this.Dispose();
			Environment.Exit(0);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dispatcherTimer1s.Stop();
				foreach (KeyValuePair<HuntRank, AudioFileReader> s in this.sounds)
				{
					if (s.Value != null)
					{
						s.Value.Dispose();
					}
				}
				if (this.hunts != null)
				{
					this.hunts.Dispose();
				}
				this.sounds.Clear();
				if (this.cts != null)
				{
					this.cts.Dispose();
				}
				this.cts = null;
				this.trayIcon.Dispose();
				if (this.hunts != null)
				{
					FFXIVHunts.Http.Dispose();
					this.hunts.Dispose();
				}
				if (Program.mem != null)
				{
					Program.mem.Dispose();
				}
				if (PersistentNamedPipeServer.Instance.IsConnected)
				{
					PersistentNamedPipeServer.Instance.Disconnect();
				}
				PersistentNamedPipeServer.Instance.Dispose();
				Settings.Default.Save();
			}
		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
			{
				Title = FFXIV_GameSense.Properties.Resources.FormSFDialogTitle,
				Filter = FFXIV_GameSense.Properties.Resources.FormSFDialogFilter + "|*.mp3;*.wav;*.wma;*.aiff;*.aac",
				CheckFileExists = true,
				CheckPathExists = true
			};
			bool? flag = ofd.ShowDialog();
			bool flag2 = true;
			if (flag.GetValueOrDefault() == flag2 & flag != null)
			{
				if (!this.SetAlarmSound((AlarmButton)sender, ofd.FileName))
				{
					((AlarmButton)sender).SetOff();
					return;
				}
			}
			else
			{
				((AlarmButton)sender).SetOff();
			}
		}

		private bool SetAlarmSound(AlarmButton r, string soundFile)
		{
			r.ToolTip = Path.GetFileName(soundFile);
			r.SetOn();
			string name = r.Name;
			if (name != null)
			{
				if (name == "SBell")
				{
					this.sounds[HuntRank.S] = new AudioFileReader(soundFile);
					Settings.Default.SBell = soundFile;
					return true;
				}
				if (name == "ABell")
				{
					this.sounds[HuntRank.A] = new AudioFileReader(soundFile);
					Settings.Default.ABell = soundFile;
					return true;
				}
				if (name == "BBell")
				{
					this.sounds[HuntRank.B] = new AudioFileReader(soundFile);
					Settings.Default.BBell = soundFile;
					return true;
				}
				if (name == "FATEBell")
				{
					this.sounds[HuntRank.FATE] = new AudioFileReader(soundFile);
					Settings.Default.FATEBell = soundFile;
					return true;
				}
			}
			return false;
		}

		private void UnsetAlarmSound(AlarmButton r)
		{
			r.ToolTip = FFXIV_GameSense.Properties.Resources.NoSoundAlert;
			r.SetOff();
			string name = r.Name;
			if (name != null)
			{
				AudioFileReader s;
				if (!(name == "SBell"))
				{
					if (!(name == "ABell"))
					{
						if (!(name == "BBell"))
						{
							if (!(name == "FATEBell"))
							{
								return;
							}
							if (this.sounds.TryGetValue(HuntRank.FATE, out s))
							{
								this.sounds.Remove(HuntRank.FATE);
							}
							Settings.Default.FATEBell = FFXIV_GameSense.Properties.Resources.NoSoundAlert;
						}
						else
						{
							if (this.sounds.TryGetValue(HuntRank.B, out s))
							{
								this.sounds.Remove(HuntRank.B);
							}
							Settings.Default.BBell = FFXIV_GameSense.Properties.Resources.NoSoundAlert;
						}
					}
					else
					{
						if (this.sounds.TryGetValue(HuntRank.A, out s))
						{
							this.sounds.Remove(HuntRank.A);
						}
						Settings.Default.ABell = FFXIV_GameSense.Properties.Resources.NoSoundAlert;
					}
				}
				else
				{
					if (this.sounds.TryGetValue(HuntRank.S, out s))
					{
						this.sounds.Remove(HuntRank.S);
					}
					Settings.Default.SBell = FFXIV_GameSense.Properties.Resources.NoSoundAlert;
				}
				if (s != null)
				{
					s.Position = s.Length;
					s.Dispose();
				}
				return;
			}
		}

		private void SBell_Click(object sender, RoutedEventArgs e)
		{
			if (this.currentCMPlacement != null)
			{
				this.currentCMPlacement.SetOff();
				this.currentCMPlacement = null;
			}
			if (((AlarmButton)sender).IsOn())
			{
				this.UnsetAlarmSound((AlarmButton)sender);
				return;
			}
			if (((AlarmButton)sender).IsEnabled)
			{
				ContextMenu cm = new ContextMenu();
				MenuItem mi = new MenuItem
				{
					Header = FFXIV_GameSense.Properties.Resources.FormSFCMNewAlert
				};
				mi.Click += this.MenuItemClickCallCheckBox;
				cm.Items.Add(mi);
				if (Settings.Default.SBell != FFXIV_GameSense.Properties.Resources.NoSoundAlert)
				{
					MenuItem miS = new MenuItem
					{
						Header = Settings.Default.SBell
					};
					miS.Click += this.MenuItemSoundSelected;
					cm.Items.Add(miS);
				}
				if (Settings.Default.ABell != FFXIV_GameSense.Properties.Resources.NoSoundAlert)
				{
					MenuItem miA = new MenuItem
					{
						Header = Settings.Default.ABell
					};
					miA.Click += this.MenuItemSoundSelected;
					cm.Items.Add(miA);
				}
				if (Settings.Default.BBell != FFXIV_GameSense.Properties.Resources.NoSoundAlert)
				{
					MenuItem miB = new MenuItem
					{
						Header = Settings.Default.BBell
					};
					miB.Click += this.MenuItemSoundSelected;
					cm.Items.Add(miB);
				}
				if (Settings.Default.FATEBell != FFXIV_GameSense.Properties.Resources.NoSoundAlert)
				{
					MenuItem miFATE = new MenuItem
					{
						Header = Settings.Default.FATEBell
					};
					miFATE.Click += this.MenuItemSoundSelected;
					cm.Items.Add(miFATE);
				}
				for (int i = 0; i < cm.Items.Count; i++)
				{
					for (int j = 0; j < cm.Items.Count; j++)
					{
						if (i != j && ((MenuItem)cm.Items[i]).Header.ToString() == ((MenuItem)cm.Items[j]).Header.ToString())
						{
							cm.Items.RemoveAt(j);
						}
					}
				}
				if (cm.Items.Count < 2)
				{
					this.CheckBox_Checked(sender, e);
					return;
				}
				cm.PlacementTarget = (this.currentCMPlacement = (AlarmButton)sender);
				cm.Closed += this.Cm_Closed;
				((AlarmButton)sender).ContextMenu = cm;
				((AlarmButton)sender).ContextMenu.IsOpen = true;
			}
		}

		private void Cm_Closed(object sender, RoutedEventArgs e)
		{
			if (this.currentCMPlacement != null)
			{
				this.currentCMPlacement.SetOff();
			}
			this.currentCMPlacement = null;
		}

		private void MenuItemSoundSelected(object sender, RoutedEventArgs e)
		{
			AlarmButton cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as AlarmButton;
			this.SetAlarmSound(cb, ((MenuItem)sender).Header.ToString());
			this.currentCMPlacement = null;
		}

		private void MenuItemClickCallCheckBox(object sender, RoutedEventArgs e)
		{
			AlarmButton cb = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as AlarmButton;
			this.CheckBox_Checked(cb, new RoutedEventArgs());
		}

		private void SCheck_Checked(object sender, RoutedEventArgs e)
		{
			if (!Settings.Default.SARR && !Settings.Default.SHW && !Settings.Default.SSB && !Settings.Default.SSHB)
			{
				Settings.Default.SSHB = (Settings.Default.SSB = (Settings.Default.SHW = (Settings.Default.SARR = true)));
			}
		}

		private void ACheck_Checked(object sender, RoutedEventArgs e)
		{
			if (!Settings.Default.AARR && !Settings.Default.AHW && !Settings.Default.ASB && !Settings.Default.ASHB)
			{
				Settings.Default.ASHB = (Settings.Default.ASB = (Settings.Default.AHW = (Settings.Default.AARR = true)));
			}
		}

		private void BCheck_Checked(object sender, RoutedEventArgs e)
		{
			if (!Settings.Default.BARR && !Settings.Default.BHW && !Settings.Default.BSB && !Settings.Default.BSHB)
			{
				Settings.Default.BSHB = (Settings.Default.BSB = (Settings.Default.BHW = (Settings.Default.BARR = true)));
			}
		}

		private void SCheck_Unchecked(object sender, RoutedEventArgs e)
		{
			this.UnsetAlarmSound(this.SBell);
		}

		private void ACheck_Unchecked(object sender, RoutedEventArgs e)
		{
			this.UnsetAlarmSound(this.ABell);
		}

		private void BCheck_Unchecked(object sender, RoutedEventArgs e)
		{
			this.UnsetAlarmSound(this.BBell);
		}

		private void FilterCheckBoxOpacityUp(object sender, RoutedEventArgs e)
		{
			((System.Windows.Controls.CheckBox)sender).Opacity = 1.0;
		}

		private void FilterCheckBoxOpacityDown(object sender, RoutedEventArgs e)
		{
			if (((System.Windows.Controls.CheckBox)sender).Name.StartsWith("S", StringComparison.Ordinal) && (!this.SARR.IsChecked).Value && (!this.SHW.IsChecked).Value && (!this.SSB.IsChecked).Value && (!this.SSHB.IsChecked).Value)
			{
				Settings.Default.notifyS = false;
			}
			else if (((System.Windows.Controls.CheckBox)sender).Name.StartsWith("A", StringComparison.Ordinal) && (!this.AARR.IsChecked).Value && (!this.AHW.IsChecked).Value && (!this.ASB.IsChecked).Value && (!this.ASHB.IsChecked).Value)
			{
				Settings.Default.notifyA = false;
			}
			else if (((System.Windows.Controls.CheckBox)sender).Name.StartsWith("B", StringComparison.Ordinal) && (!this.BARR.IsChecked).Value && (!this.BHW.IsChecked).Value && (!this.BSB.IsChecked).Value && (!this.BSHB.IsChecked).Value)
			{
				Settings.Default.notifyB = false;
			}
			((System.Windows.Controls.CheckBox)sender).Opacity = 0.34999999403953552;
		}

		private void UniformGrid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (((UniformGrid)sender).IsEnabled)
			{
				((UniformGrid)sender).Opacity = 1.0;
				return;
			}
			((UniformGrid)sender).Opacity = 0.34999999403953552;
		}

		private void OpenSettings(object sender, RoutedEventArgs e)
		{
			if (Window1.SettingsWindow != null && Window1.SettingsWindow.IsVisible)
			{
				Window1.SettingsWindow.Activate();
				return;
			}
			Window1.SettingsWindow = new SettingsForm();
			Window1.SettingsWindow.Show();
		}

		private void OpenLogViewer(object sender, RoutedEventArgs e)
		{
			if (Window1.LogView.IsVisible)
			{
				Window1.LogView.Activate();
				return;
			}
			Window1.LogView.Show();
		}

		private void FATEBell_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is AlarmButton && !(bool)e.NewValue && Settings.Default.FATEs.Count == 0)
			{
				this.UnsetAlarmSound((AlarmButton)sender);
			}
		}

		private void FATEsListView_AllFATEsDeselected(object sender, EventArgs e)
		{
			this.FATEBell.IsEnabled = false;
		}

		private void FATEsListView_FATESelected(object sender, EventArgs e)
		{
			this.FATEBell.IsEnabled = true;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.8.1.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocater = new Uri("/XIV-Hunt;component/menuformwpf.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocater);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.8.1.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.8.1.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((System.Windows.Controls.Button)target).Click += this.OpenSettings;
				return;
			case 2:
				((System.Windows.Controls.Button)target).Click += this.OpenLogViewer;
				return;
			case 3:
				this.HuntConnectionTextBlock = (TextBlock)target;
				return;
			case 4:
				this.MainTabControl = (System.Windows.Controls.TabControl)target;
				return;
			case 5:
				this.HuntNotifyGroupBox = (System.Windows.Controls.GroupBox)target;
				return;
			case 6:
				this.Grid = (Grid)target;
				return;
			case 7:
				this.SBell = (AlarmButton)target;
				return;
			case 8:
				this.ABell = (AlarmButton)target;
				return;
			case 9:
				this.BBell = (AlarmButton)target;
				return;
			case 10:
				this.FATEBell = (AlarmButton)target;
				return;
			case 11:
				this.FATEsListView = (FATEsListView)target;
				return;
			case 12:
				this.SCheck = (System.Windows.Controls.CheckBox)target;
				this.SCheck.Checked += this.SCheck_Checked;
				this.SCheck.Unchecked += this.SCheck_Unchecked;
				return;
			case 13:
				((UniformGrid)target).IsEnabledChanged += this.UniformGrid_IsEnabledChanged;
				return;
			case 14:
				this.SARR = (System.Windows.Controls.CheckBox)target;
				this.SARR.Checked += this.FilterCheckBoxOpacityUp;
				this.SARR.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 15:
				this.SHW = (System.Windows.Controls.CheckBox)target;
				this.SHW.Checked += this.FilterCheckBoxOpacityUp;
				this.SHW.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 16:
				this.SSB = (System.Windows.Controls.CheckBox)target;
				this.SSB.Checked += this.FilterCheckBoxOpacityUp;
				this.SSB.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 17:
				this.SSHB = (System.Windows.Controls.CheckBox)target;
				this.SSHB.Checked += this.FilterCheckBoxOpacityUp;
				this.SSHB.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 18:
				this.ACheck = (System.Windows.Controls.CheckBox)target;
				this.ACheck.Checked += this.ACheck_Checked;
				this.ACheck.Unchecked += this.ACheck_Unchecked;
				return;
			case 19:
				((UniformGrid)target).IsEnabledChanged += this.UniformGrid_IsEnabledChanged;
				return;
			case 20:
				this.AARR = (System.Windows.Controls.CheckBox)target;
				this.AARR.Checked += this.FilterCheckBoxOpacityUp;
				this.AARR.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 21:
				this.AHW = (System.Windows.Controls.CheckBox)target;
				this.AHW.Checked += this.FilterCheckBoxOpacityUp;
				this.AHW.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 22:
				this.ASB = (System.Windows.Controls.CheckBox)target;
				this.ASB.Checked += this.FilterCheckBoxOpacityUp;
				this.ASB.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 23:
				this.ASHB = (System.Windows.Controls.CheckBox)target;
				this.ASHB.Checked += this.FilterCheckBoxOpacityUp;
				this.ASHB.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 24:
				this.BCheck = (System.Windows.Controls.CheckBox)target;
				this.BCheck.Checked += this.BCheck_Checked;
				this.BCheck.Unchecked += this.BCheck_Unchecked;
				return;
			case 25:
				((UniformGrid)target).IsEnabledChanged += this.UniformGrid_IsEnabledChanged;
				return;
			case 26:
				this.BARR = (System.Windows.Controls.CheckBox)target;
				this.BARR.Checked += this.FilterCheckBoxOpacityUp;
				this.BARR.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 27:
				this.BHW = (System.Windows.Controls.CheckBox)target;
				this.BHW.Checked += this.FilterCheckBoxOpacityUp;
				this.BHW.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 28:
				this.BSB = (System.Windows.Controls.CheckBox)target;
				this.BSB.Checked += this.FilterCheckBoxOpacityUp;
				this.BSB.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 29:
				this.BSHB = (System.Windows.Controls.CheckBox)target;
				this.BSHB.Checked += this.FilterCheckBoxOpacityUp;
				this.BSHB.Unchecked += this.FilterCheckBoxOpacityDown;
				return;
			case 30:
				this.OverlayView = (OverlayView)target;
				return;
			case 31:
				this.ProcessComboBox = (System.Windows.Controls.ComboBox)target;
				return;
			case 32:
				this.LanguageSelector = (FFXIV_GameSense.UI.Controls.LanguageSelector)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		private readonly DispatcherTimer dispatcherTimer1s;

		private FFXIVHunts hunts;

		internal Dictionary<HuntRank, AudioFileReader> sounds = new Dictionary<HuntRank, AudioFileReader>();

		private AlarmButton currentCMPlacement;

		private readonly NotifyIcon trayIcon;

		private ViewModel vm;

		private static SettingsForm SettingsWindow;

		private static readonly LogView LogView = new LogView();

		private CancellationTokenSource cts;

		private static bool IconIsFlashing = false;

		internal TextBlock HuntConnectionTextBlock;

		internal System.Windows.Controls.TabControl MainTabControl;

		internal System.Windows.Controls.GroupBox HuntNotifyGroupBox;

		internal Grid Grid;

		internal AlarmButton SBell;

		internal AlarmButton ABell;

		internal AlarmButton BBell;

		internal AlarmButton FATEBell;

		internal FATEsListView FATEsListView;

		internal System.Windows.Controls.CheckBox SCheck;

		internal System.Windows.Controls.CheckBox SARR;

		internal System.Windows.Controls.CheckBox SHW;

		internal System.Windows.Controls.CheckBox SSB;

		internal System.Windows.Controls.CheckBox SSHB;

		internal System.Windows.Controls.CheckBox ACheck;

		internal System.Windows.Controls.CheckBox AARR;

		internal System.Windows.Controls.CheckBox AHW;

		internal System.Windows.Controls.CheckBox ASB;

		internal System.Windows.Controls.CheckBox ASHB;

		internal System.Windows.Controls.CheckBox BCheck;

		internal System.Windows.Controls.CheckBox BARR;

		internal System.Windows.Controls.CheckBox BHW;

		internal System.Windows.Controls.CheckBox BSB;

		internal System.Windows.Controls.CheckBox BSHB;

		internal OverlayView OverlayView;

		internal System.Windows.Controls.ComboBox ProcessComboBox;

		internal FFXIV_GameSense.UI.Controls.LanguageSelector LanguageSelector;

		private bool _contentLoaded;
	}
}

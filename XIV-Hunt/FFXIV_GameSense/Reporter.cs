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
	internal class Reporter
	{
		public ushort HomeWorldID { get; private set; }

		public ushort CurrentWorldID { get; private set; }

		public string Name { get; private set; }

		public Version Version { get; private set; }

		[JsonIgnore]
		public string NameAndHomeWorld
		{
			get
			{
				return this.Name + " (" + GameResources.GetWorldName(this.HomeWorldID) + ")";
			}
		}

		public ObservableHashSet<ushort> SubscribedHunts
		{
			get
			{
				return Settings.Default.Hunts;
			}
		}

		public ObservableHashSet<ushort> SubscribedFATEs
		{
			get
			{
				return Settings.Default.FATEs;
			}
		}

		public bool SubscribedToOtherWorlds
		{
			get
			{
				return Settings.Default.NotificationsFromOtherWorlds;
			}
		}

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

		protected virtual void Dispose(bool disposing)
		{
		}

		public void Dispose()
		{
			Settings.Default.PropertyChanged -= this.ReporterSettingsPropertyChanged;
			this.SubscribedHunts.CollectionChanged -= this.SubscribedHunt_CollectionChanged;
			this.SubscribedFATEs.CollectionChanged -= this.SubscribedFATEs_CollectionChanged;
			this.hubConnection = null;
			this.Hunts = null;
			this.Dispose(true);
		}

		[JsonIgnore]
		private IReadOnlyList<Hunt> Hunts;

		[JsonIgnore]
		private HubConnection hubConnection;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FFXIV_GameSense.UI;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	public class ViewModel : INotifyPropertyChanged
	{
		public ViewModel()
		{
			this.FATEEntries = new ObservableCollection<FATEListViewItem>();
			foreach (FATE f in GameResources.GetFates().DistinctBy((FATE x) => x.Name(false)))
			{
				this.FATEEntries.Add(new FATEListViewItem(f));
			}
		}

		public void Refresh()
		{
			this.OnPropertyChanged("ProcessEntries");
			if ((FFXIVHunts.Joined || FFXIVHunts.Joining) && !this.GotFATEZones && !this.IsFetchingZones)
			{
				this.GetFATEZones();
			}
		}

		public ObservableCollection<Process> ProcessEntries
		{
			get
			{
				return new ObservableCollection<Process>(FFXIVProcessHelper.GetFFXIVProcessList());
			}
		}

		public ObservableCollection<FATEListViewItem> FATEEntries { get; }

		public bool FATEsAny
		{
			get
			{
				return this.FATEEntries.Any((FATEListViewItem x) => x.Announce);
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private async Task GetFATEZones()
		{
			this.IsFetchingZones = true;
			HttpResponseMessage r = await FFXIVHunts.Http.GetAsync(new Uri("https://xivhunt.net/api/worlds/FATEIDZoneID/")).ConfigureAwait(false);
			if (r.IsSuccessStatusCode)
			{
				string e = await r.Content.ReadAsStringAsync().ConfigureAwait(false);
				Dictionary<ushort, ushort[]> fateidzoneid = JsonConvert.DeserializeObject<Dictionary<ushort, ushort[]>>(e);
				Func<FATEListViewItem, bool> <>9__1;
				await Task.Run(delegate()
				{
					IEnumerable<FATEListViewItem> fateentries = this.FATEEntries;
					Func<FATEListViewItem, bool> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((FATEListViewItem x) => fateidzoneid.ContainsKey(x.ID)));
					}
					foreach (FATEListViewItem i in fateentries.Where(predicate))
					{
						i.Zones = string.Join(", ", (from x in fateidzoneid[i.ID].Distinct<ushort>()
						select GameResources.GetZoneName((uint)x)).Distinct<string>());
					}
				}).ConfigureAwait(false);
				this.GotFATEZones = true;
			}
			this.IsFetchingZones = false;
		}

		private bool GotFATEZones;

		private bool IsFetchingZones;
	}
}

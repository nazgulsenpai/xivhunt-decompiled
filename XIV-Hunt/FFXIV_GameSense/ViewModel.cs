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
	// Token: 0x02000088 RID: 136
	public class ViewModel : INotifyPropertyChanged
	{
		// Token: 0x0600039E RID: 926 RVA: 0x000118C8 File Offset: 0x0000FAC8
		public ViewModel()
		{
			this.FATEEntries = new ObservableCollection<FATEListViewItem>();
			foreach (FATE f in GameResources.GetFates().DistinctBy((FATE x) => x.Name(false)))
			{
				this.FATEEntries.Add(new FATEListViewItem(f));
			}
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00011954 File Offset: 0x0000FB54
		public void Refresh()
		{
			this.OnPropertyChanged("ProcessEntries");
			if ((FFXIVHunts.Joined || FFXIVHunts.Joining) && !this.GotFATEZones && !this.IsFetchingZones)
			{
				this.GetFATEZones();
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00011986 File Offset: 0x0000FB86
		public ObservableCollection<Process> ProcessEntries
		{
			get
			{
				return new ObservableCollection<Process>(FFXIVProcessHelper.GetFFXIVProcessList());
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00011992 File Offset: 0x0000FB92
		public ObservableCollection<FATEListViewItem> FATEEntries { get; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x0001199A File Offset: 0x0000FB9A
		public bool FATEsAny
		{
			get
			{
				return this.FATEEntries.Any((FATEListViewItem x) => x.Announce);
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x000119C6 File Offset: 0x0000FBC6
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060003A4 RID: 932 RVA: 0x000119E0 File Offset: 0x0000FBE0
		// (remove) Token: 0x060003A5 RID: 933 RVA: 0x00011A18 File Offset: 0x0000FC18
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x060003A6 RID: 934 RVA: 0x00011A50 File Offset: 0x0000FC50
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

		// Token: 0x040002C9 RID: 713
		private bool GotFATEZones;

		// Token: 0x040002CA RID: 714
		private bool IsFetchingZones;
	}
}

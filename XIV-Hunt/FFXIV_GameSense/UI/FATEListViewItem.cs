using System;
using System.ComponentModel;
using FFXIV_GameSense.Properties;
using XIVDB;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000AC RID: 172
	public class FATEListViewItem : INotifyPropertyChanged
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x000148A0 File Offset: 0x00012AA0
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x000148A8 File Offset: 0x00012AA8
		public ushort ID { get; private set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x000148B1 File Offset: 0x00012AB1
		public byte ClassJobLevel
		{
			get
			{
				return GameResources.GetFATEInfo(this.ID).ClassJobLevel;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x000148C3 File Offset: 0x00012AC3
		public string Icon
		{
			get
			{
				return "https://xivhunt.net/images/" + GameResources.GetFATEInfo(this.ID).IconMap;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x000148DF File Offset: 0x00012ADF
		public string Name
		{
			get
			{
				return GameResources.GetFATEInfo(this.ID).Name;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x000148F1 File Offset: 0x00012AF1
		// (set) Token: 0x0600046C RID: 1132 RVA: 0x00014902 File Offset: 0x00012B02
		public string Zones
		{
			get
			{
				return this.zones ?? string.Empty;
			}
			set
			{
				if (this.zones != value)
				{
					this.zones = value;
					this.OnPropertyChanged("Zones");
				}
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x00014924 File Offset: 0x00012B24
		// (set) Token: 0x0600046E RID: 1134 RVA: 0x0001493C File Offset: 0x00012B3C
		public bool Announce
		{
			get
			{
				return Settings.Default.FATEs.Contains(this.ID);
			}
			set
			{
				if (!value)
				{
					while (Settings.Default.FATEs.Contains(this.ID))
					{
						Settings.Default.FATEs.Remove(this.ID);
						this.OnPropertyChanged("Announce");
					}
					return;
				}
				if (!Settings.Default.FATEs.Contains(this.ID))
				{
					Settings.Default.FATEs.Add(this.ID);
					this.OnPropertyChanged("Announce");
				}
			}
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x000149BE File Offset: 0x00012BBE
		public FATEListViewItem(FATE f)
		{
			if (f == null)
			{
				throw new ArgumentNullException("f");
			}
			this.ID = f.ID;
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000470 RID: 1136 RVA: 0x000149E0 File Offset: 0x00012BE0
		// (remove) Token: 0x06000471 RID: 1137 RVA: 0x00014A18 File Offset: 0x00012C18
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000472 RID: 1138 RVA: 0x00014A4D File Offset: 0x00012C4D
		private void OnPropertyChanged(string prop)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		// Token: 0x0400036D RID: 877
		private string zones;
	}
}

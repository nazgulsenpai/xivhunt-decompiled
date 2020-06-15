using System;
using System.ComponentModel;
using FFXIV_GameSense.Properties;
using XIVDB;

namespace FFXIV_GameSense.UI
{
	public class FATEListViewItem : INotifyPropertyChanged
	{
		public ushort ID { get; private set; }

		public byte ClassJobLevel
		{
			get
			{
				return GameResources.GetFATEInfo(this.ID).ClassJobLevel;
			}
		}

		public string Icon
		{
			get
			{
				return "https://xivhunt.net/images/" + GameResources.GetFATEInfo(this.ID).IconMap;
			}
		}

		public string Name
		{
			get
			{
				return GameResources.GetFATEInfo(this.ID).Name;
			}
		}

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

		public FATEListViewItem(FATE f)
		{
			if (f == null)
			{
				throw new ArgumentNullException("f");
			}
			this.ID = f.ID;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string prop)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		private string zones;
	}
}

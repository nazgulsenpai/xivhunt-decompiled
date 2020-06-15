using System;
using System.ComponentModel;

namespace FFXIV_GameSense.UI
{
	public class AlarmButtonModel : INotifyPropertyChanged
	{
		public string Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (value != this.icon)
				{
					this.icon = value;
					this.OnPropertyChanged("Icon");
				}
			}
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

		private string icon = "/Resources/Images/sound_off.png";
	}
}

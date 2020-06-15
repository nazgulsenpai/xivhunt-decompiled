using System;
using System.ComponentModel;
using System.Windows.Media;

namespace FFXIV_GameSense.Overlay
{
	public class EntityOverlayControlViewModel : INotifyPropertyChanged
	{
		public string Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (this.icon != value)
				{
					this.icon = value;
					this.OnPropertyChanged("Icon");
				}
			}
		}

		public string Name
		{
			get
			{
				return this.name ?? string.Empty;
			}
			set
			{
				if (this.name != value)
				{
					this.name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		public Brush NameColor
		{
			get
			{
				return this.nameColor;
			}
			set
			{
				if (this.nameColor != value)
				{
					this.nameColor = value;
					this.OnPropertyChanged("NameColor");
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

		private string icon;

		private string name;

		private Brush nameColor = new SolidColorBrush(Colors.Black);
	}
}

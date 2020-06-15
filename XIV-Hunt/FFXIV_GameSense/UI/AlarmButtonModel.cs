using System;
using System.ComponentModel;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000A8 RID: 168
	public class AlarmButtonModel : INotifyPropertyChanged
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000455 RID: 1109 RVA: 0x0001469C File Offset: 0x0001289C
		// (set) Token: 0x06000456 RID: 1110 RVA: 0x000146A4 File Offset: 0x000128A4
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

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000457 RID: 1111 RVA: 0x000146C8 File Offset: 0x000128C8
		// (remove) Token: 0x06000458 RID: 1112 RVA: 0x00014700 File Offset: 0x00012900
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000459 RID: 1113 RVA: 0x00014735 File Offset: 0x00012935
		private void OnPropertyChanged(string prop)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		// Token: 0x04000369 RID: 873
		private string icon = "/Resources/Images/sound_off.png";
	}
}

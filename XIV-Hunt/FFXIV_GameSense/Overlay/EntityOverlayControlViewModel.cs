using System;
using System.ComponentModel;
using System.Windows.Media;

namespace FFXIV_GameSense.Overlay
{
	// Token: 0x020000C2 RID: 194
	public class EntityOverlayControlViewModel : INotifyPropertyChanged
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x000178E1 File Offset: 0x00015AE1
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x000178E9 File Offset: 0x00015AE9
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

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0001790B File Offset: 0x00015B0B
		// (set) Token: 0x06000508 RID: 1288 RVA: 0x0001791C File Offset: 0x00015B1C
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

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000509 RID: 1289 RVA: 0x0001793E File Offset: 0x00015B3E
		// (set) Token: 0x0600050A RID: 1290 RVA: 0x00017946 File Offset: 0x00015B46
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

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x0600050B RID: 1291 RVA: 0x00017964 File Offset: 0x00015B64
		// (remove) Token: 0x0600050C RID: 1292 RVA: 0x0001799C File Offset: 0x00015B9C
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x0600050D RID: 1293 RVA: 0x000179D1 File Offset: 0x00015BD1
		private void OnPropertyChanged(string prop)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(prop));
		}

		// Token: 0x040003D0 RID: 976
		private string icon;

		// Token: 0x040003D1 RID: 977
		private string name;

		// Token: 0x040003D2 RID: 978
		private Brush nameColor = new SolidColorBrush(Colors.Black);
	}
}

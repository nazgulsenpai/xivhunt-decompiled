using System;
using System.Globalization;
using System.Windows.Data;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000AA RID: 170
	internal class ByteToPercentageConverter : IValueConverter
	{
		// Token: 0x06000460 RID: 1120 RVA: 0x000147D3 File Offset: 0x000129D3
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is byte)
			{
				return (float)((byte)value) / 100f;
			}
			return null;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x000147F1 File Offset: 0x000129F1
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double)
			{
				return (byte)((double)value * 100.0);
			}
			return null;
		}
	}
}

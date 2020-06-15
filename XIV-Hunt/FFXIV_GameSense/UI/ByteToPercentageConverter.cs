using System;
using System.Globalization;
using System.Windows.Data;

namespace FFXIV_GameSense.UI
{
	internal class ByteToPercentageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is byte)
			{
				return (float)((byte)value) / 100f;
			}
			return null;
		}

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

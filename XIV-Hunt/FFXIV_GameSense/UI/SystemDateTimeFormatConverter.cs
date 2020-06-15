using System;
using System.Globalization;
using System.Windows.Data;

namespace FFXIV_GameSense.UI
{
	internal class SystemDateTimeFormatConverter : IValueConverter
	{
		private static string GetSysDateTimeFormat()
		{
			CultureInfo ci = NativeMethods.GetSystemDefaultCultureInfo();
			return ci.DateTimeFormat.ShortDatePattern + " " + ci.DateTimeFormat.ShortTimePattern;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is DateTime)
			{
				return ((DateTime)value).ToString(SystemDateTimeFormatConverter.SystemDateTimeFormat, NativeMethods.GetSystemDefaultCultureInfo());
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static readonly string SystemDateTimeFormat = SystemDateTimeFormatConverter.GetSysDateTimeFormat();
	}
}

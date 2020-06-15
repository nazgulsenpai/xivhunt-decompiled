using System;
using System.Globalization;
using System.Windows.Data;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000A9 RID: 169
	internal class SystemDateTimeFormatConverter : IValueConverter
	{
		// Token: 0x0600045B RID: 1115 RVA: 0x00014764 File Offset: 0x00012964
		private static string GetSysDateTimeFormat()
		{
			CultureInfo ci = NativeMethods.GetSystemDefaultCultureInfo();
			return ci.DateTimeFormat.ShortDatePattern + " " + ci.DateTimeFormat.ShortTimePattern;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00014798 File Offset: 0x00012998
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is DateTime)
			{
				return ((DateTime)value).ToString(SystemDateTimeFormatConverter.SystemDateTimeFormat, NativeMethods.GetSystemDefaultCultureInfo());
			}
			return null;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x00014521 File Offset: 0x00012721
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0400036B RID: 875
		private static readonly string SystemDateTimeFormat = SystemDateTimeFormatConverter.GetSysDateTimeFormat();
	}
}

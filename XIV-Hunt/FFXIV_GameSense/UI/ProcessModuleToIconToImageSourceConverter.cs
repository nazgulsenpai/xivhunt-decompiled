using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Splat;

namespace FFXIV_GameSense.UI
{
	internal class ProcessModuleToIconToImageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is ProcessModule && File.Exists((value as ProcessModule).FileName))
			{
				using (Icon icon = Icon.ExtractAssociatedIcon((value as ProcessModule).FileName))
				{
					return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				}
			}
			LogHost.Default.Warn(CultureInfo.CurrentCulture, "Attempted to convert {0} instead of ProcessModule object in ProcessModuleToIconToImageSourceConverter", new object[]
			{
				value
			});
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

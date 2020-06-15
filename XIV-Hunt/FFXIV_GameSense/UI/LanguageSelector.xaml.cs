using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using FFXIV_GameSense.Properties;
using FFXIV_GameSense.UI.Controls;

namespace FFXIV_GameSense.UI
{
	public partial class LanguageSelector : Window
	{
		public LanguageSelector()
		{
			this.InitializeComponent();
			base.Title = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.LanguageSelectorTitle, Program.AssemblyName.Name);
		}

		private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CultureInfo culture = CultureInfo.GetCultureInfo(Settings.Default.LanguageCI);
			base.Title = string.Format(culture, FFXIV_GameSense.Properties.Resources.ResourceManager.GetString("LanguageSelectorTitle", culture), Program.AssemblyName.Name);
			this.InfoTextBlock.Text = FFXIV_GameSense.Properties.Resources.ResourceManager.GetString("LanguageSelectorInfo", culture);
			this.Button.Content = FFXIV_GameSense.Properties.Resources.ResourceManager.GetString("LanguageSelectorContinue", culture);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.Button_Click(sender, e);
			}
		}
	}
}

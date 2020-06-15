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
	// Token: 0x020000B3 RID: 179
	public partial class LanguageSelector : Window
	{
		// Token: 0x0600049D RID: 1181 RVA: 0x0001565F File Offset: 0x0001385F
		public LanguageSelector()
		{
			this.InitializeComponent();
			base.Title = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.LanguageSelectorTitle, Program.AssemblyName.Name);
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001568C File Offset: 0x0001388C
		private void LanguageSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CultureInfo culture = CultureInfo.GetCultureInfo(Settings.Default.LanguageCI);
			base.Title = string.Format(culture, FFXIV_GameSense.Properties.Resources.ResourceManager.GetString("LanguageSelectorTitle", culture), Program.AssemblyName.Name);
			this.InfoTextBlock.Text = FFXIV_GameSense.Properties.Resources.ResourceManager.GetString("LanguageSelectorInfo", culture);
			this.Button.Content = FFXIV_GameSense.Properties.Resources.ResourceManager.GetString("LanguageSelectorContinue", culture);
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x00015705 File Offset: 0x00013905
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x00015713 File Offset: 0x00013913
		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				this.Button_Click(sender, e);
			}
		}
	}
}

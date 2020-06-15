using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using FFXIV_GameSense.Properties;

namespace FFXIV_GameSense.UI.Controls
{
	// Token: 0x020000C0 RID: 192
	public partial class LanguageSelector : UserControl
	{
		// Token: 0x060004EF RID: 1263 RVA: 0x00017217 File Offset: 0x00015417
		public LanguageSelector()
		{
			this.InitializeComponent();
			this.LanguageComboBox.SelectionChanged += this.LanguageComboBox_SelectionChanged;
			base.GotFocus += this.LanguageSelector_GotFocus;
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0001724E File Offset: 0x0001544E
		private void LanguageSelector_GotFocus(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
			{
				this.LanguageComboBox.Focus();
			}));
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00017269 File Offset: 0x00015469
		private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Settings.Default.Save();
			if (this.RestartOnChange)
			{
				Updater.RestartApp();
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00017282 File Offset: 0x00015482
		// (set) Token: 0x060004F3 RID: 1267 RVA: 0x00017294 File Offset: 0x00015494
		public bool RestartOnChange
		{
			get
			{
				return (bool)base.GetValue(LanguageSelector.RestartOnChangeProperty);
			}
			set
			{
				base.SetValue(LanguageSelector.RestartOnChangeProperty, value);
			}
		}

		// Token: 0x040003C8 RID: 968
		public static readonly DependencyProperty RestartOnChangeProperty = DependencyProperty.Register("RestartOnChange", typeof(bool), typeof(LanguageSelector), new PropertyMetadata(false));
	}
}

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
	public partial class LanguageSelector : UserControl
	{
		public LanguageSelector()
		{
			this.InitializeComponent();
			this.LanguageComboBox.SelectionChanged += this.LanguageComboBox_SelectionChanged;
			base.GotFocus += this.LanguageSelector_GotFocus;
		}

		private void LanguageSelector_GotFocus(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(delegate()
			{
				this.LanguageComboBox.Focus();
			}));
		}

		private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Settings.Default.Save();
			if (this.RestartOnChange)
			{
				Updater.RestartApp();
			}
		}

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

		public static readonly DependencyProperty RestartOnChangeProperty = DependencyProperty.Register("RestartOnChange", typeof(bool), typeof(LanguageSelector), new PropertyMetadata(false));
	}
}

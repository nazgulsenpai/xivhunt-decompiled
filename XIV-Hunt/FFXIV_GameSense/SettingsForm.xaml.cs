using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using FFXIV_GameSense.Properties;
using Microsoft.Win32;
using NAudio.CoreAudioApi;

namespace FFXIV_GameSense
{
	// Token: 0x020000A1 RID: 161
	public partial class SettingsForm : Window
	{
		// Token: 0x0600041E RID: 1054 RVA: 0x0001387C File Offset: 0x00011A7C
		public SettingsForm()
		{
			this.InitializeComponent();
			object cv = this.registryKey.GetValue(Program.AssemblyName.Name);
			if (cv != null && cv.Equals(SettingsForm.StartProcessPath))
			{
				this.StartWithWindowsCB.IsChecked = new bool?(true);
			}
			this.StartWithWindowsCB.Checked += this.StartWithWindowsCB_Checked;
			this.StartWithWindowsCB.Unchecked += this.StartWithWindowsCB_Unchecked;
			base.Closing += this.SettingsForm_Closing;
			if (!Directory.Exists(Settings.Default.PerformDirectory))
			{
				this.PerformDirectoryTextBox.Text = string.Empty;
			}
			this.RefreshAudioDevicesComboBox();
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0001394C File Offset: 0x00011B4C
		private void RefreshAudioDevicesComboBox()
		{
			using (MMDeviceEnumerator audioEndPointEnumerator = new MMDeviceEnumerator())
			{
				foreach (MMDevice audioEndPoint in audioEndPointEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
				{
					if (!this.AudioDevicesComboBox.Items.Contains(audioEndPoint.FriendlyName))
					{
						this.AudioDevicesComboBox.Items.Add(audioEndPoint.FriendlyName);
					}
				}
				if (!this.AudioDevicesComboBox.Items.Contains(Settings.Default.AudioDevice))
				{
					Settings.Default.AudioDevice = audioEndPointEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console).FriendlyName;
				}
				this.AudioDevicesComboBox.SelectedItem = Settings.Default.AudioDevice;
			}
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00013A2C File Offset: 0x00011C2C
		private void SettingsForm_Closing(object sender, CancelEventArgs e)
		{
			Settings.Default.Save();
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00013A38 File Offset: 0x00011C38
		private void StartWithWindowsCB_Unchecked(object sender, RoutedEventArgs e)
		{
			this.registryKey.DeleteValue(Program.AssemblyName.Name, false);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00013A50 File Offset: 0x00011C50
		private void StartWithWindowsCB_Checked(object sender, RoutedEventArgs e)
		{
			this.registryKey.SetValue(Program.AssemblyName.Name, SettingsForm.StartProcessPath);
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00013A6C File Offset: 0x00011C6C
		private void FATEPercentInterval_TextChanged(object sender, TextChangedEventArgs e)
		{
			System.Windows.Controls.TextBox textbox = sender as System.Windows.Controls.TextBox;
			int value;
			if (int.TryParse(textbox.Text, out value))
			{
				if (value > 100)
				{
					textbox.Text = 100.ToString(CultureInfo.CurrentCulture);
					return;
				}
				if (value < 0)
				{
					textbox.Text = 0.ToString(CultureInfo.CurrentCulture);
				}
			}
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x00013AC4 File Offset: 0x00011CC4
		private void PerformDirectoryTextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && Directory.Exists(dialog.SelectedPath))
				{
					Settings.Default.PerformDirectory = dialog.SelectedPath;
				}
			}
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00013B1C File Offset: 0x00011D1C
		private void LogOutButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.Cookies = string.Empty;
			Updater.RestartApp();
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00013B32 File Offset: 0x00011D32
		private void ForgetPerformDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.PerformDirectory = string.Empty;
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00013B43 File Offset: 0x00011D43
		private void AudioDevicesComboBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.RefreshAudioDevicesComboBox();
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00013B4B File Offset: 0x00011D4B
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (SoundPlayer.AudioFileReader != null)
			{
				SoundPlayer.AudioFileReader.Volume = (float)e.NewValue;
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00013B65 File Offset: 0x00011D65
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.ChatChannelComboBox.ItemsSource = Enum.GetValues(typeof(ChatChannel)).Cast<ChatChannel>();
		}

		// Token: 0x04000343 RID: 835
		private readonly RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		// Token: 0x04000344 RID: 836
		internal static readonly string StartProcessPath = Path.Combine(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.NthLastIndexOf(Path.DirectorySeparatorChar.ToString(), 2) + 1), "Update.exe --processStart XIV-Hunt.exe");
	}
}

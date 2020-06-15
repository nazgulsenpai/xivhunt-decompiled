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
	public partial class SettingsForm : Window
	{
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

		private void SettingsForm_Closing(object sender, CancelEventArgs e)
		{
			Settings.Default.Save();
		}

		private void StartWithWindowsCB_Unchecked(object sender, RoutedEventArgs e)
		{
			this.registryKey.DeleteValue(Program.AssemblyName.Name, false);
		}

		private void StartWithWindowsCB_Checked(object sender, RoutedEventArgs e)
		{
			this.registryKey.SetValue(Program.AssemblyName.Name, SettingsForm.StartProcessPath);
		}

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

		private void LogOutButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.Cookies = string.Empty;
			Updater.RestartApp();
		}

		private void ForgetPerformDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.PerformDirectory = string.Empty;
		}

		private void AudioDevicesComboBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.RefreshAudioDevicesComboBox();
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (SoundPlayer.AudioFileReader != null)
			{
				SoundPlayer.AudioFileReader.Volume = (float)e.NewValue;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.ChatChannelComboBox.ItemsSource = Enum.GetValues(typeof(ChatChannel)).Cast<ChatChannel>();
		}

		private readonly RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

		internal static readonly string StartProcessPath = Path.Combine(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.NthLastIndexOf(Path.DirectorySeparatorChar.ToString(), 2) + 1), "Update.exe --processStart XIV-Hunt.exe");
	}
}

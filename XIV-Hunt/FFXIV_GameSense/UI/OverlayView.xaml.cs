using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Threading;
using FFXIV_GameSense.Properties;
using Process.NET;
using Process.NET.Memory;

namespace FFXIV_GameSense.UI
{
	public partial class OverlayView : UserControl, IDisposable
	{
		public OverlayView()
		{
			this.InitializeComponent();
			this.RadarEntityScaleTextBox.TextChanged += this.RadarEntityScaleTextBox_TextChanged;
			this.RadarEntityOpacityTextBox.TextChanged += this.RadarEntityOpacityTextBox_TextChanged;
			this.RadarMaxFrameRateTextBox.TextChanged += this.RadarMaxFrameRateTextBox_TextChanged;
			this.RadarBGOpacityTextBox.TextChanged += this.RadarBGOpacityTextBox_TextChanged;
		}

		private void RadarEntityScaleTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			float value;
			if (float.TryParse(textbox.Text, out value))
			{
				if (value > 2f)
				{
					textbox.Text = 2.ToString(CultureInfo.CurrentCulture);
					return;
				}
				if ((double)value < 0.5)
				{
					textbox.Text = 0.5.ToString(CultureInfo.CurrentCulture);
					return;
				}
			}
			else
			{
				textbox.Text = 1.ToString(CultureInfo.CurrentCulture);
			}
		}

		private void RadarEntityOpacityTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			byte value;
			if (byte.TryParse(textbox.Text, out value))
			{
				if (value > 100)
				{
					textbox.Text = 100.ToString(CultureInfo.CurrentCulture);
					return;
				}
				if (value < 0)
				{
					textbox.Text = 0.ToString(CultureInfo.CurrentCulture);
					return;
				}
			}
			else
			{
				textbox.Text = 100.ToString(CultureInfo.CurrentCulture);
			}
		}

		private void RadarBGOpacityTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			byte value;
			if (!byte.TryParse(textbox.Text, out value))
			{
				textbox.Text = 0.ToString(CultureInfo.CurrentCulture);
				return;
			}
			if (value > 100)
			{
				textbox.Text = 100.ToString(CultureInfo.CurrentCulture);
			}
			else if (value < 0)
			{
				textbox.Text = 0.ToString(CultureInfo.CurrentCulture);
			}
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.SetBackgroundOpacity();
		}

		private void RadarMaxFrameRateTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			byte value;
			if (!byte.TryParse(textbox.Text, out value))
			{
				textbox.Text = 30.ToString(CultureInfo.CurrentCulture);
				return;
			}
			if (value > 144)
			{
				textbox.Text = 144.ToString(CultureInfo.CurrentCulture);
			}
			else if (value < 1)
			{
				textbox.Text = 1.ToString(CultureInfo.CurrentCulture);
			}
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.SetNewFrameRate();
		}

		private void _2DRadarToggle(object sender, RoutedEventArgs e)
		{
			ToggleButton b = (ToggleButton)sender;
			if (b.IsChecked.GetValueOrDefault())
			{
				this.cts = new CancellationTokenSource();
				this.RadarOverlayThread = new Thread(delegate()
				{
					this.ro = new RadarOverlay(this.cts.Token);
					ProcessSharp ps = new ProcessSharp(Program.mem.Process.Id, MemoryType.Remote);
					this.ro.Initialize(ps.WindowFactory.MainWindow);
					this.ro.Enable();
					Dispatcher.Run();
				})
				{
					IsBackground = true
				};
				this.RadarOverlayThread.SetApartmentState(ApartmentState.STA);
				this.RadarOverlayThread.Start();
				Task.Run(async delegate()
				{
					await Task.Delay(1000).ConfigureAwait(false);
					if (!this.cts.IsCancellationRequested && !Settings.Default.RadarEnableClickthru)
					{
						RadarOverlay radarOverlay2 = this.ro;
						if (radarOverlay2 != null)
						{
							radarOverlay2.MakeClickable();
						}
					}
				});
				return;
			}
			bool? isChecked = b.IsChecked;
			bool flag = true;
			if (!(isChecked.GetValueOrDefault() == flag & isChecked != null))
			{
				CancellationTokenSource cancellationTokenSource = this.cts;
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Cancel();
				}
				RadarOverlay radarOverlay = this.ro;
				if (radarOverlay != null)
				{
					radarOverlay.Dispose();
				}
				this.ro = null;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (disposing)
				{
					if (this.cts != null && !this.cts.IsCancellationRequested)
					{
						this.cts.Cancel();
						Thread.Sleep(1000 / (int)Settings.Default.RadarMaxFrameRate * 2);
					}
					if (this.cts != null)
					{
						this.cts.Dispose();
					}
					if (this.ro != null)
					{
						this.ro.Dispose();
					}
				}
				this.ro = null;
				this.disposedValue = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void ClickthruCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.MakeClickthru();
		}

		private void ClickthruCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.MakeClickable();
		}

		private void ResizeCheckBox_Checked_1(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.DisableResizeMode();
		}

		private void ResizeCheckBox_Unchecked_1(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.EnableResizeMode();
		}

		private Thread RadarOverlayThread;

		private RadarOverlay ro;

		private CancellationTokenSource cts;

		private bool disposedValue;
	}
}

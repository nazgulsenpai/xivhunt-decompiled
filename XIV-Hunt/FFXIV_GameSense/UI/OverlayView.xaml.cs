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
	// Token: 0x020000B8 RID: 184
	public partial class OverlayView : UserControl, IDisposable
	{
		// Token: 0x060004B8 RID: 1208 RVA: 0x00016180 File Offset: 0x00014380
		public OverlayView()
		{
			this.InitializeComponent();
			this.RadarEntityScaleTextBox.TextChanged += this.RadarEntityScaleTextBox_TextChanged;
			this.RadarEntityOpacityTextBox.TextChanged += this.RadarEntityOpacityTextBox_TextChanged;
			this.RadarMaxFrameRateTextBox.TextChanged += this.RadarMaxFrameRateTextBox_TextChanged;
			this.RadarBGOpacityTextBox.TextChanged += this.RadarBGOpacityTextBox_TextChanged;
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x000161F8 File Offset: 0x000143F8
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

		// Token: 0x060004BA RID: 1210 RVA: 0x00016278 File Offset: 0x00014478
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

		// Token: 0x060004BB RID: 1211 RVA: 0x000162E4 File Offset: 0x000144E4
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

		// Token: 0x060004BC RID: 1212 RVA: 0x00016360 File Offset: 0x00014560
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

		// Token: 0x060004BD RID: 1213 RVA: 0x000163E4 File Offset: 0x000145E4
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

		// Token: 0x060004BE RID: 1214 RVA: 0x000164A4 File Offset: 0x000146A4
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

		// Token: 0x060004BF RID: 1215 RVA: 0x00016527 File Offset: 0x00014727
		public void Dispose()
		{
			this.Dispose(true);
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x00016530 File Offset: 0x00014730
		private void ClickthruCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.MakeClickthru();
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x00016542 File Offset: 0x00014742
		private void ClickthruCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.MakeClickable();
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x00016554 File Offset: 0x00014754
		private void ResizeCheckBox_Checked_1(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.DisableResizeMode();
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00016566 File Offset: 0x00014766
		private void ResizeCheckBox_Unchecked_1(object sender, RoutedEventArgs e)
		{
			RadarOverlay radarOverlay = this.ro;
			if (radarOverlay == null)
			{
				return;
			}
			radarOverlay.EnableResizeMode();
		}

		// Token: 0x040003A3 RID: 931
		private Thread RadarOverlayThread;

		// Token: 0x040003A4 RID: 932
		private RadarOverlay ro;

		// Token: 0x040003A5 RID: 933
		private CancellationTokenSource cts;

		// Token: 0x040003A6 RID: 934
		private bool disposedValue;
	}
}

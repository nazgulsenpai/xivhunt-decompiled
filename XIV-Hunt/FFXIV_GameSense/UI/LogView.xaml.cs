using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using FFXIV_GameSense.Properties;
using Splat;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000B5 RID: 181
	public partial class LogView : Window
	{
		// Token: 0x060004AC RID: 1196 RVA: 0x00015E38 File Offset: 0x00014038
		public LogView()
		{
			this.InitializeComponent();
			ComboBoxItem[] ComboBoxItems = new ComboBoxItem[this.LogLevelSelectComboBox.Items.Count];
			this.LogLevelSelectComboBox.Items.CopyTo(ComboBoxItems, 0);
			this.LogLevelColors = ComboBoxItems.ToDictionary((ComboBoxItem x) => (LogLevel)Convert.ToInt32(x.Tag, CultureInfo.CurrentCulture), (ComboBoxItem y) => y.Foreground);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00015EC4 File Offset: 0x000140C4
		public void AddLogLine(string text, LogLevel level)
		{
			if (level > (LogLevel)Settings.Default.LogLevel)
			{
				this.LogViewRTB.Dispatcher.Invoke(delegate()
				{
					while (this.LogViewRTB.Document.Blocks.Count > 255)
					{
						this.LogViewRTB.Document.Blocks.Remove(this.LogViewRTB.Document.Blocks.FirstBlock);
					}
					bool scrollToEnd = this.IsVerticalScrollOnBottom();
					TextRange tr = new TextRange(this.LogViewRTB.Document.ContentEnd, this.LogViewRTB.Document.ContentEnd);
					text = string.Concat(new string[]
					{
						DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture),
						" ",
						level.ToString(),
						" ",
						text,
						Environment.NewLine
					});
					try
					{
						tr.Text = text;
						tr.ApplyPropertyValue(TextElement.ForegroundProperty, this.LogLevelColors[level]);
					}
					catch (Exception)
					{
					}
					if (scrollToEnd)
					{
						this.LogViewRTB.ScrollToEnd();
					}
				});
			}
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00015F1A File Offset: 0x0001411A
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			base.Visibility = Visibility.Hidden;
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00015F2C File Offset: 0x0001412C
		private bool IsVerticalScrollOnBottom()
		{
			double dVer = this.LogViewRTB.VerticalOffset;
			double dViewport = this.LogViewRTB.ViewportHeight;
			double dExtent = this.LogViewRTB.ExtentHeight;
			return dVer != 0.0 && dVer + dViewport == dExtent;
		}

		// Token: 0x04000399 RID: 921
		private readonly Dictionary<LogLevel, Brush> LogLevelColors;
	}
}

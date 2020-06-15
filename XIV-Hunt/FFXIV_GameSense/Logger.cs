using System;
using System.ComponentModel;
using FFXIV_GameSense.UI;
using Splat;

namespace FFXIV_GameSense
{
	// Token: 0x020000A6 RID: 166
	public class Logger : ILogger
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000443 RID: 1091 RVA: 0x000144DB File Offset: 0x000126DB
		// (set) Token: 0x06000444 RID: 1092 RVA: 0x000144E3 File Offset: 0x000126E3
		public LogLevel Level { get; set; } = LogLevel.Info;

		// Token: 0x06000445 RID: 1093 RVA: 0x000144EC File Offset: 0x000126EC
		public Logger(LogView lv)
		{
			this.LogView = lv;
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00014502 File Offset: 0x00012702
		public void Write(string message, LogLevel level)
		{
			this.LogView.AddLogLine(message.Remove(0, "LogHost".Length), level);
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00014521 File Offset: 0x00012721
		public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000365 RID: 869
		private readonly LogView LogView;
	}
}

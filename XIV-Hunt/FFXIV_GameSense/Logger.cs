using System;
using System.ComponentModel;
using FFXIV_GameSense.UI;
using Splat;

namespace FFXIV_GameSense
{
	public class Logger : ILogger
	{
		public LogLevel Level { get; set; } = LogLevel.Info;

		public Logger(LogView lv)
		{
			this.LogView = lv;
		}

		public void Write(string message, LogLevel level)
		{
			this.LogView.AddLogLine(message.Remove(0, "LogHost".Length), level);
		}

		public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
		{
			throw new NotImplementedException();
		}

		private readonly LogView LogView;
	}
}

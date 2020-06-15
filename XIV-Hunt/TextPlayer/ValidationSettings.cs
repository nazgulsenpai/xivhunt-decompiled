using System;
using FFXIV_GameSense.Properties;

namespace TextPlayer
{
	public abstract class ValidationSettings
	{
		public ValidationSettings()
		{
		}

		public int MaxSize { get; set; } = Settings.Default.MMLMaxSizeBytes;

		public TimeSpan MaxDuration { get; set; } = Settings.Default.MMLMaxDuration;

		public byte MinTempo { get; set; } = 32;

		public byte MaxTempo { get; set; } = byte.MaxValue;

		public byte MinOctave { get; set; } = 1;

		public byte MaxOctave { get; set; } = 10;
	}
}

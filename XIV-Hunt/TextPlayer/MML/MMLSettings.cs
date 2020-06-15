using System;

namespace TextPlayer.MML
{
	public class MMLSettings : ValidationSettings
	{
		public int MinVolume { get; set; } = 1;

		public int MaxVolume { get; set; } = 15;
	}
}

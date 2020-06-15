using System;

namespace FFXIV_GameSense
{
	internal class ContentFinder
	{
		public ContentFinderState State { get; set; }

		public byte RouletteID { get; set; }

		public ushort ContentFinderConditionID { get; set; }

		public bool IsDutyRouletteQueued()
		{
			return (this.RouletteID > 0 && this.RouletteID < 10) || this.RouletteID == 15 || this.RouletteID >= 17;
		}
	}
}

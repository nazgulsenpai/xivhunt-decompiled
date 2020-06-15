using System;

namespace FFXIV_GameSense
{
	internal class ContentFinderOffsets
	{
		internal int StateOffset { get; private set; }

		internal int RouletteIdOffset { get; private set; }

		public ContentFinderOffsets(GameRegion region)
		{
			if (region == GameRegion.Global)
			{
				this.StateOffset = 81;
				this.RouletteIdOffset = 86;
				return;
			}
			this.StateOffset = 113;
			this.RouletteIdOffset = 118;
		}
	}
}

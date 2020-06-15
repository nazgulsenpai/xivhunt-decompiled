using System;

namespace FFXIV_GameSense
{
	// Token: 0x0200005D RID: 93
	internal class ContentFinderOffsets
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0000DF0A File Offset: 0x0000C10A
		// (set) Token: 0x06000287 RID: 647 RVA: 0x0000DF12 File Offset: 0x0000C112
		internal int StateOffset { get; private set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000288 RID: 648 RVA: 0x0000DF1B File Offset: 0x0000C11B
		// (set) Token: 0x06000289 RID: 649 RVA: 0x0000DF23 File Offset: 0x0000C123
		internal int RouletteIdOffset { get; private set; }

		// Token: 0x0600028A RID: 650 RVA: 0x0000DF2C File Offset: 0x0000C12C
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

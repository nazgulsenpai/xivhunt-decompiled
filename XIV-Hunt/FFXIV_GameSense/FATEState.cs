using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000074 RID: 116
	public enum FATEState : byte
	{
		// Token: 0x0400023A RID: 570
		Running = 2,
		// Token: 0x0400023B RID: 571
		Ended = 4,
		// Token: 0x0400023C RID: 572
		Failed,
		// Token: 0x0400023D RID: 573
		Preparation = 7,
		// Token: 0x0400023E RID: 574
		WaitingForEnd
	}
}

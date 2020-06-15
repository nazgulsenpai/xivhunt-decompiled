using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000048 RID: 72
	public class DataCenterInstanceMatchInfo
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000A882 File Offset: 0x00008A82
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000A88A File Offset: 0x00008A8A
		public uint ID { get; set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x0000A893 File Offset: 0x00008A93
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x0000A89B File Offset: 0x00008A9B
		public DateTime StartTime { get; set; }
	}
}

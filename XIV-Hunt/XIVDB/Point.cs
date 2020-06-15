using System;

namespace XIVDB
{
	// Token: 0x02000006 RID: 6
	public class Point
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002112 File Offset: 0x00000312
		// (set) Token: 0x06000014 RID: 20 RVA: 0x0000211A File Offset: 0x0000031A
		public ushort Map_id { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002123 File Offset: 0x00000323
		// (set) Token: 0x06000016 RID: 22 RVA: 0x0000212B File Offset: 0x0000032B
		public ushort Placename_id { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002134 File Offset: 0x00000334
		// (set) Token: 0x06000018 RID: 24 RVA: 0x0000213C File Offset: 0x0000033C
		public AppPosition App_position { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002145 File Offset: 0x00000345
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000214D File Offset: 0x0000034D
		public AppData App_data { get; set; }
	}
}

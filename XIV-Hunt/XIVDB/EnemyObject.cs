using System;

namespace XIVDB
{
	// Token: 0x02000003 RID: 3
	public class EnemyObject
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020C6 File Offset: 0x000002C6
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000020CE File Offset: 0x000002CE
		public ushort Id { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020D7 File Offset: 0x000002D7
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020DF File Offset: 0x000002DF
		public Map_Data Map_data { get; set; }
	}
}

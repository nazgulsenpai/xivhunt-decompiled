using System;

namespace XIVAPI
{
	// Token: 0x02000014 RID: 20
	public class Item
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008C RID: 140 RVA: 0x000035A2 File Offset: 0x000017A2
		// (set) Token: 0x0600008D RID: 141 RVA: 0x000035AA File Offset: 0x000017AA
		public string Name { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008E RID: 142 RVA: 0x000035B3 File Offset: 0x000017B3
		// (set) Token: 0x0600008F RID: 143 RVA: 0x000035BB File Offset: 0x000017BB
		public byte Rarity { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000090 RID: 144 RVA: 0x000035C4 File Offset: 0x000017C4
		// (set) Token: 0x06000091 RID: 145 RVA: 0x000035CC File Offset: 0x000017CC
		public uint ID { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000092 RID: 146 RVA: 0x000035D5 File Offset: 0x000017D5
		// (set) Token: 0x06000093 RID: 147 RVA: 0x000035DD File Offset: 0x000017DD
		public bool CanBeHq { get; set; }
	}
}

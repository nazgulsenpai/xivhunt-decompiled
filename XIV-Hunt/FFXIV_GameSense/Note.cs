using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000099 RID: 153
	internal class Note
	{
		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x0001265A File Offset: 0x0001085A
		// (set) Token: 0x060003E1 RID: 993 RVA: 0x00012662 File Offset: 0x00010862
		public byte Id { get; set; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x0001266B File Offset: 0x0001086B
		// (set) Token: 0x060003E3 RID: 995 RVA: 0x00012673 File Offset: 0x00010873
		public string Name { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x0001267C File Offset: 0x0001087C
		// (set) Token: 0x060003E5 RID: 997 RVA: 0x00012684 File Offset: 0x00010884
		public uint Wait { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x0001268D File Offset: 0x0001088D
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x00012695 File Offset: 0x00010895
		public uint Length { get; set; }
	}
}

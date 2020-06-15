using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000066 RID: 102
	public class Monster : Combatant
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0000E861 File Offset: 0x0000CA61
		// (set) Token: 0x06000304 RID: 772 RVA: 0x0000E869 File Offset: 0x0000CA69
		public ushort BNpcNameID { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000E872 File Offset: 0x0000CA72
		// (set) Token: 0x06000306 RID: 774 RVA: 0x0000E87A File Offset: 0x0000CA7A
		public uint FateID { get; set; }
	}
}

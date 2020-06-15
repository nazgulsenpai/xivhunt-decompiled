using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000067 RID: 103
	public class EObject : Entity
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000308 RID: 776 RVA: 0x0000E883 File Offset: 0x0000CA83
		// (set) Token: 0x06000309 RID: 777 RVA: 0x0000E88B File Offset: 0x0000CA8B
		public EObjType SubType { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0000E894 File Offset: 0x0000CA94
		// (set) Token: 0x0600030B RID: 779 RVA: 0x0000E89C File Offset: 0x0000CA9C
		public bool CairnIsUnlocked { get; set; }
	}
}

using System;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000AF RID: 175
	public class PerformanceListViewItem
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x00014AFE File Offset: 0x00012CFE
		// (set) Token: 0x0600047C RID: 1148 RVA: 0x00014B06 File Offset: 0x00012D06
		public string RelativePath { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600047D RID: 1149 RVA: 0x00014B0F File Offset: 0x00012D0F
		// (set) Token: 0x0600047E RID: 1150 RVA: 0x00014B17 File Offset: 0x00012D17
		public DateTime LastModified { get; set; }
	}
}

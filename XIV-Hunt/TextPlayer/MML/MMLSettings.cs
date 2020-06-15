using System;

namespace TextPlayer.MML
{
	// Token: 0x02000022 RID: 34
	public class MMLSettings : ValidationSettings
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00004B38 File Offset: 0x00002D38
		// (set) Token: 0x06000101 RID: 257 RVA: 0x00004B40 File Offset: 0x00002D40
		public int MinVolume { get; set; } = 1;

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00004B49 File Offset: 0x00002D49
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00004B51 File Offset: 0x00002D51
		public int MaxVolume { get; set; } = 15;
	}
}

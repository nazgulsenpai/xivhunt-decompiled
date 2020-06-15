using System;
using FFXIV_GameSense.Properties;

namespace TextPlayer
{
	// Token: 0x0200001A RID: 26
	public abstract class ValidationSettings
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x00003B5C File Offset: 0x00001D5C
		public ValidationSettings()
		{
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00003BB1 File Offset: 0x00001DB1
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x00003BB9 File Offset: 0x00001DB9
		public int MaxSize { get; set; } = Settings.Default.MMLMaxSizeBytes;

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00003BC2 File Offset: 0x00001DC2
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x00003BCA File Offset: 0x00001DCA
		public TimeSpan MaxDuration { get; set; } = Settings.Default.MMLMaxDuration;

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00003BD3 File Offset: 0x00001DD3
		// (set) Token: 0x060000CA RID: 202 RVA: 0x00003BDB File Offset: 0x00001DDB
		public byte MinTempo { get; set; } = 32;

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00003BE4 File Offset: 0x00001DE4
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00003BEC File Offset: 0x00001DEC
		public byte MaxTempo { get; set; } = byte.MaxValue;

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00003BF5 File Offset: 0x00001DF5
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00003BFD File Offset: 0x00001DFD
		public byte MinOctave { get; set; } = 1;

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00003C06 File Offset: 0x00001E06
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00003C0E File Offset: 0x00001E0E
		public byte MaxOctave { get; set; } = 10;
	}
}

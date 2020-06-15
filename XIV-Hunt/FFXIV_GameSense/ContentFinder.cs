using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000077 RID: 119
	internal class ContentFinder
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000343 RID: 835 RVA: 0x0000EB9F File Offset: 0x0000CD9F
		// (set) Token: 0x06000344 RID: 836 RVA: 0x0000EBA7 File Offset: 0x0000CDA7
		public ContentFinderState State { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000345 RID: 837 RVA: 0x0000EBB0 File Offset: 0x0000CDB0
		// (set) Token: 0x06000346 RID: 838 RVA: 0x0000EBB8 File Offset: 0x0000CDB8
		public byte RouletteID { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0000EBC1 File Offset: 0x0000CDC1
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0000EBC9 File Offset: 0x0000CDC9
		public ushort ContentFinderConditionID { get; set; }

		// Token: 0x06000349 RID: 841 RVA: 0x0000EBD2 File Offset: 0x0000CDD2
		public bool IsDutyRouletteQueued()
		{
			return (this.RouletteID > 0 && this.RouletteID < 10) || this.RouletteID == 15 || this.RouletteID >= 17;
		}
	}
}

using System;
using Newtonsoft.Json;

namespace FFXIV_GameSense
{
	// Token: 0x0200004C RID: 76
	internal class FATEReport : FATE
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000226 RID: 550 RVA: 0x0000B5C1 File Offset: 0x000097C1
		// (set) Token: 0x06000227 RID: 551 RVA: 0x0000B5C9 File Offset: 0x000097C9
		[JsonProperty("wId")]
		public ushort WorldId { get; set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000B5D2 File Offset: 0x000097D2
		// (set) Token: 0x06000229 RID: 553 RVA: 0x0000B5DA File Offset: 0x000097DA
		[JsonProperty("i")]
		public byte Instance { get; set; }

		// Token: 0x0600022A RID: 554 RVA: 0x0000B5E3 File Offset: 0x000097E3
		public FATEReport()
		{
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000B60C File Offset: 0x0000980C
		public FATEReport(FATE fate) : base(fate)
		{
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000B638 File Offset: 0x00009838
		public override bool Equals(object obj)
		{
			FATEReport item = obj as FATEReport;
			if (item == null)
			{
				return false;
			}
			if (this.WorldId > 1000 && this.WorldId < 2000 && (base.ID > 1425 || base.ZoneID == 156 || base.ZoneID == 152))
			{
				return base.ID.Equals(item.ID) && this.WorldId.Equals(item.WorldId) && this.Instance.Equals(item.Instance);
			}
			if (base.IsDataCenterShared())
			{
				return base.ID.Equals(item.ID);
			}
			return base.ID.Equals(item.ID) && this.WorldId.Equals(item.WorldId);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000B720 File Offset: 0x00009920
		public override int GetHashCode()
		{
			if (this.WorldId > 1000 && this.WorldId < 2000 && (base.ID > 1425 || base.ZoneID == 156 || base.ZoneID == 152))
			{
				return base.ID.GetHashCode() ^ this.WorldId.GetHashCode() ^ this.Instance.GetHashCode();
			}
			if (base.IsDataCenterShared())
			{
				return base.ID.GetHashCode();
			}
			return base.ID.GetHashCode() ^ this.WorldId.GetHashCode();
		}

		// Token: 0x0400017A RID: 378
		[JsonIgnore]
		public DateTime lastPutInChat = DateTime.MinValue;

		// Token: 0x0400017B RID: 379
		[JsonIgnore]
		public DateTime lastReportedDead = DateTime.MinValue;

		// Token: 0x0400017C RID: 380
		[JsonIgnore]
		public byte LastReportedProgress = byte.MaxValue;
	}
}

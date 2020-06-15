using System;
using Newtonsoft.Json;

namespace FFXIV_GameSense
{
	internal class FATEReport : FATE
	{
		[JsonProperty("wId")]
		public ushort WorldId { get; set; }

		[JsonProperty("i")]
		public byte Instance { get; set; }

		public FATEReport()
		{
		}

		public FATEReport(FATE fate) : base(fate)
		{
		}

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

		[JsonIgnore]
		public DateTime lastPutInChat = DateTime.MinValue;

		[JsonIgnore]
		public DateTime lastReportedDead = DateTime.MinValue;

		[JsonIgnore]
		public byte LastReportedProgress = byte.MaxValue;
	}
}

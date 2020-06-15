using System;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	public class FATE
	{
		[JsonProperty("id")]
		public ushort ID { get; set; }

		[JsonIgnore]
		public string ReadName { get; set; }

		public string Name(bool stripTags = false)
		{
			if (!stripTags)
			{
				return this.FATEInfo.NameWithTags;
			}
			return this.FATEInfo.Name;
		}

		public byte Progress { get; set; }

		[JsonProperty("x")]
		public float PosX { get; set; }

		[JsonIgnore]
		public float PosZ { get; set; }

		[JsonProperty("y")]
		public float PosY { get; set; }

		public ushort ZoneID { get; set; }

		public FATEState State { get; set; }

		public uint StartTimeEpoch { get; set; }

		public ushort Duration { get; set; }

		public DateTime LastReported { get; set; }

		[JsonIgnore]
		public DateTime EndTime
		{
			get
			{
				return DateTimeOffset.FromUnixTimeSeconds((long)((ulong)(this.StartTimeEpoch + (uint)this.Duration))).UtcDateTime;
			}
		}

		[JsonIgnore]
		public TimeSpan TimeRemaining
		{
			get
			{
				return this.EndTime - Program.mem.GetServerUtcTime();
			}
		}

		[JsonIgnore]
		public bool HasEnded
		{
			get
			{
				return this.State == FATEState.Ended || this.State == FATEState.Failed;
			}
		}

		[JsonIgnore]
		public FATEInfo FATEInfo
		{
			get
			{
				return GameResources.GetFATEInfo(this.ID);
			}
		}

		public FATE()
		{
		}

		public FATE(FATE fate)
		{
			if (fate == null)
			{
				throw new ArgumentNullException("fate");
			}
			this.ID = fate.ID;
			this.ReadName = fate.ReadName;
			this.Progress = fate.Progress;
			this.PosX = fate.PosX;
			this.PosZ = fate.PosZ;
			this.PosY = fate.PosY;
			this.ZoneID = fate.ZoneID;
			this.State = fate.State;
			this.StartTimeEpoch = fate.StartTimeEpoch;
			this.Duration = fate.Duration;
		}

		public override bool Equals(object obj)
		{
			FATE f = obj as FATE;
			return f != null && obj != null && this.ID == f.ID;
		}

		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		public bool IsDataCenterShared()
		{
			if (this.ID <= 962 || this.ID >= 1101)
			{
				FATEInfo fateinfo = this.FATEInfo;
				return fateinfo != null && fateinfo.EurekaFate;
			}
			return true;
		}
	}
}

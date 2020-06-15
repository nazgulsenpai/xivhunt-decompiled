using System;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x02000073 RID: 115
	public class FATE
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000318 RID: 792 RVA: 0x0000E8AD File Offset: 0x0000CAAD
		// (set) Token: 0x06000319 RID: 793 RVA: 0x0000E8B5 File Offset: 0x0000CAB5
		[JsonProperty("id")]
		public ushort ID { get; set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0000E8BE File Offset: 0x0000CABE
		// (set) Token: 0x0600031B RID: 795 RVA: 0x0000E8C6 File Offset: 0x0000CAC6
		[JsonIgnore]
		public string ReadName { get; set; }

		// Token: 0x0600031C RID: 796 RVA: 0x0000E8CF File Offset: 0x0000CACF
		public string Name(bool stripTags = false)
		{
			if (!stripTags)
			{
				return this.FATEInfo.NameWithTags;
			}
			return this.FATEInfo.Name;
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000E8EB File Offset: 0x0000CAEB
		// (set) Token: 0x0600031E RID: 798 RVA: 0x0000E8F3 File Offset: 0x0000CAF3
		public byte Progress { get; set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000E8FC File Offset: 0x0000CAFC
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000E904 File Offset: 0x0000CB04
		[JsonProperty("x")]
		public float PosX { get; set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000E90D File Offset: 0x0000CB0D
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000E915 File Offset: 0x0000CB15
		[JsonIgnore]
		public float PosZ { get; set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0000E91E File Offset: 0x0000CB1E
		// (set) Token: 0x06000324 RID: 804 RVA: 0x0000E926 File Offset: 0x0000CB26
		[JsonProperty("y")]
		public float PosY { get; set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0000E92F File Offset: 0x0000CB2F
		// (set) Token: 0x06000326 RID: 806 RVA: 0x0000E937 File Offset: 0x0000CB37
		public ushort ZoneID { get; set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000E940 File Offset: 0x0000CB40
		// (set) Token: 0x06000328 RID: 808 RVA: 0x0000E948 File Offset: 0x0000CB48
		public FATEState State { get; set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000E951 File Offset: 0x0000CB51
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0000E959 File Offset: 0x0000CB59
		public uint StartTimeEpoch { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000E962 File Offset: 0x0000CB62
		// (set) Token: 0x0600032C RID: 812 RVA: 0x0000E96A File Offset: 0x0000CB6A
		public ushort Duration { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600032D RID: 813 RVA: 0x0000E973 File Offset: 0x0000CB73
		// (set) Token: 0x0600032E RID: 814 RVA: 0x0000E97B File Offset: 0x0000CB7B
		public DateTime LastReported { get; set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600032F RID: 815 RVA: 0x0000E984 File Offset: 0x0000CB84
		[JsonIgnore]
		public DateTime EndTime
		{
			get
			{
				return DateTimeOffset.FromUnixTimeSeconds((long)((ulong)(this.StartTimeEpoch + (uint)this.Duration))).UtcDateTime;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000330 RID: 816 RVA: 0x0000E9AC File Offset: 0x0000CBAC
		[JsonIgnore]
		public TimeSpan TimeRemaining
		{
			get
			{
				return this.EndTime - Program.mem.GetServerUtcTime();
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000331 RID: 817 RVA: 0x0000E9C3 File Offset: 0x0000CBC3
		[JsonIgnore]
		public bool HasEnded
		{
			get
			{
				return this.State == FATEState.Ended || this.State == FATEState.Failed;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000332 RID: 818 RVA: 0x0000E9D9 File Offset: 0x0000CBD9
		[JsonIgnore]
		public FATEInfo FATEInfo
		{
			get
			{
				return GameResources.GetFATEInfo(this.ID);
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x000020E8 File Offset: 0x000002E8
		public FATE()
		{
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000E9E8 File Offset: 0x0000CBE8
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

		// Token: 0x06000335 RID: 821 RVA: 0x0000EA84 File Offset: 0x0000CC84
		public override bool Equals(object obj)
		{
			FATE f = obj as FATE;
			return f != null && obj != null && this.ID == f.ID;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000EAB0 File Offset: 0x0000CCB0
		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000EACB File Offset: 0x0000CCCB
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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x02000049 RID: 73
	internal class Hunt
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001EB RID: 491 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		// (set) Token: 0x060001EC RID: 492 RVA: 0x0000A8AC File Offset: 0x00008AAC
		[JsonProperty("wId")]
		internal ushort WorldId { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000A8B5 File Offset: 0x00008AB5
		// (set) Token: 0x060001EE RID: 494 RVA: 0x0000A8BD File Offset: 0x00008ABD
		[JsonProperty]
		internal ushort Id { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000A8C6 File Offset: 0x00008AC6
		[JsonProperty("r")]
		internal HuntRank Rank
		{
			get
			{
				return Hunt.RankMap[this.Id];
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x0000A8D8 File Offset: 0x00008AD8
		// (set) Token: 0x060001F1 RID: 497 RVA: 0x0000A8E0 File Offset: 0x00008AE0
		[JsonProperty]
		internal DateTime LastReported { get; set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000A8E9 File Offset: 0x00008AE9
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x0000A8F1 File Offset: 0x00008AF1
		[JsonProperty("i")]
		internal byte Instance { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001F4 RID: 500 RVA: 0x0000A8FA File Offset: 0x00008AFA
		// (set) Token: 0x060001F5 RID: 501 RVA: 0x0000A902 File Offset: 0x00008B02
		[JsonProperty("x")]
		internal float LastX { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0000A90B File Offset: 0x00008B0B
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x0000A913 File Offset: 0x00008B13
		[JsonProperty("y")]
		internal float LastY { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x0000A91C File Offset: 0x00008B1C
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x0000A924 File Offset: 0x00008B24
		[JsonProperty]
		internal bool LastAlive { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000A92D File Offset: 0x00008B2D
		// (set) Token: 0x060001FB RID: 507 RVA: 0x0000A935 File Offset: 0x00008B35
		[JsonProperty]
		internal uint OccurrenceID { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001FC RID: 508 RVA: 0x0000A93E File Offset: 0x00008B3E
		[JsonIgnore]
		internal string WorldName
		{
			get
			{
				return GameResources.GetWorldName(this.WorldId);
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001FD RID: 509 RVA: 0x0000A94B File Offset: 0x00008B4B
		[JsonIgnore]
		internal string Name
		{
			get
			{
				return GameResources.GetEnemyName((uint)this.Id, false);
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000A95C File Offset: 0x00008B5C
		private static Dictionary<ushort, HuntRank> IdexRanks()
		{
			Dictionary<ushort, HuntRank> r = new Dictionary<ushort, HuntRank>
			{
				{
					8653,
					HuntRank.S
				},
				{
					8654,
					HuntRank.A
				},
				{
					8655,
					HuntRank.A
				},
				{
					8656,
					HuntRank.B
				},
				{
					8657,
					HuntRank.B
				},
				{
					8890,
					HuntRank.S
				},
				{
					8891,
					HuntRank.A
				},
				{
					8892,
					HuntRank.A
				},
				{
					8893,
					HuntRank.B
				},
				{
					8894,
					HuntRank.B
				},
				{
					8895,
					HuntRank.S
				},
				{
					8896,
					HuntRank.A
				},
				{
					8897,
					HuntRank.A
				},
				{
					8898,
					HuntRank.B
				},
				{
					8899,
					HuntRank.B
				},
				{
					8900,
					HuntRank.S
				},
				{
					8901,
					HuntRank.A
				},
				{
					8902,
					HuntRank.A
				},
				{
					8903,
					HuntRank.B
				},
				{
					8904,
					HuntRank.B
				},
				{
					8905,
					HuntRank.S
				},
				{
					8906,
					HuntRank.A
				},
				{
					8907,
					HuntRank.A
				},
				{
					8908,
					HuntRank.B
				},
				{
					8909,
					HuntRank.B
				},
				{
					8910,
					HuntRank.S
				},
				{
					8911,
					HuntRank.A
				},
				{
					8912,
					HuntRank.A
				},
				{
					8913,
					HuntRank.B
				},
				{
					8914,
					HuntRank.B
				}
			};
			for (ushort i = 6002; i < 6014; i += 1)
			{
				r.Add(i, HuntRank.B);
			}
			for (ushort j = 5990; j < 6002; j += 1)
			{
				r.Add(j, HuntRank.A);
			}
			for (ushort k = 5984; k < 5990; k += 1)
			{
				r.Add(k, HuntRank.S);
			}
			for (ushort l = 4350; l < 4362; l += 1)
			{
				r.Add(l, HuntRank.B);
			}
			for (ushort m = 4362; m < 4374; m += 1)
			{
				r.Add(m, HuntRank.A);
			}
			for (ushort n = 4374; n < 4381; n += 1)
			{
				r.Add(n, HuntRank.S);
			}
			r.Remove(4379);
			for (ushort i2 = 2919; i2 < 2936; i2 += 1)
			{
				r.Add(i2, HuntRank.B);
			}
			for (ushort i3 = 2936; i3 < 2953; i3 += 1)
			{
				r.Add(i3, HuntRank.A);
			}
			for (ushort i4 = 2953; i4 < 2970; i4 += 1)
			{
				r.Add(i4, HuntRank.S);
			}
			return r;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000AC07 File Offset: 0x00008E07
		public Hunt()
		{
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000AC25 File Offset: 0x00008E25
		internal Hunt(ushort _id, ushort wid)
		{
			this.WorldId = wid;
			this.Id = _id;
			this.LastReported = DateTime.MinValue;
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000AC5C File Offset: 0x00008E5C
		internal bool IsARR
		{
			get
			{
				return this.Id < 3000;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000AC6B File Offset: 0x00008E6B
		internal bool IsHW
		{
			get
			{
				return this.Id > 3000 && this.Id < 5000;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000203 RID: 515 RVA: 0x0000AC89 File Offset: 0x00008E89
		internal bool IsSB
		{
			get
			{
				return this.Id > 5000 && this.Id < 8140;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000ACA7 File Offset: 0x00008EA7
		internal bool IsSHB
		{
			get
			{
				return this.Id > 8140;
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000ACB8 File Offset: 0x00008EB8
		public override bool Equals(object obj)
		{
			Hunt item = obj as Hunt;
			if (item == null)
			{
				return false;
			}
			if (this.WorldId > 1000 && this.WorldId < 2000 && (this.IsSHB || FFXIVHunts.GetZoneId(this.Id) == 156 || FFXIVHunts.GetZoneId(this.Id) == 152))
			{
				return this.Id.Equals(item.Id) && this.WorldId.Equals(item.WorldId) && this.Instance.Equals(item.Instance);
			}
			return this.Id.Equals(item.Id) && this.WorldId.Equals(item.WorldId);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000AD88 File Offset: 0x00008F88
		public override int GetHashCode()
		{
			if (this.WorldId > 1000 && this.WorldId < 2000 && (this.IsSHB || FFXIVHunts.GetZoneId(this.Id) == 156 || FFXIVHunts.GetZoneId(this.Id) == 152))
			{
				return this.Id.GetHashCode() ^ this.WorldId.GetHashCode() ^ this.Instance.GetHashCode();
			}
			return this.Id.GetHashCode() ^ this.WorldId.GetHashCode();
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000AE25 File Offset: 0x00009025
		internal static bool TryGetHuntRank(ushort HuntID, out HuntRank hr)
		{
			return Hunt.RankMap.TryGetValue(HuntID, out hr);
		}

		// Token: 0x04000163 RID: 355
		internal DateTime lastPutInChat = DateTime.MinValue;

		// Token: 0x04000164 RID: 356
		internal DateTime lastReportedDead = DateTime.MinValue;

		// Token: 0x04000169 RID: 361
		[JsonIgnore]
		internal static readonly Dictionary<ushort, HuntRank> RankMap = Hunt.IdexRanks();
	}
}

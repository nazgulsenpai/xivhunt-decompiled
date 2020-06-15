using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	internal class Hunt
	{
		[JsonProperty("wId")]
		internal ushort WorldId { get; set; }

		[JsonProperty]
		internal ushort Id { get; set; }

		[JsonProperty("r")]
		internal HuntRank Rank
		{
			get
			{
				return Hunt.RankMap[this.Id];
			}
		}

		[JsonProperty]
		internal DateTime LastReported { get; set; }

		[JsonProperty("i")]
		internal byte Instance { get; set; }

		[JsonProperty("x")]
		internal float LastX { get; set; }

		[JsonProperty("y")]
		internal float LastY { get; set; }

		[JsonProperty]
		internal bool LastAlive { get; set; }

		[JsonProperty]
		internal uint OccurrenceID { get; set; }

		[JsonIgnore]
		internal string WorldName
		{
			get
			{
				return GameResources.GetWorldName(this.WorldId);
			}
		}

		[JsonIgnore]
		internal string Name
		{
			get
			{
				return GameResources.GetEnemyName((uint)this.Id, false);
			}
		}

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

		public Hunt()
		{
		}

		internal Hunt(ushort _id, ushort wid)
		{
			this.WorldId = wid;
			this.Id = _id;
			this.LastReported = DateTime.MinValue;
		}

		internal bool IsARR
		{
			get
			{
				return this.Id < 3000;
			}
		}

		internal bool IsHW
		{
			get
			{
				return this.Id > 3000 && this.Id < 5000;
			}
		}

		internal bool IsSB
		{
			get
			{
				return this.Id > 5000 && this.Id < 8140;
			}
		}

		internal bool IsSHB
		{
			get
			{
				return this.Id > 8140;
			}
		}

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

		public override int GetHashCode()
		{
			if (this.WorldId > 1000 && this.WorldId < 2000 && (this.IsSHB || FFXIVHunts.GetZoneId(this.Id) == 156 || FFXIVHunts.GetZoneId(this.Id) == 152))
			{
				return this.Id.GetHashCode() ^ this.WorldId.GetHashCode() ^ this.Instance.GetHashCode();
			}
			return this.Id.GetHashCode() ^ this.WorldId.GetHashCode();
		}

		internal static bool TryGetHuntRank(ushort HuntID, out HuntRank hr)
		{
			return Hunt.RankMap.TryGetValue(HuntID, out hr);
		}

		internal DateTime lastPutInChat = DateTime.MinValue;

		internal DateTime lastReportedDead = DateTime.MinValue;

		[JsonIgnore]
		internal static readonly Dictionary<ushort, HuntRank> RankMap = Hunt.IdexRanks();
	}
}

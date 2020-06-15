using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x0200004D RID: 77
	[JsonObject]
	internal class World
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600022E RID: 558 RVA: 0x0000B7CF File Offset: 0x000099CF
		// (set) Token: 0x0600022F RID: 559 RVA: 0x0000B7D7 File Offset: 0x000099D7
		[JsonProperty("id")]
		internal ushort Id { get; set; }

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000230 RID: 560 RVA: 0x0000B7E0 File Offset: 0x000099E0
		// (set) Token: 0x06000231 RID: 561 RVA: 0x0000B7E8 File Offset: 0x000099E8
		[JsonProperty("hunts")]
		internal List<Hunt> Hunts { get; set; } = new List<Hunt>();

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000232 RID: 562 RVA: 0x0000B7F1 File Offset: 0x000099F1
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000B7F9 File Offset: 0x000099F9
		internal List<FATEReport> FATEs { get; set; } = new List<FATEReport>();

		// Token: 0x06000234 RID: 564 RVA: 0x0000B804 File Offset: 0x00009A04
		public World(ushort wid)
		{
			this.Id = wid;
			if (GameResources.IsKoreanWorld(wid))
			{
				this.FATEs = (from x in GameResources.GetFates()
				select new FATEReport(x)
				{
					WorldId = this.Id
				}).ToList<FATEReport>();
				return;
			}
			foreach (FATE f in GameResources.GetFates())
			{
				if (f.ID > 1425)
				{
					for (byte i = 1; i <= 3; i += 1)
					{
						this.FATEs.Add(new FATEReport(f)
						{
							WorldId = this.Id,
							Instance = i
						});
					}
				}
				else
				{
					this.FATEs.Add(new FATEReport(f)
					{
						WorldId = this.Id
					});
				}
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000B8FC File Offset: 0x00009AFC
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000B918 File Offset: 0x00009B18
		public override bool Equals(object obj)
		{
			World w = (World)obj;
			return w != null && this.Id == w.Id;
		}
	}
}

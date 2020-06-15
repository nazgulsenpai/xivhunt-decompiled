using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using XIVDB;

namespace FFXIV_GameSense
{
	[JsonObject]
	internal class World
	{
		[JsonProperty("id")]
		internal ushort Id { get; set; }

		[JsonProperty("hunts")]
		internal List<Hunt> Hunts { get; set; } = new List<Hunt>();

		internal List<FATEReport> FATEs { get; set; } = new List<FATEReport>();

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

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			World w = (World)obj;
			return w != null && this.Id == w.Id;
		}
	}
}

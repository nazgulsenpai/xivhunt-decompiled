using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using FFXIV_GameSense;
using FFXIV_GameSense.Properties;

namespace XIVDB
{
	// Token: 0x0200000D RID: 13
	internal static class GameResources
	{
		// Token: 0x06000040 RID: 64 RVA: 0x00002410 File Offset: 0x00000610
		internal static IEnumerable<RelicNote> GetRelicNotes()
		{
			CsvParser csv = new CsvParser(Resources.RelicNote);
			while (csv.Advance())
			{
				yield return new RelicNote(csv);
			}
			yield break;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000241C File Offset: 0x0000061C
		private static Dictionary<ushort, FATEInfo> IndexFates()
		{
			Dictionary<ushort, FATEInfo> d = new Dictionary<ushort, FATEInfo>();
			CsvParser csv = new CsvParser(Resources.Fate);
			while (csv.Advance())
			{
				if (!string.IsNullOrWhiteSpace(csv["Name"].Trim('"')))
				{
					d.Add(ushort.Parse(csv["#"]), new FATEInfo(csv));
				}
			}
			return d;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000247C File Offset: 0x0000067C
		private static bool ValidWorld(string s)
		{
			string[] col = s.Split(',', StringSplitOptions.None);
			ushort num;
			return ushort.TryParse(col[0], out num) && ((col[4] == "True" && col[3].Trim('"') != "INVALID") || col[3].Trim('"') == "China" || col[3].Trim('"') == "Korea");
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000024F2 File Offset: 0x000006F2
		internal static bool IsValidWorldID(ushort id)
		{
			return GameResources.World.ContainsKey(id);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000024FF File Offset: 0x000006FF
		internal static bool IsChineseWorld(ushort id)
		{
			return id > 1023 && id < 1170;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002513 File Offset: 0x00000713
		internal static bool IsKoreanWorld(ushort id)
		{
			return id > 2074 && id < 2079;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002528 File Offset: 0x00000728
		internal static FATEInfo GetFATEInfo(ushort iD)
		{
			FATEInfo fi;
			if (GameResources.Fate.TryGetValue(iD, out fi))
			{
				return fi;
			}
			return null;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002548 File Offset: 0x00000748
		internal static ushort GetFateId(string name, bool ignoreCase = false)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return 0;
			}
			foreach (KeyValuePair<ushort, FATEInfo> f in GameResources.Fate)
			{
				if (f.Value.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
				{
					return f.Key;
				}
			}
			return 0;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000025C8 File Offset: 0x000007C8
		public static List<FATE> GetFates()
		{
			List<FATE> i = new List<FATE>();
			foreach (KeyValuePair<ushort, FATEInfo> f in GameResources.Fate)
			{
				if (f.Key != 122 && f.Key != 145 && f.Key != 151 && f.Key != 130 && f.Key != 173 && f.Key != 182)
				{
					i.Add(new FATE
					{
						ID = f.Key
					});
				}
			}
			return i;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002684 File Offset: 0x00000884
		public static string GetEnemyName(uint id, bool plural = false)
		{
			string[] lines = Resources.BNpcName.Split(GameResources.lineEnding, StringSplitOptions.None).Skip(3).ToArray<string>();
			if ((ulong)id > (ulong)((long)lines.Length))
			{
				return string.Empty;
			}
			plural = (plural && (!(Thread.CurrentThread.CurrentUICulture.Name == "ja-JP") && !(Thread.CurrentThread.CurrentUICulture.Name == "zh-CN")) && !(Thread.CurrentThread.CurrentUICulture.Name == "ko-KR"));
			string result = lines[(int)id].Split(',', StringSplitOptions.None)[plural ? 3 : 1].Trim('"');
			if (Thread.CurrentThread.CurrentUICulture.Name == "de-DE")
			{
				result = Regex.Replace(result, "\\[(.*?)\\]", string.Empty);
			}
			return Thread.CurrentThread.CurrentUICulture.TextInfo.ToTitleCase(result);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002774 File Offset: 0x00000974
		public static bool GetEnemyId(string Name, out ushort id)
		{
			int i = 0;
			string[] lines = Resources.BNpcName.Split(GameResources.lineEnding, StringSplitOptions.None).Skip(3).ToArray<string>();
			bool noPlural = !(Thread.CurrentThread.CurrentUICulture.Name == "ja-JP") && !(Thread.CurrentThread.CurrentUICulture.Name == "zh-CN") && !(Thread.CurrentThread.CurrentUICulture.Name == "ko-KR");
			while (i < lines.Length - 1)
			{
				string singular = lines[i].Split(',', StringSplitOptions.None)[1].Trim('"');
				string plural = lines[i].Split(',', StringSplitOptions.None)[3].Trim('"');
				if (Thread.CurrentThread.CurrentUICulture.Name == "de-DE")
				{
					singular = Regex.Replace(singular, "\\[(.*?)\\]", string.Empty);
					plural = Regex.Replace(plural, "\\[(.*?)\\]", string.Empty);
				}
				if (!noPlural)
				{
					plural = plural.Replace("<SoftHyphen/>", string.Empty);
				}
				ushort _id;
				if ((singular.Equals(Name, StringComparison.OrdinalIgnoreCase) || plural.Equals(Name, StringComparison.OrdinalIgnoreCase)) && ushort.TryParse(lines[i].Split(',', StringSplitOptions.None)[0], out _id))
				{
					id = _id;
					return true;
				}
				i++;
			}
			id = 0;
			return false;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000028C0 File Offset: 0x00000AC0
		public static string GetWorldName(ushort id)
		{
			World wn;
			if (!GameResources.World.TryGetValue(id, out wn))
			{
				return "Unknown World: " + id.ToString();
			}
			return wn.Name;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000028F4 File Offset: 0x00000AF4
		public static World GetWorld(ushort id)
		{
			World w;
			if (GameResources.World.TryGetValue(id, out w))
			{
				return w;
			}
			return null;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002914 File Offset: 0x00000B14
		internal static string GetZoneName(uint zoneId)
		{
			int i = 0;
			string[] lines = Resources.TerritoryType.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries);
			while (i < lines.Length)
			{
				if (lines[i].Split(',', StringSplitOptions.None)[0] == zoneId.ToString())
				{
					return lines[i].Split(',', StringSplitOptions.None)[6].Trim('"');
				}
				i++;
			}
			return "Unknown zoneID: " + zoneId.ToString();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002984 File Offset: 0x00000B84
		internal static string GetMapCodeName(uint zoneId)
		{
			int i = 0;
			string[] lines = Resources.TerritoryType.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries);
			while (i < lines.Length)
			{
				if (lines[i].Split(',', StringSplitOptions.None)[0] == zoneId.ToString())
				{
					return lines[i].Split(',', StringSplitOptions.None)[1].Trim('"');
				}
				i++;
			}
			return null;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000029E4 File Offset: 0x00000BE4
		internal static ushort GetZoneIdFromCodeName(string name)
		{
			int i = 0;
			string[] lines = Resources.TerritoryType.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries).Skip(3).ToArray<string>();
			while (i < lines.Length)
			{
				if (lines[i].Split(',', StringSplitOptions.None)[1].Trim('"').Equals(name))
				{
					return ushort.Parse(lines[i].Split(',', StringSplitOptions.None)[0]);
				}
				i++;
			}
			return 0;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002A4C File Offset: 0x00000C4C
		public static ushort GetSizeFactor(ushort zoneId, ushort mapId = 0)
		{
			if (mapId == 0)
			{
				mapId = GameResources.GetMapIds(zoneId).Min<ushort>();
			}
			if (GameResources.CachedSizeFactors.ContainsKey(mapId))
			{
				return GameResources.CachedSizeFactors[mapId];
			}
			ushort x;
			if (ushort.TryParse(Resources.Map.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries).Skip(3).ElementAt((int)mapId).Split(',', StringSplitOptions.None)[8], out x))
			{
				GameResources.CachedSizeFactors.Add(mapId, x);
				return x;
			}
			return 0;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002AC0 File Offset: 0x00000CC0
		internal static ushort MapIdToZoneId(uint mapId)
		{
			return GameResources.GetZoneIdFromCodeName(string.Concat<char>(Resources.Map.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries).Skip(3).ToArray<string>()[(int)mapId].Split(',', StringSplitOptions.None)[7].Trim('"').TakeWhile((char c) => c != '/')));
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002B2C File Offset: 0x00000D2C
		internal static byte[] GetMapMarkerZoneId(ushort zoneId, ushort subMapId = 0)
		{
			List<byte> bl = BitConverter.GetBytes(zoneId).Reverse<byte>().ToList<byte>();
			IEnumerable<ushort> mapIds = GameResources.GetMapIds(zoneId);
			if (subMapId != 0 && mapIds.Contains(subMapId))
			{
				bl.AddRange(BitConverter.GetBytes(subMapId).Reverse<byte>().ToList<byte>());
			}
			else
			{
				bl.AddRange(BitConverter.GetBytes(mapIds.Min<ushort>()).Reverse<byte>().ToList<byte>());
			}
			bl.RemoveAll((byte x) => x == 0);
			switch (bl.Count)
			{
			case 2:
				bl.Insert(0, 244);
				break;
			case 3:
				bl.Insert(0, 252);
				break;
			case 4:
				bl.Insert(0, 254);
				break;
			}
			return bl.ToArray();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002C01 File Offset: 0x00000E01
		private static IEnumerable<ushort> GetMapIds(ushort zoneId)
		{
			string[] lines = Resources.Map.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries);
			string name = GameResources.GetMapCodeName((uint)zoneId);
			int num;
			for (int i = 0; i < lines.Length; i = num + 1)
			{
				string[] line = lines[i].Split(',', StringSplitOptions.None);
				ushort x;
				if ((string.Concat<char>(line[7].Skip(1).TakeWhile((char c) => c != '/')) == name || line[16].Trim('"') == name) && ushort.TryParse(line[0], out x))
				{
					yield return x;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002C14 File Offset: 0x00000E14
		private static ushort GetEnemyId(string huntSearchTerm)
		{
			int i = 0;
			string[] lines = Resources.BNpcName.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries);
			while (i < lines.Length)
			{
				ushort id;
				if (lines[i].Split(',', StringSplitOptions.None)[1].Trim('"').Equals(huntSearchTerm, StringComparison.OrdinalIgnoreCase) && ushort.TryParse(lines[i].Split(',', StringSplitOptions.None)[0], out id))
				{
					return id;
				}
				i++;
			}
			return 0;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002C78 File Offset: 0x00000E78
		internal static bool TryGetDailyHuntInfo(string huntSearchTerm, out Tuple<ushort, ushort, float, float> huntInfo)
		{
			huntInfo = new Tuple<ushort, ushort, float, float>(0, 0, 0f, 0f);
			ushort id = GameResources.GetEnemyId(huntSearchTerm.Trim());
			if (id == 0)
			{
				return false;
			}
			int i = 0;
			string[] lines = Resources.DailyHunts.Split(GameResources.lineEnding, StringSplitOptions.None);
			while (i < lines.Length)
			{
				if (lines[i].Split('|', StringSplitOptions.None)[0].Equals(id.ToString()))
				{
					string[] values = lines[i].Split('|', StringSplitOptions.None);
					huntInfo = new Tuple<ushort, ushort, float, float>(id, ushort.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
					return true;
				}
				i++;
			}
			return false;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002D14 File Offset: 0x00000F14
		internal static bool TryGetZoneID(string ZoneName, out ushort ZoneID)
		{
			if (!string.IsNullOrWhiteSpace(ZoneName))
			{
				foreach (string line in Resources.TerritoryType.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries).Skip(3))
				{
					if (line.Split(',', StringSplitOptions.None)[6].Trim('"').Equals(ZoneName, StringComparison.OrdinalIgnoreCase))
					{
						ZoneID = ushort.Parse(line.Split(',', StringSplitOptions.None)[0]);
						return true;
					}
				}
			}
			ZoneID = 0;
			return false;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002DAC File Offset: 0x00000FAC
		internal static IEnumerable<Note> GetPerformanceNotes()
		{
			foreach (string text in Resources.Perform.Split(GameResources.lineEnding, StringSplitOptions.RemoveEmptyEntries).Skip(4))
			{
				string[] np = text.Split(',', StringSplitOptions.None);
				yield return new Note
				{
					Id = byte.Parse(np[0]) + 23,
					Name = np[2].Trim('"')
				};
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x04000019 RID: 25
		private const string SquareBrauquetsRegex = "\\[(.*?)\\]";

		// Token: 0x0400001A RID: 26
		internal const string HtmlTagRegex = "<.*?>";

		// Token: 0x0400001B RID: 27
		private static readonly string[] lineEnding = new string[]
		{
			Environment.NewLine
		};

		// Token: 0x0400001C RID: 28
		private static readonly Dictionary<ushort, FATEInfo> Fate = GameResources.IndexFates();

		// Token: 0x0400001D RID: 29
		private static readonly Dictionary<ushort, ushort> CachedSizeFactors = new Dictionary<ushort, ushort>();

		// Token: 0x0400001E RID: 30
		internal static readonly Dictionary<ushort, World> World = (from line in Resources.World.Split(GameResources.lineEnding, StringSplitOptions.None).Skip(3).Where(new Func<string, bool>(GameResources.ValidWorld))
		select line.Split(',', StringSplitOptions.None)).ToDictionary((string[] line) => ushort.Parse(line[0]), (string[] line) => new World(line));
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XIVDB
{
	public class RelicNote
	{
		public string BookName { get; private set; }

		public List<FATEInfo> FATEs { get; private set; }

		public RelicNote(CsvParser csv)
		{
			this.BookName = csv["EventItem"].Trim('"');
			IEnumerable<KeyValuePair<string, int>> fatecols = from x in csv.Columns
			where x.Key.StartsWith("Fate", StringComparison.OrdinalIgnoreCase)
			select x;
			this.FATEs = new List<FATEInfo>(fatecols.Count<KeyValuePair<string, int>>());
			foreach (KeyValuePair<string, int> col in fatecols)
			{
				this.FATEs.Add(GameResources.GetFATEInfo(GameResources.GetFateId(Regex.Replace(csv[col.Key].Trim('"'), "<.*?>", string.Empty), true)));
			}
		}
	}
}

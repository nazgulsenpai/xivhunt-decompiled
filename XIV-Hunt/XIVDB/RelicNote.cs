using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XIVDB
{
	// Token: 0x0200000B RID: 11
	public class RelicNote
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000038 RID: 56 RVA: 0x000022FA File Offset: 0x000004FA
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002302 File Offset: 0x00000502
		public string BookName { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003A RID: 58 RVA: 0x0000230B File Offset: 0x0000050B
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00002313 File Offset: 0x00000513
		public List<FATEInfo> FATEs { get; private set; }

		// Token: 0x0600003C RID: 60 RVA: 0x0000231C File Offset: 0x0000051C
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

using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_GameSense;

namespace XIVDB
{
	// Token: 0x02000012 RID: 18
	public class CsvParser
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003283 File Offset: 0x00001483
		// (set) Token: 0x0600007B RID: 123 RVA: 0x0000328B File Offset: 0x0000148B
		public Dictionary<string, int> Columns { get; private set; } = new Dictionary<string, int>();

		// Token: 0x0600007C RID: 124 RVA: 0x00003294 File Offset: 0x00001494
		public CsvParser(string csvstring)
		{
			string[] header = csvstring.Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries)[1].Split(',', StringSplitOptions.None);
			int i;
			for (i = 0; i < header.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(header[i]))
				{
					this.Columns.Add(header[i], i);
				}
			}
			this.colCount = i;
			this.Records = this.SplitRecords(csvstring.Substring(csvstring.NthIndexOf("\r\n", 0, 3) + "\r\n".Length));
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000332C File Offset: 0x0000152C
		private List<string[]> SplitRecords(string recordsStr)
		{
			List<string[]> records = new List<string[]>();
			for (int valuestart = 0; valuestart < recordsStr.Length; valuestart++)
			{
				bool inQuote = false;
				string[] record = new string[this.colCount];
				int col = 0;
				for (int chariterator = valuestart + 1; chariterator < recordsStr.Length; chariterator++)
				{
					if (!inQuote && recordsStr[chariterator] == '"')
					{
						inQuote = true;
					}
					else if (inQuote && (recordsStr.Substring(chariterator, 2) == "\"," || recordsStr.Substring(chariterator, 3) == "\"\r\n"))
					{
						inQuote = false;
					}
					if (!inQuote && (recordsStr[chariterator] == ',' || recordsStr.Substring(chariterator, "\r\n".Length) == "\r\n"))
					{
						string[] array = record;
						int num = col;
						int num2 = valuestart;
						int length = chariterator - num2;
						array[num] = recordsStr.Substring(num2, length);
						valuestart = chariterator + 1;
						col++;
						if (col == this.colCount)
						{
							break;
						}
						if (recordsStr.Substring(valuestart, "\r\n".Length) == "\r\n")
						{
							valuestart++;
							break;
						}
					}
				}
				records.Add(record);
			}
			return records;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000344C File Offset: 0x0000164C
		public bool Advance()
		{
			int num = this.recordIterator + 1;
			this.recordIterator = num;
			return num < this.Records.Count;
		}

		// Token: 0x1700001E RID: 30
		public string this[string colname]
		{
			get
			{
				return this.Records.ElementAt(this.recordIterator)[this.Columns[colname]];
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003497 File Offset: 0x00001697
		public bool HasColum(string colname)
		{
			return this.Columns.ContainsKey(colname);
		}

		// Token: 0x04000033 RID: 51
		private const string LineSep = "\r\n";

		// Token: 0x04000034 RID: 52
		private readonly int colCount;

		// Token: 0x04000036 RID: 54
		private readonly List<string[]> Records;

		// Token: 0x04000037 RID: 55
		private int recordIterator;
	}
}

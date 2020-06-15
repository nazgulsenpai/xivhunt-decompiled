using System;
using System.Collections.Generic;
using System.Linq;
using FFXIV_GameSense;

namespace XIVDB
{
	public class CsvParser
	{
		public Dictionary<string, int> Columns { get; private set; } = new Dictionary<string, int>();

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

		public bool Advance()
		{
			int num = this.recordIterator + 1;
			this.recordIterator = num;
			return num < this.Records.Count;
		}

		public string this[string colname]
		{
			get
			{
				return this.Records.ElementAt(this.recordIterator)[this.Columns[colname]];
			}
		}

		public bool HasColum(string colname)
		{
			return this.Columns.ContainsKey(colname);
		}

		private const string LineSep = "\r\n";

		private readonly int colCount;

		private readonly List<string[]> Records;

		private int recordIterator;
	}
}

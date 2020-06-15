using System;
using System.Text.RegularExpressions;

namespace XIVDB
{
	public class FATEInfo
	{
		public ushort ID { get; private set; }

		public byte ClassJobLevel { get; private set; }

		public string Name { get; private set; }

		public string NameWithTags { get; private set; }

		public string IconMap { get; private set; }

		public bool EurekaFate { get; private set; }

		public FATEInfo(CsvParser csv)
		{
			this.ID = ushort.Parse(csv["#"]);
			this.NameWithTags = csv["Name"].Trim(new char[]
			{
				'"',
				' '
			});
			this.ClassJobLevel = byte.Parse(csv["ClassJobLevel"]);
			this.Name = Regex.Replace(this.NameWithTags, "<.*?>", string.Empty);
			this.IconMap = csv["Icon{Map}"].Trim('"').Replace(".tex", ".png");
			if (csv.HasColum("EurekaFate"))
			{
				this.EurekaFate = (csv["EurekaFate"].Trim('"') == "1");
			}
		}
	}
}

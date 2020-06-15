using System;
using System.Text.RegularExpressions;

namespace XIVDB
{
	// Token: 0x0200000A RID: 10
	public class FATEInfo
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000021BC File Offset: 0x000003BC
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000021C4 File Offset: 0x000003C4
		public ushort ID { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000021CD File Offset: 0x000003CD
		// (set) Token: 0x0600002E RID: 46 RVA: 0x000021D5 File Offset: 0x000003D5
		public byte ClassJobLevel { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021DE File Offset: 0x000003DE
		// (set) Token: 0x06000030 RID: 48 RVA: 0x000021E6 File Offset: 0x000003E6
		public string Name { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000031 RID: 49 RVA: 0x000021EF File Offset: 0x000003EF
		// (set) Token: 0x06000032 RID: 50 RVA: 0x000021F7 File Offset: 0x000003F7
		public string NameWithTags { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002200 File Offset: 0x00000400
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002208 File Offset: 0x00000408
		public string IconMap { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002211 File Offset: 0x00000411
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002219 File Offset: 0x00000419
		public bool EurekaFate { get; private set; }

		// Token: 0x06000037 RID: 55 RVA: 0x00002224 File Offset: 0x00000424
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

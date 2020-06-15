using System;
using System.Collections.Generic;
using System.Linq;

namespace XIVDB
{
	// Token: 0x02000013 RID: 19
	public class World
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000034A5 File Offset: 0x000016A5
		// (set) Token: 0x06000082 RID: 130 RVA: 0x000034AD File Offset: 0x000016AD
		public ushort ID { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000083 RID: 131 RVA: 0x000034B6 File Offset: 0x000016B6
		// (set) Token: 0x06000084 RID: 132 RVA: 0x000034BE File Offset: 0x000016BE
		public string Name { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000034C7 File Offset: 0x000016C7
		// (set) Token: 0x06000086 RID: 134 RVA: 0x000034CF File Offset: 0x000016CF
		public string DataCenterName { get; private set; }

		// Token: 0x06000087 RID: 135 RVA: 0x000034D8 File Offset: 0x000016D8
		public World(string[] line)
		{
			if (line == null)
			{
				throw new ArgumentNullException("line", "Line is empty");
			}
			this.ID = ushort.Parse(line[0]);
			this.Name = line[1].Trim('"');
			this.DataCenterName = line[3].Trim('"');
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000352C File Offset: 0x0000172C
		public IEnumerable<World> GetWorldsOnSameDataCenter()
		{
			return from x in GameResources.World.Values
			where x.DataCenterName == this.DataCenterName
			select x;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000354C File Offset: 0x0000174C
		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003568 File Offset: 0x00001768
		public override bool Equals(object obj)
		{
			World w = (World)obj;
			return w != null && this.ID == w.ID;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace XIVDB
{
	public class World
	{
		public ushort ID { get; private set; }

		public string Name { get; private set; }

		public string DataCenterName { get; private set; }

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

		public IEnumerable<World> GetWorldsOnSameDataCenter()
		{
			return from x in GameResources.World.Values
			where x.DataCenterName == this.DataCenterName
			select x;
		}

		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			World w = (World)obj;
			return w != null && this.ID == w.ID;
		}
	}
}

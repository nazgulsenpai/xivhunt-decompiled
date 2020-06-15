using System;

namespace XIVDB
{
	public class Point
	{
		public ushort Map_id { get; set; }

		public ushort Placename_id { get; set; }

		public AppPosition App_position { get; set; }

		public AppData App_data { get; set; }
	}
}

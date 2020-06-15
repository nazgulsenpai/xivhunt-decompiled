using System;

namespace FFXIV_GameSense
{
	internal class Note
	{
		public byte Id { get; set; }

		public string Name { get; set; }

		public uint Wait { get; set; }

		public uint Length { get; set; }
	}
}

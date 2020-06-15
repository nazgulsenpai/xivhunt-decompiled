using System;

namespace FFXIV_GameSense
{
	public class Monster : Combatant
	{
		public ushort BNpcNameID { get; set; }

		public uint FateID { get; set; }
	}
}

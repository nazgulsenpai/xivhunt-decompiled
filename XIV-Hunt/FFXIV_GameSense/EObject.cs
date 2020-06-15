using System;

namespace FFXIV_GameSense
{
	public class EObject : Entity
	{
		public EObjType SubType { get; set; }

		public bool CairnIsUnlocked { get; set; }
	}
}

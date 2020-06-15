using System;

namespace FFXIV_GameSense
{
	internal class CombatantOffsets
	{
		internal int Name { get; private set; }

		internal int ID { get; private set; }

		internal int OwnerID { get; private set; }

		internal int Type { get; private set; }

		internal int EffectiveDistance { get; private set; }

		internal int PosX { get; private set; }

		internal int PosZ { get; private set; }

		internal int PosY { get; private set; }

		internal int Heading { get; private set; }

		internal int FateID { get; private set; }

		internal int EventType { get; private set; }

		internal int CairnIsUnlocked { get; private set; }

		internal int BNpcNameID { get; private set; }

		internal int TargetID { get; private set; }

		internal int TargetID2 { get; private set; }

		internal int Job { get; private set; }

		internal int Level { get; private set; }

		internal int CurrentHP { get; private set; }

		internal int MaxHP { get; private set; }

		internal int CurrentMP { get; private set; }

		internal int MaxMP { get; private set; }

		internal int CurrentGP { get; private set; }

		internal int MaxGP { get; private set; }

		internal int CurrentCP { get; private set; }

		internal int MaxCP { get; private set; }

		internal int StatusEffectsStart { get; private set; }

		public CombatantOffsets(GameRegion region)
		{
			this.Name = 48;
			this.ID = 116;
			this.OwnerID = 132;
			this.Type = 140;
			this.EffectiveDistance = 146;
			this.PosX = 160;
			this.PosZ = this.PosX + 4;
			this.PosY = this.PosZ + 4;
			this.Heading = this.PosY + 8;
			this.FateID = 232;
			this.EventType = 400;
			this.CairnIsUnlocked = 418;
			this.TargetID = 472;
			this.TargetID2 = 2448;
			int offset = (region == GameRegion.Global) ? 6288 : 6300;
			this.StatusEffectsStart = offset + ((region == GameRegion.Global) ? 200 : 204);
			this.BNpcNameID = offset - ((region == GameRegion.Global) ? 40 : 32);
			this.CurrentHP = offset + 8;
			this.MaxHP = offset + 12;
			this.CurrentMP = offset + 16;
			this.CurrentGP = offset + 26;
			this.MaxGP = offset + 28;
			this.CurrentCP = offset + 30;
			this.MaxCP = offset + 32;
			if (region == GameRegion.Global)
			{
				offset += 6;
			}
			this.Job = offset + 64;
			this.Level = offset + 66;
		}
	}
}

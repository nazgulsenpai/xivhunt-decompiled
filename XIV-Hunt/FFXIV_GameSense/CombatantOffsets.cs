using System;

namespace FFXIV_GameSense
{
	// Token: 0x0200005F RID: 95
	internal class CombatantOffsets
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600028B RID: 651 RVA: 0x0000DF58 File Offset: 0x0000C158
		// (set) Token: 0x0600028C RID: 652 RVA: 0x0000DF60 File Offset: 0x0000C160
		internal int Name { get; private set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600028D RID: 653 RVA: 0x0000DF69 File Offset: 0x0000C169
		// (set) Token: 0x0600028E RID: 654 RVA: 0x0000DF71 File Offset: 0x0000C171
		internal int ID { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000DF7A File Offset: 0x0000C17A
		// (set) Token: 0x06000290 RID: 656 RVA: 0x0000DF82 File Offset: 0x0000C182
		internal int OwnerID { get; private set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0000DF8B File Offset: 0x0000C18B
		// (set) Token: 0x06000292 RID: 658 RVA: 0x0000DF93 File Offset: 0x0000C193
		internal int Type { get; private set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000293 RID: 659 RVA: 0x0000DF9C File Offset: 0x0000C19C
		// (set) Token: 0x06000294 RID: 660 RVA: 0x0000DFA4 File Offset: 0x0000C1A4
		internal int EffectiveDistance { get; private set; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000295 RID: 661 RVA: 0x0000DFAD File Offset: 0x0000C1AD
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000DFB5 File Offset: 0x0000C1B5
		internal int PosX { get; private set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000297 RID: 663 RVA: 0x0000DFBE File Offset: 0x0000C1BE
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0000DFC6 File Offset: 0x0000C1C6
		internal int PosZ { get; private set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000299 RID: 665 RVA: 0x0000DFCF File Offset: 0x0000C1CF
		// (set) Token: 0x0600029A RID: 666 RVA: 0x0000DFD7 File Offset: 0x0000C1D7
		internal int PosY { get; private set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600029B RID: 667 RVA: 0x0000DFE0 File Offset: 0x0000C1E0
		// (set) Token: 0x0600029C RID: 668 RVA: 0x0000DFE8 File Offset: 0x0000C1E8
		internal int Heading { get; private set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600029D RID: 669 RVA: 0x0000DFF1 File Offset: 0x0000C1F1
		// (set) Token: 0x0600029E RID: 670 RVA: 0x0000DFF9 File Offset: 0x0000C1F9
		internal int FateID { get; private set; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600029F RID: 671 RVA: 0x0000E002 File Offset: 0x0000C202
		// (set) Token: 0x060002A0 RID: 672 RVA: 0x0000E00A File Offset: 0x0000C20A
		internal int EventType { get; private set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x0000E013 File Offset: 0x0000C213
		// (set) Token: 0x060002A2 RID: 674 RVA: 0x0000E01B File Offset: 0x0000C21B
		internal int CairnIsUnlocked { get; private set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000E024 File Offset: 0x0000C224
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000E02C File Offset: 0x0000C22C
		internal int BNpcNameID { get; private set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000E035 File Offset: 0x0000C235
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x0000E03D File Offset: 0x0000C23D
		internal int TargetID { get; private set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060002A7 RID: 679 RVA: 0x0000E046 File Offset: 0x0000C246
		// (set) Token: 0x060002A8 RID: 680 RVA: 0x0000E04E File Offset: 0x0000C24E
		internal int TargetID2 { get; private set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000E057 File Offset: 0x0000C257
		// (set) Token: 0x060002AA RID: 682 RVA: 0x0000E05F File Offset: 0x0000C25F
		internal int Job { get; private set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002AB RID: 683 RVA: 0x0000E068 File Offset: 0x0000C268
		// (set) Token: 0x060002AC RID: 684 RVA: 0x0000E070 File Offset: 0x0000C270
		internal int Level { get; private set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000E079 File Offset: 0x0000C279
		// (set) Token: 0x060002AE RID: 686 RVA: 0x0000E081 File Offset: 0x0000C281
		internal int CurrentHP { get; private set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000E08A File Offset: 0x0000C28A
		// (set) Token: 0x060002B0 RID: 688 RVA: 0x0000E092 File Offset: 0x0000C292
		internal int MaxHP { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000E09B File Offset: 0x0000C29B
		// (set) Token: 0x060002B2 RID: 690 RVA: 0x0000E0A3 File Offset: 0x0000C2A3
		internal int CurrentMP { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x0000E0AC File Offset: 0x0000C2AC
		// (set) Token: 0x060002B4 RID: 692 RVA: 0x0000E0B4 File Offset: 0x0000C2B4
		internal int MaxMP { get; private set; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x0000E0BD File Offset: 0x0000C2BD
		// (set) Token: 0x060002B6 RID: 694 RVA: 0x0000E0C5 File Offset: 0x0000C2C5
		internal int CurrentGP { get; private set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x0000E0CE File Offset: 0x0000C2CE
		// (set) Token: 0x060002B8 RID: 696 RVA: 0x0000E0D6 File Offset: 0x0000C2D6
		internal int MaxGP { get; private set; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x0000E0DF File Offset: 0x0000C2DF
		// (set) Token: 0x060002BA RID: 698 RVA: 0x0000E0E7 File Offset: 0x0000C2E7
		internal int CurrentCP { get; private set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060002BB RID: 699 RVA: 0x0000E0F0 File Offset: 0x0000C2F0
		// (set) Token: 0x060002BC RID: 700 RVA: 0x0000E0F8 File Offset: 0x0000C2F8
		internal int MaxCP { get; private set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060002BD RID: 701 RVA: 0x0000E101 File Offset: 0x0000C301
		// (set) Token: 0x060002BE RID: 702 RVA: 0x0000E109 File Offset: 0x0000C309
		internal int StatusEffectsStart { get; private set; }

		// Token: 0x060002BF RID: 703 RVA: 0x0000E114 File Offset: 0x0000C314
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

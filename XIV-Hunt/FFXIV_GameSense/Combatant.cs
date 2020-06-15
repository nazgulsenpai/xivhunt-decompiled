using System;
using System.Collections.Generic;

namespace FFXIV_GameSense
{
	// Token: 0x02000064 RID: 100
	public abstract class Combatant : Entity
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0000E724 File Offset: 0x0000C924
		// (set) Token: 0x060002E8 RID: 744 RVA: 0x0000E72C File Offset: 0x0000C92C
		public JobEnum Job { get; set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000E735 File Offset: 0x0000C935
		public string JobName
		{
			get
			{
				return Enum.GetName(typeof(JobEnum), this.Job);
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0000E751 File Offset: 0x0000C951
		// (set) Token: 0x060002EB RID: 747 RVA: 0x0000E759 File Offset: 0x0000C959
		public byte Level { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0000E762 File Offset: 0x0000C962
		// (set) Token: 0x060002ED RID: 749 RVA: 0x0000E76A File Offset: 0x0000C96A
		public uint CurrentHP { get; set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002EE RID: 750 RVA: 0x0000E773 File Offset: 0x0000C973
		// (set) Token: 0x060002EF RID: 751 RVA: 0x0000E77B File Offset: 0x0000C97B
		public uint MaxHP { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x0000E784 File Offset: 0x0000C984
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x0000E78C File Offset: 0x0000C98C
		public uint CurrentMP { get; set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x0000E795 File Offset: 0x0000C995
		public ushort MaxMP { get; } = 10000;

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000E79D File Offset: 0x0000C99D
		// (set) Token: 0x060002F4 RID: 756 RVA: 0x0000E7A5 File Offset: 0x0000C9A5
		public ushort MaxGP { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x0000E7AE File Offset: 0x0000C9AE
		// (set) Token: 0x060002F6 RID: 758 RVA: 0x0000E7B6 File Offset: 0x0000C9B6
		public ushort CurrentGP { get; set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0000E7BF File Offset: 0x0000C9BF
		// (set) Token: 0x060002F8 RID: 760 RVA: 0x0000E7C7 File Offset: 0x0000C9C7
		public ushort MaxCP { get; set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x0000E7D0 File Offset: 0x0000C9D0
		// (set) Token: 0x060002FA RID: 762 RVA: 0x0000E7D8 File Offset: 0x0000C9D8
		public ushort CurrentCP { get; set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002FB RID: 763 RVA: 0x0000E7E1 File Offset: 0x0000C9E1
		public List<Status> StatusList { get; } = new List<Status>();

		// Token: 0x060002FC RID: 764 RVA: 0x0000E7E9 File Offset: 0x0000C9E9
		public bool IsBattleClass()
		{
			return this.Job < JobEnum.CRP || this.Job > JobEnum.FSH;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0000E800 File Offset: 0x0000CA00
		public bool IsGatherer()
		{
			return Combatant.IsGatherer(this.Job);
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000E80D File Offset: 0x0000CA0D
		public static bool IsGatherer(JobEnum j)
		{
			return j >= JobEnum.MIN && j <= JobEnum.FSH;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000E81E File Offset: 0x0000CA1E
		public bool IsCrafter()
		{
			return Combatant.IsCrafter(this.Job);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000E82B File Offset: 0x0000CA2B
		public static bool IsCrafter(JobEnum j)
		{
			return j >= JobEnum.CRP && j <= JobEnum.CUL;
		}
	}
}

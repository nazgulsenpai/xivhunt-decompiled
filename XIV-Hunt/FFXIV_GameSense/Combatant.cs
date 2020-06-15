using System;
using System.Collections.Generic;

namespace FFXIV_GameSense
{
	public abstract class Combatant : Entity
	{
		public JobEnum Job { get; set; }

		public string JobName
		{
			get
			{
				return Enum.GetName(typeof(JobEnum), this.Job);
			}
		}

		public byte Level { get; set; }

		public uint CurrentHP { get; set; }

		public uint MaxHP { get; set; }

		public uint CurrentMP { get; set; }

		public ushort MaxMP { get; } = 10000;

		public ushort MaxGP { get; set; }

		public ushort CurrentGP { get; set; }

		public ushort MaxCP { get; set; }

		public ushort CurrentCP { get; set; }

		public List<Status> StatusList { get; } = new List<Status>();

		public bool IsBattleClass()
		{
			return this.Job < JobEnum.CRP || this.Job > JobEnum.FSH;
		}

		public bool IsGatherer()
		{
			return Combatant.IsGatherer(this.Job);
		}

		public static bool IsGatherer(JobEnum j)
		{
			return j >= JobEnum.MIN && j <= JobEnum.FSH;
		}

		public bool IsCrafter()
		{
			return Combatant.IsCrafter(this.Job);
		}

		public static bool IsCrafter(JobEnum j)
		{
			return j >= JobEnum.CRP && j <= JobEnum.CUL;
		}
	}
}

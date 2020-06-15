using System;

namespace TextPlayer.MML
{
	// Token: 0x02000021 RID: 33
	public class MMLPlayerTrack : MMLPlayer
	{
		// Token: 0x060000FA RID: 250 RVA: 0x00004ACB File Offset: 0x00002CCB
		public MMLPlayerTrack(MultiTrackMMLPlayer parent)
		{
			this.Parent = parent;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00004ADA File Offset: 0x00002CDA
		protected override void PlayNote(Note note, int channel, TimeSpan time)
		{
			this.Parent.PlayNote(note, channel, this, time);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00004AEB File Offset: 0x00002CEB
		protected override void SetTempo(MMLCommand cmd)
		{
			if (base.Mode == MMLMode.Mabinogi)
			{
				this.Parent.SetTempo(Convert.ToInt32(cmd.Args[0]));
				return;
			}
			base.SetTempo(cmd);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00002E6C File Offset: 0x0000106C
		protected override void CalculateDuration()
		{
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000FE RID: 254 RVA: 0x00004B19 File Offset: 0x00002D19
		public MultiTrackMMLPlayer Parent { get; }
	}
}

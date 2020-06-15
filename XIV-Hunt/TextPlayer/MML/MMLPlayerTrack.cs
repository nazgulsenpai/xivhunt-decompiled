using System;

namespace TextPlayer.MML
{
	public class MMLPlayerTrack : MMLPlayer
	{
		public MMLPlayerTrack(MultiTrackMMLPlayer parent)
		{
			this.Parent = parent;
		}

		protected override void PlayNote(Note note, int channel, TimeSpan time)
		{
			this.Parent.PlayNote(note, channel, this, time);
		}

		protected override void SetTempo(MMLCommand cmd)
		{
			if (base.Mode == MMLMode.Mabinogi)
			{
				this.Parent.SetTempo(Convert.ToInt32(cmd.Args[0]));
				return;
			}
			base.SetTempo(cmd);
		}

		protected override void CalculateDuration()
		{
		}

		public MultiTrackMMLPlayer Parent { get; }
	}
}

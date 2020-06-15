using System;

namespace TextPlayer.MML
{
	public struct MMLLength
	{
		public MMLLength(int length, bool dotted)
		{
			this.Length = length;
			this.Dotted = dotted;
		}

		public TimeSpan ToTimeSpan(double secondsPerMeasure)
		{
			double length = 1.0 / (double)this.Length;
			if (this.Dotted)
			{
				length *= 1.5;
			}
			return new TimeSpan((long)(secondsPerMeasure * length * 10000000.0));
		}

		public int Length;

		public bool Dotted;
	}
}

using System;

namespace TextPlayer.MML
{
	// Token: 0x0200001E RID: 30
	public struct MMLLength
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x00003E25 File Offset: 0x00002025
		public MMLLength(int length, bool dotted)
		{
			this.Length = length;
			this.Dotted = dotted;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00003E38 File Offset: 0x00002038
		public TimeSpan ToTimeSpan(double secondsPerMeasure)
		{
			double length = 1.0 / (double)this.Length;
			if (this.Dotted)
			{
				length *= 1.5;
			}
			return new TimeSpan((long)(secondsPerMeasure * length * 10000000.0));
		}

		// Token: 0x04000060 RID: 96
		public int Length;

		// Token: 0x04000061 RID: 97
		public bool Dotted;
	}
}

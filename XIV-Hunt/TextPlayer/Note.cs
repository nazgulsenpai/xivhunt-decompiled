using System;

namespace TextPlayer
{
	// Token: 0x02000017 RID: 23
	public struct Note
	{
		// Token: 0x060000BA RID: 186 RVA: 0x00003A14 File Offset: 0x00001C14
		public double GetFrequency(Note? tuningNote = null)
		{
			if (tuningNote == null)
			{
				tuningNote = new Note?(Note.A4);
			}
			int dist = (int)(this.GetStep() - tuningNote.Value.GetStep());
			return 440.0 * Math.Pow(1.0594630943592953, (double)dist);
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00003A68 File Offset: 0x00001C68
		public byte GetStep()
		{
			byte step;
			switch (this.Type)
			{
			case 'a':
				if (!this.Sharp)
				{
					step = 9;
					goto IL_87;
				}
				step = 10;
				goto IL_87;
			case 'b':
				step = 11;
				goto IL_87;
			case 'c':
				if (!this.Sharp)
				{
					step = 0;
					goto IL_87;
				}
				step = 1;
				goto IL_87;
			case 'd':
				if (!this.Sharp)
				{
					step = 2;
					goto IL_87;
				}
				step = 3;
				goto IL_87;
			case 'e':
				step = 4;
				goto IL_87;
			case 'f':
				if (!this.Sharp)
				{
					step = 5;
					goto IL_87;
				}
				step = 6;
				goto IL_87;
			}
			if (!this.Sharp)
			{
				step = 7;
			}
			else
			{
				step = 8;
			}
			IL_87:
			return step + (byte)(this.Octave * 12 - 12);
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00003B0E File Offset: 0x00001D0E
		public static Note A4
		{
			get
			{
				if (Note.a4initialized)
				{
					return Note.a4;
				}
				Note.a4.Octave = 4;
				Note.a4.Type = 'a';
				Note.a4initialized = true;
				return Note.a4;
			}
		}

		// Token: 0x04000043 RID: 67
		public int Octave;

		// Token: 0x04000044 RID: 68
		public TimeSpan Length;

		// Token: 0x04000045 RID: 69
		public char Type;

		// Token: 0x04000046 RID: 70
		public bool Sharp;

		// Token: 0x04000047 RID: 71
		public float Volume;

		// Token: 0x04000048 RID: 72
		public TimeSpan TimeSpan;

		// Token: 0x04000049 RID: 73
		private static bool a4initialized;

		// Token: 0x0400004A RID: 74
		private static Note a4;
	}
}

using System;

namespace TextPlayer
{
	public struct Note
	{
		public double GetFrequency(Note? tuningNote = null)
		{
			if (tuningNote == null)
			{
				tuningNote = new Note?(Note.A4);
			}
			int dist = (int)(this.GetStep() - tuningNote.Value.GetStep());
			return 440.0 * Math.Pow(1.0594630943592953, (double)dist);
		}

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

		public int Octave;

		public TimeSpan Length;

		public char Type;

		public bool Sharp;

		public float Volume;

		public TimeSpan TimeSpan;

		private static bool a4initialized;

		private static Note a4;
	}
}

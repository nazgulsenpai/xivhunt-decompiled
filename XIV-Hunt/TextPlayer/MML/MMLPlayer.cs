using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextPlayer.MML
{
	// Token: 0x02000020 RID: 32
	public abstract class MMLPlayer : MusicPlayer
	{
		// Token: 0x060000D9 RID: 217 RVA: 0x00003E80 File Offset: 0x00002080
		public MMLPlayer()
		{
			this.Settings = new MMLSettings();
			this.SetDefaultValues();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00003EF8 File Offset: 0x000020F8
		private void SetDefaultValues()
		{
			this.octave = 4;
			this.length = new MMLLength(4, false);
			this.Tempo = 120;
			this.volume = 8;
			this.cmdIndex = 0;
			this.nextNoteIndex = 0;
			this.nextCommand = 0.0;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00003F48 File Offset: 0x00002148
		public override void Load(string str)
		{
			this.commands = new List<MMLCommand>();
			MatchCollection matches = Regex.Matches(str.Replace("\n", "").Replace("\r", ""), this.mmlPatterns);
			for (int i = 0; i < matches.Count; i++)
			{
				this.commands.Add(MMLCommand.Parse(matches[i].Value));
			}
			this.CalculateDuration();
			this.SetDefaultValues();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00003FC4 File Offset: 0x000021C4
		protected virtual void CalculateDuration()
		{
			TimeSpan dur = TimeSpan.Zero;
			for (int i = 0; i < this.commands.Count; i++)
			{
				MMLCommand cmd = this.commands[i];
				switch (cmd.Type)
				{
				case MMLCommandType.Tempo:
					this.SetTempo(cmd);
					break;
				case MMLCommandType.Length:
					this.SetLength(cmd);
					break;
				case MMLCommandType.Note:
				case MMLCommandType.NoteNumber:
				{
					double measureLength;
					Note note = this.GetNote(cmd, out measureLength, this.length, this.octave, this.spm);
					dur += note.Length;
					break;
				}
				case MMLCommandType.Rest:
				{
					double measureLength;
					dur += this.GetRest(cmd, out measureLength).ToTimeSpan(this.spm);
					break;
				}
				}
				if (dur > this.Settings.MaxDuration)
				{
					throw new SongDurationException("Song exceeded maximum duration " + this.Settings.MaxDuration.ToString());
				}
			}
			this.duration = dur;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000040D8 File Offset: 0x000022D8
		public override void Play(TimeSpan currentTime)
		{
			base.Play(currentTime);
			this.nextTick = this.lastTime;
			this.nextNote = 0.0;
			this.curMeasure = 0.0;
			this.nextNoteIndex = 0;
			this.nextCommand = 0.0;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000412C File Offset: 0x0000232C
		public override void Stop()
		{
			base.Stop();
			this.SetDefaultValues();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000413C File Offset: 0x0000233C
		protected virtual void ProcessCommands()
		{
			bool noteFound = false;
			while (!noteFound && this.cmdIndex < this.commands.Count)
			{
				MMLCommand cmd = this.commands[this.cmdIndex];
				if (cmd.Type == MMLCommandType.Note || cmd.Type == MMLCommandType.NoteNumber)
				{
					return;
				}
				if (cmd.Type == MMLCommandType.Rest)
				{
					return;
				}
				this.ProcessCommand(cmd);
				this.cmdIndex++;
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000041A8 File Offset: 0x000023A8
		public override void Update(TimeSpan currentTime)
		{
			if (!this.playing)
			{
				return;
			}
			while (currentTime >= this.nextTick)
			{
				if (this.curMeasure >= this.nextCommand)
				{
					this.ProcessCommands();
				}
				if (this.curMeasure >= this.nextNote)
				{
					if (this.cmdIndex < this.commands.Count)
					{
						this.NextNote();
					}
					else
					{
						this.Stop();
					}
				}
				this.nextTick += new TimeSpan((long)(0.0078125 * this.spm * 10000000.0));
				this.curMeasure += 0.0078125;
			}
			base.Update(currentTime);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00004264 File Offset: 0x00002464
		private void NextNote()
		{
			bool noteFound = false;
			while (!noteFound && this.cmdIndex < this.commands.Count)
			{
				MMLCommand cmd = this.commands[this.cmdIndex];
				if (cmd.Type == MMLCommandType.Note || cmd.Type == MMLCommandType.NoteNumber)
				{
					noteFound = true;
					double measureLength;
					Note note = this.GetNote(cmd, out measureLength, this.length, this.octave, this.spm);
					this.nextCommand = this.nextNote + measureLength;
					if (this.cmdIndex >= this.nextNoteIndex)
					{
						bool tied = false;
						int lookAheadIndex = this.cmdIndex + 1;
						int lookAheadOctave = this.octave;
						int lookAheadTempo = this.tempo;
						double lookAheadSpm = this.spm;
						MMLLength lookAheadLen = this.length;
						while (lookAheadIndex < this.commands.Count)
						{
							MMLCommandType type = this.commands[lookAheadIndex].Type;
							if (type == MMLCommandType.Tie)
							{
								tied = true;
							}
							else
							{
								if (type == MMLCommandType.Rest)
								{
									break;
								}
								if (type == MMLCommandType.Octave)
								{
									lookAheadOctave = Convert.ToInt32(this.commands[lookAheadIndex].Args[0]);
								}
								else if (type == MMLCommandType.OctaveDown)
								{
									lookAheadOctave--;
								}
								else if (type == MMLCommandType.OctaveDown)
								{
									lookAheadOctave++;
								}
								else if (type == MMLCommandType.Tempo)
								{
									this.SetTempoAndSecondsPerMeasure(Convert.ToInt32(this.commands[lookAheadIndex].Args[0]), ref lookAheadTempo, ref lookAheadSpm);
								}
								else if (type == MMLCommandType.Length)
								{
									this.SetLength(this.commands[lookAheadIndex], ref lookAheadLen);
								}
								else if (type == MMLCommandType.Note || type == MMLCommandType.NoteNumber)
								{
									if (!tied)
									{
										break;
									}
									tied = false;
									double tiedLength;
									Note tiedNote = this.GetNote(this.commands[lookAheadIndex], out tiedLength, lookAheadLen, lookAheadOctave, lookAheadSpm);
									if (tiedNote.Sharp != note.Sharp || tiedNote.Type != note.Type || tiedNote.Octave != note.Octave)
									{
										break;
									}
									note.Length += tiedNote.Length;
									measureLength += tiedLength;
									this.nextNoteIndex = lookAheadIndex + 1;
								}
							}
							lookAheadIndex++;
						}
						this.nextNote += measureLength;
						this.notes.Add(this.ValidateAndPlayNote(note));
					}
				}
				else if (cmd.Type == MMLCommandType.Rest)
				{
					double measureLength2;
					this.GetRest(cmd, out measureLength2);
					this.nextNote += measureLength2;
					this.nextCommand = this.nextNote;
					noteFound = true;
				}
				else
				{
					this.ProcessCommand(cmd);
				}
				this.cmdIndex++;
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000044F8 File Offset: 0x000026F8
		private Note ValidateAndPlayNote(Note note)
		{
			if (note.Octave < (int)this.Settings.MinOctave)
			{
				note.Octave = (int)this.Settings.MinOctave;
			}
			else if (note.Octave > (int)this.Settings.MaxOctave)
			{
				note.Octave = (int)this.Settings.MaxOctave;
			}
			note.Volume = Math.Max(0f, Math.Min(note.Volume, 1f));
			if (!base.Muted)
			{
				this.PlayNote(note, 0, this.nextTick);
			}
			note.TimeSpan = this.nextTick;
			return note;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004598 File Offset: 0x00002798
		private MMLLength GetRest(MMLCommand cmd, out double measureLength)
		{
			MMLLength rest = this.GetLength(cmd.Args[0], cmd.Args[1]);
			measureLength = 1.0 / (double)rest.Length * (rest.Dotted ? 1.5 : 1.0);
			return rest;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000045F5 File Offset: 0x000027F5
		private Note GetNote(MMLCommand cmd, out double measureLength, MMLLength defaultLength, int currentOctave, double currentSpm)
		{
			if (cmd.Type == MMLCommandType.Note)
			{
				return this.GetNoteNormal(cmd, out measureLength, defaultLength, currentOctave, currentSpm);
			}
			return this.GetNoteNumber(cmd, out measureLength, defaultLength, currentSpm);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000461C File Offset: 0x0000281C
		private Note GetNoteNormal(MMLCommand cmd, out double measureLength, MMLLength defaultLength, int currentOctave, double currentSpm)
		{
			Note note = new Note
			{
				Volume = (float)this.volume / 15f,
				Octave = currentOctave,
				Type = cmd.Args[0].ToLowerInvariant()[0]
			};
			string text = cmd.Args[1];
			if (text != null)
			{
				if (text == "#" || text == "+")
				{
					base.Step(ref note, 1);
					goto IL_9C;
				}
				if (text == "-")
				{
					base.Step(ref note, -1);
					goto IL_9C;
				}
			}
			note.Sharp = false;
			IL_9C:
			MMLLength mmlLen = this.GetLength(cmd.Args[2], cmd.Args[3], defaultLength);
			note.Length = mmlLen.ToTimeSpan(currentSpm);
			measureLength = 1.0 / (double)mmlLen.Length;
			if (mmlLen.Dotted)
			{
				measureLength *= 1.5;
			}
			return note;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00004720 File Offset: 0x00002920
		private Note GetNoteNumber(MMLCommand cmd, out double measureLength, MMLLength defaultLength, double currentSpm)
		{
			Note note = new Note
			{
				Volume = (float)this.volume / 15f,
				Octave = 1,
				Type = 'c'
			};
			int noteNumber = Convert.ToInt32(cmd.Args[0]) - 12;
			int octavesUp = noteNumber / 12;
			int steps;
			if (octavesUp != 0)
			{
				steps = noteNumber % (octavesUp * 12);
			}
			else
			{
				steps = noteNumber;
			}
			note.Octave += octavesUp;
			base.Step(ref note, steps);
			MMLLength mmlLen = this.GetLength("", cmd.Args[1], defaultLength);
			note.Length = mmlLen.ToTimeSpan(currentSpm);
			measureLength = 1.0 / (double)mmlLen.Length;
			if (mmlLen.Dotted)
			{
				measureLength *= 1.5;
			}
			return note;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000047F0 File Offset: 0x000029F0
		protected virtual void ProcessCommand(MMLCommand cmd)
		{
			switch (cmd.Type)
			{
			case MMLCommandType.Tempo:
				this.SetTempo(cmd);
				return;
			case MMLCommandType.Length:
				this.SetLength(cmd);
				return;
			case MMLCommandType.Volume:
				this.SetVolume(Convert.ToInt32(cmd.Args[0]));
				return;
			case MMLCommandType.Octave:
				this.SetOctave(Convert.ToInt32(cmd.Args[0]));
				return;
			case MMLCommandType.OctaveDown:
				this.SetOctave(this.octave - 1);
				return;
			case MMLCommandType.OctaveUp:
				this.SetOctave(this.octave + 1);
				return;
			default:
				return;
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00004880 File Offset: 0x00002A80
		protected virtual void SetLength(MMLCommand cmd)
		{
			this.SetLength(cmd, ref this.length);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000488F File Offset: 0x00002A8F
		protected virtual void SetLength(MMLCommand cmd, ref MMLLength len)
		{
			len = new MMLLength(Convert.ToInt32(cmd.Args[0]), cmd.Args[1] != "");
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000048C4 File Offset: 0x00002AC4
		protected virtual void SetOctave(int newOctave)
		{
			this.octave = newOctave;
			if (this.octave < (int)this.Settings.MinOctave)
			{
				this.octave = (int)this.Settings.MinOctave;
				return;
			}
			if (this.octave > (int)this.Settings.MaxOctave)
			{
				this.octave = (int)this.Settings.MaxOctave;
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00004921 File Offset: 0x00002B21
		protected virtual void SetTempo(MMLCommand cmd)
		{
			this.Tempo = Convert.ToInt32(cmd.Args[0]);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000493C File Offset: 0x00002B3C
		protected virtual void SetVolume(int vol)
		{
			if (this.Mode == MMLMode.ArcheAge)
			{
				vol = (int)Math.Round(Math.Min(1.0, Math.Max(0.0, (double)vol / 127.0)) * 15.0);
			}
			this.volume = vol;
			if (this.volume < this.Settings.MinVolume)
			{
				this.volume = this.Settings.MinVolume;
				return;
			}
			if (this.volume > this.Settings.MaxVolume)
			{
				this.volume = this.Settings.MaxVolume;
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000049DC File Offset: 0x00002BDC
		private MMLLength GetLength(string number, string dot)
		{
			return this.GetLength(number, dot, this.length);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000049EC File Offset: 0x00002BEC
		private MMLLength GetLength(string number, string dot, MMLLength defaultLength)
		{
			MMLLength i = defaultLength;
			if (!string.IsNullOrEmpty(number))
			{
				i = new MMLLength(Convert.ToInt32(number), dot != "");
			}
			else if (!string.IsNullOrEmpty(dot))
			{
				i.Dotted = true;
			}
			return i;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00004A2E File Offset: 0x00002C2E
		private void SetTempoAndSecondsPerMeasure(int value, ref int tempo, ref double spm)
		{
			tempo = Math.Max((int)this.Settings.MinTempo, Math.Min((int)this.Settings.MaxTempo, value));
			spm = 60.0 / ((double)tempo / 4.0);
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00004A6C File Offset: 0x00002C6C
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x00004A74 File Offset: 0x00002C74
		public int Tempo
		{
			get
			{
				return this.tempo;
			}
			set
			{
				this.SetTempoAndSecondsPerMeasure(value, ref this.tempo, ref this.spm);
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x00004A89 File Offset: 0x00002C89
		public List<MMLCommand> Commands
		{
			get
			{
				return this.commands;
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00004A91 File Offset: 0x00002C91
		public TimeSpan NextTick
		{
			get
			{
				return this.nextTick;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x00004A99 File Offset: 0x00002C99
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00004AA1 File Offset: 0x00002CA1
		public MMLSettings Settings { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00004AAA File Offset: 0x00002CAA
		internal override ValidationSettings ValidationSettings
		{
			get
			{
				return this.Settings;
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00004AB2 File Offset: 0x00002CB2
		public override TimeSpan Duration
		{
			get
			{
				return this.duration;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x00004ABA File Offset: 0x00002CBA
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00004AC2 File Offset: 0x00002CC2
		public MMLMode Mode { get; set; }

		// Token: 0x04000065 RID: 101
		public const double Tick = 0.0078125;

		// Token: 0x04000066 RID: 102
		protected TimeSpan nextTick;

		// Token: 0x04000067 RID: 103
		protected double nextNote;

		// Token: 0x04000068 RID: 104
		protected double nextCommand;

		// Token: 0x04000069 RID: 105
		protected double curMeasure;

		// Token: 0x0400006A RID: 106
		private int octave;

		// Token: 0x0400006B RID: 107
		private MMLLength length;

		// Token: 0x0400006C RID: 108
		private int tempo;

		// Token: 0x0400006D RID: 109
		protected double spm;

		// Token: 0x0400006E RID: 110
		private int volume;

		// Token: 0x0400006F RID: 111
		private TimeSpan duration;

		// Token: 0x04000070 RID: 112
		protected List<MMLCommand> commands;

		// Token: 0x04000071 RID: 113
		internal List<Note> notes = new List<Note>();

		// Token: 0x04000072 RID: 114
		protected int cmdIndex;

		// Token: 0x04000073 RID: 115
		protected int nextNoteIndex;

		// Token: 0x04000074 RID: 116
		protected string mmlPatterns = string.Concat(new string[]
		{
			"[tT]\\d{1,3}|[lL]",
			MMLCommand.lengths,
			"\\.?|[vV]\\d+|[oO]\\d|<|>|[a-gA-G](\\+|#|-)?",
			MMLCommand.lengths,
			"?\\.?|[rR]",
			MMLCommand.lengths,
			"?\\.?|[nN]\\d+\\.?|&"
		});
	}
}

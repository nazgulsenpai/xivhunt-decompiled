using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TextPlayer.MML
{
	public abstract class MMLPlayer : MusicPlayer
	{
		public MMLPlayer()
		{
			this.Settings = new MMLSettings();
			this.SetDefaultValues();
		}

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

		public override void Play(TimeSpan currentTime)
		{
			base.Play(currentTime);
			this.nextTick = this.lastTime;
			this.nextNote = 0.0;
			this.curMeasure = 0.0;
			this.nextNoteIndex = 0;
			this.nextCommand = 0.0;
		}

		public override void Stop()
		{
			base.Stop();
			this.SetDefaultValues();
		}

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

		private MMLLength GetRest(MMLCommand cmd, out double measureLength)
		{
			MMLLength rest = this.GetLength(cmd.Args[0], cmd.Args[1]);
			measureLength = 1.0 / (double)rest.Length * (rest.Dotted ? 1.5 : 1.0);
			return rest;
		}

		private Note GetNote(MMLCommand cmd, out double measureLength, MMLLength defaultLength, int currentOctave, double currentSpm)
		{
			if (cmd.Type == MMLCommandType.Note)
			{
				return this.GetNoteNormal(cmd, out measureLength, defaultLength, currentOctave, currentSpm);
			}
			return this.GetNoteNumber(cmd, out measureLength, defaultLength, currentSpm);
		}

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

		protected virtual void SetLength(MMLCommand cmd)
		{
			this.SetLength(cmd, ref this.length);
		}

		protected virtual void SetLength(MMLCommand cmd, ref MMLLength len)
		{
			len = new MMLLength(Convert.ToInt32(cmd.Args[0]), cmd.Args[1] != "");
		}

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

		protected virtual void SetTempo(MMLCommand cmd)
		{
			this.Tempo = Convert.ToInt32(cmd.Args[0]);
		}

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

		private MMLLength GetLength(string number, string dot)
		{
			return this.GetLength(number, dot, this.length);
		}

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

		private void SetTempoAndSecondsPerMeasure(int value, ref int tempo, ref double spm)
		{
			tempo = Math.Max((int)this.Settings.MinTempo, Math.Min((int)this.Settings.MaxTempo, value));
			spm = 60.0 / ((double)tempo / 4.0);
		}

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

		public List<MMLCommand> Commands
		{
			get
			{
				return this.commands;
			}
		}

		public TimeSpan NextTick
		{
			get
			{
				return this.nextTick;
			}
		}

		public MMLSettings Settings { get; set; }

		internal override ValidationSettings ValidationSettings
		{
			get
			{
				return this.Settings;
			}
		}

		public override TimeSpan Duration
		{
			get
			{
				return this.duration;
			}
		}

		public MMLMode Mode { get; set; }

		public const double Tick = 0.0078125;

		protected TimeSpan nextTick;

		protected double nextNote;

		protected double nextCommand;

		protected double curMeasure;

		private int octave;

		private MMLLength length;

		private int tempo;

		protected double spm;

		private int volume;

		private TimeSpan duration;

		protected List<MMLCommand> commands;

		internal List<Note> notes = new List<Note>();

		protected int cmdIndex;

		protected int nextNoteIndex;

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

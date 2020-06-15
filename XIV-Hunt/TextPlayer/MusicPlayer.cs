using System;
using System.IO;
using System.Text;

namespace TextPlayer
{
	// Token: 0x02000016 RID: 22
	public abstract class MusicPlayer : IMusicPlayer
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x000020E8 File Offset: 0x000002E8
		public MusicPlayer()
		{
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000035E8 File Offset: 0x000017E8
		public void FromFile(string file)
		{
			using (StreamReader stream = new StreamReader(file))
			{
				this.Load(stream);
			}
		}

		// Token: 0x060000A7 RID: 167
		public abstract void Load(string str);

		// Token: 0x060000A8 RID: 168 RVA: 0x00003620 File Offset: 0x00001820
		public void Load(StreamReader stream)
		{
			StringBuilder strBuilder = new StringBuilder();
			char[] buffer = new char[1024];
			while (!stream.EndOfStream)
			{
				int bytesRead = stream.ReadBlock(buffer, 0, buffer.Length);
				strBuilder.Append(buffer, 0, bytesRead);
			}
			this.Load(strBuilder.ToString());
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000366C File Offset: 0x0000186C
		public virtual void Play()
		{
			this.Play(new TimeSpan(DateTime.Now.Ticks));
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00003691 File Offset: 0x00001891
		public virtual void Play(TimeSpan currentTime)
		{
			if (this.playing)
			{
				throw new InvalidOperationException(base.GetType().ToString() + " was already playing.");
			}
			this.playing = true;
			this.lastTime = currentTime;
			this.startTime = currentTime;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000036CB File Offset: 0x000018CB
		public virtual void Stop()
		{
			this.playing = false;
			this.startTime = TimeSpan.Zero;
			this.lastTime = TimeSpan.Zero;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000036EC File Offset: 0x000018EC
		public virtual void Update()
		{
			this.Update(new TimeSpan(DateTime.Now.Ticks));
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00003711 File Offset: 0x00001911
		public virtual void Update(TimeSpan currentTime)
		{
			if (this.Playing)
			{
				this.lastTime = currentTime;
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003724 File Offset: 0x00001924
		public virtual void Seek(TimeSpan position)
		{
			this.Seek(new TimeSpan(DateTime.Now.Ticks));
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003749 File Offset: 0x00001949
		public virtual void Seek(TimeSpan currentTime, TimeSpan position)
		{
			bool flag = this.Muted;
			this.Stop();
			this.Mute();
			this.Play(currentTime - position);
			this.Update(currentTime);
			if (!flag)
			{
				this.Unmute();
			}
		}

		// Token: 0x060000B0 RID: 176
		protected abstract void PlayNote(Note note, int channel, TimeSpan time);

		// Token: 0x060000B1 RID: 177 RVA: 0x0000377C File Offset: 0x0000197C
		protected void Step(ref Note note, int steps)
		{
			if (steps == 0)
			{
				return;
			}
			if (steps > 0)
			{
				for (int i = 0; i < steps; i++)
				{
					switch (note.Type)
					{
					case 'a':
						if (!note.Sharp)
						{
							note.Sharp = true;
						}
						else
						{
							note.Type = 'b';
							note.Sharp = false;
						}
						break;
					case 'b':
						note.Type = 'c';
						note.Octave++;
						break;
					case 'c':
						if (!note.Sharp)
						{
							note.Sharp = true;
						}
						else
						{
							note.Type = 'd';
							note.Sharp = false;
						}
						break;
					case 'd':
						if (!note.Sharp)
						{
							note.Sharp = true;
						}
						else
						{
							note.Type = 'e';
							note.Sharp = false;
						}
						break;
					case 'e':
						note.Type = 'f';
						break;
					case 'f':
						if (!note.Sharp)
						{
							note.Sharp = true;
						}
						else
						{
							note.Type = 'g';
							note.Sharp = false;
						}
						break;
					case 'g':
						if (!note.Sharp)
						{
							note.Sharp = true;
						}
						else
						{
							note.Type = 'a';
							note.Sharp = false;
						}
						break;
					}
				}
				return;
			}
			for (int j = 0; j < Math.Abs(steps); j++)
			{
				switch (note.Type)
				{
				case 'a':
					if (note.Sharp)
					{
						note.Sharp = false;
					}
					else
					{
						note.Type = 'g';
						note.Sharp = true;
					}
					break;
				case 'b':
					note.Type = 'a';
					note.Sharp = true;
					break;
				case 'c':
					if (note.Sharp)
					{
						note.Sharp = false;
					}
					else
					{
						note.Type = 'b';
						note.Octave--;
					}
					break;
				case 'd':
					if (note.Sharp)
					{
						note.Sharp = false;
					}
					else
					{
						note.Type = 'c';
						note.Sharp = true;
					}
					break;
				case 'e':
					note.Type = 'd';
					note.Sharp = true;
					break;
				case 'f':
					if (note.Sharp)
					{
						note.Sharp = false;
					}
					else
					{
						note.Type = 'e';
					}
					break;
				case 'g':
					if (note.Sharp)
					{
						note.Sharp = false;
					}
					else
					{
						note.Type = 'f';
						note.Sharp = true;
					}
					break;
				}
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000039C3 File Offset: 0x00001BC3
		public virtual void Mute()
		{
			this.muted = true;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000039CC File Offset: 0x00001BCC
		public virtual void Unmute()
		{
			this.muted = false;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000039D5 File Offset: 0x00001BD5
		public bool Playing
		{
			get
			{
				return this.playing;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000039DD File Offset: 0x00001BDD
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x000039E5 File Offset: 0x00001BE5
		public bool Muted
		{
			get
			{
				return this.muted;
			}
			set
			{
				if (this.muted == value)
				{
					return;
				}
				if (value)
				{
					this.Mute();
					return;
				}
				this.Unmute();
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B7 RID: 183
		internal abstract ValidationSettings ValidationSettings { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B8 RID: 184
		public abstract TimeSpan Duration { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00003A01 File Offset: 0x00001C01
		public virtual TimeSpan Elapsed
		{
			get
			{
				return this.lastTime - this.startTime;
			}
		}

		// Token: 0x0400003F RID: 63
		protected bool playing;

		// Token: 0x04000040 RID: 64
		protected TimeSpan lastTime;

		// Token: 0x04000041 RID: 65
		protected TimeSpan startTime;

		// Token: 0x04000042 RID: 66
		private bool muted;
	}
}

using System;
using System.IO;
using System.Text;

namespace TextPlayer
{
	public abstract class MusicPlayer : IMusicPlayer
	{
		public MusicPlayer()
		{
		}

		public void FromFile(string file)
		{
			using (StreamReader stream = new StreamReader(file))
			{
				this.Load(stream);
			}
		}

		public abstract void Load(string str);

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

		public virtual void Play()
		{
			this.Play(new TimeSpan(DateTime.Now.Ticks));
		}

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

		public virtual void Stop()
		{
			this.playing = false;
			this.startTime = TimeSpan.Zero;
			this.lastTime = TimeSpan.Zero;
		}

		public virtual void Update()
		{
			this.Update(new TimeSpan(DateTime.Now.Ticks));
		}

		public virtual void Update(TimeSpan currentTime)
		{
			if (this.Playing)
			{
				this.lastTime = currentTime;
			}
		}

		public virtual void Seek(TimeSpan position)
		{
			this.Seek(new TimeSpan(DateTime.Now.Ticks));
		}

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

		protected abstract void PlayNote(Note note, int channel, TimeSpan time);

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

		public virtual void Mute()
		{
			this.muted = true;
		}

		public virtual void Unmute()
		{
			this.muted = false;
		}

		public bool Playing
		{
			get
			{
				return this.playing;
			}
		}

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

		internal abstract ValidationSettings ValidationSettings { get; }

		public abstract TimeSpan Duration { get; }

		public virtual TimeSpan Elapsed
		{
			get
			{
				return this.lastTime - this.startTime;
			}
		}

		protected bool playing;

		protected TimeSpan lastTime;

		protected TimeSpan startTime;

		private bool muted;
	}
}

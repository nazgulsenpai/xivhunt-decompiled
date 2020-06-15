using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextPlayer.MML
{
	public abstract class MultiTrackMMLPlayer : IMusicPlayer
	{
		public MultiTrackMMLPlayer()
		{
			this.Tracks = new List<MMLPlayerTrack>();
			this.Settings = new MMLSettings();
		}

		protected abstract void PlayNote(Note note, int channel, TimeSpan time);

		internal virtual void PlayNote(Note note, int channel, MMLPlayerTrack track, TimeSpan time)
		{
			int index = this.Tracks.IndexOf(track);
			if (index < 0)
			{
				return;
			}
			this.PlayNote(note, index, time);
		}

		public virtual void SetTempo(int tempo)
		{
			if (this.mmlMode == MMLMode.ArcheAge)
			{
				return;
			}
			foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
			{
				mmlplayerTrack.Tempo = tempo;
			}
		}

		public virtual void Play()
		{
			this.Play(new TimeSpan(DateTime.Now.Ticks));
		}

		public virtual void Play(TimeSpan currentTime)
		{
			foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
			{
				mmlplayerTrack.Play(currentTime);
			}
			this.startTime = currentTime;
		}

		public virtual void Update()
		{
			this.Update(new TimeSpan(DateTime.Now.Ticks));
		}

		public virtual void Update(TimeSpan currentTime)
		{
			if (this.mmlMode == MMLMode.Mabinogi)
			{
				while (currentTime >= this.NextTick)
				{
					if (!this.Playing)
					{
						break;
					}
					foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
					{
						mmlplayerTrack.Update(mmlplayerTrack.NextTick);
					}
				}
			}
			else
			{
				foreach (MMLPlayerTrack mmlplayerTrack2 in this.Tracks)
				{
					mmlplayerTrack2.Update(currentTime);
				}
			}
			this.lastTime = currentTime;
			if (!this.Playing)
			{
				this.Stop();
			}
		}

		protected virtual void CalculateDuration()
		{
			bool storedMute = this.Muted;
			this.Stop();
			this.Mute();
			this.Play(TimeSpan.Zero);
			while (this.Playing)
			{
				foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
				{
					mmlplayerTrack.Update(mmlplayerTrack.NextTick);
				}
				if (this.NextTick > this.Settings.MaxDuration)
				{
					throw new SongDurationException("Song exceeded maximum duration " + this.Settings.MaxDuration.ToString());
				}
			}
			this.Duration = this.NextTick;
			if (!storedMute)
			{
				this.Unmute();
			}
		}

		public virtual void Stop()
		{
			foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
			{
				mmlplayerTrack.Stop();
			}
			this.lastTime = TimeSpan.Zero;
			this.startTime = TimeSpan.Zero;
		}

		public virtual void Seek(TimeSpan position)
		{
			this.Seek(new TimeSpan(DateTime.Now.Ticks), position);
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

		public void FromFile(string file, int maxTracks)
		{
			using (StreamReader stream = new StreamReader(file))
			{
				this.Load(stream, maxTracks);
			}
		}

		public void FromFile(string file)
		{
			this.FromFile(file, 0);
		}

		public void Load(string code, int maxTracks)
		{
			string trimmedCode = code.Trim().TrimEnd(new char[]
			{
				'\n',
				'\r'
			}).TrimStart(new char[]
			{
				'\n',
				'\r'
			});
			if (trimmedCode.StartsWith("MML@", StringComparison.InvariantCultureIgnoreCase))
			{
				trimmedCode = trimmedCode.Replace("MML@", "");
			}
			if (trimmedCode.EndsWith(";", StringComparison.InvariantCultureIgnoreCase))
			{
				trimmedCode = trimmedCode.Remove(trimmedCode.Length - 1);
			}
			string[] tokens = code.Split(',', StringSplitOptions.None);
			if (tokens.Length > maxTracks && maxTracks > 0)
			{
				throw new MalformedMMLException("Maximum number of tracks exceeded. Count: " + tokens.Length.ToString() + ", max: " + maxTracks.ToString());
			}
			this.Tracks = new List<MMLPlayerTrack>();
			for (int i = 0; i < tokens.Length; i++)
			{
				MMLPlayerTrack track = new MMLPlayerTrack(this)
				{
					Settings = this.Settings
				};
				track.Load(tokens[i]);
				track.Mode = this.mmlMode;
				this.Tracks.Add(track);
			}
			this.CalculateDuration();
		}

		public void Load(string code)
		{
			this.Load(code, 0);
		}

		public void Load(StreamReader stream, int maxTracks)
		{
			StringBuilder strBuilder = new StringBuilder();
			char[] buffer = new char[1024];
			while (!stream.EndOfStream)
			{
				int bytesRead = stream.ReadBlock(buffer, 0, buffer.Length);
				strBuilder.Append(buffer, 0, bytesRead);
			}
			this.Load(strBuilder.ToString(), maxTracks);
		}

		public void Load(StreamReader stream)
		{
			this.Load(stream, 0);
		}

		public virtual void Mute()
		{
			this.muted = true;
		}

		public virtual void Unmute()
		{
			this.muted = false;
		}

		public List<MMLPlayerTrack> Tracks { get; private set; }

		public bool Playing
		{
			get
			{
				for (int i = 0; i < this.Tracks.Count; i++)
				{
					if (this.Tracks[i].Playing)
					{
						return true;
					}
				}
				return false;
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

		public TimeSpan Duration { get; private set; }

		public bool Loop { get; set; }

		private TimeSpan NextTick
		{
			get
			{
				long max = 0L;
				for (int i = 0; i < this.Tracks.Count; i++)
				{
					max = Math.Max(max, this.Tracks[i].NextTick.Ticks);
				}
				return new TimeSpan(max);
			}
		}

		public MMLSettings Settings { get; set; }

		public virtual TimeSpan Elapsed
		{
			get
			{
				return this.lastTime - this.startTime;
			}
		}

		public MMLMode Mode
		{
			get
			{
				return this.mmlMode;
			}
			set
			{
				this.mmlMode = value;
				for (int i = 0; i < this.Tracks.Count; i++)
				{
					this.Tracks[i].Mode = this.mmlMode;
				}
			}
		}

		private bool muted;

		protected TimeSpan startTime;

		protected TimeSpan lastTime;

		private MMLMode mmlMode;
	}
}

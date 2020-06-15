using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextPlayer.MML
{
	// Token: 0x02000023 RID: 35
	public abstract class MultiTrackMMLPlayer : IMusicPlayer
	{
		// Token: 0x06000104 RID: 260 RVA: 0x00004B5A File Offset: 0x00002D5A
		public MultiTrackMMLPlayer()
		{
			this.Tracks = new List<MMLPlayerTrack>();
			this.Settings = new MMLSettings();
		}

		// Token: 0x06000105 RID: 261
		protected abstract void PlayNote(Note note, int channel, TimeSpan time);

		// Token: 0x06000106 RID: 262 RVA: 0x00004B78 File Offset: 0x00002D78
		internal virtual void PlayNote(Note note, int channel, MMLPlayerTrack track, TimeSpan time)
		{
			int index = this.Tracks.IndexOf(track);
			if (index < 0)
			{
				return;
			}
			this.PlayNote(note, index, time);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00004BA4 File Offset: 0x00002DA4
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

		// Token: 0x06000108 RID: 264 RVA: 0x00004C00 File Offset: 0x00002E00
		public virtual void Play()
		{
			this.Play(new TimeSpan(DateTime.Now.Ticks));
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00004C28 File Offset: 0x00002E28
		public virtual void Play(TimeSpan currentTime)
		{
			foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
			{
				mmlplayerTrack.Play(currentTime);
			}
			this.startTime = currentTime;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00004C80 File Offset: 0x00002E80
		public virtual void Update()
		{
			this.Update(new TimeSpan(DateTime.Now.Ticks));
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00004CA8 File Offset: 0x00002EA8
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

		// Token: 0x0600010C RID: 268 RVA: 0x00004D78 File Offset: 0x00002F78
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

		// Token: 0x0600010D RID: 269 RVA: 0x00004E50 File Offset: 0x00003050
		public virtual void Stop()
		{
			foreach (MMLPlayerTrack mmlplayerTrack in this.Tracks)
			{
				mmlplayerTrack.Stop();
			}
			this.lastTime = TimeSpan.Zero;
			this.startTime = TimeSpan.Zero;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00004EB8 File Offset: 0x000030B8
		public virtual void Seek(TimeSpan position)
		{
			this.Seek(new TimeSpan(DateTime.Now.Ticks), position);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00004EDE File Offset: 0x000030DE
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

		// Token: 0x06000110 RID: 272 RVA: 0x00004F10 File Offset: 0x00003110
		public void FromFile(string file, int maxTracks)
		{
			using (StreamReader stream = new StreamReader(file))
			{
				this.Load(stream, maxTracks);
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00004F48 File Offset: 0x00003148
		public void FromFile(string file)
		{
			this.FromFile(file, 0);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00004F54 File Offset: 0x00003154
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

		// Token: 0x06000113 RID: 275 RVA: 0x00005060 File Offset: 0x00003260
		public void Load(string code)
		{
			this.Load(code, 0);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000506C File Offset: 0x0000326C
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

		// Token: 0x06000115 RID: 277 RVA: 0x000050B7 File Offset: 0x000032B7
		public void Load(StreamReader stream)
		{
			this.Load(stream, 0);
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000050C1 File Offset: 0x000032C1
		public virtual void Mute()
		{
			this.muted = true;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000050CA File Offset: 0x000032CA
		public virtual void Unmute()
		{
			this.muted = false;
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000118 RID: 280 RVA: 0x000050D3 File Offset: 0x000032D3
		// (set) Token: 0x06000119 RID: 281 RVA: 0x000050DB File Offset: 0x000032DB
		public List<MMLPlayerTrack> Tracks { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000050E4 File Offset: 0x000032E4
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600011B RID: 283 RVA: 0x0000511D File Offset: 0x0000331D
		// (set) Token: 0x0600011C RID: 284 RVA: 0x00005125 File Offset: 0x00003325
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00005141 File Offset: 0x00003341
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00005149 File Offset: 0x00003349
		public TimeSpan Duration { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00005152 File Offset: 0x00003352
		// (set) Token: 0x06000120 RID: 288 RVA: 0x0000515A File Offset: 0x0000335A
		public bool Loop { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00005164 File Offset: 0x00003364
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000122 RID: 290 RVA: 0x000051B0 File Offset: 0x000033B0
		// (set) Token: 0x06000123 RID: 291 RVA: 0x000051B8 File Offset: 0x000033B8
		public MMLSettings Settings { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000051C1 File Offset: 0x000033C1
		public virtual TimeSpan Elapsed
		{
			get
			{
				return this.lastTime - this.startTime;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000125 RID: 293 RVA: 0x000051D4 File Offset: 0x000033D4
		// (set) Token: 0x06000126 RID: 294 RVA: 0x000051DC File Offset: 0x000033DC
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

		// Token: 0x0400007A RID: 122
		private bool muted;

		// Token: 0x0400007B RID: 123
		protected TimeSpan startTime;

		// Token: 0x0400007C RID: 124
		protected TimeSpan lastTime;

		// Token: 0x0400007D RID: 125
		private MMLMode mmlMode;
	}
}

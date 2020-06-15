using System;
using System.IO;

namespace TextPlayer
{
	// Token: 0x02000015 RID: 21
	internal interface IMusicPlayer
	{
		// Token: 0x06000095 RID: 149
		void FromFile(string file);

		// Token: 0x06000096 RID: 150
		void Load(string str);

		// Token: 0x06000097 RID: 151
		void Load(StreamReader stream);

		// Token: 0x06000098 RID: 152
		void Play();

		// Token: 0x06000099 RID: 153
		void Play(TimeSpan currentTime);

		// Token: 0x0600009A RID: 154
		void Stop();

		// Token: 0x0600009B RID: 155
		void Update();

		// Token: 0x0600009C RID: 156
		void Update(TimeSpan currentTime);

		// Token: 0x0600009D RID: 157
		void Seek(TimeSpan position);

		// Token: 0x0600009E RID: 158
		void Seek(TimeSpan currentTime, TimeSpan position);

		// Token: 0x0600009F RID: 159
		void Mute();

		// Token: 0x060000A0 RID: 160
		void Unmute();

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000A1 RID: 161
		bool Playing { get; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A2 RID: 162
		bool Muted { get; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000A3 RID: 163
		TimeSpan Duration { get; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000A4 RID: 164
		TimeSpan Elapsed { get; }
	}
}

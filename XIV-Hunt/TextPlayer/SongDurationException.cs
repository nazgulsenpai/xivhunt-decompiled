using System;

namespace TextPlayer
{
	// Token: 0x02000018 RID: 24
	[Serializable]
	public class SongDurationException : Exception
	{
		// Token: 0x060000BE RID: 190 RVA: 0x00003B3F File Offset: 0x00001D3F
		public SongDurationException()
		{
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00003B47 File Offset: 0x00001D47
		public SongDurationException(string message) : base(message)
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003B50 File Offset: 0x00001D50
		public SongDurationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

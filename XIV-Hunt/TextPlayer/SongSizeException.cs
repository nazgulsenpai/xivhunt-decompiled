using System;

namespace TextPlayer
{
	// Token: 0x02000019 RID: 25
	[Serializable]
	public class SongSizeException : Exception
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x00003B3F File Offset: 0x00001D3F
		public SongSizeException()
		{
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00003B47 File Offset: 0x00001D47
		public SongSizeException(string message) : base(message)
		{
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00003B50 File Offset: 0x00001D50
		public SongSizeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

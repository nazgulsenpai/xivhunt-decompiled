using System;

namespace TextPlayer.MML
{
	// Token: 0x0200001B RID: 27
	[Serializable]
	public class MalformedMMLException : Exception
	{
		// Token: 0x060000D1 RID: 209 RVA: 0x00003B3F File Offset: 0x00001D3F
		public MalformedMMLException()
		{
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00003B47 File Offset: 0x00001D47
		public MalformedMMLException(string message) : base(message)
		{
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00003B50 File Offset: 0x00001D50
		public MalformedMMLException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

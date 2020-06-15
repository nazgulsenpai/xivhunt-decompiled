using System;
using System.Runtime.Serialization;

namespace FFXIV_GameSense
{
	// Token: 0x02000051 RID: 81
	[Serializable]
	public class MemoryScanException : Exception
	{
		// Token: 0x0600023F RID: 575 RVA: 0x00003B3F File Offset: 0x00001D3F
		public MemoryScanException()
		{
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00003B47 File Offset: 0x00001D47
		public MemoryScanException(string message) : base(message)
		{
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00003B50 File Offset: 0x00001D50
		public MemoryScanException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000B9A3 File Offset: 0x00009BA3
		protected MemoryScanException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;
using System.Runtime.Serialization;

namespace FFXIV_GameSense
{
	[Serializable]
	public class MemoryScanException : Exception
	{
		public MemoryScanException()
		{
		}

		public MemoryScanException(string message) : base(message)
		{
		}

		public MemoryScanException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected MemoryScanException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}

using System;

namespace TextPlayer.MML
{
	[Serializable]
	public class MalformedMMLException : Exception
	{
		public MalformedMMLException()
		{
		}

		public MalformedMMLException(string message) : base(message)
		{
		}

		public MalformedMMLException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

using System;

namespace TextPlayer
{
	[Serializable]
	public class SongDurationException : Exception
	{
		public SongDurationException()
		{
		}

		public SongDurationException(string message) : base(message)
		{
		}

		public SongDurationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

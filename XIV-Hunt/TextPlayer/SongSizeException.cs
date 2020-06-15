using System;

namespace TextPlayer
{
	[Serializable]
	public class SongSizeException : Exception
	{
		public SongSizeException()
		{
		}

		public SongSizeException(string message) : base(message)
		{
		}

		public SongSizeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

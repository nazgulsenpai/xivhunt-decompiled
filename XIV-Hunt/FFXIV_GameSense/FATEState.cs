using System;

namespace FFXIV_GameSense
{
	public enum FATEState : byte
	{
		Running = 2,
		Ended = 4,
		Failed,
		Preparation = 7,
		WaitingForEnd
	}
}

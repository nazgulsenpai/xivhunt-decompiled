using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFXIV_GameSense.IPC
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct PipeMessage
	{
		public PipeMessage(int pid, PMCommand cmd, byte data)
		{
			this.PID = pid;
			this.Cmd = cmd;
			this.Data = new byte[]
			{
				data
			}.Concat(Enumerable.Repeat<byte>(0, 4229)).ToArray<byte>();
		}

		public PipeMessage(int pid, PMCommand cmd, byte[] data)
		{
			this.PID = pid;
			this.Cmd = cmd;
			if (data.Length > 4230)
			{
				throw new ArgumentException("Buffer too small", "data");
			}
			this.Data = data.Concat(Enumerable.Repeat<byte>(0, 4230 - data.Length)).ToArray<byte>();
		}

		private readonly int PID;

		private readonly PMCommand Cmd;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4230)]
		private readonly byte[] Data;
	}
}

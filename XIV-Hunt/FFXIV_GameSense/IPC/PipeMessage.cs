using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace FFXIV_GameSense.IPC
{
	// Token: 0x020000CB RID: 203
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct PipeMessage
	{
		// Token: 0x06000641 RID: 1601 RVA: 0x00019439 File Offset: 0x00017639
		public PipeMessage(int pid, PMCommand cmd, byte data)
		{
			this.PID = pid;
			this.Cmd = cmd;
			this.Data = new byte[]
			{
				data
			}.Concat(Enumerable.Repeat<byte>(0, 4229)).ToArray<byte>();
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00019470 File Offset: 0x00017670
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

		// Token: 0x040003DF RID: 991
		private readonly int PID;

		// Token: 0x040003E0 RID: 992
		private readonly PMCommand Cmd;

		// Token: 0x040003E1 RID: 993
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4230)]
		private readonly byte[] Data;
	}
}

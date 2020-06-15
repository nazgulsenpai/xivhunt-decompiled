using System;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FFXIV_GameSense.IPC
{
	// Token: 0x020000CA RID: 202
	internal class PersistentNamedPipeServer
	{
		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x0600063B RID: 1595 RVA: 0x000192F4 File Offset: 0x000174F4
		internal static NamedPipeServerStream Instance
		{
			get
			{
				if (PersistentNamedPipeServer.NPS == null)
				{
					object npslock = PersistentNamedPipeServer.NPSlock;
					lock (npslock)
					{
						if (PersistentNamedPipeServer.NPS == null)
						{
							PersistentNamedPipeServer.Initialize();
						}
					}
				}
				return PersistentNamedPipeServer.NPS;
			}
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0001934C File Offset: 0x0001754C
		private static void Initialize()
		{
			PersistentNamedPipeServer.NPS = new NamedPipeServerStream(PersistentNamedPipeServer.pipename, PipeDirection.Out, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 128, 12288);
			PersistentNamedPipeServer.NPS.WaitForConnectionAsync();
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00019382 File Offset: 0x00017582
		internal static void Restart()
		{
			if (PersistentNamedPipeServer.NPS != null)
			{
				if (PersistentNamedPipeServer.NPS.IsConnected)
				{
					PersistentNamedPipeServer.NPS.Disconnect();
				}
				PersistentNamedPipeServer.NPS.Dispose();
			}
			PersistentNamedPipeServer.Initialize();
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x000193B8 File Offset: 0x000175B8
		internal static bool SendPipeMessage(PipeMessage pipeMessage)
		{
			if (PersistentNamedPipeServer.Instance.IsConnected)
			{
				int size = Marshal.SizeOf<PipeMessage>(pipeMessage);
				byte[] bytes = new byte[size];
				IntPtr ptr = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr<PipeMessage>(pipeMessage, ptr, false);
				Marshal.Copy(ptr, bytes, 0, size);
				Marshal.FreeHGlobal(ptr);
				PersistentNamedPipeServer.Instance.Write(bytes, 0, bytes.Length);
				PersistentNamedPipeServer.Instance.WaitForPipeDrain();
				return true;
			}
			return false;
		}

		// Token: 0x040003DC RID: 988
		private static readonly string pipename = Assembly.GetExecutingAssembly().GetName().Name;

		// Token: 0x040003DD RID: 989
		private static volatile NamedPipeServerStream NPS;

		// Token: 0x040003DE RID: 990
		private static readonly object NPSlock = new object();
	}
}

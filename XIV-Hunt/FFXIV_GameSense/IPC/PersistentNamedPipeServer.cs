using System;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FFXIV_GameSense.IPC
{
	internal class PersistentNamedPipeServer
	{
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

		private static void Initialize()
		{
			PersistentNamedPipeServer.NPS = new NamedPipeServerStream(PersistentNamedPipeServer.pipename, PipeDirection.Out, 254, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 128, 12288);
			PersistentNamedPipeServer.NPS.WaitForConnectionAsync();
		}

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

		private static readonly string pipename = Assembly.GetExecutingAssembly().GetName().Name;

		private static volatile NamedPipeServerStream NPS;

		private static readonly object NPSlock = new object();
	}
}

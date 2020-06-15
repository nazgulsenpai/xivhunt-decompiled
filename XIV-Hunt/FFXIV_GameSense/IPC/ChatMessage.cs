using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFXIV_GameSense.IPC
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct ChatMessage
	{
		public ChatMessage(ChatMessage chatMessage, uint sid, byte isLocal)
		{
			this.LogKind = (byte)chatMessage.Channel;
			byte[] array;
			if (chatMessage.Sender == null)
			{
				array = Encoding.UTF8.GetBytes(" ");
			}
			else
			{
				Sender sender = chatMessage.Sender;
				array = ((sender != null) ? sender.ToArray(true, false) : null);
			}
			byte[] data = array;
			if (data.Length > 128)
			{
				throw new ArgumentException("Buffer too small", "chatMessage");
			}
			this.SenderName = data.Concat(Enumerable.Repeat<byte>(0, 128 - data.Length)).ToArray<byte>();
			data = chatMessage.ToArray(true);
			if (data.Length > 4096)
			{
				throw new ArgumentException("Buffer too small", "chatMessage");
			}
			this.Message = data.Concat(Enumerable.Repeat<byte>(0, 4096 - data.Length)).ToArray<byte>();
			this.SenderActorId = sid;
			this.IsLocal = isLocal;
		}

		private readonly byte LogKind;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private readonly byte[] SenderName;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
		private readonly byte[] Message;

		private readonly uint SenderActorId;

		private readonly byte IsLocal;
	}
}

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFXIV_GameSense.IPC
{
	// Token: 0x020000CC RID: 204
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct ChatMessage
	{
		// Token: 0x06000643 RID: 1603 RVA: 0x000194C8 File Offset: 0x000176C8
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

		// Token: 0x040003E2 RID: 994
		private readonly byte LogKind;

		// Token: 0x040003E3 RID: 995
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		private readonly byte[] SenderName;

		// Token: 0x040003E4 RID: 996
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
		private readonly byte[] Message;

		// Token: 0x040003E5 RID: 997
		private readonly uint SenderActorId;

		// Token: 0x040003E6 RID: 998
		private readonly byte IsLocal;
	}
}

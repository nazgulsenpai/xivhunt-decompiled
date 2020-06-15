using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FFXIV_GameSense
{
	// Token: 0x02000029 RID: 41
	public class Sender
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000152 RID: 338 RVA: 0x000061E1 File Offset: 0x000043E1
		// (set) Token: 0x06000153 RID: 339 RVA: 0x000061E9 File Offset: 0x000043E9
		[JsonProperty]
		public string Name { get; private set; }

		// Token: 0x06000154 RID: 340 RVA: 0x000061F4 File Offset: 0x000043F4
		public Sender(IEnumerable<byte> arr, out int msgStart)
		{
			if (arr.IndexOf(Sender.LinkStart) == 0)
			{
				this.Name = Encoding.UTF8.GetString(arr.Skip(9).Take((int)(arr.ElementAt(8) - 1)).ToArray<byte>());
				msgStart = arr.Skip(9).TakeWhile((byte x) => x != 58).Count<byte>() + 10;
				return;
			}
			this.Name = Encoding.UTF8.GetString(arr.TakeWhile((byte x) => x != 58).ToArray<byte>());
			msgStart = arr.TakeWhile((byte x) => x != 58).Count<byte>() + 1;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000062DE File Offset: 0x000044DE
		public Sender(string v)
		{
			this.Name = v;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x000062F0 File Offset: 0x000044F0
		internal byte[] ToArray(bool link = true, bool world = true)
		{
			List<byte> arr = new List<byte>();
			if (!string.IsNullOrWhiteSpace(this.Name))
			{
				byte[] i = Encoding.UTF8.GetBytes(this.Name);
				if (link)
				{
					arr.AddRange(Sender.LinkStartTemplate);
					arr.AddRange(i);
					arr[2] = Convert.ToByte(i.Length + 8);
					arr.Add(3);
					arr[9] = Convert.ToByte(i.Length + 1);
				}
				arr.AddRange(i);
				arr.AddRange(Sender.LinkEnd);
			}
			return arr.ToArray();
		}

		// Token: 0x04000096 RID: 150
		[JsonIgnore]
		private static readonly byte[] LinkStart = new byte[]
		{
			2,
			39
		};

		// Token: 0x04000097 RID: 151
		[JsonIgnore]
		private static readonly byte[] LinkEnd = new byte[]
		{
			2,
			39,
			7,
			207,
			1,
			1,
			1,
			byte.MaxValue,
			1,
			3
		};

		// Token: 0x04000098 RID: 152
		[JsonIgnore]
		private static readonly byte[] LinkStartTemplate = new byte[]
		{
			2,
			39,
			0,
			1,
			31,
			1,
			1,
			byte.MaxValue,
			11,
			0
		};
	}
}

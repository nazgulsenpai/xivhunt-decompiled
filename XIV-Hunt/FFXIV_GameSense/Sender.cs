using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FFXIV_GameSense
{
	public class Sender
	{
		[JsonProperty]
		public string Name { get; private set; }

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

		public Sender(string v)
		{
			this.Name = v;
		}

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

		[JsonIgnore]
		private static readonly byte[] LinkStart = new byte[]
		{
			2,
			39
		};

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

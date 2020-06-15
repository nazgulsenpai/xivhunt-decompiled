using System;

namespace FFXIV_GameSense
{
	public class Status
	{
		public short ID { get; set; }

		public short Value { get; set; }

		public float Timer { get; set; }

		public uint CasterId { get; set; }

		public override bool Equals(object obj)
		{
			return obj is Status && obj != null && this.ID.Equals(((Status)obj).ID) && this.CasterId.Equals(((Status)obj).CasterId);
		}

		public override int GetHashCode()
		{
			return (int)((uint)this.ID ^ this.CasterId);
		}
	}
}

using System;

namespace FFXIV_GameSense
{
	// Token: 0x02000076 RID: 118
	public class Status
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000338 RID: 824 RVA: 0x0000EAFA File Offset: 0x0000CCFA
		// (set) Token: 0x06000339 RID: 825 RVA: 0x0000EB02 File Offset: 0x0000CD02
		public short ID { get; set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600033A RID: 826 RVA: 0x0000EB0B File Offset: 0x0000CD0B
		// (set) Token: 0x0600033B RID: 827 RVA: 0x0000EB13 File Offset: 0x0000CD13
		public short Value { get; set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600033C RID: 828 RVA: 0x0000EB1C File Offset: 0x0000CD1C
		// (set) Token: 0x0600033D RID: 829 RVA: 0x0000EB24 File Offset: 0x0000CD24
		public float Timer { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600033E RID: 830 RVA: 0x0000EB2D File Offset: 0x0000CD2D
		// (set) Token: 0x0600033F RID: 831 RVA: 0x0000EB35 File Offset: 0x0000CD35
		public uint CasterId { get; set; }

		// Token: 0x06000340 RID: 832 RVA: 0x0000EB40 File Offset: 0x0000CD40
		public override bool Equals(object obj)
		{
			return obj is Status && obj != null && this.ID.Equals(((Status)obj).ID) && this.CasterId.Equals(((Status)obj).CasterId);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0000EB90 File Offset: 0x0000CD90
		public override int GetHashCode()
		{
			return (int)((uint)this.ID ^ this.CasterId);
		}
	}
}

using System;
using System.Globalization;
using XIVDB;

namespace FFXIV_GameSense
{
	// Token: 0x02000063 RID: 99
	public class Entity
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000E306 File Offset: 0x0000C506
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x0000E30E File Offset: 0x0000C50E
		public uint ID { get; set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x0000E317 File Offset: 0x0000C517
		// (set) Token: 0x060002C9 RID: 713 RVA: 0x0000E31F File Offset: 0x0000C51F
		public uint OwnerID { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060002CA RID: 714 RVA: 0x0000E328 File Offset: 0x0000C528
		// (set) Token: 0x060002CB RID: 715 RVA: 0x0000E330 File Offset: 0x0000C530
		public int Order { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060002CC RID: 716 RVA: 0x0000E339 File Offset: 0x0000C539
		// (set) Token: 0x060002CD RID: 717 RVA: 0x0000E341 File Offset: 0x0000C541
		public uint TargetID { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0000E34A File Offset: 0x0000C54A
		// (set) Token: 0x060002CF RID: 719 RVA: 0x0000E352 File Offset: 0x0000C552
		public string Name { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000E35B File Offset: 0x0000C55B
		// (set) Token: 0x060002D1 RID: 721 RVA: 0x0000E363 File Offset: 0x0000C563
		public float PosX { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002D2 RID: 722 RVA: 0x0000E36C File Offset: 0x0000C56C
		// (set) Token: 0x060002D3 RID: 723 RVA: 0x0000E374 File Offset: 0x0000C574
		public float PosY { get; set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002D4 RID: 724 RVA: 0x0000E37D File Offset: 0x0000C57D
		// (set) Token: 0x060002D5 RID: 725 RVA: 0x0000E385 File Offset: 0x0000C585
		public float PosZ { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002D6 RID: 726 RVA: 0x0000E38E File Offset: 0x0000C58E
		// (set) Token: 0x060002D7 RID: 727 RVA: 0x0000E396 File Offset: 0x0000C596
		public float Heading { get; set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x0000E39F File Offset: 0x0000C59F
		public float HeadingDegree
		{
			get
			{
				return (float)(((double)this.Heading + 3.1415926535897931) * 57.295779513082323);
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x0000E3BD File Offset: 0x0000C5BD
		// (set) Token: 0x060002DA RID: 730 RVA: 0x0000E3C5 File Offset: 0x0000C5C5
		public byte EffectiveDistance { get; set; }

		// Token: 0x060002DB RID: 731 RVA: 0x0000E3D0 File Offset: 0x0000C5D0
		public float GetDistanceTo(Entity target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			float num = Math.Abs(this.PosX - target.PosX);
			float distanceY = Math.Abs(this.PosY - target.PosY);
			float distanceZ = Math.Abs(this.PosZ - target.PosZ);
			return (float)Math.Sqrt((double)(num * num + distanceY * distanceY + distanceZ * distanceZ));
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000E434 File Offset: 0x0000C634
		public float GetDistanceTo(float x, float y, float z)
		{
			if (z.Equals(0f))
			{
				return this.GetHorizontalDistanceTo(x, y);
			}
			float num = Math.Abs(this.PosX - x);
			float distanceY = Math.Abs(this.PosY - y);
			float distanceZ = Math.Abs(this.PosZ - z);
			return (float)Math.Sqrt((double)(num * num + distanceY * distanceY + distanceZ * distanceZ));
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000E494 File Offset: 0x0000C694
		private float GetHorizontalDistanceTo(float x, float y)
		{
			float num = Math.Abs(this.PosX - x);
			float distanceY = Math.Abs(this.PosY - y);
			return (float)Math.Sqrt((double)(num * num + distanceY * distanceY));
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000E4CC File Offset: 0x0000C6CC
		public float GetHorizontalDistanceTo(Entity target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			float num = Math.Abs(this.PosX - target.PosX);
			float distanceY = Math.Abs(this.PosY - target.PosY);
			return (float)Math.Sqrt((double)(num * num + distanceY * distanceY));
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000E51C File Offset: 0x0000C71C
		public string GetPosReadable(ushort zoneId)
		{
			if (GameResources.GetSizeFactor(zoneId, 0) == 0)
			{
				return "Unknown size_factor: " + GameResources.GetSizeFactor(zoneId, 0).ToString(CultureInfo.CurrentCulture);
			}
			return string.Concat(new string[]
			{
				"( ",
				this.GetXReadable(zoneId, 0).ToString("0.0", CultureInfo.CurrentCulture).Replace(',', '.'),
				" , ",
				this.GetYReadable(zoneId, 0).ToString("0.0", CultureInfo.CurrentCulture).Replace(',', '.'),
				" )"
			});
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000E5C0 File Offset: 0x0000C7C0
		internal float GetXReadable(ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetXReadable(this.PosX, zoneId, mapId);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000E5CF File Offset: 0x0000C7CF
		internal static float GetXReadable(float PosX, ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetCoordReadable(PosX, zoneId, mapId);
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000E5D9 File Offset: 0x0000C7D9
		internal float GetYReadable(ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetYReadable(this.PosY, zoneId, mapId);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000E5CF File Offset: 0x0000C7CF
		internal static float GetYReadable(float PosY, ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetCoordReadable(PosY, zoneId, mapId);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000E5E8 File Offset: 0x0000C7E8
		private static float GetCoordReadable(float c, ushort zoneId, ushort mapId = 0)
		{
			c *= 0.02f;
			ushort sizeFactor = GameResources.GetSizeFactor(zoneId, mapId);
			if (sizeFactor <= 100)
			{
				if (sizeFactor != 0)
				{
					if (sizeFactor == 95)
					{
						return c + 22.5f;
					}
					if (sizeFactor != 100)
					{
						goto IL_81;
					}
					return c + 21.5f;
				}
			}
			else if (sizeFactor <= 300)
			{
				if (sizeFactor != 200)
				{
					if (sizeFactor != 300)
					{
						goto IL_81;
					}
					return c + 7.5f;
				}
			}
			else
			{
				if (sizeFactor == 400)
				{
					return c + 6f;
				}
				if (sizeFactor != 800)
				{
					goto IL_81;
				}
				return c + 3.5f;
			}
			return c + 11.25f;
			IL_81:
			return 0f;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000E67C File Offset: 0x0000C87C
		internal static float GetCoordFromReadable(float r, ushort zoneId, ushort mapId = 0)
		{
			ushort sizeFactor = GameResources.GetSizeFactor(zoneId, mapId);
			if (sizeFactor <= 100)
			{
				if (sizeFactor != 0)
				{
					if (sizeFactor == 95)
					{
						r -= 22.5f;
						goto IL_91;
					}
					if (sizeFactor != 100)
					{
						goto IL_8A;
					}
					r -= 21.5f;
					goto IL_91;
				}
			}
			else if (sizeFactor <= 300)
			{
				if (sizeFactor != 200)
				{
					if (sizeFactor != 300)
					{
						goto IL_8A;
					}
					r -= 7.5f;
					goto IL_91;
				}
			}
			else
			{
				if (sizeFactor == 400)
				{
					r -= 6f;
					goto IL_91;
				}
				if (sizeFactor != 800)
				{
					goto IL_8A;
				}
				r -= 3.5f;
				goto IL_91;
			}
			r -= 11.25f;
			goto IL_91;
			IL_8A:
			r = 0f;
			IL_91:
			return r /= 0.02f;
		}
	}
}

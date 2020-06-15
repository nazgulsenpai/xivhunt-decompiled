using System;
using System.Globalization;
using XIVDB;

namespace FFXIV_GameSense
{
	public class Entity
	{
		public uint ID { get; set; }

		public uint OwnerID { get; set; }

		public int Order { get; set; }

		public uint TargetID { get; set; }

		public string Name { get; set; }

		public float PosX { get; set; }

		public float PosY { get; set; }

		public float PosZ { get; set; }

		public float Heading { get; set; }

		public float HeadingDegree
		{
			get
			{
				return (float)(((double)this.Heading + 3.1415926535897931) * 57.295779513082323);
			}
		}

		public byte EffectiveDistance { get; set; }

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

		private float GetHorizontalDistanceTo(float x, float y)
		{
			float num = Math.Abs(this.PosX - x);
			float distanceY = Math.Abs(this.PosY - y);
			return (float)Math.Sqrt((double)(num * num + distanceY * distanceY));
		}

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

		internal float GetXReadable(ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetXReadable(this.PosX, zoneId, mapId);
		}

		internal static float GetXReadable(float PosX, ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetCoordReadable(PosX, zoneId, mapId);
		}

		internal float GetYReadable(ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetYReadable(this.PosY, zoneId, mapId);
		}

		internal static float GetYReadable(float PosY, ushort zoneId, ushort mapId = 0)
		{
			return Entity.GetCoordReadable(PosY, zoneId, mapId);
		}

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

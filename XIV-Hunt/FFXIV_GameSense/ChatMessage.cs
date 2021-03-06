﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FFXIV_GameSense.Properties;
using Newtonsoft.Json;
using XIVAPI;
using XIVDB;

namespace FFXIV_GameSense
{
	internal class ChatMessage
	{
		[JsonIgnore]
		internal DateTime Timestamp { get; set; }

		[JsonProperty]
		private uint Epoch
		{
			get
			{
				return this.Timestamp.ToEpoch();
			}
		}

		[JsonProperty]
		internal ChatChannel Channel { get; set; }

		internal ChatFilter Filter { get; set; }

		[JsonProperty]
		internal Sender Sender { get; set; }

		[JsonProperty]
		public byte[] Message { get; private set; }

		[JsonIgnore]
		internal string MessageString
		{
			get
			{
				return Encoding.UTF8.GetString(this.Message);
			}
			set
			{
				this.Message = Encoding.UTF8.GetBytes(value);
			}
		}

		internal ChatMessage()
		{
			this.Timestamp = DateTime.UtcNow;
			this.Channel = Settings.Default.ChatChannel;
			this.Filter = ChatFilter.Unknown;
			this.Message = null;
		}

		internal ChatMessage(string message)
		{
			this.Timestamp = DateTime.UtcNow;
			this.Channel = Settings.Default.ChatChannel;
			this.Filter = ChatFilter.Unknown;
			this.Message = Encoding.UTF8.GetBytes(message);
		}

		internal void PostpendToMessage(string postpend)
		{
			this.Message = this.Message.Concat(Encoding.UTF8.GetBytes(postpend)).ToArray<byte>();
		}

		internal ChatMessage(byte[] arr)
		{
			if (arr.Length < 10)
			{
				return;
			}
			this.Timestamp = ChatMessage.UnixTimeStampToDateTime(BitConverter.ToUInt32(arr.Take(4).ToArray<byte>(), 0));
			this.Channel = (ChatChannel)arr[4];
			this.Filter = (ChatFilter)arr[5];
			int msgStart;
			this.Sender = new Sender(arr.Skip(9), ref msgStart);
			this.Message = arr.Skip(9 + msgStart).ToArray<byte>();
		}

		private static DateTime UnixTimeStampToDateTime(uint unixTimeStamp)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
		}

		internal byte[] ToArray(bool onlyMessage = false)
		{
			List<byte> a = new List<byte>();
			if (!onlyMessage)
			{
				a.AddRange(BitConverter.GetBytes(this.Epoch));
				a.Add((byte)this.Channel);
				a.Add((byte)this.Filter);
				a.AddRange(new byte[2]);
				a.Add(Convert.ToByte(':'));
				if (this.Sender != null && !string.IsNullOrEmpty(this.Sender.Name))
				{
					a.AddRange(this.Sender.ToArray(true, true));
				}
				a.Add(Convert.ToByte(':'));
			}
			if (this.Message != null)
			{
				a.AddRange(ChatMessage.ReplaceTags(this.Message));
			}
			return a.ToArray();
		}

		internal static byte[] ReplaceTags(byte[] msg)
		{
			foreach (KeyValuePair<string, byte[]> kvp in ChatMessage.Tags)
			{
				msg = msg.ReplaceSequence(Encoding.UTF8.GetBytes(kvp.Key), kvp.Value);
			}
			return msg;
		}

		internal static ChatMessage MakeItemChatMessage(Item Item, string prepend = "", string postpend = "", bool HQ = false)
		{
			ChatMessage cm = new ChatMessage();
			byte[] raritycolor = ChatMessage.RarityColors.ContainsKey((int)Item.Rarity) ? ChatMessage.RarityColors[(int)Item.Rarity] : ChatMessage.RarityColors.First<KeyValuePair<int, byte[]>>().Value;
			byte[] ItemHeader1And2 = new byte[]
			{
				2,
				19,
				6,
				254,
				byte.MaxValue
			}.Concat(raritycolor).Concat(new byte[]
			{
				3,
				2,
				39,
				7,
				3,
				242
			}).ToArray<byte>();
			byte[] ItemHeaderEnd = new byte[]
			{
				2,
				1,
				3
			};
			byte[] end = new byte[]
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
				3,
				2,
				19,
				2,
				236,
				3
			};
			HQ = (HQ && Item.CanBeHq);
			if (HQ)
			{
				byte[] array = ItemHeader1And2;
				array[array.Length - 1] = 246;
				byte[] array2 = ItemHeader1And2;
				int num = array2.Length - 3;
				array2[num] += 1;
				end = ChatMessage.HQChar.Concat(end).ToArray<byte>();
				Item.Name += " ";
			}
			byte[] idba = BitConverter.GetBytes(HQ ? (Item.ID + 1000000u) : Item.ID).Reverse<byte>().SkipWhile((byte x) => x == 0).ToArray<byte>();
			if (Item.ID <= 255u)
			{
				ItemHeader1And2 = ItemHeader1And2.Take(ItemHeader1And2.Length - 1).ToArray<byte>();
				byte[] array3 = new byte[2];
				int num2 = 0;
				byte[] array4 = idba;
				int num3 = 0;
				byte b = array4[num3] + 1;
				array4[num3] = b;
				array3[num2] = b;
				array3[1] = 2;
				idba = array3;
				ItemHeaderEnd = ItemHeaderEnd.Skip(1).ToArray<byte>();
				byte[] array5 = ItemHeader1And2;
				int num4 = 11;
				array5[num4] -= 2;
			}
			else if (idba.Last<byte>() == 0)
			{
				byte[] array6 = ItemHeader1And2;
				int num5 = array6.Length - 1;
				array6[num5] -= 1;
				idba = new byte[]
				{
					idba[0]
				};
				byte[] array7 = ItemHeader1And2;
				int num6 = 11;
				array7[num6] -= 1;
			}
			ItemHeader1And2 = ItemHeader1And2.Concat(idba).ToArray<byte>();
			byte[] color = new byte[]
			{
				2,
				19,
				6,
				254,
				byte.MaxValue,
				byte.MaxValue,
				123,
				26,
				3
			};
			if (Array.IndexOf(ItemHeader1And2, 0) > -1)
			{
				throw new ArgumentException("ItemHeader contains 0x00. Params: " + Item.ID.ToString(), "Item");
			}
			cm.Message = Encoding.UTF8.GetBytes(prepend).Concat(ItemHeader1And2).Concat(ItemHeaderEnd).Concat(color).Concat(ChatMessage.arrow).Concat(Encoding.UTF8.GetBytes(Item.Name)).Concat(end).ToArray<byte>();
			if (!string.IsNullOrEmpty(postpend))
			{
				cm.Message = cm.Message.Concat(Encoding.UTF8.GetBytes(postpend)).ToArray<byte>();
			}
			return cm;
		}

		internal static ChatMessage MakePosChatMessage(string prepend, ushort zoneId, float x, float y, string postpend = "", ushort mapId = 0)
		{
			ChatMessage cm = new ChatMessage();
			byte[] pos = new byte[]
			{
				2,
				39,
				18,
				4
			};
			byte[] posZone = GameResources.GetMapMarkerZoneId(zoneId, mapId);
			byte[] posX = ChatMessage.CoordToFlagPosCoord(x);
			byte[] posY = ChatMessage.CoordToFlagPosCoord(y);
			byte[] posEnd = new byte[]
			{
				byte.MaxValue,
				1,
				3
			};
			pos = pos.Concat(posZone).Concat(posX).Concat(posY).Concat(posEnd).ToArray<byte>();
			pos[2] = Convert.ToByte(pos.Length - 3);
			byte[] color = new byte[]
			{
				2,
				19,
				6,
				254,
				byte.MaxValue,
				163,
				234,
				243,
				3
			};
			byte[] end = new byte[]
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
			if (Array.IndexOf(posEnd, 0) > -1)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"posPost contains 0x00. Params: ",
					zoneId.ToString(),
					" ",
					x.ToString(),
					" ",
					y.ToString()
				}));
			}
			if (prepend.Contains("<pos>"))
			{
				string[] source = prepend.Split(new string[]
				{
					"<pos>"
				}, 2, StringSplitOptions.None);
				prepend = source.First<string>();
				postpend = source.Last<string>() + postpend;
			}
			else if (postpend.Contains("<pos>"))
			{
				string[] split = postpend.Split(new string[]
				{
					"<pos>"
				}, 2, StringSplitOptions.None);
				prepend += split.First<string>();
				postpend = split.Last<string>();
			}
			cm.Message = Encoding.UTF8.GetBytes(prepend).Concat(pos).Concat(color).Concat(ChatMessage.arrow).Concat(Encoding.UTF8.GetBytes(string.Concat(new string[]
			{
				GameResources.GetZoneName((uint)zoneId),
				" ( ",
				Entity.GetXReadable(x, zoneId, mapId).ToString("0.0", CultureInfo.CurrentCulture).Replace(',', '.'),
				"  , ",
				Entity.GetYReadable(y, zoneId, mapId).ToString("0.0", CultureInfo.CurrentCulture).Replace(',', '.'),
				" )"
			}))).Concat(end).ToArray<byte>();
			if (!string.IsNullOrEmpty(postpend))
			{
				cm.Message = cm.Message.Concat(Encoding.UTF8.GetBytes(postpend)).ToArray<byte>();
			}
			return cm;
		}

		private static byte[] CoordToFlagPosCoord(float coordinate)
		{
			coordinate *= 1000f;
			if (coordinate == 0f)
			{
				coordinate += 1f;
			}
			byte[] t = BitConverter.GetBytes((int)coordinate);
			while (t.Last<byte>() == 0)
			{
				Array.Resize<byte>(ref t, t.Length - 1);
			}
			Array.Reverse<byte>(t);
			byte[] first;
			switch (t.Length)
			{
			case 2:
				first = new byte[]
				{
					242
				};
				break;
			case 3:
				first = new byte[]
				{
					246
				};
				break;
			case 4:
				first = new byte[]
				{
					254
				};
				break;
			default:
				first = new byte[]
				{
					242,
					1
				};
				break;
			}
			t = first.Concat(t).ToArray<byte>();
			for (int i = 0; i < t.Length; i++)
			{
				if (t[i] == 0)
				{
					byte[] array = t;
					int num = i;
					array[num] += 1;
				}
			}
			return t;
		}

		[JsonIgnore]
		private const string possep = "<pos>";

		[JsonIgnore]
		private static readonly Dictionary<string, byte[]> Tags = new Dictionary<string, byte[]>
		{
			{
				"<Emphasis>",
				new byte[]
				{
					2,
					26,
					2,
					2,
					3
				}
			},
			{
				"</Emphasis>",
				new byte[]
				{
					2,
					26,
					2,
					1,
					3
				}
			},
			{
				"<SoftHyphen/>",
				new byte[]
				{
					2,
					22,
					1,
					3
				}
			},
			{
				"<Indent/>",
				new byte[]
				{
					2,
					29,
					1,
					3
				}
			},
			{
				"<22/>",
				new byte[]
				{
					2,
					22,
					1,
					3
				}
			}
		};

		[JsonIgnore]
		private static readonly byte[] arrow = new byte[]
		{
			238,
			130,
			187,
			2,
			19,
			2,
			236,
			3
		};

		[JsonIgnore]
		private static readonly byte[] HQChar = new byte[]
		{
			238,
			128,
			188
		};

		[JsonIgnore]
		private static readonly Dictionary<int, byte[]> RarityColors = new Dictionary<int, byte[]>
		{
			{
				1,
				new byte[]
				{
					243,
					243,
					243
				}
			},
			{
				2,
				new byte[]
				{
					192,
					byte.MaxValue,
					192
				}
			},
			{
				3,
				new byte[]
				{
					89,
					144,
					byte.MaxValue
				}
			},
			{
				4,
				new byte[]
				{
					179,
					140,
					byte.MaxValue
				}
			},
			{
				7,
				new byte[]
				{
					250,
					137,
					182
				}
			}
		};
	}
}

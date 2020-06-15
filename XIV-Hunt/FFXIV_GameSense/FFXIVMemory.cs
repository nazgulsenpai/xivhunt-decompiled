using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFXIV_GameSense.IPC;
using FFXIV_GameSense.MML;
using FFXIV_GameSense.Properties;
using Splat;
using TextPlayer;
using TextPlayer.MML;
using XIVDB;

namespace FFXIV_GameSense
{
	public class FFXIVMemory : IDisposable
	{
		internal List<Entity> Combatants { get; private set; }

		private object CombatantsLock
		{
			get
			{
				return new object();
			}
		}

		internal event EventHandler<CommandEventArgs> OnNewCommand = delegate(object <p0>, CommandEventArgs <p1>)
		{
		};

		private bool Is64Bit
		{
			get
			{
				return this._mode == FFXIVMemory.FFXIVClientMode.FFXIV64;
			}
		}

		internal byte GetZoneInstance()
		{
			if (this.region != GameRegion.Global)
			{
				return this.GetByteArray(this.instanceAddress, 1u)[0];
			}
			return 0;
		}

		public FFXIVMemory(Process process)
		{
			int[] array = new int[3];
			array[0] = 64;
			array[1] = 984;
			this.chatLogStartOffset = array;
			this.chatLogTailOffset = new int[]
			{
				64,
				992
			};
			this.homeWorldIdOffset = new int[]
			{
				40,
				648
			};
			this.currentWorldIdOffset = new int[]
			{
				72,
				8,
				1976,
				556
			};
			int[] array2 = new int[2];
			array2[0] = 5880;
			this.fateListOffset = array2;
			this.mapIdOffset = new int[]
			{
				32240,
				19820
			};
			this.charmapAddress = IntPtr.Zero;
			this.targetAddress = IntPtr.Zero;
			this.fateListAddress = IntPtr.Zero;
			this.zoneIdAddress = IntPtr.Zero;
			this.mapIdAddress = IntPtr.Zero;
			this.serverTimeAddress = IntPtr.Zero;
			this.chatLogStartAddress = IntPtr.Zero;
			this.chatLogTailAddress = IntPtr.Zero;
			this.homeWorldIdAddress = IntPtr.Zero;
			this.currentWorldIdAddress = IntPtr.Zero;
			this.contentFinderConditionAddress = IntPtr.Zero;
			this.currentContentFinderConditionAddress = IntPtr.Zero;
			this.lastFailedCommandAddress = IntPtr.Zero;
			this.instanceAddress = IntPtr.Zero;
			base..ctor();
			if (process == null)
			{
				throw new ArgumentNullException("process");
			}
			this.Process = process;
			if (process.ProcessName.Equals("ffxiv", StringComparison.OrdinalIgnoreCase))
			{
				this._mode = FFXIVMemory.FFXIVClientMode.FFXIV32;
				throw new NotSupportedException("32Bit (DX9) FFXIV is no longer supported");
			}
			if (process.ProcessName.Equals("ffxiv_dx11", StringComparison.OrdinalIgnoreCase))
			{
				this._mode = FFXIVMemory.FFXIVClientMode.FFXIV64;
			}
			else
			{
				this._mode = FFXIVMemory.FFXIVClientMode.Unknown;
			}
			this.GetPointerAddress();
			this.Combatants = new List<Entity>();
			this.cts = new CancellationTokenSource();
			this._thread = new Thread(new ThreadStart(this.ScanMemoryLoop))
			{
				IsBackground = true
			};
			this._thread.Start();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.cts.Cancel();
				this.cts.Dispose();
				while (this._thread.IsAlive)
				{
					Thread.Sleep(5);
				}
				Process process = this.Process;
				if (process == null)
				{
					return;
				}
				process.Dispose();
			}
		}

		private void ScanMemoryLoop()
		{
			int interval = 50;
			uint cnt = 0u;
			while (!this.cts.IsCancellationRequested)
			{
				Thread.Sleep(interval);
				if (cnt % 10u == 0u && !this.ValidateProcess())
				{
					Thread.Sleep(1000);
					return;
				}
				this.ScanFailedCommand();
				if (cnt % 10u == 0u)
				{
					this.ScanCombatants();
				}
				if (cnt >= 4294967290u)
				{
					cnt = 0u;
				}
				else
				{
					cnt += 1u;
				}
			}
		}

		private void ScanFailedCommand()
		{
			string cmd = this.GetLastFailedCommand();
			if (string.IsNullOrWhiteSpace(cmd))
			{
				return;
			}
			this.WipeLastFailedCommand(62);
			Command command;
			if (cmd.StartsWith("/", StringComparison.Ordinal) && cmd.Length > 1 && Enum.TryParse<Command>(cmd.Split(' ', StringSplitOptions.None)[0].Substring(1), true, out command))
			{
				CommandEventArgs cmdargs = new CommandEventArgs(command, cmd.Substring(cmd.IndexOf(' ') + 1).Trim());
				this.OnNewCommand(this, cmdargs);
			}
		}

		private void ScanCombatants()
		{
			List<Entity> c = this.GetCombatantList();
			object combatantsLock = this.CombatantsLock;
			lock (combatantsLock)
			{
				this.Combatants = c;
			}
		}

		public Process Process { get; }

		public bool ValidateProcess()
		{
			return this.Process != null && !this.Process.HasExited && ((!(this.charmapAddress == IntPtr.Zero) && !(this.targetAddress == IntPtr.Zero) && !(this.homeWorldIdAddress == IntPtr.Zero) && this.IsValidServerId()) || this.GetPointerAddress());
		}

		private bool GetPointerAddress()
		{
			List<string> fail = new List<string>();
			bool bRIP = this.Is64Bit;
			List<IntPtr> list = this.SigScan("e8********488bbc24********488b7424**488b0d", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.homeWorldIdAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.homeWorldIdAddress = this.ResolvePointerPath(list[0], this.homeWorldIdOffset);
			}
			if (this.homeWorldIdAddress == IntPtr.Zero)
			{
				fail.Add("homeWorldIdAddress");
			}
			if (GameResources.IsChineseWorld(this.GetHomeWorldId()))
			{
				this.region = GameRegion.Chinese;
			}
			else if (GameResources.IsKoreanWorld(this.GetHomeWorldId()))
			{
				this.region = GameRegion.Korean;
			}
			else
			{
				this.region = GameRegion.Global;
			}
			if (this.region != GameRegion.Global)
			{
				this.currentContentFinderConditionSignature = "75**33d2488d0d********e8********48393d";
				this.currentContentFinderConditionOffset = 12;
				this.mapIdOffset = new int[]
				{
					30200,
					19820
				};
				this.chatLogStartOffset[0] = (this.chatLogTailOffset[0] = 48);
				this.currentWorldIdOffset[2] -= 8;
				this.serverTimeOffset[0] = 5912;
				this.serverTimeOffset[2] -= 8;
			}
			this.combatantOffsets = new CombatantOffsets(this.region);
			this.contentFinderOffsets = new ContentFinderOffsets(this.region);
			LogHost.Default.Info(this.region.ToString() + " (DX" + (this.Is64Bit ? "11" : "9") + ") game detected.");
			list = this.SigScan("48c741**********48890d", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.currentWorldIdAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.currentWorldIdAddress = this.ResolvePointerPath(list[0], this.currentWorldIdOffset);
			}
			if (this.currentWorldIdAddress == IntPtr.Zero)
			{
				fail.Add("currentWorldIdAddress");
			}
			list = this.SigScan("488b42**48c1e8**3d********77**8bc0488d0d", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.charmapAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.charmapAddress = list[0];
			}
			if (this.charmapAddress == IntPtr.Zero)
			{
				fail.Add("charmapAddress");
			}
			if (this.region == GameRegion.Chinese || this.region == GameRegion.Korean)
			{
				list = this.SigScan("33ff44897c24**488b0d", 0, bRIP, false);
				if (list == null || list.Count == 0)
				{
					this.instanceAddress = IntPtr.Zero;
				}
				if (list.Count == 1)
				{
					this.instanceAddress = this.ResolvePointerPath(list[0], new int[]
					{
						1580
					});
				}
				if (this.instanceAddress == IntPtr.Zero)
				{
					fail.Add("instanceAddress");
				}
			}
			list = this.SigScan("5fc3483935********75**483935", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.targetAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.targetAddress = list[0];
			}
			if (this.targetAddress == IntPtr.Zero)
			{
				fail.Add("targetAddress");
			}
			list = this.SigScan("e8********f30f108d********4c8d85********0fb705", 0, bRIP, false);
			if (list.Count == 1)
			{
				this.zoneIdAddress = list[0];
			}
			if (this.zoneIdAddress == IntPtr.Zero)
			{
				fail.Add("zoneIdAddress");
			}
			list = this.SigScan("74**488b42**488905********48890d", 0, bRIP, false);
			if (list.Count == 1)
			{
				this.mapIdAddress = this.ResolvePointerPath(list[0], this.mapIdOffset);
			}
			if (this.mapIdAddress == IntPtr.Zero)
			{
				fail.Add("mapIdAddress");
			}
			list = this.SigScan("488b10488bc8ff52**488bf8488b0d", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.serverTimeAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.serverTimeAddress = this.ResolvePointerPath(list[0], this.serverTimeOffset);
			}
			if (this.serverTimeAddress == IntPtr.Zero)
			{
				fail.Add("serverTimeAddress");
			}
			list = this.SigScan("e8********85c0740e488b0d********33D2E8********488b0d", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.chatLogStartAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.chatLogStartAddress = this.ResolvePointerPath(list[0], this.chatLogStartOffset);
				this.chatLogTailAddress = this.ResolvePointerPath(list[0], this.chatLogTailOffset);
			}
			if (this.chatLogStartAddress == IntPtr.Zero)
			{
				fail.Add("chatLogStartAddress");
			}
			if (this.chatLogTailAddress == IntPtr.Zero)
			{
				fail.Add("chatLogTailAddress");
			}
			list = this.SigScan("be********488bcbe8********4881c3********4883ee**75**488b05", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.fateListAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.fateListAddress = list[0];
			}
			if (this.fateListAddress == IntPtr.Zero)
			{
				fail.Add("fateListAddress");
			}
			list = this.SigScan(this.currentContentFinderConditionSignature, 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.currentContentFinderConditionAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.currentContentFinderConditionAddress = list[0] + this.currentContentFinderConditionOffset;
			}
			if (this.currentContentFinderConditionAddress == IntPtr.Zero)
			{
				fail.Add("currentContentFinderConditionAddress");
			}
			list = this.SigScan("440fb643**488d51**488d0d", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.contentFinderConditionAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.contentFinderConditionAddress = list[0] + 244;
			}
			if (this.contentFinderConditionAddress == IntPtr.Zero)
			{
				fail.Add("contentFinderConditionAddress");
			}
			list = this.SigScan("0f84********488b07488bcfff50**488bc8e8********488bc84c8d05", 0, bRIP, false);
			if (list == null || list.Count == 0)
			{
				this.lastFailedCommandAddress = IntPtr.Zero;
			}
			if (list.Count == 1)
			{
				this.lastFailedCommandAddress = list[0];
			}
			if (this.lastFailedCommandAddress == IntPtr.Zero)
			{
				fail.Add("lastFailedCommandAddress");
			}
			if (this.GetSelfCombatant() == null)
			{
				throw new MemoryScanException(string.Format(CultureInfo.CurrentCulture, Resources.FailedToSigScan, "charmapAddress"));
			}
			if (fail.Any<string>())
			{
				throw new MemoryScanException(string.Format(CultureInfo.CurrentCulture, Resources.FailedToSigScan, string.Join(",", fail)));
			}
			return !fail.Any<string>();
		}

		private void WipeLastFailedCommand(byte len = 62)
		{
			if (len > 62)
			{
				len = 62;
			}
			byte[] arr = new byte[(int)len];
			uint num;
			NativeMethods.WriteProcessMemory(this.Process.Handle, new IntPtr((long)this.GetUInt64(this.lastFailedCommandAddress, 0)), arr, new IntPtr(arr.Length), out num);
		}

		private string GetLastFailedCommand()
		{
			return FFXIVMemory.GetStringFromBytes(this.GetByteArray(new IntPtr((long)this.GetUInt64(this.lastFailedCommandAddress, 0)), 70u), 0, 70);
		}

		internal ushort GetCurrentContentFinderCondition()
		{
			return this.GetUInt16(this.currentContentFinderConditionAddress, 0);
		}

		internal ContentFinder GetContentFinder()
		{
			byte[] ba = this.GetByteArray(this.contentFinderConditionAddress, 256u);
			return new ContentFinder
			{
				ContentFinderConditionID = BitConverter.ToUInt16(ba, 0),
				State = (ContentFinderState)((this.region != GameRegion.Global) ? (ba[this.contentFinderOffsets.StateOffset] + 1) : ba[this.contentFinderOffsets.StateOffset]),
				RouletteID = ba[this.contentFinderOffsets.RouletteIdOffset]
			};
		}

		internal List<ChatMessage> ReadChatLogBackwards(ushort count = 1000, Predicate<ChatMessage> filter = null, Predicate<ChatMessage> stopOn = null)
		{
			List<ChatMessage> ChatLog = new List<ChatMessage>();
			ulong length = this.GetUInt64(this.chatLogTailAddress, 0) - (ulong)this.chatLogStartAddress.ToInt64();
			byte[] ws = this.GetByteArray(this.chatLogStartAddress, (uint)length);
			int currentStart = ws.Length;
			int currentEnd = ws.Length;
			while (currentStart > 0 && count > 0)
			{
				currentStart--;
				if (ws[currentStart] == 0 && ws[currentStart - 1] == 0)
				{
					currentStart -= 7;
					ChatMessage cm = new ChatMessage(ws.Skip(currentStart).Take(currentEnd - currentStart).ToArray<byte>());
					if (stopOn != null && stopOn(cm))
					{
						break;
					}
					if (filter == null || filter(cm))
					{
						ChatLog.Add(cm);
					}
					currentEnd = currentStart;
					count -= 1;
				}
			}
			ChatLog.Reverse();
			return ChatLog;
		}

		private async Task TryInject()
		{
			string pathToDLLFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Hunt_x64.dll");
			GameRegion gameRegion = this.region;
			byte[] array;
			if (gameRegion != GameRegion.Chinese)
			{
				if (gameRegion != GameRegion.Korean)
				{
					array = Resources.Hunt_x64;
				}
				else
				{
					array = Resources.Hunt_x64_KR;
				}
			}
			else
			{
				array = Resources.Hunt_x64_CN;
			}
			byte[] embeddedDLL = array;
			if (!File.Exists(pathToDLLFile) || !File.ReadAllBytes(pathToDLLFile).SequenceEqual(embeddedDLL))
			{
				try
				{
					File.WriteAllBytes(pathToDLLFile, embeddedDLL);
				}
				catch (Exception e)
				{
					LogHost.Default.InfoException("TryInject", e);
				}
			}
			if (!PersistentNamedPipeServer.Instance.IsConnected)
			{
				if (!pathToDLLFile.ContainsNonANSICharacters())
				{
					LogHost.Default.Info("Injecting code using LoadLibraryA");
					NativeMethods.InjectDLL(this.Process, pathToDLLFile, NativeMethods.LoadLibraryVariant.LoadLibraryA);
					await FFXIVMemory.WaitForNamedPipeServerConnection();
				}
				if (!PersistentNamedPipeServer.Instance.IsConnected)
				{
					LogHost.Default.Info("Injecting code using LoadLibraryW");
					NativeMethods.InjectDLL(this.Process, pathToDLLFile, NativeMethods.LoadLibraryVariant.LoadLibraryW);
					await FFXIVMemory.WaitForNamedPipeServerConnection();
				}
			}
		}

		private static async Task WaitForNamedPipeServerConnection()
		{
			int w = 0;
			while (!PersistentNamedPipeServer.Instance.IsConnected && w < 1000)
			{
				await Task.Delay(100).ConfigureAwait(false);
				w += 100;
			}
			if (PersistentNamedPipeServer.Instance.IsConnected)
			{
				LogHost.Default.Info("IPC connected");
			}
			else
			{
				LogHost.Default.Warn("IPC failed or took too long.");
			}
		}

		internal async Task WriteChatMessage(ChatMessage cm)
		{
			await this.TryInject().ConfigureAwait(false);
			PipeMessage pipeMessage = new PipeMessage(this.Process.Id, PMCommand.PrintMessage, new ChatMessage(cm, 0u, 1).ToByteArray<ChatMessage>());
			LogHost.Default.Debug("Writing: " + cm.MessageString + Environment.NewLine + BitConverter.ToString(cm.Message));
			PersistentNamedPipeServer.SendPipeMessage(pipeMessage);
		}

		private IntPtr ResolvePointerPath(IntPtr address, IEnumerable<int> offsets)
		{
			ulong bytes = this.GetUInt64(address, 0);
			foreach (int offset in offsets)
			{
				address = IntPtr.Add((IntPtr)((long)bytes), offset);
				bytes = this.GetUInt64(address, 0);
			}
			return address;
		}

		internal DateTime GetServerUtcTime()
		{
			byte[] ba = this.GetByteArray(this.serverTimeAddress, 6u);
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(BitConverter.ToUInt32(ba, 0)).AddMilliseconds((double)BitConverter.ToUInt16(ba, 4));
		}

		public unsafe Entity GetTargetCombatant()
		{
			byte[] source = this.GetByteArray(this.targetAddress, 128u);
			byte[] array;
			byte* p;
			if ((array = source) == null || array.Length == 0)
			{
				p = null;
			}
			else
			{
				p = &array[0];
			}
			IntPtr address = new IntPtr(*(long*)p);
			array = null;
			if (address.ToInt64() <= 0L)
			{
				return null;
			}
			source = this.GetByteArray(address, 16192u);
			return this.GetEntityFromByteArray(source);
		}

		public PC GetSelfCombatant()
		{
			PC self = null;
			IntPtr address = (IntPtr)((long)this.GetUInt64(this.charmapAddress, 0));
			if (address.ToInt64() > 0L)
			{
				byte[] source = this.GetByteArray(address, 16192u);
				self = (PC)this.GetEntityFromByteArray(source);
			}
			return self;
		}

		private unsafe ulong GetUInt64(IntPtr address, int offset = 0)
		{
			byte[] value = new byte[8];
			this.Peek(IntPtr.Add(address, offset), value);
			ulong result;
			fixed (byte* ptr = &value[0])
			{
				result = (ulong)(*(long*)ptr);
			}
			return result;
		}

		private bool IsValidServerId()
		{
			if (!new ushort[]
			{
				this.GetHomeWorldId(),
				this.GetCurrentWorldId()
			}.All((ushort x) => GameResources.IsValidWorldID(x)))
			{
				LogHost.Default.Warn("World IDs read from memory are invalid");
				return false;
			}
			return true;
		}

		internal ushort GetCurrentWorldId()
		{
			return this.GetUInt16(this.currentWorldIdAddress, 0);
		}

		internal ushort GetHomeWorldId()
		{
			return this.GetUInt16(this.homeWorldIdAddress, 0);
		}

		internal List<FATE> GetFateList()
		{
			IntPtr liststart = this.ResolvePointerPath(this.fateListAddress, this.fateListOffset);
			List<IntPtr> fatePtrs = new List<IntPtr>(8);
			List<FATE> fates = new List<FATE>(8);
			int size = 64;
			for (int i = 0; i < size; i += 8)
			{
				IntPtr ptr = (IntPtr)((long)this.GetUInt64(liststart, i));
				if (ptr.Equals(IntPtr.Zero))
				{
					break;
				}
				fatePtrs.Add(ptr);
			}
			ushort currentZone = this.GetZoneId();
			if (currentZone == 0)
			{
				return fates;
			}
			foreach (IntPtr ptr2 in fatePtrs.Distinct<IntPtr>())
			{
				FATE f = this.GetFateFromByteArray(this.GetByteArray(ptr2, 2376u));
				if (f == null)
				{
					break;
				}
				if (!fates.Contains(f) && f.ZoneID == currentZone)
				{
					fates.Add(f);
				}
			}
			return fates;
		}

		private FATE GetFateFromByteArray(byte[] ba)
		{
			int offset = (this.region == GameRegion.Global) ? 1056 : 1024;
			FATE f = new FATE
			{
				ID = BitConverter.ToUInt16(ba, 24),
				StartTimeEpoch = BitConverter.ToUInt32(ba, 32),
				Duration = BitConverter.ToUInt16(ba, 40),
				ReadName = FFXIVMemory.GetStringFromBytes(ba, 226, 256),
				State = (FATEState)ba[940],
				Progress = ba[(this.region == GameRegion.Global) ? 952 : 947],
				PosX = BitConverter.ToSingle(ba, offset),
				PosZ = BitConverter.ToSingle(ba, offset += 4),
				PosY = BitConverter.ToSingle(ba, offset += 4),
				ZoneID = BitConverter.ToUInt16(ba, offset + 540)
			};
			if (f.ID == 0 || f.Progress < 0 || f.Progress > 100 || !f.PosX.IsWithin(-1024, 1024) || !f.PosY.IsWithin(-1024, 1024))
			{
				return null;
			}
			return f;
		}

		internal unsafe List<Entity> GetCombatantList()
		{
			uint num = 344u;
			List<Entity> result = new List<Entity>();
			uint sz = 8u;
			byte[] source = this.GetByteArray(this.charmapAddress, sz * num);
			if (source == null || source.Length == 0)
			{
				return result;
			}
			int i = 0;
			while ((long)i < (long)((ulong)num))
			{
				byte[] array;
				byte* bp;
				if ((array = source) == null || array.Length == 0)
				{
					bp = null;
				}
				else
				{
					bp = &array[0];
				}
				IntPtr p = new IntPtr(*(long*)(bp + (long)i * (long)((ulong)sz)));
				array = null;
				if (!(p == IntPtr.Zero))
				{
					byte[] c = this.GetByteArray(p, 9680u);
					Entity entity = this.GetEntityFromByteArray(c);
					if (!(entity is Minion) && !(entity is Housing) && !(entity is GatheringPoint) && !(entity is NPC) && !(entity is Area) && !(entity is Retainer) && entity.ID != 0u && entity.ID != 3758096384u && !result.Exists((Entity x) => x.ID == entity.ID))
					{
						entity.Order = i;
						result.Add(entity);
					}
				}
				i++;
			}
			return result;
		}

		internal unsafe Entity GetEntityFromByteArray(byte[] source)
		{
			Entity entity;
			fixed (byte[] array = source)
			{
				byte* p;
				if (source == null || array.Length == 0)
				{
					p = null;
				}
				else
				{
					p = &array[0];
				}
				entity = (Entity)Activator.CreateInstance(FFXIVMemory.ObjectTypeMap[FFXIVMemory.ObjectTypeMap.ContainsKey(p[this.combatantOffsets.Type]) ? p[this.combatantOffsets.Type] : 0]);
				entity.Name = FFXIVMemory.GetStringFromBytes(source, this.combatantOffsets.Name, 256);
				entity.ID = *(uint*)(p + this.combatantOffsets.ID);
				entity.OwnerID = *(uint*)(p + this.combatantOffsets.OwnerID);
				if (entity.OwnerID == 3758096384u)
				{
					entity.OwnerID = 0u;
				}
				entity.EffectiveDistance = p[this.combatantOffsets.EffectiveDistance];
				entity.PosX = *(float*)(p + this.combatantOffsets.PosX);
				entity.PosZ = *(float*)(p + this.combatantOffsets.PosZ);
				entity.PosY = *(float*)(p + this.combatantOffsets.PosY);
				entity.Heading = *(float*)(p + this.combatantOffsets.Heading);
				if (entity is EObject)
				{
					IntPtr eventTypeAddr = *(IntPtr*)(p + this.combatantOffsets.EventType);
					((EObject)entity).SubType = (EObjType)this.GetUInt16(eventTypeAddr, 4);
					if (((EObject)entity).SubType == EObjType.CairnOfPassage || ((EObject)entity).SubType == EObjType.CairnOfReturn || ((EObject)entity).SubType == EObjType.BeaconOfPassage || ((EObject)entity).SubType == EObjType.BeaconOfReturn)
					{
						((EObject)entity).CairnIsUnlocked = (p[this.combatantOffsets.CairnIsUnlocked] == 4);
					}
				}
				if (entity is Monster)
				{
					((Monster)entity).FateID = *(uint*)(p + this.combatantOffsets.FateID);
					((Monster)entity).BNpcNameID = *(ushort*)(p + this.combatantOffsets.BNpcNameID);
				}
				entity.TargetID = *(uint*)(p + this.combatantOffsets.TargetID);
				if (entity.TargetID == 3758096384u)
				{
					entity.TargetID = *(uint*)(p + this.combatantOffsets.TargetID2);
				}
				if (entity is Combatant)
				{
					((Combatant)entity).Job = (JobEnum)p[this.combatantOffsets.Job];
					((Combatant)entity).Level = p[this.combatantOffsets.Level];
					((Combatant)entity).CurrentHP = *(uint*)(p + this.combatantOffsets.CurrentHP);
					((Combatant)entity).MaxHP = *(uint*)(p + this.combatantOffsets.MaxHP);
					((Combatant)entity).CurrentMP = *(uint*)(p + this.combatantOffsets.CurrentMP);
					((Combatant)entity).CurrentGP = *(ushort*)(p + this.combatantOffsets.CurrentGP);
					((Combatant)entity).MaxGP = *(ushort*)(p + this.combatantOffsets.MaxGP);
					((Combatant)entity).CurrentCP = *(ushort*)(p + this.combatantOffsets.CurrentCP);
					((Combatant)entity).MaxCP = *(ushort*)(p + this.combatantOffsets.MaxCP);
					int offset = this.combatantOffsets.StatusEffectsStart;
					for (int countedStatusEffects = 0; countedStatusEffects < 32; countedStatusEffects++)
					{
						Status status = new Status
						{
							ID = *(short*)(p + offset)
						};
						if (status.ID != 0)
						{
							status.Value = *(short*)(p + (offset + 2));
							status.Timer = *(float*)(p + (offset + 4));
							status.CasterId = *(uint*)(p + (offset + 8));
							(entity as Combatant).StatusList.Add(status);
						}
						offset += 12;
					}
				}
			}
			return entity;
		}

		internal async Task PlayPerformance(Performance p, CancellationToken performCancelationToken)
		{
			if (!PersistentNamedPipeServer.Instance.IsConnected)
			{
				await this.TryInject().ConfigureAwait(false);
			}
			await p.Play(this.Process.Id, performCancelationToken).ConfigureAwait(false);
		}

		internal async Task PlayMML(ImplementedPlayer p, CancellationToken performCancelationToken)
		{
			p.Unmute();
			p.Play();
			if (!PersistentNamedPipeServer.Instance.IsConnected)
			{
				await this.TryInject().ConfigureAwait(false);
			}
			PipeMessage noteOff = new PipeMessage(this.Process.Id, PMCommand.PlayNote, 0);
			using (List<MMLPlayerTrack>.Enumerator enumerator = p.Tracks.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MMLPlayerTrack track = enumerator.Current;
					new Thread(delegate()
					{
						TimeSpan ts = new TimeSpan(0L);
						foreach (Note note in track.notes)
						{
							TimeSpan w = note.TimeSpan - ts;
							if (w.TotalMilliseconds > 0.0)
							{
								Thread.Sleep(w);
							}
							if (performCancelationToken.IsCancellationRequested)
							{
								break;
							}
							PersistentNamedPipeServer.SendPipeMessage(new PipeMessage(this.Process.Id, PMCommand.PlayNote, note.GetStep()));
							Thread.Sleep(note.Length);
							PersistentNamedPipeServer.SendPipeMessage(noteOff);
							ts = note.TimeSpan + note.Length;
						}
					})
					{
						Priority = ThreadPriority.Highest,
						IsBackground = true
					}.Start();
				}
			}
		}

		private static string GetStringFromBytes(byte[] source, int offset = 0, int size = 256)
		{
			byte[] bytes = new byte[size];
			Array.Copy(source, offset, bytes, 0, size);
			int realSize = 0;
			for (int i = 0; i < size; i++)
			{
				if (bytes[i] == 0)
				{
					realSize = i;
					break;
				}
			}
			Array.Resize<byte>(ref bytes, realSize);
			return Encoding.UTF8.GetString(bytes);
		}

		private bool Peek(IntPtr address, byte[] buffer)
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr nSize = new IntPtr(buffer.Length);
			return NativeMethods.ReadProcessMemory(this.Process.Handle, address, buffer, nSize, ref zero);
		}

		public byte[] GetByteArray(IntPtr address, uint length)
		{
			byte[] data = new byte[length];
			this.Peek(address, data);
			return data;
		}

		private unsafe int GetInt32(IntPtr address, int offset = 0)
		{
			byte[] value = new byte[4];
			this.Peek(IntPtr.Add(address, offset), value);
			int result;
			fixed (byte* ptr = &value[0])
			{
				result = *(int*)ptr;
			}
			return result;
		}

		private unsafe uint GetUInt32(IntPtr address, int offset = 0)
		{
			byte[] value = new byte[4];
			this.Peek(IntPtr.Add(address, offset), value);
			uint result;
			fixed (byte* ptr = &value[0])
			{
				result = *(uint*)ptr;
			}
			return result;
		}

		private unsafe ushort GetUInt16(IntPtr address, int offset = 0)
		{
			byte[] value = new byte[2];
			this.Peek(IntPtr.Add(address, offset), value);
			ushort result;
			fixed (byte* ptr = &value[0])
			{
				result = *(ushort*)ptr;
			}
			return result;
		}

		private List<IntPtr> SigScan(string pattern, int offset = 0, bool bRIP = false, bool noFollow = false)
		{
			if (pattern == null || pattern.Length % 2 != 0)
			{
				return new List<IntPtr>();
			}
			byte?[] array = new byte?[pattern.Length / 2];
			for (int i = 0; i < pattern.Length / 2; i++)
			{
				string text = pattern.Substring(i * 2, 2);
				if (text == "**")
				{
					array[i] = null;
				}
				else
				{
					array[i] = new byte?(Convert.ToByte(text, 16));
				}
			}
			int num = 65536;
			IntPtr baseAddress = this.Process.MainModule.BaseAddress;
			IntPtr intPtr = IntPtr.Add(baseAddress, this.Process.MainModule.ModuleMemorySize);
			IntPtr intPtr2 = baseAddress;
			byte[] array2 = new byte[num];
			List<IntPtr> list = new List<IntPtr>();
			while (intPtr2.ToInt64() < intPtr.ToInt64())
			{
				IntPtr zero = IntPtr.Zero;
				IntPtr nSize = new IntPtr(num);
				if (IntPtr.Add(intPtr2, num).ToInt64() > intPtr.ToInt64())
				{
					nSize = (IntPtr)(intPtr.ToInt64() - intPtr2.ToInt64());
				}
				if (NativeMethods.ReadProcessMemory(this.Process.Handle, intPtr2, array2, nSize, ref zero))
				{
					int num2 = 0;
					while ((long)num2 < zero.ToInt64() - (long)array.Length - (long)offset - 4L + 1L)
					{
						int num3 = 0;
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j] == null)
							{
								num3++;
							}
							else
							{
								if (array[j].Value != array2[num2 + j])
								{
									break;
								}
								num3++;
							}
						}
						if (num3 == array.Length)
						{
							IntPtr item;
							if (noFollow)
							{
								item = IntPtr.Add(intPtr2, num2);
							}
							else if (bRIP)
							{
								item = new IntPtr(BitConverter.ToInt32(array2, num2 + array.Length + offset));
								item = new IntPtr(intPtr2.ToInt64() + (long)num2 + (long)array.Length + 4L + item.ToInt64());
							}
							else if (this.Is64Bit)
							{
								item = new IntPtr(BitConverter.ToInt64(array2, num2 + array.Length + offset));
								item = new IntPtr(item.ToInt64());
							}
							else
							{
								item = new IntPtr(BitConverter.ToInt32(array2, num2 + array.Length + offset));
								item = new IntPtr(item.ToInt64());
							}
							list.Add(item);
						}
						num2++;
					}
				}
				intPtr2 = IntPtr.Add(intPtr2, num);
			}
			return list;
		}

		public ushort GetZoneId()
		{
			return this.GetUInt16(this.zoneIdAddress, 0);
		}

		public ushort GetMapId()
		{
			return this.GetUInt16(this.mapIdAddress, 0);
		}

		private readonly Thread _thread;

		private readonly CancellationTokenSource cts;

		private static readonly Dictionary<byte, Type> ObjectTypeMap = new Dictionary<byte, Type>
		{
			{
				0,
				typeof(Entity)
			},
			{
				1,
				typeof(PC)
			},
			{
				2,
				typeof(Monster)
			},
			{
				3,
				typeof(NPC)
			},
			{
				4,
				typeof(Treasure)
			},
			{
				5,
				typeof(Aetheryte)
			},
			{
				6,
				typeof(GatheringPoint)
			},
			{
				7,
				typeof(EObject)
			},
			{
				8,
				typeof(Mount)
			},
			{
				9,
				typeof(Minion)
			},
			{
				10,
				typeof(Retainer)
			},
			{
				11,
				typeof(Area)
			},
			{
				12,
				typeof(Housing)
			},
			{
				13,
				typeof(Cutscene)
			},
			{
				14,
				typeof(CardStand)
			}
		};

		private const string charmapSignature = "488b42**48c1e8**3d********77**8bc0488d0d";

		private const string targetSignature = "5fc3483935********75**483935";

		private const string zoneIdSignature = "e8********f30f108d********4c8d85********0fb705";

		private const string mapIdSignature = "74**488b42**488905********48890d";

		private const string serverTimeSignature = "488b10488bc8ff52**488bf8488b0d";

		private const string chatLogStartSignature = "e8********85c0740e488b0d********33D2E8********488b0d";

		private const string fateListSignature = "be********488bcbe8********4881c3********4883ee**75**488b05";

		private const string contentFinderConditionSignature = "440fb643**488d51**488d0d";

		private const string homeWorldIdSignature = "e8********488bbc24********488b7424**488b0d";

		private const string currentWorldIdSignature = "48c741**********48890d";

		private const string lastFailedCommandSignature = "0f84********488b07488bcfff50**488bc8e8********488bc84c8d05";

		private string currentContentFinderConditionSignature = "48899f********48899f********48899f********48899f********899f********0fb71d";

		private const string instanceSignature = "33ff44897c24**488b0d";

		private const int contentFinderConditionOffset = 244;

		private int currentContentFinderConditionOffset;

		private const int instanceOffset = 1580;

		private readonly int[] serverTimeOffset = new int[]
		{
			5384,
			8,
			2124
		};

		private readonly int[] chatLogStartOffset;

		private readonly int[] chatLogTailOffset;

		private readonly int[] homeWorldIdOffset;

		private readonly int[] currentWorldIdOffset;

		private readonly int[] fateListOffset;

		private int[] mapIdOffset;

		private readonly FFXIVMemory.FFXIVClientMode _mode;

		private GameRegion region;

		private CombatantOffsets combatantOffsets;

		private ContentFinderOffsets contentFinderOffsets;

		private IntPtr charmapAddress;

		private IntPtr targetAddress;

		private IntPtr fateListAddress;

		private IntPtr zoneIdAddress;

		private IntPtr mapIdAddress;

		private IntPtr serverTimeAddress;

		private IntPtr chatLogStartAddress;

		private IntPtr chatLogTailAddress;

		private IntPtr homeWorldIdAddress;

		private IntPtr currentWorldIdAddress;

		private IntPtr contentFinderConditionAddress;

		private IntPtr currentContentFinderConditionAddress;

		private IntPtr lastFailedCommandAddress;

		private IntPtr instanceAddress;

		public enum FFXIVClientMode
		{
			Unknown,
			FFXIV32,
			FFXIV64
		}
	}
}

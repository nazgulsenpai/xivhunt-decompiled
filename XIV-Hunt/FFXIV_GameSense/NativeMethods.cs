using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using FFXIV_GameSense.IPC;
using Splat;

namespace FFXIV_GameSense
{
	// Token: 0x0200008C RID: 140
	internal static class NativeMethods
	{
		// Token: 0x060003B1 RID: 945
		[DllImport("kernel32.dll")]
		internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);

		// Token: 0x060003B2 RID: 946
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out uint lpNumberOfBytesWritten);

		// Token: 0x060003B3 RID: 947
		[DllImport("kernel32.dll")]
		internal static extern int CloseHandle(IntPtr hProcess);

		// Token: 0x060003B4 RID: 948
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, NativeMethods.AllocationType flAllocationType, NativeMethods.MemoryProtection flProtect);

		// Token: 0x060003B5 RID: 949
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		// Token: 0x060003B6 RID: 950
		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		// Token: 0x060003B7 RID: 951
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

		// Token: 0x060003B8 RID: 952
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x060003B9 RID: 953
		[DllImport("kernel32.dll")]
		internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

		// Token: 0x060003BA RID: 954
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref NativeMethods.FLASHWINFO pwfi);

		// Token: 0x060003BB RID: 955
		[DllImport("user32.dll")]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		// Token: 0x060003BC RID: 956
		[DllImport("user32.dll")]
		internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		// Token: 0x060003BD RID: 957
		[DllImport("user32.dll")]
		internal static extern bool IsIconic(IntPtr hWnd);

		// Token: 0x060003BE RID: 958
		[DllImport("shell32.dll", SetLastError = true)]
		internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

		// Token: 0x060003BF RID: 959
		[DllImport("user32.dll")]
		internal static extern IntPtr GetForegroundWindow();

		// Token: 0x060003C0 RID: 960
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		// Token: 0x060003C1 RID: 961
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(IntPtr hWnd, out NativeMethods.RECT lpRect);

		// Token: 0x060003C2 RID: 962
		[DllImport("kernel32.dll")]
		private static extern uint GetSystemDefaultLCID();

		// Token: 0x060003C3 RID: 963 RVA: 0x00011DBE File Offset: 0x0000FFBE
		internal static CultureInfo GetSystemDefaultCultureInfo()
		{
			return new CultureInfo((int)NativeMethods.GetSystemDefaultLCID(), true);
		}

		// Token: 0x060003C4 RID: 964
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		// Token: 0x060003C5 RID: 965
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		// Token: 0x060003C6 RID: 966 RVA: 0x00011DCB File Offset: 0x0000FFCB
		internal static IntPtr GetWindowLongPtr3264(IntPtr hWnd, int nIndex)
		{
			if (Environment.Is64BitOperatingSystem)
			{
				return NativeMethods.GetWindowLongPtr(hWnd, nIndex);
			}
			return NativeMethods.GetWindowLong(hWnd, nIndex);
		}

		// Token: 0x060003C7 RID: 967
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		// Token: 0x060003C8 RID: 968
		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x060003C9 RID: 969 RVA: 0x00011DE3 File Offset: 0x0000FFE3
		internal static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong)
		{
			if (Environment.Is64BitOperatingSystem)
			{
				return NativeMethods.SetWindowLongPtr(hWnd, nIndex, new IntPtr(dwNewLong));
			}
			return new IntPtr(NativeMethods.SetWindowLong(hWnd, nIndex, (int)dwNewLong));
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00011E08 File Offset: 0x00010008
		internal static void InjectDLL(Process Process, string DLLName, NativeMethods.LoadLibraryVariant loadLibraryVariant)
		{
			IntPtr hProcess = Process.Handle;
			byte[] DLLNameAsBytes = (loadLibraryVariant == NativeMethods.LoadLibraryVariant.LoadLibraryW) ? Encoding.Unicode.GetBytes(DLLName) : Encoding.Default.GetBytes(DLLName);
			IntPtr LenWrite = new IntPtr(DLLNameAsBytes.Length + 1);
			IntPtr AllocMem = NativeMethods.VirtualAllocEx(hProcess, (IntPtr)null, LenWrite, NativeMethods.AllocationType.Commit, NativeMethods.MemoryProtection.ExecuteReadWrite);
			uint num;
			NativeMethods.WriteProcessMemory(hProcess, AllocMem, DLLNameAsBytes, LenWrite, out num);
			IntPtr Injector = NativeMethods.GetProcAddress(NativeMethods.GetModuleHandle("kernel32.dll"), loadLibraryVariant.ToString());
			IntPtr hThread = NativeMethods.CreateRemoteThread(hProcess, (IntPtr)null, IntPtr.Zero, Injector, AllocMem, 0u, out num);
			uint Result = NativeMethods.WaitForSingleObject(hThread, 10000u);
			if ((ulong)Result == 128UL || (ulong)Result == 258UL || Result == 4294967295u)
			{
				LogHost.Default.Error("hThread [ 2 ] Error!");
				NativeMethods.CloseHandle(hThread);
				return;
			}
			int w = 0;
			while (!PersistentNamedPipeServer.Instance.IsConnected && w < 1000)
			{
				Thread.Sleep(10);
				w += 10;
			}
			NativeMethods.VirtualFreeEx(hProcess, AllocMem, (UIntPtr)0UL, 32768u);
			NativeMethods.CloseHandle(hThread);
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00011F24 File Offset: 0x00010124
		internal static bool FlashTaskbarIcon(Process process, uint duration, bool stopOnFocus = false)
		{
			return NativeMethods.FlashWindowEx(process, duration, stopOnFocus ? 14u : 2u);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00011F35 File Offset: 0x00010135
		internal static bool FlashTaskbarIcon(Process process)
		{
			return NativeMethods.FlashWindowEx(process, uint.MaxValue, 14u);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00011F40 File Offset: 0x00010140
		internal static bool StopFlashWindowEx(Process process)
		{
			return NativeMethods.FlashWindowEx(process, 0u, 0u);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00011F4C File Offset: 0x0001014C
		private static bool FlashWindowEx(Process process, uint duration, uint flags)
		{
			NativeMethods.FLASHWINFO pwfi = default(NativeMethods.FLASHWINFO);
			pwfi.cbSize = Convert.ToUInt32(Marshal.SizeOf<NativeMethods.FLASHWINFO>(pwfi));
			pwfi.hwnd = process.MainWindowHandle;
			pwfi.dwFlags = flags;
			pwfi.uCount = duration;
			pwfi.dwTimeout = 0u;
			return NativeMethods.FlashWindowEx(ref pwfi);
		}

		// Token: 0x040002DB RID: 731
		private const uint FLASHW_STOP = 0u;

		// Token: 0x040002DC RID: 732
		private const uint FLASHW_CAPTION = 1u;

		// Token: 0x040002DD RID: 733
		private const uint FLASHW_TASKBAR = 2u;

		// Token: 0x040002DE RID: 734
		private const uint FLASHW_ALL = 3u;

		// Token: 0x040002DF RID: 735
		private const uint FLASHW_TIMER = 4u;

		// Token: 0x040002E0 RID: 736
		private const uint FLASHW_TIMERNOFG = 12u;

		// Token: 0x0200008D RID: 141
		internal enum LoadLibraryVariant
		{
			// Token: 0x040002E2 RID: 738
			LoadLibraryA,
			// Token: 0x040002E3 RID: 739
			LoadLibraryW
		}

		// Token: 0x0200008E RID: 142
		[Flags]
		private enum SnapshotFlags : uint
		{
			// Token: 0x040002E5 RID: 741
			TH32CS_SNAPHEAPLIST = 1u,
			// Token: 0x040002E6 RID: 742
			TH32CS_SNAPPROCESS = 2u,
			// Token: 0x040002E7 RID: 743
			TH32CS_SNAPTHREAD = 4u,
			// Token: 0x040002E8 RID: 744
			TH32CS_SNAPMODULE = 8u,
			// Token: 0x040002E9 RID: 745
			TH32CS_SNAPMODULE32 = 16u,
			// Token: 0x040002EA RID: 746
			TH32CS_INHERIT = 2147483648u,
			// Token: 0x040002EB RID: 747
			TH32CS_SNAPALL = 15u,
			// Token: 0x040002EC RID: 748
			NoHeaps = 1073741824u
		}

		// Token: 0x0200008F RID: 143
		[Flags]
		public enum AllocationType
		{
			// Token: 0x040002EE RID: 750
			Commit = 4096,
			// Token: 0x040002EF RID: 751
			Reserve = 8192,
			// Token: 0x040002F0 RID: 752
			Unknown = 12288,
			// Token: 0x040002F1 RID: 753
			Decommit = 16384,
			// Token: 0x040002F2 RID: 754
			Release = 32768,
			// Token: 0x040002F3 RID: 755
			Reset = 524288,
			// Token: 0x040002F4 RID: 756
			Physical = 4194304,
			// Token: 0x040002F5 RID: 757
			TopDown = 1048576,
			// Token: 0x040002F6 RID: 758
			WriteWatch = 2097152,
			// Token: 0x040002F7 RID: 759
			LargePages = 536870912
		}

		// Token: 0x02000090 RID: 144
		[Flags]
		public enum MemoryProtection
		{
			// Token: 0x040002F9 RID: 761
			Execute = 16,
			// Token: 0x040002FA RID: 762
			ExecuteRead = 32,
			// Token: 0x040002FB RID: 763
			ExecuteReadWrite = 64,
			// Token: 0x040002FC RID: 764
			ExecuteWriteCopy = 128,
			// Token: 0x040002FD RID: 765
			NoAccess = 1,
			// Token: 0x040002FE RID: 766
			ReadOnly = 2,
			// Token: 0x040002FF RID: 767
			ReadWrite = 4,
			// Token: 0x04000300 RID: 768
			WriteCopy = 8,
			// Token: 0x04000301 RID: 769
			GuardModifierflag = 256,
			// Token: 0x04000302 RID: 770
			NoCacheModifierflag = 512,
			// Token: 0x04000303 RID: 771
			WriteCombineModifierflag = 1024
		}

		// Token: 0x02000091 RID: 145
		public struct RECT
		{
			// Token: 0x04000304 RID: 772
			public int Left;

			// Token: 0x04000305 RID: 773
			public int Top;

			// Token: 0x04000306 RID: 774
			public int Right;

			// Token: 0x04000307 RID: 775
			public int Bottom;
		}

		// Token: 0x02000092 RID: 146
		private struct FLASHWINFO
		{
			// Token: 0x04000308 RID: 776
			public uint cbSize;

			// Token: 0x04000309 RID: 777
			public IntPtr hwnd;

			// Token: 0x0400030A RID: 778
			public uint dwFlags;

			// Token: 0x0400030B RID: 779
			public uint uCount;

			// Token: 0x0400030C RID: 780
			public uint dwTimeout;
		}

		// Token: 0x02000093 RID: 147
		public struct MODULEENTRY32
		{
			// Token: 0x0400030D RID: 781
			internal uint dwSize;

			// Token: 0x0400030E RID: 782
			internal uint th32ModuleID;

			// Token: 0x0400030F RID: 783
			internal uint th32ProcessID;

			// Token: 0x04000310 RID: 784
			internal uint GlblcntUsage;

			// Token: 0x04000311 RID: 785
			internal uint ProccntUsage;

			// Token: 0x04000312 RID: 786
			internal IntPtr modBaseAddr;

			// Token: 0x04000313 RID: 787
			internal uint modBaseSize;

			// Token: 0x04000314 RID: 788
			internal IntPtr hModule;

			// Token: 0x04000315 RID: 789
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			internal string szModule;

			// Token: 0x04000316 RID: 790
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string szExePath;
		}
	}
}

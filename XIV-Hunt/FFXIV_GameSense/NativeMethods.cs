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
	internal static class NativeMethods
	{
		[DllImport("kernel32.dll")]
		internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, ref IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out uint lpNumberOfBytesWritten);

		[DllImport("kernel32.dll")]
		internal static extern int CloseHandle(IntPtr hProcess);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, NativeMethods.AllocationType flAllocationType, NativeMethods.MemoryProtection flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, IntPtr dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FlashWindowEx(ref NativeMethods.FLASHWINFO pwfi);

		[DllImport("user32.dll")]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		internal static extern bool IsIconic(IntPtr hWnd);

		[DllImport("shell32.dll", SetLastError = true)]
		internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(IntPtr hWnd, out NativeMethods.RECT lpRect);

		[DllImport("kernel32.dll")]
		private static extern uint GetSystemDefaultLCID();

		internal static CultureInfo GetSystemDefaultCultureInfo()
		{
			return new CultureInfo((int)NativeMethods.GetSystemDefaultLCID(), true);
		}

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

		internal static IntPtr GetWindowLongPtr3264(IntPtr hWnd, int nIndex)
		{
			if (Environment.Is64BitOperatingSystem)
			{
				return NativeMethods.GetWindowLongPtr(hWnd, nIndex);
			}
			return NativeMethods.GetWindowLong(hWnd, nIndex);
		}

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		internal static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, long dwNewLong)
		{
			if (Environment.Is64BitOperatingSystem)
			{
				return NativeMethods.SetWindowLongPtr(hWnd, nIndex, new IntPtr(dwNewLong));
			}
			return new IntPtr(NativeMethods.SetWindowLong(hWnd, nIndex, (int)dwNewLong));
		}

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

		internal static bool FlashTaskbarIcon(Process process, uint duration, bool stopOnFocus = false)
		{
			return NativeMethods.FlashWindowEx(process, duration, stopOnFocus ? 14u : 2u);
		}

		internal static bool FlashTaskbarIcon(Process process)
		{
			return NativeMethods.FlashWindowEx(process, uint.MaxValue, 14u);
		}

		internal static bool StopFlashWindowEx(Process process)
		{
			return NativeMethods.FlashWindowEx(process, 0u, 0u);
		}

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

		private const uint FLASHW_STOP = 0u;

		private const uint FLASHW_CAPTION = 1u;

		private const uint FLASHW_TASKBAR = 2u;

		private const uint FLASHW_ALL = 3u;

		private const uint FLASHW_TIMER = 4u;

		private const uint FLASHW_TIMERNOFG = 12u;

		internal enum LoadLibraryVariant
		{
			LoadLibraryA,
			LoadLibraryW
		}

		[Flags]
		private enum SnapshotFlags : uint
		{
			TH32CS_SNAPHEAPLIST = 1u,
			TH32CS_SNAPPROCESS = 2u,
			TH32CS_SNAPTHREAD = 4u,
			TH32CS_SNAPMODULE = 8u,
			TH32CS_SNAPMODULE32 = 16u,
			TH32CS_INHERIT = 2147483648u,
			TH32CS_SNAPALL = 15u,
			NoHeaps = 1073741824u
		}

		[Flags]
		public enum AllocationType
		{
			Commit = 4096,
			Reserve = 8192,
			Unknown = 12288,
			Decommit = 16384,
			Release = 32768,
			Reset = 524288,
			Physical = 4194304,
			TopDown = 1048576,
			WriteWatch = 2097152,
			LargePages = 536870912
		}

		[Flags]
		public enum MemoryProtection
		{
			Execute = 16,
			ExecuteRead = 32,
			ExecuteReadWrite = 64,
			ExecuteWriteCopy = 128,
			NoAccess = 1,
			ReadOnly = 2,
			ReadWrite = 4,
			WriteCopy = 8,
			GuardModifierflag = 256,
			NoCacheModifierflag = 512,
			WriteCombineModifierflag = 1024
		}

		public struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}

		private struct FLASHWINFO
		{
			public uint cbSize;

			public IntPtr hwnd;

			public uint dwFlags;

			public uint uCount;

			public uint dwTimeout;
		}

		public struct MODULEENTRY32
		{
			internal uint dwSize;

			internal uint th32ModuleID;

			internal uint th32ProcessID;

			internal uint GlblcntUsage;

			internal uint ProccntUsage;

			internal IntPtr modBaseAddr;

			internal uint modBaseSize;

			internal IntPtr hModule;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			internal string szModule;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string szExePath;
		}
	}
}

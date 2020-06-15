using System;
using System.Diagnostics;

namespace FFXIV_GameSense
{
	public static class ApplicationRunningHelper
	{
		public static bool AlreadyRunning()
		{
			bool result;
			try
			{
				Process me = Process.GetCurrentProcess();
				Process[] arrProcesses = Process.GetProcessesByName(me.ProcessName);
				for (int i = 0; i < arrProcesses.Length; i++)
				{
					if (arrProcesses[i].MainModule.FileName == me.MainModule.FileName && arrProcesses[i].Id != me.Id)
					{
						IntPtr hWnd = arrProcesses[i].MainWindowHandle;
						if (NativeMethods.IsIconic(hWnd))
						{
							NativeMethods.ShowWindowAsync(hWnd, 9);
						}
						NativeMethods.SetForegroundWindow(hWnd);
						return true;
					}
				}
				result = false;
			}
			catch
			{
				result = true;
			}
			return result;
		}
	}
}

using System;
using System.Diagnostics;

namespace FFXIV_GameSense
{
	// Token: 0x02000094 RID: 148
	public static class ApplicationRunningHelper
	{
		// Token: 0x060003CF RID: 975 RVA: 0x00011FA0 File Offset: 0x000101A0
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

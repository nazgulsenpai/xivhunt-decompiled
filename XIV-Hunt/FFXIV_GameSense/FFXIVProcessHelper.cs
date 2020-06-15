using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Splat;

namespace FFXIV_GameSense
{
	// Token: 0x02000079 RID: 121
	public static class FFXIVProcessHelper
	{
		// Token: 0x0600034B RID: 843 RVA: 0x0000EC00 File Offset: 0x0000CE00
		public static IList<Process> GetFFXIVProcessList()
		{
			return (from x in Process.GetProcessesByName("ffxiv")
			where FFXIVProcessHelper.ValidateProcess("ffxiv.exe", x)
			select x).Union(from x in Process.GetProcessesByName("ffxiv_dx11")
			where FFXIVProcessHelper.ValidateProcess("ffxiv_dx11.exe", x)
			select x).ToList<Process>();
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000EC74 File Offset: 0x0000CE74
		private static bool ValidateProcess(string exeName, Process process)
		{
			try
			{
				return !process.HasExited && process.MainModule != null && process.MainModule.ModuleName.Equals(exeName, StringComparison.OrdinalIgnoreCase);
			}
			catch (Win32Exception ex)
			{
				LogHost.Default.ErrorException("One or more FFXIV processes could not be validated. " + App.GetAppTitle() + " might be lacking privileges.", ex);
			}
			return false;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000ECE0 File Offset: 0x0000CEE0
		public static Process GetFFXIVProcess(int pid = 0)
		{
			Process result;
			try
			{
				IList<Process> list = FFXIVProcessHelper.GetFFXIVProcessList();
				if (pid == 0)
				{
					if (list.Any<Process>())
					{
						result = (from x in list
						orderby x.Id
						select x).FirstOrDefault<Process>();
					}
					else
					{
						result = null;
					}
				}
				else
				{
					result = list.FirstOrDefault((Process x) => x.Id == pid);
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000275 RID: 629
		private const string DX9ExeName = "ffxiv.exe";

		// Token: 0x04000276 RID: 630
		internal const string DX11ExeName = "ffxiv_dx11.exe";
	}
}

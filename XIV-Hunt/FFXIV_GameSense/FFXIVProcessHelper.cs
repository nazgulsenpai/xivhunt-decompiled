using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Splat;

namespace FFXIV_GameSense
{
	public static class FFXIVProcessHelper
	{
		public static IList<Process> GetFFXIVProcessList()
		{
			return (from x in Process.GetProcessesByName("ffxiv")
			where FFXIVProcessHelper.ValidateProcess("ffxiv.exe", x)
			select x).Union(from x in Process.GetProcessesByName("ffxiv_dx11")
			where FFXIVProcessHelper.ValidateProcess("ffxiv_dx11.exe", x)
			select x).ToList<Process>();
		}

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

		private const string DX9ExeName = "ffxiv.exe";

		internal const string DX11ExeName = "ffxiv_dx11.exe";
	}
}

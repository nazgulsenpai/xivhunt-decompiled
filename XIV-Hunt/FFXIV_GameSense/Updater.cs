using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using FFXIV_GameSense.Properties;
using Squirrel;

namespace FFXIV_GameSense
{
	public static class Updater
	{
		public static Task Create(CancellationToken token)
		{
			return new Task(delegate()
			{
				Updater.CheckAndApplyUpdates();
			}, token, TaskCreationOptions.LongRunning);
		}

		private static void CheckAndApplyUpdates()
		{
			bool shouldRestart = false;
			try
			{
				using (UpdateManager mgr = new UpdateManager(Settings.Default.UpdateLocation, null, null, null))
				{
					if (mgr.CheckForUpdate(false, null).Result.ReleasesToApply.Any<ReleaseEntry>())
					{
						Updater.BackupSettings();
						shouldRestart = true;
						Updater.DeleteOldVersions();
						mgr.UpdateApp(null).Wait();
					}
				}
			}
			catch (Exception ex)
			{
				App.WriteExceptionToErrorFile(ex);
			}
			if (shouldRestart)
			{
				Updater.RestartApp();
			}
		}

		internal static void OnAppUpdate()
		{
			using (UpdateManager mgr = new UpdateManager(Settings.Default.UpdateLocation, null, null, null))
			{
				mgr.RemoveUninstallerRegistryEntry();
				mgr.CreateUninstallerRegistryEntry();
			}
		}

		internal static void OnFirstRun()
		{
			Updater.BackupLastStandaloneSettings();
			Updater.RestoreSettings();
			Settings.Default.Reload();
		}

		private static void BackupSettings()
		{
			string filePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
			string destination = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\last.config";
			File.Copy(filePath, destination, true);
		}

		private static void BackupLastStandaloneSettings()
		{
			string gsDir = Directory.GetParent(Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath)).Parent.FullName;
			if (!Directory.Exists(gsDir))
			{
				return;
			}
			string settings = Path.Combine((from x in new DirectoryInfo((from x in new DirectoryInfo(gsDir).EnumerateDirectories()
			orderby x.CreationTime descending
			select x).FirstOrDefault<DirectoryInfo>().FullName).EnumerateDirectories()
			orderby x.CreationTime descending
			select x).FirstOrDefault<DirectoryInfo>().FullName, "user.config");
			if (File.Exists(settings))
			{
				string destination = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\last.config";
				File.Copy(settings, destination, true);
			}
		}

		internal static void RestartApp()
		{
			Settings.Default.Save();
			if (!App.IsSquirrelInstall())
			{
				string executablePath = System.Windows.Forms.Application.ExecutablePath;
				int length = executablePath.Length;
				int num = 0;
				int length2 = length - 3 - num;
				Process.Start(executablePath.Substring(num, length2) + "exe");
				System.Windows.Application.Current.Shutdown();
				return;
			}
			UpdateManager.RestartApp(SettingsForm.StartProcessPath, null);
		}

		private static void DeleteOldVersions()
		{
			DirectoryInfo appDir = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.Parent;
			foreach (DirectoryInfo oldDir in (from x in appDir.EnumerateDirectories("app-*")
			orderby x.CreationTimeUtc descending
			select x).Skip(2))
			{
				try
				{
					oldDir.Delete(true);
				}
				catch
				{
				}
			}
			string packagesDir = Path.Combine(appDir.FullName, "packages");
			if (!Directory.Exists(packagesDir))
			{
				return;
			}
			foreach (FileInfo oldPack in (from x in new DirectoryInfo(packagesDir).EnumerateFiles("*.nupkg")
			orderby x.CreationTimeUtc descending
			select x).Skip(4))
			{
				try
				{
					oldPack.Delete();
				}
				catch
				{
				}
			}
		}

		internal static void RestoreSettings()
		{
			string destFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
			string sourceFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\..\\last.config";
			if (!File.Exists(sourceFile))
			{
				return;
			}
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(destFile));
			}
			catch
			{
			}
			try
			{
				File.Copy(sourceFile, destFile, true);
			}
			catch
			{
			}
			try
			{
				File.Delete(sourceFile);
			}
			catch
			{
			}
		}
	}
}

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FFXIV_GameSense.Properties;
using FFXIV_GameSense.UI;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Splat;
using Squirrel;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace FFXIV_GameSense
{
	// Token: 0x02000024 RID: 36
	public partial class App : Application
	{
		// Token: 0x06000127 RID: 295 RVA: 0x00005220 File Offset: 0x00003420
		protected override void OnStartup(StartupEventArgs e)
		{
			AppDomain.CurrentDomain.UnhandledException += App.CurrentDomain_UnhandledException;
			try
			{
				NativeMethods.SetCurrentProcessExplicitAppUserModelID("com.squirrel.XIVHunt.XIV-Hunt");
			}
			catch
			{
			}
			if (ApplicationRunningHelper.AlreadyRunning())
			{
				Thread.Sleep(2000);
				if (ApplicationRunningHelper.AlreadyRunning())
				{
					return;
				}
			}
			bool isFirstInstall = App.RestoreSettings();
			if (isFirstInstall && App.IsSquirrelInstall())
			{
				try
				{
					Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
					new LanguageSelector().ShowDialog();
				}
				catch (Exception)
				{
				}
			}
			if (App.IsSquirrelInstall())
			{
				SquirrelAwareApp.HandleEvents(null, delegate(Version v)
				{
					Updater.OnAppUpdate();
				}, null, null, new Action(Updater.OnFirstRun), null);
				using (CancellationTokenSource cts = new CancellationTokenSource())
				{
					Task task = Updater.Create(cts.Token);
					task.Start();
					task.Wait();
				}
			}
			base.MainWindow = new Window1();
			Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
			base.MainWindow.Show();
			base.OnStartup(e);
			if (isFirstInstall && App.IsSquirrelInstall())
			{
				Task.Run(async delegate()
				{
					await Task.Delay(1000).ConfigureAwait(false);
					App.TryShowInstalledShortcutInfoToast();
				});
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00005378 File Offset: 0x00003578
		private static bool RestoreSettings()
		{
			bool isFirstInstall = false;
			try
			{
				if (Settings.Default.CallUpgrade)
				{
					Updater.RestoreSettings();
					Settings.Default.Reload();
					isFirstInstall = Settings.Default.CallUpgrade;
					Settings.Default.CallUpgrade = false;
					Settings.Default.Save();
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = new Exception("Failed to restore previous settings. Restoring default settings.", ex);
				App.WriteExceptionToErrorFile(ex2);
				LogHost.Default.WarnException(ex2.Message, ex);
				Settings.Default.Reset();
				Settings.Default.Reload();
				return true;
			}
			return isFirstInstall;
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00005414 File Offset: 0x00003614
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			App.WriteExceptionToErrorFile((Exception)e.ExceptionObject);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00005428 File Offset: 0x00003628
		internal static void WriteExceptionToErrorFile(Exception ex)
		{
			File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "error.txt"), string.Format("{0} {1}:{2}{3}{4}{5}", new object[]
			{
				DateTime.UtcNow,
				ex.GetType().ToString(),
				ex.Message,
				Environment.NewLine,
				ex.StackTrace,
				Environment.NewLine
			}));
			if (ex.InnerException != null)
			{
				File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "error.txt"), string.Concat(new string[]
				{
					"-InnerException ",
					ex.InnerException.GetType().ToString(),
					":",
					ex.InnerException.Message,
					Environment.NewLine,
					ex.InnerException.StackTrace,
					Environment.NewLine
				}));
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000550C File Offset: 0x0000370C
		internal static bool IsSquirrelInstall()
		{
			return File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "..", "Update.exe"));
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00005534 File Offset: 0x00003734
		private static void TryShowInstalledShortcutInfoToast()
		{
			try
			{
				App.ShowInstalledShortcutInfoToast();
			}
			catch
			{
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000555C File Offset: 0x0000375C
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ShowInstalledShortcutInfoToast()
		{
			try
			{
				RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Notifications\\Settings\\com.squirrel.XIVHunt.XIV-Hunt", true);
				registryKey.SetValue("ShowInActionCenter", 1, RegistryValueKind.DWord);
				registryKey.Close();
				ToastContent toastContent = new ToastContent
				{
					Audio = new ToastAudio
					{
						Silent = true
					},
					Visual = new ToastVisual
					{
						BindingGeneric = new ToastBindingGeneric
						{
							Children = 
							{
								new AdaptiveText
								{
									Text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.ToastNotificationAppInstalledShortcut, Program.AssemblyName.Name)
								}
							}
						}
					}
				};
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(toastContent.GetContent());
				ToastNotification toastNotification = new ToastNotification(xmlDocument);
				toastNotification.put_ExpirationTime(new DateTimeOffset?(DateTimeOffset.Now.AddHours(6.0)));
				ToastNotification toast = toastNotification;
				ToastNotificationManager.CreateToastNotifier("com.squirrel.XIVHunt.XIV-Hunt").Show(toast);
			}
			catch (Exception ex)
			{
				LogHost.Default.WarnException("Could not show toast.", ex);
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00005660 File Offset: 0x00003860
		public static string GetAppTitle()
		{
			AssemblyTitleAttribute assemblyTitleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false);
			if (assemblyTitleAttribute == null)
			{
				return null;
			}
			return assemblyTitleAttribute.Title;
		}

		// Token: 0x04000082 RID: 130
		internal const string AppID = "com.squirrel.XIVHunt.XIV-Hunt";
	}
}

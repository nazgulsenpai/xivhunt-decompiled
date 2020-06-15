using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using FFXIV_GameSense.Properties;
using Splat;
using XIVDB;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000B4 RID: 180
	public partial class LogInForm : Window
	{
		// Token: 0x060004A4 RID: 1188 RVA: 0x000157EC File Offset: 0x000139EC
		public LogInForm(ushort wid)
		{
			this.InitializeComponent();
			string text;
			if (GameResources.IsChineseWorld(wid) || GameResources.IsKoreanWorld(wid))
			{
				text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.FormLoginKRCN, "XIVHunt.net");
			}
			else
			{
				text = string.Format(CultureInfo.CurrentCulture, FFXIV_GameSense.Properties.Resources.FormLogin, "XIVHunt.net", GameResources.GetWorldName(wid));
			}
			Hyperlink link = new Hyperlink(new Run("XIVHunt.net"))
			{
				NavigateUri = new Uri("https://xivhunt.net/Account/Login")
			};
			link.RequestNavigate += LogInForm.Link_RequestNavigate;
			foreach (string s in text.SplitAndKeep("XIVHunt.net", StringComparison.Ordinal))
			{
				if (s.Equals("XIVHunt.net", StringComparison.Ordinal))
				{
					this.InfoTextBlock.Inlines.Add(link);
				}
				else
				{
					this.InfoTextBlock.Inlines.Add(s);
				}
			}
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x000158F8 File Offset: 0x00013AF8
		internal static void Link_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			try
			{
				Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			}
			catch
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					Process.Start(new ProcessStartInfo("cmd", "/c start " + e.Uri.AbsoluteUri.Replace("&", "^&"))
					{
						CreateNoWindow = true
					});
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Process.Start("xdg-open", e.Uri.AbsoluteUri);
				}
				else
				{
					if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					{
						throw;
					}
					Process.Start("open", e.Uri.AbsoluteUri);
				}
			}
			e.Handled = true;
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x000159D0 File Offset: 0x00013BD0
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.LoginFailedTextBlock.Visibility = Visibility.Hidden;
			Cookie receivedCookie = null;
			Cookie twofaCookie = null;
			bool authresult = false;
			try
			{
				authresult = (this.PasswordBox.Password.Length > 7 && LogInForm.AuthenticateUser(this.EmailTextBox.Text, this.PasswordBox.Password, this.TwoFABox.Password, out receivedCookie, out twofaCookie));
			}
			catch (Exception ex)
			{
				LogHost.Default.InfoException("An exception occured while trying to log in", ex);
			}
			if (authresult)
			{
				base.DialogResult = new bool?(true);
				if (receivedCookie != null)
				{
					this.receivedCookies.Add(receivedCookie);
				}
				if (twofaCookie != null)
				{
					this.receivedCookies.Add(twofaCookie);
				}
				Settings.Default.Cookies = Convert.ToBase64String(LogInForm.ObjectToByteArray(this.receivedCookies));
				Settings.Default.Save();
				base.Close();
				return;
			}
			this.LoginFailedTextBlock.Visibility = Visibility.Visible;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00015ABC File Offset: 0x00013CBC
		private static bool AuthenticateUser(string user, string password, string twofa, out Cookie authCookie, out Cookie twofaCookie)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://xivhunt.net/Account/RemoteLogin"));
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.CookieContainer = new CookieContainer();
			request.AllowAutoRedirect = true;
			string authCredentials = string.Concat(new string[]
			{
				"Email=",
				WebUtility.UrlEncode(user),
				"&Password=",
				WebUtility.UrlEncode(password),
				"&RememberMe=true"
			});
			if (!string.IsNullOrWhiteSpace(twofa))
			{
				authCredentials = authCredentials + "&TwoFactorCode=" + twofa;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(authCredentials);
			request.ContentLength = (long)bytes.Length;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				authCookie = response.Cookies[".AspNetCore.Identity.Application"];
				if (authCookie == null && !string.IsNullOrWhiteSpace(twofa) && response.Cookies["Identity.TwoFactorUserId"] != null)
				{
					LogInForm.TwoFA(twofa, response.Cookies["Identity.TwoFactorUserId"], out authCookie, out twofaCookie);
				}
				else
				{
					twofaCookie = null;
				}
			}
			return authCookie != null;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x00015C14 File Offset: 0x00013E14
		private static bool TwoFA(string twofa, Cookie twofauseridcookie, out Cookie authCookie, out Cookie twofaCookie)
		{
			HttpWebRequest request = WebRequest.Create(new Uri("https://xivhunt.net/Account/LoginWith2fa")) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.CookieContainer = new CookieContainer();
			request.CookieContainer.Add(twofauseridcookie);
			request.AllowAutoRedirect = true;
			string authCredentials = "TwoFactorCode=" + twofa + "&RememberMachine=true&RememberMe=true";
			byte[] bytes = Encoding.UTF8.GetBytes(authCredentials);
			request.ContentLength = (long)bytes.Length;
			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(bytes, 0, bytes.Length);
			}
			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				Cookie cookie;
				twofaCookie = (cookie = null);
				authCookie = cookie;
				authCookie = response.Cookies[".AspNetCore.Identity.Application"];
				twofaCookie = response.Cookies["Identity.TwoFactorRememberMe"];
			}
			return authCookie != null && twofaCookie != null;
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00015D24 File Offset: 0x00013F24
		private static byte[] ObjectToByteArray(object obj)
		{
			BinaryFormatter bf = new BinaryFormatter();
			byte[] result;
			using (MemoryStream ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				result = ms.ToArray();
			}
			return result;
		}

		// Token: 0x04000389 RID: 905
		private const string Login2FaUrl = "https://xivhunt.net/Account/LoginWith2fa";

		// Token: 0x0400038A RID: 906
		private const string RemoteLoginUrl = "https://xivhunt.net/Account/RemoteLogin";

		// Token: 0x0400038B RID: 907
		private const string AccountLoginUrl = "https://xivhunt.net/Account/Login";

		// Token: 0x0400038C RID: 908
		private const string IdentityCookieName = ".AspNetCore.Identity.Application";

		// Token: 0x0400038D RID: 909
		private const string TwoFactorUserIdCookieName = "Identity.TwoFactorUserId";

		// Token: 0x0400038E RID: 910
		private const string FormEncodingType = "application/x-www-form-urlencoded";

		// Token: 0x0400038F RID: 911
		internal const string TwoFactorRememberMeCookieName = "Identity.TwoFactorRememberMe";

		// Token: 0x04000390 RID: 912
		internal const string XIVHuntNet = "XIVHunt.net";

		// Token: 0x04000391 RID: 913
		internal CookieContainer receivedCookies = new CookieContainer(2);
	}
}

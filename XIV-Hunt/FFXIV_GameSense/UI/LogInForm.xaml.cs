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
	public partial class LogInForm : Window
	{
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

		private const string Login2FaUrl = "https://xivhunt.net/Account/LoginWith2fa";

		private const string RemoteLoginUrl = "https://xivhunt.net/Account/RemoteLogin";

		private const string AccountLoginUrl = "https://xivhunt.net/Account/Login";

		private const string IdentityCookieName = ".AspNetCore.Identity.Application";

		private const string TwoFactorUserIdCookieName = "Identity.TwoFactorUserId";

		private const string FormEncodingType = "application/x-www-form-urlencoded";

		internal const string TwoFactorRememberMeCookieName = "Identity.TwoFactorRememberMe";

		internal const string XIVHuntNet = "XIVHunt.net";

		internal CookieContainer receivedCookies = new CookieContainer(2);
	}
}

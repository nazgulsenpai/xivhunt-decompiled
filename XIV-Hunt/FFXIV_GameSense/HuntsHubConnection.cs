using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FFXIV_GameSense.Properties;
using FFXIV_GameSense.UI;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Splat;

namespace FFXIV_GameSense
{
	internal class HuntsHubConnection
	{
		public HubConnection Connection { get; private set; }

		public bool Connected { get; private set; }

		public HuntsHubConnection()
		{
			this.Connection = new HubConnectionBuilder().WithUrl("https://xivhunt.net/SignalR/HuntsHub", delegate(HttpConnectionOptions o)
			{
				o.Cookies = HuntsHubConnection.Login(Program.mem.GetHomeWorldId());
			}).AddNewtonsoftJsonProtocol<IHubConnectionBuilder>().Build();
			this.Connection.Closed += this.Connection_Closed;
		}

		private Task Connection_Closed(Exception arg)
		{
			LogHost.Default.InfoException("HuntsHubConnection closed due to: ", arg);
			this.Connected = false;
			return Task.CompletedTask;
		}

		internal async Task<bool> Connect(Window1 w1)
		{
			HuntsHubConnection.<>c__DisplayClass12_0 CS$<>8__locals1 = new HuntsHubConnection.<>c__DisplayClass12_0();
			CS$<>8__locals1.w1 = w1;
			if (this.Connected)
			{
				await this.Connection.StopAsync(default(CancellationToken)).ConfigureAwait(false);
			}
			this.Connected = false;
			bool result;
			if (this.IsConnecting)
			{
				result = false;
			}
			else
			{
				this.IsConnecting = true;
				while (!this.Connected)
				{
					int num = 0;
					try
					{
						Dispatcher dispatcher = CS$<>8__locals1.w1.HuntConnectionTextBlock.Dispatcher;
						Func<string> callback;
						if ((callback = CS$<>8__locals1.<>9__0) == null)
						{
							HuntsHubConnection.<>c__DisplayClass12_0 CS$<>8__locals2 = CS$<>8__locals1;
							Func<string> func = () => CS$<>8__locals1.w1.HuntConnectionTextBlock.Text = Resources.FormConnecting;
							CS$<>8__locals2.<>9__0 = func;
							callback = func;
						}
						dispatcher.Invoke<string>(callback);
						await this.Connection.StartAsync(default(CancellationToken)).ConfigureAwait(false);
						this.Connected = true;
					}
					catch (Exception obj)
					{
						num = 1;
					}
					object obj;
					if (num == 1)
					{
						Exception e = (Exception)obj;
						if (e is HttpRequestException && e.Message.Contains("401"))
						{
							LogHost.Default.WarnException("Failed to connect.", e);
							await this.Connection.DisposeAsync();
							this.Connection.Closed -= this.Connection_Closed;
							Settings.Default.Cookies = string.Empty;
							Settings.Default.Save();
							this.Connection = new HubConnectionBuilder().WithUrl("https://xivhunt.net/SignalR/HuntsHub", delegate(HttpConnectionOptions o)
							{
								o.Cookies = HuntsHubConnection.Login(Program.mem.GetHomeWorldId());
							}).Build();
							this.Connection.Closed += this.Connection_Closed;
							this.IsConnecting = false;
							return true;
						}
						HuntsHubConnection.<>c__DisplayClass12_1 CS$<>8__locals3 = new HuntsHubConnection.<>c__DisplayClass12_1();
						CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals1;
						CS$<>8__locals3.wtime = 5000;
						LogHost.Default.InfoException(string.Format(CultureInfo.CurrentCulture, Resources.FormConnectingRetrying, 5), e);
						while (CS$<>8__locals3.wtime > 0)
						{
							CS$<>8__locals3.CS$<>8__locals1.w1.HuntConnectionTextBlock.Dispatcher.Invoke<string>(() => CS$<>8__locals3.CS$<>8__locals1.w1.HuntConnectionTextBlock.Text = string.Format(CultureInfo.CurrentCulture, Resources.FormConnectingRetrying, CS$<>8__locals3.wtime / 1000));
							await Task.Delay(1000);
							CS$<>8__locals3.wtime -= 1000;
						}
						CS$<>8__locals3 = null;
					}
					obj = null;
				}
				this.IsConnecting = false;
				result = false;
			}
			return result;
		}

		internal static CookieContainer Login(ushort sid)
		{
			CookieContainer cc = null;
			if (!string.IsNullOrWhiteSpace(Settings.Default.Cookies))
			{
				cc = (CookieContainer)HuntsHubConnection.ByteArrayToObject(Convert.FromBase64String(Settings.Default.Cookies));
			}
			while (!HuntsHubConnection.TestCC(cc))
			{
				Application.Current.Dispatcher.Invoke(delegate()
				{
					LogInForm lif = new LogInForm(sid);
					if (lif.ShowDialog().Value && lif.receivedCookies.Count > 0)
					{
						cc = lif.receivedCookies;
					}
					if (lif.receivedCookies.Count == 0)
					{
						Environment.Exit(0);
					}
				});
			}
			return cc;
		}

		private static bool TestCC(CookieContainer cc)
		{
			if (cc == null)
			{
				return false;
			}
			CookieCollection ccs = cc.GetCookies(new Uri("https://xivhunt.net/"));
			for (int i = 0; i < ccs.Count; i++)
			{
				if (ccs[i].Name == "Identity.TwoFactorRememberMe")
				{
					return true;
				}
			}
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri("https://xivhunt.net/Manage/VerifiedCharacters"));
			request.CookieContainer = cc;
			request.AllowAutoRedirect = true;
			bool result;
			try
			{
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					result = (response.StatusCode == HttpStatusCode.OK);
				}
			}
			catch (WebException ex)
			{
				using (HttpWebResponse response2 = (HttpWebResponse)ex.Response)
				{
					result = (response2.StatusCode == HttpStatusCode.OK);
				}
			}
			return result;
		}

		private static object ByteArrayToObject(byte[] arrBytes)
		{
			object result;
			using (MemoryStream memStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				memStream.Write(arrBytes, 0, arrBytes.Length);
				memStream.Seek(0L, SeekOrigin.Begin);
				result = binaryFormatter.Deserialize(memStream);
			}
			return result;
		}

		private const string HuntsHubEndpoint = "SignalR/HuntsHub";

		private bool IsConnecting;
	}
}

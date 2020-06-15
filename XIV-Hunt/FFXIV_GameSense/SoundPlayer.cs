using System;
using System.Threading.Tasks;
using FFXIV_GameSense.Properties;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace FFXIV_GameSense
{
	// Token: 0x020000A2 RID: 162
	internal static class SoundPlayer
	{
		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x00013DF3 File Offset: 0x00011FF3
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x00013DFA File Offset: 0x00011FFA
		internal static AudioFileReader AudioFileReader { get; private set; }

		// Token: 0x0600042F RID: 1071 RVA: 0x00013E04 File Offset: 0x00012004
		public static async Task Play(AudioFileReader reader)
		{
			using (WasapiOut WasapiOutDevice = new WasapiOut(SoundPlayer.FindSelectedDevice(), AudioClientShareMode.Shared, true, 50))
			{
				SoundPlayer.AudioFileReader = reader;
				reader.Position = 0L;
				reader.Volume = Settings.Default.Volume;
				WasapiOutDevice.Init(reader);
				WasapiOutDevice.Play();
				while (WasapiOutDevice.PlaybackState == PlaybackState.Playing)
				{
					await Task.Delay(100).ConfigureAwait(false);
				}
			}
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00013E4C File Offset: 0x0001204C
		private static MMDevice FindSelectedDevice()
		{
			MMDevice defaultAudioEndpoint;
			using (MMDeviceEnumerator audioEndPointEnumerator = new MMDeviceEnumerator())
			{
				if (!string.IsNullOrWhiteSpace(Settings.Default.AudioDevice))
				{
					foreach (MMDevice audioEndPoint in audioEndPointEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
					{
						if (audioEndPoint.FriendlyName.Equals(Settings.Default.AudioDevice))
						{
							return audioEndPoint;
						}
					}
				}
				defaultAudioEndpoint = audioEndPointEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
			}
			return defaultAudioEndpoint;
		}
	}
}

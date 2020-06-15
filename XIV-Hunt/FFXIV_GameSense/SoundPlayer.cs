using System;
using System.Threading.Tasks;
using FFXIV_GameSense.Properties;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace FFXIV_GameSense
{
	internal static class SoundPlayer
	{
		internal static AudioFileReader AudioFileReader { get; private set; }

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

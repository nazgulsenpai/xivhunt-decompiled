using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace FFXIV_GameSense.Properties
{
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.1.0.0")]
	public sealed partial class Settings : ApplicationSettingsBase
	{
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfUnsignedShort xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
		public ObservableHashSet<ushort> FATEs
		{
			get
			{
				return (ObservableHashSet<ushort>)this["FATEs"];
			}
			set
			{
				this["FATEs"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfUnsignedShort xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />")]
		public ObservableHashSet<ushort> Hunts
		{
			get
			{
				return (ObservableHashSet<ushort>)this["Hunts"];
			}
			set
			{
				this["Hunts"] = value;
			}
		}

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("No sound alert")]
		public string SBell
		{
			get
			{
				return (string)this["SBell"];
			}
			set
			{
				this["SBell"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("No sound alert")]
		public string ABell
		{
			get
			{
				return (string)this["ABell"];
			}
			set
			{
				this["ABell"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("No sound alert")]
		public string FATEBell
		{
			get
			{
				return (string)this["FATEBell"];
			}
			set
			{
				this["FATEBell"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("No sound alert")]
		public string BBell
		{
			get
			{
				return (string)this["BBell"];
			}
			set
			{
				this["BBell"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool notifyS
		{
			get
			{
				return (bool)this["notifyS"];
			}
			set
			{
				this["notifyS"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool notifyA
		{
			get
			{
				return (bool)this["notifyA"];
			}
			set
			{
				this["notifyA"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool notifyB
		{
			get
			{
				return (bool)this["notifyB"];
			}
			set
			{
				this["notifyB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool MinimizeToTray
		{
			get
			{
				return (bool)this["MinimizeToTray"];
			}
			set
			{
				this["MinimizeToTray"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool SSB
		{
			get
			{
				return (bool)this["SSB"];
			}
			set
			{
				this["SSB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool SHW
		{
			get
			{
				return (bool)this["SHW"];
			}
			set
			{
				this["SHW"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool SARR
		{
			get
			{
				return (bool)this["SARR"];
			}
			set
			{
				this["SARR"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool ASB
		{
			get
			{
				return (bool)this["ASB"];
			}
			set
			{
				this["ASB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool AHW
		{
			get
			{
				return (bool)this["AHW"];
			}
			set
			{
				this["AHW"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool AARR
		{
			get
			{
				return (bool)this["AARR"];
			}
			set
			{
				this["AARR"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool BSB
		{
			get
			{
				return (bool)this["BSB"];
			}
			set
			{
				this["BSB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool BHW
		{
			get
			{
				return (bool)this["BHW"];
			}
			set
			{
				this["BHW"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool BARR
		{
			get
			{
				return (bool)this["BARR"];
			}
			set
			{
				this["BARR"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("en-US")]
		public string LanguageCI
		{
			get
			{
				return (string)this["LanguageCI"];
			}
			set
			{
				this["LanguageCI"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool CallUpgrade
		{
			get
			{
				return (bool)this["CallUpgrade"];
			}
			set
			{
				this["CallUpgrade"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("5")]
		public float HuntInterval
		{
			get
			{
				return (float)this["HuntInterval"];
			}
			set
			{
				this["HuntInterval"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("2")]
		public float FATEInterval
		{
			get
			{
				return (float)this["FATEInterval"];
			}
			set
			{
				this["FATEInterval"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool StartMinimized
		{
			get
			{
				return (bool)this["StartMinimized"];
			}
			set
			{
				this["StartMinimized"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string LastEmail
		{
			get
			{
				return (string)this["LastEmail"];
			}
			set
			{
				this["LastEmail"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string Cookies
		{
			get
			{
				return (string)this["Cookies"];
			}
			set
			{
				this["Cookies"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool TrackFATEAfterQuery
		{
			get
			{
				return (bool)this["TrackFATEAfterQuery"];
			}
			set
			{
				this["TrackFATEAfterQuery"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool notifyDutyRoulette
		{
			get
			{
				return (bool)this["notifyDutyRoulette"];
			}
			set
			{
				this["notifyDutyRoulette"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool OncePerHunt
		{
			get
			{
				return (bool)this["OncePerHunt"];
			}
			set
			{
				this["OncePerHunt"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool ForgetOnZoneChange
		{
			get
			{
				return (bool)this["ForgetOnZoneChange"];
			}
			set
			{
				this["ForgetOnZoneChange"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool NoAnnouncementsInContent
		{
			get
			{
				return (bool)this["NoAnnouncementsInContent"];
			}
			set
			{
				this["NoAnnouncementsInContent"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public float FATEMinimumMinutesRemaining
		{
			get
			{
				return (float)this["FATEMinimumMinutesRemaining"];
			}
			set
			{
				this["FATEMinimumMinutesRemaining"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public byte FATEMinimumPercentInterval
		{
			get
			{
				return (byte)this["FATEMinimumPercentInterval"];
			}
			set
			{
				this["FATEMinimumPercentInterval"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool FlashTaskbarIconOnDFPop
		{
			get
			{
				return (bool)this["FlashTaskbarIconOnDFPop"];
			}
			set
			{
				this["FlashTaskbarIconOnDFPop"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool FlashTaskbarIconOnHuntAndFATEs
		{
			get
			{
				return (bool)this["FlashTaskbarIconOnHuntAndFATEs"];
			}
			set
			{
				this["FlashTaskbarIconOnHuntAndFATEs"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string PerformDirectory
		{
			get
			{
				return (string)this["PerformDirectory"];
			}
			set
			{
				this["PerformDirectory"] = value;
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("https://xivhunt.net/Releases")]
		public string UpdateLocation
		{
			get
			{
				return (string)this["UpdateLocation"];
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("16384")]
		public int MMLMaxSizeBytes
		{
			get
			{
				return (int)this["MMLMaxSizeBytes"];
			}
		}

		[ApplicationScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("00:15:00")]
		public TimeSpan MMLMaxDuration
		{
			get
			{
				return (TimeSpan)this["MMLMaxDuration"];
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("1")]
		public byte LogLevel
		{
			get
			{
				return (byte)this["LogLevel"];
			}
			set
			{
				this["LogLevel"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displayCairns
		{
			get
			{
				return (bool)this["displayCairns"];
			}
			set
			{
				this["displayCairns"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displayGoldTreasureCoffers
		{
			get
			{
				return (bool)this["displayGoldTreasureCoffers"];
			}
			set
			{
				this["displayGoldTreasureCoffers"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displayMonsters
		{
			get
			{
				return (bool)this["displayMonsters"];
			}
			set
			{
				this["displayMonsters"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displayOtherPCs
		{
			get
			{
				return (bool)this["displayOtherPCs"];
			}
			set
			{
				this["displayOtherPCs"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displaySelf
		{
			get
			{
				return (bool)this["displaySelf"];
			}
			set
			{
				this["displaySelf"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displaySilverTreasureCoffers
		{
			get
			{
				return (bool)this["displaySilverTreasureCoffers"];
			}
			set
			{
				this["displaySilverTreasureCoffers"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool displayTreasureCoffers
		{
			get
			{
				return (bool)this["displayTreasureCoffers"];
			}
			set
			{
				this["displayTreasureCoffers"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool RadarEnableClickthru
		{
			get
			{
				return (bool)this["RadarEnableClickthru"];
			}
			set
			{
				this["RadarEnableClickthru"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0, 0")]
		public Point RadarWindowOffset
		{
			get
			{
				return (Point)this["RadarWindowOffset"];
			}
			set
			{
				this["RadarWindowOffset"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0, 0")]
		public Size RadarWindowSize
		{
			get
			{
				return (Size)this["RadarWindowSize"];
			}
			set
			{
				this["RadarWindowSize"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool RadarDisableResize
		{
			get
			{
				return (bool)this["RadarDisableResize"];
			}
			set
			{
				this["RadarDisableResize"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("30")]
		public byte RadarMaxFrameRate
		{
			get
			{
				return (byte)this["RadarMaxFrameRate"];
			}
			set
			{
				this["RadarMaxFrameRate"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("0")]
		public byte RadarBGOpacity
		{
			get
			{
				return (byte)this["RadarBGOpacity"];
			}
			set
			{
				this["RadarBGOpacity"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("1")]
		public float RadarEntityScale
		{
			get
			{
				return (float)this["RadarEntityScale"];
			}
			set
			{
				this["RadarEntityScale"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("100")]
		public byte RadarEntityOpacity
		{
			get
			{
				return (byte)this["RadarEntityOpacity"];
			}
			set
			{
				this["RadarEntityOpacity"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("1")]
		public float RadarZoom
		{
			get
			{
				return (float)this["RadarZoom"];
			}
			set
			{
				this["RadarZoom"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("1")]
		public float Volume
		{
			get
			{
				return (float)this["Volume"];
			}
			set
			{
				this["Volume"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		public string AudioDevice
		{
			get
			{
				return (string)this["AudioDevice"];
			}
			set
			{
				this["AudioDevice"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool NotificationsFromOtherWorlds
		{
			get
			{
				return (bool)this["NotificationsFromOtherWorlds"];
			}
			set
			{
				this["NotificationsFromOtherWorlds"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool SSHB
		{
			get
			{
				return (bool)this["SSHB"];
			}
			set
			{
				this["SSHB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool ASHB
		{
			get
			{
				return (bool)this["ASHB"];
			}
			set
			{
				this["ASHB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("True")]
		public bool BSHB
		{
			get
			{
				return (bool)this["BSHB"];
			}
			set
			{
				this["BSHB"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("False")]
		public bool FPSUnlock
		{
			get
			{
				return (bool)this["FPSUnlock"];
			}
			set
			{
				this["FPSUnlock"] = value;
			}
		}

		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("Echo")]
		public ChatChannel ChatChannel
		{
			get
			{
				return (ChatChannel)this["ChatChannel"];
			}
			set
			{
				this["ChatChannel"] = value;
			}
		}

		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}

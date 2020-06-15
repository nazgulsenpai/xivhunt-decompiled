using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace FFXIV_GameSense.Properties
{
	// Token: 0x020000C8 RID: 200
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.1.0.0")]
	public sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x000189FD File Offset: 0x00016BFD
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x00018A0F File Offset: 0x00016C0F
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

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00018A1D File Offset: 0x00016C1D
		// (set) Token: 0x060005BD RID: 1469 RVA: 0x00018A2F File Offset: 0x00016C2F
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

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060005BE RID: 1470 RVA: 0x00018A3D File Offset: 0x00016C3D
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060005BF RID: 1471 RVA: 0x00018A44 File Offset: 0x00016C44
		// (set) Token: 0x060005C0 RID: 1472 RVA: 0x00018A56 File Offset: 0x00016C56
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

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060005C1 RID: 1473 RVA: 0x00018A64 File Offset: 0x00016C64
		// (set) Token: 0x060005C2 RID: 1474 RVA: 0x00018A76 File Offset: 0x00016C76
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

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060005C3 RID: 1475 RVA: 0x00018A84 File Offset: 0x00016C84
		// (set) Token: 0x060005C4 RID: 1476 RVA: 0x00018A96 File Offset: 0x00016C96
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

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060005C5 RID: 1477 RVA: 0x00018AA4 File Offset: 0x00016CA4
		// (set) Token: 0x060005C6 RID: 1478 RVA: 0x00018AB6 File Offset: 0x00016CB6
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

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x00018AC4 File Offset: 0x00016CC4
		// (set) Token: 0x060005C8 RID: 1480 RVA: 0x00018AD6 File Offset: 0x00016CD6
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

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060005C9 RID: 1481 RVA: 0x00018AE9 File Offset: 0x00016CE9
		// (set) Token: 0x060005CA RID: 1482 RVA: 0x00018AFB File Offset: 0x00016CFB
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

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060005CB RID: 1483 RVA: 0x00018B0E File Offset: 0x00016D0E
		// (set) Token: 0x060005CC RID: 1484 RVA: 0x00018B20 File Offset: 0x00016D20
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

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060005CD RID: 1485 RVA: 0x00018B33 File Offset: 0x00016D33
		// (set) Token: 0x060005CE RID: 1486 RVA: 0x00018B45 File Offset: 0x00016D45
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

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060005CF RID: 1487 RVA: 0x00018B58 File Offset: 0x00016D58
		// (set) Token: 0x060005D0 RID: 1488 RVA: 0x00018B6A File Offset: 0x00016D6A
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

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x00018B7D File Offset: 0x00016D7D
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x00018B8F File Offset: 0x00016D8F
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

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x00018BA2 File Offset: 0x00016DA2
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x00018BB4 File Offset: 0x00016DB4
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

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060005D5 RID: 1493 RVA: 0x00018BC7 File Offset: 0x00016DC7
		// (set) Token: 0x060005D6 RID: 1494 RVA: 0x00018BD9 File Offset: 0x00016DD9
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

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060005D7 RID: 1495 RVA: 0x00018BEC File Offset: 0x00016DEC
		// (set) Token: 0x060005D8 RID: 1496 RVA: 0x00018BFE File Offset: 0x00016DFE
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

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x00018C11 File Offset: 0x00016E11
		// (set) Token: 0x060005DA RID: 1498 RVA: 0x00018C23 File Offset: 0x00016E23
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

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x00018C36 File Offset: 0x00016E36
		// (set) Token: 0x060005DC RID: 1500 RVA: 0x00018C48 File Offset: 0x00016E48
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

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060005DD RID: 1501 RVA: 0x00018C5B File Offset: 0x00016E5B
		// (set) Token: 0x060005DE RID: 1502 RVA: 0x00018C6D File Offset: 0x00016E6D
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

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060005DF RID: 1503 RVA: 0x00018C80 File Offset: 0x00016E80
		// (set) Token: 0x060005E0 RID: 1504 RVA: 0x00018C92 File Offset: 0x00016E92
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

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x00018CA5 File Offset: 0x00016EA5
		// (set) Token: 0x060005E2 RID: 1506 RVA: 0x00018CB7 File Offset: 0x00016EB7
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

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x00018CC5 File Offset: 0x00016EC5
		// (set) Token: 0x060005E4 RID: 1508 RVA: 0x00018CD7 File Offset: 0x00016ED7
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

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x00018CEA File Offset: 0x00016EEA
		// (set) Token: 0x060005E6 RID: 1510 RVA: 0x00018CFC File Offset: 0x00016EFC
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

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x00018D0F File Offset: 0x00016F0F
		// (set) Token: 0x060005E8 RID: 1512 RVA: 0x00018D21 File Offset: 0x00016F21
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

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x00018D34 File Offset: 0x00016F34
		// (set) Token: 0x060005EA RID: 1514 RVA: 0x00018D46 File Offset: 0x00016F46
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

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00018D59 File Offset: 0x00016F59
		// (set) Token: 0x060005EC RID: 1516 RVA: 0x00018D6B File Offset: 0x00016F6B
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

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x00018D79 File Offset: 0x00016F79
		// (set) Token: 0x060005EE RID: 1518 RVA: 0x00018D8B File Offset: 0x00016F8B
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

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x00018D99 File Offset: 0x00016F99
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x00018DAB File Offset: 0x00016FAB
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

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x00018DBE File Offset: 0x00016FBE
		// (set) Token: 0x060005F2 RID: 1522 RVA: 0x00018DD0 File Offset: 0x00016FD0
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

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x00018DE3 File Offset: 0x00016FE3
		// (set) Token: 0x060005F4 RID: 1524 RVA: 0x00018DF5 File Offset: 0x00016FF5
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

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060005F5 RID: 1525 RVA: 0x00018E08 File Offset: 0x00017008
		// (set) Token: 0x060005F6 RID: 1526 RVA: 0x00018E1A File Offset: 0x0001701A
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

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x00018E2D File Offset: 0x0001702D
		// (set) Token: 0x060005F8 RID: 1528 RVA: 0x00018E3F File Offset: 0x0001703F
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

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060005F9 RID: 1529 RVA: 0x00018E52 File Offset: 0x00017052
		// (set) Token: 0x060005FA RID: 1530 RVA: 0x00018E64 File Offset: 0x00017064
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

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060005FB RID: 1531 RVA: 0x00018E77 File Offset: 0x00017077
		// (set) Token: 0x060005FC RID: 1532 RVA: 0x00018E89 File Offset: 0x00017089
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

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x00018E9C File Offset: 0x0001709C
		// (set) Token: 0x060005FE RID: 1534 RVA: 0x00018EAE File Offset: 0x000170AE
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

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x00018EC1 File Offset: 0x000170C1
		// (set) Token: 0x06000600 RID: 1536 RVA: 0x00018ED3 File Offset: 0x000170D3
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

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000601 RID: 1537 RVA: 0x00018EE6 File Offset: 0x000170E6
		// (set) Token: 0x06000602 RID: 1538 RVA: 0x00018EF8 File Offset: 0x000170F8
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

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x06000603 RID: 1539 RVA: 0x00018F06 File Offset: 0x00017106
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

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000604 RID: 1540 RVA: 0x00018F18 File Offset: 0x00017118
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

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x00018F2A File Offset: 0x0001712A
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

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x00018F3C File Offset: 0x0001713C
		// (set) Token: 0x06000607 RID: 1543 RVA: 0x00018F4E File Offset: 0x0001714E
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

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000608 RID: 1544 RVA: 0x00018F61 File Offset: 0x00017161
		// (set) Token: 0x06000609 RID: 1545 RVA: 0x00018F73 File Offset: 0x00017173
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

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00018F86 File Offset: 0x00017186
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x00018F98 File Offset: 0x00017198
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

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600060C RID: 1548 RVA: 0x00018FAB File Offset: 0x000171AB
		// (set) Token: 0x0600060D RID: 1549 RVA: 0x00018FBD File Offset: 0x000171BD
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

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x00018FD0 File Offset: 0x000171D0
		// (set) Token: 0x0600060F RID: 1551 RVA: 0x00018FE2 File Offset: 0x000171E2
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

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000610 RID: 1552 RVA: 0x00018FF5 File Offset: 0x000171F5
		// (set) Token: 0x06000611 RID: 1553 RVA: 0x00019007 File Offset: 0x00017207
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

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06000612 RID: 1554 RVA: 0x0001901A File Offset: 0x0001721A
		// (set) Token: 0x06000613 RID: 1555 RVA: 0x0001902C File Offset: 0x0001722C
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

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0001903F File Offset: 0x0001723F
		// (set) Token: 0x06000615 RID: 1557 RVA: 0x00019051 File Offset: 0x00017251
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

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x00019064 File Offset: 0x00017264
		// (set) Token: 0x06000617 RID: 1559 RVA: 0x00019076 File Offset: 0x00017276
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

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x00019089 File Offset: 0x00017289
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x0001909B File Offset: 0x0001729B
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

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x000190AE File Offset: 0x000172AE
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x000190C0 File Offset: 0x000172C0
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

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x000190D3 File Offset: 0x000172D3
		// (set) Token: 0x0600061D RID: 1565 RVA: 0x000190E5 File Offset: 0x000172E5
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

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x000190F8 File Offset: 0x000172F8
		// (set) Token: 0x0600061F RID: 1567 RVA: 0x0001910A File Offset: 0x0001730A
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

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0001911D File Offset: 0x0001731D
		// (set) Token: 0x06000621 RID: 1569 RVA: 0x0001912F File Offset: 0x0001732F
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

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00019142 File Offset: 0x00017342
		// (set) Token: 0x06000623 RID: 1571 RVA: 0x00019154 File Offset: 0x00017354
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

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000624 RID: 1572 RVA: 0x00019167 File Offset: 0x00017367
		// (set) Token: 0x06000625 RID: 1573 RVA: 0x00019179 File Offset: 0x00017379
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

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000626 RID: 1574 RVA: 0x0001918C File Offset: 0x0001738C
		// (set) Token: 0x06000627 RID: 1575 RVA: 0x0001919E File Offset: 0x0001739E
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

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000628 RID: 1576 RVA: 0x000191B1 File Offset: 0x000173B1
		// (set) Token: 0x06000629 RID: 1577 RVA: 0x000191C3 File Offset: 0x000173C3
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

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600062A RID: 1578 RVA: 0x000191D6 File Offset: 0x000173D6
		// (set) Token: 0x0600062B RID: 1579 RVA: 0x000191E8 File Offset: 0x000173E8
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

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600062C RID: 1580 RVA: 0x000191F6 File Offset: 0x000173F6
		// (set) Token: 0x0600062D RID: 1581 RVA: 0x00019208 File Offset: 0x00017408
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

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001921B File Offset: 0x0001741B
		// (set) Token: 0x0600062F RID: 1583 RVA: 0x0001922D File Offset: 0x0001742D
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

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x00019240 File Offset: 0x00017440
		// (set) Token: 0x06000631 RID: 1585 RVA: 0x00019252 File Offset: 0x00017452
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

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x00019265 File Offset: 0x00017465
		// (set) Token: 0x06000633 RID: 1587 RVA: 0x00019277 File Offset: 0x00017477
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

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x0001928A File Offset: 0x0001748A
		// (set) Token: 0x06000635 RID: 1589 RVA: 0x0001929C File Offset: 0x0001749C
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

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x000192AF File Offset: 0x000174AF
		// (set) Token: 0x06000637 RID: 1591 RVA: 0x000192C1 File Offset: 0x000174C1
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

		// Token: 0x040003DB RID: 987
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}

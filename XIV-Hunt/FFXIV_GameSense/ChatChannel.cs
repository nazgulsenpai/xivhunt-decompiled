using System;

namespace FFXIV_GameSense
{
	// Token: 0x0200002C RID: 44
	[Serializable]
	public enum ChatChannel : byte
	{
		// Token: 0x040000A0 RID: 160
		None,
		// Token: 0x040000A1 RID: 161
		Debug,
		// Token: 0x040000A2 RID: 162
		UrgentInformation,
		// Token: 0x040000A3 RID: 163
		GeneralInformation,
		// Token: 0x040000A4 RID: 164
		Say = 10,
		// Token: 0x040000A5 RID: 165
		Shout,
		// Token: 0x040000A6 RID: 166
		Tell,
		// Token: 0x040000A7 RID: 167
		Tell_Receive,
		// Token: 0x040000A8 RID: 168
		Party,
		// Token: 0x040000A9 RID: 169
		Alliance,
		// Token: 0x040000AA RID: 170
		Linkshell1,
		// Token: 0x040000AB RID: 171
		Linkshell2,
		// Token: 0x040000AC RID: 172
		Linkshell3,
		// Token: 0x040000AD RID: 173
		Linkshell4,
		// Token: 0x040000AE RID: 174
		Linkshell5,
		// Token: 0x040000AF RID: 175
		Linkshell6,
		// Token: 0x040000B0 RID: 176
		Linkshell7,
		// Token: 0x040000B1 RID: 177
		Linkshell8,
		// Token: 0x040000B2 RID: 178
		FreeCompany,
		// Token: 0x040000B3 RID: 179
		NoviceNetwork = 27,
		// Token: 0x040000B4 RID: 180
		CustomEmotes,
		// Token: 0x040000B5 RID: 181
		StandardEmotes,
		// Token: 0x040000B6 RID: 182
		Yell,
		// Token: 0x040000B7 RID: 183
		Party2 = 32,
		// Token: 0x040000B8 RID: 184
		PvP = 36,
		// Token: 0x040000B9 RID: 185
		CWLS1,
		// Token: 0x040000BA RID: 186
		Damage = 41,
		// Token: 0x040000BB RID: 187
		FailedAttacks,
		// Token: 0x040000BC RID: 188
		Actions,
		// Token: 0x040000BD RID: 189
		Items,
		// Token: 0x040000BE RID: 190
		HealingMagic,
		// Token: 0x040000BF RID: 191
		BeneficialMagic,
		// Token: 0x040000C0 RID: 192
		DetrimentalEffects,
		// Token: 0x040000C1 RID: 193
		Echo = 56,
		// Token: 0x040000C2 RID: 194
		SystemMessages,
		// Token: 0x040000C3 RID: 195
		SystemErrorMessages,
		// Token: 0x040000C4 RID: 196
		BattleSystemMessages,
		// Token: 0x040000C5 RID: 197
		GatheringSystemMessages,
		// Token: 0x040000C6 RID: 198
		NPCSay,
		// Token: 0x040000C7 RID: 199
		LootNotices,
		// Token: 0x040000C8 RID: 200
		CharacterProgress = 64,
		// Token: 0x040000C9 RID: 201
		LootMessages,
		// Token: 0x040000CA RID: 202
		CraftingMessages,
		// Token: 0x040000CB RID: 203
		GatheringMessages,
		// Token: 0x040000CC RID: 204
		NPCAnnouncements,
		// Token: 0x040000CD RID: 205
		FCAnnouncements,
		// Token: 0x040000CE RID: 206
		FCLoginMessages,
		// Token: 0x040000CF RID: 207
		RetainerSaleReports,
		// Token: 0x040000D0 RID: 208
		PFRecruitmentNoficiation,
		// Token: 0x040000D1 RID: 209
		SignSettings,
		// Token: 0x040000D2 RID: 210
		DiceRolls,
		// Token: 0x040000D3 RID: 211
		NoviceNetworkNotifications,
		// Token: 0x040000D4 RID: 212
		CWLS2 = 101,
		// Token: 0x040000D5 RID: 213
		CWLS3,
		// Token: 0x040000D6 RID: 214
		CWLS4,
		// Token: 0x040000D7 RID: 215
		CWLS5,
		// Token: 0x040000D8 RID: 216
		CWLS6,
		// Token: 0x040000D9 RID: 217
		CWLS7,
		// Token: 0x040000DA RID: 218
		CWLS8
	}
}

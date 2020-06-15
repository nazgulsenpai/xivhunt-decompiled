using System;
using System.Collections.Generic;
using System.Linq;
using XIVDB;

namespace FFXIV_GameSense.UI
{
	// Token: 0x020000AD RID: 173
	public class FATEPresetViewItem
	{
		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x00014A66 File Offset: 0x00012C66
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x00014A6E File Offset: 0x00012C6E
		public string Name { get; private set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x00014A77 File Offset: 0x00012C77
		// (set) Token: 0x06000476 RID: 1142 RVA: 0x00014A7F File Offset: 0x00012C7F
		public IEnumerable<ushort> FATEIDs { get; private set; }

		// Token: 0x06000477 RID: 1143 RVA: 0x00014A88 File Offset: 0x00012C88
		public FATEPresetViewItem(RelicNote book)
		{
			if (book == null)
			{
				throw new ArgumentNullException("book");
			}
			this.Name = book.BookName.FirstLetterToUpperCase();
			this.FATEIDs = from x in book.FATEs
			select x.ID;
		}
	}
}

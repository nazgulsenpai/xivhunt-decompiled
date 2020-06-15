using System;
using System.Collections.Generic;
using System.Linq;
using XIVDB;

namespace FFXIV_GameSense.UI
{
	public class FATEPresetViewItem
	{
		public string Name { get; private set; }

		public IEnumerable<ushort> FATEIDs { get; private set; }

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

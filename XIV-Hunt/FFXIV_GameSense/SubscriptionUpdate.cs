using System;
using System.Collections;
using System.Collections.Specialized;

namespace FFXIV_GameSense
{
	internal class SubscriptionUpdate
	{
		public string Type { get; private set; }

		public NotifyCollectionChangedAction ChangeAction { get; private set; }

		public IEnumerable Items { get; private set; }

		public SubscriptionUpdate(string t, NotifyCollectionChangedAction changedAction, IEnumerable items)
		{
			this.Type = t;
			this.ChangeAction = changedAction;
			this.Items = items;
		}
	}
}

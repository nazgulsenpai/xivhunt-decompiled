using System;
using System.Collections;
using System.Collections.Specialized;

namespace FFXIV_GameSense
{
	// Token: 0x02000050 RID: 80
	internal class SubscriptionUpdate
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000B953 File Offset: 0x00009B53
		// (set) Token: 0x06000239 RID: 569 RVA: 0x0000B95B File Offset: 0x00009B5B
		public string Type { get; private set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600023A RID: 570 RVA: 0x0000B964 File Offset: 0x00009B64
		// (set) Token: 0x0600023B RID: 571 RVA: 0x0000B96C File Offset: 0x00009B6C
		public NotifyCollectionChangedAction ChangeAction { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600023C RID: 572 RVA: 0x0000B975 File Offset: 0x00009B75
		// (set) Token: 0x0600023D RID: 573 RVA: 0x0000B97D File Offset: 0x00009B7D
		public IEnumerable Items { get; private set; }

		// Token: 0x0600023E RID: 574 RVA: 0x0000B986 File Offset: 0x00009B86
		public SubscriptionUpdate(string t, NotifyCollectionChangedAction changedAction, IEnumerable items)
		{
			this.Type = t;
			this.ChangeAction = changedAction;
			this.Items = items;
		}
	}
}

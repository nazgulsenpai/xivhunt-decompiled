using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace FFXIV_GameSense.Properties
{
	// Token: 0x020000C3 RID: 195
	[Serializable]
	public class ObservableHashSet<T> : INotifyCollectionChanged, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ISet<T>, IDeserializationCallback, ISerializable
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x00017A02 File Offset: 0x00015C02
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x00017A0A File Offset: 0x00015C0A
		private HashSet<T> HashSet { get; set; } = new HashSet<T>();

		// Token: 0x06000511 RID: 1297 RVA: 0x00017A13 File Offset: 0x00015C13
		public ObservableHashSet() : this(EqualityComparer<T>.Default)
		{
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x00017A20 File Offset: 0x00015C20
		public ObservableHashSet(IEqualityComparer<T> comparer)
		{
			this.HashSet = new HashSet<T>(comparer);
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x00017A3F File Offset: 0x00015C3F
		public int Count
		{
			get
			{
				return this.HashSet.Count;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000514 RID: 1300 RVA: 0x00017A4C File Offset: 0x00015C4C
		public bool IsReadOnly
		{
			get
			{
				return ((ICollection<T>)this.HashSet).IsReadOnly;
			}
		}

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000515 RID: 1301 RVA: 0x00017A5C File Offset: 0x00015C5C
		// (remove) Token: 0x06000516 RID: 1302 RVA: 0x00017A94 File Offset: 0x00015C94
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		// Token: 0x06000517 RID: 1303 RVA: 0x00017AC9 File Offset: 0x00015CC9
		private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
			if (collectionChanged == null)
			{
				return;
			}
			collectionChanged(this, args);
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x00017ADD File Offset: 0x00015CDD
		private void OnNotifyCollectionChanged(IList newItems, IList oldItems)
		{
			this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems));
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00017AED File Offset: 0x00015CED
		public void Add(T item)
		{
			if (this.HashSet.Add(item))
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x00017B10 File Offset: 0x00015D10
		public void Clear()
		{
			if (this.HashSet.Count == 0)
			{
				return;
			}
			List<T> oldItems = this.HashSet.ToList<T>();
			this.HashSet.Clear();
			this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems));
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x00017B4F File Offset: 0x00015D4F
		public bool Contains(T item)
		{
			return this.HashSet.Contains(item);
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00017B5D File Offset: 0x00015D5D
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.HashSet.CopyTo(array, arrayIndex);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00017B6C File Offset: 0x00015D6C
		public bool Remove(T item)
		{
			if (this.HashSet.Remove(item))
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}
			return false;
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x00017B8F File Offset: 0x00015D8F
		public IEnumerator<T> GetEnumerator()
		{
			return this.HashSet.GetEnumerator();
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x00017BA1 File Offset: 0x00015DA1
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x00017BA9 File Offset: 0x00015DA9
		bool ISet<T>.Add(T item)
		{
			bool flag = this.HashSet.Add(item);
			if (flag)
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
			return flag;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00017BCC File Offset: 0x00015DCC
		public bool AddRange(IEnumerable<T> items)
		{
			List<T> itemsToAdd = (from x in items
			where !this.HashSet.Contains(x)
			select x).ToList<T>();
			if (!itemsToAdd.Any<T>())
			{
				return false;
			}
			int countBeforeAdd = this.Count;
			foreach (T i in itemsToAdd)
			{
				this.HashSet.Add(i);
			}
			this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsToAdd));
			return this.Count == countBeforeAdd + itemsToAdd.Count<T>();
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x00017C68 File Offset: 0x00015E68
		public bool RemoveRange(IEnumerable<T> items)
		{
			List<T> itemsToRemove = (from x in items
			where this.HashSet.Contains(x)
			select x).ToList<T>();
			if (!itemsToRemove.Any<T>())
			{
				return false;
			}
			int countBeforeRemove = this.Count;
			foreach (T i in itemsToRemove)
			{
				this.HashSet.Remove(i);
			}
			this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsToRemove));
			return this.Count == countBeforeRemove + itemsToRemove.Count<T>();
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00017D04 File Offset: 0x00015F04
		public void UnionWith(IEnumerable<T> other)
		{
			int c = this.Count;
			IEnumerable<T> uniqueItemsToAdd = from x in other
			where !this.HashSet.Contains(x)
			select x;
			this.HashSet.UnionWith(other);
			if (this.Count != c)
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, uniqueItemsToAdd));
			}
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00017D50 File Offset: 0x00015F50
		public void IntersectWith(IEnumerable<T> other)
		{
			List<T> itemsToBeRemoved = (from x in this.HashSet
			where !other.Contains(x)
			select x).ToList<T>();
			if (itemsToBeRemoved.Any<T>())
			{
				this.HashSet.IntersectWith(other);
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsToBeRemoved));
			}
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00017DB0 File Offset: 0x00015FB0
		public void ExceptWith(IEnumerable<T> other)
		{
			List<T> itemsToBeRemoved = (from x in this.HashSet
			where other.Contains(x)
			select x).ToList<T>();
			if (itemsToBeRemoved.Any<T>())
			{
				this.HashSet.ExceptWith(other);
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsToBeRemoved));
			}
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00017E10 File Offset: 0x00016010
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			IEnumerable<T> itemsToBeRemoved = from x in this.HashSet
			where other.Contains(x)
			select x;
			IEnumerable<T> itemsToBeAdded = from x in this.HashSet
			where other.Contains(x)
			select x;
			if (!itemsToBeAdded.Any<T>() && !itemsToBeRemoved.Any<T>())
			{
				return;
			}
			this.RemoveRange(itemsToBeRemoved);
			this.AddRange(itemsToBeAdded);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00017E7B File Offset: 0x0001607B
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsSubsetOf(other);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00017E89 File Offset: 0x00016089
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsSupersetOf(other);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x00017E97 File Offset: 0x00016097
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsProperSupersetOf(other);
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x00017EA5 File Offset: 0x000160A5
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsProperSubsetOf(other);
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00017EB3 File Offset: 0x000160B3
		public bool Overlaps(IEnumerable<T> other)
		{
			return this.HashSet.Overlaps(other);
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00017EC1 File Offset: 0x000160C1
		public bool SetEquals(IEnumerable<T> other)
		{
			return this.HashSet.SetEquals(other);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00017ECF File Offset: 0x000160CF
		public void OnDeserialization(object sender)
		{
			this.HashSet.OnDeserialization(sender);
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00017EDD File Offset: 0x000160DD
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.HashSet.GetObjectData(info, context);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;

namespace FFXIV_GameSense.Properties
{
	[Serializable]
	public class ObservableHashSet<T> : INotifyCollectionChanged, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ISet<T>, IDeserializationCallback, ISerializable
	{
		private HashSet<T> HashSet { get; set; } = new HashSet<T>();

		public ObservableHashSet() : this(EqualityComparer<T>.Default)
		{
		}

		public ObservableHashSet(IEqualityComparer<T> comparer)
		{
			this.HashSet = new HashSet<T>(comparer);
		}

		public int Count
		{
			get
			{
				return this.HashSet.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((ICollection<T>)this.HashSet).IsReadOnly;
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
			if (collectionChanged == null)
			{
				return;
			}
			collectionChanged(this, args);
		}

		private void OnNotifyCollectionChanged(IList newItems, IList oldItems)
		{
			this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItems, oldItems));
		}

		public void Add(T item)
		{
			if (this.HashSet.Add(item))
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

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

		public bool Contains(T item)
		{
			return this.HashSet.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.HashSet.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (this.HashSet.Remove(item))
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.HashSet.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		bool ISet<T>.Add(T item)
		{
			bool flag = this.HashSet.Add(item);
			if (flag)
			{
				this.OnNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
			return flag;
		}

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

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsSupersetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsProperSupersetOf(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return this.HashSet.IsProperSubsetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return this.HashSet.Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return this.HashSet.SetEquals(other);
		}

		public void OnDeserialization(object sender)
		{
			this.HashSet.OnDeserialization(sender);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.HashSet.GetObjectData(info, context);
		}
	}
}

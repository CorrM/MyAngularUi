using System;
using System.Collections.Generic;
using System.Text;
using MAU.Attributes;
using MAU.Core;

namespace MAU.Helper.Types
{
	public class MauDataList<T> : List<T>, IMauDataType
	{
		public string MauDataName { get; }
		public MauElement Holder { get; }

		public MauDataList(MauElement holder, string mauDataName)
		{
			MauDataName = mauDataName;
			Holder = holder;
		}

		public void UpdateData()
		{
			MauVariable.UpdateVar(Holder, MauDataName);
		}

		public new void Add(T item)
		{
			base.Add(item);
			UpdateData();
		}
		public new void AddRange(IEnumerable<T> collection)
		{
			base.AddRange(collection);
			UpdateData();
		}
		public new void Clear()
		{
			base.Clear();
			UpdateData();
		}
		public new void Insert(int index, T item)
		{
			base.Insert(index, item);
			UpdateData();
		}
		public new void InsertRange(int index, IEnumerable<T> collection)
		{
			base.InsertRange(index, collection);
			UpdateData();
		}
		public new bool Remove(T item)
		{
			bool retVal = base.Remove(item);
			UpdateData();

			return retVal;
		}
		public new int RemoveAll(Predicate<T> match)
		{
			int retVal = base.RemoveAll(match);
			UpdateData();

			return retVal;
		}
		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			UpdateData();
		}
		public new void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			UpdateData();
		}
		public new void Reverse(int index, int count)
		{
			base.Reverse(index, count);
			UpdateData();
		}
		public new void Reverse()
		{
			base.Reverse();
			UpdateData();
		}
		public new void Sort(Comparison<T> comparison)
		{
			base.Sort(comparison);
			UpdateData();
		}
		public new void Sort(int index, int count, IComparer<T> comparer)
		{
			base.Sort(index, count, comparer);
			UpdateData();
		}
		public new void Sort()
		{
			base.Sort();
			UpdateData();
		}
		public new void Sort(IComparer<T> comparer)
		{
			base.Sort(comparer);
			UpdateData();
		}
	}
}

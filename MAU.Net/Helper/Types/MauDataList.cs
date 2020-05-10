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
	}
}

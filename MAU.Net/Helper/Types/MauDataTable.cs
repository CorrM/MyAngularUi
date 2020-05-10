using System;
using System.Collections.Generic;
using System.Text;
using MAU.Attributes;
using MAU.Core;

namespace MAU.Helper.Types
{
	public class MauDataTable : IMauDataType
	{
		public string MauDataName { get; }
		public MauElement Holder { get; }

		public MauDataList<List<string>> Rows { get; }
		public MauDataList<string> Columns { get; }

		public MauDataTable(MauElement holder, string mauDataName)
		{
			MauDataName = mauDataName;
			Holder = holder;

			Rows = new MauDataList<List<string>>(holder, mauDataName);
			Columns = new MauDataList<string>(holder, mauDataName);
		}

		public void UpdateData() { }
	}
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;

namespace MAU.ReadyElement
{
	public class MauTable : MauElement
	{
		#region [ Mau Properties ]

		#endregion

		#region [ Mau Variable ]

		[MauVariable]
		public DataTable Content { get; }

		#endregion

		#region [ Mau Methods ]

		#endregion

		public MauTable(MauComponent parentComponent, string mauId) : base(parentComponent, mauId)
		{
			Content = new DataTable();
		}

		public void UpdateTable()
		{
			MauVariable.UpdateVar(this, nameof(Content));
		}
	}
}

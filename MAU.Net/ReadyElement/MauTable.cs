using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using MAU.Helper.Types;

namespace MAU.ReadyElement
{
	public class MauTable : MauElement
	{
		#region [ Mau Properties ]

		#endregion

		#region [ Mau Variable ]

		[MauVariable]
		public MauDataTable Content { get; }

		#endregion

		#region [ Mau Methods ]

		#endregion

		public MauTable(MauComponent parentComponent, string mauId) : base(parentComponent, mauId)
		{
			Content = new MauDataTable(this, nameof(Content));
		}
	}
}

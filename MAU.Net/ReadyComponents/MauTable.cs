using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Types;

namespace MAU.ReadyComponents
{
	public class MauTable : MauComponent
	{
		#region [ Mau Properties ]

		#endregion

		#region [ Mau Variable ]

		[MauVariable]
		public MauDataTable Content { get; }

		#endregion

		#region [ Mau Methods ]

		#endregion

		public MauTable(string mauId) : base(mauId)
		{
			Content = new MauDataTable(this, nameof(Content));
		}
	}
}

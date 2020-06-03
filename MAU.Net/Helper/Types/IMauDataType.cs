using MAU.Core;

namespace MAU.Helper.Types
{
	public interface IMauDataType
	{
		public string MauDataName { get; }
		public MauComponent Holder { get; }

		public void UpdateData();
	}
}
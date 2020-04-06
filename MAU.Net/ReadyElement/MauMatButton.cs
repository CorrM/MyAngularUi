using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Types;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement
{
	public class MauMatButton : MauElement
	{
		#region [ Mau Properties ]

		[MauProperty("color", MauPropertyType.ComponentProperty)]
		public ThemePalette Color { get; set; }

		[MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
		public bool DisableRipple { get; set; }

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		[MauProperty("isIconButton", MauPropertyType.ComponentProperty)]
		public bool IsIconButton { get; set; }

		[MauProperty("isRoundButton", MauPropertyType.ComponentProperty)]
		public bool IsRoundButton { get; set; }

		#endregion

		#region [ Mau Methods ]

		[MauMethod("focus", MauMethodType.ComponentMethod)]
		public void Focus() { }

		#endregion

		public MauMatButton(MauComponent parentComponent, string mauId) : base(parentComponent, mauId) { }

		public void Test()
		{
			//MauPropertyType gg = MauPropertyType.ComponentProperty;
			//object val = MauEnumMember.HasAttribute(gg);
			//val = MauEnumMember.HasAttribute(Color);

			//if (val == null)
			//{

			//}
		}
	}
}

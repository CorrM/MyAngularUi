using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;

namespace MAU.ReadyElement.Angular
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
	}
}

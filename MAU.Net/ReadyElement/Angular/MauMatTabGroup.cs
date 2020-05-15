using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement.Angular
{
	public class MauMatTabGroup : MauElement
	{
		#region [ Mau Events ]

		[MauEvent("animationDone")]
		public event MauEventHandler AnimationDone;

		[MauEvent("focusChange")]
		public event MauEventHandler FocusChange;

		[MauEvent("selectedIndexChange")]
		public event MauEventHandler SelectedIndexChange;

		[MauEvent("selectedTabChange")]
		public event MauEventHandler SelectedTabChange;

		#endregion

		#region [ Mau Properties ]

		[MauProperty("animationDuration", MauPropertyType.ComponentProperty)]
		public string AnimationDuration { get; set; }

		[MauProperty("backgroundColor", MauPropertyType.ComponentProperty)]
		public ThemePalette BackgroundColor { get; set; }

		[MauProperty("color", MauPropertyType.ComponentProperty)]
		public ThemePalette Color { get; set; }

		[MauProperty("disablePagination", MauPropertyType.ComponentProperty)]
		public bool DisablePagination { get; set; }

		[MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
		public bool DisableRipple { get; set; }

		[MauProperty("dynamicHeight", MauPropertyType.ComponentProperty)]
		public bool DynamicHeight { get; set; }

		[MauProperty("headerPosition", MauPropertyType.ComponentProperty)]
		public MatTabHeaderPosition HeaderPosition { get; set; }

		[MauProperty("selectedIndex", MauPropertyType.ComponentProperty)]
		public int SelectedIndex { get; set; }

		#endregion

		#region [ Mau Methods ]

		[MauMethod("realignInkBar", MauMethodType.ComponentMethod)]
		public void RealignInkBar() { }

		#endregion

		public MauMatTabGroup(MauComponent parentComponent, string mauId) : base(parentComponent, mauId) { }
	}
}

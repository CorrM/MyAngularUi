using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement.Angular
{
	public class MauMatSlideToggle : MauElement
	{
		#region [ Mau Events ]

		[MauEvent("change")]
		public event MauEventHandler Change;

		[MauEvent("toggleChange")]
		public event MauEventHandler ToggleChange;

		#endregion

		#region [ Mau Properties ]

		[MauProperty("checked", MauPropertyType.ComponentProperty)]
		public bool Checked { get; set; }

		[MauProperty("color", MauPropertyType.ComponentProperty)]
		public ThemePalette Color { get; set; }

		[MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
		public bool DisableRipple { get; set; }

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		[MauProperty("id", MauPropertyType.ComponentProperty)]
		public string Id { get; set; }

		[MauProperty("labelPosition", MauPropertyType.ComponentProperty)]
		public CheckBoxLabelPosition LabelPosition { get; set; }

		[MauProperty("name", MauPropertyType.ComponentProperty)]
		public string Name { get; set; }

		[MauProperty("required", MauPropertyType.ComponentProperty)]
		public bool Required { get; set; }

		[MauProperty("inputId", MauPropertyType.ComponentProperty)]
		public string InputId { get; set; }

		#endregion

		#region [ Mau Methods ]

		[MauMethod("focus", MauMethodType.ComponentMethod)]
		public void Focus() { }

		[MauMethod("toggle", MauMethodType.ComponentMethod)]
		public void Toggle() { }

		#endregion

		public MauMatSlideToggle(MauComponent parentComponent, string mauId) : base(parentComponent, mauId) { }
	}
}

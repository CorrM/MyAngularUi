using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement.Angular
{
	public class MauMatInput : MauElement
	{
		#region [ Mau Events ]

		[MauEvent("input")]
		public event MauEventHandler InputChange;

		#endregion

		#region [ Mau Properties ]

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		[MauProperty("placeholder", MauPropertyType.NativeAttribute)]
		public string Placeholder { get; set; }

		[MauProperty("autocomplete", MauPropertyType.NativeAttribute)]
		public OnOff Autocomplete { get; set; }

		[MauProperty("type", MauPropertyType.ComponentProperty)]
		public string InputType { get; set; }

		[MauProperty("readOnly", MauPropertyType.ComponentProperty)]
		public bool Readonly { get; set; }

		[MauProperty("value", MauPropertyType.NativeProperty)]
		public string Value { get; set; }

		#endregion

		public MauMatInput(MauComponent parentComponent, string mauId) : base(parentComponent, mauId) { }
	}
}

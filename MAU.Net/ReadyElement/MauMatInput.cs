using MAU.Attributes;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement
{
	public class MauMatInput : MauElement
	{
		#region [ Events ]

		[MauEvent("input")]
		public event MauEventHandler InputChange;

		#endregion

		#region [ Public Props ]

		[MauProperty("placeholder", MauPropertyType.NativeAttribute)]
		public string Placeholder { get; set; }

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

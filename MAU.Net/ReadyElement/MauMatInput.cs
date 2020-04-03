using MAU.Attributes;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement
{
	public class MauMatInput : MauElement
	{
		#region [ Events ]

		#endregion

		#region [ Public Props ]

		[MauProperty("placeholder", MauPropertyType.NativeAttribute)]
		public string Placeholder { get; set; }

		[MauProperty("type", MauPropertyType.ComponentProperty)]
		public string InputType { get; set; }

		[MauProperty("readonly", MauPropertyType.ComponentProperty)]
		public bool Readonly { get; set; }

		#endregion

		public MauMatInput(MauComponent parentComponent, string mauId) : base(parentComponent, mauId) { }
	}
}

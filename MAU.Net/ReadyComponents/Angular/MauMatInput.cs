using MAU.Attributes;
using MAU.Core;
using MAU.Events;
using MAU.Helper.Enums;
using Newtonsoft.Json.Linq;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular
{
	public class MauMatInput : MauComponent
	{
		#region [ Mau Events ]

		[MauEvent("input")]
		public event MauEventHandler OnInputChange;

		public event MauEventHandler OnValueChange;

		#endregion

		#region [ Mau Properties ]

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		[MauProperty("placeholder", MauPropertyType.NativeAttribute)]
		public string Placeholder { get; set; }

		[MauProperty("autocomplete", MauPropertyType.NativeAttribute)]
		public Off Autocomplete { get; set; }

		[MauProperty("type", MauPropertyType.ComponentProperty)]
		public string InputType { get; set; }

		[MauProperty("readOnly", MauPropertyType.ComponentProperty)]
		public bool Readonly { get; set; }

		[MauProperty("value", MauPropertyType.NativeProperty)]
		public string Value { get; set; }

		#endregion

		public MauMatInput(string mauId) : base(mauId) { }

		/// <summary>
		/// Auto called function, called when <see cref="Value"/> changed
		/// </summary>
		/// <param name="mauComponent">Instance of the property.</param>
		private static void ValueOnSet(MauComponent mauComponent)
		{
			((MauMatInput) mauComponent).OnValueChange?.Invoke(mauComponent, new MauEventInfo("ValueChange", "ValueChange", new JObject()));
		}
	}
}

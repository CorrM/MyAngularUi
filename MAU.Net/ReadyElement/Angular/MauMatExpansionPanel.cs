using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyElement.Angular
{
	public class MauMatExpansionPanel : MauElement
	{
		#region [ Mau Events ]

		[MauEvent("afterCollapse")]
		public event MauEventHandler AfterCollapse;

		[MauEvent("afterExpand")]
		public event MauEventHandler AfterExpand;

		[MauEvent("closed")]
		public event MauEventHandler Closed;

		[MauEvent("destroyed")]
		public event MauEventHandler Destroyed;

		[MauEvent("opened")]
		public event MauEventHandler Opened;

		#endregion

		#region [ Mau Properties ]

		[MauProperty("disabled", MauPropertyType.ComponentProperty)]
		public bool Disabled { get; set; }

		[MauProperty("expanded", MauPropertyType.ComponentProperty)]
		public bool Expanded { get; set; }

		[MauProperty("hideToggle", MauPropertyType.ComponentProperty)]
		public bool HideToggle { get; set; }

		[MauProperty("togglePosition", MauPropertyType.ComponentProperty)]
		public MatAccordionTogglePosition TogglePosition { get; set; }

		[MauProperty("id", MauPropertyType.ComponentProperty)]
		public string Id { get; set; }

		#endregion

		#region [ Mau Methods ]

		[MauMethod("close", MauMethodType.ComponentMethod)]
		public void Close() { }

		[MauMethod("open", MauMethodType.ComponentMethod)]
		public void Open() { }

		[MauMethod("toggle", MauMethodType.ComponentMethod)]
		public void Toggle() { }

		#endregion

		public MauMatExpansionPanel(MauComponent parentComponent, string mauId) : base(parentComponent, mauId) { }
	}
}

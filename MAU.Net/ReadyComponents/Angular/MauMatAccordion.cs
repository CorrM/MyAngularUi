using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;

namespace MAU.ReadyComponents.Angular
{
	public class MauMatAccordion : MauComponent
	{
		#region [ Mau Properties ]

		[MauProperty("displayMode", MauPropertyType.ComponentProperty)]
		public MatAccordionDisplayMode DisplayMode { get; set; }

		[MauProperty("hideToggle", MauPropertyType.ComponentProperty)]
		public bool HideToggle { get; set; }

		[MauProperty("multi", MauPropertyType.ComponentProperty)]
		public bool Multi { get; set; }

		[MauProperty("togglePosition", MauPropertyType.ComponentProperty)]
		public MatAccordionTogglePosition TogglePosition { get; set; }

		#endregion

		#region [ Mau Methods ]

		[MauMethod("closeAll", MauMethodType.ComponentMethod)]
		public void CloseAll() { }

		[MauMethod("openAll", MauMethodType.ComponentMethod)]
		public void OpenAll() { }

		#endregion

		public MauMatAccordion(string mauId) : base(mauId) { }
	}
}

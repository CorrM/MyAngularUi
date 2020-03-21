using MAU.Attributes;
using static MAU.Events.UiEventHandler;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		[UiEvent("click")]
		public event MauEventHandler Click;

		public UiButton(string id) : base(id) { }
	}
}

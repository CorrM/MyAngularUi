using System.Diagnostics;
using MAU.Attributes;
using MAU.Events;
using Newtonsoft.Json.Linq;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		[UiEvent("click")]
		public event UiEventHandler.MauEventHandler Click;

		public UiButton(string id) : base(id) { }
	}
}

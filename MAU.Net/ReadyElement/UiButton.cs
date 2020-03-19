using System.Diagnostics;
using MAU.Attributes;
using Newtonsoft.Json.Linq;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		public UiButton(string id) : base(id) { }

		[UiEvent("click")]
		public void OnClick(string eventType, JObject eventData)
		{
			Debug.WriteLine("Click Event Called");
			Html = "<b>CorrM</b>";
		}
	}
}

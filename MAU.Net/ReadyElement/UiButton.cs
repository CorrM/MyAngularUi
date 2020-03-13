using System.Diagnostics;
using MAU.Attributes;
using Newtonsoft.Json.Linq;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		[UiProperty("data-gg")]
		public string Gg { get; set; }

		public UiButton(string id) : base(id) { }

		[UiEvent("click")]
		public void OnClick(string eventType, JObject eventData)
		{
			Debug.WriteLine("Click Event Called");
			Debug.WriteLine($"Gg => {Gg}");
			Html = "<b>CorrM</b>";
		}
	}
}

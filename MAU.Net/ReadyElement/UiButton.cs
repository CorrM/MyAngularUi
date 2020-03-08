using System.Diagnostics;
using MAU.Attributes;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		[UiProperty("innerText")]
		public string Text { get; set; }

		public UiButton(string id) : base(id) { }

		[UiEvent("click")]
		public void OnClick()
		{
			Debug.WriteLine("Click Event Called");
			Text = "CorrM";
		}
	}
}

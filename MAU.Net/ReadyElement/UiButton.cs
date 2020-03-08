using System.Diagnostics;
using MAU.Attributes;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		

		public UiButton(string id) : base(id) { }

		[UiEvent("click")]
		public void OnClick()
		{
			Debug.WriteLine("Click Event Called");
			Html = "<b>CorrM</b>";
		}
	}
}

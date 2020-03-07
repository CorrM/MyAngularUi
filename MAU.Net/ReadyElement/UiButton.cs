using System.Diagnostics;

namespace MAU.ReadyElement
{
	public class UiButton : UiElement
	{
		public UiButton(string id) : base(id)
		{
		}

		[UiEvent("click")]
		public void OnClick()
		{
			Debug.WriteLine("Click Event Called");
		}
	}
}

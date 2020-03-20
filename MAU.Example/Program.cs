using System;
using System.Threading.Tasks;
using MAU.ReadyElement;

namespace MAU.Example
{
	public class Program
	{
		public static void Main(string[] args) => MainTask().GetAwaiter().GetResult();

		private static void InitElements()
		{
			// FirstUi
			var btn = new UiButton("FirstUi") { Html = "<b>CorrMHere</b>" };
			MyAngularUi.RegisterUi(btn);
		}

		public static async Task MainTask()
		{
			MyAngularUi.Setup(3000);
			await MyAngularUi.Start();

			InitElements();

			while (true)
				await Task.Delay(8);
		}
	}
}

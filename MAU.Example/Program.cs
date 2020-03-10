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
			var btn = new UiButton("FirstUi")
			{
				Html = "<b>TEST</b>"
			};
			MyAngularUi.RegisterUi(btn);
		}

		public static async Task MainTask()
		{
			// Init MyAngularUi
			using var ws = MyAngularUi.Instance(3000);
			await ws.Start();

			InitElements();

			while (true)
				await Task.Delay(8);
		}
	}
}

using System;
using System.Threading.Tasks;
using MAU.ReadyElement;

namespace MAU.Example
{
	public class Program
	{
		public static void Main(string[] args) => MainTask().GetAwaiter().GetResult();

		public static async Task MainTask()
		{
			MyAngularUi.RegisterUi(new UiButton("FirstUi"));

			using var ws = new MyAngularUi(3000);
			await ws.Start();
			while (true)
				await Task.Delay(8);
		}
	}
}

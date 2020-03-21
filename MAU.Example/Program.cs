using System;
using System.Threading.Tasks;
using MAU.ReadyElement;
using Newtonsoft.Json.Linq;

namespace MAU.Example
{
	public class Program
	{
		public static void Main(string[] args) => MainTask().GetAwaiter().GetResult();

		private static void InitElements()
		{
			// FirstUi
			var btn = new UiButton("FirstUi") { Html = "<b>CorrMHere</b>" };
			btn.Click += Btn_Click;
			MyAngularUi.RegisterUi(btn);
		}

		private static void Btn_Click(string eventType, JObject eventData)
		{
			Console.WriteLine(eventType + " Event Called.");
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

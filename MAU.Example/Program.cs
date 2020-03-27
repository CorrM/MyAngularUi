using System;
using System.Threading.Tasks;
using MAU.ReadyElement;
using Newtonsoft.Json.Linq;

namespace MAU.Example
{
	public class Program
	{
		public static void Main(string[] args) => MainTask().GetAwaiter().GetResult();

		private static async Task InitElements()
		{
			// FirstUi
			var sele = new MauSelect("FirstUi", "option");
			sele.Click += Btn_Click;
			MyAngularUi.RegisterUi(sele);

			await sele.AddOption("", "");
		}

		private static void Btn_Click(string eventName, Events.MauEventInfo eventInfo)
		{
			Console.WriteLine(eventInfo.TypeName + " Event Called.");
		}

		public static async Task MainTask()
		{
			MyAngularUi.Setup(3000);
			await MyAngularUi.Start();

			await InitElements();

			while (true)
				await Task.Delay(8);
		}
	}
}

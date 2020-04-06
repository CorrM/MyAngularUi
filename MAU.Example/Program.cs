using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAU.ReadyElement;
using Newtonsoft.Json.Linq;

namespace MAU.Example
{
	public class Program
	{
		public static void Main(string[] args) => MainTask().GetAwaiter().GetResult();

		public static async Task MainTask()
		{
			// Setup MyAngularUi
			MyAngularUi.Setup(3000);
			await MyAngularUi.Start();

			await Task.Delay(1000);

			// 
			var setupComp = new SetupComponent();

			while (true)
				await Task.Delay(8);
		}
	}
}

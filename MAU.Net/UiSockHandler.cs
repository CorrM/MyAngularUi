using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketNet;
using WebSocketNet.Server;

namespace MAU
{
	public class UiSockHandler : WebSocketBehavior
	{
		public static bool Finished = false;

		protected override void OnOpen()
		{
			Debug.WriteLine("OPENED");
			var ret = new JObject
			{
				{ "message", "Connected" }
			};
			Send(ret.ToString());
			Finished = true;
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Debug.WriteLine(e.Data);
			Send("GG");
		}
	}
}

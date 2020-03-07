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
		public enum RequestType
		{
			None = 0,
			GetEvents = 1,
			EventCallback = 2,
		}

		public static bool Finished = false;

		protected override void OnOpen()
		{
			Debug.WriteLine("OPENED");
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Debug.WriteLine(e.Data);
			Finished = true;

			JObject jsonData = JObject.Parse(e.Data);
			int orderId = jsonData["orderId"].Value<int>();
			var requestType = (RequestType)jsonData["requestType"].Value<int>();
			Debug.WriteLine($"order => {orderId}, Type => {requestType:G}");

			var ret = new JObject
			{
				{ "orderId", orderId },
				{ "uiElementId", "Connected" },
				{ "message", "Connected" }
			};
			Send(ret.ToString());

		}
	}
}

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
			Debug.WriteLine($"Recv > {e.Data}");

			// Decode json
			JObject jsonData = JObject.Parse(e.Data);

			// Get request info
			int orderId = jsonData["orderId"].Value<int>();
			string uiId = jsonData["uiElementId"].Value<string>();
			var requestType = (RequestType)jsonData["requestType"].Value<int>();

			// Check if ui is registered
			if (!MyAngularUi.GetUiElement(uiId, out UiElement uiElement))
				throw new KeyNotFoundException("UiElement not found.");

			// Process
			JObject ret = null;
			switch (requestType)
			{
				case RequestType.None:
					break;

				case RequestType.GetEvents:
					ret = new JObject
					{
						{ "orderId", orderId },
						{ "uiElementId", uiId },
						{ "events", new JArray(uiElement.Events) }
					};
					break;

				case RequestType.EventCallback:
					string eventName = jsonData["data"]["eventName"].Value<string>();
					uiElement.FireEvent(eventName);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			// Send response
			if (ret == null)
			{
				ret = new JObject
				{
					{ "orderId", orderId }
				};
			}

			Debug.WriteLine($"Send > {ret.ToString(Formatting.None)}");
			Send(ret.ToString());
			Finished = true;
		}
	}
}

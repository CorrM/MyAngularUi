using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketNet;
using WebSocketNet.Server;

namespace MAU
{
	public class MyAngularUi : IDisposable
	{
		public enum RequestType
		{
			None = 0,
			GetEvents = 1,
			EventCallback = 2,
			GetPropValue = 3,
			SetPropValue = 4
		}

		#region [ Fields ]

		private static MyAngularUi _instance;
		private static Dictionary<string, UiElement> UiElements { get; } = new Dictionary<string, UiElement>();

		#endregion

		#region [ Props ]

		public WebSocketServer WebSocket { get; private set; }
		public int Port { get; }

		#endregion

		#region [ Consturtor ]

		protected MyAngularUi(int webSocketPort)
		{
			Port = webSocketPort;
		}
		public static MyAngularUi Instance(int webSocketPort)
		{
			return _instance ??= new MyAngularUi(webSocketPort);
		}
		public static MyAngularUi Instance()
		{
			if (_instance == null)
				throw new NullReferenceException("Call 'Instance(int)' first");

			return _instance;
		}

		#endregion

		#region [ WebSocket Wrapper ]

		public Task<bool> Start()
		{
			return Task.Run(() =>
			{
				WebSocket = new WebSocketServer(Port);
				WebSocket.AddWebSocketService<UiSockHandler>("/UiHandler");

				WebSocket.Start();
				return WebSocket.IsListening;
			});
		}

		internal static Task Send(string uiElementId, RequestType requestType, JObject data = default)
		{
			if (_instance == null)
				throw new NullReferenceException("Create Instance First.!");

			return Task.Run(() =>
			{
				var dSend = new JObject
				{
					{ "requestType", (int)requestType },
					{ "uiElementId", uiElementId },
					{ "data", data }
				};

				string dataToSend = dSend.ToString(Formatting.None);

				if (UiSockHandler.Instance.Send(dataToSend))
					Debug.WriteLine($"Send > {dataToSend}");

				Debug.WriteLine("===============");
			});
		}

		internal async Task OnMessage(MessageEventArgs e)
		{
			Debug.WriteLine($"Recv > {e.Data}");

			// Decode json
			JObject jsonRequest = JObject.Parse(e.Data);
			var jsonData = jsonRequest["data"].Value<JObject>();

			// Get request info
			string uiId = jsonRequest["uiElementId"].Value<string>();
			var requestType = (RequestType)jsonRequest["requestType"].Value<int>();

			// Check if ui is registered
			if (!GetUiElement(uiId, out UiElement uiElement))
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
						{ "events", new JArray(uiElement.Events) }
					};
					break;

				case RequestType.EventCallback:
					string eventName = jsonData["eventName"].Value<string>();
					_ = Task.Run(() => uiElement.FireEvent(eventName));
					break;

				case RequestType.GetPropValue:
					string propName = jsonData["propName"].Value<string>();
					string propValue = jsonData["propValue"].Value<string>();

					uiElement.SetPropValue(propName, propValue);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			ret ??= new JObject();
			
			// Send response
			await Send(uiId, requestType, ret);
		}
		#endregion

		#region [ UiHandler ]

		public static void RegisterUi(UiElement element)
		{
			UiElements.Add(element.Id, element);
		}
		public static void RegisterUi(ICollection<UiElement> element)
		{
			foreach (UiElement uiElement in element)
				UiElements.Add(uiElement.Id, uiElement);
		}
		public static bool GetUiElement(string elementId, out UiElement element)
		{
			if (UiElements.ContainsKey(elementId))
			{
				element = UiElements[elementId];
				return true;
			}

			element = null;
			return false;
		}

		#endregion

		#region IDisposable Support
		private bool _disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue)
				return;

			if (disposing)
			{
				WebSocket?.Stop();
			}

			_disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}

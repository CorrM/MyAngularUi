using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketNet;
using WebSocketNet.Server;
using MAU.Core;
using MAU.DataParser;
using MAU.WebSocket;

namespace MAU
{
	public static class MyAngularUi
	{
		public enum RequestType
		{
			/// <summary>
			/// I just love to set none in my enums xD
			/// </summary>
			None = 0,

			/// <summary>
			/// Angular request to know what events,
			/// handled in dotnet
			/// </summary>
			GetEvents = 1,

			/// <summary>
			/// Angular fire event
			/// </summary>
			EventCallback = 2,

			/// <summary>
			/// Request angular to send property value
			/// </summary>
			GetPropValue = 3,

			/// <summary>
			/// Set property value in angular side
			/// </summary>
			SetPropValue = 4,

			/// <summary>
			/// Execute TypeScript code in front-end side
			/// </summary>
			ExecuteCode = 5,

			/// <summary>
			/// Set variable value in front-end side
			/// </summary>
			SetVarValue = 6,

			/// <summary>
			/// Call method on front-end side
			/// </summary>
			CallMethod = 7,

			/// <summary>
			/// Get style from front-end side
			/// </summary>
			GetStyle = 8,

			/// <summary>
			/// Set style on front-end side
			/// </summary>
			SetStyle = 9,

			/// <summary>
			/// Remove style on front-end side
			/// </summary>
			RemoveStyle = 9
		}

		#region [ Static Fields ]

		private static Thread _mainTimer;
		private static Queue<string> _requestsQueue;
		private static Dictionary<string, MauElement> _mauElements;
		private static bool _working;
		private static Dictionary<Type, dynamic> _varParsers;

		#endregion

		#region [ Public Static Props ]

		public static WebSocketServer WebSocket { get; private set; }
		public static int Port { get; private set; }
		public static bool Init { get; private set; }

		#endregion

		#region [ Methods ]

		private static void InitVarParser()
		{
			_varParsers = typeof(MyAngularUi).Assembly.GetTypes()
				.Where(t => t.IsClass && t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(MauDataParser<>))
				.Select(val => (dynamic)Activator.CreateInstance(val))
				.ToDictionary(key => (Type)key.TargetType, val => val);
		}
		public static JToken ParseMauData(Type varType, object varObj)
		{
			dynamic parser = !_varParsers.ContainsKey(varType)
				? _varParsers[typeof(object)]
				: _varParsers[varType];

			return parser.Parse((dynamic)varObj);
		}
		public static JToken ParseMauData<T>(T var)
		{
			return ParseMauData(typeof(T), var);
		}

		public static void Setup(int webSocketPort)
		{
			if (Init)
				return;

			Init = true;
			Port = webSocketPort;

			_mauElements = new Dictionary<string, MauElement>();
			_requestsQueue = new Queue<string>();
			_mainTimer = new Thread(async () =>
			{
				while (_working)
				{
					await TimerHandler();
					await Task.Delay(8);
				}
			});

			InitVarParser();
		}
		public static void Stop()
		{
			Init = false;
			WebSocket?.Stop();
		}
		public static Task<bool> Start()
		{
			return Task.Run(() =>
			{
				if (!Init)
					return false;

				if (WebSocket != null)
					return WebSocket.IsListening;

				WebSocket = new WebSocketServer(Port);
				WebSocket.AddWebSocketService<MauSockHandler>("/UiHandler");
				WebSocket.Start();

				_working = true;
				_mainTimer.Start();

				return WebSocket.IsListening;
			});
		}

		internal static Task<bool> Send(string dataToSend)
		{
			return Task.Run(() =>
			{
				if (string.IsNullOrWhiteSpace(dataToSend))
					return false;

				if (!Init)
					throw new NullReferenceException("Call 'Setup` Function First.");

				if (MauSockHandler.Instance == null)
					return false;

				bool sendState = MauSockHandler.Instance.Send(dataToSend);
				return sendState;
			});
		}
		internal static async Task<bool> Send(string mauElementId, RequestType requestType, JObject data)
		{
			var dSend = new JObject
			{
				{ "requestType", (int)requestType },
				{ "mauElementId", mauElementId },
				{ "data", data }
			};

			string dataToSend = dSend.ToString(Formatting.None);
			bool sendState = await Send(dataToSend);

			if (sendState)
				Debug.WriteLine($"Send > {dataToSend}");

			// Queue this request to send when connect again
			if (!sendState && !string.IsNullOrWhiteSpace(dataToSend))
				_requestsQueue.Enqueue(dataToSend);

			Debug.WriteLine("===============");
			return sendState;
		}
		internal static Task<bool> SendRequest(string mauElementId, RequestType requestType, JObject data)
		{
			return Send(mauElementId, requestType, data ?? new JObject());
		}
		internal static Task<bool> SendTsCode(string mauElementId, string code)
		{
			var data = new JObject()
			{
				{ "code", code }
			};

			return SendRequest(mauElementId, RequestType.ExecuteCode, data);
		}
		internal static async Task OnMessage(MessageEventArgs e)
		{
			// Decode json
			JObject jsonRequest = JObject.Parse(e.Data);
			var jsonData = jsonRequest["data"].Value<JObject>();

			// Get request info
			string uiId = jsonRequest["mauElementId"].Value<string>();
			var requestType = (RequestType)jsonRequest["requestType"].Value<int>();

			// Check if ui is registered
			if (!GetUiElement(uiId, out MauElement uiElement))
			{
				// //////////////////////////////////////////////////////////////// Remove comment prefex when finish debug
				// throw new KeyNotFoundException("UiElement not found.");
				return;
			}
			Debug.WriteLine($"Recv > {e.Data}");

			// Process
			switch (requestType)
			{
				case RequestType.None:
					break;

				case RequestType.GetEvents:
					var ret = new JObject
					{
						{ "events", new JArray(uiElement.Events) }
					};

					// Send response
					await Send(uiId, requestType, ret);
					return;

				case RequestType.EventCallback:
					string eventName = jsonData["eventName"].Value<string>();
					string eventType = jsonData["eventType"].Value<string>();
					JObject eventData = JObject.Parse(jsonData["data"].Value<string>());

					uiElement.FireEvent(eventName, eventType, eventData);
					return;

				case RequestType.GetPropValue:
					string propName = jsonData["propName"].Value<string>();
					string propValue = jsonData["propValue"].Value<string>();

					uiElement.SetPropValue(propName, propValue);
					return;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion

		#region [ UiHandler ]

		private static async Task TimerHandler()
		{
			// Handle request queue
			if (_requestsQueue.Count > 0)
			{
				string dataToSend = _requestsQueue.Peek();
				if (await Send(dataToSend))
					_requestsQueue.Dequeue();
			}
		}
		public static bool MauRegistered(string mauId)
		{
			return _mauElements.ContainsKey(mauId);
		}
		public static void RegisterUi(MauElement element)
		{
			_mauElements.Add(element.MauId, element);

			// Request value from angular
			foreach ((string propName, PropertyInfo _) in element.HandledProps.Select(x => (x.Key, x.Value)))
				element.GetPropValue(propName);
		}
		public static void RegisterUi(ICollection<MauElement> element)
		{
			foreach (MauElement uiElement in element)
				_mauElements.Add(uiElement.MauId, uiElement);
		}
		public static bool GetUiElement(string elementId, out MauElement element)
		{
			if (_mauElements.ContainsKey(elementId))
			{
				element = _mauElements[elementId];
				return true;
			}

			element = null;
			return false;
		}

		#endregion
	}
}

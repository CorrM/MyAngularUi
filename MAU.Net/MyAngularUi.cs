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
	/// <summary>
	/// MyAngularUi main class
	/// </summary>
	public static class MyAngularUi
	{
		#region [ Types ]

		/// <summary>
		/// Struct contains request information
		/// </summary>
		public struct RequestState
		{
			/// <summary>
			/// Request Id that's used to communicate with front-end side
			/// </summary>
			public int RequestId;

			/// <summary>
			/// Request send successfully
			/// </summary>
			public bool SuccessSend;
		}

		/// <summary>
		/// Struct contains response information
		/// </summary>
		private struct ResponseInfo
		{
			public int RequestId;
			public string MauId;
			public RequestType RequestType;
			public JObject Data;
		}

		/// <summary>
		/// Type of request for MyAngularUi
		/// </summary>
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
			/// Get return of <see cref="CallMethod"/> from front-end side
			/// </summary>
			ReceiveMethod = 8,

			/// <summary>
			/// Set style on front-end side
			/// </summary>
			SetStyle = 9,

			/// <summary>
			/// Remove style on front-end side
			/// </summary>
			RemoveStyle = 10,

			/// <summary>
			/// Set/Add class on front-end side
			/// </summary>
			AddClass = 11,

			/// <summary>
			/// Remove class on front-end side
			/// </summary>
			RemoveClass = 12,

			/// <summary>
			/// Call method in one of `MauAccessibleServices` on front-end side
			/// </summary>
			ServiceMethodCall = 13
		}

		#endregion

		#region [ Private Fields ]

		private static Timer _mainTimer;
		private static Queue<string> _requestsQueue;
		private static Dictionary<string, MauElement> _mauElements;
		private static bool _working;
		private static Dictionary<Type, dynamic> _varParsers;

		#endregion

		#region [ Public Props ]

		public static WebSocketServer WebSocket { get; private set; }
		public static int Port { get; private set; }
		public static bool Init { get; private set; }

		#endregion

		internal static Task<RequestState> ExecuteTsCode(string mauElementId, string code)
		{
			var data = new JObject()
			{
				{ "code", code }
			};

			return SendRequest(mauElementId, RequestType.ExecuteCode, data);
		}

		#region [ Methods ]

		#region [ Parser ]

		private static void InitParsers()
		{
			_varParsers = typeof(MyAngularUi).Assembly.GetTypes()
				.Where(t => t.IsClass && t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(MauDataParser<>))
				.Select(val => (dynamic)Activator.CreateInstance(val))
				.ToDictionary(key => (Type)key.TargetType, val => val);
		}
		internal static JToken ParseMauDataToFrontEnd(Type varType, object varObj)
		{
			dynamic parser = !_varParsers.ContainsKey(varType)
				? _varParsers[typeof(object)]
				: _varParsers[varType];

			return parser.ParseToFrontEnd((dynamic)varObj);
		}
		internal static JToken ParseMauDataToFrontEnd<T>(T var)
		{
			return ParseMauDataToFrontEnd(typeof(T), var);
		}
		internal static object ParseMauDataFromFrontEnd(Type varType, JToken varObj)
		{
			dynamic parser = !_varParsers.ContainsKey(varType)
				? _varParsers[typeof(object)]
				: _varParsers[varType];

			return parser.ParseFromFrontEnd((dynamic)varObj);
		}
		internal static T ParseMauDataFromFrontEnd<T>(JToken var)
		{
			return (T)ParseMauDataFromFrontEnd(typeof(T), var);
		}

		#endregion

		#region [ Mau ]

		/// <summary>
		/// Init MyAngularUi
		/// </summary>
		/// <param name="webSocketPort">Port to use for communication</param>
		public static void Setup(int webSocketPort)
		{
			if (Init)
				return;

			Init = true;
			Port = webSocketPort;

			_mauElements = new Dictionary<string, MauElement>();
			_requestsQueue = new Queue<string>();
			_mainTimer = new Timer(TimerHandler, null, 0, 8);

			InitParsers();
		}

		/// <summary>
		/// Start MyAngularUi
		/// </summary>
		/// <returns>If start correctly return true</returns>
		public static Task<bool> Start()
		{
			return Task.Run(() =>
			{
				if (!Init)
					return false;

				if (WebSocket != null)
					return WebSocket.IsListening;

				WebSocket = new WebSocketServer(Port);
				WebSocket.AddWebSocketService<MauSockHandler>("/MauHandler");
				WebSocket.Start();

				_working = true;

				return WebSocket.IsListening;
			});
		}

		/// <summary>
		/// Stop MyAngularUi
		/// </summary>
		public static void Stop()
		{
			Init = false;
			WebSocket?.Stop();
		}

		/// <summary>
		/// When angular disconnect, this function will sync
		/// all [ Element, Events, Props, Vars ] with angular again
		/// </summary>
		private static async Task ReSyncMauInfo()
		{

		}

		#endregion

		#region [ WebSocket ]

		/// <summary>
		/// Send raw-string to front-end side
		/// </summary>
		/// <param name="dataToSend"></param>
		/// <returns></returns>
		private static Task<bool> Send(string dataToSend)
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

		/// <summary>
		/// Only use when you want to send response to front-end request,
		/// Just set the same requestId from front-end request
		/// </summary>
		/// <param name="requestId">front-end request id</param>
		/// <param name="mauElementId">MauId of your target</param>
		/// <param name="requestType">Type of request</param>
		/// <param name="data">Request data</param>
		/// <returns>Information about request state</returns>
		internal static async Task<RequestState> SendResponse(int requestId, string mauElementId, RequestType requestType, JObject data)
		{
			var dSend = new JObject
			{
				{ "requestId", requestId },
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

			return new RequestState() { RequestId = requestId, SuccessSend = sendState };
		}

		/// <summary>
		/// Use to send new request to front-end side
		/// </summary>
		/// <param name="mauElementId">MauId of your target</param>
		/// <param name="requestType">Type of request</param>
		/// <param name="data">Request Data</param>
		/// <returns></returns>
		internal static Task<RequestState> SendRequest(string mauElementId, RequestType requestType, JObject data)
		{
			int requestId = Utils.RandomInt(1, 100000);
			return SendResponse(requestId, mauElementId, requestType, data ?? new JObject());
		}

		internal static async Task OnMessage(MessageEventArgs e)
		{
			// Decode json
			JObject jsonRequest = JObject.Parse(e.Data);

			// Get request info
			var response = new ResponseInfo()
			{
				RequestId = jsonRequest["requestId"]!.Value<int>(),
				MauId = jsonRequest["mauElementId"]!.Value<string>(),
				RequestType = (RequestType)jsonRequest["requestType"]!.Value<int>(),
				Data = jsonRequest["data"]!.Value<JObject>()
			};

			// Check if MauElement is registered, And get it
			if (!GetMauElement(response.MauId, out MauElement mauElement))
			{
				////////////////////////////////////////////////////////////////////////// Remove when finish Debug
				// throw new KeyNotFoundException("MauElement not found.");
				return;
			}

			Debug.WriteLine($"Recv > {e.Data}");

			// Process
			switch (response.RequestType)
			{
				case RequestType.None:
					break;

				case RequestType.GetEvents:
					var ret = new JObject
					{
						{ "events", new JArray(mauElement.Events) }
					};

					// Send response
					await SendRequest(response.MauId, response.RequestType, ret);
					break;

				case RequestType.EventCallback:
					string eventName = response.Data["eventName"]!.Value<string>();
					string eventType = response.Data["eventType"]!.Value<string>();
					JObject eventData = JObject.Parse(response.Data["data"]!.Value<string>());

					mauElement.FireEvent(eventName, eventType, eventData);
					break;

				case RequestType.GetPropValue:
					string propName = response.Data["propName"]!.Value<string>();
					JToken propValType = response.Data["propValue"];

					mauElement.SetPropValue(propName, propValType);
					break;

				case RequestType.ReceiveMethod:
					string methodName = response.Data["methodName"]!.Value<string>();
					JToken methodRet = response.Data["methodRet"];

					mauElement.SetMethodRetValue(response.RequestId, methodName, methodRet);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		internal static async Task OnOpen()
		{
			await ReSyncMauInfo();
		}
		internal static async Task OnClose(CloseEventArgs e)
		{
			await Task.Delay(0);
		}
		#endregion

		#region [ Helper ]

		private static async void TimerHandler(object o)
		{
			if (!_working)
				return;

			// Handle request queue
			if (_requestsQueue.Count > 0)
			{
				string dataToSend = _requestsQueue.Peek();
				if (await Send(dataToSend))
					_requestsQueue.Dequeue();
			}
		}
		internal static bool IsMauRegistered(string mauId)
		{
			return _mauElements.ContainsKey(mauId);
		}
		internal static void RegisterMau(MauElement element)
		{
			_mauElements.Add(element.MauId, element);

			// Request value from angular
			foreach ((string propName, PropertyInfo _) in element.HandledProps.Select(x => (x.Key, x.Value)))
				element.GetPropValue(propName);
		}
		internal static void RegisterMau(IEnumerable<MauElement> element)
		{
			foreach (MauElement uiElement in element)
				_mauElements.Add(uiElement.MauId, uiElement);
		}
		internal static bool GetMauElement(string elementId, out MauElement element)
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

		#endregion
	}
}

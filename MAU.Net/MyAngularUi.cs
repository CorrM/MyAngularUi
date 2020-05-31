using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MAU.Attributes;
using Newtonsoft.Json;
using MAU.Core;
using MAU.DataParser;
using MAU.Helper;
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
			ServiceMethodCall = 13,

			/// <summary>
			/// Tell angular side all elements are ready [Props, Vars and etc]
			/// </summary>
			DotNetReady = 14
		}

		#endregion

		#region [ Private Fields ]

		private static Dictionary<string, MauElement> _mauElements;
		private static Dictionary<Type, dynamic> _varParsers;

		#endregion

		#region [ Internal Props ]

		internal static ConcurrentDictionary<int, object> OrdersResponse { get; private set; }
		internal static List<MauComponent> RegisteredComponents { get; private set; }

		#endregion

		#region [ Public Props ]

		public static bool Connected { get; private set; }
		public static MauWebSocket WebSocket { get; private set; }
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
			OrdersResponse = new ConcurrentDictionary<int, object>();
			RegisteredComponents = new List<MauComponent>();

			InitParsers();
		}

		/// <summary>
		/// Start MyAngularUi
		/// </summary>
		/// <returns>If start correctly return true</returns>
		public static bool Start()
		{
			if (!Init)
				return false;

			WebSocket = new MauWebSocket(Port);
			WebSocket.OnOpen += () => Task.Run(OnOpen);
			WebSocket.OnClose += () => Task.Run(OnClose);
			WebSocket.OnMessage += (message) => Task.Run(() => OnMessage(message));
			WebSocket.Start();

			return true;
		}

		/// <summary>
		/// Stop MyAngularUi
		/// </summary>
		public static void Stop()
		{
			WebSocket?.Dispose();
		}

		/// <summary>
		/// When angular disconnect, this function will sync
		/// all [ Element, Events, Props, Vars ] with angular again
		/// </summary>
		private static async Task ReSyncMauElements()
		{
			foreach ((string _, MauElement mauElement) in _mauElements)
			{
				// Vars [ Must be first ]
				foreach ((string varName, PropertyInfo _) in mauElement.HandledVars)
					await MauVariable.SendMauVariable(mauElement, varName);

				// Props
				foreach ((string propName, BoolHolder<PropertyInfo> _) in mauElement.GetValidToSetHandledProps())
					await MauProperty.SendMauProp(mauElement, propName);
			}

			if (_mauElements.Count > 0)
				await SendRequest(string.Empty, RequestType.DotNetReady, null);
		}

		#endregion

		#region [ WebSocket ]

		/// <summary>
		/// Send raw-string to front-end side
		/// </summary>
		/// <param name="dataToSend"></param>
		/// <returns></returns>
		private static async Task<bool> Send(string dataToSend)
		{
			if (!Init)
				throw new NullReferenceException("Call 'Setup` Function First.");

			if (string.IsNullOrWhiteSpace(dataToSend) || !Connected)
				return false;

			return await WebSocket.Send(dataToSend);
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
			{
				Debug.WriteLine($"Send > {dataToSend}");
				Debug.WriteLine("===============");
			}

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

		internal static async Task OnMessage(string message)
		{
			// Decode json
			JObject jsonRequest = JObject.Parse(message);
			Debug.WriteLine($"Recv > {message}");

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
		internal static void OnOpen()
		{
			Connected = true;
			ReSyncMauElements().GetAwaiter().GetResult();
		}
		internal static void OnClose()
		{
			Connected = false;
		}

		#endregion

		#region [ Helper ]

		internal static bool IsElementRegistered(string mauId)
		{
			return _mauElements.ContainsKey(mauId);
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
		public static IReadOnlyCollection<MauComponent> GetAllComponents()
		{
			return RegisteredComponents.AsReadOnly();
		}
		public static void RegisterComponents(MauComponent mauComponent)
		{
			RegisteredComponents.Add(mauComponent);
		}

		internal static async Task RegisterElement(MauElement mauElement)
		{
			_mauElements.Add(mauElement.MauId, mauElement);

			// Request value from angular side
			foreach ((string propName, BoolHolder<PropertyInfo> _) in mauElement.GetValidToSetHandledProps())
				await mauElement.GetPropValue(propName);
		}
		internal static async Task RegisterElement(IEnumerable<MauElement> element)
		{
			foreach (MauElement uiElement in element)
				await RegisterElement(uiElement);
		}

		#endregion

		#endregion
	}
}

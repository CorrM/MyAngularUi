using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			/// Tell angular what events handled in dotnet
			/// </summary>
			SetEvents = 1,

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
			/// Tell angular side all components are ready [Props, Vars and etc]
			/// </summary>
			DotNetReady = 14,

			/// <summary>
			/// Get/Send data from/to angular
			/// </summary>
			CustomData = 15
		}

		#endregion

		#region [ Private Fields ]

		private static ConcurrentDictionary<string, MauComponent> _mauComponents;
		private static Dictionary<Type, dynamic> _varParsers;

		#endregion

		#region [ Internal Props ]

		internal static Assembly AppAssembly { get; private set; }
		internal static ConcurrentDictionary<int, object> OrdersResponse { get; private set; }
		internal static Dictionary<string, object> MauContainers { get; private set; }

		#endregion

		#region [ Public Props ]

		public delegate void CustomData(JObject data);
		public static event CustomData OnCustomData;

		public static bool Connected { get; private set; }
		public static MauWebSocket WebSocket { get; private set; }
		public static int Port { get; private set; }
		public static bool Init { get; private set; }

		#endregion

		internal static RequestState ExecuteTsCode(string mauComponentId, string code)
		{
			var data = new JObject()
			{
				{ "code", code }
			};

			return SendRequest(mauComponentId, RequestType.ExecuteCode, data);
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
				? _varParsers[typeof(object)] // MauDefaultParser
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
		public static void Setup(Assembly assembly)
		{
			if (Init)
				return;
			Init = true;

			AppAssembly = assembly;
			_mauComponents = new ConcurrentDictionary<string, MauComponent>();
			OrdersResponse = new ConcurrentDictionary<int, object>();
			MauContainers = new Dictionary<string, object>();

			InitParsers();
			BootStrapMau();
		}

		/// <summary>
		/// Start MyAngularUi
		/// </summary>
		/// <returns>If start correctly return true</returns>
		public static async Task Start(int webSocketPort = 2911)
		{
			if (!Init)
				return;

			Port = webSocketPort;
			WebSocket = new MauWebSocket(Port);
			WebSocket.OnOpen += () => Task.Run(OnOpen);
			WebSocket.OnClose += () => Task.Run(OnClose);
			WebSocket.OnMessage += (message) => Task.Run(() => OnMessage(message));
			WebSocket.Start();

			while (true)
				await Task.Delay(8);
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
		/// all [ Components, Events, Props, Vars ] with angular again
		/// </summary>
		private static void ReSyncMauComponents()
		{
			foreach ((string _, MauComponent mauComponent) in _mauComponents)
			{
				// Vars [ Must be first ]
				foreach ((string varName, PropertyInfo _) in mauComponent.HandledVars)
					MauVariable.SendMauVariable(mauComponent, varName);

				// Props
				foreach ((string propName, BoolHolder<PropertyInfo> _) in mauComponent.GetValidToSetHandledProps())
					MauProperty.SendMauProp(mauComponent, propName);

				// Events
				MauEvent.SendMauEvents(mauComponent);
			}

			if (_mauComponents.Count > 0)
				SendRequest(string.Empty, RequestType.DotNetReady);
		}

		#endregion

		#region [ WebSocket ]

		/// <summary>
		/// Send raw-string to front-end side
		/// </summary>
		/// <param name="dataToSend"></param>
		/// <returns></returns>
		private static bool Send(string dataToSend)
		{
			if (!Init)
				throw new NullReferenceException("Call 'Setup` Function First.");

			if (string.IsNullOrWhiteSpace(dataToSend) || !Connected)
				return false;

			return WebSocket.Send(dataToSend).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Only use when you want to send response to front-end request,
		/// Just set the same requestId from front-end request
		/// </summary>
		/// <param name="requestId">front-end request id</param>
		/// <param name="mauComponentId">MauId of your target</param>
		/// <param name="requestType">Type of request</param>
		/// <param name="data">Request data</param>
		/// <returns>Information about request state</returns>
		internal static RequestState SendResponse(int requestId, string mauComponentId, RequestType requestType, JObject data)
		{
			var dSend = new JObject
			{
				{ "requestId", requestId },
				{ "requestType", (int)requestType },
				{ "mauComponentId", mauComponentId },
				{ "data", data }
			};

			string dataToSend = dSend.ToString(Formatting.None);
			bool sendState = Send(dataToSend);

			return new RequestState() { RequestId = requestId, SuccessSend = sendState };
		}

		/// <summary>
		/// Use to send new request to front-end side
		/// </summary>
		/// <param name="mauComponentId">MauId of your target</param>
		/// <param name="requestType">Type of request</param>
		/// <param name="data">Request Data</param>
		/// <returns></returns>
		internal static RequestState SendRequest(string mauComponentId, RequestType requestType, JObject data)
		{
			int requestId = Utils.RandomInt(1, 100000);
			return SendResponse(requestId, mauComponentId, requestType, data ?? new JObject());
		}
		internal static RequestState SendRequest(string mauComponentId, RequestType requestType)
		{
			return SendRequest(mauComponentId, requestType, null);
		}

		public static bool SendCustomData(JObject data)
		{
			if (!WebSocket.IsConnected())
				return false;

			return SendRequest(null, RequestType.CustomData, data).SuccessSend;
		}
		internal static void OnMessage(string message)
		{
			// Decode json
			JObject jsonRequest = JObject.Parse(message);

			// Get request info
			var response = new ResponseInfo()
			{
				RequestId = jsonRequest["requestId"]!.Value<int>(),
				MauId = jsonRequest["mauComponentId"]!.Value<string>(),
				RequestType = (RequestType)jsonRequest["requestType"]!.Value<int>(),
				Data = jsonRequest["data"]!.Value<JObject>()
			};

			// ! for request not need [MauComponent], ex: just need 'RequestType' and 'Data'.
			switch (response.RequestType)
			{
				case RequestType.CustomData:
					Task.Run(() => OnCustomData?.Invoke(response.Data));
					break;
			}

			// Check if MauComponent is registered, And get it
			if (!GetMauComponent(response.MauId, out MauComponent mauComponent))
				return;

			// ! for request need [MauComponent].
			switch (response.RequestType)
			{
				case RequestType.EventCallback:
					string eventName = response.Data["eventName"]!.Value<string>();
					string eventType = response.Data["eventType"]!.Value<string>();
					JObject eventData = JObject.Parse(response.Data["data"]!.Value<string>());

					mauComponent.FireEvent(eventName, eventType, eventData);
					break;

				case RequestType.GetPropValue:
					string propName = response.Data["propName"]!.Value<string>();
					JToken propValType = response.Data["propValue"];

					mauComponent.SetPropValue(propName, propValType);
					break;

				case RequestType.ReceiveMethod:
					string methodName = response.Data["methodName"]!.Value<string>();
					JToken methodRet = response.Data["methodRet"];

					mauComponent.SetMethodRetValue(response.RequestId, methodName, methodRet);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		internal static void OnOpen()
		{
			Connected = true;
			ReSyncMauComponents();
		}
		internal static void OnClose()
		{
			Connected = false;
		}

		#endregion

		#region [ Helper ]

		internal static bool IsComponentRegistered(string mauComponentId)
		{
			return _mauComponents.ContainsKey(mauComponentId);
		}
		internal static bool GetMauComponent(string mauComponentId, out MauComponent component)
		{
			if (_mauComponents.ContainsKey(mauComponentId))
			{
				component = _mauComponents[mauComponentId];
				return true;
			}

			component = null;
			return false;
		}
		internal static void RegisterComponent(MauComponent mauComponent)
		{
			if (string.IsNullOrWhiteSpace(mauComponent.MauId))
				return;

			// Register itself
			if (MyAngularUi.IsComponentRegistered(mauComponent.MauId))
				throw new Exception("MauComponent with same mauId was registered.");

			_mauComponents.TryAdd(mauComponent.MauId, mauComponent);

			// Request value from angular side
			foreach ((string propName, BoolHolder<PropertyInfo> _) in mauComponent.GetValidToSetHandledProps())
				mauComponent.GetPropValue(propName);
		}
		private static void BootStrapMau()
		{
			// Create instance of all 'MauParentComponent'
			foreach (Type item in AppAssembly.GetTypes().Where(t => !t.IsAbstract && MauContainer.HasAttribute(t)))
			{
				if (!item.IsSealed)
					throw new Exception($"MauContainer must be 'Sealed', '{item.FullName}'.");

				if (!MauContainers.TryAdd(item.FullName, Activator.CreateInstance(item)))
					throw new Exception($"Can't register MauContainer '{item.FullName}'.");
			}
		}
		internal static IReadOnlyDictionary<string, MauComponent> GetAllComponents()
		{
			return new ReadOnlyDictionary<string, MauComponent>(_mauComponents);
		}
		public static T GetMauContainer<T>() where T : class
		{
			Type t = typeof(T);
			if (!MauContainer.HasAttribute(t))
				throw new Exception($"'{t.FullName}' not a MauContainer.");

			string compName = typeof(T).FullName;
			if (!MauContainers.ContainsKey(compName))
				return null;

			return (T)MauContainers[compName];
		}

		#endregion

		#endregion
	}
}

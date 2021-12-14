using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MAU.Attributes;
using MAU.Core;
using MAU.DataParser;
using MAU.Helper;
using MAU.Helper.Holders;
using MAU.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//[assembly: MauException]
namespace MAU;

/// <summary>
/// MyAngularUi main class
/// </summary>
public static class MyAngularUi
{
    #region [ Types ]

    /// <summary>
    /// Struct contains request information
    /// </summary>
    public readonly struct RequestState
    {
        /// <summary>
        /// Request Id that's used to communicate with front-end side
        /// </summary>
        public int RequestId { get; }

        /// <summary>
        /// Request send successfully
        /// </summary>
        public bool SuccessSent { get; }

        public RequestState(int requestId, bool successSent)
        {
            RequestId = requestId;
            SuccessSent = successSent;
        }
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
    internal enum RequestType
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
        /// Tell angular side all components are ready [Props, Vars and etc]
        /// </summary>
        DotNetReady = 13,

        /// <summary>
        /// Get/Send data from/to angular
        /// </summary>
        CustomData = 14,

        /// <summary>
        /// Call method on .Net side from angular side
        /// </summary>
        CallNetMethod = 15
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

    #region [ Public Events ]

    public delegate ValueTask CustomData(string id, JObject data);

    public static event CustomData OnCustomData;
    public static event Action<Exception> OnUnhandledException;

    #endregion

    #region [ Public Props ]

    private static MauWebSocket WebSocket { get; set; }
    public static int Port { get; private set; }
    public static bool IsConnected { get; private set; }
    public static bool IsInit { get; private set; }

    #endregion

    /*private static Task<RequestState> ExecuteTsCodeAsync(string mauComponentId, string code)
    {
        var data = new JObject()
        {
            { "code", code }
        };

        return SendRequestAsync(mauComponentId, RequestType.ExecuteCode, data);
    }*/

    #region [ Methods ]

    #region [ Parser ]

    private static void InitParsers()
    {
        _varParsers = typeof(MyAngularUi).Assembly.GetTypes()
            .Where(t => t.IsClass && t.BaseType?.IsGenericType == true && t.BaseType.GetGenericTypeDefinition() == typeof(MauDataParser<>))
            .Select(val => (dynamic)Activator.CreateInstance(val))
            .ToDictionary(key => (Type)key.TargetType, val => val);
    }
    internal static JToken ParseMauDataToFrontEnd(Type varType, object varObj)
    {
        dynamic parser = !_varParsers.ContainsKey(varType)
            ? _varParsers[typeof(object)]
            : _varParsers[varType];

        return parser.ParseToFrontEnd(varType, (dynamic)varObj);
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
    /// <param name="assembly"></param>
    public static void Init(Assembly assembly)
    {
        if (IsInit)
            return;
        IsInit = true;

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
    public static async Task StartAsync(int webSocketPort = 2911)
    {
        if (!IsInit)
            return;

        Port = webSocketPort;
        WebSocket = new MauWebSocket(Port);
        WebSocket.OnOpen += () => Task.Run(OpenCallback);
        WebSocket.OnClose += () => Task.Run(CloseCallback);
        WebSocket.OnMessage += (message) => Task.Run(() => OnMessage(message));
        WebSocket.Start();

        while (true)
            await Task.Delay(8).ConfigureAwait(false);
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
        foreach ((_, MauComponent mauComponent) in _mauComponents)
        {
            // Vars [ Must be first ]
            foreach ((string varName, PropertyInfo _) in mauComponent.HandledVars)
                MauVariable.SendMauVariable(mauComponent, varName);

            // Props
            Dictionary<string, MauPropertyHolder> props = mauComponent.GetValidToSetHandledProps();
            foreach ((string propName, _) in props)
                MauProperty.SendMauProp(mauComponent, propName);

            // Events
            MauEventAttribute.SendMauEventsAsync(mauComponent);
        }

        if (_mauComponents.Count > 0)
            SendRequest(string.Empty, RequestType.DotNetReady);
    }

    #endregion

    #region [ WebSocket ]

    /// <summary>
    /// Send raw-string to front-end side
    /// </summary>
    /// <param name="dataToSend">Data to send</param>
    private static async Task<bool> SendAsync(string dataToSend)
    {
        if (!IsInit)
            throw new NullReferenceException("Call 'Setup` Function First.");

        if (string.IsNullOrWhiteSpace(dataToSend) || !IsConnected)
            return false;

#if DEBUG
        // Debug.WriteLine($"SEND > {dataToSend}");
#endif

        return await WebSocket.SendAsync(dataToSend).ConfigureAwait(false);
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
    internal static async Task<RequestState> SendResponseAsync(int requestId, string mauComponentId, RequestType requestType, JObject data)
    {
        var dSend = new JObject
        {
            { "requestId", requestId },
            { "requestType", (int)requestType },
            { "mauComponentId", mauComponentId },
            { "data", data }
        };

        string dataToSend = dSend.ToString(Formatting.None);
        bool sendState = await SendAsync(dataToSend).ConfigureAwait(false);

        return new RequestState(requestId, sendState);
    }

    /// <summary>
    /// Use to send new request to front-end side
    /// </summary>
    /// <param name="mauComponentId">MauId of your target</param>
    /// <param name="requestType">Type of request</param>
    /// <param name="data">Request Data</param>
    internal static Task<RequestState> SendRequestAsync(string mauComponentId, RequestType requestType, JObject data)
    {
        int requestId = Utils.RandomInt(1, 100000);
        return SendResponseAsync(requestId, mauComponentId, requestType, data ?? new JObject());
    }

    internal static void SendRequest(string mauComponentId, RequestType requestType)
    {
        SendRequestAsync(mauComponentId, requestType, null);
    }

    public static async Task<RequestState> SendCustomDataAsync(string id, JObject data)
    {
        if (!WebSocket.IsConnected())
            return default;

        var dataToSend = new JObject
        {
            { "id", id },
            { "data", data }
        };

        return await SendRequestAsync(null, RequestType.CustomData, dataToSend).ConfigureAwait(false);
    }

    internal static void OnMessage(string message)
    {
#if DEBUG
        // Debug.WriteLine($"RECV > {message}");
#endif
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
        if (response.RequestType == RequestType.CustomData)
        {
            _ = Task.Run(async () =>
            {
                if (OnCustomData != null)
                    await OnCustomData.Invoke(response.Data["id"]!.Value<string>(), response.Data["data"]!.Value<JObject>()).ConfigureAwait(false);
            });
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

            case RequestType.CallNetMethod:
                string methodCallerName = response.Data["methodName"]!.Value<string>();
                JToken methodArgs = response.Data["methodArgs"];

                List<object> argsList = methodArgs!.Select(arrItem => ParseMauDataFromFrontEnd(typeof(object), arrItem)).ToList();

                Task.Run(async () =>
                {
                    object ret = mauComponent.CallMethod(methodCallerName, argsList);
                    await SendResponseAsync(response.RequestId, response.MauId, RequestType.CallNetMethod, new JObject
                    {
                        { "methodName", methodCallerName },
                        { "methodRet", ParseMauDataToFrontEnd(ret) }
                    }).ConfigureAwait(false);
                });
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal static void OpenCallback()
    {
        IsConnected = true;
        ReSyncMauComponents();
    }

    internal static void CloseCallback()
    {
        IsConnected = false;
    }

    #endregion

    #region [ Helper ]

    internal static void RaiseException(Exception ex)
    {
        OnUnhandledException?.Invoke(ex);
    }

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
        foreach ((string propName, MauPropertyHolder _) in mauComponent.GetValidToSetHandledProps())
            mauComponent.RequestPropValue(propName);
    }

    private static void BootStrapMau()
    {
        // Create instance of all 'MauParentComponent'
        foreach (Type item in AppAssembly.GetTypes().Where(t => !t.IsAbstract && MauContainerAttribute.HasAttribute(t)))
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
        if (!MauContainerAttribute.HasAttribute(t))
            throw new Exception($"'{t.FullName}' not a MauContainer.");

        string compName = typeof(T).FullName;
        if (!MauContainers.ContainsKey(compName!))
            return null;

        return (T)MauContainers[compName];
    }

    #endregion

    #endregion
}
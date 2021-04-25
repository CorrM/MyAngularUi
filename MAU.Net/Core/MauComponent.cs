using MAU.Attributes;
using MAU.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MAU.Helper.Holders;
using static MAU.Events.MauEventHandlers;

namespace MAU.Core
{
    public abstract class MauComponent
    {

        internal readonly Dictionary<string, EventInfo> HandledEvents;
        internal readonly Dictionary<string, MauPropertyHolder> HandledProps;
        internal readonly Dictionary<string, PropertyInfo> HandledVars;
        internal readonly Dictionary<string, MethodInfo> HandledMethods;

        #region [ Public Proparties ]

        public IReadOnlyCollection<string> MauEvents => HandledEvents.Keys.ToList().AsReadOnly();
        public IReadOnlyCollection<string> MauProps => HandledProps.Keys.ToList().AsReadOnly();
        public IReadOnlyCollection<string> MauVars => HandledVars.Keys.ToList().AsReadOnly();
        public IReadOnlyCollection<string> MauMethods => HandledMethods.Keys.ToList().AsReadOnly();
        public string MauId { get; }

        #endregion

        #region [ UI Proparties ]

        //[MauProperty("innerText", MauPropertyType.NativeProperty)]
        //public string InnerText { get; set; }

        //[MauProperty("innerHTML", MauPropertyType.NativeProperty)]
        //public string InnerHtml { get; set; }

        //[MauProperty("textContent", MauPropertyType.NativeProperty)]
        //public string TextContent { get; set; }

        [MauProperty("style", MauProperty.MauPropertyType.NativeProperty, PropStatus = MauProperty.MauPropertyStatus.ReadOnly)]
        public string Style { get; internal set; }

        [MauProperty("className", MauProperty.MauPropertyType.NativeProperty, PropStatus = MauProperty.MauPropertyStatus.ReadOnly)]
        public string ClassName { get; internal set; }

        [MauProperty("id", MauProperty.MauPropertyType.NativeProperty, PropStatus = MauProperty.MauPropertyStatus.ReadOnly)]
        public string Id { get; internal set; }

        #endregion

        #region [ UI Events ]

        [MauEvent("click")]
        public event MauEventHandlerAsync OnClick;

        [MauEvent("dblclick")]
        public event MauEventHandlerAsync OnDoubleClick;

        #endregion

        #region [ UI Methods ]

        public void SetStyle(string styleName, string styleValue, string childQuerySelector = "")
        {
            var data = new JObject
            {
                {"styleName", styleName},
                {"styleValue", styleValue},
                {"childQuerySelector", childQuerySelector},
            };

            MyAngularUi.SendRequestAsync(MauId, MyAngularUi.RequestType.SetStyle, data);
        }
        public void RemoveStyle(string styleName)
        {
            var data = new JObject
            {
                {"styleName", styleName}
            };

            MyAngularUi.SendRequestAsync(MauId, MyAngularUi.RequestType.RemoveStyle, data);
        }

        public void AddClass(string className)
        {
            var data = new JObject
            {
                {"className", className}
            };

            MyAngularUi.SendRequestAsync(MauId, MyAngularUi.RequestType.AddClass, data);
        }
        public void RemoveClass(string className)
        {
            var data = new JObject
            {
                {"className", className}
            };

            MyAngularUi.SendRequestAsync(MauId, MyAngularUi.RequestType.RemoveClass, data);
        }

        #endregion

        internal MauPropertyHolder GetMauPropHolder(string propName)
        {
            return HandledProps.ContainsKey(propName)
                ? HandledProps[propName]
                : null;
        }
        internal MauMethod GetMauMethodAttribute(string methodName)
        {
            return HandledMethods.ContainsKey(methodName)
                ? HandledMethods[methodName].GetCustomAttribute<MauMethod>()
                : null;
        }
        internal Type GetPropType(string propName)
        {
            return HandledProps.ContainsKey(propName)
                ? HandledProps[propName].Holder.PropertyType
                : null;
        }
        internal Type GetMethodReturnType(string methodName)
        {
            return HandledMethods.ContainsKey(methodName)
                ? HandledMethods[methodName].ReturnType
                : null;
        }

        protected MauComponent(string mauId)
        {
            if (MyAngularUi.IsComponentRegistered(mauId))
                throw new ArgumentOutOfRangeException(nameof(mauId), "MauComponent with same mauId was registered.");

            MauId = mauId;
            HandledEvents = new Dictionary<string, EventInfo>();
            HandledProps = new Dictionary<string, MauPropertyHolder>();
            HandledVars = new Dictionary<string, PropertyInfo>();
            HandledMethods = new Dictionary<string, MethodInfo>();

            Init();
        }

        private void Init()
        {
            // Events
            {
                EventInfo[] eventInfos = this.GetType().GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (EventInfo eventInfo in eventInfos.Where(MauEventAttribute.HasAttribute))
                {
                    var attr = eventInfo.GetCustomAttribute<MauEventAttribute>();
                    HandledEvents.Add(attr.EventName, eventInfo);
                }
            }

            // Properties
            {
                PropertyInfo[] propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo propertyInfo in propertyInfos.Where(MauProperty.HasAttribute))
                {
                    var attr = propertyInfo.GetCustomAttribute<MauProperty>();
                    HandledProps.Add(attr.PropertyName, new MauPropertyHolder(propertyInfo, true));
                }
            }

            // Vars
            {
                PropertyInfo[] varInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (PropertyInfo varInfo in varInfos.Where(MauVariable.HasAttribute))
                    HandledVars.Add(varInfo.Name, varInfo);
            }

            // Methods
            {
                MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (MethodInfo methodInfo in methodInfos.Where(MauMethod.HasAttribute))
                {
                    var attr = methodInfo.GetCustomAttribute<MauMethod>();
                    HandledMethods.Add(attr.MethodName, methodInfo);
                }
            }

            MyAngularUi.RegisterComponent(this);
        }

        internal void FireEvent(string eventName, string eventType, JObject eventData)
        {
            if (!HandledEvents.ContainsKey(eventName))
                return;

            string netEventName = HandledEvents[eventName].Name;
            Type t = this.GetType();
            FieldInfo fi = null;

            // Search for that event
            while (t != null)
            {
                fi = t.GetField(netEventName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fi != null)
                    break;
                t = t.BaseType;
            }

            if (fi == null)
                return;

            // Get event
            var eventDelegate = (MulticastDelegate)fi.GetValue(this);

            // There any subscriber .?
            if (eventDelegate is null)
                return;

            // Invoke all subscribers
            foreach (Delegate handler in eventDelegate.GetInvocationList())
            {
                // https://stackoverflow.com/questions/20350397/how-can-i-tell-if-a-c-sharp-method-is-async-await-via-reflection
                // Fire & Forget
                Task.Run(async () =>
                {
                    try
                    {
                        object[] param = { this, new MauEventInfo(eventName, eventType, eventData) };
                        await ((ValueTask)handler.Method.Invoke(handler.Target, param)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        MyAngularUi.RaiseException(ex);
                    }
                });
            }
        }
        internal object CallMethod(string methodCallerName, List<object> methodArgs)
        {
            if (!HandledMethods.ContainsKey(methodCallerName))
                return null;

            try
            {
                return HandledMethods[methodCallerName].Invoke(this, methodArgs.ToArray());
            }
            catch (Exception ex)
            {
                MyAngularUi.RaiseException(ex);
            }

            return null;
        }
        internal Dictionary<string, MauPropertyHolder> GetValidToSetHandledProps()
        {
            return this.HandledProps
                //.Where(x => this.GetMauPropAttribute(x.Key).PropStatus != MauProperty.MauPropertyStatus.ReadOnly)
                .OrderBy(x => this.GetMauPropHolder(x.Key).PropAttr.Important)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Send request to angular side to send property value via <see cref="MyAngularUi.OnMessage"/>
        /// </summary>
        /// <param name="propName">Property name</param>
        internal void RequestPropValue(string propName)
        {
            if (!HandledProps.ContainsKey(propName))
                return;

            MauPropertyHolder mauProperty = GetMauPropHolder(propName);
            var data = new JObject
            {
                {"propName", propName},
                {"propType", (int)mauProperty.PropAttr.PropType},
                {"propStatus", (int)mauProperty.PropAttr.PropStatus}, // Needed by `SetPropHandler`
                {"propForce", mauProperty.PropAttr.ForceSet} // Needed by `SetPropHandler`
			};

            MyAngularUi.SendRequestAsync(MauId, MyAngularUi.RequestType.GetPropValue, data);
        }
        internal void SetPropValue(string propName, JToken propValueJson)
        {
            if (!HandledProps.ContainsKey(propName))
                return;

            Type propValType = GetPropType(propName);
            object propValue = MyAngularUi.ParseMauDataFromFrontEnd(propValType, propValueJson);

            // Make valid enum value
            MauEnumMemberAttribute.GetValidEnumValue(propValType, ref propValue);

            // Deadlock will not happen because of 'HandledProps[propName].HandleOnSet'
            lock (HandledProps[propName])
                HandledProps[propName].DoWithoutHandling(p => p.Holder.SetValue(this, propValue));
        }

        internal void SetMethodRetValue(int callMethodRequestId, string methodName, JToken methodRetValueJson)
        {
            if (!HandledMethods.ContainsKey(methodName))
                return;

            Type methodRetType = GetMethodReturnType(methodName);
            object methodRet = MyAngularUi.ParseMauDataFromFrontEnd(methodRetType, methodRetValueJson);

            // Make valid enum value
            MauEnumMemberAttribute.GetValidEnumValue(methodRetType, ref methodRet);

            MyAngularUi.OrdersResponse.TryAdd(callMethodRequestId, methodRet);
        }
        internal object GetMethodRetValue(int callMethodRequestId)
        {
            while (!MyAngularUi.OrdersResponse.ContainsKey(callMethodRequestId) && MyAngularUi.IsConnected)
                Thread.Sleep(1);

            if (!MyAngularUi.IsConnected)
                return null;

            // Get and remove data
            MyAngularUi.OrdersResponse.TryRemove(callMethodRequestId, out object ret);

            return ret;
        }
    }
}

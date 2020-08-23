using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MAU.Attributes;
using MAU.Events;
using MAU.Helper;
using Newtonsoft.Json.Linq;
using static MAU.Events.MauEventHandlers;

namespace MAU.Core
{
	public abstract class MauComponent
	{
		#region [ Internal Props ]

		#endregion

		#region [ Internal Fields ]

		internal readonly Dictionary<string, EventInfo> HandledEvents;
		internal readonly Dictionary<string, BoolHolder<PropertyInfo>> HandledProps; // bool for handleOnSet
		internal readonly Dictionary<string, PropertyInfo> HandledVars;
		internal readonly Dictionary<string, MethodInfo> HandledMethods;

		#endregion

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

		#endregion

		#region [ UI Events ]

		[MauEvent("click")]
		public event MauEventHandler Click;

		[MauEvent("dblclick")]
		public event MauEventHandler DoubleClick;

		#endregion

		#region [ UI Methods ]

		public void SetStyle(string styleName, string styleValue)
		{
			var data = new JObject
			{
				{"styleName", styleName},
				{"styleValue", styleValue},
			};

			MyAngularUi.SendRequest(MauId, MyAngularUi.RequestType.SetStyle, data);
		}
		public void RemoveStyle(string styleName)
		{
			var data = new JObject
			{
				{"styleName", styleName}
			};

			MyAngularUi.SendRequest(MauId, MyAngularUi.RequestType.RemoveStyle, data);
		}

		public void AddClass(string className)
		{
			var data = new JObject
			{
				{"className", className}
			};

			MyAngularUi.SendRequest(MauId, MyAngularUi.RequestType.AddClass, data);
		}
		public void RemoveClass(string className)
		{
			var data = new JObject
			{
				{"className", className}
			};

			MyAngularUi.SendRequest(MauId, MyAngularUi.RequestType.RemoveClass, data);
		}

		#endregion

		internal MauProperty GetMauPropAttribute(string propName)
		{
			return HandledProps.ContainsKey(propName)
				? HandledProps[propName].Holder.GetCustomAttribute<MauProperty>(false)
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
			HandledProps = new Dictionary<string, BoolHolder<PropertyInfo>>();
			HandledVars = new Dictionary<string, PropertyInfo>();
			HandledMethods = new Dictionary<string, MethodInfo>();

			Init();
		}

		private void Init()
		{
			// Events
			{
				EventInfo[] eventInfos = this.GetType().GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (EventInfo eventInfo in eventInfos.Where(MauEvent.HasAttribute))
				{
					var attr = eventInfo.GetCustomAttribute<MauEvent>();
					HandledEvents.Add(attr.EventName, eventInfo);
				}
			}

			// Properties
			{
				PropertyInfo[] propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				foreach (PropertyInfo propertyInfo in propertyInfos.Where(MauProperty.HasAttribute))
				{
					var attr = propertyInfo.GetCustomAttribute<MauProperty>();
					HandledProps.Add(attr.PropertyName, new BoolHolder<PropertyInfo>(propertyInfo, true));
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
			if (eventDelegate == null)
				return;

			// Invoke all subscribers
			foreach (Delegate handler in eventDelegate.GetInvocationList())
			{
				Task.Run(() =>
				{
					try
					{
						handler.Method.Invoke(handler.Target, new object[] { this, new MauEventInfo(eventName, eventType, eventData) });
					}
					catch (Exception ex)
					{
                        MyAngularUi.RaiseExeption(ex.InnerException);
					}
				});
			}
		}
		internal Dictionary<string, BoolHolder<PropertyInfo>> GetValidToSetHandledProps()
		{
			Dictionary<string, BoolHolder<PropertyInfo>> componentsProps = this.HandledProps
				//.Where(x => !this.GetMauPropAttribute(x.Key).ReadOnly)
				.OrderBy(x => this.GetMauPropAttribute(x.Key).Important)
				.ToDictionary(x => x.Key, x => x.Value);

			return componentsProps;
		}

		internal void RequestPropValue(string propName)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			MauProperty mauPropertyAttr = GetMauPropAttribute(propName);
			var data = new JObject
			{
				{"propName", propName},
				{"propType", (int)mauPropertyAttr.PropType},
				{"propStatus", (int)mauPropertyAttr.PropStatus} // Needed by `SetPropHandler`
			};

			MyAngularUi.SendRequest(MauId, MyAngularUi.RequestType.GetPropValue, data);
		}
		internal void SetPropValue(string propName, JToken propValueJson)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			Type propValType = GetPropType(propName);
			object propValue = MyAngularUi.ParseMauDataFromFrontEnd(propValType, propValueJson);

			// Make valid enum value
			MauEnumMember.GetValidEnumValue(propValType, ref propValue);

			// Deadlock will not happen because of 'HandledProps[propName].Value' (HandleOnSet)
			lock (HandledProps[propName])
			{
				HandledProps[propName].Value = false;
				HandledProps[propName].Holder.SetValue(this, propValue);
				HandledProps[propName].Value = true;
			}
		}

		internal void SetMethodRetValue(int callMethodRequestId, string methodName, JToken methodRetValueJson)
		{
			if (!HandledMethods.ContainsKey(methodName))
				return;

			Type methodRetType = GetMethodReturnType(methodName);
			object methodRet = MyAngularUi.ParseMauDataFromFrontEnd(methodRetType, methodRetValueJson);

			// Make valid enum value
			MauEnumMember.GetValidEnumValue(methodRetType, ref methodRet);

			MyAngularUi.OrdersResponse.TryAdd(callMethodRequestId, methodRet);
		}
		internal object GetMethodRetValue(int callMethodRequestId)
		{
			while (!MyAngularUi.OrdersResponse.ContainsKey(callMethodRequestId) && MyAngularUi.Connected)
				Thread.Sleep(1);

            if (!MyAngularUi.Connected)
                return null;

			// Get and remove data
			MyAngularUi.OrdersResponse.TryRemove(callMethodRequestId, out object ret);

			return ret;
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MAU.Attributes;
using MAU.Events;
using MAU.Helper;
using Newtonsoft.Json.Linq;
using static MAU.Events.MauEventHandlers;
using static MAU.MyAngularUi;

namespace MAU.Core
{
	public abstract class MauComponent
	{
		#region [ Internal Props ]

		internal bool AngularSent { get; set; }

		#endregion

		#region [ Internal Fields ]

		internal readonly Dictionary<string, EventInfo> HandledEvents;
		internal readonly Dictionary<string, BoolHolder<PropertyInfo>> HandledProps; // bool for handleOnSet
		internal readonly Dictionary<string, PropertyInfo> HandledVars;
		internal readonly Dictionary<string, MethodInfo> HandledMethods;

		#endregion

		#region [ Public Proparties ]

		public IReadOnlyCollection<string> Events => HandledEvents.Keys.ToList().AsReadOnly();
		public IReadOnlyCollection<string> Props => HandledProps.Keys.ToList().AsReadOnly();
		public IReadOnlyCollection<string> Vars => HandledVars.Keys.ToList().AsReadOnly();
		public IReadOnlyCollection<string> Methods => HandledMethods.Keys.ToList().AsReadOnly();
		public string MauId { get; }

		#endregion

		#region [ UI Proparties ]

		//[MauProperty("innerText", MauPropertyType.NativeProperty)]
		//public string InnerText { get; set; }

		//[MauProperty("innerHTML", MauPropertyType.NativeProperty)]
		//public string InnerHtml { get; set; }

		//[MauProperty("textContent", MauPropertyType.NativeProperty)]
		//public string TextContent { get; set; }

		[MauProperty("style", MauProperty.MauPropertyType.NativeProperty, ReadOnly = true)]
		public string Style { get; internal set; }

		[MauProperty("className", MauProperty.MauPropertyType.NativeProperty, ReadOnly = true)]
		public string ClassName { get; internal set; }

		#endregion

		#region [ UI Events ]

		[MauEvent("click")]
		public event MauEventHandler Click;

		[MauEvent("dblclick")]
		public event MauEventHandler DoubleClick;

		#endregion

		#region [ UI Methods ]

		public async Task SetStyle(string styleName, string styleValue)
		{
			var data = new JObject
			{
				{"styleName", styleName},
				{"styleValue", styleValue},
			};

			await MyAngularUi.SendRequest(MauId, RequestType.SetStyle, data);
		}
		public async Task RemoveStyle(string styleName)
		{
			var data = new JObject
			{
				{"styleName", styleName}
			};

			await MyAngularUi.SendRequest(MauId, RequestType.RemoveStyle, data);
		}

		public async Task AddClass(string className)
		{
			var data = new JObject
			{
				{"className", className}
			};

			await MyAngularUi.SendRequest(MauId, RequestType.AddClass, data);
		}
		public async Task RemoveClass(string className)
		{
			var data = new JObject
			{
				{"className", className}
			};

			await MyAngularUi.SendRequest(MauId, RequestType.RemoveClass, data);
		}

		#endregion

		protected MauComponent(string mauId)
		{
			if (MyAngularUi.IsComponentRegistered(mauId))
				throw new ArgumentOutOfRangeException(nameof(mauId), "MauComponent with same mauId was registered.");

			MauId = mauId;
			HandledEvents = new Dictionary<string, EventInfo>();
			HandledProps = new Dictionary<string, BoolHolder<PropertyInfo>>();
			HandledVars = new Dictionary<string, PropertyInfo>();
			HandledMethods = new Dictionary<string, MethodInfo>();

			InitComponents();
		}

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

		private void InitComponents()
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
				{
					HandledVars.Add(varInfo.Name, varInfo);
				}
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
				Task.Run(() => handler.Method.Invoke(handler.Target, new object[] { this, new MauEventInfo(eventName, eventType, eventData) }));
		}
		internal Dictionary<string, BoolHolder<PropertyInfo>> GetValidToSetHandledProps()
		{
			Dictionary<string, BoolHolder<PropertyInfo>> componentsProps = this.HandledProps
				//.Where(x => !this.GetMauPropAttribute(x.Key).ReadOnly)
				.OrderBy(x => this.GetMauPropAttribute(x.Key).Important)
				.ToDictionary(x => x.Key, x => x.Value);

			return componentsProps;
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

		internal async Task GetPropValue(string propName)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			MauProperty mauPropertyAttr = GetMauPropAttribute(propName);
			var data = new JObject
			{
				{"propName", propName},
				{"propType", (int)mauPropertyAttr.PropType}
			};

			await MyAngularUi.SendRequest(MauId, RequestType.GetPropValue, data);
		}
		internal void SetPropValue(string propName, JToken propValueJson)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			Type propValType = GetPropType(propName);
			object propValue = ParseMauDataFromFrontEnd(propValType, propValueJson);

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
			if (methodRetType == typeof(void))
				return;

			object methodRet = ParseMauDataFromFrontEnd(methodRetType, methodRetValueJson);

			// Make valid enum value
			MauEnumMember.GetValidEnumValue(methodRetType, ref methodRet);

			MyAngularUi.OrdersResponse.TryAdd(callMethodRequestId, methodRet);
		}
		internal object GetMethodRetValue(int callMethodRequestId)
		{
			while (!MyAngularUi.OrdersResponse.ContainsKey(callMethodRequestId))
				Thread.Sleep(1);

			// Get and remove data
			MyAngularUi.OrdersResponse.TryRemove(callMethodRequestId, out object ret);

			return ret;
		}
	}
}

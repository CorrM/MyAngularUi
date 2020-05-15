using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MAU.Attributes;
using MAU.Events;
using Newtonsoft.Json.Linq;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;
using static MAU.MyAngularUi;

namespace MAU.Core
{
	public abstract class MauElement
	{
		#region [ Internal Props ]

		/// <summary>
		/// Not fire <see cref="MauProperty.OnSetValue"/> when set value fetched from front-end side
		/// </summary>
		internal bool HandleOnSet { get; set; } = true;
		internal MauComponent ParentComponent { get; }

		#endregion

		#region [ Internal Fields ]

		internal readonly Dictionary<string, EventInfo> HandledEvents;
		internal readonly Dictionary<string, PropertyInfo> HandledProps;
		internal readonly Dictionary<string, MethodInfo> HandledMethods;
		internal readonly Dictionary<string, object> MethodsRetValues;

		#endregion

		#region [ Public Proparties ]

		public IReadOnlyCollection<string> Events => HandledEvents.Keys.ToList();
		public string MauId { get; }

		#endregion

		#region [ UI Proparties ]

		//[MauProperty("innerText", MauPropertyType.NativeProperty)]
		//public string InnerText { get; set; }

		//[MauProperty("innerHTML", MauPropertyType.NativeProperty)]
		//public string InnerHtml { get; set; }

		//[MauProperty("textContent", MauPropertyType.NativeProperty)]
		//public string TextContent { get; set; }

		[MauProperty("style", MauPropertyType.NativeProperty)]
		public string Style { get; internal set; }

		[MauProperty("className", MauPropertyType.NativeProperty)]
		public string ClassName { get; internal set; }

		#endregion

		#region [ UI Events ]

		[MauEvent("click")]
		public event MauEventHandler Click;

		#endregion

		#region [ UI Methods ]

		public void SetStyle(string styleName, string styleValue)
		{
			var data = new JObject
			{
				{"styleName", styleName},
				{"styleValue", styleValue},
			};

			_ = MyAngularUi.SendRequest(MauId, RequestType.SetStyle, data);
		}
		public void RemoveStyle(string styleName)
		{
			var data = new JObject
			{
				{"styleName", styleName}
			};

			_ = MyAngularUi.SendRequest(MauId, RequestType.RemoveStyle, data);
		}
		public void AddClass(string className)
		{
			var data = new JObject
			{
				{"className", className}
			};

			_ = MyAngularUi.SendRequest(MauId, RequestType.AddClass, data);
		}
		public void RemoveClass(string className)
		{
			var data = new JObject
			{
				{"className", className}
			};

			_ = MyAngularUi.SendRequest(MauId, RequestType.RemoveClass, data);
		}

		#endregion

		protected MauElement(MauComponent parentComponent, string mauId)
		{
			if (MyAngularUi.IsMauRegistered(mauId))
				throw new ArgumentOutOfRangeException(nameof(mauId), "MauElement with same mauId was registered.");

			ParentComponent = parentComponent;
			MauId = mauId;
			HandledEvents = new Dictionary<string, EventInfo>();
			HandledProps = new Dictionary<string, PropertyInfo>();
			HandledMethods = new Dictionary<string, MethodInfo>();
			MethodsRetValues = new Dictionary<string, object>();

			InitElements();
		}

		internal MauProperty GetMauPropAttribute(string propName)
		{
			return HandledProps.ContainsKey(propName)
				? HandledProps[propName].GetCustomAttribute<MauProperty>()
				: null;
		}
		internal MauMethod GetMauMethodAttribute(string methodName)
		{
			return HandledMethods.ContainsKey(methodName)
				? HandledMethods[methodName].GetCustomAttribute<MauMethod>()
				: null;
		}

		private void InitElements()
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
					HandledProps.Add(attr.PropertyName, propertyInfo);
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
				_ = Task.Run(() => handler.Method.Invoke(handler.Target, new object[] { this, new MauEventInfo(eventName, eventType, eventData) }));
		}

		internal Type GetPropType(string propName)
		{
			return HandledProps.ContainsKey(propName)
				? HandledProps[propName].PropertyType
				: null;
		}
		internal Type GetMethodReturnType(string methodName)
		{
			return HandledMethods.ContainsKey(methodName)
				? HandledMethods[methodName].ReturnType
				: null;
		}

		internal void SetPropValue(string propName, JToken propValueJson)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			HandleOnSet = false;
			Type propValType = GetPropType(propName);
			object propValue = ParseMauDataFromFrontEnd(propValType, propValueJson);

			// Make valid enum value
			MauEnumMember.GetValidEnumValue(propValType, ref propValue);

			HandledProps[propName].SetValue(this, propValue);
			HandleOnSet = true;
		}
		internal void GetPropValue(string propName)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			MauProperty mauPropertyAttr = GetMauPropAttribute(propName);
			var data = new JObject
			{
				{"propName", propName},
				{"propType", (int)mauPropertyAttr.PropType}
			};

			_ = MyAngularUi.SendRequest(MauId, RequestType.GetPropValue, data);
		}

		internal void SetMethodRetValue(string methodName, JToken methodRetValueJson)
		{
			if (!HandledMethods.ContainsKey(methodName))
				return;

			Type methodRetType = GetMethodReturnType(methodName);
			object methodRet = ParseMauDataFromFrontEnd(methodRetType, methodRetValueJson);

			// Make valid enum value
			MauEnumMember.GetValidEnumValue(methodRetType, ref methodRet);

			// HandledMethods[methodName].SetValue(this, val);
		}
		internal object GetMethodRetValue(string methodName)
		{
			return new object();
		}
	}
}

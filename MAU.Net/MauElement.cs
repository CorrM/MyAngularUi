using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MAU.Attributes;
using MAU.Events;
using Newtonsoft.Json.Linq;
using static MAU.Events.MauEventHandlers;
using static MAU.MyAngularUi;

namespace MAU
{
	public abstract class MauElement
	{

		#region [ Internal Fields ]

		/// <summary>
		/// Not fire <see cref="MauProperty.OnSetValue"/> when set value local
		/// </summary>
		internal bool HandleOnSet { get; set; } = true;

		internal readonly Dictionary<string, EventInfo> HandledEvents;
		internal readonly Dictionary<string, PropertyInfo> HandledProps;

		#endregion

		#region [ Public Proparties ]

		public IReadOnlyCollection<string> Events => HandledEvents.Keys.ToList();
		public string Id { get; }

		#endregion

		#region [ UI Proparties ]

		[MauProperty("innerText", false)]
		public string Text { get; set; }

		[MauProperty("innerHTML", false)]
		public string Html { get; set; }

		[MauProperty("textContent", false)]
		public string TextContent { get; set; }

		#endregion

		#region [ UI Events ]

		[MauEvent("click")]
		public event MauEventHandler Click;

		#endregion

		protected MauElement(string id)
		{
			Id = id;
			HandledEvents = new Dictionary<string, EventInfo>();
			HandledProps = new Dictionary<string, PropertyInfo>();

			InitElements();
		}

		private void InitElements()
		{
			// Events
			{
				EventInfo[] eventInfos = this.GetType().GetEvents(BindingFlags.Public | BindingFlags.Instance);
				foreach (EventInfo eventInfo in eventInfos.Where(MauEvent.HasAttribute))
				{
					var attr = eventInfo.GetCustomAttribute<MauEvent>();
					HandledEvents.Add(attr.EventName, eventInfo);
				}
			}

			// Properties
			{
				PropertyInfo[] propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo propertyInfo in propertyInfos.Where(MauProperty.HasAttribute))
				{
					var attr = propertyInfo.GetCustomAttribute<MauProperty>();
					HandledProps.Add(attr.PropertyName, propertyInfo);
				}
			}
		}
		internal MauProperty GetUiPropAttribute(string propName)
		{
			return HandledProps.ContainsKey(propName)
				? HandledProps[propName].GetCustomAttribute<MauProperty>()
				: null;
		}

		public void FireEvent(string eventName, string eventType, JObject eventData)
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
			foreach (var handler in eventDelegate.GetInvocationList())
				_ = Task.Run(() => handler.Method.Invoke(handler.Target, new object[] { eventName, new MauEventInfo(eventType, eventData) }));
		}
		public void SetPropValue(string propName, object propValue)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			HandleOnSet = false;
			HandledProps[propName].SetValue(this, propValue);
			HandleOnSet = true;
		}
		public void GetPropValue(string propName)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			var data = new JObject
			{
				{"propName", propName},
				{"propIsAttr", GetUiPropAttribute(propName).IsAttribute}
			};

			_ = MyAngularUi.SendRequest(Id, RequestType.GetPropValue, data);
		}
	}
}

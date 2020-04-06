using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
		/// Not fire <see cref="MauProperty.OnSetValue"/> when set value local
		/// </summary>
		internal bool HandleOnSet { get; set; } = true;
		internal MauComponent ParentComponent { get; }

		#endregion

		#region [ Internal Fields ]

		internal readonly Dictionary<string, EventInfo> HandledEvents;
		internal readonly Dictionary<string, PropertyInfo> HandledProps;

		#endregion

		#region [ Public Proparties ]

		public IReadOnlyCollection<string> Events => HandledEvents.Keys.ToList();
		public string MauId { get; }

		#endregion

		#region [ UI Proparties ]

		[MauProperty("innerText", MauPropertyType.NativeProperty)]
		public string Text { get; set; }

		[MauProperty("innerHTML", MauPropertyType.NativeProperty)]
		public string Html { get; set; }

		[MauProperty("textContent", MauPropertyType.NativeProperty)]
		public string TextContent { get; set; }

		#endregion

		#region [ UI Events ]

		[MauEvent("click")]
		public event MauEventHandler Click;

		#endregion

		protected MauElement(MauComponent parentComponent, string mauId)
		{
			if (MyAngularUi.MauRegistered(mauId))
				throw new ArgumentOutOfRangeException(nameof(mauId), "MauElement with same mauId was registered.");

			ParentComponent = parentComponent;
			MauId = mauId;
			HandledEvents = new Dictionary<string, EventInfo>();
			HandledProps = new Dictionary<string, PropertyInfo>();

			InitElements();
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
				_ = Task.Run(() => handler.Method.Invoke(handler.Target, new object[] { this, new MauEventInfo(eventName, eventType, eventData) }));
		}
		public void SetPropValue(string propName, object propValue)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			HandleOnSet = false;
			Type valType = HandledProps[propName].PropertyType;

			// Make valid value
			object val;
			if (valType == typeof(string))
			{
				val = propValue;
			}

			else if (valType.IsEnum)
			{
				if (!MauEnumMember.HasNotSetValue(valType))
					throw new Exception($"NoSet must to be in any MauProperty value is 'Enum', {valType.FullName}");

				string enumValName = valType.GetFields()
					.Where(f => MauEnumMember.HasAttribute(f))
					.Where(f => f.GetCustomAttributes<MauEnumMember>(false).FirstOrDefault().GetValue().Equals(propValue))
					.FirstOrDefault()?.Name;

				if (string.IsNullOrEmpty(enumValName))
					val = Enum.ToObject(valType, 0);
				else
					val = Enum.Parse(valType, enumValName);
			}

			else
			{
				val = propValue == null
					? Activator.CreateInstance(valType)
					: Convert.ChangeType(propValue, valType);
			}


			HandledProps[propName].SetValue(this, val);
			HandleOnSet = true;
		}
		public void GetPropValue(string propName)
		{
			if (!HandledProps.ContainsKey(propName))
				return;

			MauProperty mauPropertyAttr = GetUiPropAttribute(propName);
			var data = new JObject
			{
				{"propName", propName},
				{"propType", (int)mauPropertyAttr.PropType}
			};

			_ = MyAngularUi.SendRequest(MauId, RequestType.GetPropValue, data);
		}
	}
}

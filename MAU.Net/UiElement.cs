using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MAU.Attributes;
using Newtonsoft.Json.Linq;
using static MAU.MyAngularUi;

namespace MAU
{
	public abstract class UiElement
	{

		#region [ Internal Fields ]

		/// <summary>
		/// Not fire <see cref="UiProperty.OnSetValue"/> when set value local
		/// </summary>
		internal bool HandleOnSet { get; set; } = true;

		internal readonly Dictionary<string, MethodInfo> HandledEvents;
		internal readonly Dictionary<string, PropertyInfo> HandledProps;

		#endregion

		#region [ Public Proparties ]

		public IReadOnlyCollection<string> Events => HandledEvents.Keys.ToList();
		public string Id { get; }

		#endregion

		#region [ UI Proparties ]

		[UiProperty("innerText", false)]
		public string Text { get; set; }

		[UiProperty("innerHTML", false)]
		public string Html { get; set; }

		[UiProperty("textContent", false)]
		public string TextContent { get; set; }

		#endregion

		protected UiElement(string id)
		{
			Id = id;
			HandledEvents = new Dictionary<string, MethodInfo>();
			HandledProps = new Dictionary<string, PropertyInfo>();

			InitElements();
		}

		private void InitElements()
		{
			// Events
			{
				MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
				foreach (MethodInfo methodInfo in methodInfos.Where(UiEvent.HasAttribute))
				{
					var attr = methodInfo.GetCustomAttribute<UiEvent>();
					HandledEvents.Add(attr.EventName, methodInfo);
				}
			}

			// Properties
			{
				PropertyInfo[] propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo propertyInfo in propertyInfos.Where(UiProperty.HasAttribute))
				{
					var attr = propertyInfo.GetCustomAttribute<UiProperty>();
					HandledProps.Add(attr.PropertyName, propertyInfo);
				}
			}
		}
		internal UiProperty GetUiPropAttribute(string propName)
		{
			return HandledProps.ContainsKey(propName)
				? HandledProps[propName].GetCustomAttribute<UiProperty>()
				: null;
		}

		public void FireEvent(string eventName, string eventType, JObject eventData)
		{
			if (HandledEvents.ContainsKey(eventName))
				_ = Task.Run(() => HandledEvents[eventName].Invoke(this, new object[] { eventType, eventData }));
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

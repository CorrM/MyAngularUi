using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MAU.Attributes;

namespace MAU
{
	public abstract class UiElement
	{
		#region [ Private Proparties ]

		private Dictionary<string, MethodInfo> _handledEvents { get; }
		private Dictionary<string, PropertyInfo> _handledProps { get; }

		#endregion

		#region [ Public Proparties ]

		public IReadOnlyCollection<string> Events => _handledEvents.Keys.ToList();
		public string Id { get; }

		#endregion

		#region [ UI Proparties ]

		[UiProperty("innerText", false)]
		public string Text { get; set; }

		[UiProperty("innerHTML", false)]
		public string Html { get; set; }

		#endregion

		protected UiElement(string id)
		{
			Id = id;
			_handledEvents = new Dictionary<string, MethodInfo>();

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
					_handledEvents.Add(attr.EventName, methodInfo);
				}
			}

			// Properties
			{
				PropertyInfo[] propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (PropertyInfo propertyInfo in propertyInfos.Where(UiProperty.HasAttribute))
				{
					var attr = propertyInfo.GetCustomAttribute<UiProperty>();
					_handledProps.Add(attr.PropertyName, propertyInfo);
				}
			}
		}

		public void FireEvent(string eventName)
		{
			if (_handledEvents.ContainsKey(eventName))
				_handledEvents[eventName].Invoke(this, Array.Empty<object>());
		}

		public void SetProp(string propName, object propValue)
		{
			if (_handledProps.ContainsKey(propName))
				_handledProps[propName].SetValue(this, propValue);
		}
	}
}

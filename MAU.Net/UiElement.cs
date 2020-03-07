using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAU
{
	public abstract class UiElement
	{
		public string Id { get; }
		private Dictionary<string, MethodInfo> _events { get; }
		public IReadOnlyCollection<string> Events => _events.Keys.ToList();

		protected UiElement(string id)
		{
			Id = id;
			_events = new Dictionary<string, MethodInfo>();

			InitEvents();
		}

		private void InitEvents()
		{
			// Get events
			MethodInfo[] thisType = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
			foreach (MethodInfo methodInfo in thisType.Where(UiEvent.HasAttribute))
			{
				var attr = methodInfo.GetCustomAttribute<UiEvent>();
				_events.Add(attr.EventName, methodInfo);
			}
		}

		public void FireEvent(string eventName)
		{
			if (_events.ContainsKey(eventName))
				_events[eventName].Invoke(this, Array.Empty<object>());
		}
	}
}

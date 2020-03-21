using System;
using System.Linq;
using System.Reflection;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Event)]
	public sealed class UiEvent : Attribute
	{
		public string EventName { get; }

		public UiEvent(string eventName)
		{
			EventName = eventName;
		}

		public static bool HasAttribute(EventInfo methodInfo)
		{
			return methodInfo.GetCustomAttributes(typeof(UiEvent), false).Any();
		}
	}
}

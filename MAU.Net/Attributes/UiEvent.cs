using System;
using System.Linq;
using System.Reflection;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class UiEvent : Attribute
	{
		public string EventName { get; }

		public UiEvent(string eventName)
		{
			EventName = eventName;
		}

		public static bool HasAttribute(MethodInfo methodInfo)
		{
			return methodInfo.GetCustomAttributes(typeof(UiEvent), false).Any();
		}
	}
}

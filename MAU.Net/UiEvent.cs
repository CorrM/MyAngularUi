using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAU
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

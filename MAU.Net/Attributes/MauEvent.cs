using System;
using System.Linq;
using System.Reflection;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Event)]
	public sealed class MauEvent : Attribute
	{
		public string EventName { get; }

		public MauEvent(string eventName)
		{
			EventName = eventName;
		}

		public static bool HasAttribute(EventInfo eventInfo)
		{
			return eventInfo.GetCustomAttributes(typeof(MauEvent), false).Any();
		}
	}
}

using System;
using System.Linq;
using System.Reflection;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class MauVariable : Attribute
	{
		public string ComponentName { get; }

		public MauVariable(string componentName)
		{
			ComponentName = componentName;
		}

		public static bool HasAttribute(EventInfo methodInfo)
		{
			return methodInfo.GetCustomAttributes(typeof(MauEvent), false).Any();
		}
	}
}

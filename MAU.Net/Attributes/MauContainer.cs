using System;
using System.Linq;
using System.Reflection;
using MAU.Core;
using Newtonsoft.Json.Linq;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class MauContainer : Attribute
	{
		public static bool HasAttribute(Type type)
		{
			return type.GetCustomAttributes(typeof(MauContainer), false).Any();
		}
	}
}

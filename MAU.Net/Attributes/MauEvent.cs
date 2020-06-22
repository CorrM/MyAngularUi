using System;
using System.Linq;
using System.Reflection;
using MAU.Core;
using Newtonsoft.Json.Linq;

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

		internal static MyAngularUi.RequestState SendMauEvents(MauComponent holder)
		{
			var ret = new JObject
			{
				{ "events", new JArray(holder.Events) }
			};

			// Send response
			return MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetEvents, ret);
		}
	}
}

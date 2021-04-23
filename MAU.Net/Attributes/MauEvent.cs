using MAU.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MAU.Attributes
{
    [AttributeUsage(AttributeTargets.Event)]
    public sealed class MauEvent : Attribute
    {
        public string EventName { get; }

        public static bool HasAttribute(EventInfo eventInfo) => Attribute.IsDefined(eventInfo, typeof(MauEvent));

        public MauEvent(string eventName)
        {
            EventName = eventName;
        }

        internal static Task<MyAngularUi.RequestState> SendMauEventsAsync(MauComponent holder)
        {
            var ret = new JObject
            {
                { "events", new JArray(holder.MauEvents) }
            };

            // Send response
            return MyAngularUi.SendRequestAsync(holder.MauId, MyAngularUi.RequestType.SetEvents, ret);
        }
    }
}

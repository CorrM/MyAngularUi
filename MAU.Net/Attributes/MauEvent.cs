using System;
using System.Reflection;
using System.Threading.Tasks;
using MAU.Core;
using Newtonsoft.Json.Linq;

namespace MAU.Attributes;

[AttributeUsage(AttributeTargets.Event)]
public sealed class MauEventAttribute : Attribute
{
    public string EventName { get; }

    public static bool HasAttribute(EventInfo eventInfo) => Attribute.IsDefined(eventInfo, typeof(MauEventAttribute));

    public MauEventAttribute(string eventName)
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
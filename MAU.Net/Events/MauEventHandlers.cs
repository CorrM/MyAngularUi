using System.Threading.Tasks;
using MAU.Core;

namespace MAU.Events;

public static class MauEventHandlers
{
    public delegate ValueTask MauEventHandlerAsync(MauComponent component, MauEventInfo eventInfo);
}
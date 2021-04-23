using MAU.Core;
using System.Threading.Tasks;

namespace MAU.Events
{
    public static class MauEventHandlers
    {
        public delegate ValueTask MauEventHandlerAsync(MauComponent component, MauEventInfo eventInfo);
    }
}

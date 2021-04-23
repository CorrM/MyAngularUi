using MAU.Attributes;

namespace MAU.Helper.Enums
{
    public enum OneOffEnum
    {
        [MauEnumMember("")]
        NotSet,

        [MauEnumMember("on")]
        On,

        [MauEnumMember("off")]
        Off
    }
}

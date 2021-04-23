using MAU.Attributes;

namespace MAU.Helper.Enums
{
    public enum ThemePalette
    {
        [MauEnumMember("")]
        NotSet,

        [MauEnumMember("primary")]
        Primary,

        [MauEnumMember("accent")]
        Accent,

        [MauEnumMember("warn")]
        Warn
    }
}

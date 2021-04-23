using MAU.Attributes;

namespace MAU.Helper.Enums
{
    public enum ProgressBarMode
    {
        [MauEnumMember("")]
        NotSet,

        [MauEnumMember("determinate")]
        Determinate,

        [MauEnumMember("indeterminate")]
        Indeterminate,

        [MauEnumMember("buffer")]
        Buffer,

        [MauEnumMember("query")]
        Query
    }
}

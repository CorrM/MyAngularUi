using MAU.Attributes;
using MAU.Core;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular;

public enum NgAlertType
{
    [MauEnumMember("")]
    NotSet,

    [MauEnumMember("success")]
    Success,

    [MauEnumMember("info")]
    Info,

    [MauEnumMember("warning")]
    Warning,

    [MauEnumMember("danger")]
    Danger,

    [MauEnumMember("primary")]
    Primary,

    [MauEnumMember("secondary")]
    Secondary,

    [MauEnumMember("light")]
    Light,

    [MauEnumMember("dark")]
    Dark,
}

public class MauNgAlert : MauComponent
{
    #region [ Mau Events ]

    [MauEvent("closed")]
    public event MauEventHandlerAsync OnClosed;

    #endregion

    #region [ Mau Variable ]

    [MauVariable("Text")]
    public string Text { get; set; }

    [MauVariable("Visible")]
    public bool Visible { get; set; }

    #endregion

    #region [ Mau Properties ]

    [MauProperty("dismissible", MauPropertyType.ComponentProperty)]
    public bool Dismissible { get; set; }

    [MauProperty("animation", MauPropertyType.ComponentProperty)]
    public bool Animation { get; set; }

    [MauProperty("type", MauPropertyType.ComponentProperty)]
    public NgAlertType Type { get; set; }

    #endregion

    #region [ Mau Methods ]

    [MauMethod("close", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Close() { }

    #endregion

    public MauNgAlert(string mauId) : base(mauId) { }
}
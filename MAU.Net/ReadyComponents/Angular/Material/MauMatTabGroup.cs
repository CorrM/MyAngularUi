using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular.Material;

public class MauMatTabGroup : MauComponent
{
    #region [ Mau Events ]

    [MauEvent("animationDone")]
    public event MauEventHandlerAsync OnAnimationDone;

    [MauEvent("focusChange")]
    public event MauEventHandlerAsync OnFocusChange;

    [MauEvent("selectedIndexChange")]
    public event MauEventHandlerAsync OnSelectedIndexChange;

    [MauEvent("selectedTabChange")]
    public event MauEventHandlerAsync OnSelectedTabChange;

    #endregion

    #region [ Mau Properties ]

    [MauProperty("animationDuration", MauPropertyType.ComponentProperty)]
    public string AnimationDuration { get; set; }

    [MauProperty("backgroundColor", MauPropertyType.ComponentProperty)]
    public ThemePalette BackgroundColor { get; set; }

    [MauProperty("color", MauPropertyType.ComponentProperty)]
    public ThemePalette Color { get; set; }

    [MauProperty("disablePagination", MauPropertyType.ComponentProperty)]
    public bool DisablePagination { get; set; }

    [MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
    public bool DisableRipple { get; set; }

    [MauProperty("dynamicHeight", MauPropertyType.ComponentProperty)]
    public bool DynamicHeight { get; set; }

    [MauProperty("headerPosition", MauPropertyType.ComponentProperty)]
    public MatTabHeaderPosition HeaderPosition { get; set; }

    [MauProperty("selectedIndex", MauPropertyType.ComponentProperty)]
    public int SelectedIndex { get; set; }

    #endregion

    #region [ Mau Methods ]

    [MauMethod("realignInkBar", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void RealignInkBar()
    {
    }

    #endregion

    public MauMatTabGroup(string mauId) : base(mauId)
    {
    }
}
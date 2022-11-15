using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular.Material;

public class MauMatVerticalStepper : MauComponent
{
    #region [ Mau Events ]

    [MauEvent("animationDone")]
    public event MauEventHandlerAsync OnAnimationDone;

    [MauEvent("selectionChange")]
    public event MauEventHandlerAsync OnSelectionChange;

    #endregion

    #region [ Mau Methods ]

    [MauMethod("next", MauMethod.MauMethodType.ComponentMethod, MauMethod.MauMethodCallType.ExecuteInAngular)]
    public void Next()
    {
    }

    [MauMethod("previous", MauMethod.MauMethodType.ComponentMethod, MauMethod.MauMethodCallType.ExecuteInAngular)]
    public void Previous()
    {
    }

    [MauMethod("reset", MauMethod.MauMethodType.ComponentMethod, MauMethod.MauMethodCallType.ExecuteInAngular)]
    public void Reset()
    {
    }

    #endregion

    #region [ Mau Properties ]

    [MauProperty("color", MauPropertyType.ComponentProperty)]
    public ThemePalette Color { get; set; }

    [MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
    public bool DisableRipple { get; set; }

    [MauProperty("linear", MauPropertyType.ComponentProperty)]
    public bool Linear { get; set; }

    [MauProperty("selectedIndex", MauPropertyType.ComponentProperty)]
    public int SelectedIndex { get; set; }

    #endregion

    public MauMatVerticalStepper(string mauId) : base(mauId)
    {
    }
}
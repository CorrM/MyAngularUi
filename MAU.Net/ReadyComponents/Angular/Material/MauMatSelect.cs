using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Types;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular.Material;

public class MauMatSelect : MauComponent
{
    #region [ Mau Events ]

    [MauEvent("openedChange")]
    public event MauEventHandlerAsync OnOpenedChange;

    [MauEvent("selectionChange")]
    public event MauEventHandlerAsync OnSelectionChange;

    [MauEvent("valueChange")]
    public event MauEventHandlerAsync OnValueChange;

    #endregion

    #region [ Mau Variable ]

    [MauVariable("Options")]
    public MauDataList<string> Options { get; set; }

    #endregion

    #region [ Mau Properties ]

    [MauProperty("disableOptionCentering", MauPropertyType.ComponentProperty)]
    public bool DisableOptionCentering { get; set; }

    [MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
    public bool DisableRipple { get; set; }

    [MauProperty("disabled", MauPropertyType.ComponentProperty, Important = true)]
    public bool Disabled { get; set; }

    [MauProperty("placeholder", MauPropertyType.ComponentProperty)]
    public string Placeholder { get; set; }

    [MauProperty("required", MauPropertyType.ComponentProperty)]
    public bool Required { get; set; }

    [MauProperty("typeaheadDebounceInterval", MauPropertyType.ComponentProperty)]
    public long TypeaheadDebounceInterval { get; set; }

    [MauProperty("value", MauPropertyType.ComponentProperty)]
    public string SelectedItem { get; private set; }

    [MauProperty("empty", MauPropertyType.ComponentProperty, PropStatus = MauPropertyStatus.ReadOnly)]
    public bool Empty { get; set; }

    [MauProperty("focused", MauPropertyType.ComponentProperty, PropStatus = MauPropertyStatus.ReadOnly)]
    public bool Focused { get; set; }

    [MauProperty("panelOpen", MauPropertyType.ComponentProperty, PropStatus = MauPropertyStatus.ReadOnly)]
    public bool PanelOpen { get; set; }

    #endregion

    #region [ Mau Methods ]

    [MauMethod("close", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Close()
    {
    }

    [MauMethod("focus", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Focus()
    {
    }

    [MauMethod("open", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Open()
    {
    }

    [MauMethod("toggle", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Toggle()
    {
    }

    #endregion

    public MauMatSelect(string mauId) : base(mauId)
    {
        Options = new MauDataList<string>(this, nameof(Options));
    }

    #region [ Options Controlling ]

    public bool SelectOption(string newOption)
    {
        if (!Options.Contains(newOption))
            return false;

        SelectedItem = newOption;
        return true;
    }

    public bool SelectOption(int newOptionIndex)
    {
        if (newOptionIndex >= Options.Count)
            return false;

        SelectedItem = Options[newOptionIndex];
        return true;
    }

    #endregion
}
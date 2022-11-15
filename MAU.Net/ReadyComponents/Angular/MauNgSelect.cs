using System.Threading.Tasks;
using MAU.Attributes;
using MAU.Core;
using MAU.Events;
using MAU.Helper.Types;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular;

// Todo: Make MauValueResolverAttribute
// To mark how to resolve that type, that type must to be MauDataParser
/*
public class NgSelectNgOption
{
	[MauProperty("disabled", MauPropertyType.ComponentProperty)]
	public bool Disabled { get; set; }

	[MauProperty("htmlId", MauPropertyType.ComponentProperty)]
	public string HtmlId { get; set; }

	[MauProperty("index", MauPropertyType.ComponentProperty)]
	public int Index { get; set; }

	[MauProperty("label", MauPropertyType.ComponentProperty)]
	public string Label { get; set; }

	[MauProperty("marked", MauPropertyType.ComponentProperty)]
	public bool Marked { get; set; }

	[MauProperty("selected", MauPropertyType.ComponentProperty)]
	public bool Selected { get; set; }

	[MauProperty("value", MauPropertyType.ComponentProperty)]
	public string Value { get; set; }
}

public class NgSelectItemsList
{
	[MauProperty("_items", MauPropertyType.ComponentProperty)]
	public List<NgSelectNgOption> Items { get; set; }
}
*/
public class MauNgSelect : MauComponent
{
    #region [ Mau Events ]

    [MauEvent("addEvent")]
    public event MauEventHandlerAsync OnAdd;

    [MauEvent("blurEvent")]
    public event MauEventHandlerAsync OnBlur;

    [MauEvent("changeEvent")]
    private event MauEventHandlerAsync OnChangeImp;

    [MauEvent("closeEvent")]
    public event MauEventHandlerAsync OnClose;

    [MauEvent("clearEvent")]
    public event MauEventHandlerAsync OnClear;

    [MauEvent("focusEvent")]
    public event MauEventHandlerAsync OnFocus;

    [MauEvent("searchEvent")]
    public event MauEventHandlerAsync OnSearch;

    [MauEvent("openEvent")]
    public event MauEventHandlerAsync OnOpen;

    [MauEvent("removeEvent")]
    public event MauEventHandlerAsync OnRemove;

    [MauEvent("scroll")]
    public event MauEventHandlerAsync OnScroll;

    [MauEvent("scrollToEnd")]
    public event MauEventHandlerAsync OnScrollToEnd;

    public event MauEventHandlerAsync OnChange;

    #endregion

    #region [ Mau Variable ]

    [MauVariable("Options")]
    public MauDataList<string> Options { get; set; }

    #endregion

    #region [ Mau Properties ]

    [MauProperty("readonly", MauPropertyType.ComponentProperty)]
    public bool Readonly { get; set; }

    /*[MauProperty("itemsList", MauPropertyType.ComponentProperty)]
    public NgSelectItemsList ItemsList { get; set; }*/

    /*[MauProperty("selectedValues", MauPropertyType.ComponentProperty)]
    public List<string> SelectedValues { get; set; }*/

    [MauProperty("_selectedIndex", MauPropertyType.ComponentProperty)]
    private int SelectedIndexImpl { get; set; } // Don't remove

    [MauProperty("selectedIndex", MauPropertyType.ComponentProperty)]
    public int SelectedIndex { get; set; }

    [MauProperty("appearance", MauPropertyType.ComponentProperty)]
    public string Appearance { get; set; }

    [MauProperty("appendTo", MauPropertyType.ComponentProperty)]
    public string AppendTo { get; set; }

    [MauProperty("bindValue", MauPropertyType.ComponentProperty)]
    public string BindValue { get; set; }

    [MauProperty("bindLabel", MauPropertyType.ComponentProperty)]
    public string BindLabel { get; set; }

    [MauProperty("closeOnSelect", MauPropertyType.ComponentProperty)]
    public bool CloseOnSelect { get; set; }

    [MauProperty("clearAllText", MauPropertyType.ComponentProperty)]
    public string ClearAllText { get; set; }

    [MauProperty("focused", MauPropertyType.ComponentProperty)]
    public bool Focused { get; set; }

    [MauProperty("clearable", MauPropertyType.ComponentProperty)]
    public bool Clearable { get; set; }

    [MauProperty("clearOnBackspace", MauPropertyType.ComponentProperty)]
    public bool ClearOnBackspace { get; set; }

    [MauProperty("notFoundText", MauPropertyType.ComponentProperty)]
    public string NotFoundText { get; set; }

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

    [MauMethod("blur", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Blur()
    {
    }

    #endregion

    public MauNgSelect(string mauId) : base(mauId)
    {
        Options = new MauDataList<string>(this, nameof(Options));
        OnChangeImp += NgSelect_OnChangeImpAsync;
    }

    private async ValueTask NgSelect_OnChangeImpAsync(MauComponent component, MauEventInfo eventInfo)
    {
        string value = eventInfo.Data.Value<string>("value");
        //_selectedIndex = Options.FindIndex(opt => opt == value);
        await (OnChange?.Invoke(component, eventInfo).ConfigureAwait(false) ?? default);
    }

    public string GetSelectedValue()
    {
        return SelectedIndex == -1 || Options.Count <= SelectedIndex
            ? string.Empty
            : Options[SelectedIndex];
    }
}
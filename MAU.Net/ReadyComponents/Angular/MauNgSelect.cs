using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Types;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular
{
    public class MauNgSelect : MauComponent
    {
        #region [ Mau Events ]

        [MauEvent("add")]
        public event MauEventHandlerAsync OnAdd;

        [MauEvent("blur")]
        public event MauEventHandlerAsync OnBlur;

        [MauEvent("change")]
        public event MauEventHandlerAsync OnChange;

        [MauEvent("close")]
        public event MauEventHandlerAsync OnClose;

        [MauEvent("clear")]
        public event MauEventHandlerAsync OnClear;

        [MauEvent("focus")]
        public event MauEventHandlerAsync OnFocus;

        [MauEvent("search")]
        public event MauEventHandlerAsync OnSearch;

        [MauEvent("open")]
        public event MauEventHandlerAsync OnOpen;

        [MauEvent("remove")]
        public event MauEventHandlerAsync OnRemove;

        [MauEvent("scroll")]
        public event MauEventHandlerAsync OnScroll;

        [MauEvent("scrollToEnd")]
        public event MauEventHandlerAsync OnScrollToEnd;

        #endregion

        #region [ Mau Variable ]

        [MauVariable("Options")]
        public MauDataList<string> Options { get; set; }

        #endregion

        #region [ Mau Properties ]

        public bool Disabled { get; set; }
        public string SelectedItem { get; set; }


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

        [MauProperty("clearable", MauPropertyType.ComponentProperty)]
        public bool Clearable { get; set; }

        [MauProperty("clearOnBackspace", MauPropertyType.ComponentProperty)]
        public bool ClearOnBackspace { get; set; }

        [MauProperty("notFoundText", MauPropertyType.ComponentProperty)]
        public string NotFoundText { get; set; }

        #endregion

        #region [ Mau Methods ]

        [MauMethod("close", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Close() { }

        [MauMethod("focus", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Focus() { }

        [MauMethod("open", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Open() { }

        [MauMethod("blur", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Blur() { }

        #endregion

        public MauNgSelect(string mauId) : base(mauId)
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
}

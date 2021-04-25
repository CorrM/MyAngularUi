using MAU.Attributes;
using MAU.Core;
using MAU.Events;
using MAU.Helper.Enums;
using MAU.Helper.Types;
using Newtonsoft.Json.Linq;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular
{
    public class MauAutocomplete : MauComponent
    {
        #region [ Mau Variable ]

        [MauVariable("Data")]
        public MauDataList<string> Data { get; set; }

        #endregion

        #region [ Mau Events ]

        [MauEvent("selected")]
        public event MauEventHandlerAsync OnSelected;

        [MauEvent("inputChanged")]
        public event MauEventHandlerAsync OnInputChanged;

        [MauEvent("inputFocused")]
        public event MauEventHandlerAsync OnInputFocused;

        [MauEvent("inputCleared")]
        public event MauEventHandlerAsync OnInputCleared;

        [MauEvent("opened")]
        public event MauEventHandlerAsync OnOpened;

        [MauEvent("closed")]
        public event MauEventHandlerAsync OnClosed;

        [MauEvent("scrolledToEnd")]
        public event MauEventHandlerAsync OnScrolledToEnd;

        [MauEvent("input")]
        public event MauEventHandlerAsync OnInputChange;

        #endregion

        #region [ Mau Properties ]

        [MauProperty("disabled", MauPropertyType.ComponentProperty, ForceSet = true)]
        public bool Disabled { get; set; }

        [MauProperty("placeholder", MauPropertyType.ComponentProperty)]
        public string Placeholder { get; set; }

        [MauProperty("query", MauPropertyType.ComponentProperty)]
        public string Query { get; set; }

        [MauProperty("selectedIdx", MauPropertyType.ComponentProperty)]
        public int SelectedIdx { get; set; }

        #endregion

        #region [ Mau Methods ]

        [MauMethod("open", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Open() { }

        [MauMethod("close", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Close() { }

        [MauMethod("focus", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Focus() { }

        [MauMethod("clear", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Clear() { }

        #endregion

        public MauAutocomplete(string mauId) : base(mauId)
        {
            Data = new MauDataList<string>(this, nameof(Data));
        }
    }
}

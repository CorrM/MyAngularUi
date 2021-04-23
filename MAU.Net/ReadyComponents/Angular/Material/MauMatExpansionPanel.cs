using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular.Material
{
    public class MauMatExpansionPanel : MauComponent
    {
        #region [ Mau Events ]

        [MauEvent("afterCollapse")]
        public event MauEventHandlerAsync OnAfterCollapse;

        [MauEvent("afterExpand")]
        public event MauEventHandlerAsync OnAfterExpand;

        [MauEvent("closed")]
        public event MauEventHandlerAsync OnClosed;

        [MauEvent("destroyed")]
        public event MauEventHandlerAsync OnDestroyed;

        [MauEvent("opened")]
        public event MauEventHandlerAsync OnOpened;

        #endregion

        #region [ Mau Properties ]

        [MauProperty("disabled", MauPropertyType.ComponentProperty)]
        public bool Disabled { get; set; }

        [MauProperty("expanded", MauPropertyType.ComponentProperty)]
        public bool Expanded { get; set; }

        [MauProperty("hideToggle", MauPropertyType.ComponentProperty)]
        public bool HideToggle { get; set; }

        [MauProperty("togglePosition", MauPropertyType.ComponentProperty)]
        public MatAccordionTogglePosition TogglePosition { get; set; }

        [MauProperty("id", MauPropertyType.ComponentProperty)]
        public string Id { get; set; }

        #endregion

        #region [ Mau Methods ]

        [MauMethod("close", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Close() { }

        [MauMethod("open", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Open() { }

        [MauMethod("toggle", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Toggle() { }

        #endregion

        public MauMatExpansionPanel(string mauId) : base(mauId) { }
    }
}

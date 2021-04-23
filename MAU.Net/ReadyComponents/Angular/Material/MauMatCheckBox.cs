using MAU.Attributes;
using MAU.Core;
using MAU.Helper.Enums;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;
using static MAU.Events.MauEventHandlers;

namespace MAU.ReadyComponents.Angular.Material
{
    public class MauMatCheckBox : MauComponent
    {
        #region [ Mau Events ]

        [MauEvent("change")]
        public event MauEventHandlerAsync OnChange;

        [MauEvent("indeterminateChange")]
        public event MauEventHandlerAsync OnIndeterminateChange;

        #endregion

        #region [ Mau Properties ]

        [MauProperty("checked", MauPropertyType.ComponentProperty)]
        public bool Checked { get; set; }

        [MauProperty("color", MauPropertyType.ComponentProperty)]
        public ThemePalette Color { get; set; }

        [MauProperty("disableRipple", MauPropertyType.ComponentProperty)]
        public bool DisableRipple { get; set; }

        [MauProperty("disabled", MauPropertyType.ComponentProperty)]
        public bool Disabled { get; set; }

        [MauProperty("id", MauPropertyType.ComponentProperty)]
        public string Id { get; set; }

        [MauProperty("indeterminate", MauPropertyType.ComponentProperty)]
        public bool Indeterminate { get; set; }

        [MauProperty("labelPosition", MauPropertyType.ComponentProperty)]
        public CheckBoxLabelPosition LabelPosition { get; set; }

        [MauProperty("name", MauPropertyType.ComponentProperty)]
        public string Name { get; set; }

        [MauProperty("required", MauPropertyType.ComponentProperty)]
        public bool Required { get; set; }

        [MauProperty("value", MauPropertyType.ComponentProperty)]
        public string Value { get; set; }

        [MauProperty("inputId", MauPropertyType.ComponentProperty, PropStatus = MauPropertyStatus.ReadOnly)]
        public string InputId { get; set; }

        #endregion

        #region [ Mau Methods ]

        [MauMethod("focus", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Focus() { }

        [MauMethod("toggle", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
        public void Toggle() { }

        #endregion

        public MauMatCheckBox(string mauId) : base(mauId) { }
    }
}

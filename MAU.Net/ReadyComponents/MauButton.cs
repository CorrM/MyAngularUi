using MAU.Attributes;
using MAU.Core;
using static MAU.Attributes.MauMethod;
using static MAU.Attributes.MauProperty;

namespace MAU.ReadyComponents;

public class MauButton : MauComponent
{
    #region [ Mau Properties ]

    [MauProperty("disabled", MauPropertyType.NativeProperty)]
    public bool Disabled { get; set; }

    #endregion

    #region [ Mau Methods ]

    [MauMethod("focus", MauMethodType.ComponentMethod, MauMethodCallType.ExecuteInAngular)]
    public void Focus() { }

    #endregion

    public MauButton(string mauId) : base(mauId) { }
}
using System;
using System.Reflection;
using MAU.Core;
using MAU.Helper.Holders;
using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace MAU.Attributes;

[AttributeUsage(AttributeTargets.Property)]
[MulticastAttributeUsage(MulticastTargets.Property, PersistMetaData = true)]
[PSerializable]
public sealed class MauProperty : LocationInterceptionAspect
{
    public enum MauPropertyType
    {
        NativeAttribute = 0,
        NativeProperty = 1,
        ComponentProperty = 2
    }
    public enum MauPropertyStatus
    {
        None = 0,
        Normal = 1,
        ReadOnly = 2,
        SetOnly = 3
    }

    public string PropertyName { get; private set; }
    public MauPropertyType PropType { get; private set; }
    public MauPropertyStatus PropStatus { get; set; }
    public bool Important { get; set; }

    /// <summary>
    /// Set property value even not exists
    /// </summary>
    public bool ForceSet { get; set; }

    public MauProperty(string propertyName, MauPropertyType propType)
    {
        PropertyName = propertyName;
        PropType = propType;
    }

    public static bool HasAttribute(PropertyInfo propertyInfo) => IsDefined(propertyInfo, typeof(MauProperty));

    internal static void SendMauProp(MauComponent holder, string mauPropName)
    {
        MauPropertyHolder mauPropHolder = holder.GetMauPropHolder(mauPropName);
        Type propType = holder.HandledProps[mauPropName].Holder.PropertyType;
        object propValue = holder.HandledProps[mauPropName].Holder.GetValue(holder);

        // bypass is for props not yet changed from .Net side
        // so it's just to not override angular prop value
        bool bypass = false;

        if (!mauPropHolder.Touched)
        {
            bypass = true;
        }
        else if (mauPropHolder.PropAttr.PropStatus == MauPropertyStatus.ReadOnly)
        {
            bypass = true;
        }
        else if (propType.IsEnum)
        {
            if (!MauEnumMemberAttribute.HasNotSetValue(propValue.GetType()))
                throw new Exception($"NotSet must to be in any MauProperty value is 'Enum', {propValue.GetType().FullName}");

            if (MauEnumMemberAttribute.HasAttribute((Enum)propValue))
                propValue = MauEnumMemberAttribute.GetValue((Enum)propValue);

            // If it's NotSet just ignore so the angular value will be set,
            // Angular value will be in .Net side, so the value will be correct here.
            // E.g: Color prop if it's NotSet in .Net then use and don't change
            // Angular value.
            switch (propValue)
            {
                case int:
                case long and 0:
                case string propValStr when string.IsNullOrWhiteSpace(propValStr):
                    bypass = true;
                    break;
            }
        }
        else if (propValue is null && propType == typeof(string))
        {
            // null not same as empty string
            bypass = true;
        }

        // Don't send .Net value and ask angular for it's value
        if (bypass)
        {
            holder.RequestPropValue(mauPropName);
            return;
        }

        var data = new JObject
        {
            {"propType", (int)mauPropHolder.PropAttr.PropType},
            {"propStatus", (int)mauPropHolder.PropAttr.PropStatus},
            {"propForce", mauPropHolder.PropAttr.ForceSet},
            {"propName", mauPropName},
            {"propVal", MyAngularUi.ParseMauDataToFrontEnd(propType, propValue)}
        };

        MyAngularUi.SendRequestAsync(holder.MauId, RequestType.SetPropValue, data);
    }

    public override void OnSetValue(LocationInterceptionArgs args)
    {
        var holder = (MauComponent)args.Instance;
        MauPropertyHolder mauPropHolder = holder.GetMauPropHolder(PropertyName);
        object curValue = args.GetCurrentValue();

        // Mark this property as it's changed by .Net
        mauPropHolder.Touched = true;

        // Set value first, so i can reflect it
        base.OnSetValue(args);

        // Todo: make that 'callBackMethod' an global var 
        mauPropHolder.SetCallBackMethod ??= holder.GetType().GetMethod($"{args.LocationName}OnSet", BindingFlags.NonPublic | BindingFlags.Static);
        if (mauPropHolder.SetCallBackMethod is not null && args.Value?.Equals(curValue) == false)
            mauPropHolder.SetCallBackMethod.Invoke(null, new object[] { holder });

        // HandleOnSet .?
        if (!mauPropHolder.HandleOnSet)
            return;

        // if it's set from angular side then will not hit this part
        // because of 'HandleOnSet' will be 'false'
        if (PropStatus == MauPropertyStatus.ReadOnly)
            throw new Exception($"This prop '{holder.MauId}.{PropertyName}' is 'ReadOnly'.");

        SendMauProp(holder, PropertyName);
    }
}
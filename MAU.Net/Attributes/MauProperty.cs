using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using MAU.Core;

namespace MAU.Attributes
{
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
			Normal = 0,
			ReadOnly = 1
		}

		public string PropertyName { get; private set; }
		public MauPropertyType PropType { get; private set; }
		public MauPropertyStatus PropStatus { get; set; } = MauPropertyStatus.Normal;
		public bool Important { get; set; }

		public MauProperty(string propertyName, MauPropertyType propType)
		{
			PropertyName = propertyName;
			PropType = propType;
		}

		public static bool HasAttribute(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttributes<MauProperty>(false).Any();
		}
		internal static MyAngularUi.RequestState SendMauProp(MauComponent holder, string mauPropName)
		{
			MauProperty mauProp = holder.GetMauPropAttribute(mauPropName);
			Type propType = holder.HandledProps[mauPropName].Holder.PropertyType;
			object propValue = holder.HandledProps[mauPropName].Holder.GetValue(holder);
			bool bypass = false;

			if (mauProp.PropStatus == MauPropertyStatus.ReadOnly)
			{
				bypass = true;
			}
			else if (propType.IsEnum)
			{
				if (!MauEnumMember.HasNotSetValue(propValue.GetType()))
					throw new Exception($"NotSet must to be in any MauProperty value is 'Enum', {propValue.GetType().FullName}");

				if (MauEnumMember.HasAttribute((Enum)propValue))
					propValue = MauEnumMember.GetValue((Enum)propValue);

				// If it's NotSet just ignore so the angular value will be set,
				// Angular value will be in .Net side, so the value will be correct here.
				// E.g: Color prop if it's NotSet in .Net then use and don't change
				// Angular value.
				switch (propValue)
				{
					case int _:
					case long propValNum when propValNum == 0:
					case string propValStr when string.IsNullOrWhiteSpace(propValStr):
						bypass = true;
						break;
				}
			}
			else if (propValue == null && propType == typeof(string))
			{
				// null not same as empty string
				bypass = true;
			}

			if (bypass)
			{
				holder.RequestPropValue(mauPropName);
				return default;
			}

			var data = new JObject
			{
				{"propType", (int)mauProp.PropType},
				{"propStatus", (int)mauProp.PropStatus},
				{"propName", mauPropName},
				{"propVal", MyAngularUi.ParseMauDataToFrontEnd(propType, propValue)}
			};

			return MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetPropValue, data);
		}

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			var holder = (MauComponent)args.Instance;
			object curValue = args.GetCurrentValue();

			// Set value first, so i can reflect it
			base.OnSetValue(args);

			// 
			MethodInfo callBackMethod = holder.GetType().GetMethod($"{args.LocationName}OnSet", BindingFlags.NonPublic | BindingFlags.Static);
			if (callBackMethod != null && args.Value != null && !args.Value.Equals(curValue))
				callBackMethod.Invoke(null, new object[] { holder });

			if (!holder.HandledProps[PropertyName].Value) // HandleOnSet .?
				return;

			// if it's set from angular side then will not hit this part
			// because of 'HandleOnSet' will be 'false'
			if (PropStatus == MauPropertyStatus.ReadOnly)
				throw new Exception($"This prop '{holder.MauId}.{PropertyName}' is 'ReadOnly'.");

			SendMauProp(holder, PropertyName);
		}
	}
}

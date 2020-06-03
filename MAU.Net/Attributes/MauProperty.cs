using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

		public string PropertyName { get; private set; }
		public MauPropertyType PropType { get; private set; }
		public bool Important { get; set; }
		public bool ReadOnly { get; set; }

		public MauProperty(string propertyName, MauPropertyType propType)
		{
			PropertyName = propertyName;
			PropType = propType;
		}

		public static bool HasAttribute(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttributes<MauProperty>(false).Any();
		}
		internal static async Task<MyAngularUi.RequestState> SendMauProp(MauComponent holder, string mauPropName)
		{
			MauProperty mauProp = holder.GetMauPropAttribute(mauPropName);
			Type propType = holder.HandledProps[mauPropName].Holder.PropertyType;
			object propValue = holder.HandledProps[mauPropName].Holder.GetValue(holder);

			if (propType.IsEnum)
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
						await holder.GetPropValue(mauPropName);
						return default;
				}
			}
			else if (propValue == null && propType == typeof(string))
			{
				propValue = string.Empty;
			}

			var data = new JObject
			{
				{"propType", (int)mauProp.PropType},
				{"propName", mauPropName},
				{"propVal", MyAngularUi.ParseMauDataToFrontEnd(propValue)}
			};

			return await MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetPropValue, data);
		}

		public override async void OnSetValue(LocationInterceptionArgs args)
		{
			// Set value first, so i can reflect it
			base.OnSetValue(args);

			if (!MyAngularUi.Connected)
				return;

			var holder = (MauComponent)args.Instance;
			if (!holder.HandledProps[PropertyName].Value) // HandleOnSet .?
				return;

			// if it's set from angular side then will not hit this part
			// because of 'HandleOnSet' will be 'false'
			if (ReadOnly)
				throw new Exception($"This prop '{holder.MauId}.{PropertyName}' is 'ReadOnly'.");

			await SendMauProp(holder, PropertyName);
		}
	}
}

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

		public MauProperty(string propertyName, MauPropertyType propType)
		{
			PropertyName = propertyName;
			PropType = propType;
		}

		public static bool HasAttribute(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttributes<MauProperty>(false).Any();
		}
		internal static async Task<MyAngularUi.RequestState> SendMauProp(MauElement holder, string mauPropName, object mauPropValue, MauPropertyType mauPropType)
		{
			object value = mauPropValue;
			if (value.GetType().IsEnum)
			{
				if (!MauEnumMember.HasNotSetValue(value.GetType()))
					throw new Exception($"NotSet must to be in any MauProperty value is 'Enum', {value.GetType().FullName}");

				if (MauEnumMember.HasAttribute((Enum)value))
					value = MauEnumMember.GetValue((Enum)value);
			}

			var data = new JObject
			{
				{"propType", (int)mauPropType},
				{"propName", mauPropName},
				{"propVal", MyAngularUi.ParseMauDataToFrontEnd(value)}
			};

			return await MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetPropValue, data);
		}

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			// Set value first, so i can reflect it
			base.OnSetValue(args);

			if (!MyAngularUi.Connected)
				return;

			var holder = (MauElement)args.Instance;
			if (!holder.HandleOnSet)
			{
				base.OnSetValue(args);
				return;
			}

			_ = SendMauProp(holder, PropertyName, args.Value, PropType);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			base.OnSetValue(args);

			var holder = (MauElement)args.Instance;
			object value = args.Value;
			if (!holder.HandleOnSet)
			{
				base.OnSetValue(args);
				return;
			}

			if (args.Value.GetType().IsEnum)
			{
				if (!MauEnumMember.HasNotSetValue(args.Value.GetType()))
					throw new Exception($"NoSet must to be in any MauProperty value is 'Enum', {args.Value.GetType().FullName}");

				if (MauEnumMember.HasAttribute((Enum)args.Value))
					value = MauEnumMember.GetValue((Enum)args.Value);
			}

			var data = new JObject
			{
				{"propType", (int)PropType},
				{"propName", PropertyName},
				//{"propVal", (dynamic)value}
				{"propVal", MyAngularUi.ParseMauDataToFrontEnd(value)}
			};

			_ = MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetPropValue, data);
		}
	}
}

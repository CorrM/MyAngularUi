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

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	[MulticastAttributeUsage(PersistMetaData = true)]
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

		public MauProperty(string propertyName, MauPropertyType propType = MauPropertyType.NativeAttribute)
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
			var holder = (MauElement)args.Instance;

			if (!holder.HandleOnSet)
			{
				base.OnSetValue(args);
				return;
			}

			var data = new JObject
			{
				{"propType", (int)PropType},
				{"propName", PropertyName},
				{"propVal", JsonConvert.SerializeObject(args.Value)}
			};

			_ = MyAngularUi.SendRequest(holder.Id, MyAngularUi.RequestType.SetPropValue, data);
			base.OnSetValue(args);
		}

	}
}

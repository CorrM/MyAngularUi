using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
		public string PropertyName { get; private set; }
		public bool IsAttribute { get; private set; }

		public MauProperty(string propertyName, bool isAttribute = true)
		{
			PropertyName = propertyName;
			IsAttribute = isAttribute;
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
				{"propIsAttr", IsAttribute},
				{"propName", PropertyName},
				{"propVal", args.Value.ToString()}
			};

			_ = MyAngularUi.SendRequest(holder.Id, MyAngularUi.RequestType.SetPropValue, data);
			base.OnSetValue(args);
		}

	}
}

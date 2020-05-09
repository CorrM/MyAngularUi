using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MAU.Core;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	[MulticastAttributeUsage(MulticastTargets.Property, PersistMetaData = true)]
	[PSerializable]
	public sealed class MauVariable : LocationInterceptionAspect
	{
		public static bool HasAttribute(FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes(typeof(MauVariable), false).Any();
		}
		public static bool HasAttribute(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttributes(typeof(MauVariable), false).Any();
		}

		private static void UpdateVarBase(object mauComponentElement, string mauVarName)
		{
			PropertyInfo pInfo = mauComponentElement.GetType().GetProperty(mauVarName);
			if (pInfo == null)
				throw new ArgumentException("Variable not found", nameof(mauVarName));
			if (!HasAttribute(pInfo))
				throw new ArgumentException("Variable not 'MauVariable'", nameof(mauVarName));

			// Get Holder Component
			string varPrefix;
			if (mauComponentElement.GetType().IsSubclassOf(typeof(MauElement)))
				varPrefix = ((MauElement)mauComponentElement).MauId;
			else if (mauComponentElement.GetType().IsSubclassOf(typeof(MauElement)))
				varPrefix = ((MauComponent)mauComponentElement).ComponentName;
			else
				throw new Exception("'MauVariable' Only valid on ('MauElement', 'MauComponent')");

			// Ui not register yet (RegisterComponent function not called)
			if (string.IsNullOrWhiteSpace(varPrefix))
				return;

			// Get Data
			var data = new JObject
			{
				{ "varName", $"{varPrefix}_{mauVarName}" },
				{ "varValue", MyAngularUi.ParseMauData(pInfo.PropertyType, pInfo.GetValue(mauComponentElement)) }
			};

			// Send Data
			_ = MyAngularUi.SendRequest("null", MyAngularUi.RequestType.SetVarValue, data);
		}
		public static void UpdateVar(MauComponent mauComponent, string mauVarName)
		{
			UpdateVarBase(mauComponent, mauVarName);
		}
		public static void UpdateVar(MauElement mauElement, string mauVarName)
		{
			UpdateVarBase(mauElement, mauVarName);
		}

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			// Set value first, so i can reflect it in `UpdateVarBase`
			base.OnSetValue(args);
			UpdateVarBase(args.Instance, args.LocationName);
		}
	}
}

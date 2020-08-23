using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
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
        public string VariableName { get; set; }

		public MauVariable(string variableName)
        {
            VariableName = variableName;

        }

        public static bool HasAttribute(FieldInfo fieldInfo)
		{
			return fieldInfo.GetCustomAttributes(typeof(MauVariable), false).Any();
		}
		public static bool HasAttribute(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttributes(typeof(MauVariable), false).Any();
		}

		internal static MyAngularUi.RequestState SendMauVariable(MauComponent holder, string mauVarName)
		{
			PropertyInfo pInfo = holder.GetType().GetProperty(mauVarName);
			if (pInfo == null)
				throw new ArgumentException("Variable not found", nameof(mauVarName));
			if (!HasAttribute(pInfo))
				throw new ArgumentException("Variable not 'MauVariable'", nameof(mauVarName));

			// Ui not register yet (RegisterComponent function not called)
			if (string.IsNullOrWhiteSpace(holder.MauId))
				return default;

			// Get Data
			var data = new JObject
			{
				{ "varName", mauVarName },
				{ "varValue", MyAngularUi.ParseMauDataToFrontEnd(pInfo.PropertyType, pInfo.GetValue(holder)) }
			};

			return MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetVarValue, data);
		}
		public static void UpdateVar(MauComponent mauComponent, string mauVarName)
		{
			SendMauVariable(mauComponent, mauVarName);
		}

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			// Set value first, so i can reflect it in `UpdateVarBase`
			base.OnSetValue(args);
			SendMauVariable((MauComponent)args.Instance, VariableName /*args.LocationName*/);
		}
	}
}

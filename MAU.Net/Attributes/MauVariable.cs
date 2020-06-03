using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MAU.Core;
using System.Threading.Tasks;

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

		internal static async Task<MyAngularUi.RequestState> SendMauVariable(MauComponent holder, string mauVarName)
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

			return await MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.SetVarValue, data);
		}
		public static async Task UpdateVar(MauComponent mauComponent, string mauVarName)
		{
			await SendMauVariable(mauComponent, mauVarName);
		}

		public override void OnSetValue(LocationInterceptionArgs args)
		{
			// Set value first, so i can reflect it in `UpdateVarBase`
			base.OnSetValue(args);
			SendMauVariable((MauComponent)args.Instance, args.LocationName).GetAwaiter().GetResult();
		}
	}
}

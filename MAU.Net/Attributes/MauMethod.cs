using MAU.Core;
using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAU.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[MulticastAttributeUsage(MulticastTargets.Method, PersistMetaData = true)]
	[PSerializable]
	public sealed class MauMethod : OnMethodBoundaryAspect
	{
		public enum MauMethodType
		{
			NativeMethod = 0,
			ComponentMethod = 1
		}

		public string MethodName { get; private set; }
		public MauMethodType MethodType { get; private set; }

		public MauMethod(string methodName, MauMethodType methodType)
		{
			MethodName = methodName;
			MethodType = methodType;
		}

		public static bool HasAttribute(MethodInfo methodInfo)
		{
			return methodInfo.GetCustomAttributes<MauMethod>(false).Any();
		}

		public override void OnEntry(MethodExecutionArgs args)
		{
			var holder = (MauElement)args.Instance;

			if (!MyAngularUi.IsMauRegistered(holder.MauId))
				throw new Exception("Register MauElement first. And don't call methods before register the MauElement.");

			base.OnEntry(args);
		}
		public override void OnExit(MethodExecutionArgs args)
		{
			var holder = (MauElement)args.Instance;

			var data = new JObject
			{
				{"methodType", (int)MethodType},
				{"methodName", MethodName},
				{"methodArgs", JArray.FromObject(args.Arguments.ToArray())}
			};

			_ = MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.CallMethod, data);

			if (((MethodInfo) args.Method).ReturnType == typeof(void))
				return;

			args.ReturnValue = holder.GetMethodRetValue(MethodName);
		}
	}
}

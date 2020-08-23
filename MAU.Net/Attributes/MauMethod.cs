using MAU.Core;
using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;
using System;
using System.Linq;
using System.Reflection;

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
			if (MyAngularUi.Connected)
			{
				var holder = (MauComponent)args.Instance;

				if (!MyAngularUi.IsComponentRegistered(holder.MauId))
					throw new Exception("Register MauComponent first. And don't call methods before register the MauComponent.");
			}

			base.OnEntry(args);
		}
		public override void OnExit(MethodExecutionArgs args)
		{
			if (!MyAngularUi.Connected)
			{
				base.OnExit(args);
				return;
			}

			var holder = (MauComponent)args.Instance;
			var data = new JObject
			{
				{"methodType", (int)MethodType},
				{"methodName", MethodName},
				{"methodArgs", JArray.FromObject(args.Arguments.ToArray())}
			};

			MyAngularUi.RequestState request = MyAngularUi.SendRequest(holder.MauId, MyAngularUi.RequestType.CallMethod, data);

			// If its void function then just wait until execution finish
            if (((MethodInfo) args.Method).ReturnType == typeof(void))
            {
				// Wait function
                holder.GetMethodRetValue(request.RequestId);
				return;
			}

			// Set return value of function
			args.ReturnValue = holder.GetMethodRetValue(request.RequestId);
		}
	}
}

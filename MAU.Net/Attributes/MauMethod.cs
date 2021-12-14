using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MAU.Core;
using Newtonsoft.Json.Linq;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using PostSharp.Serialization;

namespace MAU.Attributes;
// Todo: WebSocket case MemoryLeak List<byte> size not reset
// https://github.com/statianzo/Fleck/blob/d7634519dc872a95a9c28bde8d8cb20241890ef2/src/Fleck/ReadState.cs#L10
// https://github.com/statianzo/Fleck/blob/d7634519dc872a95a9c28bde8d8cb20241890ef2/src/Fleck/Handlers/ComposableHandler.cs#L11

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
    public enum MauMethodCallType
    {
        ExecuteInAngular,
        ExecuteFromAngular
    }

    public string MethodName { get; private set; }
    public MauMethodType MethodType { get; private set; }
    public MauMethodCallType MethodCallType { get; private set; }

    public static bool HasAttribute(MethodInfo methodInfo) => Attribute.IsDefined(methodInfo, typeof(MauMethod));

    public MauMethod(string methodName, MauMethodType methodType, MauMethodCallType methodCallType)
    {
        MethodName = methodName;
        MethodType = methodType;
        MethodCallType = methodCallType;
    }

    public override void OnEntry(MethodExecutionArgs args)
    {
        if (MyAngularUi.IsConnected)
        {
            var holder = (MauComponent)args.Instance;

            if (!MyAngularUi.IsComponentRegistered(holder.MauId))
                throw new Exception("Register MauComponent first. And don't call methods before register the MauComponent.");
        }

        base.OnEntry(args);
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        Type retType = ((MethodInfo)args.Method).ReturnType;

        if (MethodCallType == MauMethodCallType.ExecuteFromAngular || !MyAngularUi.IsConnected)
        {
            base.OnExit(args);
            return;
        }

        // Prepare Args
        List<object> argsToSend = args.Arguments.ToList();
        for (int i = 0; i < argsToSend.Count; i++)
        {
            object param = argsToSend[i];
            if (param?.GetType().IsEnum != true || !MauEnumMemberAttribute.HasAttribute((Enum)param))
                continue;

            argsToSend[i] = MauEnumMemberAttribute.GetValue((Enum)param);
        }

        // Send
        var holder = (MauComponent)args.Instance;
        var data = new JObject
        {
            {"methodType", (int)MethodType},
            {"methodName", MethodName},
            {"methodArgs", JArray.FromObject(argsToSend)}
        };

        MyAngularUi.RequestState request = MyAngularUi.SendRequestAsync(holder.MauId, MyAngularUi.RequestType.CallMethod, data).GetAwaiter().GetResult();

        // Wait return
        // If its void function then just wait until execution finish
        if (retType == typeof(void))
        {
            // Wait function
            holder.GetMethodRetValue(request.RequestId);
            return;
        }

        // Wait and set return value of function
        object ret = holder.GetMethodRetValue(request.RequestId);
        if (ret is not null)
            args.ReturnValue = ret /*?? Activator.CreateInstance(retType)*/;
    }
}
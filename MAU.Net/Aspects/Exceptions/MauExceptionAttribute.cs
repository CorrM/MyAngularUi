using PostSharp.Aspects;
using PostSharp.Serialization;

namespace MAU.Aspects.Exceptions
{
    [PSerializable]
    internal class MauExceptionAttribute : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            MyAngularUi.RaiseException(args.Exception);
        }
    }
}

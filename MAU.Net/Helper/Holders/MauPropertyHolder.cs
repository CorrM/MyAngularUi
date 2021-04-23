using System;
using System.Reflection;
using MAU.Attributes;

namespace MAU.Helper.Holders
{
    public class MauPropertyHolder
    {
        public PropertyInfo Holder { get; }
        public MauProperty PropAttr { get; }
        public MethodInfo SetCallBackMethod { get; set; }
        public bool HandleOnSet { get; private set; }
        public bool Touched { get; set; }

        public MauPropertyHolder(PropertyInfo holder, bool handleOnSet)
        {
            Holder = holder;
            HandleOnSet = handleOnSet;
            PropAttr = Holder.GetCustomAttribute<MauProperty>(false);
        }

        public void DoWithoutHandling(Action<MauPropertyHolder> body)
        {
            HandleOnSet = false;
            body?.Invoke(this);
            HandleOnSet = true;
        }
    }
}
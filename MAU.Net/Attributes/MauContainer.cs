using System;

namespace MAU.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MauContainerAttribute : Attribute
{
    public static bool HasAttribute(Type type) => IsDefined(type, typeof(MauContainerAttribute));
}
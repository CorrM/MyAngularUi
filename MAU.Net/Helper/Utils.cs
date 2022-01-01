using System;

namespace MAU.Helper;

public static class Utils
{
    private static readonly Random _randomizer = new();

    public static int RandomInt(int min, int max)
    {
        return _randomizer.Next(min, max);
    }
    public static bool IsIEnumerable(Type typeToCheck)
    {
        return typeToCheck != typeof(string) &&
               typeToCheck.GetInterface("IEnumerable`1") is not null;
    }
}
using System;
using System.Diagnostics;

namespace MAU.Helper
{
	[DebuggerDisplay("{" + nameof(Value) + "}")]
	public class BoolHolder<T>
	{
		public T Holder { get; set; }
		public bool Value { get; set; }

		public BoolHolder(T holder, bool value)
		{
			Holder = holder;
			Value = value;
		}
	}

	public static class Utils
	{
		private static readonly Random _randomizer = new Random();

		public static int RandomInt(int min, int max)
		{
			return _randomizer.Next(min, max);
		}
		public static bool IsIEnumerable(Type typeToCheck)
		{
			return typeToCheck != typeof(string) &&
				typeToCheck.GetInterface("IEnumerable`1") != null;
		}
	}
}

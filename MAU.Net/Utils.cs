using MAU.Attributes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MAU
{
	public static class Utils
	{
		private static readonly Random Randomizer = new Random();

		public static int RandomInt(int min, int max)
		{
			return Randomizer.Next(min, max);
		}
		public static bool IsIEnumerable(Type typeToCheck)
		{
			return typeToCheck != typeof(string) &&
				typeToCheck.GetInterface("IEnumerable`1") != null;
		}
	}
}

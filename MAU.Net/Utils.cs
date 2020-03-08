using System;
using System.Collections.Generic;
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
	}
}

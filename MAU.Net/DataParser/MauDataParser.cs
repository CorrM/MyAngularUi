using System;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public abstract class MauDataParser<TVar>
	{
		public Type TargetType { get; } = typeof(TVar);

		public abstract JToken Parse(TVar varObj);
	}
}

using System;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public abstract class MauDataParser<TVar>
	{
		public Type TargetType { get; } = typeof(TVar);

		public abstract JToken ParseToFrontEnd(TVar varObj);
		public abstract TVar ParseFromFrontEnd(JToken varObj);
	}
}

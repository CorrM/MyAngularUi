using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MAU.Variables
{
	public abstract class MauVarParser<TVar>
	{
		public Type TargetType { get; } = typeof(TVar);

		public abstract string Parse(TVar varObj);
	}
}

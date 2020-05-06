using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MAU.Variables
{
	public class DefaultParser : MauVarParser<object>
	{
		public override string Parse(object varObj)
		{
			Type varType = varObj.GetType();
			return Utils.IsIEnumerable(varType)
				? JArray.FromObject(varObj).ToString()
				: JObject.FromObject(varObj).ToString();
		}
	}
}

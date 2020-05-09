using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class DefaultParser : MauDataParser<object>
	{
		public override JToken Parse(object varObj)
		{
			Type varType = varObj.GetType();

			if (Utils.IsIEnumerable(varType))
				return JArray.FromObject(varObj);

			try
			{
				return JObject.FromObject(varObj);
			}
			catch (ArgumentException)
			{
				return JToken.FromObject(varObj);
			}
		}
	}
}

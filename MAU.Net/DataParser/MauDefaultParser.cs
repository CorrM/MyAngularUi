using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class MauDefaultParser : MauDataParser<object>
	{
		public override JToken Parse(object varObj)
		{
			Type varType = varObj.GetType();

			if (Utils.IsIEnumerable(varType))
				return JArray.FromObject(varObj);

			try
			{
				if (varType != typeof(string) && varType != typeof(bool))
					return JObject.FromObject(varObj);
			}
			catch
			{
				// ignored
			}

			return JToken.FromObject(varObj);
		}
	}
}

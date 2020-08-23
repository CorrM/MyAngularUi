using System;
using MAU.Helper.Types;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class MauJObjectParser : MauDataParser<JObject>
	{
		public override JToken ParseToFrontEnd(Type varType, JObject varObj)
		{
			return varObj;
		}

		public override JObject ParseFromFrontEnd(JToken varObj)
		{
			if (!varObj.HasValues)
				return new JObject();

			try
			{
				return JObject.Parse(varObj.ToString());
			}
			catch
			{
				return new JObject();
			}
		}
	}
}

using System;
using MAU.Helper.Types;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class MauJArrayParser : MauDataParser<JArray>
	{
		public override JToken ParseToFrontEnd(Type varType, JArray varObj)
		{
			return varObj;
		}

		public override JArray ParseFromFrontEnd(JToken varObj)
		{
			if (!varObj.HasValues)
				return new JArray();

			try
			{
				return JArray.Parse(varObj.ToString());
			}
			catch (Exception ex)
			{
				return new JArray();
			}
		}
	}
}

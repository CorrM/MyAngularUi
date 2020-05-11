using System;
using MAU.Helper.Types;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class MauDataTableParser : MauDataParser<MauDataTable>
	{
		public override JToken ParseToFrontEnd(MauDataTable varObj)
		{
			var retVal = new JObject()
			{
				{ "Columns", JArray.FromObject(varObj.Columns) },
				{ "Rows", JArray.FromObject(varObj.Rows) }
			};

			return retVal;
		}

		public override MauDataTable ParseFromFrontEnd(JToken varObj)
		{
			throw new NotImplementedException();
		}
	}
}

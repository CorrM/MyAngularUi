using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class DataTableParser : MauDataParser<DataTable>
	{
		public override JToken Parse(DataTable varObj)
		{
			Console.WriteLine("GGGGGGGGGGGGG");
			return JToken.FromObject("GGGG");
		}
	}
}

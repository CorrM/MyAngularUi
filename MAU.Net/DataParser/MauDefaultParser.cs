using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class MauDefaultParser : MauDataParser<object>
	{
		public override JToken ParseToFrontEnd(object varObj)
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

		public override object ParseFromFrontEnd(JToken varObj)
		{
			object retVar;
			switch (varObj.Type)
			{
				case JTokenType.Null:
					retVar = null;
					break;
				case JTokenType.Object:
					retVar = varObj.ToString();
					break;
				case JTokenType.Array:
					retVar = varObj.Values<string>().ToList();
					break;
				case JTokenType.Integer:
					retVar = varObj.Value<int>();
					break;
				case JTokenType.Float:
					retVar = varObj.Value<float>();
					break;
				case JTokenType.String:
					retVar = varObj.Value<string>();
					break;
				case JTokenType.Boolean:
					retVar = varObj.Value<bool>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return retVar;
		}
	}
}

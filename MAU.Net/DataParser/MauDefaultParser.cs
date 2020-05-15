using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser
{
	public class MauDefaultParser : MauDataParser<object>
	{
		private static object GetJsonArray(JToken array)
		{
			JTokenType? arrayType = array.Children().FirstOrDefault()?.Type;

			if (arrayType == null)
				return array.Values<string>().ToList();

			object retVar;
			switch (arrayType)
			{
				case JTokenType.Object:
					retVar = array.Values<JObject>().ToList();
					break;
				case JTokenType.Integer:
					retVar = array.Values<int>().ToList();
					break;
				case JTokenType.Float:
					retVar = array.Values<float>().ToList();
					break;
				case JTokenType.String:
					retVar = array.Values<string>().ToList();
					break;
				case JTokenType.Boolean:
					retVar = array.Values<bool>().ToList();
					break;
				default:
					retVar = array.Values<string>().ToList();
					break;
			}

			return retVar;
		}

		public override JToken ParseToFrontEnd(object varObj)
		{
			Type varType = varObj.GetType();

			if (Utils.IsIEnumerable(varType))
				return JArray.FromObject(varObj);

			try
			{
				if (varType != typeof(string) && varType != typeof(bool) && varType != typeof(int) && varType != typeof(long))
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
					retVar = GetJsonArray(varObj);
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

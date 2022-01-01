using System;
using System.Linq;
using MAU.Helper;
using Newtonsoft.Json.Linq;

namespace MAU.DataParser;

public class MauDefaultParser : MauDataParser<object>
{
    private static object GetJsonArray(JToken array)
    {
        JTokenType? arrayType = array.Children().FirstOrDefault()?.Type;

        if (arrayType is null)
            return array.Values<string>().ToList();

        object retVar = arrayType switch
        {
            JTokenType.Object => array.Values<JObject>().ToList(),
            JTokenType.Integer => array.Values<int>().ToList(),
            JTokenType.Float => array.Values<float>().ToList(),
            JTokenType.String => array.Values<string>().ToList(),
            JTokenType.Boolean => array.Values<bool>().ToList(),
            _ => array.Values<string>().ToList()
        };

        return retVar;
    }

    public override JToken ParseToFrontEnd(Type varType, object varObj)
    {
        if (varObj is null)
            return null;

        // ToDo: Try to get data in IEnumerable and pass it to parser
        if (Utils.IsIEnumerable(varType) || varType.IsArray)
            return JArray.FromObject(varObj);

        /*
        try
        {
            if (varType != typeof(string) && varType != typeof(bool) && varType != typeof(int) && varType != typeof(long))
                return JObject.FromObject(varObj);
        }
        catch
        {
            // ignored
        }
        */

        return JToken.FromObject(varObj);
    }
    public override object ParseFromFrontEnd(JToken varObj)
    {
        if (varObj is null)
            return null;

        object retVar = varObj.Type switch
        {
            JTokenType.Null => null,
            JTokenType.Object => varObj.ToString(),
            JTokenType.Array => GetJsonArray(varObj),
            JTokenType.Integer => varObj.Value<int>(),
            JTokenType.Float => varObj.Value<float>(),
            JTokenType.String => varObj.Value<string>(),
            JTokenType.Boolean => varObj.Value<bool>(),
            _ => throw new ArgumentOutOfRangeException()
        };

        return retVar;
    }
}
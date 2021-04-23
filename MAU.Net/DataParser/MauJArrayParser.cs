using Newtonsoft.Json.Linq;
using System;

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
            catch (Exception)
            {
                return new JArray();
            }
        }
    }
}

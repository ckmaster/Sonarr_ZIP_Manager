using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sonarr_ZIP_Manager
{
    public interface IJsonManager
    {
        JArray DeserializeArray(string responseBody);
        List<string> SelectTokensInTopLevelJArray(JArray jsonArray, string toSelect);
        string SelectStringTokensInTopLevelJObject(JObject jsonObject, string toSelect);
        long SelectIntTokensInTopLevelJObject(JObject jsonObject, string toSelect);
    }

    public class JsonManager : IJsonManager
    {
        JArray IJsonManager.DeserializeArray(string responseBody)
        {
            JArray jsonArray = new JArray();
            try
            {
                jsonArray = JsonConvert.DeserializeObject<JArray>(responseBody);
                return jsonArray;
            }
            catch (JsonReaderException jre)
            {
                Globals.errLogger.CreateOrAppend("An error occurred deserializing JSON\n", jre);
                return jsonArray;
            }
        }

        List<string> IJsonManager.SelectTokensInTopLevelJArray(JArray jsonArray, string toSelect)
        {
            List<string> tokens = new List<string>();
            foreach (JObject jsonObject in jsonArray)
            {
                tokens.Add((string)jsonObject.SelectToken(toSelect));
            }
            return tokens;
        }

        string IJsonManager.SelectStringTokensInTopLevelJObject(JObject jsonObject, string toSelect)
        {
            return ((string)jsonObject.SelectToken(toSelect));
        }

        long IJsonManager.SelectIntTokensInTopLevelJObject(JObject jsonObject, string toSelect)
        {
            return ((long)jsonObject.SelectToken(toSelect));
        }
    }
}

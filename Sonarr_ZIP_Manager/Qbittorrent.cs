using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sonarr_ZIP_Manager
{
    public class Qbittorrent
    {
        private static IWebRequestFactory webReqFac;
        private static IWebResponseFactory webRespFac;
        private static string cookie;

        public Qbittorrent()
        {
            string baseUrl = "http://ckmasterplex:8081/api/v2";
            webReqFac = new WebRequestFactory(baseUrl);
            cookie = Login();
        }

        private static string Login()
        {
            string urlParams = "/auth/login";
            string method = "POST";
            List<string[]> headers = new List<string[]>();
            //headers.Add(new string[] { "Content-type", "application/x-www-form-urlencoded" });
            string contentType = "application/x-www-form-urlencoded";
            string body = "username=admin&password=administrator";
            webRespFac = new WebResponseFactory(webReqFac.BuildRequest(urlParams, method, contentType, headers, body));
            HttpWebResponse response = webRespFac.ExecuteRequest();
            string cookieHolder = response.GetResponseHeader("set-cookie");
            string[] cookieHolderPieces = cookieHolder.Split(new string[] { "; " }, StringSplitOptions.None);
            return cookieHolderPieces[0];
        }

        public Dictionary<string, bool> CheckDownloadStatus(List<string> folders)
        {
            Dictionary<string, bool> downloadFinished = new Dictionary<string, bool>();
            string urlParams = "/torrents/info";
            string method = "POST";
            List<string[]> headers = new List<string[]>();
            headers.Add(new string[] { "Cookie", cookie });
            string contentType = "application/json";
            webRespFac = new WebResponseFactory(webReqFac.BuildRequest(urlParams, method, contentType, headers, ""));
            string responseBody = webRespFac.GetResponseBody(webRespFac.ExecuteRequest());
            IJsonManager jManager = new JsonManager();
            JArray jsonArray = jManager.DeserializeArray(responseBody);
            foreach (JObject jsonObject in jsonArray)
            {
                string name = jManager.SelectStringTokensInTopLevelJObject(jsonObject, "name");
                foreach(string s in folders)
                {
                    if (String.Equals(name, s))
                    {
                        int amount_left = jManager.SelectIntTokensInTopLevelJObject(jsonObject, "amount_left");
                        if (amount_left == 0)
                        {
                            downloadFinished.Add(name, true);
                        }
                        else
                        {
                            downloadFinished.Add(name, false);
                        }
                        break;
                    }

                }
            }
            return downloadFinished;
        }

        public void Logout()
        {
            string urlParams = "/auth/logout";
            string method = "POST";
            string contentType = "application/json";
            List<string[]> headers = new List<string[]>();
            headers.Add(new string[] { "Cookie", cookie });
            webRespFac = new WebResponseFactory(webReqFac.BuildRequest(urlParams, method, contentType, headers, ""));
            webRespFac.ExecuteRequest();
        }
    }
}

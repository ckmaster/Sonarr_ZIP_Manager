﻿using System;
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
    public class Sonarr
    {
        public Sonarr()
        {
         
        }

        public HttpWebRequest QueuedItems()
        {
            IWebRequestFactory webReqFac = new WebRequestFactory("http://ckmasterplex:8989/api");
            string urlParameters = "/queue";
            string method = "GET";
            List<string[]> headers = new List<string[]>();
            headers.Add(new string[] { "Accept", "application/json" });
            headers.Add(new string[] { "X-Api-Key", "e2ddb6b1b3e046bdab2a9a494a5a4d0c" });
            return webReqFac.BuildRequest(urlParameters, method, headers);
        }

        public List<string> GetDownloadFolders(JArray ja)
        {
            IJsonManager jm = new JsonManager();
            return jm.SelectTokensInTopLevelJArray(ja, "title");
        }

        public void PushExtracted(Dictionary<string, bool> downloadFolders)
        {
            string basePath = @"\\\\ckmasterplex\\h$\\Downloads\\";
            //string basePath = @"H:\\Downloads\\";
            IWebRequestFactory webReqFac = new WebRequestFactory("http://ckmasterplex:8989/api");
            string urlParameters = "/command";
            string method = "POST";
            string contentType = "application/json";
            List<string[]> headers = new List<string[]>();
            headers.Add(new string[] { "X-Api-Key", "e2ddb6b1b3e046bdab2a9a494a5a4d0c" });
            foreach(KeyValuePair<string, bool> entry in downloadFolders)
            {
                string path = $@"{basePath}{entry.Key}";
                string body =
                $@"
                {{
                    ""importMode"": ""copy"",
                    ""name"": ""downloadedEpisodesScan"",
                    ""path"": ""{path}""
                }}
                ";
                IWebResponseFactory webRespFac = new WebResponseFactory(webReqFac.BuildRequest(urlParameters, method, contentType, headers, body));
                webRespFac.ExecuteRequest();
            }
        }
    }
}

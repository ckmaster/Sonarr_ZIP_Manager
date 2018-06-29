using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;

namespace Sonarr_ZIP_Manager
{
    public interface IWebResponseFactory
    {
        HttpWebResponse ExecuteRequest();
        string GetResponseBody(HttpWebResponse response);
    }

    class WebResponseFactory : IWebResponseFactory
    {
        HttpWebRequest request;

        public WebResponseFactory(HttpWebRequest r)
        {
            request = r;
        }

        HttpWebResponse IWebResponseFactory.ExecuteRequest()
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response;
        }

        string IWebResponseFactory.GetResponseBody(HttpWebResponse response)
        {
            string responseBody = "";
            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseBody = reader.ReadToEnd();
            }
            return responseBody;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
//using System.Net.Http;
using System.IO;

namespace Sonarr_ZIP_Manager
{
    public interface IWebRequestFactory
    {
        HttpWebRequest BuildRequest(string urlParameters, string method, List<string[]> headers);
        HttpWebRequest BuildRequest(string urlParameters, string method, string contentType, List<string[]> headers, string body);
    }

    class WebRequestFactory : IWebRequestFactory
    {
        private static string baseUrl;

        public WebRequestFactory(string b)
        {
            baseUrl = b;
        }

        HttpWebRequest IWebRequestFactory.BuildRequest(string urlParameters, string method, List<string[]> headers)
        {
            string fullUrl = $"{baseUrl}{urlParameters}";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
            request.Method = method;
            foreach (string[] s in headers)
            {
                if (!WebHeaderCollection.IsRestricted(s[0]))
                {
                    request.Headers.Add(s[0], s[1]);
                }
                else if (String.Equals("Accept", s[0]))
                {
                    request.Accept = "application/json";
                }
            }
            return request;
        }

        HttpWebRequest IWebRequestFactory.BuildRequest(string urlParameters, string method, string contentType, List<string[]> headers, string body)
        {
            string fullUrl = $"{baseUrl}{urlParameters}";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(fullUrl);
            request.Method = method;
            request.ContentType = contentType;
            foreach (string[] s in headers)
            {
                if (!WebHeaderCollection.IsRestricted(s[0]))
                {
                    request.Headers.Add(s[0], s[1]);
                }
                else if (String.Equals("Accept", s[0]))
                {
                    request.Accept = "application/json";
                }
            }
            Stream dataStream = request.GetRequestStream();
            byte[] byteArray = Encoding.UTF8.GetBytes(body);
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            return request;
        }
    }
}

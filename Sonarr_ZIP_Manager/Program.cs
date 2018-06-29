using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sonarr_ZIP_Manager
{
    class Program
    {
        public Program()
        {
            
        }

        public void Run(string[] args)
        {
            Sonarr sonarr = new Sonarr();
            IWebResponseFactory wrf = new WebResponseFactory(sonarr.QueuedItems());
            string responseBody = wrf.GetResponseBody(wrf.ExecuteRequest());
            IJsonManager jm = new JsonManager();
            JArray jsonArray = jm.DeserializeArray(responseBody);
            List<string> downloadFolders = sonarr.GetDownloadFolders(jsonArray);
            Qbittorrent qbt = new Qbittorrent();
            Dictionary<string, bool> downloadFinished = qbt.CheckDownloadStatus(downloadFolders);
            qbt.Logout();
            FileSysOps fso = new FileSysOps();
            fso.ExtractFile(downloadFinished);
            sonarr.PushExtracted(downloadFinished);
        }

        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Run(args);
        }
    }
}

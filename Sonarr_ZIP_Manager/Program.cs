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
using System.ServiceProcess;
using System.Threading;

namespace Sonarr_ZIP_Manager
{
    class Program : ServiceBase
    {
        public Program()
        {
            Thread thread = new Thread(Actions);
            thread.Start();
        }

        public static void Actions()
        {
            while (true)
            {
                SonarrActions();
                RadarrActions();
                Thread.Sleep(600000);
            }
        }

        static void SonarrActions()
        {
            Sonarr sonarr = new Sonarr();
            IWebResponseFactory webRespFac = new WebResponseFactory(sonarr.QueuedItems());
            string responseBody = webRespFac.GetResponseBody(webRespFac.ExecuteRequest());
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

        static void RadarrActions()
        {
            Radarr radarr = new Radarr();
            IWebResponseFactory webRespFac = new WebResponseFactory(radarr.QueuedItems());
            string responseBody = webRespFac.GetResponseBody(webRespFac.ExecuteRequest());
            IJsonManager jm = new JsonManager();
            JArray jsonArray = jm.DeserializeArray(responseBody);
            List<string> downloadFolders = radarr.GetDownloadFolders(jsonArray);
            Qbittorrent qbt = new Qbittorrent();
            Dictionary<string, bool> downloadFinished = qbt.CheckDownloadStatus(downloadFolders);
            qbt.Logout();
            FileSysOps fso = new FileSysOps();
            fso.ExtractFile(downloadFinished);
            radarr.PushExtracted(downloadFinished);
        }

        static void Main(string[] args)
        {
            ServiceBase.Run(new Program());
        }
    }
}

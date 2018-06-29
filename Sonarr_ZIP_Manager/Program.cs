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

        //public void Run(string[] args)
        //{
            
        //}

        public static void Actions()
        {
            while (true)
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
                Thread.Sleep(600000);
            }
        }

        //protected override void OnStart(string[] args)
        //{
        //    Program prog = new Program();
        //}

        //protected override void OnStop()
        //{

        //}

        static void Main(string[] args)
        {
            ServiceBase.Run(new Program());
        }
    }
}

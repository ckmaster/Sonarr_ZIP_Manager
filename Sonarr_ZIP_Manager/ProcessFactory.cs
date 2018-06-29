using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Sonarr_ZIP_Manager
{
    public interface IProcessFactory
    {
        ProcessStartInfo BuildProcess();
        void ExecuteSevenZip(ProcessStartInfo psi, string archiveFile, string path);
    }

    class ProcessFactory : IProcessFactory
    {
        ProcessStartInfo IProcessFactory.BuildProcess()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "CMD.EXE";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            return psi;
        }

        void IProcessFactory.ExecuteSevenZip(ProcessStartInfo psi, string archiveFile, string path)
        {
            string sevenZipPath = @"""C:\Program Files\7-Zip\7z.exe""";
            using (Process p = Process.Start(psi))
            {
                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine($@"{sevenZipPath} x -r -aos {archiveFile} -o{path}");
                    }
                }
                p.WaitForExit();
                using (StreamReader sr = p.StandardOutput)
                {
                    string stdout = sr.ReadToEnd();
                    Globals.outLogger.CreateOrAppend(stdout);
                }
                using (StreamReader sr = p.StandardError)
                {
                    string stderr = sr.ReadToEnd();
                    Globals.errLogger.CreateOrAppend(stderr);
                }
            }
        }
    }
}

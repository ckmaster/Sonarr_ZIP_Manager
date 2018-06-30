using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Sonarr_ZIP_Manager
{
    public class FileSysOps
    {
        public bool IsFolder(string folder)
        {
            FileAttributes attr = new FileAttributes();
            try
            {
                attr = File.GetAttributes(folder);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Globals.errLogger.CreateOrAppend("Not a folder\n", e);
            }
            return false;
        }

        public string FindArchiveFile(string path)
        {
            List<string> files = Directory.GetFiles(path).ToList();
            foreach (string file in files)
            {
                if (String.Equals(Path.GetExtension(file), ".rar"))
                {
                    return file;
                }
            }
            return null;
        }

        public List<string> FindArchiveFiles(string path)
        {
            List<string> archiveFiles = new List<string>();
            List<string> dirs = Directory.GetDirectories(path).ToList();
            foreach (string dir in dirs)
            {
                List<string> files = Directory.GetFiles(dir).ToList();
                foreach (string file in files)
                {
                    if (String.Equals(Path.GetExtension(file), ".rar"))
                    {
                        archiveFiles.Add(file);
                    }
                }
            }
            return archiveFiles;
        }

        public void ExtractFile(Dictionary<string, bool> downloadFinshed)
        {
            //string basePath = @"\\ckmasterplex\h$\Downloads\";
            string basePath = @"H:\Downloads\";
            foreach (KeyValuePair<string, bool> entry in downloadFinshed)
            {
                if (entry.Value)
                {
                    string path = $@"{basePath}{entry.Key}";
                    if (IsFolder(path))
                    {
                        string archiveFile = FindArchiveFile(path);
                        IProcessFactory pf = new ProcessFactory();
                        ProcessStartInfo psi = pf.BuildProcess();
                        if (!String.IsNullOrEmpty(archiveFile))
                        {
                            pf.ExecuteSevenZip(psi, archiveFile, path);
                        }
                        else
                        {
                            List<string> archiveFiles = FindArchiveFiles(path);
                            foreach(string aFile in archiveFiles)
                            {
                                pf.ExecuteSevenZip(psi, aFile, path);
                            }
                        }
                    }
                }
            }
        }
    }
}

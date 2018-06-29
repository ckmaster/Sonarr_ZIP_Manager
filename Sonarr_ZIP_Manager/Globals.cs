using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sonarr_ZIP_Manager
{
    class Globals
    {
        public static Logger outLogger = new Logger(@"C:\Users\ckmaster\Desktop\sonarr_zip_manager_stdout.log");
        public static Logger errLogger = new Logger(@"C:\Users\ckmaster\Desktop\sonarr_zip_manager_stderr.log");
    }
}

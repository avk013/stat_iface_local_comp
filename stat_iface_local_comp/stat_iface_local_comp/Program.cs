using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stat_iface_local_comp
{
    class Program
    {
        static string path_conf_file = @"c:\stat_iface_local_comp\!!!out\hosts.conf";
        static string path_conf = @"c:\stat_iface_local_comp\!!!out";
       // static string path = @"c:\stat_iface_local_comp";
        static void init_config()
        {
            //if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if (!Directory.Exists(path_conf)) Directory.CreateDirectory(path_conf);
            if (!File.Exists(path_conf_file)) File.Create(path_conf_file);
            FileInfo f = new FileInfo(path_conf_file);
        }
        public static void Main(string[] args)
        {
           init_config();
            //123
        }
         }
    
}

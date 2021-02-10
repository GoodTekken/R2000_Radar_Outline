using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTrace
{
    public static class TraceLog
    {
        public static void Write(string logMessage)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string dirLog = dir + @"/TekkenLog";
            if (Directory.Exists(dirLog) ==false)
            {
                Directory.CreateDirectory(dirLog);
            }
            using (StreamWriter sw = new StreamWriter(dirLog+@"/Log.txt",true,Encoding.UTF8))
            {
                sw.WriteLine($"{DateTime.Now}: {logMessage}");
            }
        }
    }
}

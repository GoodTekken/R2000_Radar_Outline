using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToolkit.Trace
{
    public static class TraceLog
    {
        private static void Write(string logMessage)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string dirLog = dir + @"/TekkenLog";
            if (Directory.Exists(dirLog) == false)
            {
                Directory.CreateDirectory(dirLog);
            }
            using (StreamWriter sw = new StreamWriter(dirLog + @"/Log.txt", true, Encoding.UTF8))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}: {logMessage}");
            }
        }

        private static void Write(string type, string logMessage)
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string dirLog = dir + @"/TekkenLog";
            if (Directory.Exists(dirLog) == false)
            {
                Directory.CreateDirectory(dirLog);
            }
            using (StreamWriter sw = new StreamWriter(dirLog + @"/Log.txt", true, Encoding.UTF8))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ms")}: [{type}]{logMessage}");
            }
        }

        public static void Write(TraceTypeEnum tarcetyoe, string logMessage)
        {
            switch (tarcetyoe)
            {
                case TraceTypeEnum.empty:
                    Write(" ", logMessage);
                    break;
                case TraceTypeEnum.trace:
                    Write("trace", logMessage);
                    break;
                case TraceTypeEnum.warning:
                    Write("warning", logMessage);
                    break;
                case TraceTypeEnum.error:
                    Write("error", logMessage);
                    break;
                case TraceTypeEnum.other:
                    Write("other", logMessage);
                    break;
                default:
                    break;
            }
        }
    }
}

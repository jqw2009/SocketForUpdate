using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Server
{
    public class Log
    {
        public static void WriteLog(string msg)
        {
            StreamWriter writer = null;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "log";
                string str2 = path + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                writer = new StreamWriter(str2, true);
                writer.WriteLine(msg);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public static void WriteLog(List<string> msg)
        {
            StreamWriter writer = null;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "log";
                string str2 = path + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                writer = new StreamWriter(str2, true);
                foreach (String str in msg)
                {
                    writer.WriteLine(str);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }
    }
}

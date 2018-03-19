using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Server
{
    public class Log
    {
        private static readonly object obj = new object();
        public static void WriteLog(string msg)
        {
            lock (obj)
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
                    writer.WriteLine(System.DateTime.Now.ToString("HH:mm:ss")+ "\t"+msg);
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

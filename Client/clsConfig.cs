using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Update
{
    public static class clsConfig
    {

        public static string serverIP = "";
        public static string serverPort = "";
        public static string appPath = "";
        public static string appMainName = "";

        public static string updateversion = "";
        public static string updatetime = "";
        public static string updatemsg = "";

        public static Socket ConnectServer()
        {
            try
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(serverIP);
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soc.Connect(new IPEndPoint(address, int.Parse(serverPort)));

                return soc;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string LoadXmlData()
        {
            try
            {
                if (!File.Exists("set.xml"))
                {
                    return "未找到更新配置文件'set.xml'";
                }
                XmlDocument doc = new XmlDocument();
                doc.Load("set.xml");
                XmlNode root = doc.SelectSingleNode("set");
                serverIP = (root.SelectSingleNode("serverIP")).InnerText.Trim();
                serverPort = (root.SelectSingleNode("serverPort")).InnerText.Trim();
                appPath = (root.SelectSingleNode("appPath")).InnerText.Trim();
                appMainName = (root.SelectSingleNode("appMainName")).InnerText.Trim();


                if (serverIP == string.Empty)
                {
                    return "报错\n更新配置文件里服务器IP为空，无法更新";
                }
                if (serverPort == string.Empty)
                {
                    return "报错\n更新配置文件里服务器端口为空，无法更新";
                }
                if (appPath == string.Empty)
                {
                    return "报错\n更新配置文件里主程序路径为空，无法启动主程序";
                }
                if (appMainName == string.Empty)
                {
                    return "报错\n更新配置文件里主程序名为空，无法启动程序";
                }

                LoadVersion();

                return "";
            }
            catch (Exception)
            {
                return "加载更新配置文件出错，请联系相关人员" ;
            }
        }


        private static void LoadVersion()
        {
            try
            {
                if (File.Exists("Version.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("Version.xml");
                    XmlNode root = doc.SelectSingleNode("root");
                    updateversion = (root.SelectSingleNode("updateversion")).InnerText.Trim();
                    updatetime = (root.SelectSingleNode("updatetime")).InnerText.Trim();
                    updatemsg = (root.SelectSingleNode("updatemsg")).InnerText.Trim();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static void SendVersion(Socket soc)
        {
            try
            {
                soc.Send(Encoding.Unicode.GetBytes(":" + clsConfig.updateversion + "?"));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string RecvServerFileInfo(Socket soc)
        {
            try
            {
                byte[] buffer = new byte[8192];
                int len = 0;
                string strFileInfo = "";
                while (true)
                {
                    len = soc.Receive(buffer, 0, 8192, SocketFlags.None);
                    strFileInfo += Encoding.Unicode.GetString(buffer, 0, len);
                    if (strFileInfo.Substring(strFileInfo.Length - 1, 1) == "?")
                    {
                        break;
                    }
                }
                return strFileInfo.Substring(0, strFileInfo.Length - 1);
            }
            catch (Exception)
            {
                throw;
            }
        }






    }
}

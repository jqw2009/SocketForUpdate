using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.IO;

namespace Server
{
    class clsConfig
    {
        public static string serverIP = "";
        public static string serverPort = "";
        public static string serverRate = "";
        public static string appPath = "";
        public static string appMainName = "";

        public static string updateversion = "";
        public static string updatetime = "";
        public static string updatemsg = "";

        public static string TempMsg = "";

        public static string LoadXmlData()
        {
            try
            {
                if (!File.Exists("serverset.xml"))
                {
                    return "未找到更新配置文件'serverset.xml'";
                }
                XmlDocument doc = new XmlDocument();
                doc.Load("serverset.xml");
                XmlNode root = doc.SelectSingleNode("set");
                serverIP = (root.SelectSingleNode("serverIP")).InnerText.Trim();
                serverPort = (root.SelectSingleNode("serverPort")).InnerText.Trim();
                serverRate = (root.SelectSingleNode("serverRate")).InnerText.Trim();
                appPath = (root.SelectSingleNode("appPath")).InnerText.Trim();
                appMainName = (root.SelectSingleNode("appMainName")).InnerText.Trim();

                updateversion = (root.SelectSingleNode("updateversion")).InnerText.Trim();
                updatetime = (root.SelectSingleNode("updatetime")).InnerText.Trim();
                updatemsg = (root.SelectSingleNode("updatemsg")).InnerText.Trim();

                if (serverIP == string.Empty)
                {
                    return "报错\n更新配置文件serverset.xml里服务器IP为空，无法启动";
                }
                if (serverPort == string.Empty)
                {
                    return "报错\n更新配置文件serverset.xml里服务器端口为空，无法启动";
                }
                if (!Directory.Exists(appPath))
                {
                    return "报错\n更新配置文件serverset.xml里未找到更新路径，无法启动";
                }
                //if (appPath == string.Empty)
                //{
                //    return "报错\n更新配置文件里主程序路径为空，无法启动主程序";
                //}
                //if (appMainName == string.Empty)
                //{
                //    return "报错\n更新配置文件里主程序名为空，无法启动程序";
                //}
                return "";
            }
            catch (Exception)
            {
                return "加载更新配置文件出错，请联系相关人员";
            }
        }

    }
}

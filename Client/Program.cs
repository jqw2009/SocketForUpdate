using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace Update
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //加载配置信息
            string msg = clsConfig.LoadXmlData();
            if (msg != string.Empty)
            {
                MessageBox.Show(msg);
                Environment.Exit(0);
            }
            //连接服务器
            Socket soc = null;
            try
            {
                soc = clsConfig.ConnectServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接更新服务器失败，具体信息如下\n"+ex.Message);
                Environment.Exit(0);
            }
            ////检查版本
            ////发送本地版本信息
            //clsConfig.SendVersion(soc);
            // string strVersion=clsConfig.RecvServerFileInfo(soc);

            //if (strVersion == string.Empty || strVersion == "0")
            //{
            //    ////空代表没有要更新的，0代表更新服务停止了
            //    ////没有更新文件就结束,启用主程序
            //    //StartAppMain(strVersion);
            //    //return;
            //}
            //else
            //{
            //    //显示更新原因
            //    frmReason rfrmReason = new frmReason((strVersion.Replace(":", "").Trim().Split('*'))[2]);
            //    rfrmReason.ShowDialog();
            //}


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(soc));
        }










    }
}

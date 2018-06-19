using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Concurrent; //并发集合空间

namespace Server
{
    public partial class frmMain : Form
    {

        Socket serverSocket;
        //ConcurrentBag<string> listConnect = new ConcurrentBag<string>();
        bool b_IsUpdate = false;
        bool b_IsDebugLog = true;

        //private delegate void d_show(int value);
        //private event d_show showMsg;

        int listConnect = 0;
        List<string> listIP = new List<string>();
        private readonly object objLock = new object();
        private readonly object objLock2 = new object();
        int Rate = 1024;

        void ChangeConnectCount(int value)
        {
            lock (objLock)
            {
                listConnect += value;
                //lblCount.Text = listConnect.ToString();
            }
        }

        void showIP(EndPoint ep, int AddOrDel)
        {
            lock (objLock2)
            {
                switch (AddOrDel)
                {
                    case 0://add
                        listIP.Add(((IPEndPoint)ep).Address.ToString() + ":" + ((IPEndPoint)ep).Port);
                        break;
                    case 1://delete
                        string msg = ((IPEndPoint)ep).Address.ToString() + ":" + ((IPEndPoint)ep).Port;
                        listIP.Remove(((IPEndPoint)ep).Address.ToString() + ":" + ((IPEndPoint)ep).Port);
                        break;
                }

                txtConnectIPs.Text = "";
                foreach (string str in listIP)
                {
                    txtConnectIPs.Text += str + "\r\n";
                }
            }
        }

        class fileData
        {
            public string fileName { get; set; }
            public string fileLength { get; set; }
            public string fileTime { get; set; }
        }

        public frmMain()
        {
            InitializeComponent();
            //TextBox.CheckForIllegalCrossThreadCalls = false;
            //Button.CheckForIllegalCrossThreadCalls = false;
            //Label.CheckForIllegalCrossThreadCalls = false;

            Control.CheckForIllegalCrossThreadCalls = false;

            txtIp.Text = clsConfig.serverIP;
            txtPort.Text = clsConfig.serverPort;
            cboxRate.Text = clsConfig.serverRate;

            lblVersion.Text ="更新版本："+ clsConfig.updateversion;
            lblUpdateTime.Text = "更新时间：" + clsConfig.updatetime;

        }


        private void btnListen_Click(object sender, EventArgs e)
        {
            try
            {

                if (btnListen.Text == "开启服务")
                {
                    //listConnect = new ConcurrentBag<string>();

                    if (txtIp.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("IP不可为空");
                        return;
                    }
                    if (txtPort.Text.Trim() == string.Empty)
                    {
                        MessageBox.Show("端口不可为空");
                        return;
                    }
                    if (cboxRate.Text == string.Empty)
                    {
                        MessageBox.Show("传输速度不可为空");
                        return;
                    }
                    else
                    {
                        Rate = Convert.ToInt32(cboxRate.Text) * 1024;
                    }

                    IPAddress address = IPAddress.Parse(txtIp.Text.Trim());
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(new IPEndPoint(address, int.Parse(txtPort.Text.Trim())));
                    serverSocket.Listen(1000);

                    txtIp.ReadOnly = true;
                    txtPort.ReadOnly = true;
                    cboxRate.Enabled = false;
                    btnListen.Text = "服务运行中";
                    btnListen.BackColor = Color.FromKnownColor(KnownColor.Lime);

                    b_IsUpdate = true;
                    btnUpdate.Text = "停止更新";
                    btnUpdate.BackColor = Color.FromKnownColor(KnownColor.Lime);

                    ThreadPool.QueueUserWorkItem(listenClientConnect);

                    //timer_count.Enabled = true;
                    
                }
                else if (btnListen.Text == "服务运行中")
                {
                    btnListen.Text = "停止服务中";
                    btnListen.Enabled = false;
                    timer_stopServer.Enabled = true;

                    b_IsUpdate = false;
                    btnUpdate.Enabled = false;
                    btnUpdate.Text = "停止中。。";
                    timer_update.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                //txtIp.ReadOnly = false;
                //txtPort.ReadOnly = false;
                //cboxRate.Enabled = true;
                MessageBox.Show(ex.Message+ex.StackTrace);
            }
        }

        private void listenClientConnect(object o)
        {
            try
            {
                while (true)
                {
                    Socket clientSocket = serverSocket.Accept();
                    showIP(clientSocket.RemoteEndPoint, 0);
                    //ChangeConnectCount(1);
                    if (b_IsDebugLog)
                    {
                        Log.WriteLog("获取到客户端的连接，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                    }
                    Thread td = new Thread(clientMsg);
                    td.IsBackground = true;
                    td.Start(clientSocket);
                    //ThreadPool.QueueUserWorkItem(clientMsg, clientSocket);
                }
            }
            catch (Exception ex)
            {
                txtIp.ReadOnly = false;
                txtPort.ReadOnly = false;
                cboxRate.Enabled = true;
                btnListen.Text = "开启服务";
                btnListen.Enabled = true;
                btnListen.BackColor = Color.FromKnownColor(KnownColor.Control);

                lblCount.Text = "----";

                Log.WriteLog("服务监听程序停止：" + ex.Message + ex.StackTrace);
            }
        }

        private void clientMsg(object mSocket)
        {
            Socket clientSocket = (Socket)mSocket;
            clientSocket.SendTimeout = 1000*60;
            clientSocket.ReceiveTimeout = 1000*60;
            try
            {
                //Socket clientSocket = (Socket)mSocket;
                string strClientFileInfo = "";
                if (b_IsDebugLog)
                {
                    Log.WriteLog("开始接收客户端更新数据，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                }
                int index = 0;
                while (true)
                {
                    byte[] msg = new byte[Rate];
                    int recvCount = clientSocket.Receive(msg, 0, Rate, SocketFlags.None);
                    if (b_IsDebugLog)
                    {
                        index++;
                        Log.WriteLog("第" + index + "次接收客户端更新内容 recvCount " + recvCount + "，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                    }
                    if (recvCount <= 0)
                    {
                        //0标识socket关闭了,那么就跳出结束此进程
                        //showIP(clientSocket.RemoteEndPoint, 1);
                        //ChangeConnectCount(-1);
                        if (b_IsDebugLog)
                        {
                            Log.WriteLog("客户端的连接正常关闭，recvCount为：" + recvCount + " IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                        }
                        break;
                    }

                    //获取客户端文件信息
                    //strClientFileInfo += Encoding.UTF8.GetString(msg, 0, recvCount);
                    //strClientFileInfo += Encoding.GetEncoding("GB2312").GetString(msg, 0, recvCount);
                    strClientFileInfo += Encoding.Unicode.GetString(msg, 0, recvCount);
                    if (strClientFileInfo.Substring(strClientFileInfo.Length - 1, 1) != "?")//?表示结束符号，没有结束符号则继续接受数据
                    {
                        continue;
                    }
                    //if (recvCount < Rate)//当接收的数据小于指定的大小时，就要检查客户端来源的数据正确性了
                    //{
                    //    if (strClientFileInfo.Substring(strClientFileInfo.Length - 1, 1) != "?")//?表示结束符号，没有结束符号则继续接受数据
                    //    {
                    //        if (b_IsDebugLog)
                    //        {
                    //            Log.WriteLog("客户端数据非法，退出，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                    //            Log.WriteLog("非法数据：" + strClientFileInfo);
                    //        }
                    //        break;//当数据接收完毕的时候还没有?，说明客户端数据非法，直接退出
                    //    }
                    //}
                    //else
                    //{
                    //    if (strClientFileInfo.Substring(strClientFileInfo.Length - 1, 1) != "?")//?表示结束符号，没有结束符号则继续接受数据
                    //    {
                    //        continue;
                    //    }
                    //}
                    strClientFileInfo = strClientFileInfo.Substring(0, strClientFileInfo.Length - 1);//去掉结束符号
                    if (b_IsDebugLog)
                    {
                        Log.WriteLog("接收客户端更新数据完毕，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                    }
                    if (!b_IsUpdate)
                    {
                        if (b_IsDebugLog)
                        {
                            Log.WriteLog("发送不更新代码，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                        }
                        //停止更新 用0代表不更新
                        clientSocket.Send(Encoding.Unicode.GetBytes("0?"));
                        continue;
                    }

                    //20170808 先检查版本,新加入的，是为了显示更新原因
                    // 以:开头作为版本，是为了兼容之前的更新程序，第一个字符是:说明是新的更新客户端要检查版本，没有:就是旧的直接检查更新
                    if (strClientFileInfo.Length > 0)
                    {
                        if (strClientFileInfo.Substring(0, 1) == ":")
                        {
                            string version = strClientFileInfo.Replace(":", "").Trim();
                            strClientFileInfo = "";//清空数据
                            if (version == clsConfig.updateversion)
                            {
                                if (b_IsDebugLog)
                                {
                                    Log.WriteLog("发送版本相同的代码，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                                }
                                clientSocket.Send(Encoding.Unicode.GetBytes("?"));
                                continue;
                            }
                            else
                            {
                                if (b_IsDebugLog)
                                {
                                    Log.WriteLog("发送服务器版本，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                                }
                                string versioninfo = ":" + clsConfig.updateversion + "*" + clsConfig.updatetime + "*" + clsConfig.updatemsg + "?";
                                clientSocket.Send(Encoding.Unicode.GetBytes(versioninfo));
                                continue;
                            }
                        }
                    }

                    //检查需要更新的文件
                    if (b_IsDebugLog)
                    {
                        Log.WriteLog("开始检查要更新的文件，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                    }
                    List<fileData> list = CheckUpdateFiles(strClientFileInfo);
                    if (list.Count > 0)
                    {
                        //先发送文件信息给客户端（文件名，文件大小）
                        if (b_IsDebugLog)
                        {
                            Log.WriteLog("发送文件信息到客户端，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                        }
                        if (SendFileInfo(clientSocket, list))
                        {
                            if (b_IsDebugLog)
                            {
                                Log.WriteLog("发送文件本身到客户端，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                            }
                            //返回成功后即开始发送文件
                            SendFile(clientSocket,list);
                            if (b_IsDebugLog)
                            {
                                Log.WriteLog("文件发送完毕，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                            }
                        }
                    }
                    else
                    {
                        //没有需要更新的文件时直接发送结束符号?
                        clientSocket.Send(Encoding.Unicode.GetBytes("?"));
                        if (b_IsDebugLog)
                        {
                            Log.WriteLog("客户端的没有要更新的，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port);
                        }
                    }
                }
                //clientSocket.Shutdown(SocketShutdown.Both);
                showIP(clientSocket.RemoteEndPoint, 1);
                clientSocket.Disconnect(false);
                clientSocket.Close(); //报错了，可能是关闭socket的缘故，也有可能是线程池重复使用而socket又关闭了导致的，所以就拿掉socket
                clientSocket.Dispose();
            }                
            catch (SocketException ex)
            {
                showIP(clientSocket.RemoteEndPoint, 1);
                if (b_IsDebugLog)
                {
                    Log.WriteLog("客户端的连接发生异常，IP为" + ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString() + ":" + ((IPEndPoint)clientSocket.RemoteEndPoint).Port+"\r\n 异常信息为："+ex.Message+ex.StackTrace+ex.InnerException);
                }
                clientSocket.Disconnect(false);
                clientSocket.Close(); //报错了，可能是关闭socket的缘故，也有可能是线程池重复使用而socket又关闭了导致的，所以就拿掉socket
                clientSocket.Dispose();
            }
        }

        void GetClientFileInfo()
        {
        }

        bool SendFileInfo(Socket socketSend, List<fileData> list)
        {
            try
            {
                StringBuilder strInfo = new StringBuilder();
                //*文件与大小的分隔符，|文件之间的分隔符，?结束符
                for (int i = 0; i < list.Count; i++)
                {
                    strInfo.Append(list[i].fileName + "*" + list[i].fileLength + "*" + list[i].fileTime + "|");
                }
                string strTemp = strInfo.ToString();
                strTemp = strTemp.Substring(0, strTemp.Length - 1) + "?";
                socketSend.Send(Encoding.Unicode.GetBytes(strTemp));
                //发送结束后等待客户端的成功标志
                byte[] msg = new byte[8192];
                int recvCount = socketSend.Receive(msg, 0, 8192, SocketFlags.None);
                if (recvCount == 0)
                {
                    return false;
                }
                else
                {
                    //理论上返回任何值都表示接受成功
                    //     if (Encoding.ASCII.GetString(msg) == "?")
                    //   {
                    return true;
                    //}
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        List<fileData> CheckUpdateFiles(string clientVersion)
        {
            try
            {
                //解析客户端
                string[] clientFiles = clientVersion.Split('|');//文件名，大小，时间
                ArrayList clientList = new ArrayList(clientFiles);

                //开始比较两边的数据
                List<fileData> list = new List<fileData>();
                string[] files = Directory.GetFiles(clsConfig.appPath, "*.*", SearchOption.AllDirectories);
                ArrayList listFiles = new ArrayList(files);
                if (File.Exists("set.xml"))
                {
                    listFiles.Add("set.xml");
                }
                for (int i = 0; i < listFiles.Count; i++)
                {
                    bool IsFind = false;
                    for (int j = 0; j < clientList.Count; j++)
                    {
                        if (listFiles[i].ToString().ToUpper() == clientList[j].ToString().Split('*')[0].ToUpper())
                        {
                            //如果找到了就开始比较时间，简单点，直接比较时间一不一样，不一样就添加到更新列表
                            FileInfo info = new FileInfo(listFiles[i].ToString());
                            //string str1 = info.LastWriteTime.ToString("yyyyMMddHHmmss");
                            //string str2 = clientList[j].ToString().Split('*')[1];
                            if (info.LastWriteTime.ToString("yyyyMMddHHmmss") != clientList[j].ToString().Split('*')[1])
                            {
                                fileData data = new fileData();
                                data.fileName = listFiles[i].ToString();
                                data.fileLength = info.Length.ToString();
                                data.fileTime = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                                list.Add(data);

                                IsFind = true;
                                clientList.RemoveAt(j);
                                break;
                            }
                            else
                            {
                                IsFind = true;
                                clientList.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    if (!IsFind)
                    {
                        FileInfo info = new FileInfo(listFiles[i].ToString());
                        fileData data = new fileData();
                        data.fileName = listFiles[i].ToString();
                        data.fileLength = info.Length.ToString();
                        data.fileTime = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                        list.Add(data);
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        void SendFile(Socket clientSocket,List<fileData> list)
        {
            FileStream fs = null;
            try
            {
                byte[] buffer = new byte[8192];
                int len = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    fs = new FileStream(list[i].fileName, FileMode.Open, FileAccess.Read);
                    while ((len = fs.Read(buffer, 0, 8192)) != 0)
                    {
                        clientSocket.Send(buffer, 0, len, SocketFlags.None);
                    }
                    fs.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
                throw;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void timer_stopServer_Tick(object sender, EventArgs e)
        {
            timer_stopServer.Enabled = false;
            if (listIP.Count==0)
            {
                Thread.Sleep(200);
                serverSocket.Close();

                //txtIp.ReadOnly = false;
                //txtPort.ReadOnly = false;
                //btnListen.Text = "开启服务";
                //btnListen.Enabled = true;
                //btnListen.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            else
            {
                timer_stopServer.Enabled = true;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (btnUpdate.Text == "启用更新")
            {
                b_IsUpdate = true;
                btnUpdate.Text = "停止更新";
                btnUpdate.BackColor = Color.FromKnownColor(KnownColor.Lime);
            }
            else if(btnUpdate.Text=="停止更新")
            {

                b_IsUpdate = false;
                btnUpdate.Enabled = false;
                btnUpdate.Text = "停止中。。";
                timer_update.Enabled = true;
            }
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            timer_update.Enabled = false;

            if (listIP.Count == 0)
            {
                btnUpdate.Enabled = true;
                btnUpdate.Text = "启用更新";
                btnUpdate.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            else
            {
                timer_update.Enabled = true;
            }
        }

        private void timer_count_Tick(object sender, EventArgs e)
        {
            timer_count.Enabled = false;

            lblCount.Text = listConnect.ToString();

            txtConnectIPs.Text = "";
            string[] ips= listIP.ToArray();
            foreach (string str in ips)
            {
                txtConnectIPs.Text +=str+ "\r\n";
            }

            timer_count.Enabled = true;
        }

        private void btnUpdateVersion_Click(object sender, EventArgs e)
        {
            try
            {
                frmReason rfrmReason = new frmReason();
                if (rfrmReason.ShowDialog() == DialogResult.OK)
                {
                    string version = clsConfig.updateversion;
                    if (version == string.Empty)//如果为空则赋值1
                    {
                        version = "1";
                    }
                    else
                    {
                        try
                        {
                            version = (Convert.ToInt32(version) + 1).ToString();//不为空则+1
                        }
                        catch (Exception)
                        {
                            version = "1";//如果+1失败就赋值1从头开始
                        }
                    }

                    WriteVersionFile(version);
                }
                rfrmReason.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private void UpdateServerFile(string strVersion)
        {
            try
            {
                if (!File.Exists("serverSet.xml"))
                {
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load("serverSet.xml");
                string strTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                XmlNode root = doc.SelectSingleNode("set");
                (root.SelectSingleNode("updatetime")).InnerText = strTime;
                (root.SelectSingleNode("updatemsg")).InnerText = clsConfig.TempMsg;
                (root.SelectSingleNode("updateversion")).InnerText = strVersion;

                doc.Save("serverSet.xml");

                clsConfig.updateversion = strVersion;
                clsConfig.updatetime = strTime;
                clsConfig.updatemsg = clsConfig.TempMsg;
                clsConfig.TempMsg = "";

                lblVersion.Text = "更新版本：" + clsConfig.updateversion;
                lblUpdateTime.Text = "更新时间：" + clsConfig.updatetime;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void WriteVersionFile(string version)
        {
            try
            {
                UpdateVersionHistory("version_" + version);
                UpdateServerFile(version);
            }
            catch (Exception)
            {               
                throw;
            }
        }

        private void UpdateVersionHistory(string strVersion)
        {
            try
            {
                if (!File.Exists("versionHistory.xml"))
                {
                    CreateXML("versionHistory.xml");
                }
                XmlDocument doc = new XmlDocument();
                doc.Load("versionHistory.xml");

                XmlNode root = doc.SelectSingleNode("root");
                XmlElement ele = doc.DocumentElement;
                XmlNodeList bodyNodes = ele.GetElementsByTagName(strVersion);
                if (bodyNodes.Count > 0)
                {
                    XmlNode xmlVersion = bodyNodes.Item(0);
                    (xmlVersion.SelectSingleNode("updatetime")).InnerText = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    (xmlVersion.SelectSingleNode("updatemsg")).InnerText = clsConfig.TempMsg;
                }
                else
                {
                    //创建子节点
                    XmlNode NodeVersion = doc.CreateNode(XmlNodeType.Element, strVersion, null);
                    NodeVersion.InnerText = "";
                    root.AppendChild(NodeVersion);
                    XmlNode childNode = doc.CreateNode(XmlNodeType.Element, "updatetime", null);
                    childNode.InnerText = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    NodeVersion.AppendChild(childNode);
                    childNode = doc.CreateNode(XmlNodeType.Element, "updatemsg", null);
                    childNode.InnerText = clsConfig.TempMsg;
                    NodeVersion.AppendChild(childNode);
                }
                doc.Save("versionHistory.xml");


            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CreateXML(string xmlName)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                //创建类型声明节点   
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);
                //创建根节点   
                XmlNode root = xmlDoc.CreateElement("root");
                xmlDoc.AppendChild(root);

                ////创建子节点
                //XmlNode childNode = xmlDoc.CreateNode(XmlNodeType.Element, "user", null);
                //childNode.InnerText = "";
                //root.AppendChild(childNode);
                //childNode = xmlDoc.CreateNode(XmlNodeType.Element, "companycode", null);
                //childNode.InnerText = "";
                //root.AppendChild(childNode);
                xmlDoc.Save(xmlName);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}

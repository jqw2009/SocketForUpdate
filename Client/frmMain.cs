using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Diagnostics;

using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Update
{
    public partial class frmMain : Form
    {
        Socket soc = null;

        private delegate void progressbar(Int64 value1, Int64 value2);
        private progressbar showBar_Current;
        private progressbar showBar_Total;
        private delegate void mMessage(string value);
        private mMessage msg;
      //  private delegate void d_OpenProgress();
      //  private d_OpenProgress m_OpenProgress;

        //int frmWidth = 0;
        //int frmHeight = 0;

        Bitmap bmpBG = null;
        CustomForm customForm = new CustomForm();

        protected override CreateParams CreateParams//给不规则窗体用的
        {
            get
            {
                const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义   
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作
                cp.ExStyle |= 0x00080000; // WS_EX_LAYERED
                return cp;
            }
        }

        public frmMain(Socket socket)
        {
            InitializeComponent();

            this.Hide();

            soc = socket;
            Label.CheckForIllegalCrossThreadCalls = false;
            showBar_Current = new progressbar(showBar1);
            showBar_Total = new progressbar(showBar2);
            msg = new mMessage(showMsg);
            //m_OpenProgress = new d_OpenProgress(OpenProgressBar);
        }


        void showMsg(string value)
        {
            MessageBox.Show(value);
            Environment.Exit(0);
        }

        string GetValueSize(Int64 value)
        {
            try
            {
                string strSize = "";
                double deci = 0;
                if (value < 1024)
                {
                    strSize = value + "B";
                }
                else
                {
                    deci = value % 1024;
                    value = value / 1024;
                    if (value < 1024)
                    {
                        if (deci != 0)
                        {
                            deci = Math.Round(deci / 1024, 3)+value;
                            strSize = deci + "K";
                        }
                        else
                        {
                            strSize = value + "K";
                        }
                    }
                    else
                    {
                        deci = value % 1024;
                        value = value / 1024;
                        if (deci != 0)
                        {
                            deci = Math.Round(deci / 1024, 3) + value;
                            strSize = deci + "M";
                        }
                        else
                        {
                            strSize = value + "M";
                        }
                        //strSize = value + "M";
                    }
                }

                return strSize;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        Brush brush = new SolidBrush(Color.DodgerBlue);//画刷-进度条前景色
        Brush brush2 = new SolidBrush(Color.FromArgb(210, 210, 210));//画刷-进度条背景色 
        Font fntProgress = new Font("微软雅黑", 10, FontStyle.Bold);//正文文字-进度条数字
        //画背景
        Brush brush3 = new SolidBrush(Color.FromArgb(200, 255, 255, 255));//窗体背景色

        Int64 CurrentTotalValue = 0;
        string strCurrentTotalValue = "";
        void showBar1(Int64 currentValue, Int64 totalValue)
        {
            if (totalValue != CurrentTotalValue)
            {
                CurrentTotalValue = totalValue;
                strCurrentTotalValue = GetValueSize(totalValue);
            }

            int value = (int)((double)currentValue / (double)totalValue * 390);
            Bitmap bmp = new Bitmap(bmpBG);
            Graphics g = Graphics.FromImage(bmp);
            string str = "";
            str = GetValueSize(currentValue);

            str = str + "/" + strCurrentTotalValue;

            float width = g.MeasureString(str, fntProgress).Width;

            g.DrawString(str, fntProgress, brush, 395 - (int)width, 55);
            g.FillRectangle(brush, new Rectangle(5, 75, value, 8));

            customForm.SetBits(bmp, this);
            g.Dispose();
        }

        void showBar2(Int64 currentValue, Int64 totalValue)
        {
            try
            {
                int value = (int)((double)currentValue / (double)totalValue * 390);
                Graphics g = Graphics.FromImage(bmpBG);
                g.FillRectangle(brush, new Rectangle(5, 87, value, 8));
                customForm.SetBits(bmpBG, this);
                g.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void download()
        {
            try
            {
                //发送本地版本信息
                SendVersion();
                string strVersion = RecvServerFileInfo();
                if (strVersion == string.Empty || strVersion == "0")
                {
                    //空代表没有要更新的，0代表更新服务停止了
                    //没有更新文件就结束,启用主程序
                    StartAppMain(strVersion);
                    return;
                }
                else
                {
                   //显示更新原因
                    frmReason rfrmReason = new frmReason((strVersion.Replace(":", "").Trim().Split('*'))[2]);
                    rfrmReason.ShowDialog();
                    this.Show();
                    this.Update();
                }
                //发送本地所有文件信息
                SendLocalFileInfo();
                //接受服务端发来的要更新的文件信息
                string strServerFileInfo = RecvServerFileInfo();
                if (strServerFileInfo == string.Empty)
                {
                    //没有更新文件就结束,启用主程序
                    StartAppMain(strVersion);
                    return;
                }
                else
                {
                    //如果等于0 表示服务停止更新了
                    if (strServerFileInfo=="0")
                    {
                        //启用主程序
                        StartAppMain(strVersion);
                        return;
                    }

                    //检查该程序有没有已经在运行，如果运行则给出提示框，提示关闭继续更新或者下次更新
                    if (CheckAppMainIsRun())
                    {
                        int frmMsgCount = 0;
                        while (true)
                        {
                            this.Hide();
                            frmMsg rfrmMsg = new frmMsg(frmMsgCount);
                            if (rfrmMsg.ShowDialog() == DialogResult.OK)//ok代表关闭主程序继续更新
                            {
                                //关闭主程序
                                if (!CloseAppMain())
                                {
                                   // this.Invoke(m_OpenProgress,new object[]{});
                                    this.Show();
                                    //有的话就发送个任意字符串给服务端表示已经接收到了
                                    soc.Send(Encoding.Unicode.GetBytes("?"));
                                    //开始接收更新文件
                                    RecvUpdateFiles(strServerFileInfo);//文件接收完毕后已调用socket close，所以不需要再调用
                                    StartAppMain(strVersion);
                                }
                                else
                                {
                                    //自动关闭主程序失败后
                                    frmMsgCount++;
                                    continue;
                                }
                            }
                            else
                            {
                                //选择下次更新的话则启动程序
                                //启用主程序
                                StartAppMain();
                                return;
                            }
                        }
                    }
                    else
                    {
                        //this.Invoke(m_OpenProgress, new object[] { });
                        //么有主程序运行就开始直接更新了
                        //有的话就发送个任意字符串给服务端表示已经接收到了
                        soc.Send(Encoding.UTF8.GetBytes("?"));
                        //开始接收更新文件
                        RecvUpdateFiles(strServerFileInfo);
                        //更新完成后启动主程序
                        StartAppMain(strVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke(msg, new object[] { "更新失败，异常信息如下\n"+ex.Message});
                //clientSocket.Close();
            }
        }


        void StartAppMain(string strVersion="")
        {
            try
            {
                soc.Close();
                //更新版本
                if (strVersion != string.Empty && strVersion != "0")
                {
                    UpdateVersionFile(strVersion);
                }

                ////重新加载xml配置信息获取主程序名
                string errMag = clsConfig.LoadXmlData();
                ////加载xml中的配置信息
                //if (errMag!=string.Empty)
                //{
                //    MessageBox.Show(errMag);
                //    Environment.Exit(0);
                //}

                //启动成功后务必要关闭自己
                //注释：直接点击和process.start()还是有点区别的
                //当这个程序有配置文件，或在启动时需要读取其他文件时，请配置一下 StartInfo 的 WorkingDirectory 属性为你的应用程序目录。
                ProcessStartInfo psInfo = new ProcessStartInfo();
                psInfo.FileName = Application.StartupPath + "\\" + clsConfig.appMainName;
                psInfo.UseShellExecute = false;
                psInfo.WorkingDirectory = Application.StartupPath + "\\" + clsConfig.appMainName.Substring(0, clsConfig.appMainName.LastIndexOf('\\'));//获取最后出现\\的位置索引
                psInfo.CreateNoWindow = true;

                if (File.Exists(psInfo.FileName))
                {
                    Process.Start(psInfo);
                    while (true)
                    {
                        Thread.Sleep(10);
                        if (CheckAppMainIsRun())
                        {
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    this.Invoke(msg, new object[] { "未找到启动程序\n程序路径：" + psInfo.FileName });
                    //MessageBox.Show("未找到启动程序\n程序路径：" + psInfo.FileName);
                    //Environment.Exit(0);
                }
            }
            catch (Exception)
            { throw; }
        }

        bool CloseAppMain()
        {
            //关闭后要检查一下是否真的关了，可以调用 CheckAppMainIsRun()
            //先检查是防止人为关闭了在点击更新，这样先检查一下就不会出错
            if (CheckAppMainIsRun())
            {
                string fileName = clsConfig.appMainName.Substring(clsConfig.appMainName.LastIndexOf('\\') + 1, clsConfig.appMainName.Length - clsConfig.appMainName.LastIndexOf('\\') - 1);
                fileName = fileName.Split('.')[0];
                Process[] p = Process.GetProcessesByName(fileName);
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].Kill();
                }
                Thread.Sleep(1000);
                return CheckAppMainIsRun();
            }
            else
            {
                return false;
            }
        }

        bool CheckAppMainIsRun()
        {
            string fileName = clsConfig.appMainName.Substring(clsConfig.appMainName.LastIndexOf('\\') + 1, clsConfig.appMainName.Length - clsConfig.appMainName.LastIndexOf('\\') - 1);
            fileName = fileName.Split('.')[0];
            Process[] p = Process.GetProcessesByName(fileName);
            if (p.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void RecvUpdateFiles(string strFileInfo)
        {
            try
            {
                InitDrawPropress();

                if (File.Exists("temp.update"))
                {
                    File.Delete("temp.update");
                }
                //先解析字符串  //:文件与大小的分隔符，|文件之间的分隔符，?结束符
                string[] fileInfo = strFileInfo.Split('|');

                FileStream fs = null;
                byte[] buffer = new byte[8192];
                int len = 0;
                Int64 fileLen = 0;
                Int64 totalLen = 0;
                Int64 CurrentTotalLen = 0;
                string fileName = "";

                int lastOver = 0;
                for (int i = 0; i < fileInfo.Length; i++)
                {
                    this.Invoke(showBar_Total, new object[] { i + 1, fileInfo.Length });//

                    fileName = fileInfo[i].Split('*')[0];//0 是文件名，1 是文件带下，2 是文件时间
                    //string[] temp = fileName.Split(new string[] { @"\" }, StringSplitOptions.None);
                    //fileName =clsConfig.appPath+"\\"+ temp[temp.Length - 1];
                    CheckIsExistDir(fileName);//检查是否包含文件夹路径，包含则检查文件夹是否存在，不存在则创建
                    fileLen = Convert.ToInt64(fileInfo[i].Split('*')[1]);
                    totalLen = fileLen;
                    fs = new FileStream("temp.update", FileMode.Create, FileAccess.Write);
                    while (true)
                    {
                        if (lastOver == 0)
                        {
                            len = soc.Receive(buffer, 0, 8192, SocketFlags.None);
                        }
                        if (fileLen > (len - lastOver))
                        {
                            CurrentTotalLen += len - lastOver;
                            this.Invoke(showBar_Current, new object[] { (Int64)CurrentTotalLen, (Int64)totalLen });//

                            fs.Write(buffer, lastOver, len - lastOver);
                            fileLen = fileLen - (len - lastOver);
                            lastOver = 0;
                        }
                        else
                        {
                            CurrentTotalLen += fileLen;
                            this.Invoke(showBar_Current, new object[] { (Int64)CurrentTotalLen, (Int64)totalLen });//
                            CurrentTotalLen = 0;

                            fs.Write(buffer, lastOver, (int)fileLen);
                            fs.Close();
                            File.Copy("temp.update", fileName, true);
                            FileInfo infoTime = new FileInfo(fileName);
                            infoTime.LastWriteTime = Convert.ToDateTime(fileInfo[i].Split('*')[2]);
                            if ((lastOver + fileLen) < len)
                            {
                                lastOver += (int)fileLen;
                                //lblLen.Text = (len - lastOver).ToString();////////测试用，后续会删掉
                            }
                            else
                            {
                                //lblLen.Text = (len - lastOver - fileLen).ToString();////////测试用，后续会删掉
                                lastOver = 0;
                            }
                            break;
                        }
                    }

                }

                if (File.Exists("temp.update"))
                {
                    File.Delete("temp.update");
                }
                soc.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        void CheckIsExistDir(string fileName)
        {
            if (fileName.Contains('\\'))
            {
                string dir = fileName.Substring(0, fileName.LastIndexOf('\\'));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }

        string RecvServerFileInfo()
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

        private void SendVersion()
        {
            try
            {
                soc.Send(Encoding.Unicode.GetBytes(":"+clsConfig.updateversion+"?"));
            }
            catch (Exception)
            {               
                throw;
            }
        }

        void SendLocalFileInfo()
        {
            try
            {
                //获取文件信息
                string strInfo = "";
                if (!Directory.Exists(clsConfig.appPath))
                {
                    strInfo = "?";//?表示结束符号
                }
                else
                {
                    string[] files = Directory.GetFiles(clsConfig.appPath, "*.*", SearchOption.AllDirectories);
                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo info = new FileInfo(files[i]);
                        strInfo += files[i]  + "*"+info.LastWriteTime.ToString("yyyyMMddHHmmss")+"|";//*表示分隔符号，| 表示文件之间的分隔符号
                    }

                    FileInfo infoSet = new FileInfo("set.xml");
                    strInfo += "set.xml*" + infoSet.LastWriteTime.ToString("yyyyMMddHHmmss")+"?";
                }
                soc.Send(Encoding.Unicode.GetBytes(strInfo));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                //bmpBG = new Bitmap(400, 105);
                //Graphics g = Graphics.FromImage(bmpBG);
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //Font font = new Font("微软雅黑", 18, FontStyle.Bold);//正文文字 
                ////Brush brushFont = new SolidBrush(Color.FromArgb(62, 179, 255));//画刷 
                //Brush brushFont = new SolidBrush(Color.DodgerBlue);//画刷 
                //Brush brushProgress = new SolidBrush(Color.FromArgb(210,210,210));//画刷 

                //g.FillRectangle(brush3, 0,0, 400, 105);
                ////缩图属性
                //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.SmoothingMode = SmoothingMode.HighQuality;
                //g.CompositingQuality = CompositingQuality.HighQuality;

                //g.DrawImage(Properties.Resources.yun1, new Rectangle(10, 0, 80, 80), new Rectangle(0, 0, 373, 377), GraphicsUnit.Pixel);
                //g.FillRectangle(brushProgress, new Rectangle(5, 75, 390, 8));
                //g.FillRectangle(brushProgress, new Rectangle(5, 87, 390, 8));
                //g.DrawString("检查更新", font, brushFont, 95, 40);
                //customForm.SetBits(bmpBG, this);
                //g.Dispose();

                Thread td = new Thread(download);
                td.IsBackground = true;
                td.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
                
            }
        }

        private void InitDrawPropress()
        {
            try
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;

                bmpBG = new Bitmap(400, 105);
                Graphics g = Graphics.FromImage(bmpBG);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                Font font = new Font("微软雅黑", 18, FontStyle.Bold);//正文文字 
                //Brush brushFont = new SolidBrush(Color.FromArgb(62, 179, 255));//画刷 
                Brush brushFont = new SolidBrush(Color.DodgerBlue);//画刷 
                Brush brushProgress = new SolidBrush(Color.FromArgb(210, 210, 210));//画刷 

                g.FillRectangle(brush3, 0, 0, 400, 105);
                //缩图属性
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(Properties.Resources.yun1, new Rectangle(10, 0, 80, 80), new Rectangle(0, 0, 373, 377), GraphicsUnit.Pixel);
                g.FillRectangle(brushProgress, new Rectangle(5, 75, 390, 8));
                g.FillRectangle(brushProgress, new Rectangle(5, 87, 390, 8));
                g.DrawString("检查更新", font, brushFont, 95, 40);
                customForm.SetBits(bmpBG, this);
                g.Dispose();
            }
            catch (Exception)
            {               
                throw;
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_MouseDown(object sender, MouseEventArgs e)
        {
            CustomForm.ReleaseCapture();
            CustomForm.SendMessage(this.Handle, 0x0112, 0xF010 + 0x0002, 0);//窗体移动
        }

        private void UpdateVersionFile(string VersionInfo)
        {
            try
            {
                if (!File.Exists("Version.xml"))
                {
                    CreateXML("Version.xml");
                }
                string[] info = VersionInfo.Replace(":","").Trim().Split('*');
                XmlDocument doc = new XmlDocument();
                doc.Load("Version.xml");
                XmlNode root = doc.SelectSingleNode("root");
                (root.SelectSingleNode("updatetime")).InnerText = info[1];
                (root.SelectSingleNode("updatemsg")).InnerText = info[2];
                (root.SelectSingleNode("updateversion")).InnerText = info[0];
               // clsConfig.updatemsg = info[2];

                doc.Save("Version.xml");
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

                //创建子节点
                XmlNode childNode = xmlDoc.CreateNode(XmlNodeType.Element, "updateversion", null);
                childNode.InnerText = "";
                root.AppendChild(childNode);
                childNode = xmlDoc.CreateNode(XmlNodeType.Element, "updatetime", null);
                childNode.InnerText = "";
                root.AppendChild(childNode);
                childNode = xmlDoc.CreateNode(XmlNodeType.Element, "updatemsg", null);
                childNode.InnerText = "";
                root.AppendChild(childNode);
                xmlDoc.Save(xmlName);
            }
            catch (Exception)
            {
                throw;
            }
        }




    }
}

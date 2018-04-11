using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPAddress address = IPAddress.Parse(serverIP);
            //soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //soc.Connect(new IPEndPoint(address, int.Parse(serverPort)));

            for (int i = 0; i < 1; i++)
            {
                Thread td = new Thread(td_Test);
                td.IsBackground = true;
                td.Start();
            }
        }
        private void td_Test()
        {
            while (true)
            {
                Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse("172.20.22.54");
                soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soc.Connect(new IPEndPoint(address, 8001));
                //soc.Send(Encoding.Unicode.GetBytes("?"));
                byte[] buffer=new byte[1024];
                //Thread.Sleep(100);
                //int count = soc.Receive(buffer, 0, 1024, SocketFlags.None);
                Thread.Sleep(10);
                 soc.Disconnect(true);
                soc.Close();
                soc.Dispose();
                Thread.Sleep(10);
            }
        }



    }
}

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
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPAddress address = IPAddress.Parse(serverIP);
            //soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //soc.Connect(new IPEndPoint(address, int.Parse(serverPort)));

            for (int i = 0; i < 50; i++)
            {
                Thread td = new Thread(td_Test);
                td.IsBackground = true;
                td.Start();
            }
        }
        private void td_Test()
        {
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse("192.168.252.7");
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.Connect(new IPEndPoint(address, 8381));
            Thread.Sleep(1000*20);
            soc.Close();
        }



    }
}

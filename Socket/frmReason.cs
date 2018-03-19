using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class frmReason : Form
    {
        public frmReason()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.Trim();
            if (richTextBox1.Text == string.Empty)
            {
                MessageBox.Show("原因不可为空");
                return;
            }
            if (richTextBox1.Text.Contains("*") || richTextBox1.Text.Contains("?") || richTextBox1.Text.Contains(":"))
            {
                MessageBox.Show("文本中不可含 * : ? 字符，它已作为特殊用处");
                return;
            }
            clsConfig.TempMsg = richTextBox1.Text;
            this.DialogResult = DialogResult.OK;
        }



    }
}

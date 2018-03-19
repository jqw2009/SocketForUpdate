using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Update
{
    public partial class frmMsg : Form
    {

        public frmMsg(int count)
        {
            InitializeComponent();
            if (count > 0)
            {
                lblMsg.Text = "关闭主程序失败，请选择";
                btnOK.Text = "继续关闭并更新";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult=DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

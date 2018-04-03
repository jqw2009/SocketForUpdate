namespace Server
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnListen = new System.Windows.Forms.Button();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.timer_stopServer = new System.Windows.Forms.Timer(this.components);
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.timer_count = new System.Windows.Forms.Timer(this.components);
            this.txtConnectIPs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblUpdateTime = new System.Windows.Forms.Label();
            this.btnUpdateVersion = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cboxRate = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnListen
            // 
            this.btnListen.Location = new System.Drawing.Point(14, 204);
            this.btnListen.Name = "btnListen";
            this.btnListen.Size = new System.Drawing.Size(100, 37);
            this.btnListen.TabIndex = 0;
            this.btnListen.Text = "开启服务";
            this.btnListen.UseVisualStyleBackColor = true;
            this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
            // 
            // txtIp
            // 
            this.txtIp.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIp.Location = new System.Drawing.Point(116, 60);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(156, 26);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "172.20.22.53";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "服务IP：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "服务Port：";
            // 
            // txtPort
            // 
            this.txtPort.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPort.Location = new System.Drawing.Point(116, 99);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(156, 26);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "10240";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(154, 204);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 37);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "启用更新";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // timer_stopServer
            // 
            this.timer_stopServer.Tick += new System.EventHandler(this.timer_stopServer_Tick);
            // 
            // timer_update
            // 
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "当前连接数：";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblCount.Location = new System.Drawing.Point(123, 25);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(35, 16);
            this.lblCount.TabIndex = 7;
            this.lblCount.Text = "---";
            // 
            // timer_count
            // 
            this.timer_count.Interval = 500;
            this.timer_count.Tick += new System.EventHandler(this.timer_count_Tick);
            // 
            // txtConnectIPs
            // 
            this.txtConnectIPs.BackColor = System.Drawing.Color.White;
            this.txtConnectIPs.Location = new System.Drawing.Point(308, 55);
            this.txtConnectIPs.Multiline = true;
            this.txtConnectIPs.Name = "txtConnectIPs";
            this.txtConnectIPs.ReadOnly = true;
            this.txtConnectIPs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConnectIPs.Size = new System.Drawing.Size(247, 320);
            this.txtConnectIPs.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(305, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 14);
            this.label4.TabIndex = 9;
            this.label4.Text = "当前连接IP：";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(15, 275);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(77, 14);
            this.lblVersion.TabIndex = 10;
            this.lblVersion.Text = "更新版本：";
            // 
            // lblUpdateTime
            // 
            this.lblUpdateTime.AutoSize = true;
            this.lblUpdateTime.Location = new System.Drawing.Point(15, 308);
            this.lblUpdateTime.Name = "lblUpdateTime";
            this.lblUpdateTime.Size = new System.Drawing.Size(224, 14);
            this.lblUpdateTime.TabIndex = 11;
            this.lblUpdateTime.Text = "更新时间：2017-08-07 00：00：00";
            // 
            // btnUpdateVersion
            // 
            this.btnUpdateVersion.Location = new System.Drawing.Point(18, 350);
            this.btnUpdateVersion.Name = "btnUpdateVersion";
            this.btnUpdateVersion.Size = new System.Drawing.Size(100, 37);
            this.btnUpdateVersion.TabIndex = 12;
            this.btnUpdateVersion.Text = "升级版本";
            this.btnUpdateVersion.UseVisualStyleBackColor = true;
            this.btnUpdateVersion.Click += new System.EventHandler(this.btnUpdateVersion_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "传输速度(K)：";
            // 
            // cboxRate
            // 
            this.cboxRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboxRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboxRate.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cboxRate.FormattingEnabled = true;
            this.cboxRate.Items.AddRange(new object[] {
            "8",
            "10",
            "15",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.cboxRate.Location = new System.Drawing.Point(116, 138);
            this.cboxRate.Name = "cboxRate";
            this.cboxRate.Size = new System.Drawing.Size(156, 24);
            this.cboxRate.TabIndex = 15;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 400);
            this.Controls.Add(this.cboxRate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnUpdateVersion);
            this.Controls.Add(this.lblUpdateTime);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtConnectIPs);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.btnListen);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "更新服务V20180403_1122";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnListen;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Timer timer_stopServer;
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Timer timer_count;
        private System.Windows.Forms.TextBox txtConnectIPs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblUpdateTime;
        private System.Windows.Forms.Button btnUpdateVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboxRate;
    }
}


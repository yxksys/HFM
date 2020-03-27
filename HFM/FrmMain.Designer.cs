namespace HFM
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MnsFunction = new System.Windows.Forms.MenuStrip();
            this.SystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartRunningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuperUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RetreatSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ParameterSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaintainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TestHardwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HistoricalRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutHFMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StsFoot = new System.Windows.Forms.StatusStrip();
            this.Tsslbl_Name = new System.Windows.Forms.ToolStripStatusLabel();
            this.Tsslbl_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.Tsslbl_Tcp = new System.Windows.Forms.ToolStripStatusLabel();
            this.Tsslbl_NowTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.MnsFunction.SuspendLayout();
            this.StsFoot.SuspendLayout();
            this.SuspendLayout();
            // 
            // MnsFunction
            // 
            this.MnsFunction.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SystemToolStripMenuItem,
            this.SetUpToolStripMenuItem,
            this.MaintainToolStripMenuItem,
            this.RecordToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.MnsFunction.Location = new System.Drawing.Point(0, 0);
            this.MnsFunction.Name = "MnsFunction";
            this.MnsFunction.Size = new System.Drawing.Size(792, 28);
            this.MnsFunction.TabIndex = 1;
            this.MnsFunction.Text = "menuStrip1";
            // 
            // SystemToolStripMenuItem
            // 
            this.SystemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartRunningToolStripMenuItem,
            this.SuperUserToolStripMenuItem,
            this.ChangePasswordToolStripMenuItem,
            this.RetreatSystemToolStripMenuItem});
            this.SystemToolStripMenuItem.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.SystemToolStripMenuItem.Name = "SystemToolStripMenuItem";
            this.SystemToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.SystemToolStripMenuItem.Text = "系统(S)";
            // 
            // StartRunningToolStripMenuItem
            // 
            this.StartRunningToolStripMenuItem.Name = "StartRunningToolStripMenuItem";
            this.StartRunningToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.StartRunningToolStripMenuItem.Text = "开始运行";
            this.StartRunningToolStripMenuItem.Click += new System.EventHandler(this.StartRunningToolStripMenuItem_Click);
            // 
            // SuperUserToolStripMenuItem
            // 
            this.SuperUserToolStripMenuItem.Name = "SuperUserToolStripMenuItem";
            this.SuperUserToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.SuperUserToolStripMenuItem.Text = "超级用户";
            this.SuperUserToolStripMenuItem.Click += new System.EventHandler(this.SuperUserToolStripMenuItem_Click);
            // 
            // ChangePasswordToolStripMenuItem
            // 
            this.ChangePasswordToolStripMenuItem.Name = "ChangePasswordToolStripMenuItem";
            this.ChangePasswordToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.ChangePasswordToolStripMenuItem.Text = "修改密码";
            this.ChangePasswordToolStripMenuItem.Click += new System.EventHandler(this.ChangePasswordToolStripMenuItem_Click);
            // 
            // RetreatSystemToolStripMenuItem
            // 
            this.RetreatSystemToolStripMenuItem.Name = "RetreatSystemToolStripMenuItem";
            this.RetreatSystemToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.RetreatSystemToolStripMenuItem.Text = "退出系统";
            this.RetreatSystemToolStripMenuItem.Click += new System.EventHandler(this.RetreatSystemToolStripMenuItem_Click);
            // 
            // SetUpToolStripMenuItem
            // 
            this.SetUpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ParameterSettingToolStripMenuItem});
            this.SetUpToolStripMenuItem.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.SetUpToolStripMenuItem.Name = "SetUpToolStripMenuItem";
            this.SetUpToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.SetUpToolStripMenuItem.Text = "设置(P)";
            // 
            // ParameterSettingToolStripMenuItem
            // 
            this.ParameterSettingToolStripMenuItem.Name = "ParameterSettingToolStripMenuItem";
            this.ParameterSettingToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.ParameterSettingToolStripMenuItem.Text = "参数设置";
            this.ParameterSettingToolStripMenuItem.Click += new System.EventHandler(this.ParameterSettingToolStripMenuItem_Click);
            // 
            // MaintainToolStripMenuItem
            // 
            this.MaintainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CalibrationToolStripMenuItem,
            this.TestHardwareToolStripMenuItem});
            this.MaintainToolStripMenuItem.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.MaintainToolStripMenuItem.Name = "MaintainToolStripMenuItem";
            this.MaintainToolStripMenuItem.Size = new System.Drawing.Size(68, 24);
            this.MaintainToolStripMenuItem.Text = "维护(T)";
            // 
            // RecordToolStripMenuItem
            // 
            this.RecordToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HistoricalRecordToolStripMenuItem});
            this.RecordToolStripMenuItem.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.RecordToolStripMenuItem.Name = "RecordToolStripMenuItem";
            this.RecordToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.RecordToolStripMenuItem.Text = "记录(R)";
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutHFMToolStripMenuItem});
            this.AboutToolStripMenuItem.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.AboutToolStripMenuItem.Text = "关于(A)";
            // 
            // CalibrationToolStripMenuItem
            // 
            this.CalibrationToolStripMenuItem.Name = "CalibrationToolStripMenuItem";
            this.CalibrationToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.CalibrationToolStripMenuItem.Text = "测试刻度";
            this.CalibrationToolStripMenuItem.Click += new System.EventHandler(this.CalibrationToolStripMenuItem_Click);
            // 
            // TestHardwareToolStripMenuItem
            // 
            this.TestHardwareToolStripMenuItem.Name = "TestHardwareToolStripMenuItem";
            this.TestHardwareToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.TestHardwareToolStripMenuItem.Text = "硬件检测";
            this.TestHardwareToolStripMenuItem.Click += new System.EventHandler(this.TestHardwareToolStripMenuItem_Click);
            // 
            // HistoricalRecordToolStripMenuItem
            // 
            this.HistoricalRecordToolStripMenuItem.Name = "HistoricalRecordToolStripMenuItem";
            this.HistoricalRecordToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.HistoricalRecordToolStripMenuItem.Text = "历史记录";
            this.HistoricalRecordToolStripMenuItem.Click += new System.EventHandler(this.HistoricalRecordToolStripMenuItem_Click);
            // 
            // AboutHFMToolStripMenuItem
            // 
            this.AboutHFMToolStripMenuItem.Name = "AboutHFMToolStripMenuItem";
            this.AboutHFMToolStripMenuItem.Size = new System.Drawing.Size(180, 24);
            this.AboutHFMToolStripMenuItem.Text = "关于HFM";
            this.AboutHFMToolStripMenuItem.Click += new System.EventHandler(this.AboutHFMToolStripMenuItem_Click);
            // 
            // StsFoot
            // 
            this.StsFoot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsslbl_Name,
            this.Tsslbl_Status,
            this.Tsslbl_Tcp,
            this.Tsslbl_NowTime});
            this.StsFoot.Location = new System.Drawing.Point(0, 541);
            this.StsFoot.Name = "StsFoot";
            this.StsFoot.Size = new System.Drawing.Size(792, 25);
            this.StsFoot.TabIndex = 2;
            this.StsFoot.Text = "statusStrip1";
            // 
            // Tsslbl_Name
            // 
            this.Tsslbl_Name.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.Tsslbl_Name.Name = "Tsslbl_Name";
            this.Tsslbl_Name.Size = new System.Drawing.Size(152, 20);
            this.Tsslbl_Name.Text = "HFM手脚污染监测仪|";
            // 
            // Tsslbl_Status
            // 
            this.Tsslbl_Status.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.Tsslbl_Status.Name = "Tsslbl_Status";
            this.Tsslbl_Status.Size = new System.Drawing.Size(73, 20);
            this.Tsslbl_Status.Text = "通讯状态";
            // 
            // Tsslbl_Tcp
            // 
            this.Tsslbl_Tcp.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.Tsslbl_Tcp.Name = "Tsslbl_Tcp";
            this.Tsslbl_Tcp.Size = new System.Drawing.Size(105, 20);
            this.Tsslbl_Tcp.Text = "网络连接状态";
            // 
            // Tsslbl_NowTime
            // 
            this.Tsslbl_NowTime.AutoSize = false;
            this.Tsslbl_NowTime.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.Tsslbl_NowTime.Name = "Tsslbl_NowTime";
            this.Tsslbl_NowTime.Size = new System.Drawing.Size(429, 20);
            this.Tsslbl_NowTime.Text = "2020年3月27日 18点42分";
            this.Tsslbl_NowTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.StsFoot);
            this.Controls.Add(this.MnsFunction);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.MnsFunction;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HFM100 手脚表面污染监测仪";
            this.MnsFunction.ResumeLayout(false);
            this.MnsFunction.PerformLayout();
            this.StsFoot.ResumeLayout(false);
            this.StsFoot.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MnsFunction;
        private System.Windows.Forms.ToolStripMenuItem SystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StartRunningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SuperUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ChangePasswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RetreatSystemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ParameterSettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MaintainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CalibrationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TestHardwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HistoricalRecordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutHFMToolStripMenuItem;
        private System.Windows.Forms.StatusStrip StsFoot;
        private System.Windows.Forms.ToolStripStatusLabel Tsslbl_Name;
        private System.Windows.Forms.ToolStripStatusLabel Tsslbl_Status;
        private System.Windows.Forms.ToolStripStatusLabel Tsslbl_Tcp;
        private System.Windows.Forms.ToolStripStatusLabel Tsslbl_NowTime;
    }
}
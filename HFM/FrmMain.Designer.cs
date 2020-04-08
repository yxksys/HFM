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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.MnsFunction = new System.Windows.Forms.MenuStrip();
            this.SystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartRunningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuperUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RetreatSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ParameterSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaintainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TestHardwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HistoricalRecordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            resources.ApplyResources(this.MnsFunction, "MnsFunction");
            this.MnsFunction.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SystemToolStripMenuItem,
            this.SetUpToolStripMenuItem,
            this.MaintainToolStripMenuItem,
            this.RecordToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.MnsFunction.Name = "MnsFunction";
            // 
            // SystemToolStripMenuItem
            // 
            resources.ApplyResources(this.SystemToolStripMenuItem, "SystemToolStripMenuItem");
            this.SystemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartRunningToolStripMenuItem,
            this.SuperUserToolStripMenuItem,
            this.ChangePasswordToolStripMenuItem,
            this.RetreatSystemToolStripMenuItem});
            this.SystemToolStripMenuItem.Name = "SystemToolStripMenuItem";
            // 
            // StartRunningToolStripMenuItem
            // 
            resources.ApplyResources(this.StartRunningToolStripMenuItem, "StartRunningToolStripMenuItem");
            this.StartRunningToolStripMenuItem.Name = "StartRunningToolStripMenuItem";
            this.StartRunningToolStripMenuItem.Click += new System.EventHandler(this.StartRunningToolStripMenuItem_Click);
            // 
            // SuperUserToolStripMenuItem
            // 
            resources.ApplyResources(this.SuperUserToolStripMenuItem, "SuperUserToolStripMenuItem");
            this.SuperUserToolStripMenuItem.Name = "SuperUserToolStripMenuItem";
            this.SuperUserToolStripMenuItem.Click += new System.EventHandler(this.SuperUserToolStripMenuItem_Click);
            // 
            // ChangePasswordToolStripMenuItem
            // 
            resources.ApplyResources(this.ChangePasswordToolStripMenuItem, "ChangePasswordToolStripMenuItem");
            this.ChangePasswordToolStripMenuItem.Name = "ChangePasswordToolStripMenuItem";
            this.ChangePasswordToolStripMenuItem.Click += new System.EventHandler(this.ChangePasswordToolStripMenuItem_Click);
            // 
            // RetreatSystemToolStripMenuItem
            // 
            resources.ApplyResources(this.RetreatSystemToolStripMenuItem, "RetreatSystemToolStripMenuItem");
            this.RetreatSystemToolStripMenuItem.Name = "RetreatSystemToolStripMenuItem";
            this.RetreatSystemToolStripMenuItem.Click += new System.EventHandler(this.RetreatSystemToolStripMenuItem_Click);
            // 
            // SetUpToolStripMenuItem
            // 
            resources.ApplyResources(this.SetUpToolStripMenuItem, "SetUpToolStripMenuItem");
            this.SetUpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ParameterSettingToolStripMenuItem});
            this.SetUpToolStripMenuItem.Name = "SetUpToolStripMenuItem";
            // 
            // ParameterSettingToolStripMenuItem
            // 
            resources.ApplyResources(this.ParameterSettingToolStripMenuItem, "ParameterSettingToolStripMenuItem");
            this.ParameterSettingToolStripMenuItem.Name = "ParameterSettingToolStripMenuItem";
            this.ParameterSettingToolStripMenuItem.Click += new System.EventHandler(this.ParameterSettingToolStripMenuItem_Click);
            // 
            // MaintainToolStripMenuItem
            // 
            resources.ApplyResources(this.MaintainToolStripMenuItem, "MaintainToolStripMenuItem");
            this.MaintainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CalibrationToolStripMenuItem,
            this.TestHardwareToolStripMenuItem});
            this.MaintainToolStripMenuItem.Name = "MaintainToolStripMenuItem";
            // 
            // CalibrationToolStripMenuItem
            // 
            resources.ApplyResources(this.CalibrationToolStripMenuItem, "CalibrationToolStripMenuItem");
            this.CalibrationToolStripMenuItem.Name = "CalibrationToolStripMenuItem";
            this.CalibrationToolStripMenuItem.Click += new System.EventHandler(this.CalibrationToolStripMenuItem_Click);
            // 
            // TestHardwareToolStripMenuItem
            // 
            resources.ApplyResources(this.TestHardwareToolStripMenuItem, "TestHardwareToolStripMenuItem");
            this.TestHardwareToolStripMenuItem.Name = "TestHardwareToolStripMenuItem";
            this.TestHardwareToolStripMenuItem.Click += new System.EventHandler(this.TestHardwareToolStripMenuItem_Click);
            // 
            // RecordToolStripMenuItem
            // 
            resources.ApplyResources(this.RecordToolStripMenuItem, "RecordToolStripMenuItem");
            this.RecordToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HistoricalRecordToolStripMenuItem});
            this.RecordToolStripMenuItem.Name = "RecordToolStripMenuItem";
            // 
            // HistoricalRecordToolStripMenuItem
            // 
            resources.ApplyResources(this.HistoricalRecordToolStripMenuItem, "HistoricalRecordToolStripMenuItem");
            this.HistoricalRecordToolStripMenuItem.Name = "HistoricalRecordToolStripMenuItem";
            this.HistoricalRecordToolStripMenuItem.Click += new System.EventHandler(this.HistoricalRecordToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            resources.ApplyResources(this.AboutToolStripMenuItem, "AboutToolStripMenuItem");
            this.AboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutHFMToolStripMenuItem});
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            // 
            // AboutHFMToolStripMenuItem
            // 
            resources.ApplyResources(this.AboutHFMToolStripMenuItem, "AboutHFMToolStripMenuItem");
            this.AboutHFMToolStripMenuItem.Name = "AboutHFMToolStripMenuItem";
            this.AboutHFMToolStripMenuItem.Click += new System.EventHandler(this.AboutHFMToolStripMenuItem_Click);
            // 
            // StsFoot
            // 
            resources.ApplyResources(this.StsFoot, "StsFoot");
            this.StsFoot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Tsslbl_Name,
            this.Tsslbl_Status,
            this.Tsslbl_Tcp,
            this.Tsslbl_NowTime});
            this.StsFoot.Name = "StsFoot";
            // 
            // Tsslbl_Name
            // 
            resources.ApplyResources(this.Tsslbl_Name, "Tsslbl_Name");
            this.Tsslbl_Name.Name = "Tsslbl_Name";
            // 
            // Tsslbl_Status
            // 
            resources.ApplyResources(this.Tsslbl_Status, "Tsslbl_Status");
            this.Tsslbl_Status.Name = "Tsslbl_Status";
            // 
            // Tsslbl_Tcp
            // 
            resources.ApplyResources(this.Tsslbl_Tcp, "Tsslbl_Tcp");
            this.Tsslbl_Tcp.Name = "Tsslbl_Tcp";
            // 
            // Tsslbl_NowTime
            // 
            resources.ApplyResources(this.Tsslbl_NowTime, "Tsslbl_NowTime");
            this.Tsslbl_NowTime.Name = "Tsslbl_NowTime";
            // 
            // FrmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StsFoot);
            this.Controls.Add(this.MnsFunction);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.MnsFunction;
            this.Name = "FrmMain";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
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
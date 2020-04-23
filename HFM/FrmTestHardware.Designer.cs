namespace HFM
{
    partial class FrmTestHardware
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTestHardware));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.GrpWork = new System.Windows.Forms.GroupBox();
            this.DgvWork = new System.Windows.Forms.DataGridView();
            this.LblBetacnt = new System.Windows.Forms.Label();
            this.LblBetacps = new System.Windows.Forms.Label();
            this.LblAlphacnt = new System.Windows.Forms.Label();
            this.LblAlphacps = new System.Windows.Forms.Label();
            this.LblV = new System.Windows.Forms.Label();
            this.LblRF = new System.Windows.Forms.Label();
            this.LblLF = new System.Windows.Forms.Label();
            this.LblRHB = new System.Windows.Forms.Label();
            this.LblRHP = new System.Windows.Forms.Label();
            this.LblLHB = new System.Windows.Forms.Label();
            this.LblLHP = new System.Windows.Forms.Label();
            this.LblStatus = new System.Windows.Forms.Label();
            this.LblβTotalcnt = new System.Windows.Forms.Label();
            this.Lblβcountrate = new System.Windows.Forms.Label();
            this.LblαTotalcnt = new System.Windows.Forms.Label();
            this.Lblαcountrate = new System.Windows.Forms.Label();
            this.LblHighVoltage = new System.Windows.Forms.Label();
            this.GrpFrisker = new System.Windows.Forms.GroupBox();
            this.LblFriskercps = new System.Windows.Forms.Label();
            this.TxtFriskercount = new System.Windows.Forms.TextBox();
            this.LblFriskercount = new System.Windows.Forms.Label();
            this.GrpSensorstate = new System.Windows.Forms.GroupBox();
            this.TxtFriskerState = new System.Windows.Forms.TextBox();
            this.LblFriskerState = new System.Windows.Forms.Label();
            this.TxtRHandState = new System.Windows.Forms.TextBox();
            this.LblRHandState = new System.Windows.Forms.Label();
            this.TxtLHandState = new System.Windows.Forms.TextBox();
            this.LblLHandState = new System.Windows.Forms.Label();
            this.GrpDetectorSelfTest = new System.Windows.Forms.GroupBox();
            this.BtnBetaCheck = new System.Windows.Forms.Button();
            this.BtnAlphaCheck = new System.Windows.Forms.Button();
            this.GrpSelfTestParameter = new System.Windows.Forms.GroupBox();
            this.Lblμs = new System.Windows.Forms.Label();
            this.BtnSelfCheck = new System.Windows.Forms.Button();
            this.TxtPWidth = new System.Windows.Forms.TextBox();
            this.CmbControl = new System.Windows.Forms.ComboBox();
            this.LblPWidth = new System.Windows.Forms.Label();
            this.LblControl = new System.Windows.Forms.Label();
            this.CmbPulse = new System.Windows.Forms.ComboBox();
            this.LblPulse = new System.Windows.Forms.Label();
            this.TxtCheckCount = new System.Windows.Forms.TextBox();
            this.LblSelfcount = new System.Windows.Forms.Label();
            this.LblTimeWork = new System.Windows.Forms.Label();
            this.bkWorkerReceiveData = new System.ComponentModel.BackgroundWorker();
            this.LHP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LHB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RHP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RHB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GrpWork.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvWork)).BeginInit();
            this.GrpFrisker.SuspendLayout();
            this.GrpSensorstate.SuspendLayout();
            this.GrpDetectorSelfTest.SuspendLayout();
            this.GrpSelfTestParameter.SuspendLayout();
            this.SuspendLayout();
            // 
            // GrpWork
            // 
            resources.ApplyResources(this.GrpWork, "GrpWork");
            this.GrpWork.Controls.Add(this.DgvWork);
            this.GrpWork.Controls.Add(this.LblBetacnt);
            this.GrpWork.Controls.Add(this.LblBetacps);
            this.GrpWork.Controls.Add(this.LblAlphacnt);
            this.GrpWork.Controls.Add(this.LblAlphacps);
            this.GrpWork.Controls.Add(this.LblV);
            this.GrpWork.Controls.Add(this.LblRF);
            this.GrpWork.Controls.Add(this.LblLF);
            this.GrpWork.Controls.Add(this.LblRHB);
            this.GrpWork.Controls.Add(this.LblRHP);
            this.GrpWork.Controls.Add(this.LblLHB);
            this.GrpWork.Controls.Add(this.LblLHP);
            this.GrpWork.Controls.Add(this.LblStatus);
            this.GrpWork.Controls.Add(this.LblβTotalcnt);
            this.GrpWork.Controls.Add(this.Lblβcountrate);
            this.GrpWork.Controls.Add(this.LblαTotalcnt);
            this.GrpWork.Controls.Add(this.Lblαcountrate);
            this.GrpWork.Controls.Add(this.LblHighVoltage);
            this.GrpWork.Name = "GrpWork";
            this.GrpWork.TabStop = false;
            // 
            // DgvWork
            // 
            this.DgvWork.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvWork.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DgvWork.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvWork.ColumnHeadersVisible = false;
            this.DgvWork.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LHP,
            this.LHB,
            this.RHP,
            this.RHB,
            this.LF,
            this.RF});
            resources.ApplyResources(this.DgvWork, "DgvWork");
            this.DgvWork.Name = "DgvWork";
            this.DgvWork.RowHeadersVisible = false;
            this.DgvWork.RowTemplate.Height = 35;
            // 
            // LblBetacnt
            // 
            resources.ApplyResources(this.LblBetacnt, "LblBetacnt");
            this.LblBetacnt.Name = "LblBetacnt";
            // 
            // LblBetacps
            // 
            resources.ApplyResources(this.LblBetacps, "LblBetacps");
            this.LblBetacps.Name = "LblBetacps";
            // 
            // LblAlphacnt
            // 
            resources.ApplyResources(this.LblAlphacnt, "LblAlphacnt");
            this.LblAlphacnt.Name = "LblAlphacnt";
            // 
            // LblAlphacps
            // 
            resources.ApplyResources(this.LblAlphacps, "LblAlphacps");
            this.LblAlphacps.Name = "LblAlphacps";
            // 
            // LblV
            // 
            resources.ApplyResources(this.LblV, "LblV");
            this.LblV.Name = "LblV";
            // 
            // LblRF
            // 
            resources.ApplyResources(this.LblRF, "LblRF");
            this.LblRF.Name = "LblRF";
            // 
            // LblLF
            // 
            resources.ApplyResources(this.LblLF, "LblLF");
            this.LblLF.Name = "LblLF";
            // 
            // LblRHB
            // 
            resources.ApplyResources(this.LblRHB, "LblRHB");
            this.LblRHB.Name = "LblRHB";
            // 
            // LblRHP
            // 
            resources.ApplyResources(this.LblRHP, "LblRHP");
            this.LblRHP.Name = "LblRHP";
            // 
            // LblLHB
            // 
            resources.ApplyResources(this.LblLHB, "LblLHB");
            this.LblLHB.Name = "LblLHB";
            // 
            // LblLHP
            // 
            resources.ApplyResources(this.LblLHP, "LblLHP");
            this.LblLHP.Name = "LblLHP";
            // 
            // LblStatus
            // 
            resources.ApplyResources(this.LblStatus, "LblStatus");
            this.LblStatus.Name = "LblStatus";
            // 
            // LblβTotalcnt
            // 
            resources.ApplyResources(this.LblβTotalcnt, "LblβTotalcnt");
            this.LblβTotalcnt.Name = "LblβTotalcnt";
            // 
            // Lblβcountrate
            // 
            resources.ApplyResources(this.Lblβcountrate, "Lblβcountrate");
            this.Lblβcountrate.Name = "Lblβcountrate";
            // 
            // LblαTotalcnt
            // 
            resources.ApplyResources(this.LblαTotalcnt, "LblαTotalcnt");
            this.LblαTotalcnt.Name = "LblαTotalcnt";
            // 
            // Lblαcountrate
            // 
            resources.ApplyResources(this.Lblαcountrate, "Lblαcountrate");
            this.Lblαcountrate.Name = "Lblαcountrate";
            // 
            // LblHighVoltage
            // 
            resources.ApplyResources(this.LblHighVoltage, "LblHighVoltage");
            this.LblHighVoltage.Name = "LblHighVoltage";
            // 
            // GrpFrisker
            // 
            this.GrpFrisker.Controls.Add(this.LblFriskercps);
            this.GrpFrisker.Controls.Add(this.TxtFriskercount);
            this.GrpFrisker.Controls.Add(this.LblFriskercount);
            resources.ApplyResources(this.GrpFrisker, "GrpFrisker");
            this.GrpFrisker.Name = "GrpFrisker";
            this.GrpFrisker.TabStop = false;
            // 
            // LblFriskercps
            // 
            resources.ApplyResources(this.LblFriskercps, "LblFriskercps");
            this.LblFriskercps.Name = "LblFriskercps";
            // 
            // TxtFriskercount
            // 
            this.TxtFriskercount.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.TxtFriskercount, "TxtFriskercount");
            this.TxtFriskercount.Name = "TxtFriskercount";
            this.TxtFriskercount.ReadOnly = true;
            // 
            // LblFriskercount
            // 
            resources.ApplyResources(this.LblFriskercount, "LblFriskercount");
            this.LblFriskercount.Name = "LblFriskercount";
            // 
            // GrpSensorstate
            // 
            this.GrpSensorstate.Controls.Add(this.TxtFriskerState);
            this.GrpSensorstate.Controls.Add(this.LblFriskerState);
            this.GrpSensorstate.Controls.Add(this.TxtRHandState);
            this.GrpSensorstate.Controls.Add(this.LblRHandState);
            this.GrpSensorstate.Controls.Add(this.TxtLHandState);
            this.GrpSensorstate.Controls.Add(this.LblLHandState);
            resources.ApplyResources(this.GrpSensorstate, "GrpSensorstate");
            this.GrpSensorstate.Name = "GrpSensorstate";
            this.GrpSensorstate.TabStop = false;
            // 
            // TxtFriskerState
            // 
            this.TxtFriskerState.BackColor = System.Drawing.Color.Orange;
            resources.ApplyResources(this.TxtFriskerState, "TxtFriskerState");
            this.TxtFriskerState.Name = "TxtFriskerState";
            this.TxtFriskerState.ReadOnly = true;
            // 
            // LblFriskerState
            // 
            resources.ApplyResources(this.LblFriskerState, "LblFriskerState");
            this.LblFriskerState.Name = "LblFriskerState";
            // 
            // TxtRHandState
            // 
            this.TxtRHandState.BackColor = System.Drawing.Color.Orange;
            resources.ApplyResources(this.TxtRHandState, "TxtRHandState");
            this.TxtRHandState.Name = "TxtRHandState";
            this.TxtRHandState.ReadOnly = true;
            // 
            // LblRHandState
            // 
            resources.ApplyResources(this.LblRHandState, "LblRHandState");
            this.LblRHandState.Name = "LblRHandState";
            // 
            // TxtLHandState
            // 
            this.TxtLHandState.BackColor = System.Drawing.Color.Orange;
            resources.ApplyResources(this.TxtLHandState, "TxtLHandState");
            this.TxtLHandState.Name = "TxtLHandState";
            this.TxtLHandState.ReadOnly = true;
            // 
            // LblLHandState
            // 
            resources.ApplyResources(this.LblLHandState, "LblLHandState");
            this.LblLHandState.Name = "LblLHandState";
            // 
            // GrpDetectorSelfTest
            // 
            this.GrpDetectorSelfTest.Controls.Add(this.BtnBetaCheck);
            this.GrpDetectorSelfTest.Controls.Add(this.BtnAlphaCheck);
            resources.ApplyResources(this.GrpDetectorSelfTest, "GrpDetectorSelfTest");
            this.GrpDetectorSelfTest.Name = "GrpDetectorSelfTest";
            this.GrpDetectorSelfTest.TabStop = false;
            // 
            // BtnBetaCheck
            // 
            resources.ApplyResources(this.BtnBetaCheck, "BtnBetaCheck");
            this.BtnBetaCheck.Name = "BtnBetaCheck";
            this.BtnBetaCheck.UseVisualStyleBackColor = true;
            this.BtnBetaCheck.Click += new System.EventHandler(this.BtnBetaCheck_Click);
            // 
            // BtnAlphaCheck
            // 
            resources.ApplyResources(this.BtnAlphaCheck, "BtnAlphaCheck");
            this.BtnAlphaCheck.Name = "BtnAlphaCheck";
            this.BtnAlphaCheck.UseVisualStyleBackColor = true;
            this.BtnAlphaCheck.Click += new System.EventHandler(this.BtnAlphaCheck_Click);
            // 
            // GrpSelfTestParameter
            // 
            this.GrpSelfTestParameter.Controls.Add(this.Lblμs);
            this.GrpSelfTestParameter.Controls.Add(this.BtnSelfCheck);
            this.GrpSelfTestParameter.Controls.Add(this.TxtPWidth);
            this.GrpSelfTestParameter.Controls.Add(this.CmbControl);
            this.GrpSelfTestParameter.Controls.Add(this.LblPWidth);
            this.GrpSelfTestParameter.Controls.Add(this.LblControl);
            this.GrpSelfTestParameter.Controls.Add(this.CmbPulse);
            this.GrpSelfTestParameter.Controls.Add(this.LblPulse);
            this.GrpSelfTestParameter.Controls.Add(this.TxtCheckCount);
            this.GrpSelfTestParameter.Controls.Add(this.LblSelfcount);
            resources.ApplyResources(this.GrpSelfTestParameter, "GrpSelfTestParameter");
            this.GrpSelfTestParameter.Name = "GrpSelfTestParameter";
            this.GrpSelfTestParameter.TabStop = false;
            // 
            // Lblμs
            // 
            resources.ApplyResources(this.Lblμs, "Lblμs");
            this.Lblμs.Name = "Lblμs";
            // 
            // BtnSelfCheck
            // 
            resources.ApplyResources(this.BtnSelfCheck, "BtnSelfCheck");
            this.BtnSelfCheck.Name = "BtnSelfCheck";
            this.BtnSelfCheck.UseVisualStyleBackColor = true;
            this.BtnSelfCheck.Click += new System.EventHandler(this.BtnSelfCheck_Click);
            // 
            // TxtPWidth
            // 
            resources.ApplyResources(this.TxtPWidth, "TxtPWidth");
            this.TxtPWidth.Name = "TxtPWidth";
            this.TxtPWidth.Enter += new System.EventHandler(this.TxtPWidth_Enter);
            // 
            // CmbControl
            // 
            this.CmbControl.DropDownWidth = 44;
            this.CmbControl.FormattingEnabled = true;
            this.CmbControl.Items.AddRange(new object[] {
            resources.GetString("CmbControl.Items"),
            resources.GetString("CmbControl.Items1")});
            resources.ApplyResources(this.CmbControl, "CmbControl");
            this.CmbControl.Name = "CmbControl";
            // 
            // LblPWidth
            // 
            resources.ApplyResources(this.LblPWidth, "LblPWidth");
            this.LblPWidth.Name = "LblPWidth";
            // 
            // LblControl
            // 
            resources.ApplyResources(this.LblControl, "LblControl");
            this.LblControl.Name = "LblControl";
            // 
            // CmbPulse
            // 
            this.CmbPulse.DropDownWidth = 44;
            this.CmbPulse.FormattingEnabled = true;
            this.CmbPulse.Items.AddRange(new object[] {
            resources.GetString("CmbPulse.Items"),
            resources.GetString("CmbPulse.Items1")});
            resources.ApplyResources(this.CmbPulse, "CmbPulse");
            this.CmbPulse.Name = "CmbPulse";
            // 
            // LblPulse
            // 
            resources.ApplyResources(this.LblPulse, "LblPulse");
            this.LblPulse.Name = "LblPulse";
            // 
            // TxtCheckCount
            // 
            resources.ApplyResources(this.TxtCheckCount, "TxtCheckCount");
            this.TxtCheckCount.Name = "TxtCheckCount";
            this.TxtCheckCount.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtCheckCount_MouseClick);
            // 
            // LblSelfcount
            // 
            resources.ApplyResources(this.LblSelfcount, "LblSelfcount");
            this.LblSelfcount.Name = "LblSelfcount";
            // 
            // LblTimeWork
            // 
            resources.ApplyResources(this.LblTimeWork, "LblTimeWork");
            this.LblTimeWork.ForeColor = System.Drawing.Color.Lime;
            this.LblTimeWork.Name = "LblTimeWork";
            // 
            // bkWorkerReceiveData
            // 
            this.bkWorkerReceiveData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BkWorkerReceiveData_DoWork);
            this.bkWorkerReceiveData.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BkWorkerReceiveData_ProgressChanged);
            // 
            // LHP
            // 
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 15F);
            this.LHP.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.LHP, "LHP");
            this.LHP.Name = "LHP";
            // 
            // LHB
            // 
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 15F);
            this.LHB.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.LHB, "LHB");
            this.LHB.Name = "LHB";
            // 
            // RHP
            // 
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 15F);
            this.RHP.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.RHP, "RHP");
            this.RHP.Name = "RHP";
            // 
            // RHB
            // 
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 15F);
            this.RHB.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.RHB, "RHB");
            this.RHB.Name = "RHB";
            // 
            // LF
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 15F);
            this.LF.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.LF, "LF");
            this.LF.Name = "LF";
            // 
            // RF
            // 
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 15F);
            this.RF.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.RF, "RF");
            this.RF.Name = "RF";
            // 
            // FrmTestHardware
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.LblTimeWork);
            this.Controls.Add(this.GrpSelfTestParameter);
            this.Controls.Add(this.GrpDetectorSelfTest);
            this.Controls.Add(this.GrpSensorstate);
            this.Controls.Add(this.GrpFrisker);
            this.Controls.Add(this.GrpWork);
            this.Name = "FrmTestHardware";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTestHardware_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmTestHardware_FormClosed);
            this.Load += new System.EventHandler(this.FrmTestHardware_Load);
            this.GrpWork.ResumeLayout(false);
            this.GrpWork.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvWork)).EndInit();
            this.GrpFrisker.ResumeLayout(false);
            this.GrpFrisker.PerformLayout();
            this.GrpSensorstate.ResumeLayout(false);
            this.GrpSensorstate.PerformLayout();
            this.GrpDetectorSelfTest.ResumeLayout(false);
            this.GrpSelfTestParameter.ResumeLayout(false);
            this.GrpSelfTestParameter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GrpWork;
        private System.Windows.Forms.Label LblβTotalcnt;
        private System.Windows.Forms.Label Lblβcountrate;
        private System.Windows.Forms.Label LblαTotalcnt;
        private System.Windows.Forms.Label Lblαcountrate;
        private System.Windows.Forms.Label LblHighVoltage;
        private System.Windows.Forms.Label LblLHP;
        private System.Windows.Forms.Label LblStatus;
        private System.Windows.Forms.Label LblRF;
        private System.Windows.Forms.Label LblLF;
        private System.Windows.Forms.Label LblRHB;
        private System.Windows.Forms.Label LblRHP;
        private System.Windows.Forms.Label LblLHB;
        private System.Windows.Forms.Label LblV;
        private System.Windows.Forms.Label LblBetacnt;
        private System.Windows.Forms.Label LblBetacps;
        private System.Windows.Forms.Label LblAlphacnt;
        private System.Windows.Forms.Label LblAlphacps;
        private System.Windows.Forms.GroupBox GrpFrisker;
        private System.Windows.Forms.GroupBox GrpSensorstate;
        private System.Windows.Forms.GroupBox GrpDetectorSelfTest;
        private System.Windows.Forms.GroupBox GrpSelfTestParameter;
        private System.Windows.Forms.TextBox TxtFriskercount;
        private System.Windows.Forms.Label LblFriskercount;
        private System.Windows.Forms.Label LblFriskercps;
        private System.Windows.Forms.TextBox TxtFriskerState;
        private System.Windows.Forms.Label LblFriskerState;
        private System.Windows.Forms.TextBox TxtRHandState;
        private System.Windows.Forms.Label LblRHandState;
        private System.Windows.Forms.TextBox TxtLHandState;
        private System.Windows.Forms.Label LblLHandState;
        public System.Windows.Forms.Button BtnBetaCheck;
        public System.Windows.Forms.Button BtnAlphaCheck;
        private System.Windows.Forms.Label LblSelfcount;
        private System.Windows.Forms.TextBox TxtCheckCount;
        private System.Windows.Forms.ComboBox CmbPulse;
        private System.Windows.Forms.Label LblPulse;
        public System.Windows.Forms.Button BtnSelfCheck;
        private System.Windows.Forms.TextBox TxtPWidth;
        private System.Windows.Forms.ComboBox CmbControl;
        private System.Windows.Forms.Label LblPWidth;
        private System.Windows.Forms.Label LblControl;
        private System.Windows.Forms.Label Lblμs;
        private System.Windows.Forms.Label LblTimeWork;
        private System.Windows.Forms.DataGridView DgvWork;
        private System.ComponentModel.BackgroundWorker bkWorkerReceiveData;
        private System.Windows.Forms.DataGridViewTextBoxColumn LHP;
        private System.Windows.Forms.DataGridViewTextBoxColumn LHB;
        private System.Windows.Forms.DataGridViewTextBoxColumn RHP;
        private System.Windows.Forms.DataGridViewTextBoxColumn RHB;
        private System.Windows.Forms.DataGridViewTextBoxColumn LF;
        private System.Windows.Forms.DataGridViewTextBoxColumn RF;
    }
}
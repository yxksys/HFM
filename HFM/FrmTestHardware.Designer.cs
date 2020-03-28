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
            this.GrpWork = new System.Windows.Forms.GroupBox();
            this.DgvWork = new System.Windows.Forms.DataGridView();
            this.LHP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LHB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RHP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RHB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RF = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.GrpWork.Location = new System.Drawing.Point(12, 12);
            this.GrpWork.Name = "GrpWork";
            this.GrpWork.Size = new System.Drawing.Size(728, 269);
            this.GrpWork.TabIndex = 33;
            this.GrpWork.TabStop = false;
            // 
            // DgvWork
            // 
            this.DgvWork.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvWork.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvWork.ColumnHeadersVisible = false;
            this.DgvWork.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LHP,
            this.LHB,
            this.RHP,
            this.RHB,
            this.LF,
            this.RF});
            this.DgvWork.Location = new System.Drawing.Point(130, 52);
            this.DgvWork.Name = "DgvWork";
            this.DgvWork.RowHeadersVisible = false;
            this.DgvWork.RowTemplate.Height = 35;
            this.DgvWork.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.DgvWork.Size = new System.Drawing.Size(491, 210);
            this.DgvWork.TabIndex = 17;
            // 
            // LHP
            // 
            this.LHP.HeaderText = "LHP";
            this.LHP.Name = "LHP";
            this.LHP.Width = 82;
            // 
            // LHB
            // 
            this.LHB.HeaderText = "LHB";
            this.LHB.Name = "LHB";
            this.LHB.Width = 82;
            // 
            // RHP
            // 
            this.RHP.HeaderText = "RHP";
            this.RHP.Name = "RHP";
            this.RHP.Width = 82;
            // 
            // RHB
            // 
            this.RHB.HeaderText = "RHB";
            this.RHB.Name = "RHB";
            this.RHB.Width = 82;
            // 
            // LF
            // 
            this.LF.HeaderText = "LF";
            this.LF.Name = "LF";
            this.LF.Width = 80;
            // 
            // RF
            // 
            this.RF.HeaderText = "RF";
            this.RF.Name = "RF";
            this.RF.Width = 80;
            // 
            // LblBetacnt
            // 
            this.LblBetacnt.AutoSize = true;
            this.LblBetacnt.Location = new System.Drawing.Point(627, 204);
            this.LblBetacnt.Name = "LblBetacnt";
            this.LblBetacnt.Size = new System.Drawing.Size(62, 20);
            this.LblBetacnt.TabIndex = 16;
            this.LblBetacnt.Text = "单位:cnt";
            // 
            // LblBetacps
            // 
            this.LblBetacps.AutoSize = true;
            this.LblBetacps.Location = new System.Drawing.Point(627, 163);
            this.LblBetacps.Name = "LblBetacps";
            this.LblBetacps.Size = new System.Drawing.Size(67, 20);
            this.LblBetacps.TabIndex = 15;
            this.LblBetacps.Text = "单位:cps";
            // 
            // LblAlphacnt
            // 
            this.LblAlphacnt.AutoSize = true;
            this.LblAlphacnt.Location = new System.Drawing.Point(627, 125);
            this.LblAlphacnt.Name = "LblAlphacnt";
            this.LblAlphacnt.Size = new System.Drawing.Size(62, 20);
            this.LblAlphacnt.TabIndex = 14;
            this.LblAlphacnt.Text = "单位:cnt";
            // 
            // LblAlphacps
            // 
            this.LblAlphacps.AutoSize = true;
            this.LblAlphacps.Location = new System.Drawing.Point(627, 88);
            this.LblAlphacps.Name = "LblAlphacps";
            this.LblAlphacps.Size = new System.Drawing.Size(67, 20);
            this.LblAlphacps.TabIndex = 13;
            this.LblAlphacps.Text = "单位:cps";
            // 
            // LblV
            // 
            this.LblV.AutoSize = true;
            this.LblV.Location = new System.Drawing.Point(627, 52);
            this.LblV.Name = "LblV";
            this.LblV.Size = new System.Drawing.Size(54, 20);
            this.LblV.TabIndex = 12;
            this.LblV.Text = "单位:V";
            // 
            // LblRF
            // 
            this.LblRF.Location = new System.Drawing.Point(556, 22);
            this.LblRF.Name = "LblRF";
            this.LblRF.Size = new System.Drawing.Size(41, 20);
            this.LblRF.TabIndex = 11;
            this.LblRF.Text = "右脚";
            // 
            // LblLF
            // 
            this.LblLF.Location = new System.Drawing.Point(487, 22);
            this.LblLF.Name = "LblLF";
            this.LblLF.Size = new System.Drawing.Size(41, 20);
            this.LblLF.TabIndex = 10;
            this.LblLF.Text = "左脚";
            // 
            // LblRHB
            // 
            this.LblRHB.Location = new System.Drawing.Point(393, 22);
            this.LblRHB.Name = "LblRHB";
            this.LblRHB.Size = new System.Drawing.Size(57, 20);
            this.LblRHB.TabIndex = 9;
            this.LblRHB.Text = "右手背";
            // 
            // LblRHP
            // 
            this.LblRHP.Location = new System.Drawing.Point(305, 22);
            this.LblRHP.Name = "LblRHP";
            this.LblRHP.Size = new System.Drawing.Size(57, 20);
            this.LblRHP.TabIndex = 8;
            this.LblRHP.Text = "右手心";
            // 
            // LblLHB
            // 
            this.LblLHB.Location = new System.Drawing.Point(220, 22);
            this.LblLHB.Name = "LblLHB";
            this.LblLHB.Size = new System.Drawing.Size(57, 20);
            this.LblLHB.TabIndex = 7;
            this.LblLHB.Text = "左手背";
            // 
            // LblLHP
            // 
            this.LblLHP.Location = new System.Drawing.Point(138, 22);
            this.LblLHP.Name = "LblLHP";
            this.LblLHP.Size = new System.Drawing.Size(57, 20);
            this.LblLHP.TabIndex = 6;
            this.LblLHP.Text = "左手心";
            // 
            // LblStatus
            // 
            this.LblStatus.Location = new System.Drawing.Point(14, 243);
            this.LblStatus.Name = "LblStatus";
            this.LblStatus.Size = new System.Drawing.Size(87, 19);
            this.LblStatus.TabIndex = 5;
            this.LblStatus.Text = "工作状态";
            this.LblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblβTotalcnt
            // 
            this.LblβTotalcnt.Location = new System.Drawing.Point(16, 204);
            this.LblβTotalcnt.Name = "LblβTotalcnt";
            this.LblβTotalcnt.Size = new System.Drawing.Size(87, 19);
            this.LblβTotalcnt.TabIndex = 4;
            this.LblβTotalcnt.Text = "β总计数";
            this.LblβTotalcnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Lblβcountrate
            // 
            this.Lblβcountrate.Location = new System.Drawing.Point(14, 164);
            this.Lblβcountrate.Name = "Lblβcountrate";
            this.Lblβcountrate.Size = new System.Drawing.Size(87, 19);
            this.Lblβcountrate.TabIndex = 3;
            this.Lblβcountrate.Text = "β计数率";
            this.Lblβcountrate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblαTotalcnt
            // 
            this.LblαTotalcnt.Location = new System.Drawing.Point(14, 126);
            this.LblαTotalcnt.Name = "LblαTotalcnt";
            this.LblαTotalcnt.Size = new System.Drawing.Size(87, 19);
            this.LblαTotalcnt.TabIndex = 2;
            this.LblαTotalcnt.Text = "α总计数";
            this.LblαTotalcnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Lblαcountrate
            // 
            this.Lblαcountrate.Location = new System.Drawing.Point(13, 88);
            this.Lblαcountrate.Name = "Lblαcountrate";
            this.Lblαcountrate.Size = new System.Drawing.Size(92, 18);
            this.Lblαcountrate.TabIndex = 1;
            this.Lblαcountrate.Text = "α计数率";
            this.Lblαcountrate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblHighVoltage
            // 
            this.LblHighVoltage.Location = new System.Drawing.Point(16, 52);
            this.LblHighVoltage.Name = "LblHighVoltage";
            this.LblHighVoltage.Size = new System.Drawing.Size(87, 19);
            this.LblHighVoltage.TabIndex = 0;
            this.LblHighVoltage.Text = "探头高压";
            this.LblHighVoltage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GrpFrisker
            // 
            this.GrpFrisker.Controls.Add(this.LblFriskercps);
            this.GrpFrisker.Controls.Add(this.TxtFriskercount);
            this.GrpFrisker.Controls.Add(this.LblFriskercount);
            this.GrpFrisker.Location = new System.Drawing.Point(12, 288);
            this.GrpFrisker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GrpFrisker.Name = "GrpFrisker";
            this.GrpFrisker.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GrpFrisker.Size = new System.Drawing.Size(231, 70);
            this.GrpFrisker.TabIndex = 34;
            this.GrpFrisker.TabStop = false;
            this.GrpFrisker.Text = "衣物探头";
            // 
            // LblFriskercps
            // 
            this.LblFriskercps.AutoSize = true;
            this.LblFriskercps.Location = new System.Drawing.Point(193, 30);
            this.LblFriskercps.Name = "LblFriskercps";
            this.LblFriskercps.Size = new System.Drawing.Size(31, 20);
            this.LblFriskercps.TabIndex = 17;
            this.LblFriskercps.Text = "cps";
            // 
            // TxtFriskercount
            // 
            this.TxtFriskercount.BackColor = System.Drawing.SystemColors.Control;
            this.TxtFriskercount.Location = new System.Drawing.Point(102, 28);
            this.TxtFriskercount.Name = "TxtFriskercount";
            this.TxtFriskercount.ReadOnly = true;
            this.TxtFriskercount.Size = new System.Drawing.Size(86, 26);
            this.TxtFriskercount.TabIndex = 18;
            this.TxtFriskercount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LblFriskercount
            // 
            this.LblFriskercount.Location = new System.Drawing.Point(28, 31);
            this.LblFriskercount.Name = "LblFriskercount";
            this.LblFriskercount.Size = new System.Drawing.Size(66, 19);
            this.LblFriskercount.TabIndex = 17;
            this.LblFriskercount.Text = "计数";
            this.LblFriskercount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GrpSensorstate
            // 
            this.GrpSensorstate.Controls.Add(this.TxtFriskerState);
            this.GrpSensorstate.Controls.Add(this.LblFriskerState);
            this.GrpSensorstate.Controls.Add(this.TxtRHandState);
            this.GrpSensorstate.Controls.Add(this.LblRHandState);
            this.GrpSensorstate.Controls.Add(this.TxtLHandState);
            this.GrpSensorstate.Controls.Add(this.LblLHandState);
            this.GrpSensorstate.Location = new System.Drawing.Point(249, 288);
            this.GrpSensorstate.Name = "GrpSensorstate";
            this.GrpSensorstate.Size = new System.Drawing.Size(491, 70);
            this.GrpSensorstate.TabIndex = 35;
            this.GrpSensorstate.TabStop = false;
            this.GrpSensorstate.Text = "红外状态";
            // 
            // TxtFriskerState
            // 
            this.TxtFriskerState.BackColor = System.Drawing.Color.Orange;
            this.TxtFriskerState.Location = new System.Drawing.Point(385, 31);
            this.TxtFriskerState.Name = "TxtFriskerState";
            this.TxtFriskerState.ReadOnly = true;
            this.TxtFriskerState.Size = new System.Drawing.Size(72, 26);
            this.TxtFriskerState.TabIndex = 23;
            // 
            // LblFriskerState
            // 
            this.LblFriskerState.AutoSize = true;
            this.LblFriskerState.Location = new System.Drawing.Point(330, 31);
            this.LblFriskerState.Name = "LblFriskerState";
            this.LblFriskerState.Size = new System.Drawing.Size(41, 20);
            this.LblFriskerState.TabIndex = 22;
            this.LblFriskerState.Text = "衣物";
            // 
            // TxtRHandState
            // 
            this.TxtRHandState.BackColor = System.Drawing.Color.Orange;
            this.TxtRHandState.Location = new System.Drawing.Point(224, 31);
            this.TxtRHandState.Name = "TxtRHandState";
            this.TxtRHandState.ReadOnly = true;
            this.TxtRHandState.Size = new System.Drawing.Size(72, 26);
            this.TxtRHandState.TabIndex = 21;
            // 
            // LblRHandState
            // 
            this.LblRHandState.AutoSize = true;
            this.LblRHandState.Location = new System.Drawing.Point(172, 31);
            this.LblRHandState.Name = "LblRHandState";
            this.LblRHandState.Size = new System.Drawing.Size(41, 20);
            this.LblRHandState.TabIndex = 20;
            this.LblRHandState.Text = "右手";
            // 
            // TxtLHandState
            // 
            this.TxtLHandState.BackColor = System.Drawing.Color.Orange;
            this.TxtLHandState.Location = new System.Drawing.Point(71, 31);
            this.TxtLHandState.Name = "TxtLHandState";
            this.TxtLHandState.ReadOnly = true;
            this.TxtLHandState.Size = new System.Drawing.Size(72, 26);
            this.TxtLHandState.TabIndex = 19;
            // 
            // LblLHandState
            // 
            this.LblLHandState.AutoSize = true;
            this.LblLHandState.Location = new System.Drawing.Point(21, 31);
            this.LblLHandState.Name = "LblLHandState";
            this.LblLHandState.Size = new System.Drawing.Size(41, 20);
            this.LblLHandState.TabIndex = 17;
            this.LblLHandState.Text = "左手";
            // 
            // GrpDetectorSelfTest
            // 
            this.GrpDetectorSelfTest.Controls.Add(this.BtnBetaCheck);
            this.GrpDetectorSelfTest.Controls.Add(this.BtnAlphaCheck);
            this.GrpDetectorSelfTest.Location = new System.Drawing.Point(12, 366);
            this.GrpDetectorSelfTest.Name = "GrpDetectorSelfTest";
            this.GrpDetectorSelfTest.Size = new System.Drawing.Size(195, 70);
            this.GrpDetectorSelfTest.TabIndex = 36;
            this.GrpDetectorSelfTest.TabStop = false;
            this.GrpDetectorSelfTest.Text = "探头自检";
            // 
            // BtnBetaCheck
            // 
            this.BtnBetaCheck.Location = new System.Drawing.Point(114, 19);
            this.BtnBetaCheck.Name = "BtnBetaCheck";
            this.BtnBetaCheck.Size = new System.Drawing.Size(75, 39);
            this.BtnBetaCheck.TabIndex = 1;
            this.BtnBetaCheck.Text = "β自检";
            this.BtnBetaCheck.UseVisualStyleBackColor = true;
            this.BtnBetaCheck.Click += new System.EventHandler(this.BtnBetaCheck_Click);
            // 
            // BtnAlphaCheck
            // 
            this.BtnAlphaCheck.Location = new System.Drawing.Point(20, 21);
            this.BtnAlphaCheck.Name = "BtnAlphaCheck";
            this.BtnAlphaCheck.Size = new System.Drawing.Size(75, 39);
            this.BtnAlphaCheck.TabIndex = 0;
            this.BtnAlphaCheck.Text = "α自检";
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
            this.GrpSelfTestParameter.Location = new System.Drawing.Point(213, 366);
            this.GrpSelfTestParameter.Name = "GrpSelfTestParameter";
            this.GrpSelfTestParameter.Size = new System.Drawing.Size(527, 70);
            this.GrpSelfTestParameter.TabIndex = 37;
            this.GrpSelfTestParameter.TabStop = false;
            this.GrpSelfTestParameter.Text = "自检参数";
            // 
            // Lblμs
            // 
            this.Lblμs.AutoSize = true;
            this.Lblμs.Location = new System.Drawing.Point(439, 29);
            this.Lblμs.Name = "Lblμs";
            this.Lblμs.Size = new System.Drawing.Size(23, 20);
            this.Lblμs.TabIndex = 27;
            this.Lblμs.Text = "μs";
            this.Lblμs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BtnSelfCheck
            // 
            this.BtnSelfCheck.Location = new System.Drawing.Point(468, 18);
            this.BtnSelfCheck.Name = "BtnSelfCheck";
            this.BtnSelfCheck.Size = new System.Drawing.Size(53, 39);
            this.BtnSelfCheck.TabIndex = 2;
            this.BtnSelfCheck.Text = "自检";
            this.BtnSelfCheck.UseVisualStyleBackColor = true;
            this.BtnSelfCheck.Click += new System.EventHandler(this.BtnSelfCheck_Click);
            // 
            // TxtPWidth
            // 
            this.TxtPWidth.Location = new System.Drawing.Point(405, 25);
            this.TxtPWidth.Name = "TxtPWidth";
            this.TxtPWidth.Size = new System.Drawing.Size(32, 26);
            this.TxtPWidth.TabIndex = 26;
            this.TxtPWidth.Text = "3";
            // 
            // CmbControl
            // 
            this.CmbControl.DropDownWidth = 44;
            this.CmbControl.FormattingEnabled = true;
            this.CmbControl.Items.AddRange(new object[] {
            "L(低)",
            "H(高)"});
            this.CmbControl.Location = new System.Drawing.Point(305, 23);
            this.CmbControl.Name = "CmbControl";
            this.CmbControl.Size = new System.Drawing.Size(44, 28);
            this.CmbControl.TabIndex = 25;
            this.CmbControl.Text = "低";
            // 
            // LblPWidth
            // 
            this.LblPWidth.AutoSize = true;
            this.LblPWidth.Location = new System.Drawing.Point(355, 30);
            this.LblPWidth.Name = "LblPWidth";
            this.LblPWidth.Size = new System.Drawing.Size(41, 20);
            this.LblPWidth.TabIndex = 24;
            this.LblPWidth.Text = "脉宽";
            this.LblPWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblControl
            // 
            this.LblControl.AutoSize = true;
            this.LblControl.Location = new System.Drawing.Point(246, 30);
            this.LblControl.Name = "LblControl";
            this.LblControl.Size = new System.Drawing.Size(41, 20);
            this.LblControl.TabIndex = 23;
            this.LblControl.Text = "控制";
            this.LblControl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CmbPulse
            // 
            this.CmbPulse.DropDownWidth = 44;
            this.CmbPulse.FormattingEnabled = true;
            this.CmbPulse.Items.AddRange(new object[] {
            "L(低)",
            "H(高)"});
            this.CmbPulse.Location = new System.Drawing.Point(196, 24);
            this.CmbPulse.Name = "CmbPulse";
            this.CmbPulse.Size = new System.Drawing.Size(44, 28);
            this.CmbPulse.TabIndex = 22;
            this.CmbPulse.Text = "低";
            // 
            // LblPulse
            // 
            this.LblPulse.AutoSize = true;
            this.LblPulse.Location = new System.Drawing.Point(145, 31);
            this.LblPulse.Name = "LblPulse";
            this.LblPulse.Size = new System.Drawing.Size(41, 20);
            this.LblPulse.TabIndex = 21;
            this.LblPulse.Text = "脉冲";
            this.LblPulse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtCheckCount
            // 
            this.TxtCheckCount.Location = new System.Drawing.Point(93, 25);
            this.TxtCheckCount.Name = "TxtCheckCount";
            this.TxtCheckCount.Size = new System.Drawing.Size(46, 26);
            this.TxtCheckCount.TabIndex = 20;
            this.TxtCheckCount.Text = "1000";
            // 
            // LblSelfcount
            // 
            this.LblSelfcount.Location = new System.Drawing.Point(13, 30);
            this.LblSelfcount.Name = "LblSelfcount";
            this.LblSelfcount.Size = new System.Drawing.Size(74, 20);
            this.LblSelfcount.TabIndex = 19;
            this.LblSelfcount.Text = "计数";
            this.LblSelfcount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblTimeWork
            // 
            this.LblTimeWork.AutoSize = true;
            this.LblTimeWork.Font = new System.Drawing.Font("Arial Narrow", 42F, System.Drawing.FontStyle.Bold);
            this.LblTimeWork.ForeColor = System.Drawing.Color.Lime;
            this.LblTimeWork.Location = new System.Drawing.Point(158, 440);
            this.LblTimeWork.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblTimeWork.Name = "LblTimeWork";
            this.LblTimeWork.Size = new System.Drawing.Size(370, 66);
            this.LblTimeWork.TabIndex = 38;
            this.LblTimeWork.Text = "测量剩余时间";
            // 
            // bkWorkerReceiveData
            // 
            this.bkWorkerReceiveData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BkWorkerReceiveData_DoWork);
            this.bkWorkerReceiveData.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BkWorkerReceiveData_ProgressChanged);
            // 
            // FrmTestHardware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 516);
            this.ControlBox = false;
            this.Controls.Add(this.LblTimeWork);
            this.Controls.Add(this.GrpSelfTestParameter);
            this.Controls.Add(this.GrpDetectorSelfTest);
            this.Controls.Add(this.GrpSensorstate);
            this.Controls.Add(this.GrpFrisker);
            this.Controls.Add(this.GrpWork);
            this.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrmTestHardware";
            this.Text = "硬件检测";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn LHP;
        private System.Windows.Forms.DataGridViewTextBoxColumn LHB;
        private System.Windows.Forms.DataGridViewTextBoxColumn RHP;
        private System.Windows.Forms.DataGridViewTextBoxColumn RHB;
        private System.Windows.Forms.DataGridViewTextBoxColumn LF;
        private System.Windows.Forms.DataGridViewTextBoxColumn RF;
        private System.ComponentModel.BackgroundWorker bkWorkerReceiveData;
    }
}
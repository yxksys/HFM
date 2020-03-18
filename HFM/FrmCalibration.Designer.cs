namespace HFM
{
    partial class FrmCalibration
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
            this.TabCalibration = new System.Windows.Forms.TabControl();
            this.TabpageCalibration = new System.Windows.Forms.TabPage();
            this.GrpCalibration = new System.Windows.Forms.GroupBox();
            this.Lbl__time = new System.Windows.Forms.Label();
            this.DgvInformation = new System.Windows.Forms.DataGridView();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Channel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alpha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Beta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HV = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TxtSFR = new System.Windows.Forms.TextBox();
            this.LblSFR = new System.Windows.Forms.Label();
            this.BtnCalibrate = new System.Windows.Forms.Button();
            this.BtnSet = new System.Windows.Forms.Button();
            this.CmbNuclideSelect = new System.Windows.Forms.ComboBox();
            this.Txtβ = new System.Windows.Forms.TextBox();
            this.Txtα = new System.Windows.Forms.TextBox();
            this.Lblβ = new System.Windows.Forms.Label();
            this.Lblα = new System.Windows.Forms.Label();
            this.LblNuclide = new System.Windows.Forms.Label();
            this.LblThreshold = new System.Windows.Forms.Label();
            this.TxtCount = new System.Windows.Forms.TextBox();
            this.TxtHV = new System.Windows.Forms.TextBox();
            this.LblCount = new System.Windows.Forms.Label();
            this.LblHV = new System.Windows.Forms.Label();
            this.TxtResult = new System.Windows.Forms.TextBox();
            this.TxtMeasuringTime = new System.Windows.Forms.TextBox();
            this.CmbChannelSelection = new System.Windows.Forms.ComboBox();
            this.LblResult = new System.Windows.Forms.Label();
            this.LblMeasuringTime = new System.Windows.Forms.Label();
            this.LblChannelSelection = new System.Windows.Forms.Label();
            this.bkWorkerReceiveData = new System.ComponentModel.BackgroundWorker();
            this.TabCalibration.SuspendLayout();
            this.TabpageCalibration.SuspendLayout();
            this.GrpCalibration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvInformation)).BeginInit();
            this.SuspendLayout();
            // 
            // TabCalibration
            // 
            this.TabCalibration.Controls.Add(this.TabpageCalibration);
            this.TabCalibration.Dock = System.Windows.Forms.DockStyle.Top;
            this.TabCalibration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.TabCalibration.ItemSize = new System.Drawing.Size(76, 25);
            this.TabCalibration.Location = new System.Drawing.Point(0, 0);
            this.TabCalibration.Name = "TabCalibration";
            this.TabCalibration.SelectedIndex = 0;
            this.TabCalibration.Size = new System.Drawing.Size(818, 900);
            this.TabCalibration.TabIndex = 0;
            // 
            // TabpageCalibration
            // 
            this.TabpageCalibration.Controls.Add(this.GrpCalibration);
            this.TabpageCalibration.Location = new System.Drawing.Point(4, 29);
            this.TabpageCalibration.Name = "TabpageCalibration";
            this.TabpageCalibration.Padding = new System.Windows.Forms.Padding(3);
            this.TabpageCalibration.Size = new System.Drawing.Size(810, 867);
            this.TabpageCalibration.TabIndex = 0;
            this.TabpageCalibration.Text = "仪器刻度";
            // 
            // GrpCalibration
            // 
            this.GrpCalibration.Controls.Add(this.Lbl__time);
            this.GrpCalibration.Controls.Add(this.DgvInformation);
            this.GrpCalibration.Controls.Add(this.TxtSFR);
            this.GrpCalibration.Controls.Add(this.LblSFR);
            this.GrpCalibration.Controls.Add(this.BtnCalibrate);
            this.GrpCalibration.Controls.Add(this.BtnSet);
            this.GrpCalibration.Controls.Add(this.CmbNuclideSelect);
            this.GrpCalibration.Controls.Add(this.Txtβ);
            this.GrpCalibration.Controls.Add(this.Txtα);
            this.GrpCalibration.Controls.Add(this.Lblβ);
            this.GrpCalibration.Controls.Add(this.Lblα);
            this.GrpCalibration.Controls.Add(this.LblNuclide);
            this.GrpCalibration.Controls.Add(this.LblThreshold);
            this.GrpCalibration.Controls.Add(this.TxtCount);
            this.GrpCalibration.Controls.Add(this.TxtHV);
            this.GrpCalibration.Controls.Add(this.LblCount);
            this.GrpCalibration.Controls.Add(this.LblHV);
            this.GrpCalibration.Controls.Add(this.TxtResult);
            this.GrpCalibration.Controls.Add(this.TxtMeasuringTime);
            this.GrpCalibration.Controls.Add(this.CmbChannelSelection);
            this.GrpCalibration.Controls.Add(this.LblResult);
            this.GrpCalibration.Controls.Add(this.LblMeasuringTime);
            this.GrpCalibration.Controls.Add(this.LblChannelSelection);
            this.GrpCalibration.Location = new System.Drawing.Point(6, 0);
            this.GrpCalibration.Name = "GrpCalibration";
            this.GrpCalibration.Size = new System.Drawing.Size(760, 859);
            this.GrpCalibration.TabIndex = 0;
            this.GrpCalibration.TabStop = false;
            // 
            // Lbl__time
            // 
            this.Lbl__time.AutoSize = true;
            this.Lbl__time.Location = new System.Drawing.Point(37, 143);
            this.Lbl__time.Name = "Lbl__time";
            this.Lbl__time.Size = new System.Drawing.Size(39, 20);
            this.Lbl__time.TabIndex = 22;
            this.Lbl__time.Text = "time";
            // 
            // DgvInformation
            // 
            this.DgvInformation.AllowUserToDeleteRows = false;
            this.DgvInformation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvInformation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Status,
            this.Channel,
            this.Area,
            this.Alpha,
            this.Beta,
            this.HV});
            this.DgvInformation.Location = new System.Drawing.Point(27, 166);
            this.DgvInformation.Name = "DgvInformation";
            this.DgvInformation.RowHeadersVisible = false;
            this.DgvInformation.RowTemplate.Height = 23;
            this.DgvInformation.Size = new System.Drawing.Size(690, 673);
            this.DgvInformation.TabIndex = 21;
            // 
            // Status
            // 
            this.Status.HeaderText = "状态";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.Width = 150;
            // 
            // Channel
            // 
            this.Channel.HeaderText = "通道";
            this.Channel.Name = "Channel";
            this.Channel.ReadOnly = true;
            this.Channel.Width = 130;
            // 
            // Area
            // 
            this.Area.HeaderText = "面积(cm2)";
            this.Area.Name = "Area";
            this.Area.ReadOnly = true;
            // 
            // Alpha
            // 
            this.Alpha.HeaderText = "α计数";
            this.Alpha.Name = "Alpha";
            this.Alpha.ReadOnly = true;
            // 
            // Beta
            // 
            this.Beta.HeaderText = "β计数";
            this.Beta.Name = "Beta";
            this.Beta.ReadOnly = true;
            // 
            // HV
            // 
            this.HV.HeaderText = "高压（V）";
            this.HV.Name = "HV";
            this.HV.ReadOnly = true;
            this.HV.Width = 110;
            // 
            // TxtSFR
            // 
            this.TxtSFR.Location = new System.Drawing.Point(622, 66);
            this.TxtSFR.Name = "TxtSFR";
            this.TxtSFR.Size = new System.Drawing.Size(96, 26);
            this.TxtSFR.TabIndex = 20;
            this.TxtSFR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtSFR.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtSFR_KeyPress);
            // 
            // LblSFR
            // 
            this.LblSFR.Location = new System.Drawing.Point(507, 71);
            this.LblSFR.Name = "LblSFR";
            this.LblSFR.Size = new System.Drawing.Size(116, 18);
            this.LblSFR.TabIndex = 19;
            this.LblSFR.Text = "表面发射率";
            this.LblSFR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnCalibrate
            // 
            this.BtnCalibrate.Location = new System.Drawing.Point(643, 25);
            this.BtnCalibrate.Name = "BtnCalibrate";
            this.BtnCalibrate.Size = new System.Drawing.Size(75, 26);
            this.BtnCalibrate.TabIndex = 18;
            this.BtnCalibrate.Text = "刻度";
            this.BtnCalibrate.UseVisualStyleBackColor = true;
            this.BtnCalibrate.Click += new System.EventHandler(this.BtnCalibrate_Click);
            // 
            // BtnSet
            // 
            this.BtnSet.Location = new System.Drawing.Point(556, 25);
            this.BtnSet.Name = "BtnSet";
            this.BtnSet.Size = new System.Drawing.Size(75, 26);
            this.BtnSet.TabIndex = 17;
            this.BtnSet.Text = "设置";
            this.BtnSet.UseVisualStyleBackColor = true;
            this.BtnSet.Click += new System.EventHandler(this.BtnSet_Click);
            // 
            // CmbNuclideSelect
            // 
            this.CmbNuclideSelect.DropDownWidth = 74;
            this.CmbNuclideSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.CmbNuclideSelect.FormattingEnabled = true;
            this.CmbNuclideSelect.Location = new System.Drawing.Point(408, 67);
            this.CmbNuclideSelect.Name = "CmbNuclideSelect";
            this.CmbNuclideSelect.Size = new System.Drawing.Size(93, 28);
            this.CmbNuclideSelect.TabIndex = 16;
            this.CmbNuclideSelect.DropDown += new System.EventHandler(this.CmbNuclideSelect_DropDown);
            // 
            // Txtβ
            // 
            this.Txtβ.Location = new System.Drawing.Point(508, 25);
            this.Txtβ.Name = "Txtβ";
            this.Txtβ.Size = new System.Drawing.Size(42, 26);
            this.Txtβ.TabIndex = 15;
            this.Txtβ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Txtβ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txtβ_KeyPress);
            // 
            // Txtα
            // 
            this.Txtα.Location = new System.Drawing.Point(435, 26);
            this.Txtα.Name = "Txtα";
            this.Txtα.Size = new System.Drawing.Size(42, 26);
            this.Txtα.TabIndex = 14;
            this.Txtα.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Txtα.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Txtα_KeyPress);
            // 
            // Lblβ
            // 
            this.Lblβ.Location = new System.Drawing.Point(486, 28);
            this.Lblβ.Name = "Lblβ";
            this.Lblβ.Size = new System.Drawing.Size(17, 20);
            this.Lblβ.TabIndex = 13;
            this.Lblβ.Text = "β";
            this.Lblβ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Lblα
            // 
            this.Lblα.Location = new System.Drawing.Point(417, 30);
            this.Lblα.Name = "Lblα";
            this.Lblα.Size = new System.Drawing.Size(17, 20);
            this.Lblα.TabIndex = 12;
            this.Lblα.Text = "α";
            this.Lblα.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblNuclide
            // 
            this.LblNuclide.Location = new System.Drawing.Point(310, 69);
            this.LblNuclide.Name = "LblNuclide";
            this.LblNuclide.Size = new System.Drawing.Size(102, 18);
            this.LblNuclide.TabIndex = 11;
            this.LblNuclide.Text = "核素";
            this.LblNuclide.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblThreshold
            // 
            this.LblThreshold.Location = new System.Drawing.Point(310, 30);
            this.LblThreshold.Name = "LblThreshold";
            this.LblThreshold.Size = new System.Drawing.Size(102, 18);
            this.LblThreshold.TabIndex = 10;
            this.LblThreshold.Text = "阈值(mV)";
            this.LblThreshold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxtCount
            // 
            this.TxtCount.Location = new System.Drawing.Point(256, 69);
            this.TxtCount.Name = "TxtCount";
            this.TxtCount.Size = new System.Drawing.Size(54, 26);
            this.TxtCount.TabIndex = 9;
            this.TxtCount.Text = "1";
            this.TxtCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtCount_KeyPress);
            // 
            // TxtHV
            // 
            this.TxtHV.Location = new System.Drawing.Point(256, 26);
            this.TxtHV.Name = "TxtHV";
            this.TxtHV.Size = new System.Drawing.Size(54, 26);
            this.TxtHV.TabIndex = 8;
            this.TxtHV.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtHV.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtHV_KeyPress);
            // 
            // LblCount
            // 
            this.LblCount.Location = new System.Drawing.Point(193, 71);
            this.LblCount.Name = "LblCount";
            this.LblCount.Size = new System.Drawing.Size(60, 18);
            this.LblCount.TabIndex = 7;
            this.LblCount.Text = "次    数 ";
            this.LblCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LblHV
            // 
            this.LblHV.Location = new System.Drawing.Point(193, 29);
            this.LblHV.Name = "LblHV";
            this.LblHV.Size = new System.Drawing.Size(60, 18);
            this.LblHV.TabIndex = 6;
            this.LblHV.Text = "高压(V)";
            this.LblHV.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtResult
            // 
            this.TxtResult.Location = new System.Drawing.Point(118, 108);
            this.TxtResult.Name = "TxtResult";
            this.TxtResult.Size = new System.Drawing.Size(600, 26);
            this.TxtResult.TabIndex = 5;
            // 
            // TxtMeasuringTime
            // 
            this.TxtMeasuringTime.Location = new System.Drawing.Point(119, 67);
            this.TxtMeasuringTime.Name = "TxtMeasuringTime";
            this.TxtMeasuringTime.Size = new System.Drawing.Size(71, 26);
            this.TxtMeasuringTime.TabIndex = 4;
            this.TxtMeasuringTime.Text = "5";
            this.TxtMeasuringTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TxtMeasuringTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtMeasuringTime_KeyPress);
            // 
            // CmbChannelSelection
            // 
            this.CmbChannelSelection.DropDownWidth = 74;
            this.CmbChannelSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.CmbChannelSelection.FormattingEnabled = true;
            this.CmbChannelSelection.Location = new System.Drawing.Point(118, 25);
            this.CmbChannelSelection.Name = "CmbChannelSelection";
            this.CmbChannelSelection.Size = new System.Drawing.Size(74, 23);
            this.CmbChannelSelection.TabIndex = 3;
            this.CmbChannelSelection.SelectedValueChanged += new System.EventHandler(this.CmbChannelSelection_SelectedValueChanged);
            // 
            // LblResult
            // 
            this.LblResult.Location = new System.Drawing.Point(2, 108);
            this.LblResult.Name = "LblResult";
            this.LblResult.Size = new System.Drawing.Size(116, 18);
            this.LblResult.TabIndex = 2;
            this.LblResult.Text = "测量结果";
            this.LblResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblMeasuringTime
            // 
            this.LblMeasuringTime.Location = new System.Drawing.Point(2, 69);
            this.LblMeasuringTime.Name = "LblMeasuringTime";
            this.LblMeasuringTime.Size = new System.Drawing.Size(116, 18);
            this.LblMeasuringTime.TabIndex = 1;
            this.LblMeasuringTime.Text = "测量时间(s)";
            this.LblMeasuringTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblChannelSelection
            // 
            this.LblChannelSelection.Location = new System.Drawing.Point(0, 28);
            this.LblChannelSelection.Name = "LblChannelSelection";
            this.LblChannelSelection.Size = new System.Drawing.Size(116, 18);
            this.LblChannelSelection.TabIndex = 0;
            this.LblChannelSelection.Text = "通道选择";
            this.LblChannelSelection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bkWorkerReceiveData
            // 
            this.bkWorkerReceiveData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BkWorkerReceiveData_DoWork);
            this.bkWorkerReceiveData.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BkWorkerReceiveData_ProgressChanged);
            // 
            // FrmCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(835, 722);
            this.ControlBox = false;
            this.Controls.Add(this.TabCalibration);
            this.Name = "FrmCalibration";
            this.Text = "测试刻度";
            this.Load += new System.EventHandler(this.FrmCalibration_Load);
            this.TabCalibration.ResumeLayout(false);
            this.TabpageCalibration.ResumeLayout(false);
            this.GrpCalibration.ResumeLayout(false);
            this.GrpCalibration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvInformation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl TabCalibration;
        public System.Windows.Forms.TabPage TabpageCalibration;
        private System.Windows.Forms.GroupBox GrpCalibration;
        private System.Windows.Forms.Label LblChannelSelection;
        private System.Windows.Forms.Label LblMeasuringTime;
        private System.Windows.Forms.Label LblResult;
        public System.Windows.Forms.TextBox TxtMeasuringTime;
        private System.Windows.Forms.ComboBox CmbChannelSelection;
        private System.Windows.Forms.TextBox TxtResult;
        private System.Windows.Forms.Label LblHV;
        private System.Windows.Forms.Label LblThreshold;
        public System.Windows.Forms.TextBox TxtCount;
        public System.Windows.Forms.TextBox TxtHV;
        private System.Windows.Forms.Label LblCount;
        private System.Windows.Forms.DataGridView DgvInformation;
        public System.Windows.Forms.TextBox TxtSFR;
        private System.Windows.Forms.Label LblSFR;
        private System.Windows.Forms.Button BtnCalibrate;
        private System.Windows.Forms.Button BtnSet;
        private System.Windows.Forms.ComboBox CmbNuclideSelect;
        public System.Windows.Forms.TextBox Txtβ;
        public System.Windows.Forms.TextBox Txtα;
        private System.Windows.Forms.Label Lblβ;
        private System.Windows.Forms.Label Lblα;
        private System.Windows.Forms.Label LblNuclide;
        private System.ComponentModel.BackgroundWorker bkWorkerReceiveData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Channel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Area;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alpha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Beta;
        private System.Windows.Forms.DataGridViewTextBoxColumn HV;
        private System.Windows.Forms.Label Lbl__time;
    }
}
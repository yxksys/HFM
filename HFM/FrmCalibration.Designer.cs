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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCalibration));
            this.TabCalibration = new System.Windows.Forms.TabControl();
            this.TabpageCalibration = new System.Windows.Forms.TabPage();
            this.GrpCalibration = new System.Windows.Forms.GroupBox();
            this.PrgCalibrate = new System.Windows.Forms.ProgressBar();
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
            resources.ApplyResources(this.TabCalibration, "TabCalibration");
            this.TabCalibration.Name = "TabCalibration";
            this.TabCalibration.SelectedIndex = 0;
            // 
            // TabpageCalibration
            // 
            this.TabpageCalibration.Controls.Add(this.GrpCalibration);
            resources.ApplyResources(this.TabpageCalibration, "TabpageCalibration");
            this.TabpageCalibration.Name = "TabpageCalibration";
            // 
            // GrpCalibration
            // 
            resources.ApplyResources(this.GrpCalibration, "GrpCalibration");
            this.GrpCalibration.Controls.Add(this.PrgCalibrate);
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
            this.GrpCalibration.Name = "GrpCalibration";
            this.GrpCalibration.TabStop = false;
            // 
            // PrgCalibrate
            // 
            resources.ApplyResources(this.PrgCalibrate, "PrgCalibrate");
            this.PrgCalibrate.Name = "PrgCalibrate";
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
            resources.ApplyResources(this.DgvInformation, "DgvInformation");
            this.DgvInformation.Name = "DgvInformation";
            this.DgvInformation.RowHeadersVisible = false;
            this.DgvInformation.RowTemplate.Height = 23;
            // 
            // Status
            // 
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // Channel
            // 
            resources.ApplyResources(this.Channel, "Channel");
            this.Channel.Name = "Channel";
            this.Channel.ReadOnly = true;
            // 
            // Area
            // 
            resources.ApplyResources(this.Area, "Area");
            this.Area.Name = "Area";
            this.Area.ReadOnly = true;
            // 
            // Alpha
            // 
            resources.ApplyResources(this.Alpha, "Alpha");
            this.Alpha.Name = "Alpha";
            this.Alpha.ReadOnly = true;
            // 
            // Beta
            // 
            resources.ApplyResources(this.Beta, "Beta");
            this.Beta.Name = "Beta";
            this.Beta.ReadOnly = true;
            // 
            // HV
            // 
            resources.ApplyResources(this.HV, "HV");
            this.HV.Name = "HV";
            this.HV.ReadOnly = true;
            // 
            // TxtSFR
            // 
            resources.ApplyResources(this.TxtSFR, "TxtSFR");
            this.TxtSFR.Name = "TxtSFR";
            this.TxtSFR.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtSFR_MouseClick);
            // 
            // LblSFR
            // 
            resources.ApplyResources(this.LblSFR, "LblSFR");
            this.LblSFR.Name = "LblSFR";
            // 
            // BtnCalibrate
            // 
            resources.ApplyResources(this.BtnCalibrate, "BtnCalibrate");
            this.BtnCalibrate.Name = "BtnCalibrate";
            this.BtnCalibrate.UseVisualStyleBackColor = true;
            this.BtnCalibrate.Click += new System.EventHandler(this.BtnCalibrate_Click);
            // 
            // BtnSet
            // 
            resources.ApplyResources(this.BtnSet, "BtnSet");
            this.BtnSet.Name = "BtnSet";
            this.BtnSet.UseVisualStyleBackColor = true;
            this.BtnSet.Click += new System.EventHandler(this.BtnSet_Click);
            // 
            // CmbNuclideSelect
            // 
            this.CmbNuclideSelect.DropDownWidth = 74;
            resources.ApplyResources(this.CmbNuclideSelect, "CmbNuclideSelect");
            this.CmbNuclideSelect.FormattingEnabled = true;
            this.CmbNuclideSelect.Name = "CmbNuclideSelect";
            this.CmbNuclideSelect.DropDown += new System.EventHandler(this.CmbNuclideSelect_DropDown);
            // 
            // Txtβ
            // 
            resources.ApplyResources(this.Txtβ, "Txtβ");
            this.Txtβ.Name = "Txtβ";
            this.Txtβ.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Txtβ_MouseClick);
            // 
            // Txtα
            // 
            resources.ApplyResources(this.Txtα, "Txtα");
            this.Txtα.Name = "Txtα";
            this.Txtα.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Txtα_MouseClick);
            // 
            // Lblβ
            // 
            resources.ApplyResources(this.Lblβ, "Lblβ");
            this.Lblβ.Name = "Lblβ";
            // 
            // Lblα
            // 
            resources.ApplyResources(this.Lblα, "Lblα");
            this.Lblα.Name = "Lblα";
            // 
            // LblNuclide
            // 
            resources.ApplyResources(this.LblNuclide, "LblNuclide");
            this.LblNuclide.Name = "LblNuclide";
            // 
            // LblThreshold
            // 
            resources.ApplyResources(this.LblThreshold, "LblThreshold");
            this.LblThreshold.Name = "LblThreshold";
            // 
            // TxtCount
            // 
            resources.ApplyResources(this.TxtCount, "TxtCount");
            this.TxtCount.Name = "TxtCount";
            this.TxtCount.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtCount_MouseClick);
            // 
            // TxtHV
            // 
            resources.ApplyResources(this.TxtHV, "TxtHV");
            this.TxtHV.Name = "TxtHV";
            this.TxtHV.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtHV_MouseClick);
            // 
            // LblCount
            // 
            resources.ApplyResources(this.LblCount, "LblCount");
            this.LblCount.Name = "LblCount";
            // 
            // LblHV
            // 
            resources.ApplyResources(this.LblHV, "LblHV");
            this.LblHV.Name = "LblHV";
            // 
            // TxtResult
            // 
            resources.ApplyResources(this.TxtResult, "TxtResult");
            this.TxtResult.Name = "TxtResult";
            this.TxtResult.ReadOnly = true;
            // 
            // TxtMeasuringTime
            // 
            resources.ApplyResources(this.TxtMeasuringTime, "TxtMeasuringTime");
            this.TxtMeasuringTime.Name = "TxtMeasuringTime";
            this.TxtMeasuringTime.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtMeasuringTime_MouseClick);
            // 
            // CmbChannelSelection
            // 
            this.CmbChannelSelection.DropDownWidth = 74;
            resources.ApplyResources(this.CmbChannelSelection, "CmbChannelSelection");
            this.CmbChannelSelection.FormattingEnabled = true;
            this.CmbChannelSelection.Name = "CmbChannelSelection";
            this.CmbChannelSelection.SelectedValueChanged += new System.EventHandler(this.CmbChannelSelection_SelectedValueChanged);
            // 
            // LblResult
            // 
            resources.ApplyResources(this.LblResult, "LblResult");
            this.LblResult.Name = "LblResult";
            // 
            // LblMeasuringTime
            // 
            resources.ApplyResources(this.LblMeasuringTime, "LblMeasuringTime");
            this.LblMeasuringTime.Name = "LblMeasuringTime";
            // 
            // LblChannelSelection
            // 
            resources.ApplyResources(this.LblChannelSelection, "LblChannelSelection");
            this.LblChannelSelection.Name = "LblChannelSelection";
            // 
            // bkWorkerReceiveData
            // 
            this.bkWorkerReceiveData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BkWorkerReceiveData_DoWork);
            this.bkWorkerReceiveData.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BkWorkerReceiveData_ProgressChanged);
            // 
            // FrmCalibration
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.TabCalibration);
            this.Name = "FrmCalibration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCalibration_FormClosed);
            this.Load += new System.EventHandler(this.FrmCalibration_Load);
            this.TabCalibration.ResumeLayout(false);
            this.TabpageCalibration.ResumeLayout(false);
            this.TabpageCalibration.PerformLayout();
            this.GrpCalibration.ResumeLayout(false);
            this.GrpCalibration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvInformation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl TabCalibration;
        private System.ComponentModel.BackgroundWorker bkWorkerReceiveData;
        public System.Windows.Forms.TabPage TabpageCalibration;
        private System.Windows.Forms.GroupBox GrpCalibration;
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
        private System.Windows.Forms.Label LblThreshold;
        public System.Windows.Forms.TextBox TxtCount;
        public System.Windows.Forms.TextBox TxtHV;
        private System.Windows.Forms.Label LblCount;
        private System.Windows.Forms.Label LblHV;
        private System.Windows.Forms.TextBox TxtResult;
        public System.Windows.Forms.TextBox TxtMeasuringTime;
        private System.Windows.Forms.ComboBox CmbChannelSelection;
        private System.Windows.Forms.Label LblResult;
        private System.Windows.Forms.Label LblMeasuringTime;
        private System.Windows.Forms.Label LblChannelSelection;
        private System.Windows.Forms.ProgressBar PrgCalibrate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Channel;
        private System.Windows.Forms.DataGridViewTextBoxColumn Area;
        private System.Windows.Forms.DataGridViewTextBoxColumn Alpha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Beta;
        private System.Windows.Forms.DataGridViewTextBoxColumn HV;
    }
}
namespace HFM
{
    partial class FrmHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHistory));
            this.TabHistory = new System.Windows.Forms.TabControl();
            this.TabMeasure = new System.Windows.Forms.TabPage();
            this.BtnDeleteMeasure = new System.Windows.Forms.Button();
            this.BtnDeriveMeasure = new System.Windows.Forms.Button();
            this.DgvMeasure = new System.Windows.Forms.DataGridView();
            this.MeasureDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MeasureStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DetailedInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsEnglish = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TabCalibration = new System.Windows.Forms.TabPage();
            this.BtnDeleteCalibration = new System.Windows.Forms.Button();
            this.BtnDeriveCalibration = new System.Windows.Forms.Button();
            this.DgvCalibration = new System.Windows.Forms.DataGridView();
            this.CalibrationTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChannelID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HighVoltage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Threshold = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Efficiency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MDA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlphaBetaPercent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TabError = new System.Windows.Forms.TabPage();
            this.BtnDeleteError = new System.Windows.Forms.Button();
            this.BtnDeriveError = new System.Windows.Forms.Button();
            this.DgvError = new System.Windows.Forms.DataGridView();
            this.ErrTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Record = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsEnglishsError = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TabHistory.SuspendLayout();
            this.TabMeasure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvMeasure)).BeginInit();
            this.TabCalibration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvCalibration)).BeginInit();
            this.TabError.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvError)).BeginInit();
            this.SuspendLayout();
            // 
            // TabHistory
            // 
            resources.ApplyResources(this.TabHistory, "TabHistory");
            this.TabHistory.Controls.Add(this.TabMeasure);
            this.TabHistory.Controls.Add(this.TabCalibration);
            this.TabHistory.Controls.Add(this.TabError);
            this.TabHistory.Name = "TabHistory";
            this.TabHistory.SelectedIndex = 0;
            this.TabHistory.SelectedIndexChanged += new System.EventHandler(this.TabHistory_SelectedIndexChanged);
            // 
            // TabMeasure
            // 
            resources.ApplyResources(this.TabMeasure, "TabMeasure");
            this.TabMeasure.BackColor = System.Drawing.SystemColors.Control;
            this.TabMeasure.Controls.Add(this.BtnDeleteMeasure);
            this.TabMeasure.Controls.Add(this.BtnDeriveMeasure);
            this.TabMeasure.Controls.Add(this.DgvMeasure);
            this.TabMeasure.Name = "TabMeasure";
            // 
            // BtnDeleteMeasure
            // 
            resources.ApplyResources(this.BtnDeleteMeasure, "BtnDeleteMeasure");
            this.BtnDeleteMeasure.Name = "BtnDeleteMeasure";
            this.BtnDeleteMeasure.UseVisualStyleBackColor = true;
            this.BtnDeleteMeasure.Click += new System.EventHandler(this.BtnDeleteMeasure_Click);
            // 
            // BtnDeriveMeasure
            // 
            resources.ApplyResources(this.BtnDeriveMeasure, "BtnDeriveMeasure");
            this.BtnDeriveMeasure.Name = "BtnDeriveMeasure";
            this.BtnDeriveMeasure.UseVisualStyleBackColor = true;
            this.BtnDeriveMeasure.Click += new System.EventHandler(this.BtnDeriveMeasure_Click);
            // 
            // DgvMeasure
            // 
            resources.ApplyResources(this.DgvMeasure, "DgvMeasure");
            this.DgvMeasure.AllowUserToAddRows = false;
            this.DgvMeasure.AllowUserToDeleteRows = false;
            this.DgvMeasure.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvMeasure.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvMeasure.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MeasureDate,
            this.MeasureStatus,
            this.DetailedInfo,
            this.IsEnglish});
            this.DgvMeasure.Name = "DgvMeasure";
            this.DgvMeasure.RowTemplate.Height = 23;
            // 
            // MeasureDate
            // 
            resources.ApplyResources(this.MeasureDate, "MeasureDate");
            this.MeasureDate.Name = "MeasureDate";
            // 
            // MeasureStatus
            // 
            resources.ApplyResources(this.MeasureStatus, "MeasureStatus");
            this.MeasureStatus.Name = "MeasureStatus";
            // 
            // DetailedInfo
            // 
            this.DetailedInfo.FillWeight = 400F;
            resources.ApplyResources(this.DetailedInfo, "DetailedInfo");
            this.DetailedInfo.Name = "DetailedInfo";
            // 
            // IsEnglish
            // 
            resources.ApplyResources(this.IsEnglish, "IsEnglish");
            this.IsEnglish.Name = "IsEnglish";
            // 
            // TabCalibration
            // 
            resources.ApplyResources(this.TabCalibration, "TabCalibration");
            this.TabCalibration.Controls.Add(this.BtnDeleteCalibration);
            this.TabCalibration.Controls.Add(this.BtnDeriveCalibration);
            this.TabCalibration.Controls.Add(this.DgvCalibration);
            this.TabCalibration.Name = "TabCalibration";
            this.TabCalibration.UseVisualStyleBackColor = true;
            // 
            // BtnDeleteCalibration
            // 
            resources.ApplyResources(this.BtnDeleteCalibration, "BtnDeleteCalibration");
            this.BtnDeleteCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeleteCalibration.Name = "BtnDeleteCalibration";
            this.BtnDeleteCalibration.UseVisualStyleBackColor = false;
            this.BtnDeleteCalibration.Click += new System.EventHandler(this.BtnDeleteCalibration_Click);
            // 
            // BtnDeriveCalibration
            // 
            resources.ApplyResources(this.BtnDeriveCalibration, "BtnDeriveCalibration");
            this.BtnDeriveCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeriveCalibration.Name = "BtnDeriveCalibration";
            this.BtnDeriveCalibration.UseVisualStyleBackColor = false;
            this.BtnDeriveCalibration.Click += new System.EventHandler(this.BtnDeriveCalibration_Click);
            // 
            // DgvCalibration
            // 
            resources.ApplyResources(this.DgvCalibration, "DgvCalibration");
            this.DgvCalibration.AllowUserToAddRows = false;
            this.DgvCalibration.AllowUserToDeleteRows = false;
            this.DgvCalibration.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvCalibration.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvCalibration.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CalibrationTime,
            this.ChannelID,
            this.HighVoltage,
            this.Threshold,
            this.Efficiency,
            this.MDA,
            this.AlphaBetaPercent});
            this.DgvCalibration.GridColor = System.Drawing.SystemColors.Control;
            this.DgvCalibration.Name = "DgvCalibration";
            this.DgvCalibration.ReadOnly = true;
            this.DgvCalibration.RowTemplate.Height = 23;
            // 
            // CalibrationTime
            // 
            resources.ApplyResources(this.CalibrationTime, "CalibrationTime");
            this.CalibrationTime.Name = "CalibrationTime";
            this.CalibrationTime.ReadOnly = true;
            // 
            // ChannelID
            // 
            resources.ApplyResources(this.ChannelID, "ChannelID");
            this.ChannelID.Name = "ChannelID";
            this.ChannelID.ReadOnly = true;
            // 
            // HighVoltage
            // 
            resources.ApplyResources(this.HighVoltage, "HighVoltage");
            this.HighVoltage.Name = "HighVoltage";
            this.HighVoltage.ReadOnly = true;
            // 
            // Threshold
            // 
            resources.ApplyResources(this.Threshold, "Threshold");
            this.Threshold.Name = "Threshold";
            this.Threshold.ReadOnly = true;
            // 
            // Efficiency
            // 
            resources.ApplyResources(this.Efficiency, "Efficiency");
            this.Efficiency.Name = "Efficiency";
            this.Efficiency.ReadOnly = true;
            // 
            // MDA
            // 
            resources.ApplyResources(this.MDA, "MDA");
            this.MDA.Name = "MDA";
            this.MDA.ReadOnly = true;
            // 
            // AlphaBetaPercent
            // 
            resources.ApplyResources(this.AlphaBetaPercent, "AlphaBetaPercent");
            this.AlphaBetaPercent.Name = "AlphaBetaPercent";
            this.AlphaBetaPercent.ReadOnly = true;
            // 
            // TabError
            // 
            resources.ApplyResources(this.TabError, "TabError");
            this.TabError.Controls.Add(this.BtnDeleteError);
            this.TabError.Controls.Add(this.BtnDeriveError);
            this.TabError.Controls.Add(this.DgvError);
            this.TabError.Name = "TabError";
            this.TabError.UseVisualStyleBackColor = true;
            // 
            // BtnDeleteError
            // 
            resources.ApplyResources(this.BtnDeleteError, "BtnDeleteError");
            this.BtnDeleteError.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeleteError.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnDeleteError.Name = "BtnDeleteError";
            this.BtnDeleteError.UseVisualStyleBackColor = false;
            this.BtnDeleteError.Click += new System.EventHandler(this.BtnDeleteError_Click);
            // 
            // BtnDeriveError
            // 
            resources.ApplyResources(this.BtnDeriveError, "BtnDeriveError");
            this.BtnDeriveError.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeriveError.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnDeriveError.Name = "BtnDeriveError";
            this.BtnDeriveError.UseVisualStyleBackColor = false;
            this.BtnDeriveError.Click += new System.EventHandler(this.BtnDeriveError_Click);
            // 
            // DgvError
            // 
            resources.ApplyResources(this.DgvError, "DgvError");
            this.DgvError.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvError.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvError.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ErrTime,
            this.Record,
            this.IsEnglishsError});
            this.DgvError.Name = "DgvError";
            this.DgvError.RowTemplate.Height = 23;
            // 
            // ErrTime
            // 
            resources.ApplyResources(this.ErrTime, "ErrTime");
            this.ErrTime.Name = "ErrTime";
            // 
            // Record
            // 
            resources.ApplyResources(this.Record, "Record");
            this.Record.Name = "Record";
            // 
            // IsEnglishsError
            // 
            resources.ApplyResources(this.IsEnglishsError, "IsEnglishsError");
            this.IsEnglishsError.Name = "IsEnglishsError";
            // 
            // FrmHistory
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.TabHistory);
            this.Name = "FrmHistory";
            this.Load += new System.EventHandler(this.FrmHistory_Load);
            this.TabHistory.ResumeLayout(false);
            this.TabMeasure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvMeasure)).EndInit();
            this.TabCalibration.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvCalibration)).EndInit();
            this.TabError.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DgvError)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage TabMeasure;
        private System.Windows.Forms.TabPage TabCalibration;
        private System.Windows.Forms.DataGridView DgvMeasure;
        private System.Windows.Forms.TabControl TabHistory;
        private System.Windows.Forms.DataGridView DgvCalibration;
        private System.Windows.Forms.TabPage TabError;
        public System.Windows.Forms.DataGridView DgvError;
        private System.Windows.Forms.Button BtnDeleteCalibration;
        private System.Windows.Forms.Button BtnDeriveCalibration;
        private System.Windows.Forms.Button BtnDeleteError;
        private System.Windows.Forms.Button BtnDeriveError;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalibrationTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelID;
        private System.Windows.Forms.DataGridViewTextBoxColumn HighVoltage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Threshold;
        private System.Windows.Forms.DataGridViewTextBoxColumn Efficiency;
        private System.Windows.Forms.DataGridViewTextBoxColumn MDA;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlphaBetaPercent;
        private System.Windows.Forms.DataGridViewTextBoxColumn MeasureDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn MeasureStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn DetailedInfo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsEnglish;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Record;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsEnglishsError;
        private System.Windows.Forms.Button BtnDeleteMeasure;
        private System.Windows.Forms.Button BtnDeriveMeasure;
    }
}
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
            this.TabHistory = new System.Windows.Forms.TabControl();
            this.TabMeasure = new System.Windows.Forms.TabPage();
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
            this.TabHistory.Controls.Add(this.TabMeasure);
            this.TabHistory.Controls.Add(this.TabCalibration);
            this.TabHistory.Controls.Add(this.TabError);
            this.TabHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabHistory.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.TabHistory.Location = new System.Drawing.Point(0, 0);
            this.TabHistory.Name = "TabHistory";
            this.TabHistory.SelectedIndex = 0;
            this.TabHistory.Size = new System.Drawing.Size(792, 516);
            this.TabHistory.TabIndex = 3;
            this.TabHistory.SelectedIndexChanged += new System.EventHandler(this.TabHistory_SelectedIndexChanged);
            // 
            // TabMeasure
            // 
            this.TabMeasure.BackColor = System.Drawing.SystemColors.Control;
            this.TabMeasure.Controls.Add(this.DgvMeasure);
            this.TabMeasure.Location = new System.Drawing.Point(4, 29);
            this.TabMeasure.Name = "TabMeasure";
            this.TabMeasure.Padding = new System.Windows.Forms.Padding(3);
            this.TabMeasure.Size = new System.Drawing.Size(784, 483);
            this.TabMeasure.TabIndex = 0;
            this.TabMeasure.Text = "测量日志";
            // 
            // DgvMeasure
            // 
            this.DgvMeasure.AllowUserToAddRows = false;
            this.DgvMeasure.AllowUserToDeleteRows = false;
            this.DgvMeasure.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvMeasure.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvMeasure.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MeasureDate,
            this.MeasureStatus,
            this.DetailedInfo,
            this.IsEnglish});
            this.DgvMeasure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvMeasure.Location = new System.Drawing.Point(3, 3);
            this.DgvMeasure.Name = "DgvMeasure";
            this.DgvMeasure.RowTemplate.Height = 23;
            this.DgvMeasure.Size = new System.Drawing.Size(778, 477);
            this.DgvMeasure.TabIndex = 0;
            // 
            // MeasureDate
            // 
            this.MeasureDate.HeaderText = "测量时间";
            this.MeasureDate.Name = "MeasureDate";
            this.MeasureDate.Width = 150;
            // 
            // MeasureStatus
            // 
            this.MeasureStatus.HeaderText = "测量状态";
            this.MeasureStatus.Name = "MeasureStatus";
            this.MeasureStatus.Width = 120;
            // 
            // DetailedInfo
            // 
            this.DetailedInfo.FillWeight = 400F;
            this.DetailedInfo.HeaderText = "详细信息";
            this.DetailedInfo.Name = "DetailedInfo";
            this.DetailedInfo.Width = 370;
            // 
            // IsEnglish
            // 
            this.IsEnglish.HeaderText = "是否英文";
            this.IsEnglish.Name = "IsEnglish";
            // 
            // TabCalibration
            // 
            this.TabCalibration.Controls.Add(this.BtnDeleteCalibration);
            this.TabCalibration.Controls.Add(this.BtnDeriveCalibration);
            this.TabCalibration.Controls.Add(this.DgvCalibration);
            this.TabCalibration.Location = new System.Drawing.Point(4, 29);
            this.TabCalibration.Name = "TabCalibration";
            this.TabCalibration.Padding = new System.Windows.Forms.Padding(3);
            this.TabCalibration.Size = new System.Drawing.Size(784, 483);
            this.TabCalibration.TabIndex = 1;
            this.TabCalibration.Text = "刻度日志";
            this.TabCalibration.UseVisualStyleBackColor = true;
            // 
            // BtnDeleteCalibration
            // 
            this.BtnDeleteCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeleteCalibration.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDeleteCalibration.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.BtnDeleteCalibration.Location = new System.Drawing.Point(66, 457);
            this.BtnDeleteCalibration.Name = "BtnDeleteCalibration";
            this.BtnDeleteCalibration.Size = new System.Drawing.Size(67, 23);
            this.BtnDeleteCalibration.TabIndex = 5;
            this.BtnDeleteCalibration.Text = "删除数据";
            this.BtnDeleteCalibration.UseVisualStyleBackColor = false;
            // 
            // BtnDeriveCalibration
            // 
            this.BtnDeriveCalibration.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeriveCalibration.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDeriveCalibration.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.BtnDeriveCalibration.Location = new System.Drawing.Point(3, 457);
            this.BtnDeriveCalibration.Name = "BtnDeriveCalibration";
            this.BtnDeriveCalibration.Size = new System.Drawing.Size(67, 23);
            this.BtnDeriveCalibration.TabIndex = 4;
            this.BtnDeriveCalibration.Text = "导出数据";
            this.BtnDeriveCalibration.UseVisualStyleBackColor = false;
            this.BtnDeriveCalibration.Click += new System.EventHandler(this.BtnDeriveCalibration_Click);
            // 
            // DgvCalibration
            // 
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
            this.DgvCalibration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvCalibration.GridColor = System.Drawing.SystemColors.Control;
            this.DgvCalibration.Location = new System.Drawing.Point(3, 3);
            this.DgvCalibration.Name = "DgvCalibration";
            this.DgvCalibration.ReadOnly = true;
            this.DgvCalibration.RowTemplate.Height = 23;
            this.DgvCalibration.Size = new System.Drawing.Size(778, 477);
            this.DgvCalibration.TabIndex = 3;
            // 
            // CalibrationTime
            // 
            this.CalibrationTime.HeaderText = "刻度时间";
            this.CalibrationTime.Name = "CalibrationTime";
            this.CalibrationTime.ReadOnly = true;
            this.CalibrationTime.Width = 105;
            // 
            // ChannelID
            // 
            this.ChannelID.HeaderText = "刻度通道";
            this.ChannelID.Name = "ChannelID";
            this.ChannelID.ReadOnly = true;
            this.ChannelID.Width = 105;
            // 
            // HighVoltage
            // 
            this.HighVoltage.HeaderText = "高压值";
            this.HighVoltage.Name = "HighVoltage";
            this.HighVoltage.ReadOnly = true;
            this.HighVoltage.Width = 105;
            // 
            // Threshold
            // 
            this.Threshold.HeaderText = "阈值";
            this.Threshold.Name = "Threshold";
            this.Threshold.ReadOnly = true;
            this.Threshold.Width = 105;
            // 
            // Efficiency
            // 
            this.Efficiency.HeaderText = "效率";
            this.Efficiency.Name = "Efficiency";
            this.Efficiency.ReadOnly = true;
            this.Efficiency.Width = 105;
            // 
            // MDA
            // 
            this.MDA.HeaderText = "探测下限";
            this.MDA.Name = "MDA";
            this.MDA.ReadOnly = true;
            this.MDA.Width = 105;
            // 
            // AlphaBetaPercent
            // 
            this.AlphaBetaPercent.HeaderText = "串道比";
            this.AlphaBetaPercent.Name = "AlphaBetaPercent";
            this.AlphaBetaPercent.ReadOnly = true;
            this.AlphaBetaPercent.Width = 105;
            // 
            // TabError
            // 
            this.TabError.Controls.Add(this.BtnDeleteError);
            this.TabError.Controls.Add(this.BtnDeriveError);
            this.TabError.Controls.Add(this.DgvError);
            this.TabError.Location = new System.Drawing.Point(4, 29);
            this.TabError.Name = "TabError";
            this.TabError.Padding = new System.Windows.Forms.Padding(3);
            this.TabError.Size = new System.Drawing.Size(784, 483);
            this.TabError.TabIndex = 2;
            this.TabError.Text = "故障日志";
            this.TabError.UseVisualStyleBackColor = true;
            // 
            // BtnDeleteError
            // 
            this.BtnDeleteError.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeleteError.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDeleteError.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.BtnDeleteError.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnDeleteError.Location = new System.Drawing.Point(66, 457);
            this.BtnDeleteError.Name = "BtnDeleteError";
            this.BtnDeleteError.Size = new System.Drawing.Size(67, 23);
            this.BtnDeleteError.TabIndex = 4;
            this.BtnDeleteError.Text = "删除数据";
            this.BtnDeleteError.UseVisualStyleBackColor = false;
            this.BtnDeleteError.Click += new System.EventHandler(this.BtnDeleteError_Click);
            // 
            // BtnDeriveError
            // 
            this.BtnDeriveError.BackColor = System.Drawing.SystemColors.Control;
            this.BtnDeriveError.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDeriveError.Font = new System.Drawing.Font("Arial Narrow", 8F);
            this.BtnDeriveError.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnDeriveError.Location = new System.Drawing.Point(3, 457);
            this.BtnDeriveError.Name = "BtnDeriveError";
            this.BtnDeriveError.Size = new System.Drawing.Size(67, 23);
            this.BtnDeriveError.TabIndex = 3;
            this.BtnDeriveError.Text = "导出数据";
            this.BtnDeriveError.UseVisualStyleBackColor = false;
            this.BtnDeriveError.Click += new System.EventHandler(this.BtnDeriveError_Click);
            // 
            // DgvError
            // 
            this.DgvError.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DgvError.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvError.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ErrTime,
            this.Record,
            this.IsEnglishsError});
            this.DgvError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvError.Location = new System.Drawing.Point(3, 3);
            this.DgvError.Name = "DgvError";
            this.DgvError.RowTemplate.Height = 23;
            this.DgvError.Size = new System.Drawing.Size(778, 477);
            this.DgvError.TabIndex = 2;
            // 
            // ErrTime
            // 
            this.ErrTime.HeaderText = "故障时间";
            this.ErrTime.Name = "ErrTime";
            this.ErrTime.Width = 150;
            // 
            // Record
            // 
            this.Record.HeaderText = "故障记录";
            this.Record.Name = "Record";
            this.Record.Width = 480;
            // 
            // IsEnglishsError
            // 
            this.IsEnglishsError.HeaderText = "是否英文";
            this.IsEnglishsError.Name = "IsEnglishsError";
            this.IsEnglishsError.Width = 105;
            // 
            // FrmHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 516);
            this.ControlBox = false;
            this.Controls.Add(this.TabHistory);
            this.Name = "FrmHistory";
            this.Text = "历史记录";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn MeasureDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn MeasureStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn DetailedInfo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsEnglish;
        private System.Windows.Forms.DataGridView DgvCalibration;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalibrationTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelID;
        private System.Windows.Forms.DataGridViewTextBoxColumn HighVoltage;
        private System.Windows.Forms.DataGridViewTextBoxColumn Threshold;
        private System.Windows.Forms.DataGridViewTextBoxColumn Efficiency;
        private System.Windows.Forms.DataGridViewTextBoxColumn MDA;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlphaBetaPercent;
        private System.Windows.Forms.TabPage TabError;
        public System.Windows.Forms.DataGridView DgvError;
        private System.Windows.Forms.Button BtnDeleteCalibration;
        private System.Windows.Forms.Button BtnDeriveCalibration;
        private System.Windows.Forms.Button BtnDeleteError;
        private System.Windows.Forms.Button BtnDeriveError;
        private System.Windows.Forms.DataGridViewTextBoxColumn ErrTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Record;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsEnglishsError;
    }
}
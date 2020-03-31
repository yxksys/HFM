namespace HFM
{
    partial class FrmClothes
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
            this.Txt = new System.Windows.Forms.Label();
            this.LblBackground = new System.Windows.Forms.Label();
            this.LblMeasure = new System.Windows.Forms.Label();
            this.TxtBackgroundValue = new System.Windows.Forms.TextBox();
            this.TxtMeasureValue = new System.Windows.Forms.TextBox();
            this.loadingCircle = new HFM.Controls.LoadingCircle();
            this.PrgClothAlarm_1 = new System.Windows.Forms.ProgressBar();
            this.PrgClothAlarm_2 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Txt
            // 
            this.Txt.Font = new System.Drawing.Font("Arial Narrow", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Txt.Location = new System.Drawing.Point(4, 19);
            this.Txt.Name = "Txt";
            this.Txt.Size = new System.Drawing.Size(582, 38);
            this.Txt.TabIndex = 15;
            this.Txt.Text = "正在进行衣物测量";
            this.Txt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblBackground
            // 
            this.LblBackground.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblBackground.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblBackground.Location = new System.Drawing.Point(4, 86);
            this.LblBackground.Name = "LblBackground";
            this.LblBackground.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LblBackground.Size = new System.Drawing.Size(161, 36);
            this.LblBackground.TabIndex = 16;
            this.LblBackground.Text = "本底值";
            // 
            // LblMeasure
            // 
            this.LblMeasure.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblMeasure.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblMeasure.Location = new System.Drawing.Point(4, 145);
            this.LblMeasure.Name = "LblMeasure";
            this.LblMeasure.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LblMeasure.Size = new System.Drawing.Size(161, 36);
            this.LblMeasure.TabIndex = 17;
            this.LblMeasure.Text = "测量值";
            // 
            // TxtBackgroundValue
            // 
            this.TxtBackgroundValue.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtBackgroundValue.Location = new System.Drawing.Point(180, 83);
            this.TxtBackgroundValue.Name = "TxtBackgroundValue";
            this.TxtBackgroundValue.ReadOnly = true;
            this.TxtBackgroundValue.Size = new System.Drawing.Size(300, 39);
            this.TxtBackgroundValue.TabIndex = 19;
            this.TxtBackgroundValue.TabStop = false;
            // 
            // TxtMeasureValue
            // 
            this.TxtMeasureValue.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtMeasureValue.Location = new System.Drawing.Point(180, 140);
            this.TxtMeasureValue.Name = "TxtMeasureValue";
            this.TxtMeasureValue.ReadOnly = true;
            this.TxtMeasureValue.Size = new System.Drawing.Size(300, 39);
            this.TxtMeasureValue.TabIndex = 20;
            this.TxtMeasureValue.TabStop = false;
            // 
            // loadingCircle
            // 
            this.loadingCircle.Active = false;
            this.loadingCircle.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircle.Color = System.Drawing.Color.Orange;
            this.loadingCircle.InnerCircleRadius = 5;
            this.loadingCircle.Location = new System.Drawing.Point(486, 132);
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 12;
            this.loadingCircle.OuterCircleRadius = 11;
            this.loadingCircle.RotationSpeed = 100;
            this.loadingCircle.Size = new System.Drawing.Size(75, 56);
            this.loadingCircle.SpokeThickness = 2;
            this.loadingCircle.StylePreset = HFM.Controls.LoadingCircle.StylePresets.MacOSX;
            this.loadingCircle.TabIndex = 21;
            this.loadingCircle.Text = "loadingCircle1";
            // 
            // PrgClothAlarm_1
            // 
            this.PrgClothAlarm_1.Location = new System.Drawing.Point(180, 204);
            this.PrgClothAlarm_1.Name = "PrgClothAlarm_1";
            this.PrgClothAlarm_1.Size = new System.Drawing.Size(300, 10);
            this.PrgClothAlarm_1.TabIndex = 22;
            // 
            // PrgClothAlarm_2
            // 
            this.PrgClothAlarm_2.Location = new System.Drawing.Point(180, 239);
            this.PrgClothAlarm_2.Name = "PrgClothAlarm_2";
            this.PrgClothAlarm_2.Size = new System.Drawing.Size(300, 10);
            this.PrgClothAlarm_2.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(4, 193);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(161, 36);
            this.label1.TabIndex = 24;
            this.label1.Text = "污染报警";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(4, 229);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label2.Size = new System.Drawing.Size(161, 36);
            this.label2.TabIndex = 25;
            this.label2.Text = "高阶报警";
            // 
            // FrmClothes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SpringGreen;
            this.ClientSize = new System.Drawing.Size(589, 308);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PrgClothAlarm_2);
            this.Controls.Add(this.PrgClothAlarm_1);
            this.Controls.Add(this.loadingCircle);
            this.Controls.Add(this.TxtMeasureValue);
            this.Controls.Add(this.TxtBackgroundValue);
            this.Controls.Add(this.LblMeasure);
            this.Controls.Add(this.LblBackground);
            this.Controls.Add(this.Txt);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmClothes";
            this.Text = "衣物探头";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmClothes_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Txt;
        private System.Windows.Forms.Label LblBackground;
        private System.Windows.Forms.Label LblMeasure;
        public System.Windows.Forms.TextBox TxtBackgroundValue;
        public System.Windows.Forms.TextBox TxtMeasureValue;
        public Controls.LoadingCircle loadingCircle;
        public System.Windows.Forms.ProgressBar PrgClothAlarm_1;
        public System.Windows.Forms.ProgressBar PrgClothAlarm_2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
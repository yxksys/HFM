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
            this.PrgClothC = new System.Windows.Forms.ProgressBar();
            this.TxtBackgroundValue = new System.Windows.Forms.TextBox();
            this.TxtMeasureValue = new System.Windows.Forms.TextBox();
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
            // PrgClothC
            // 
            this.PrgClothC.Cursor = System.Windows.Forms.Cursors.Default;
            this.PrgClothC.Location = new System.Drawing.Point(67, 214);
            this.PrgClothC.Name = "PrgClothC";
            this.PrgClothC.Size = new System.Drawing.Size(431, 25);
            this.PrgClothC.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.PrgClothC.TabIndex = 18;
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
            // FrmClothes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SpringGreen;
            this.ClientSize = new System.Drawing.Size(589, 277);
            this.ControlBox = false;
            this.Controls.Add(this.TxtMeasureValue);
            this.Controls.Add(this.TxtBackgroundValue);
            this.Controls.Add(this.PrgClothC);
            this.Controls.Add(this.LblMeasure);
            this.Controls.Add(this.LblBackground);
            this.Controls.Add(this.Txt);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmClothes";
            this.Text = "衣物探头";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Txt;
        private System.Windows.Forms.Label LblBackground;
        private System.Windows.Forms.Label LblMeasure;
        public System.Windows.Forms.ProgressBar PrgClothC;
        public System.Windows.Forms.TextBox TxtBackgroundValue;
        public System.Windows.Forms.TextBox TxtMeasureValue;
    }
}
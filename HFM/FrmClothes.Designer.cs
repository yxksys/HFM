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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmClothes));
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
            resources.ApplyResources(this.Txt, "Txt");
            this.Txt.Name = "Txt";
            // 
            // LblBackground
            // 
            resources.ApplyResources(this.LblBackground, "LblBackground");
            this.LblBackground.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblBackground.Name = "LblBackground";
            // 
            // LblMeasure
            // 
            resources.ApplyResources(this.LblMeasure, "LblMeasure");
            this.LblMeasure.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblMeasure.Name = "LblMeasure";
            // 
            // TxtBackgroundValue
            // 
            resources.ApplyResources(this.TxtBackgroundValue, "TxtBackgroundValue");
            this.TxtBackgroundValue.Name = "TxtBackgroundValue";
            this.TxtBackgroundValue.ReadOnly = true;
            this.TxtBackgroundValue.TabStop = false;
            // 
            // TxtMeasureValue
            // 
            resources.ApplyResources(this.TxtMeasureValue, "TxtMeasureValue");
            this.TxtMeasureValue.Name = "TxtMeasureValue";
            this.TxtMeasureValue.ReadOnly = true;
            this.TxtMeasureValue.TabStop = false;
            // 
            // loadingCircle
            // 
            resources.ApplyResources(this.loadingCircle, "loadingCircle");
            this.loadingCircle.Active = false;
            this.loadingCircle.BackColor = System.Drawing.Color.Transparent;
            this.loadingCircle.Color = System.Drawing.Color.Orange;
            this.loadingCircle.InnerCircleRadius = 5;
            this.loadingCircle.Name = "loadingCircle";
            this.loadingCircle.NumberSpoke = 12;
            this.loadingCircle.OuterCircleRadius = 11;
            this.loadingCircle.RotationSpeed = 100;
            this.loadingCircle.SpokeThickness = 2;
            this.loadingCircle.StylePreset = HFM.Controls.LoadingCircle.StylePresets.MacOSX;
            // 
            // PrgClothAlarm_1
            // 
            resources.ApplyResources(this.PrgClothAlarm_1, "PrgClothAlarm_1");
            this.PrgClothAlarm_1.Name = "PrgClothAlarm_1";
            // 
            // PrgClothAlarm_2
            // 
            resources.ApplyResources(this.PrgClothAlarm_2, "PrgClothAlarm_2");
            this.PrgClothAlarm_2.Name = "PrgClothAlarm_2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Name = "label2";
            // 
            // FrmClothes
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SpringGreen;
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
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmClothes";
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
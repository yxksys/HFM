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
            this.LblTitle = new System.Windows.Forms.Label();
            this.LblBKground_Clothes = new System.Windows.Forms.Label();
            this.LblMeasure = new System.Windows.Forms.Label();
            this.TxtBackgroundValue = new System.Windows.Forms.TextBox();
            this.TxtMeasureValue = new System.Windows.Forms.TextBox();
            this.PrgClothAlarm_1 = new System.Windows.Forms.ProgressBar();
            this.PrgClothAlarm_2 = new System.Windows.Forms.ProgressBar();
            this.LblAlarm_1 = new System.Windows.Forms.Label();
            this.LblAlarm_2 = new System.Windows.Forms.Label();
            this.loadingCircle = new HFM.Controls.LoadingCircle();
            this.SuspendLayout();
            // 
            // LblTitle
            // 
            resources.ApplyResources(this.LblTitle, "LblTitle");
            this.LblTitle.Name = "LblTitle";
            // 
            // LblBKground_Clothes
            // 
            resources.ApplyResources(this.LblBKground_Clothes, "LblBKground_Clothes");
            this.LblBKground_Clothes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblBKground_Clothes.Name = "LblBKground_Clothes";
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
            // LblAlarm_1
            // 
            resources.ApplyResources(this.LblAlarm_1, "LblAlarm_1");
            this.LblAlarm_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblAlarm_1.Name = "LblAlarm_1";
            // 
            // LblAlarm_2
            // 
            resources.ApplyResources(this.LblAlarm_2, "LblAlarm_2");
            this.LblAlarm_2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LblAlarm_2.Name = "LblAlarm_2";
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
            // FrmClothes
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SpringGreen;
            this.ControlBox = false;
            this.Controls.Add(this.LblAlarm_2);
            this.Controls.Add(this.LblAlarm_1);
            this.Controls.Add(this.PrgClothAlarm_2);
            this.Controls.Add(this.PrgClothAlarm_1);
            this.Controls.Add(this.loadingCircle);
            this.Controls.Add(this.TxtMeasureValue);
            this.Controls.Add(this.TxtBackgroundValue);
            this.Controls.Add(this.LblMeasure);
            this.Controls.Add(this.LblBKground_Clothes);
            this.Controls.Add(this.LblTitle);
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

        private System.Windows.Forms.Label LblTitle;
        private System.Windows.Forms.Label LblBKground_Clothes;
        private System.Windows.Forms.Label LblMeasure;
        public System.Windows.Forms.TextBox TxtBackgroundValue;
        public System.Windows.Forms.TextBox TxtMeasureValue;
        public Controls.LoadingCircle loadingCircle;
        public System.Windows.Forms.ProgressBar PrgClothAlarm_1;
        public System.Windows.Forms.ProgressBar PrgClothAlarm_2;
        private System.Windows.Forms.Label LblAlarm_1;
        private System.Windows.Forms.Label LblAlarm_2;
    }
}
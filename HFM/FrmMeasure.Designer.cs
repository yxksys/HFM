namespace HFM
{
    partial class FrmMeasure
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
            this.bkWorkerReceiveData = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // bkWorkerReceiveData
            // 
            this.bkWorkerReceiveData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkWorkerReceiveData_DoWork);
            this.bkWorkerReceiveData.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bkWorkerReceiveData_ProgressChanged);
            // 
            // FrmMeasure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Name = "FrmMeasure";
            this.Text = "FrmMeasure";
            this.Load += new System.EventHandler(this.FrmMeasure_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bkWorkerReceiveData;
    }
}
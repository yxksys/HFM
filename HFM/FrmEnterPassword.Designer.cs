namespace HFM
{
    partial class FrmEnterPassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEnterPassword));
            this.LblPassword = new System.Windows.Forms.Label();
            this.TxtPassword = new System.Windows.Forms.TextBox();
            this.BtnConfirm = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LblPassword
            // 
            resources.ApplyResources(this.LblPassword, "LblPassword");
            this.LblPassword.Name = "LblPassword";
            // 
            // TxtPassword
            // 
            resources.ApplyResources(this.TxtPassword, "TxtPassword");
            this.TxtPassword.Name = "TxtPassword";
            this.TxtPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtPassword_MouseClick);
            // 
            // BtnConfirm
            // 
            resources.ApplyResources(this.BtnConfirm, "BtnConfirm");
            this.BtnConfirm.Name = "BtnConfirm";
            this.BtnConfirm.UseVisualStyleBackColor = true;
            this.BtnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            // 
            // BtnCancel
            // 
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FrmEnterPassword
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnConfirm);
            this.Controls.Add(this.TxtPassword);
            this.Controls.Add(this.LblPassword);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmEnterPassword";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmEnterPassword_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblPassword;
        private System.Windows.Forms.TextBox TxtPassword;
        private System.Windows.Forms.Button BtnConfirm;
        private System.Windows.Forms.Button BtnCancel;
    }
}
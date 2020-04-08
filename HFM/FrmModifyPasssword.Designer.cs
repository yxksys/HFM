namespace HFM
{
    partial class FrmModifyPasssword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmModifyPasssword));
            this.LblOldPassword = new System.Windows.Forms.Label();
            this.TxtOldPassword = new System.Windows.Forms.TextBox();
            this.LblNewPassword = new System.Windows.Forms.Label();
            this.TxtNewPassword = new System.Windows.Forms.TextBox();
            this.LblFinalPsaaword = new System.Windows.Forms.Label();
            this.TxtFinalPsaaword = new System.Windows.Forms.TextBox();
            this.BtnConfirm = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LblOldPassword
            // 
            resources.ApplyResources(this.LblOldPassword, "LblOldPassword");
            this.LblOldPassword.Name = "LblOldPassword";
            // 
            // TxtOldPassword
            // 
            resources.ApplyResources(this.TxtOldPassword, "TxtOldPassword");
            this.TxtOldPassword.Name = "TxtOldPassword";
            this.TxtOldPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtOldPassword_MouseClick);
            // 
            // LblNewPassword
            // 
            resources.ApplyResources(this.LblNewPassword, "LblNewPassword");
            this.LblNewPassword.Name = "LblNewPassword";
            // 
            // TxtNewPassword
            // 
            resources.ApplyResources(this.TxtNewPassword, "TxtNewPassword");
            this.TxtNewPassword.Name = "TxtNewPassword";
            this.TxtNewPassword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtNewPassword_MouseClick);
            // 
            // LblFinalPsaaword
            // 
            resources.ApplyResources(this.LblFinalPsaaword, "LblFinalPsaaword");
            this.LblFinalPsaaword.Name = "LblFinalPsaaword";
            // 
            // TxtFinalPsaaword
            // 
            resources.ApplyResources(this.TxtFinalPsaaword, "TxtFinalPsaaword");
            this.TxtFinalPsaaword.Name = "TxtFinalPsaaword";
            this.TxtFinalPsaaword.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TxtFinalPsaaword_MouseClick);
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
            // FrmModifyPasssword
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnConfirm);
            this.Controls.Add(this.TxtFinalPsaaword);
            this.Controls.Add(this.LblFinalPsaaword);
            this.Controls.Add(this.TxtNewPassword);
            this.Controls.Add(this.LblNewPassword);
            this.Controls.Add(this.TxtOldPassword);
            this.Controls.Add(this.LblOldPassword);
            this.Name = "FrmModifyPasssword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblOldPassword;
        private System.Windows.Forms.TextBox TxtOldPassword;
        private System.Windows.Forms.Label LblNewPassword;
        private System.Windows.Forms.TextBox TxtNewPassword;
        private System.Windows.Forms.Label LblFinalPsaaword;
        private System.Windows.Forms.TextBox TxtFinalPsaaword;
        private System.Windows.Forms.Button BtnConfirm;
        private System.Windows.Forms.Button BtnCancel;
    }
}
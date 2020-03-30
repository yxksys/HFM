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
            this.LblOldPassword.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblOldPassword.Location = new System.Drawing.Point(13, 46);
            this.LblOldPassword.Name = "LblOldPassword";
            this.LblOldPassword.Size = new System.Drawing.Size(135, 18);
            this.LblOldPassword.TabIndex = 21;
            this.LblOldPassword.Text = "当前密码";
            this.LblOldPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtOldPassword
            // 
            this.TxtOldPassword.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtOldPassword.Location = new System.Drawing.Point(154, 44);
            this.TxtOldPassword.Name = "TxtOldPassword";
            this.TxtOldPassword.PasswordChar = '*';
            this.TxtOldPassword.Size = new System.Drawing.Size(132, 26);
            this.TxtOldPassword.TabIndex = 29;
            this.TxtOldPassword.TextChanged += new System.EventHandler(this.Txt_TextChanged);
            // 
            // LblNewPassword
            // 
            this.LblNewPassword.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblNewPassword.Location = new System.Drawing.Point(32, 77);
            this.LblNewPassword.Name = "LblNewPassword";
            this.LblNewPassword.Size = new System.Drawing.Size(116, 18);
            this.LblNewPassword.TabIndex = 22;
            this.LblNewPassword.Text = "新的密码";
            this.LblNewPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtNewPassword
            // 
            this.TxtNewPassword.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtNewPassword.Location = new System.Drawing.Point(154, 76);
            this.TxtNewPassword.Name = "TxtNewPassword";
            this.TxtNewPassword.PasswordChar = '*';
            this.TxtNewPassword.Size = new System.Drawing.Size(132, 26);
            this.TxtNewPassword.TabIndex = 30;
            this.TxtNewPassword.TextChanged += new System.EventHandler(this.Txt_TextChanged);
            // 
            // LblFinalPsaaword
            // 
            this.LblFinalPsaaword.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblFinalPsaaword.Location = new System.Drawing.Point(24, 108);
            this.LblFinalPsaaword.Name = "LblFinalPsaaword";
            this.LblFinalPsaaword.Size = new System.Drawing.Size(124, 18);
            this.LblFinalPsaaword.TabIndex = 31;
            this.LblFinalPsaaword.Text = "重复密码";
            this.LblFinalPsaaword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtFinalPsaaword
            // 
            this.TxtFinalPsaaword.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtFinalPsaaword.Location = new System.Drawing.Point(154, 108);
            this.TxtFinalPsaaword.Name = "TxtFinalPsaaword";
            this.TxtFinalPsaaword.PasswordChar = '*';
            this.TxtFinalPsaaword.Size = new System.Drawing.Size(132, 26);
            this.TxtFinalPsaaword.TabIndex = 32;
            this.TxtFinalPsaaword.TextChanged += new System.EventHandler(this.Txt_TextChanged);
            // 
            // BtnConfirm
            // 
            this.BtnConfirm.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnConfirm.Location = new System.Drawing.Point(84, 154);
            this.BtnConfirm.Name = "BtnConfirm";
            this.BtnConfirm.Size = new System.Drawing.Size(75, 32);
            this.BtnConfirm.TabIndex = 33;
            this.BtnConfirm.Text = "确定";
            this.BtnConfirm.UseVisualStyleBackColor = true;
            this.BtnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCancel.Location = new System.Drawing.Point(193, 154);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 32);
            this.BtnCancel.TabIndex = 34;
            this.BtnCancel.Text = "取消";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FrmModifyPasssword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 216);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnConfirm);
            this.Controls.Add(this.TxtFinalPsaaword);
            this.Controls.Add(this.LblFinalPsaaword);
            this.Controls.Add(this.TxtNewPassword);
            this.Controls.Add(this.LblNewPassword);
            this.Controls.Add(this.TxtOldPassword);
            this.Controls.Add(this.LblOldPassword);
            this.Name = "FrmModifyPasssword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改密码";
            this.Load += new System.EventHandler(this.FrmModifyPasssword_Load);
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
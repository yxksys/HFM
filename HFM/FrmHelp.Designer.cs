namespace HFM
{
    partial class FrmHelp
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
            this.LblName = new System.Windows.Forms.Label();
            this.LblVersions = new System.Windows.Forms.Label();
            this.LblNumber = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LblName
            // 
            this.LblName.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblName.Location = new System.Drawing.Point(58, 86);
            this.LblName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LblName.Name = "LblName";
            this.LblName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LblName.Size = new System.Drawing.Size(521, 23);
            this.LblName.TabIndex = 2;
            this.LblName.Text = "仪器名称：HFM100手脚表面污染监测仪";
            this.LblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LblVersions
            // 
            this.LblVersions.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblVersions.Location = new System.Drawing.Point(58, 136);
            this.LblVersions.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LblVersions.Name = "LblVersions";
            this.LblVersions.Size = new System.Drawing.Size(521, 23);
            this.LblVersions.TabIndex = 1;
            this.LblVersions.Text = "软件版本：";
            // 
            // LblNumber
            // 
            this.LblNumber.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblNumber.Location = new System.Drawing.Point(58, 186);
            this.LblNumber.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LblNumber.Name = "LblNumber";
            this.LblNumber.Size = new System.Drawing.Size(521, 23);
            this.LblNumber.TabIndex = 5;
            this.LblNumber.Text = "联系方式：0351-2202150  2203549";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(58, 236);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(521, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "版权所有（C） 山西中辐核仪器有限责任公司";
            // 
            // FrmHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 366);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LblNumber);
            this.Controls.Add(this.LblVersions);
            this.Controls.Add(this.LblName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmHelp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "关于";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LblName;
        private System.Windows.Forms.Label LblVersions;
        private System.Windows.Forms.Label LblNumber;
        private System.Windows.Forms.Label label3;
    }
}
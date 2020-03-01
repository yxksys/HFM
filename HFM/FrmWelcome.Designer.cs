namespace HFM.Components
{
    partial class FrmWelcome
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
            this.BtnTestRorW = new System.Windows.Forms.Button();
            this.BtnCommport = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnTestRorW
            // 
            this.BtnTestRorW.Location = new System.Drawing.Point(160, 30);
            this.BtnTestRorW.Name = "BtnTestRorW";
            this.BtnTestRorW.Size = new System.Drawing.Size(75, 23);
            this.BtnTestRorW.TabIndex = 0;
            this.BtnTestRorW.Text = "测试读写";
            this.BtnTestRorW.UseVisualStyleBackColor = true;
            this.BtnTestRorW.Click += new System.EventHandler(this.BtnTestRorW_Click);
            // 
            // BtnCommport
            // 
            this.BtnCommport.Location = new System.Drawing.Point(160, 84);
            this.BtnCommport.Name = "BtnCommport";
            this.BtnCommport.Size = new System.Drawing.Size(75, 23);
            this.BtnCommport.TabIndex = 1;
            this.BtnCommport.Text = "串口样例";
            this.BtnCommport.UseVisualStyleBackColor = true;
            this.BtnCommport.Click += new System.EventHandler(this.BtnCommport_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(258, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(597, 293);
            this.dataGridView1.TabIndex = 2;
            // 
            // FrmWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 393);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.BtnCommport);
            this.Controls.Add(this.BtnTestRorW);
            this.Name = "FrmWelcome";
            this.Text = "FrmWelcome";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnTestRorW;
        private System.Windows.Forms.Button BtnCommport;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
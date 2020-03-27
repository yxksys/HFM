namespace HFM
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
            this.components = new System.ComponentModel.Container();
            this.BtnTestRorW = new System.Windows.Forms.Button();
            this.BtnCommport = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.hv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.alpha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.alphacnt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.beta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.betacnt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.start = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
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
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.hv,
            this.alpha,
            this.alphacnt,
            this.beta,
            this.betacnt,
            this.start});
            this.dataGridView1.Location = new System.Drawing.Point(728, 176);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(218, 171);
            this.dataGridView1.TabIndex = 2;
            // 
            // hv
            // 
            this.hv.HeaderText = "hv";
            this.hv.Name = "hv";
            this.hv.ReadOnly = true;
            // 
            // alpha
            // 
            this.alpha.HeaderText = "alpha";
            this.alpha.Name = "alpha";
            this.alpha.ReadOnly = true;
            // 
            // alphacnt
            // 
            this.alphacnt.HeaderText = "alphacnt";
            this.alphacnt.Name = "alphacnt";
            this.alphacnt.ReadOnly = true;
            // 
            // beta
            // 
            this.beta.HeaderText = "beta";
            this.beta.Name = "beta";
            this.beta.ReadOnly = true;
            // 
            // betacnt
            // 
            this.betacnt.HeaderText = "betacnt";
            this.betacnt.Name = "betacnt";
            this.betacnt.ReadOnly = true;
            // 
            // start
            // 
            this.start.HeaderText = "start";
            this.start.Name = "start";
            this.start.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(36, 362);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(565, 197);
            this.textBox1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(135, 222);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(757, 430);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FrmWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 585);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.BtnCommport);
            this.Controls.Add(this.BtnTestRorW);
            this.IsMdiContainer = true;
            this.Name = "FrmWelcome";
            this.Text = "FrmWelcome";
            this.Load += new System.EventHandler(this.FrmWelcome_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnTestRorW;
        private System.Windows.Forms.Button BtnCommport;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn hv;
        private System.Windows.Forms.DataGridViewTextBoxColumn alpha;
        private System.Windows.Forms.DataGridViewTextBoxColumn alphacnt;
        private System.Windows.Forms.DataGridViewTextBoxColumn beta;
        private System.Windows.Forms.DataGridViewTextBoxColumn betacnt;
        private System.Windows.Forms.DataGridViewTextBoxColumn start;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
    }
}
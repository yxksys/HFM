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
            this.Time_Welcome_cloes = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // Time_Welcome_cloes
            // 
            this.Time_Welcome_cloes.Interval = 1000;
            this.Time_Welcome_cloes.Tick += new System.EventHandler(this.Time_Welcome_cloes_Tick);
            // 
            // FrmWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::HFM.Properties.Resources.welcome;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IsMdiContainer = true;
            this.Name = "FrmWelcome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "欢迎";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmWelcome_FormClosed);
            this.Load += new System.EventHandler(this.FrmWelcome_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer Time_Welcome_cloes;
    }
}
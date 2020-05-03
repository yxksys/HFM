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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHelp));
            this.LblName = new System.Windows.Forms.Label();
            this.LblVersions = new System.Windows.Forms.Label();
            this.LblNumber = new System.Windows.Forms.Label();
            this.LblCopyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LblName
            // 
            resources.ApplyResources(this.LblName, "LblName");
            this.LblName.Name = "LblName";
            // 
            // LblVersions
            // 
            resources.ApplyResources(this.LblVersions, "LblVersions");
            this.LblVersions.Name = "LblVersions";
            // 
            // LblNumber
            // 
            resources.ApplyResources(this.LblNumber, "LblNumber");
            this.LblNumber.Name = "LblNumber";
            // 
            // LblCopyright
            // 
            resources.ApplyResources(this.LblCopyright, "LblCopyright");
            this.LblCopyright.Name = "LblCopyright";
            // 
            // FrmHelp
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LblCopyright);
            this.Controls.Add(this.LblNumber);
            this.Controls.Add(this.LblVersions);
            this.Controls.Add(this.LblName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmHelp";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LblName;
        private System.Windows.Forms.Label LblVersions;
        private System.Windows.Forms.Label LblNumber;
        private System.Windows.Forms.Label LblCopyright;
    }
}
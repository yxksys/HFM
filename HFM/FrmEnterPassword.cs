using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HFM
{
    public partial class FrmEnterPassword : Form
    {

        private bool isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);

        public FrmEnterPassword()
        {
            InitializeComponent();
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            FrmKeyIn frmKeyIn = new FrmKeyIn();
            frmKeyIn.sendPassword += new SendPassword(ReceivePassword);
            frmKeyIn.Show();
        }
        void ReceivePassword(string str)
        {
            TxtPassword.Text = str;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmEnterPassword_Load(object sender, EventArgs e)
        {
            if (isEnglish == true )
            {
                this.Text = "Maintenance";
                BtnCancel.Text = "Cancel";
                BtnConfirm.Text = "Enter";
                LblPassword.Text = "Password";
            }
            else
            {
                this.Text ="维护密码";
                BtnCancel.Text = "取消";
                BtnConfirm.Text = "确认";
                LblPassword.Text = "密码";
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {

        }
    }
}

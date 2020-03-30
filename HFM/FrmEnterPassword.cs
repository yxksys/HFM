using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{

    public partial class FrmEnterPassword : Form
    {
        #region 字段
        private string _value="";
        private bool _isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        private User _user = new User();

        #endregion
        #region 方法
        public FrmEnterPassword()
        {
            InitializeComponent();
        }
        #endregion 

        #region 传值
        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            FrmKeyIn frmKeyIn = new FrmKeyIn(ReceiveValue,_value);
            frmKeyIn.Show();
        }
        void ReceiveValue(string value)
        {
            TxtPassword.Text = value;
        }
        #endregion

        #region 确认
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            _user.Login(TxtPassword.Text);
            if (_user == null)
            {
            }
            else
            {

            }

        }
        #endregion

        #region 取消
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 窗口加载前
        private void FrmEnterPassword_Load(object sender, EventArgs e)
        {
            if (_isEnglish == true )
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
        #endregion
    }
}

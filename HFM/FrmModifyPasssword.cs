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
    public partial class FrmModifyPasssword : Form
    {

        #region 字段

        TextBox _tempTextBox;
        /// <summary>
        /// 传值
        /// </summary>
        private string _value = "";
        /// <summary>
        /// 判断系统数据库中读取是否开启英文
        /// </summary>
        /// 
        private Components.User _user = new User();
        private bool _isEnglish = false;

        #endregion

        public FrmModifyPasssword()
        {
            InitializeComponent();
        }


        #region 窗体加载前
        private void FrmModifyPasssword_Load(object sender, EventArgs e)
        {
            if(_isEnglish)
            {
                BtnCancel.Text = "Close";
                BtnConfirm.Text = "OK";
                LblFinalPsaaword.Text = "Re-enter Password";
                LblNewPassword.Text = "New Password";
                LblOldPassword.Text = "Current Password";
                this.Text = "Change password";
            }
            else
            {
                BtnCancel.Text = "取消";
                BtnConfirm.Text = "确定";
                LblFinalPsaaword.Text = "重复密码";
                LblNewPassword.Text = "新的密码";
                LblOldPassword.Text = "当前密码";
                this.Text = "修改密码";
            }
        }
        #endregion


        #region 传值
        private void Txt_TextChanged(object sender, EventArgs e)
        {
            FrmKeyIn frmKeyIn = new FrmKeyIn(ReceiveValue, _value);
            frmKeyIn.Show();

            _tempTextBox = (TextBox)sender;
            void ReceiveValue(string value)
            {
                if(_tempTextBox==TxtFinalPsaaword)
                {
                    TxtFinalPsaaword.Text = _value;
                }
                else if(_tempTextBox == TxtNewPassword)
                {
                    TxtNewPassword.Text = _value;
                }
                else if(_tempTextBox==TxtOldPassword)
                {
                    TxtOldPassword.Text = _value;
                }
            }
        }
        
        #endregion

        #region 关闭按钮
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 确认按钮
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (_isEnglish)
            {
                if (TxtOldPassword == TxtFinalPsaaword)
                {
                    
                }
                else
                {
                    MessageBox.Show("Wrong user name or password!", "Error");
                }
            }
            else
            {

            }
        }
        #endregion
    }
}

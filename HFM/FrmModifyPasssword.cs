using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmModifyPasssword : Form
    {
        public FrmModifyPasssword()
        {
            InitializeComponent();
        }

        #region 文本框创建小键盘事件
        /// <summary>
        /// 当前密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtOldPassword_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtOldPassword);
        }
        /// <summary>
        /// 新的密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtNewPassword_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtNewPassword);
        }
        /// <summary>
        /// 重复密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtFinalPsaaword_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtFinalPsaaword);
        }
        #endregion


        private bool isEnglish = new Components.SystemParameter().GetParameter().IsEnglish;
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            MD5 md5=new MD5CryptoServiceProvider();
            //按旧密码查询用户
            User _user=new User().Login(Tools.MD5Encrypt32(TxtOldPassword.Text));
            
            if (_user.Role==0)
            {
                if (isEnglish)
                {
                    MessageBox.Show("Wrong user name or password!", "Error");
                }
                else
                {
                    MessageBox.Show("用户名或密码错误！", "错误");
                }
                TxtOldPassword.Clear();
                TxtOldPassword.Focus();
                
            }
            else if (TxtNewPassword.Text!=TxtFinalPsaaword.Text)
            {
                if (isEnglish)
                {
                    MessageBox.Show("Password does not match!", "Error");
                }
                else
                {
                    MessageBox.Show("新密码两次输入不一致！", "错误");
                }
                
                TxtNewPassword.Clear();
                TxtFinalPsaaword.Clear();
                TxtNewPassword.Focus();
            }
            else if (TxtNewPassword.Text == TxtFinalPsaaword.Text)
            {
                User _userpwd=new User();
                _userpwd.UserName = _user.UserName;
                _userpwd.PassWord = Tools.MD5Encrypt32(TxtNewPassword.Text);
                _user.ChangePassWord(_userpwd);
                if (isEnglish)
                {
                    MessageBox.Show("Success! Please login with new password!","OK");
                }
                else
                {
                    MessageBox.Show("修改成功，请使用新密码登录！","成功");
                }
                this.Close();
            }
            else
            {
                if (isEnglish)
                {
                    MessageBox.Show("Modification failed!");
                }
                else
                {
                    MessageBox.Show("修改失败!");
                }
                
            }
        }


        
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

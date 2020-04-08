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
        
        #endregion

        #region 确认
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            string mm = Tools.MD5Encrypt32(TxtPassword.Text);
            //给对象赋值
            _user =_user.Login(Tools.MD5Encrypt32(TxtPassword.Text));
            //判断对象的角色，超级管理员和普通用户都可以登陆成功
            if (_user.Role == 1||_user.Role==2)
            {
                bool isOpened = false;
                User.LandingUser = _user;
                FrmMain frmMain=new FrmMain();
                for (int i = 0; i < Application.OpenForms.Count; i++)
                {
                    if (frmMain.Name != Application.OpenForms[i].Text)
                    {
                        Application.OpenForms[i].Hide();
                    }
                    if (frmMain.Name == Application.OpenForms[i].Text)          //若该窗体已被打开
                    {
                        frmMain.Activate();               //激活该窗体
                        isOpened = true;                    //设置子窗体的打开标记为true
                    }
                }

                if (!isOpened)                              //若该窗体未打开,则显示该子窗体
                {
                    frmMain.Show();
                }
                this.Close();

                
                if (_isEnglish)
                {
                    MessageBox.Show("Login Successful!", "Success");
                }
                else
                {
                    MessageBox.Show("用户登录成功！", "成功");
                }
            }
            else
            {
                if (_isEnglish)
                {
                    if (MessageBox.Show("Wrong Password!", "Error") == DialogResult.OK)
                    {
                        TxtPassword.Clear();
                        TxtPassword.Focus();
                    }
                }
                else
                {
                    if (MessageBox.Show("密码错误！", "错误") == DialogResult.OK)
                    {
                        TxtPassword.Clear();
                        TxtPassword.Focus();
                    }
                }
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

        private void TxtPassword_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtPassword);
        }
    }
}

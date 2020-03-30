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
        /// <summary>
        /// 传值
        /// </summary>
        private string _value="";
        /// <summary>
        /// 判断系统数据库中读取是否开启英文
        /// </summary>
        private bool _isEnglish = false;
        /// <summary>
        /// 创建用户类的一个对象
        /// </summary>
        private User _user = new User();
        /// <summary>
        /// 登录状态
        /// </summary>
        private bool _loggingStatus = false;


        #endregion
        #region 封装
        public bool LoggingStatus { get => _loggingStatus; set => _loggingStatus = value; }
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
        private  void ReceiveValue(string value)
        {
            TxtPassword.Text = value;
        }
        #endregion

        #region 确认
        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            _user.Login(TxtPassword.Text);
            if(_isEnglish)
            { 
                if (_user == null)
                {
                    MessageBox.Show("Wrong Password!", "Error");
                }
                else
                {
                    MessageBox.Show("Login Successful!", "Success");
                    LoggingStatus = true;
                    this.Close();
                }
            }
            else
            {
                if (_user == null)
                {
                    MessageBox.Show("密码错误！", "错误");
                }
                else
                {
                    MessageBox.Show("用户登录成功！", "成功");
                    LoggingStatus = true;
                    this.Close();
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
            if (_isEnglish )
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

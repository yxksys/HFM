/**
 * ________________________________________________________________________________ 
 *
 *  描述：主窗体
 *  作者：杨旭锴
 *  版本：v1.0
 *  创建时间：2020年3月27日
 *  类名:主窗体
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
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
    public partial class FrmMain : Form
    {
        #region 字段
        /// <summary>
        /// 枚举登陆角色状态信息
        /// </summary>
        public enum LandingRole
        {
            Admin,
            User
        }
        /// <summary>
        /// 当前登陆角色
        /// </summary>
        public LandingRole _LandingRole { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 禁止子窗体被重复打开(打开窗体为默认大小)
        /// </summary>
        /// <param name="formChild">实例子窗体</param>
        public void FrmDisposeNormal(Form formChild)
        {

            bool isOpened = false;                      //定义子窗体打开标记,默认位false
            foreach (Form form in this.MdiChildren)     //循环MDI中的所有子窗体
            {
                //销毁其他不是要打开的窗口实例
                if (formChild.Name != form.Name)
                {
                    form.Dispose();
                }
                if (formChild.Name == form.Name)          //若该窗体已被打开
                {
                    formChild.Activate();               //激活该窗体
                    formChild.StartPosition = FormStartPosition.CenterParent;
                    formChild.WindowState = FormWindowState.Normal;
                    isOpened = true;                    //设置子窗体的打开标记为true
                    formChild.Dispose();                //销毁formChild实例
                    break;
                }
            }

            if (!isOpened)                              //若该窗体未打开,则显示该子窗体
            {
                formChild.MdiParent = this;
                formChild.Show();
            }

        }
        /// <summary>
        /// 禁止子窗体被重复打开(打开窗体为最大化)
        /// </summary>
        /// <param name="formChild">实例子窗体</param>
        public void FrmDisposeMax(Form formChild)
        {

            bool isOpened = false;                      //定义子窗体打开标记,默认位false
            foreach (Form form in this.MdiChildren)     //循环MDI中的所有子窗体
            {
                //销毁其他不是要打开的窗口实例
                if (formChild.Name != form.Name)
                {
                    form.Dispose();
                }
                if (formChild.Name == form.Name)          //若该窗体已被打开
                {
                    formChild.Activate();               //激活该窗体
                    formChild.StartPosition = FormStartPosition.CenterParent;
                    formChild.WindowState = FormWindowState.Maximized;
                    isOpened = true;                    //设置子窗体的打开标记为true
                    formChild.Dispose();                //销毁formChild实例
                    break;
                }
            }

            if (!isOpened)                              //若该窗体未打开,则显示该子窗体
            {
                formChild.MdiParent = this;
                formChild.WindowState = FormWindowState.Maximized;
                formChild.Show();
            }

        }
        #endregion
        
        #region 构造函数
        public FrmMain()
        {
            InitializeComponent();
        } 
        #endregion

        #region 系统
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartRunningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMeasureMain frmMeasureMain = new FrmMeasureMain();
            frmMeasureMain.Show();
            this.Hide();
        }

        /// <summary>
        /// 超级用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuperUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* 判断当前是否是超级用户
             * 是:弹出提示框,让用户选择退出当前用户状态
             * 否:弹出维护密码窗体
             */
            if (_LandingRole == LandingRole.Admin)
            {
                //提示框
                if (MessageBox.Show("是否退出当前管理用户?", "提示", MessageBoxButtons.OKCancel)==DialogResult.OK)
                {
                    _LandingRole = LandingRole.User;
                }
                else
                {
                    _LandingRole = LandingRole.Admin;
                }

            }
            else
            {
                //开起维护密码窗体
                FrmDisposeNormal(new FrmEnterPassword());
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisposeNormal(new FrmModifyPasssword());
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetreatSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //释放当前所有资源
            this.Dispose();
        }
        #endregion

        #region 设置
        /// <summary>
        /// 参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParameterSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisposeMax(new FrmPreference());
        }

        #endregion

        #region 维护
        /// <summary>
        /// 刻度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisposeMax(new FrmCalibration());
        }
        /// <summary>
        /// 硬件检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestHardwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisposeMax(new FrmTestHardware());
        }

        #endregion

        #region 记录
        /// <summary>
        /// 历史记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HistoricalRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisposeMax(new FrmHistory());
        }

        #endregion

        #region 关于
        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutHFMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //开启关于窗体
            FrmDisposeNormal(new FrmHelp());
        }
        #endregion

        
    }
}

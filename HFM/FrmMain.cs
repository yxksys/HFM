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
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmMain : Form
    {
        #region 字段

        public static bool isEnglish = new HFM.Components.SystemParameter().GetParameter().IsEnglish;        
        #endregion

        #region 实例
        /// <summary>
        /// 查询到当前工厂参数的实例
        /// </summary>
        FactoryParameter _factoryParameter = new FactoryParameter().GetParameter();
        /// <summary>
        /// 查询当前的系统参数实例
        /// </summary>
        private Components.SystemParameter _systemParameter = new Components.SystemParameter().GetParameter();
        /// <summary>
        /// 端口类实例
        /// </summary>
        public CommPort _commPort = new CommPort();
        /// <summary>
        /// 工具类实例
        /// </summary>
        private Tools _tools=new Tools();
        /// <summary>
        /// 实例化Timer类，设置间隔时间为10000毫秒
        /// </summary>
        System.Timers.Timer TmrStatus = new System.Timers.Timer(10000);

       // public static FrmMeasureMain frmMeasureMain = new FrmMeasureMain();

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
                    form.Close();
                }
                if (formChild.Name == form.Name)          //若该窗体已被打开
                {
                    formChild.Activate();               //激活该窗体
                    formChild.StartPosition = FormStartPosition.CenterParent;
                    formChild.WindowState = FormWindowState.Normal;
                    isOpened = true;                    //设置子窗体的打开标记为true
                    formChild.Close();                //销毁formChild实例
                    break;
                }
            }

            if (!isOpened)                              //若该窗体未打开,则显示该子窗体
            {
                formChild.MdiParent = this;
                formChild.Show();
            }


            //bool isOpened = false;                      //定义子窗体打开标记,默认位false
            //foreach (Form form in this.MdiChildren)     //循环MDI中的所有子窗体
            //{
            //    //销毁其他不是要打开的窗口实例
            //    if (formChild.Name != form.Name)
            //    {
            //        // form.Dispose();
            //        form.Close();
            //    }
            //    if (formChild.Name == form.Name)          //若该窗体已被打开
            //    {
            //        formChild.Activate();               //激活该窗体
            //        formChild.StartPosition = FormStartPosition.CenterParent;
            //        formChild.WindowState = FormWindowState.Normal;
            //        isOpened = true;                    //设置子窗体的打开标记为true
            //        formChild.Close();                //销毁formChild实例
            //        break;
            //    }
            //}

            //if (!isOpened)                              //若该窗体未打开,则显示该子窗体
            //{
            //    formChild.MdiParent = this;
            //    formChild.WindowState = FormWindowState.Maximized;
            //    formChild.Show();
            //}

        }
        public void FrmDispose(Form formChild)
        {

            bool isOpened = false;                      //定义子窗体打开标记,默认位false
            foreach (Form form in this.MdiChildren)     //循环MDI中的所有子窗体
            {
                //销毁其他不是要打开的窗口实例
                if (formChild.Name != form.Name)
                {
                    form.Close();
                }                
            }
            if (!isOpened)                              //若该窗体未打开,则显示该子窗体
            {                
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
                    // form.Dispose();
                    form.Close();
                }
                if (formChild.Name == form.Name)          //若该窗体已被打开
                {
                    formChild.Activate();               //激活该窗体
                    formChild.StartPosition = FormStartPosition.CenterParent;
                    formChild.WindowState = FormWindowState.Maximized;
                    isOpened = true;                    //设置子窗体的打开标记为true
                    formChild.Close();                //销毁formChild实例
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
        /// <summary>
        /// 窗口底部信息判断
        /// </summary>
        private void Tsslbl_Status_Text()
        {
            if (isEnglish)
            {
                if (Tools.FormBottomPortStatus==true)
                {
                    Tsslbl_Status.Text = @"COM Connected!";
                }
                else
                {
                    Tsslbl_Status.Text = @"COM Fault!";
                }
            }
            else
            {
                if (Tools.FormBottomPortStatus == true)
                {
                    Tsslbl_Status.Text = @"通信正常";
                }
                else
                {
                    Tsslbl_Status.Text = @"通信故障";
                }
            }
        }
        #endregion
        #region 开启端口
        //实例化串口

        public void OpenComPort()
        {
            //从配置文件获得当前串口配置
            _commPort.GetCommPortSet("commportSet");
            if (_commPort.Opened == true)
            {
                _commPort.Close();
            }           
            //打开串口
            try
            {
                _commPort.Open();
                if (_commPort.Opened)
                {
                    Tools.FormBottomPortStatus = true;                    
                }                     
            }
            catch
            {
                if (FrmMain.isEnglish == true)
                {
                    MessageBox.Show("Port open error! Please check whether the communication is normal.");
                }
                else
                {
                    MessageBox.Show("端口打开错误！请检查通讯是否正常。");
                }
                Tools.FormBottomPortStatus = false;                
            }                                                   
        }
        #endregion 开启端口

        #region 构造函数
        public FrmMain()
        {

        }
        public FrmMain(CommPort commPort)
        {
            this._commPort = commPort;
            InitializeComponent();
            Text = _factoryParameter.SoftName;            //头部软件名称显示
            Tsslbl_Name.Text = _factoryParameter.SoftName;//底部软件名称显示

            //设置timer可用
            //TmrStatus.Enabled = true;
            //设置timer
            //TmrStatus.Interval = 1000;
            //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
            //TmrStatus.AutoReset = true;
            //TmrStatus.Elapsed += new System.Timers.ElapsedEventHandler(TmrStatus_Elapsed);

        }
        #endregion

        #region 事件
        /// <summary>
        /// 计时器事件,实时更新底部状态信息和时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmrStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Tsslbl_NowTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                Tsslbl_Status_Text();
            }
            catch (Exception exception)
            {
                Tools.ErrorLog(exception.ToString());
            }
            
        } 
        #endregion

        #region 启动加载
        private void FrmMain_Load(object sender, EventArgs e)
        {            
                              
        }
        /// <summary>
        /// 启动加载首次处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Shown(object sender, EventArgs e)
        {                       
            OpenComPort();
        }

        #endregion

        #region 系统
        /// <summary>
        /// 开始运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartRunningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {

                if (Application.OpenForms[i].Name=="FrmMeasureMain")
                {
                    Application.OpenForms[i].Show();
                }
            }
            //this._commPort.Close();
            //Thread.Sleep(200);
            //Tsslbl_NowTime.Dispose();
            //FrmDispose(new FrmMeasureMain());
            this.Dispose();
        }

        /// <summary>
        /// 超级用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuperUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //关闭其他子窗体
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {

                if (this.Name != Application.OpenForms[i].Name)
                {
                    Application.OpenForms[i].Close();
                }

            }
            /* 判断当前是否是超级用户
             * 是:弹出提示框,让用户选择退出当前用户状态
             * 否:弹出维护密码窗体
             */
            if (User.LandingUser.Role ==1 )
            {
                //提示框
                if (MessageBox.Show("是否退出当前用户?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    User.LandingUser = User.LandingUser.GetUser(3);
                }
            }
            else
            {
                if (MessageBox.Show("是否重新登录？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //开起维护密码窗体
                    //FrmEnterPassword frmEnterPassword = new FrmEnterPassword();
                    //frmEnterPassword.Show();
                    FrmDisposeNormal(new FrmEnterPassword());
                    //FrmDisposeMax(new FrmEnterPassword());
                }

            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //关闭其他子窗体
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {

                if (this.Name != Application.OpenForms[i].Name)
                {
                    Application.OpenForms[i].Close();
                }

            }
            FrmDisposeNormal(new FrmModifyPasssword());
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetreatSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //退出系统
            Application.Exit();
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
            FrmDisposeMax(new FrmPreference(_commPort));
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
            FrmDisposeMax(new FrmCalibration(_commPort));
        }
        /// <summary>
        /// 硬件检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestHardwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDisposeMax(new FrmTestHardware(_commPort));
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
            FrmDisposeMax(new FrmHelp());
        }


        #endregion

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
        }
    }
}

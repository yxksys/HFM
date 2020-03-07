/**
 * ________________________________________________________________________________ 
 *
 *  描述：硬件检测窗体
 *  作者：杨旭锴
 *  版本：Beta1.1-2020年3月5日
 *  创建时间：2020年2月17日 09:41:19
 *  类名：硬件检测窗体
 *  完成：2020年3月2日 18:25:27
 *  更新：更新2020年3月1日 20:59:07 Beta1.1-2020年3月5日
 *  测试：
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmTestHardware : Form
    {
        #region 基本变量、实例
        //实例化串口
        CommPort commPort = new CommPort();
        /// <summary>
        /// 存储各个通道最终计算检测值的List
        /// </summary>
        private IList<MeasureData> calculatedMeasureDataS = new List<MeasureData>();
        /// <summary>
        /// 测量时间
        /// </summary>
        private int measuringTime;
        /// <summary>
        /// 运行状态枚举
        /// </summary>
        enum HardwarePlatformState
        {
            /// <summary>
            /// 默认状态
            /// </summary>
            Default = 0,
            /// <summary>
            /// Alpha自检
            /// </summary>
            AlphaCheck = 1,
            /// <summary>
            /// Beta自检
            /// </summary>
            BetaCheck = 2,
            /// <summary>
            /// 自检
            /// </summary>
            SelfTest = 3
        }
        
        /// <summary>
        /// 运行状态标志
        /// </summary>
        private HardwarePlatformState platformState;
        /// <summary>
        /// 系统数据库中读取的测量时间
        /// </summary>
        private int sqltime = (new HFM.Components.SystemParameter().GetParameter().MeasuringTime)<=1?10: (new HFM.Components.SystemParameter().GetParameter().MeasuringTime);
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        /// <summary>
        /// 异步线程初始化化时间,ReportProgress百分比数值
        /// </summary>
        private int bkworkTime = 0;
        #endregion
        public FrmTestHardware()
        {
            InitializeComponent();
            //能否报告进度更新。
            bkWorkerReceiveData.WorkerReportsProgress = true;
            //是否支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
        }

        #region 初始化窗体
        private void FrmTestHardware_Load(object sender, EventArgs e)
        {
            #region 中英文转换
            if (isEnglish==true)
            {
                this.Text = "Hardware Checking";//硬件检测
                LblLHP.Text = "LHP";//左手心
                LblLHB.Text = "LHB";//左手背
                LblRHP.Text = "RHP";//右手心
                LblRHB.Text = "RHB";//右手背
                LblLF.Text = "LF";//左脚
                LblRF.Text = "RF";//右脚
                LblHighVoltage.Text = "High Voltage";//探头高压
                Lblαcountrate.Text = "αcount rate";//α计数率
                LblαTotalcnt.Text = "αTotal cnt";//α总计数
                Lblβcountrate.Text = "βcount rate";//β计数率
                LblβTotalcnt.Text = "βTotal cnt";//β总计数
                LblStatus.Text = "Status";//工作状态
                LblV.Text = "V";
                LblAlphacps.Text = "cps";
                LblAlphacnt.Text = "cnt";
                LblBetacps.Text = "cps";
                LblBetacnt.Text = "cnt";
                GrpFrisker.Text = "Frisker";//衣物探头
                LblFriskercount.Text = "Count rate";//计数
                GrpSensorstate.Text = "Sensor state";//红外状态
                LblFriskerState.Text = "Frisker";//衣物
                LblRHandState.Text = "RHand";//右手
                LblLHandState.Text = "LHand";//左手
                GrpDetectorSelfTest.Text = "Detector Self-Test";//探头自检
                BtnAlphaCheck.Text = "α";//α自检
                BtnBetaCheck.Text = "β";//β自检
                GrpSelfTestParameter.Text = "Self-Test Parameter";//自检参数
                BtnSelfCheck.Text = "Run";//自检
                CmbControl.Text = "L";
                CmbPulse.Text = "L";
                LblControl.Text = "Control";//控制
                LblPWidth.Text = "P Width";//脉宽
                LblPulse.Text = "Pulse";//脉冲
                LblSelfcount.Text = "Count rate";//计数
            }
                #endregion
            //初始化运行状态为默认状态
            platformState = HardwarePlatformState.Default;
            //初始化测量时间为系统参数时间
            measuringTime = sqltime;

            #region 开启端口
            //从配置文件获得当前串口配置
            if (commPort.Opened == true)
            {
                commPort.Close();
            }
            commPort.GetCommPortSet();
            //打开串口
            try
            {
                commPort.Open();
            }
            catch
            {
                if (isEnglish == true)
                {
                    MessageBox.Show("Port open error! Please check whether the communication is normal.");
                    //return;
                }
                else
                {
                    MessageBox.Show("端口打开错误！请检查通讯是否正常。");
                    //return;
                }
            }
            #endregion

            //开启异步线程
            bkWorkerReceiveData.RunWorkerAsync();
        }
        #endregion

        #region 异步线程DoWork事件响应
        /// <summary>
        /// 异步线程DoWork事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bkWorkerReceiveData_DoWork(object sender, DoWorkEventArgs e)
        {
            //如果没有取消异步线程
            if (bkWorkerReceiveData.CancellationPending == false)
            {
                //在异步线程上执行串口读操作ReadDataFromSerialPort方法
                BackgroundWorker bkWorker = sender as BackgroundWorker;
                e.Result = ReadDataFromSerialPort(bkWorker, e);
            }

            e.Cancel = true;
        }
        #endregion

        #region 串口计时，周期清理Dgv数据表总计数，ReadDataFromSerialPort中使用
        /// <summary>
        /// 串口计时，周期清理Dgv数据表总计数
        /// </summary>
        private void TimeConutPort()
        {
            //线程计时等于测量时间
            if (bkworkTime == measuringTime)
            {
                //总计数清空
                DgvArrayClear();
                //异步初始化为0
                bkworkTime = 0;

                platformState = HardwarePlatformState.Default;
                measuringTime = sqltime;
            }
            bkworkTime = bkworkTime + 1;
        } 
        #endregion

        #region 通过串口读取下位机上传数据
        /// <summary>
        /// 通过串口读取下位机上传数据
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private byte[] ReadDataFromSerialPort(BackgroundWorker bkworker, DoWorkEventArgs e)
        {
            int errorNumber = 0; //下发自检报文出现错误计数器
            int delayTime = 200;//下发自检报文延时时间
            byte[] receiveBuffMessage = new byte[200];
            
            while (true)
            {
                #region Alpha、beta、自检下发指令
                //请求进程中断读取数据
                if (bkworker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }
                //如果当前运行状态为不是“默认状态
                //则根据不同探测类型向下位机下发相应的自检指令
                byte[] messageDate = null;
                switch (platformState)
                {
                    case HardwarePlatformState.Default:
                        TimeConutPort();
                        break;
                    //生成Alpha、Beta和自检指令报文              
                    case HardwarePlatformState.AlphaCheck:
                        //下发Alpha自检指令
                        messageDate = Components.Message.BuildMessage(0);
                        //如果不成功则：
                        if (Components.Message.SendMessage(messageDate, commPort) != true)
                        {
                            //错误计数器errorNumber+1
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                
                                bkworker.ReportProgress(bkworkTime, null);
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(delayTime);
                                continue;
                            }
                        }
                        TimeConutPort();
                        break;
                    case HardwarePlatformState.BetaCheck:
                        //下发Beta自检指令
                        messageDate = Components.Message.BuildMessage(1);
                        //如果不成功则：
                        if (Components.Message.SendMessage(messageDate, commPort) != true)
                        {
                            //错误计数器errorNumber+1
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                bkworker.ReportProgress(bkworkTime, null);
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(delayTime);
                                continue;
                            }
                        }
                        TimeConutPort();
                        break;
                    case HardwarePlatformState.SelfTest:
                        //自检脉冲临时变量
                        int pulse = 0;
                        //自检控制临时变量
                        int control = 0;
                        //判断控件脉冲当前值
                        if (CmbPulse.Created.ToString() == "L(低)")
                        {
                            pulse = 0;
                        }
                        else if (CmbPulse.Created.ToString() == "H(高)")
                        {
                            pulse = 1;
                        }
                        else
                        {
                            pulse = 0;
                        }
                        //判断控件控制当前值
                        if (CmbControl.Created.ToString() == "L(低)")
                        {
                            control = 0;
                        }
                        else if (CmbControl.Created.ToString() == "H(高)")
                        {
                            control = 1;
                        }
                        else
                        {
                            control = 0;
                        }
                        //下发自检指令
                        messageDate = Components.Message.BuildMessage(Convert.ToInt32(TxtCheckCount.Text), pulse, control, Convert.ToInt32(TxtPWidth.Text));
                        //如果不成功则：
                        if (Components.Message.SendMessage(messageDate, commPort) != true)
                        {
                            //错误计数器errorNumber+1
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                bkworker.ReportProgress(bkworkTime, null);
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(delayTime);
                                continue;
                            }
                        }
                        TimeConutPort();
                        break;
                    default:
                        break;
                }
                //延时毫秒
                Thread.Sleep(500);

                #endregion

                #region 向下位机下发“C”指令码读取数据
                //向下位机下发“C”指令码
                byte[] buffMessage = new byte[62];
                buffMessage[0] = Convert.ToByte('C');
                //判断串口是否打开，打开则用传输数据，否则用模拟数据
                if (commPort.Opened == true)
                {
                    if (HFM.Components.Message.SendMessage(buffMessage, commPort) == true)    //正式
                                                                                              //if (HFM.Components.Message.SendMessage(buffMessage, commPort) != true)      //测试使用
                    {
                        //延时
                        Thread.Sleep(100);
                        receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                        //延时
                        Thread.Sleep(500);
                        //触发向主线程返回下位机上传数据事件
                        bkworker.ReportProgress(bkworkTime, receiveBuffMessage);
                    }
                }
                else
                {
                    if (HFM.Components.Message.SendMessage(buffMessage, commPort) != true)      //测试使用
                    {
                        //延时
                        Thread.Sleep(100);
                        receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                        //延时
                        Thread.Sleep(500);
                        //触发向主线程返回下位机上传数据事件
                        bkworker.ReportProgress(bkworkTime, receiveBuffMessage);
                    }
                }
                #endregion
            }
        }
        #endregion
        
        #region Dgv列表数组
        /// <summary>
        /// 定义dgv高压
        /// </summary>
        string[] _hv = new string[6];
        /// <summary>
        /// alpha计数
        /// </summary>
        string[] _alphacps = new string[6];
        /// <summary>
        /// alpha总计数
        /// </summary>
        string[] _alphacnt = new string[6];
        /// <summary>
        /// Beta计数
        /// </summary>
        string[] _betacps = new string[6];
        /// <summary>
        /// Beta总计数
        /// </summary>
        string[] _betacnt = new string[6];
        /// <summary>
        /// 通道状态
        /// </summary>
        string[] _strat = new string[6];
        /// <summary>
        /// 衣物计数
        /// </summary>
        string frisker = "";
        /// <summary>
        /// 红外状态
        /// </summary>
        private int[] _infraredStatus=new int[7];
        /// <summary>
        /// Dgv列表总计数清零
        /// </summary>
        private void DgvArrayClear()
        {
            Array.Clear(_betacnt,0,6);
            Array.Clear(_alphacnt,0,6);
        }
        #endregion
        
        #region 异步线程读取串口数据后的ReportProgress事件响应
        //异步线程读取串口数据后的ReportProgress事件响应
        private void bkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage=null; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象            
            //判断串口是否打开，打开则用传输数据，否则用模拟数据
            if (commPort.Opened == true)
            {
                //对事件参数类中的数据对象序列化为byte[]
                if (e.UserState is byte[])
                {
                    receiveBufferMessage = (byte[])e.UserState;
                }
                //接收报文数据为空
                if (receiveBufferMessage.Length < messageBufferLength)
                {
                    //数据接收出现错误次数超限
                    if (errNumber >= 2)
                    {
                        if (isEnglish == true)
                        {
                            MessageBox.Show("Communication error! Please check whether the communication is normal.");
                            return;
                        }
                        else
                        {
                            MessageBox.Show("通讯错误！请检查通讯是否正常。");
                            return;
                        }
                        
                    }
                    else
                    {
                        errNumber++;
                    }

                    return;
                }

                //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
                measureDataS = Components.Message.ExplainMessage<MeasureData>(receiveBufferMessage);
            }
            
            #region 从监测存储到的measureDataS数据中解析到界面数值
            //临时变量
            int i = 0;
            //从列表中取出数据
            foreach (var item in measureDataS)
            {
                //通过运行状态判断衣物探头是alpha还是beta，默认alpha通道计数
                if (i == 6)
                {
                    _infraredStatus[i] = item.InfraredStatus;
                    if (platformState == HardwarePlatformState.AlphaCheck)
                    {
                        frisker = Convert.ToString(item.Alpha);
                    }
                    else if (platformState == HardwarePlatformState.BetaCheck)
                    {
                        frisker = Convert.ToString(item.Beta);
                    }
                    else
                    {
                        frisker = Convert.ToString(item.Alpha);
                    }
                    break;
                }
                _hv[i] = Convert.ToString(item.HV);
                _alphacps[i] = Convert.ToString(item.Alpha);
                _betacps[i] = Convert.ToString(item.Beta);
                _infraredStatus[i] = item.InfraredStatus;
                i++;
            }
            //赋值alpha和Beta总计数并且判断赋值通道状态
            for (i = 0; i < 6; i++)
            {
                //alpha总计数
                _alphacnt[i] = Convert.ToString(Convert.ToInt32(_alphacnt[i]) + Convert.ToInt32(_alphacps[i]));
                //beta总计数
                _betacnt[i] = Convert.ToString(Convert.ToInt32(_betacnt[i]) + Convert.ToInt32(_betacps[i]));
                //判断通道状态
                if (Convert.ToInt32(_hv[i]) == 0 && (Convert.ToInt32(_alphacnt[i]) == 0 || Convert.ToInt32(_betacnt[i]) == 0))
                {
                    if (isEnglish == true)
                    {
                        _strat[i] = "COM fault";
                    }
                    else
                    {
                        _strat[i] = "通讯故障";
                    }
                }
                else if (Convert.ToInt32(_alphacnt[i]) == 0 && Convert.ToInt32(_betacnt[i]) == 0)
                {
                    if (isEnglish == true)
                    {
                        _strat[i] = "Probe fault";
                    }
                    else
                    {
                        _strat[i] = "探头故障";
                    }
                }
                else
                {
                    if (isEnglish == true)
                    {
                        _strat[i] = "OK";
                    }
                    else
                    {
                        _strat[i] = "正常工作";
                    }
                }
            }

            DgvWork.Rows.Clear();
            DgvWork.Rows.Insert(0, _hv);
            DgvWork.Rows.Insert(1, _alphacps);
            DgvWork.Rows.Insert(2, _alphacnt);
            DgvWork.Rows.Insert(3, _betacps);
            DgvWork.Rows.Insert(4, _betacnt);
            DgvWork.Rows.Insert(5, _strat);
            TxtFriskercount.Text = frisker;

            int time = this.measuringTime - e.ProgressPercentage;
            if (isEnglish == true)
            {
                LblTimeWork.Text = "Countdown " + time + " s";
            }
            else
            {
                LblTimeWork.Text = "测量剩余时间 " + time + " 秒";
            }
            //判断右手红外状态界面显示颜色
            if (_infraredStatus[0] == 1 && _infraredStatus[1] == 1)
            {
                TxtLHandState.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
                TxtLHandState.BackColor = System.Drawing.Color.Orange;
            }
            //判断右手红外状态界面显示颜色
            if (_infraredStatus[2] == 1 && _infraredStatus[3] == 1)
            {
                TxtRHandState.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
                TxtRHandState.BackColor = System.Drawing.Color.Orange;
            }
            //
            //判断右手红外状态界面显示颜色
            if (_infraredStatus[6] == 1)
            {
                TxtFriskerState.BackColor = System.Drawing.Color.Lime;
            }
            else
            {
                TxtFriskerState.BackColor = System.Drawing.Color.Orange;
            }
            #endregion
        }
        #endregion

        #region 按钮通用方法
        /// <summary>
        /// 按钮通用方法
        /// </summary>
        /// <param name="platform">自检运行状态</param>
        private void BtnCurency(HardwarePlatformState platform)
        {
            platformState = platform;
            measuringTime = sqltime;
            DgvArrayClear();
            bkworkTime = 0;
        }
        #endregion

        #region 按钮
        /// <summary>
        /// alpha自检按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAlphaCheck_Click(object sender, EventArgs e)
        {
            BtnCurency(HardwarePlatformState.AlphaCheck);
        }
        /// <summary>
        /// Beta自检按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBetaCheck_Click(object sender, EventArgs e)
        {
            BtnCurency(HardwarePlatformState.BetaCheck);
        }
        /// <summary>
        /// 自检按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelfCheck_Click(object sender, EventArgs e)
        {
            BtnCurency(HardwarePlatformState.SelfTest);
        } 
        #endregion
    }
}

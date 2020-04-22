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
 *          更新:2020年3月18日内容,红外判断
 *  测试：测试时间2020年3月15日
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmTestHardware : Form
    {
        #region 基本变量、实例
        IList<Channel> channelS = new List<Channel>();//当前可使用的检测通道,即全部启用的监测通道
        //实例化串口
        private CommPort _commPort = new CommPort();
        /// <summary>
        /// 存储各个通道最终计算检测值的List
        /// </summary>
        private IList<MeasureData> _calculatedMeasureDataS = new List<MeasureData>();
        /// <summary>
        /// 测量时间
        /// </summary>
        private int _measuringTime;
        /// <summary>
        /// 运行状态枚举
        /// </summary>
        private enum HardwarePlatformState
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
        private HardwarePlatformState _platformState;
        /// <summary>
        /// 系统数据库中读取的测量时间
        /// </summary>
        private int _sqltime = (new Components.SystemParameter().GetParameter().MeasuringTime) <= 1 ? 10 : (new Components.SystemParameter().GetParameter().MeasuringTime);
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool _isEnglish = (new Components.SystemParameter().GetParameter().IsEnglish);
        /// <summary>
        /// 异步线程初始化化时间,ReportProgress百分比数值
        /// </summary>
        private int _bkworkTime = 0;
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
            if (_isEnglish)
            {
                Text = "Hardware Checking";//硬件检测
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
            _platformState = HardwarePlatformState.Default;
            //初始化测量时间为系统参数时间
            _measuringTime = _sqltime + 1;

            #region 开启端口
            //从配置文件获得当前串口配置
            if (_commPort.Opened == true)
            {
                _commPort.Close();
            }

            _commPort.GetCommPortSet("commportSet");
            //打开串口
            try
            {
                _commPort.Open();
                if (_commPort.Opened)
                {
                    Tools.FormBottomPortStatus = true;
                }
                else
                {
                    Tools.FormBottomPortStatus = false;
                }
            }
            catch
            {
                if (_isEnglish == true)
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
            //获得通道信息
            Channel channel = new Channel();
            channelS = channel.GetChannel();            
            if (bkWorkerReceiveData.IsBusy == false)
            {
                //开启异步线程
                bkWorkerReceiveData.RunWorkerAsync();
            }

        }
        #endregion

        #region 异步线程DoWork事件响应
        /// <summary>
        /// 异步线程DoWork事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BkWorkerReceiveData_DoWork(object sender, DoWorkEventArgs e)
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
            if (_bkworkTime == _measuringTime)
            {
                //总计数清空
                DgvArrayClear();
                //异步初始化为0
                _bkworkTime = 0;


                _measuringTime = _sqltime + 1;
            }
            _platformState = HardwarePlatformState.Default;
            _bkworkTime = _bkworkTime + 1;
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
            byte[] receiveBuffMessage = new byte[124];

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
                switch (_platformState)
                {
                    case HardwarePlatformState.Default:
                        TimeConutPort();
                        break;
                    //生成Alpha、Beta和自检指令报文              
                    case HardwarePlatformState.AlphaCheck:
                        //下发Alpha自检指令
                        messageDate = Components.Message.BuildMessage(0);
                        //如果不成功则：
                        if (Components.Message.SendMessage(messageDate, _commPort) != true)
                        {
                            //错误计数器errorNumber+1
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {

                                bkworker.ReportProgress(_bkworkTime, null);
                            }
                            else
                            {
                                Thread.Sleep(delayTime);
                                continue;
                            }
                        }
                        Thread.Sleep(2000);
                        DgvArrayClear();
                        TimeConutPort();
                        break;
                    case HardwarePlatformState.BetaCheck:
                        //下发Beta自检指令
                        messageDate = Components.Message.BuildMessage(1);
                        //如果不成功则：
                        if (Components.Message.SendMessage(messageDate, _commPort) != true)
                        {
                            //错误计数器errorNumber+1
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                bkworker.ReportProgress(_bkworkTime, null);
                            }
                            else
                            {
                                Thread.Sleep(delayTime);
                                continue;
                            }
                        }
                        Thread.Sleep(2000);
                        DgvArrayClear();
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
                        if (Components.Message.SendMessage(messageDate, _commPort) != true)
                        {
                            //错误计数器errorNumber+1
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                bkworker.ReportProgress(_bkworkTime, null);
                            }
                            else
                            {
                                Thread.Sleep(delayTime);
                                continue;
                            }
                        }
                        Thread.Sleep(2000);
                        DgvArrayClear();
                        TimeConutPort();
                        break;
                    default:
                        break;
                }
                //延时毫秒
                //Thread.Sleep(400);

                #endregion

                #region 向下位机下发“C”指令码读取数据
                //向下位机下发“C”指令码
                byte[] buffMessage = new byte[62];
                buffMessage[0] = Convert.ToByte('C');
                buffMessage[61] = Convert.ToByte(16);
                //判断串口是否打开，打开则用传输数据，否则用模拟数据
                if (_commPort.Opened == true)
                {
                    if (Components.Message.SendMessage(buffMessage, _commPort) == true)    //正式
                                                                                           //if (HFM.Components.Message.SendMessage(buffMessage, commPort) != true)      //测试使用
                    {
                        //延时
                        Thread.Sleep(200);
                        receiveBuffMessage = Components.Message.ReceiveMessage(_commPort);
                        //延时
                        Thread.Sleep(800);
                        //触发向主线程返回下位机上传数据事件
                        bkworker.ReportProgress(_bkworkTime, receiveBuffMessage);
                    }
                }
                else
                {
                    errorNumber++;
                    //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                    if (errorNumber > 5)
                    {
                        bkworker.ReportProgress(1, null);
                        bkWorkerReceiveData.CancelAsync();
                    }
                    else
                    {
                        Thread.Sleep(delayTime);
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
        private string[] _hv = new string[6];
        /// <summary>
        /// alpha计数
        /// </summary>
        private string[] _alphacps = new string[6];
        /// <summary>
        /// alpha总计数
        /// </summary>
        private string[] _alphacnt = new string[6];
        /// <summary>
        /// Beta计数
        /// </summary>
        private string[] _betacps = new string[6];
        /// <summary>
        /// Beta总计数
        /// </summary>
        private string[] _betacnt = new string[6];
        /// <summary>
        /// 通道状态
        /// </summary>
        private string[] _strat = new string[6];
        /// <summary>
        /// 衣物计数
        /// </summary>
        private string _frisker = "";
        /// <summary>
        /// 红外状态
        /// </summary>
        private int[] _infraredStatus = new int[7];
        /// <summary>
        /// Dgv列表总计数清零
        /// </summary>
        private void DgvArrayClear()
        {
            Array.Clear(_alphacps, 0, 6);
            Array.Clear(_betacps, 0, 6);
            Array.Clear(_betacnt, 0, 6);
            Array.Clear(_alphacnt, 0, 6);
        }
        #endregion

        #region 异步线程读取串口数据后的ReportProgress事件响应
        //异步线程读取串口数据后的ReportProgress事件响应
        private void BkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象            
            //判断串口是否打开，打开则用传输数据，否则用模拟数据
            if (_commPort.Opened)
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
                        if (_isEnglish)
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
                    if (_platformState == HardwarePlatformState.AlphaCheck)
                    {
                        _frisker = Convert.ToString(item.Beta);
                    }
                    else if (_platformState == HardwarePlatformState.BetaCheck)
                    {
                        _frisker = Convert.ToString(item.Beta);
                    }
                    else
                    {
                        _frisker = Convert.ToString(item.Beta);
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
                if(channelS[i].IsEnabled==false)
                {
                    if (_isEnglish)
                    {

                        _strat[i] = "Not enabled";
                    }
                    else
                    {
                        _strat[i] = "未启用";
                    }
                    continue;
                }
                //alpha总计数
                _alphacnt[i] = Convert.ToString(Convert.ToInt32(_alphacnt[i]) + Convert.ToInt32(_alphacps[i]));
                //beta总计数
                _betacnt[i] = Convert.ToString(Convert.ToInt32(_betacnt[i]) + Convert.ToInt32(_betacps[i]));
                //判断通道状态
                if (new Channel().GetChannel(i + 1).IsEnabled == false)
                {
                    DgvWork.Columns[i].DefaultCellStyle.ForeColor = Color.AntiqueWhite; //前景颜色改变
                    DgvWork.Columns[i].DefaultCellStyle.BackColor = Color.DarkGray;     //背景颜色改变
                    _hv[i] = "";            //未启用的通道信息清空
                    _alphacps[i] = "";      //未启用的通道信息清空
                    _alphacnt[i] = "";      //未启用的通道信息清空
                    _betacps[i] = "";       //未启用的通道信息清空
                    _betacnt[i] = "";       //未启用的通道信息清空

                    if (_isEnglish)
                    {
                        
                        _strat[i] = "Not enabled";
                    }
                    else
                    {
                        _strat[i] = "未启用";
                    }
                }
                else if (Convert.ToInt32(_hv[i]) == 0 && (Convert.ToInt32(_alphacnt[i]) == 0 || Convert.ToInt32(_betacnt[i]) == 0))
                {
                    if (_isEnglish)
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
                    if (_isEnglish)
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
                    if (_isEnglish)
                    {
                        _strat[i] = "OK";
                    }
                    else
                    {
                        _strat[i] = "正常工作";
                    }
                }

            }

            try
            {
                DgvWork.Rows.Clear();
                DgvWork.Rows.Insert(0, _hv);
                DgvWork.Rows.Insert(1, _alphacps);
                DgvWork.Rows.Insert(2, _alphacnt);
                DgvWork.Rows.Insert(3, _betacps);
                DgvWork.Rows.Insert(4, _betacnt);
                DgvWork.Rows.Insert(5, _strat);
                TxtFriskercount.Text = _frisker;
            }
            catch (Exception exception)
            {
                bkWorkerReceiveData.CancelAsync();
                Tools.ErrorLog(exception.ToString());
                //throw;
            }
            

            int time = _measuringTime - e.ProgressPercentage;
            if (_isEnglish)
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
                TxtLHandState.BackColor = Color.Lime;
            }
            else
            {
                TxtLHandState.BackColor = Color.Orange;
            }
            //判断右手红外状态界面显示颜色
            if (_infraredStatus[2] == 1 || _infraredStatus[3] == 1)
            {
                TxtRHandState.BackColor = Color.Lime;
            }
            else
            {
                TxtRHandState.BackColor = Color.Orange;
            }
            //
            //判断右手红外状态界面显示颜色
            if (_infraredStatus[6] == 1)
            {
                TxtFriskerState.BackColor = Color.Lime;
            }
            else
            {
                TxtFriskerState.BackColor = Color.Orange;
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
            _platformState = platform;
            _measuringTime = _sqltime + 1;
            _bkworkTime = 0;
            DgvArrayClear();
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

        /// <summary>
        /// 窗口关闭后,关闭线程,关闭端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmTestHardware_FormClosed(object sender, FormClosedEventArgs e)
        {
            _commPort.Close();
            Thread.Sleep(1000);
            bkWorkerReceiveData.CancelAsync();
        }

        private void FrmTestHardware_FormClosing(object sender, FormClosingEventArgs e)
        {
            bkWorkerReceiveData.CancelAsync();
        }

        #region 数字键盘显示
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtCheckCount_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtCheckCount);
        }
        /// <summary>
        /// 脉宽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPWidth_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtPWidth);
        } 
        #endregion
    }
}

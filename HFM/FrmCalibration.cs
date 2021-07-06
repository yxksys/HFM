﻿/**
 * ________________________________________________________________________________ 
 *
 *  描述：刻度窗体
 *  作者：杨旭锴
 *  版本：v1.0
 *  创建时间：2020年2月25日 16:58:28
 *  类名：刻度窗体
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using HFM.Components;
using Message = HFM.Components.Message;

namespace HFM
{
    public partial class FrmCalibration : Form
    {
        #region 字段、方法、实例

        #region 字段、数组
        /// <summary>
        /// 传入值
        /// </summary>
        private string _value = "";
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool _isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        /// <summary>
        /// 通道中文名称数组
        /// </summary>
        private string[] _channelName = new string[7];
        /// <summary>
        /// 通道英文名称数组
        /// </summary>
        private string[] _channelNameEnglish = new string[7];
        /// <summary>
        /// 通用循环变量初始为0
        /// </summary>
        private int _numForaech;
        /// <summary>
        /// 当前发送报文的类型
        /// </summary>
        private MessageType _messageType;
        /// <summary>
        /// 测量时间
        /// </summary>
        private int _measuringTime;
        /// <summary>
        /// 测量次数
        /// </summary>
        private int _measuringCount;
        /// <summary>
        /// 异步线程初始化化时间,ReportProgress百分比数值
        /// </summary>
        private int _bkworkTime;
        /// <summary>
        /// 刻度测量状态:false=本地测量;true=带源测量;
        /// </summary>
        private bool _sclaeState;
        /// <summary>
        /// 进度条时间
        /// </summary>
        private int _prgTime;
        #endregion

        #region 实例
        /// <summary>
        /// 串口实例
        /// </summary>
        private CommPort _commPort = null;
        /// <summary>
        /// 获取所有“通道参数”
        /// </summary>
        private List<Channel> _channelList = new Channel().GetChannel(true).ToList();
        /// <summary>
        /// 获取所有“效率参数”(核素)
        /// </summary>
        private List<EfficiencyParameter> _efficiencyList = new EfficiencyParameter().GetParameter().ToList();
        /// <summary>
        /// 工具类实例-错误提示信息
        /// </summary>
        private Tools _tools =new Tools();
        /// <summary>
        /// 当前通道道盒参数数据对象
        /// </summary>
        private ChannelParameter _setChannelParameter = new ChannelParameter();
        /// <summary>
        /// 当前通道对象数据
        /// </summary>
        private Channel _channel = new Channel();
        /// <summary>
        /// 当前核素对象数据
        /// </summary>
        private EfficiencyParameter _changedEfficiency=new EfficiencyParameter();
        /// <summary>
        /// 解析的道盒列表参数
        /// </summary>
        private IList<ChannelParameter> _channelParameters = new List<ChannelParameter>();

        
        #endregion

        #region 方法
        /// <summary>
        /// 开启串口封装的方法
        /// </summary>
        //private void OpenPort()
        //{
        //    //从配置文件获得当前串口配置
        //    if (_commPort.Opened )
        //    {
        //        _commPort.Close();
        //    }
        //    _commPort.GetCommPortSet("commportSet");
        //    //打开串口
        //    try
        //    {
        //        _commPort.Open();
        //        if (_commPort.Opened)
        //        {
        //            Tools.FormBottomPortStatus = true;
        //        }
        //        else
        //        {
        //            Tools.FormBottomPortStatus = false;
        //        }
        //    }
        //    catch
        //    {
        //        _tools.PrompMessage(1);
                
        //    }
        //}
        /// <summary>
        /// 发送消息类型：
        /// </summary>
        private enum MessageType
        {
            /// <summary>
            /// C读取类型
            /// </summary>
            CRead,
            /// <summary>
            /// P写入类型
            /// </summary>
            PSet,
            /// <summary>
            /// P读取类型
            /// </summary>
            PRead,
        }
        
        #endregion

        #endregion

        #region 初始化加载
        public FrmCalibration()
        {
            InitializeComponent();
            bkWorkerReceiveData.WorkerReportsProgress = true;
            
        }
        public FrmCalibration(CommPort commPort)
        {
            this._commPort = commPort;
            InitializeComponent();
            bkWorkerReceiveData.WorkerReportsProgress = true;

        }
        private void FrmCalibration_Load(object sender, EventArgs e)
        {
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;

            #region 中英文翻译

            if (_isEnglish)
            {
                Text = @"Calibration";//测试刻度
                TabCalibration.TabPages[0].Text = @"Calibration"; //仪器刻度
                LblChannelSelection.Text = @"Channel selection";//通道选择
                LblHV.Text = @"HV(V)";//高压(V)
                LblThreshold.Text = @"Threshold(mV)";//阈值(mV)
                BtnSet.Text = @"Set";//设置
                BtnCalibrate.Text = @"Calibrate";//刻度
                LblMeasuringTime.Text = @"Measuring time(s)";//测量时间(s)
                LblCount.Text = @"Times";//次数
                LblNuclide.Text = @"Nuclide";//核素
                LblSFR.Text = @"SFR";//表面发射率
                LblResult.Text = @"Result";//测量完毕
                DgvInformation.Columns[0].HeaderText = @"Status"; //状态
                DgvInformation.Columns[1].HeaderText = @"Channel"; //通道
                DgvInformation.Columns[2].HeaderText = @"Area"; //面积
                DgvInformation.Columns[3].HeaderText = @"αCounts(cps)"; //α计数
                DgvInformation.Columns[4].HeaderText = @"βCounts(cps)"; //β计数
                DgvInformation.Columns[5].HeaderText = @"HV"; //高压
            }

            #endregion

            #region 获得全部启用的通道添加到下拉列表中，更具系统中英文状态选择中英文
            FactoryParameter factoryParameter = new FactoryParameter().GetParameter();
            //根据系统语言填充通道下拉列表
            if (_isEnglish)
            {
                //英文通道名称
                foreach (var listChannel in _channelList)
                {
                    if (factoryParameter.IsDoubleProbe==false && (listChannel.ChannelID==2||listChannel.ChannelID==4))
                    {
                        _numForaech++;
                        continue;
                    }
                    CmbChannelSelection.Items.Add(listChannel.ChannelName_English);
                    _channelName[_numForaech] = listChannel.ChannelName_English;
                    _numForaech++;
                }
            }
            else
            {
                //中文通道名称
                foreach (var listChannel in _channelList)
                {
                    if (factoryParameter.IsDoubleProbe == false && (listChannel.ChannelID == 2 || listChannel.ChannelID == 4))
                    {
                        _numForaech++;
                        continue;
                    }
                    CmbChannelSelection.Items.Add(listChannel.ChannelName);
                    _channelNameEnglish[_numForaech] = listChannel.ChannelName;
                    _numForaech++;
                }
            }
            #endregion
            //OpenPort();
        }
        #endregion

        #region 界面下拉列表触发事件
        #region 通道下拉列表
        /// <summary>
        /// 通道下拉列表选择后（触发事件）
        /// </summary>
        private void CmbChannelSelection_SelectedValueChanged(object sender, EventArgs e)
        {
            //如果选择衣物探头后，高压和阈值变为不可用状态
            if (CmbChannelSelection.Text == "衣物探头" || CmbChannelSelection.Text== "Frisker") //_channelNameEnglish[6] || CmbChannelSelection.Text == _channelName[6])
            {
                TxtHV.Enabled = false;
                Txtα.Enabled = false;
                Txtβ.Enabled = false;
            }
            else
            {
                TxtHV.Enabled = true;
                Txtα.Enabled = true;
                Txtβ.Enabled = true;

                //
                int errorNumber = 0; //下发自检报文出现错误计数器
                int errorNumber2 = 0; //下发自检报文返回数据出现错误计数器
                int downcount = 0;
                byte[] receiveBuffMessage = null;//接受的报文
                byte[] buffMessage = new byte[62];//报文长度

                BtnCalibrate.Enabled = false;
                BtnSet.Enabled = false;
                while (downcount < 4)
                {
                    downcount++;
                    if (downcount==4)
                    {
                        BtnCalibrate.Enabled = true;
                        BtnSet.Enabled = true;
                    }
                    //向下位机下发“p”指令码
                    buffMessage[0] = Convert.ToByte('P');
                    if (Message.SendMessage(buffMessage, _commPort))    //正式
                    {
                        Thread.Sleep(10);
                        receiveBuffMessage = Message.ReceiveMessage(_commPort);
                        //解析P数据报文
                        
                        if (receiveBuffMessage.Length > 0 && receiveBuffMessage[0].ToString() != "80")//由于下位机一直上传C指令，下发P指令后有可能读回的数据还是C指令，所以将其扔掉直到读回的是P指令为止                            
                        {
                            errorNumber2++;
                            if (errorNumber2>3)
                            {
                                _tools.PrompMessage(17);
                                return;
                            }
                            continue;
                        }
                        else if (receiveBuffMessage.Length==0)
                        {
                            errorNumber++;
                            if (errorNumber > 3)
                            {
                                BtnCalibrate.Enabled = true;
                                BtnSet.Enabled = true;
                                _tools.PrompMessage(3);
                                return;
                            }
                            continue;
                        }
                        if (receiveBuffMessage[0] == Convert.ToByte('P'))
                        {
                            _channelParameters = Message.ExplainMessage<ChannelParameter>(receiveBuffMessage);//解析报文
                            try
                            {
                                foreach (var itemParameter in _channelParameters)
                                {
                                    if (CmbChannelSelection.Text == itemParameter.Channel.ChannelName_English || CmbChannelSelection.Text == itemParameter.Channel.ChannelName)
                                    {
                                        /*
                                         * 高压阈值赋值
                                         */
                                        TxtHV.Text = itemParameter.PresetHV.ToString();
                                        Txtα.Text = itemParameter.AlphaThreshold.ToString();
                                        Txtβ.Text = itemParameter.BetaThreshold.ToString();
                                        //当前通道道盒参数
                                        _setChannelParameter = itemParameter;

                                    }
                                }
                            }
                            catch (Exception)
                            {
                                errorNumber++;
                                if (errorNumber > 3)
                                {
                                    BtnCalibrate.Enabled = true;
                                    BtnSet.Enabled = true;
                                    _tools.PrompMessage(17);
                                    return;
                                }
                                continue;
                            }
                            
                        }
                    }
                    else
                    {
                        errorNumber++;
                        if (errorNumber > 4)
                        {
                            BtnCalibrate.Enabled = true;
                            BtnSet.Enabled = true;
                            _tools.PrompMessage(3);
                            return;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    //延时
                    Thread.Sleep(200);
                    
                }
                
            }
            

            var lisChanneList = _channelList.Where(n => n.ChannelName.ToString() == CmbChannelSelection.Text|| n.ChannelName_English.ToString() == CmbChannelSelection.Text).ToList();
            foreach (var b in lisChanneList)
            {
                _channel = b;
            }
            
            
            
        }
        #endregion

        #region 核素下拉列表
        private void CmbNuclideSelect_DropDown(object sender, EventArgs e)
        {
            //核素列表清空
            CmbNuclideSelect.Items.Clear();
            //通道选择为空是提示
            if (CmbChannelSelection.Text == "")
            {
                _tools.PrompMessage(10);
            }
            //根据选择的通道进行选择核素
            if (CmbNuclideSelect.Items.Count == 0)
            {
                //按通道id查询核素类型列表
                var listEfficiency = _efficiencyList.Where(n =>
                    n.Channel.ChannelName_English.ToString() == CmbChannelSelection.Text ||
                    n.Channel.ChannelName.ToString() == CmbChannelSelection.Text).ToList();
                foreach (var item in listEfficiency)
                {
                    CmbNuclideSelect.Items.Add(item.NuclideName);
                }
            }
        }
        #endregion

        #endregion

        #region 异步线程

        #region DoWork
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

        #region e.Result = ReadDataFromSerialPort(bkWorker, e);
        private object ReadDataFromSerialPort(BackgroundWorker bkWorker, DoWorkEventArgs e)
        {
            int errorNumber = 0; //下发自检报文出现错误计数器
            int delayTime = 200;//下发自检报文延时时间
            byte[] receiveBuffMessage = null;//接受的报文
            byte[] buffMessage = new byte[62];//报文长度
            while (true)
            {
                //请求进程中断读取数据
                if (bkWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }
                #region C读取指令下发并接收数据上传

                //向下位机下发“C”指令码
                buffMessage[0] = Convert.ToByte('C');
                buffMessage[61] = Convert.ToByte(16);
                if (Message.SendMessage(buffMessage, _commPort))    //正式
                {
                    //延时
                    Thread.Sleep(200);
                    receiveBuffMessage = Message.ReceiveMessage(_commPort);

                    //延时
                    Thread.Sleep(800);
                    //触发向主线程返回下位机上传数据事件
                    bkWorker.ReportProgress(1, receiveBuffMessage);
                }
                else
                {
                    errorNumber++;
                    //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                    if (errorNumber > 5)
                    {
                        bkWorker.ReportProgress(1, null);
                        bkWorkerReceiveData.CancelAsync();
                    }
                    else
                    {
                        Thread.Sleep(delayTime);
                    }
                }
                
                #endregion
                #region switch p/c指令选择弃用代码

                //switch (_messageType)
                //{
                //    #region P读取指令下发并接收数据上传
                //    case MessageType.PRead:

                //        //向下位机下发“p”指令码
                //        buffMessage[0] = Convert.ToByte('P');
                //        //buffMessage[61] = Convert.ToByte(0);
                //        _bkworkTime++;
                //        if (_bkworkTime > 4)
                //        {
                //            bkWorkerReceiveData.CancelAsync();
                //            _bkworkTime = 0;
                //            //按钮可以使用
                //            BtnCalibrate.Enabled = true;
                //            BtnSet.Enabled = true;
                //            break;
                //        }
                //        if (Message.SendMessage(buffMessage, _commPort))    //正式
                //        {

                //            //延时
                //            Thread.Sleep(100);
                //            receiveBuffMessage = Message.ReceiveMessage(_commPort);  
                //            if(receiveBuffMessage.Length>0&&receiveBuffMessage[0].ToString()!="80")//由于下位机一直上传C指令，下发P指令后有可能读回的数据还是C指令，所以将其扔掉直到读回的是P指令为止                            
                //            {
                //                continue;
                //            }
                //            //延时
                //            Thread.Sleep(200);
                //            //触发向主线程返回下位机上传数据事件
                //            bkWorker.ReportProgress(_bkworkTime, receiveBuffMessage);
                //        }
                //        else
                //        {
                //            errorNumber++;
                //            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                //            if (errorNumber > 5)
                //            {
                //                bkWorker.ReportProgress(1, null);
                //                bkWorkerReceiveData.CancelAsync();
                //                BtnCalibrate.Enabled = true;
                //                BtnSet.Enabled = true;
                //            }
                //            else
                //            {
                //                Thread.Sleep(delayTime);
                //            }
                //        }
                //        break;
                //    #endregion

                //    #region P写入指令下发
                //    case MessageType.PSet:
                //        IList<ChannelParameter> _first_setChannelP = new List<ChannelParameter>();
                //        IList<ChannelParameter> _second_setChanelP = new List<ChannelParameter>();
                //        int i = 0;
                //        //把当前的高压阈值修改的数据对象添加到列表中
                //        foreach (var itme in _channelParameters)
                //        {
                //            if (i < 4)
                //            {
                //                _first_setChannelP.Add(itme);
                //            }
                //            else
                //            {
                //                _second_setChanelP.Add(itme);
                //            }
                //            i++;
                //        }
                //        _channelParameters.Clear();
                //        _second_setChanelP.RemoveAt(3);
                //        _second_setChanelP.Add(_setChannelParameter);
                //        //生成报文
                //        buffMessage = Message.BuildMessage(_first_setChannelP);
                //        Thread.Sleep(20);
                //        buffMessage = Message.BuildMessage(_second_setChanelP);
                //        Thread.Sleep(10);
                //        //成功则关闭线程
                //        try
                //        {
                //            if (Message.SendMessage(buffMessage, _commPort))
                //            {
                //                //写入成功,返回p指令读取当前高压以确认更改成功
                //                //bkWorkerReceiveData.CancelAsync();
                //                if (_isEnglish)
                //                {
                //                    MessageBox.Show("Data has been distributed!", "Message");
                //                }
                //                else
                //                {
                //                    MessageBox.Show("数据已经下发!", "提示");
                //                }
                //                _messageType = MessageType.PRead;
                //            }
                //            //发送失败次数大于5次,提示错误并挂起线程
                //            else
                //            {
                //                errorNumber++;
                //                if (errorNumber > 5)
                //                {
                //                    _tools.PrompMessage(2);
                //                    bkWorkerReceiveData.CancelAsync();
                //                }
                //                Thread.Sleep(200);

                //            }
                //        }
                //        catch
                //        {
                //            MessageBox.Show("设置失败,请重新尝试!");
                //        }
                //        break;
                //    #endregion

                //    #region C读取指令下发并接收数据上传
                //    case MessageType.CRead:
                //        //向下位机下发“C”指令码
                //        buffMessage[0] = Convert.ToByte('C');
                //        buffMessage[61] = Convert.ToByte(16);
                //        if (Message.SendMessage(buffMessage, _commPort) )    //正式
                //        {
                //            //延时
                //            Thread.Sleep(200);
                //            receiveBuffMessage = Message.ReceiveMessage(_commPort);

                //            //延时
                //            Thread.Sleep(800);
                //            //触发向主线程返回下位机上传数据事件
                //            bkWorker.ReportProgress(1, receiveBuffMessage);
                //        }
                //        else
                //        {
                //            errorNumber++;
                //            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                //            if (errorNumber > 5)
                //            {
                //                bkWorker.ReportProgress(1, null);
                //                bkWorkerReceiveData.CancelAsync();
                //            }
                //            else
                //            {
                //                Thread.Sleep(delayTime);
                //            }
                //        }

                //        break; 

                //        #endregion
                //} 
                #endregion
            }
        }
        #endregion

        #region 刻度需要的字段

        /// <summary>
        /// 高压
        /// </summary>
        private float _hv;
        /// <summary>
        /// alpha单次时间计数
        /// </summary>
        private float _alphacps;
        /// <summary>
        /// beta单次时间计数
        /// </summary>
        private float _betacps;
        /// <summary>
        /// alpha总计数
        /// </summary>
        private float _alphacnt;
        /// <summary>
        /// Beta总计数
        /// </summary>
        private float _betacnt;
        /// <summary>
        /// alpha本地平均数
        /// </summary>
        private float _alphaNb;
        /// <summary>
        /// Beta本地平均数
        /// </summary>
        private float _betaNb;
        /// <summary>
        /// alpha带源平均数
        /// </summary>
        private float _alphaNr;
        /// <summary>
        /// beite带源平均数
        /// </summary>
        private float _betaNr;
        /// <summary>
        /// Alpah效率
        /// </summary>
        private float _effAlpha;
        /// <summary>
        /// Beta效率
        /// </summary>
        private float _effBeta;
        /// <summary>
        /// 效率
        /// </summary>
        private float _eff;
        /// <summary>
        /// 结果探测下限
        /// </summary>
        private float _resultMda;
        /// <summary>
        /// Alpha探测下限
        /// </summary>
        private float _alphaMda;
        /// <summary>
        /// Beta探测下限
        /// </summary>
        private float _betaMda;
        /// <summary>
        /// 为5%误报率所要求的标准差数
        /// </summary>
        private float _p = 0.05f;
        /// <summary>
        /// 单位时间内单次平均数组
        /// </summary>
        private string[] _addInformation = new string[6];
        /// <summary>
        /// 探测器是否合格
        /// </summary>
        private string _isStandardize;
        /// <summary>
        /// 串口多余数据计数次数
        /// </summary>
        private int throwDataCount = 0;
        #endregion

        #region ProgressChanged
        private void BkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //如果窗体已经被释放,返回
            if (this.IsDisposed==true)
            {
                return;
            }
            float area = _channel.ProbeArea;//探测面积
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象                        
            if (e.UserState is byte[])
            {
                receiveBufferMessage = (byte[])e.UserState;
            }
            //接收报文数据为空
            try
            {
                if (receiveBufferMessage.Length < messageBufferLength)
                {
                    errNumber++;
                    //数据接收出现错误次数超限
                    if (errNumber >= 2)
                    {
                        if (_isEnglish == true)
                        {
                            MessageBox.Show(@"Communication error! Please check whether the communication is normal.");
                            return;
                        }
                        else
                        {
                            MessageBox.Show(@"通讯错误！请检查通讯是否正常。");
                            return;
                        }
                    }
                    return;
                }
            }
            catch (Exception EX_NAME)
            {
                Tools.ErrorLog(EX_NAME.ToString());
                if (_isEnglish == true)
                {
                    MessageBox.Show(@"Communication error! Please check whether the communication is normal.");
                    return;
                }
                else
                {
                    MessageBox.Show(@"通讯错误！请检查通讯是否正常。");
                    return;
                }
                // Console.WriteLine(EX_NAME);
                throw;
            }
            //解析P数据报文
            if (receiveBufferMessage[0] == Convert.ToByte('P'))
            {
                return;
                _channelParameters = Message.ExplainMessage<ChannelParameter>(receiveBufferMessage);//解析报文
                foreach (var itemParameter in _channelParameters)
                {
                    if (CmbChannelSelection.Text == itemParameter.Channel.ChannelName_English || CmbChannelSelection.Text == itemParameter.Channel.ChannelName)
                    {
                        /*
                         * 高压阈值赋值
                         */
                        TxtHV.Text = itemParameter.PresetHV.ToString();
                        Txtα.Text = itemParameter.AlphaThreshold.ToString();
                        Txtβ.Text = itemParameter.BetaThreshold.ToString();
                        //当前通道道盒参数
                        _setChannelParameter = itemParameter;
                        
                    }
                }
            }

            //解析C数据报文
            if (receiveBufferMessage[0] == Convert.ToByte('C'))
            {
                IList<MeasureData> measureDatas = new List<MeasureData>();
                measureDatas = Message.ExplainMessage<MeasureData>(receiveBufferMessage);
                //扔掉2次预读取的数据
                if (throwDataCount < 2)
                {
                    for (int i = 0; i < measureDataS.Count; i++)
                    {
                        measureDataS[i].Alpha = 0;
                        measureDataS[i].Beta = 0;
                        measureDataS[i].InfraredStatus = 0;
                    }
                    throwDataCount++;
                    _commPort.ClearPortData();
                    return;
                }
                
                //解析通道数据
                foreach (var item in measureDatas)
                {
                    if (_channel.ChannelID == item.Channel.ChannelID)
                    {

                        _alphacps += item.Alpha;//单次/时间的累加和
                        _betacps += item.Beta;//单次/时间的累加和
                        _alphacnt = _alphacnt + item.Alpha;//类型内计数累加
                        _betacnt = _betacnt + item.Beta;//类型内计数累加
                        if (_channel.ChannelID==7)
                        {
                            _hv = 500;
                        }
                        else
                        {
                            _hv = item.HV;//高压
                        }
                    }
                }
                _prgTime++;//进度条计数
                if (_prgTime> Convert.ToInt32(TxtMeasuringTime.Text))
                {
                    _prgTime = 0;
                    PrgCalibrate.Value = _prgTime;//进度条显示
                }
                else
                {
                    PrgCalibrate.Value = _prgTime;//进度条显示
                }
                if (_sclaeState==false)
                {
                   
                    if (_isEnglish)
                    {
                        _addInformation[0] = "BKG";
                    }
                    else
                    {
                        _addInformation[0] = "本底测量";
                    }
                    if (this.IsDisposed!=true)
                    {
                        _addInformation[1] = CmbChannelSelection.Text;
                        _addInformation[2] = area.ToString();
                        _addInformation[3] = (String.Format("{0:f2}", (_alphacps / Convert.ToSingle(TxtMeasuringTime.Text)))).ToString();
                        _addInformation[4] = (String.Format("{0:f2}", (_betacps / Convert.ToSingle(TxtMeasuringTime.Text)))).ToString();
                        _addInformation[5] = _hv.ToString();
                        _measuringTime--;
                    }                    
                    if (_measuringTime == 0)
                    {
                        _measuringCount--;//时间为0次数减1
                        _measuringTime = Convert.ToInt16(TxtMeasuringTime.Text);//次数减1后,时间恢复测量时间                        
                        DgvInformation.Rows.Add(_addInformation);//添加本次数据
                        _alphacps = 0;
                        _betacps = 0;
                        _prgTime = 0;//进度条时间
                    }

                    if (_measuringCount==0 )
                    {
                        _alphaNb =(_alphacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text)));//本底总计数的平均值
                        _betaNb = (_betacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text )));//本地总计数的平均值
                        _alphacnt = 0;//类型内计数清零
                        _betacnt = 0;//类型内计数清零
                        _measuringTime = Convert.ToInt16(TxtMeasuringTime.Text)+1;//恢复时间为填写时间
                        _measuringCount = Convert.ToInt16(TxtCount.Text);//恢复次数
                        
                        bkWorkerReceiveData.CancelAsync();
                        Thread.Sleep(500);
                        if (_isEnglish)
                        {
                            if (MessageBox.Show(@"Please insert the source!", @"Message") == DialogResult.OK)
                            {
                                _sclaeState = true;//刻度测量状态更换为"带源测量"
                                _alphacps = 0;
                                _betacps = 0;
                                bkWorkerReceiveData.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            if (MessageBox.Show(@"请放入放射源！", @"提示") == DialogResult.OK)
                            {
                                throwDataCount = 0;
                                _sclaeState = true;//刻度测量状态更换为"带源测量"
                                _alphacps = 0;
                                _betacps = 0;
                                _alphacnt = 0;
                                _betacnt = 0;
                                _prgTime = 0;//进度条时间
                                bkWorkerReceiveData.RunWorkerAsync();
                                Thread.Sleep(200);
                            }
                        }
                        return;
                    }
                }

                if (_sclaeState == true&&_measuringCount==0)
                {
                    return;
                }
                if (_sclaeState==true)
                {
                    //_prgTime++;
                    //PrgCalibrate.Value = _prgTime;
                    if (_isEnglish)
                    {
                        _addInformation[0] = "Radioactive source";
                    }
                    else
                    {
                        _addInformation[0] = "带源测量";
                    }                    
                    _addInformation[1] = CmbChannelSelection.Text;
                    _addInformation[2] = area.ToString();
                    _addInformation[3] = (String.Format("{0:f2}", (_alphacps /Convert.ToSingle(TxtMeasuringTime.Text)))).ToString();
                    _addInformation[4] = (String.Format("{0:f2}", (_betacps /Convert.ToSingle(TxtMeasuringTime.Text)))).ToString();
                    _addInformation[5] = _hv.ToString();
                    _measuringTime--;
                    if (_measuringTime == 0)
                    {
                        _measuringCount--;
                        _measuringTime = Convert.ToInt16(TxtMeasuringTime.Text);
                        DgvInformation.Rows.Add(_addInformation);
                        _alphacps = 0;
                        _betacps = 0;
                        _prgTime = 0;//进度条时间
                    }

                    if (_measuringCount == 0 )
                    {
                        //挂起线程
                        bkWorkerReceiveData.CancelAsync();
                        _alphaNr = (_alphacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text))); ;//带源总计数的平均值
                        _betaNr = (_betacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text)));//带源总计数的平均值
                        _effAlpha =(((_alphaNr - _alphaNb)<0?0:(_alphaNr - _alphaNb) )/ Convert.ToSingle(TxtSFR.Text));//Alpha效率
                        _effBeta = (((_betaNr - _betaNb)<0?0:(_betaNr - _betaNb) ) / Convert.ToSingle(TxtSFR.Text));//Beta效率
                        _eff = _effAlpha > _effBeta ? _effAlpha*100 : _effBeta*100;//效率取Alpha或Beta的最大值
                        if (_eff>0)
                        {
                            //Beta探测下限
                            _betaMda =
                                (_p * (_betaNb / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text)) +
                                       _betaNb / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text) * 2)) +
                                 (0.005f * _betaNb)) / (_effBeta / 2) / area;
                            //Alpha探测下限
                            _alphaMda =
                                (_p * (_alphaNb / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text)) +
                                       _alphaNb / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text) * 2)) +
                                 (0.005f * _betaNb)) / (_effBeta / 2) / area;
                        }
                        else
                        {
                            _betaMda = 0;
                            _alphaMda = 0;
                        }

                        _resultMda = _effAlpha > _effBeta ? _alphaMda : _betaMda;//探测下限取值和效率一样的
                        float rangeMda;//探测下限范围
                        //取当前核素的对象
                        var listef = _efficiencyList.Where(n =>
                                n.Channel.ChannelID == _channel.ChannelID && n.NuclideName == CmbNuclideSelect.Text)
                            .ToList();
                        foreach (var item in listef)
                        {
                            _changedEfficiency = item;//当前核素的对象
                        }
                        //根据核素类型判断探测下限范围
                        if (_changedEfficiency.NuclideType=="α")
                        {
                            rangeMda = 0.037f;
                        }
                        else if (_changedEfficiency.NuclideType=="β")
                        {
                            rangeMda = 0.37f;
                        }
                        else
                        {
                            rangeMda = 0;
                        }
                        //按通道用不同的判断方法判断探测器的合格
                        if (_channel.ChannelID==7)
                        {
                            if (_eff >= 20)
                            {
                                _isStandardize = "探测器合格!";
                            }
                            else
                            {
                                _isStandardize = "探测器不合格!";
                            }
                        }
                        else
                        {
                            //判断效率大于30%同时探测下限在一定范围内为合格
                            if (_eff >= 20 && _resultMda<=rangeMda)//需要补充代码的探测器下限范围
                            {
                                _isStandardize = "探测器合格!";
                            }
                            else
                            {
                                _isStandardize = "探测器不合格!";
                            }
                        }
                        //串道比计算
                        Calibration calibration_AlphaBetaPercent = new Calibration();
                        //if (_alphacnt>0 || _betacnt>0)
                        //{
                        //    if (_efficiencyList.First(x => x.NuclideType == "α").NuclideName == CmbNuclideSelect.Text)
                        //    {
                        //        //alpha 串道计数
                        //        calibration_AlphaBetaPercent.AlphaBetaPercent = _betacnt / _alphacnt * 100;
                        //    }
                        //    else
                        //    {
                        //        //beta 串道计数
                        //        calibration_AlphaBetaPercent.AlphaBetaPercent = _alphacnt / _betacnt * 100;
                        //    }
                            
                        //}
                        if (_alphacnt > 0 && _betacnt > 0)
                        {
                            if (_changedEfficiency.NuclideType == "α")
                            {
                                //alpha 串道计数
                                calibration_AlphaBetaPercent.AlphaBetaPercent = _betacnt / _alphacnt * 100;
                            }
                            else if (_changedEfficiency.NuclideType == "β")
                            {
                                //beta 串道计数
                                calibration_AlphaBetaPercent.AlphaBetaPercent = _alphacnt / _betacnt * 100;
                            }
                        }
                        else
                        {
                            calibration_AlphaBetaPercent.AlphaBetaPercent = 0;
                        }

                        if (calibration_AlphaBetaPercent.AlphaBetaPercent<=0)
                        {
                            //串道比小与零强制为0
                            calibration_AlphaBetaPercent.AlphaBetaPercent = 0;
                        }
                        //向刻度数据表添加信息
                        Calibration calibration = new Calibration
                        {
                            Efficiency = _eff > 0 ?Convert.ToSingle( _eff.ToString("0.00")) : 0,//效率
                            MDA = _resultMda > 0 ?Convert.ToSingle( _resultMda.ToString("0.00")):0,
                            AlphaBetaPercent =Convert.ToSingle( calibration_AlphaBetaPercent.AlphaBetaPercent.ToString("0.00")),//串道比
                            CalibrationTime = DateTime.Now,
                            Channel = _changedEfficiency.Channel,
                            HighVoltage = _setChannelParameter.PresetHV,
                            Threshold =
                            Convert.ToString(
                                $"α:{_setChannelParameter.AlphaThreshold};β:{_setChannelParameter.BetaThreshold}")
                        };
                        calibration.AddData(calibration);
                        //合格则把结果存入数据库
                        if (_isStandardize== "探测器合格!")
                        {
                            //向核素数据表添加信息
                            _changedEfficiency.Efficiency = _eff;
                            _changedEfficiency.SetParameter(_changedEfficiency);
                            
                        }
                        //不合格则把数据存入文本文件
                        else if(_isStandardize== "探测器不合格!")
                        {
                            string path = $@"CalibrationLog\{DateTime.Now.ToString("yyyyMMddTHHmmss")}.txt";
                            if (!File.Exists(path))
                            {
                                // Create a file to write to.
                                FileInfo fi1 = new FileInfo(path);
                                using (StreamWriter sw = fi1.CreateText())
                                {
                                    sw.WriteLine("状态\t通道\t面积(cm2)\tα计数\tβ计数\t高压（V）");
                                    string str = "";
                                    for (int i = 0; i < DgvInformation.Rows.Count - 1; i++)
                                    {
                                        for (int j = 0; j < DgvInformation.Columns.Count; j++)
                                        {
                                            str = DgvInformation.Rows[i].Cells[j].Value.ToString().Trim();
                                            str = str + "\t";
                                            sw.Write(str);
                                        }
                                        sw.WriteLine("");
                                    }
                                    sw.WriteLine($@"{CmbNuclideSelect.Text}的效率：{_eff:F2}%，可探测下限:{_resultMda:F2}Bq/cm^2;串道比:{calibration_AlphaBetaPercent.AlphaBetaPercent:f2}%；");
                                    sw.Close();
                                }
                            }
                        }
                        //测量结果
                        TxtResult.Text = $@"{CmbNuclideSelect.Text}的效率：{_eff:F2}%，可探测下限:{_resultMda:F2}Bq/cm^2;串道比:{calibration_AlphaBetaPercent.AlphaBetaPercent:f2}%；";
                        //使刻度按钮可以使用
                        BtnCalibrate.Enabled = true;
                        BtnSet.Enabled = true;
                        GrpCalibration.Enabled = true;
                    }
                }
            }
        }
        #endregion

        #endregion

        #region 按钮

        #region 设置
        /// <summary>
        /// 设置按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSet_Click(object sender, EventArgs e)
        {
            #region 信息判断

            if (CmbChannelSelection.Text == "")
            {
                _tools.PrompMessage(9);
                return;
            }
            //判断高压、alpha、Beta阈值是否为空
            if (TxtHV.Text == "" || TxtHV.Text == null || Txtα.Text == "" || Txtα.Text == null || Txtβ.Text == "" || Txtβ.Text == null)
            {
                _tools.PrompMessage(6);
                return;
            }
            //高压>1000提示
            else if (Convert.ToInt32(TxtHV.Text) > 1000)
            {
                _tools.PrompMessage(15);
                return;
            }
            //alpha阈值大于2000提示
            else if (Convert.ToInt32(Txtα.Text) > 2000)
            {
                _tools.PrompMessage(7);
                return;
            }
            //Beta阈值大于2000提示
            else if (Convert.ToInt32(Txtβ.Text) > 2000)
            {
                _tools.PrompMessage(7);
                return;
            } 
            #endregion
            
            //当前的高压和阈值信息写入对象
            _setChannelParameter.PresetHV =  Convert.ToSingle(TxtHV.Text);
            _setChannelParameter.AlphaThreshold = Convert.ToSingle(Txtα.Text);
            _setChannelParameter.BetaThreshold = Convert.ToSingle(Txtβ.Text);
            //点击刻度和设置后使按钮不可用
            BtnCalibrate.Enabled = false;
            BtnSet.Enabled = false;
            //判断串口是否打开
            if (_commPort.Opened == true)
            {
                int errorNumber = 0; //下发自检报文出现错误计数器
                byte[] buffMessage = new byte[62];//报文长度
                IList<ChannelParameter> _first_setChannelP = new List<ChannelParameter>();
                IList<ChannelParameter> _second_setChanelP = new List<ChannelParameter>();
                int i = 0;
                //把当前的高压阈值修改的数据对象添加到列表中
                foreach (var itme in _channelParameters)
                {
                    if (i < 4)
                    {
                        _first_setChannelP.Add(itme);
                    }
                    else
                    {
                        _second_setChanelP.Add(itme);
                    }
                    i++;
                }
                //_channelParameters.Clear();
                _second_setChanelP.RemoveAt(3);
                _second_setChanelP.Add(_setChannelParameter);
                //生成报文
                buffMessage = Message.BuildMessage(_first_setChannelP);
                Thread.Sleep(20);
                buffMessage = Message.BuildMessage(_second_setChanelP);
                Thread.Sleep(10);
                //成功则关闭线程
                try
                {
                    if (Message.SendMessage(buffMessage, _commPort))
                    {
                        //写入成功,返回p指令读取当前高压以确认更改成功
                        if (_isEnglish)
                        {
                            MessageBox.Show("Data has been distributed!", "Message");
                        }
                        else
                        {
                            MessageBox.Show("数据已经下发!", "提示");
                        }
                        Thread.Sleep(1000);
                        //点击刻度和设置后使按钮可用
                        BtnCalibrate.Enabled = true;
                        BtnSet.Enabled = true;
                    }
                    //发送失败次数大于5次,提示错误并挂起线程
                    else
                    {
                        errorNumber++;
                        if (errorNumber > 5)
                        {
                            _tools.PrompMessage(2);
                            //点击刻度和设置后使按钮可用
                            BtnCalibrate.Enabled = true;
                            BtnSet.Enabled = true;
                            return;
                        }
                        Thread.Sleep(200);

                    }
                }
                catch
                {
                    //点击刻度和设置后使按钮可用
                    BtnCalibrate.Enabled = true;
                    BtnSet.Enabled = true;
                    MessageBox.Show("设置失败,请重新尝试!");
                }
            }
            else
            {
                //错误提示
                _tools.PrompMessage(2);
                //点击刻度和设置后使按钮可用
                BtnCalibrate.Enabled = true;
                BtnSet.Enabled = true;
                return;
            }
        }
        #endregion

        #region 刻度
        /// <summary>
        /// 刻度按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCalibrate_Click(object sender, EventArgs e)
        {
            PrgCalibrate.Maximum = Convert.ToInt32(TxtMeasuringTime.Text);//进度条最大进度为测量时间
            _prgTime = 0;

            DgvInformation.Rows.Clear();
            TxtResult.Text = "";
            throwDataCount = 0;
            //刻度时清理上一次的读数
            _alphacps = 0;
            _betacps = 0;
            _alphacnt = 0;
            _betacnt = 0;
            #region 信息判断
            //通道判断
            if (CmbChannelSelection.Text == "")
            {
                _tools.PrompMessage(9);
                return;
            }
            //测量时间为0强制赋值为1,避免程序异常或进入死循环
            if (TxtMeasuringTime.Text == "" || TxtMeasuringTime.Text == Convert.ToString(0))
            {
                TxtMeasuringTime.Text ="1";
            }
            else if (Convert.ToInt32( TxtMeasuringTime.Text)>99)
            {
                _tools.PrompMessage(18);
            }
            //次数为0强制赋值为1,避免程序异常或进入死循环
            if (TxtCount.Text == "" || TxtCount.Text == Convert.ToString(0))
            {
                TxtCount.Text = "1";
            }
            //核素通道是否选择
            if (CmbNuclideSelect.Text == "")
            {
                _tools.PrompMessage(10);
                return;
            }

            if (TxtSFR.Text==""||TxtSFR.Text=="0")
            {
               _tools.PrompMessage(11);
               return;
            }
            #endregion

            _sclaeState = false;//刻度状态本地测量
            _measuringTime = Convert.ToInt32(TxtMeasuringTime.Text);//测量时间
            _measuringCount = Convert.ToInt16(TxtCount.Text);//测量次数
            _messageType = MessageType.CRead;
            if (_isEnglish)
            {

            }
            //点击刻度和设置后使按钮不可用
            BtnCalibrate.Enabled = false;
            BtnSet.Enabled = false;
            GrpCalibration.Enabled = false;
            if (_isEnglish==true?MessageBox.Show(@"Please enter the emissivity!", "Message", MessageBoxButtons.OKCancel) ==DialogResult.OK: MessageBox.Show(@"进行本底测量，确认远离放射源？", @"提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //刻度时清理上一次的读数
                _alphacps = 0;
                _betacps = 0;
                if (bkWorkerReceiveData.IsBusy == true)
                {
                    bkWorkerReceiveData.CancelAsync();
                    Thread.Sleep(100);
                    bkWorkerReceiveData.RunWorkerAsync();
                }
                else
                {
                    bkWorkerReceiveData.RunWorkerAsync();
                }
            }
            else
            {
                //点击刻度和设置后使按钮可用
                BtnCalibrate.Enabled = true;
                BtnSet.Enabled = true;
                GrpCalibration.Enabled = true;
            }
            
        }


        #endregion

        #endregion

        #region 文本框需要使用小键盘输入的
        private void TxtHV_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtHV);
        }


        private void Txtα_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(Txtα);
        }

        private void Txtβ_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(Txtβ);
        }

        private void TxtMeasuringTime_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtMeasuringTime);
        }

        private void TxtCount_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtCount);
        }

        private void TxtSFR_MouseClick(object sender, MouseEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtSFR);
        }
        #endregion

        #region 窗口关闭后
        /// <summary>
        /// 窗口关闭后,关闭线程,关闭端口
        /// </summary>
        private void FrmCalibration_FormClosed(object sender, FormClosedEventArgs e)
        {
            //_commPort.Close();
            bkWorkerReceiveData.CancelAsync();
            Thread.Sleep(200);
            this.Controls.Clear();
        }

        #endregion
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!IsHandleCreated)
            {
                this.Close();
            }
        }
    }
}

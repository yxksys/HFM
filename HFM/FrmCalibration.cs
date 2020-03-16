/**
 * ________________________________________________________________________________ 
 *
 *  描述：刻度窗体
 *  作者：杨旭锴
 *  版本：Alpha v.0.63-2020年3月7日;Alpha v.0.27-2020年3月6日
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
        /// 核数
        /// </summary>
        private int _nuclideId = 0;
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
        private int _numForaech = 0;
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
        private int _bkworkTime = 0;
        /// <summary>
        /// 刻度测量状态:false=本地测量;true=带源测量;
        /// </summary>
        private bool _sclaeState = false;
        #endregion

        #region 实例
        /// <summary>
        /// 串口实例
        /// </summary>
        private CommPort _commPort = new CommPort();
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
        private Tools _tools =new Components.Tools();
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
        private void OpenPort()
        {
            //从配置文件获得当前串口配置
            if (_commPort.Opened == true)
            {
                _commPort.Close();
            }
            _commPort.GetCommPortSet();
            //打开串口
            try
            {
                _commPort.Open();
            }
            catch
            {
                _tools.PrompMessage(1);
                return;
            }
        }
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
                DgvInformation.Columns[3].HeaderText = @"αCounts"; //α计数
                DgvInformation.Columns[4].HeaderText = @"βCounts"; //β计数
                DgvInformation.Columns[5].HeaderText = @"HV"; //高压
            }

            #endregion

            #region 获得全部启用的通道添加到下拉列表中，更具系统中英文状态选择中英文


            //根据系统语言填充通道下拉列表
            if (_isEnglish==true)
            {
                //英文通道名称
                foreach (var listChannel in _channelList)
                {
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
                    CmbChannelSelection.Items.Add(listChannel.ChannelName);
                    _channelNameEnglish[_numForaech] = listChannel.ChannelName;
                    _numForaech++;
                }
            }
            #endregion
        }
        #endregion

        #region 界面下拉列表
        #region 通道下拉列表
        /// <summary>
        /// 通道下拉列表选择后（触发事件）
        /// </summary>
        private void CmbChannelSelection_SelectedValueChanged(object sender, EventArgs e)
        {
            //如果选择衣物探头后，高压和阈值变为不可用状态
            if (CmbChannelSelection.Text == _channelNameEnglish[6] || CmbChannelSelection.Text == _channelName[6])
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
            }
            //当前通讯更改为pread
            _messageType = MessageType.PRead;
            //开启端口
            OpenPort();

            var lisChanneList = _channelList.Where(n => n.ChannelName.ToString() == CmbChannelSelection.Text|| n.ChannelName_English.ToString() == CmbChannelSelection.Text).ToList();
            foreach (var b in lisChanneList)
            {
                _channel = b;
            }
            if (_commPort.Opened == true)
            {
                //开启异步线程
                if (bkWorkerReceiveData.IsBusy != true)
                {
                    bkWorkerReceiveData.RunWorkerAsync();
                }
                else
                {
                    bkWorkerReceiveData.CancelAsync();
                    Thread.Sleep(100);
                    bkWorkerReceiveData.RunWorkerAsync();
                }
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
            byte[] receiveBuffMessage = new byte[200];//接受的报文
            byte[] buffMessage = new byte[62];//报文长度
            while (true)
            {
                //请求进程中断读取数据
                if (bkWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }

                switch (_messageType)
                {
                    #region P读取指令下发并接收数据上传
                    case MessageType.PRead:

                        //向下位机下发“p”指令码
                        buffMessage[0] = Convert.ToByte('P');
                        buffMessage[61] = Convert.ToByte(1);
                        if (Components.Message.SendMessage(buffMessage, _commPort) != true)
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
                        else if (Components.Message.SendMessage(buffMessage, _commPort) == true)    //正式
                        {
                            _bkworkTime++;
                            if (_bkworkTime > 1)
                            {
                                bkWorkerReceiveData.CancelAsync();
                                _bkworkTime = 0;
                                break;
                            }
                            //延时
                            Thread.Sleep(200);
                            receiveBuffMessage = Components.Message.ReceiveMessage(_commPort);
                            //延时
                            Thread.Sleep(800);
                            //触发向主线程返回下位机上传数据事件
                            bkWorker.ReportProgress(_bkworkTime, receiveBuffMessage);
                        }

                        break;
                    #endregion

                    #region P写入指令下发
                    case MessageType.PSet:
                        //把当前的高压阈值修改的数据对象添加到列表中
                        for (int i = 0; i < _channelParameters.Count; i++)
                        {
                            if (_setChannelParameter.CheckingID == _channelParameters[i].CheckingID)
                            {
                                _channelParameters.RemoveAt(i);
                                _channelParameters.Insert(i,_setChannelParameter);
                            }
                        }
                        //格式化道盒列表为7个通道,(超过程序出错)
                        for (int i = 0; i < _channelParameters.Count; i++)
                        {
                            if (i > 6)
                            {
                                _channelParameters.RemoveAt(i);
                                i--;
                            }
                        }
                        
                        //生成报文
                        buffMessage = Message.BuildMessage(_channelParameters);
                        //成功则关闭线程
                        if (Message.SendMessage(buffMessage,_commPort)==true)
                        {
                            //写入成功,返回p指令读取当前高压以确认更改成功
                            Thread.Sleep(1000);
                            _messageType = MessageType.PRead;
                        }
                        //发送失败次数大于5次,提示错误并挂起线程
                        else
                        {
                            errorNumber++;
                            if (errorNumber > 5)
                            {
                                _tools.PrompMessage(2);
                                bkWorkerReceiveData.CancelAsync();
                            }
                            Thread.Sleep(200);
                            
                        }
                        break;
                    #endregion

                    #region C读取指令下发并接收数据上传
                    case MessageType.CRead:
                        
                        //向下位机下发“C”指令码
                        buffMessage[0] = Convert.ToByte('C');
                        buffMessage[61] = Convert.ToByte(1);
                        if (Message.SendMessage(buffMessage, _commPort) == true)    //正式
                        {
                            //延时
                            Thread.Sleep(500);
                            receiveBuffMessage = Message.ReceiveMessage(_commPort);
                            //延时
                            Thread.Sleep(500);
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

                        break; 

                        #endregion
                }
            }
        }
        #endregion

        #region 刻度需要的字段

        /// <summary>
        /// 高压
        /// </summary>
        private float _hv = 0;
        /// <summary>
        /// alpha单次时间计数
        /// </summary>
        private float _alphacps = 0;
        /// <summary>
        /// beta单次时间计数
        /// </summary>
        private float _betacps = 0;
        /// <summary>
        /// alpha总计数
        /// </summary>
        private float _alphacnt = 0;
        /// <summary>
        /// Beta总计数
        /// </summary>
        private float _betacnt = 0;
        /// <summary>
        /// alpha本地平均数
        /// </summary>
        private float _alphaNb = 0;
        /// <summary>
        /// Beta本地平均数
        /// </summary>
        private float _betaNb = 0;
        /// <summary>
        /// alpha带源平均数
        /// </summary>
        private float _alphaNr = 0;
        /// <summary>
        /// beite带源平均数
        /// </summary>
        private float _betaNr = 0;
        /// <summary>
        /// Alpah效率
        /// </summary>
        private float _effAlpha = 0;
        /// <summary>
        /// Beta效率
        /// </summary>
        private float _effBeta = 0;
        /// <summary>
        /// 效率
        /// </summary>
        private float _eff = 0;
        /// <summary>
        /// 结果探测下限
        /// </summary>
        private float _resultMda = 0;
        /// <summary>
        /// Alpha探测下限
        /// </summary>
        private float _alphaMda = 0;
        /// <summary>
        /// Beta探测下限
        /// </summary>
        private float _betaMda = 0;
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

        #endregion
        #region ProgressChanged
        private void BkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
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
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 2)
                {
                    if (_isEnglish == true)
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

            //解析P数据报文
            if (receiveBufferMessage[0] == Convert.ToByte('P'))
            {
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
                
                //解析通道数据
                foreach (var item in measureDatas)
                {
                    if (_channel.ChannelID == item.Channel.ChannelID)
                    {

                        _alphacps += item.Alpha;//单次/时间的累加和
                        _betacps += item.Beta;//单次/时间的累加和
                        _alphacnt = _alphacnt + item.Alpha;//类型内计数累加
                        _betacnt = _betacnt + item.Beta;//类型内计数累加
                        _hv = item.HV;//高压
                        
                    }
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
                    _addInformation[1] = CmbChannelSelection.Text;
                    _addInformation[2] = area.ToString();
                    _addInformation[3] = ((_alphacps / Convert.ToSingle(TxtMeasuringTime.Text))).ToString();
                    _addInformation[4] = ((_betacps / Convert.ToSingle(TxtMeasuringTime.Text))).ToString();
                    _addInformation[5] = _hv.ToString();
                    _measuringTime--;
                    if (_measuringTime == 0)
                    {
                        _measuringCount--;//时间为0次数减1
                        _measuringTime = Convert.ToInt16(TxtMeasuringTime.Text);//次数减1后,时间恢复测量时间
                        DgvInformation.Rows.Add(_addInformation);//添加本次数据
                        _alphacps = 0;
                        _betacps = 0;
                    }

                    if (_measuringCount==0 )
                    {
                        _alphaNb =(_alphacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text)));//本底总计数的平均值
                        _betaNb = (_betacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text )));//本地总计数的平均值
                        _alphacnt = 0;//类型内计数清零
                        _betacnt = 0;//类型内计数清零
                        _measuringTime = Convert.ToInt16(TxtMeasuringTime.Text);//恢复时间为填写时间
                        _measuringCount = Convert.ToInt16(TxtCount.Text);//恢复次数
                        _sclaeState = true;//刻度测量状态更换为"带源测量"
                        bkWorkerReceiveData.CancelAsync();
                        Thread.Sleep(500);
                        if (_isEnglish)
                        {
                            if (MessageBox.Show(@"Please insert the source!", @"Message") == DialogResult.OK)
                            {
                                bkWorkerReceiveData.RunWorkerAsync();
                            }
                        }
                        else
                        {
                            if (MessageBox.Show(@"请放入放射源！", @"提示") == DialogResult.OK)
                            {
                                bkWorkerReceiveData.RunWorkerAsync();
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
                    _addInformation[0] = "带源测量";
                    _addInformation[1] = CmbChannelSelection.Text;
                    _addInformation[2] = area.ToString();
                    _addInformation[3] = ((_alphacps /Convert.ToSingle(TxtMeasuringTime.Text))).ToString();
                    _addInformation[4] = ((_betacps /Convert.ToSingle(TxtMeasuringTime.Text))).ToString();
                    _addInformation[5] = _hv.ToString();
                    _measuringTime--;
                    if (_measuringTime == 0)
                    {
                        _measuringCount--;
                        _measuringTime = Convert.ToInt16(TxtMeasuringTime.Text);
                        DgvInformation.Rows.Add(_addInformation);
                        _alphacps = 0;
                        _betacps = 0;
                    }

                    if (_measuringCount == 0 )
                    {
                        //挂起线程
                        bkWorkerReceiveData.CancelAsync();
                        _alphaNr = (_alphacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text))); ;//带源总计数的平均值
                        _betaNr = (_betacnt / (Convert.ToSingle(TxtMeasuringTime.Text) * Convert.ToSingle(TxtCount.Text)));//带源总计数的平均值
                        _effAlpha =((_alphaNr - _alphaNb) / Convert.ToSingle(TxtSFR.Text));//Alpha效率
                        _effBeta = ((_betaNr - _betaNb) / Convert.ToSingle(TxtSFR.Text));//Beta效率
                        _eff = _effAlpha > _effBeta ? _effAlpha*100 : _effBeta*100;//效率取Alpha或Beta的最大值
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
                        //alphaMDA =
                        //    ((2 * p) *
                        //     (float)((alphaNB / Convert.ToDouble(TxtMeasuringTime.Text) * Convert.ToDouble(TxtCount.Text)) +
                        //      (alphaNB / Convert.ToDouble(TxtMeasuringTime.Text) * Convert.ToDouble(TxtCount.Text)) *
                        //      2)) / effAlpha/2/area;
                        //betaMDA =
                        //    ((2 * p) *
                        //     (float)((betaNB / Convert.ToDouble(TxtMeasuringTime.Text) * Convert.ToDouble(TxtCount.Text)) +
                        //             (betaNB / Convert.ToDouble(TxtMeasuringTime.Text) * Convert.ToDouble(TxtCount.Text)) *
                        //             2)) / effBeta/2/area;
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
                            if (_eff >= 30)
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
                            if (_eff >= 30 && _resultMda<=rangeMda)//需要补充代码的探测器下限范围
                            {
                                _isStandardize = "探测器合格!";
                            }
                            else
                            {
                                _isStandardize = "探测器不合格!";
                            }
                        }

                        //向刻度数据表添加信息
                        Calibration calibration = new Calibration
                        {
                            Efficiency = _eff,//效率
                            MDA = _resultMda,
                            AlphaBetaPercent = 0,//串道比
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
                                    sw.WriteLine($@"{CmbNuclideSelect.Text}的效率：{_eff:F1}%，可探测下限:{_resultMda:F3}Bq/cm^2;串道比:100.000；{_isStandardize}");
                                    sw.Close();
                                }
                            }
                        }
                        //测量结果
                        TxtResult.Text = $@"{CmbNuclideSelect.Text}的效率：{_eff:F1}%，可探测下限:{_resultMda:F3}Bq/cm^2;串道比:100.000；{_isStandardize}";
                        
                        
                       
                    }
                }

                
                Lbl__time.Text = _measuringTime.ToString()+"----"+_measuringCount;
                
                
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
            //当前发送报文类型换成p写入
            _messageType = MessageType.PSet;
            //当前的高压和阈值信息写入对象
            _setChannelParameter.PresetHV = (float) Convert.ToDouble(TxtHV.Text);
            _setChannelParameter.AlphaThreshold = (float)Convert.ToDouble(Txtα.Text);
            _setChannelParameter.BetaThreshold = (float)Convert.ToDouble(Txtβ.Text);
            //判断串口是否打开
            if (_commPort.Opened == true)
            {
                //判断线程是否运行
                if (bkWorkerReceiveData.IsBusy == false)
                {
                    bkWorkerReceiveData.RunWorkerAsync();
                }
            }
            else
            {
                //错误提示
                _tools.PrompMessage(2);
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

            if (TxtSFR.Text=="")
            {
               _tools.PrompMessage(11);
               return;
            }
            #endregion

            _sclaeState = false;//刻度状态本地测量
            _measuringTime = Convert.ToInt32(TxtMeasuringTime.Text);//测量时间
            _measuringCount = Convert.ToInt16(TxtCount.Text);//测量次数
            _messageType = MessageType.CRead;
            if (MessageBox.Show(@"进行本底测量，确认远离放射源？", @"提示")==DialogResult.OK)
            {
                if (bkWorkerReceiveData.IsBusy == true)
                {
                    bkWorkerReceiveData.CancelAsync();
                    bkWorkerReceiveData.RunWorkerAsync();
                }
                else
                {

                    bkWorkerReceiveData.RunWorkerAsync();
                }
            }
            
        }

        #endregion

        #endregion


        #region 文本框限制只能输入数字
        //测量时间文本框
        private void TxtMeasuringTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                _tools.PrompMessage(14);
            }
        }
        //次数
        private void TxtCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                _tools.PrompMessage(14);
            }
        }
        //表面发射率
        private void TxtSFR_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                _tools.PrompMessage(14);
            }
        }
        //高压
        private void TxtHV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                _tools.PrompMessage(14);
            }
        }
        //alpha阈值
        private void Txtα_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                _tools.PrompMessage(14);
            }
        }
        //Beta阈值
        private void Txtβ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                _tools.PrompMessage(14);
            }
        } 
        #endregion
    }
}

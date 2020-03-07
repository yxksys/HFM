/**
 * ________________________________________________________________________________ 
 *
 *  描述：刻度窗体
 *  作者：杨旭锴
 *  版本：Alpha v.0.0.3-2020年3月6日
 *  创建时间：2020年2月25日 16:58:28
 *  类名：刻度窗体
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private int nuclideId = 0;
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        /// <summary>
        /// 通道中文名称数组
        /// </summary>
        private string[] channelName = new string[7];
        /// <summary>
        /// 通道英文名称数组
        /// </summary>
        private string[] channelNameEnglish = new string[7];
        /// <summary>
        /// 通用循环变量初始为0
        /// </summary>
        private int numForaech = 0;
        /// <summary>
        /// 当前发送报文的类型
        /// </summary>
        private MessageType messageType;
        /// <summary>
        /// 测量时间
        /// </summary>
        private int measuringTime;
        /// <summary>
        /// 异步线程初始化化时间,ReportProgress百分比数值
        /// </summary>
        private int bkworkTime = 0;
        #endregion

        #region 实例
        /// <summary>
        /// 串口实例
        /// </summary>
        CommPort commPort = new CommPort();
        /// <summary>
        /// 获取所有“通道参数”
        /// </summary>
        List<Channel> channelList = new Channel().GetChannel(true).ToList();
        /// <summary>
        /// 获取所有“效率参数”
        /// </summary>
        List<EfficiencyParameter> efficiencyList = new EfficiencyParameter().GetParameter().ToList();
        /// <summary>
        /// 工具类实例-错误提示信息
        /// </summary>
        Tools tools =new Components.Tools();
        #endregion

        #region 方法
        /// <summary>
        /// 开启串口封装的方法
        /// </summary>
        private void OpenPort()
        {
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
                tools.PrompMessage(1);
                return;
            }
        }
        /// <summary>
        /// 发送消息类型：
        /// </summary>
        enum MessageType
        {
            /// <summary>
            /// C读取类型
            /// </summary>
            cRead,
            /// <summary>
            /// P写入类型
            /// </summary>
            pSet,
            /// <summary>
            /// P读取类型
            /// </summary>
            pRead,
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
            
            #region 获得全部启用的通道添加到下拉列表中，更具系统中英文状态选择中英文
            
            
            //根据系统语言填充通道下拉列表
            if (isEnglish==true)
            {
                //英文通道名称
                foreach (var listChannel in channelList)
                {
                    CmbChannelSelection.Items.Add(listChannel.ChannelName_English);
                    channelName[numForaech] = listChannel.ChannelName_English;
                    numForaech++;
                }
            }
            else
            {
                //中文通道名称
                foreach (var listChannel in channelList)
                {
                    CmbChannelSelection.Items.Add(listChannel.ChannelName);
                    channelNameEnglish[numForaech] = listChannel.ChannelName;
                    numForaech++;
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
            if (CmbChannelSelection.Text == channelNameEnglish[6] || CmbChannelSelection.Text == channelName[6])
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
            messageType = MessageType.pRead;
            //开启端口
            OpenPort();
            //if (commPort.Opened==true)
            //{
            //开启异步线程
            if (bkWorkerReceiveData.IsBusy != true)
            {
                bkWorkerReceiveData.RunWorkerAsync();
            }
            //}


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
                MessageBox.Show("请先进行通道选择！在选取核素！");
            }
            //根据选择的通道进行选择核素
            if (CmbNuclideSelect.Items.Count == 0)
            {

                var listEfficiency = efficiencyList.Where(n =>
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

                switch (messageType)
                {
                    case MessageType.pRead:

                        //向下位机下发“p”指令码
                        buffMessage[0] = Convert.ToByte('P');
                        if (Components.Message.SendMessage(buffMessage, commPort) != true)
                        {
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                #region 模拟数据

                                receiveBuffMessage[0] = Convert.ToByte('P');
                                receiveBuffMessage[1] = Convert.ToByte(1);
                                receiveBuffMessage[16] = Convert.ToByte(2);
                                receiveBuffMessage[31] = Convert.ToByte(3);
                                receiveBuffMessage[46] = Convert.ToByte(4);
                                receiveBuffMessage[63] = Convert.ToByte(5);
                                receiveBuffMessage[78] = Convert.ToByte(6);
                                receiveBuffMessage[93] = Convert.ToByte(7);


                                #endregion
                                //MessageBox.Show("发送超时~", "提示");
                                //bkWorkerReceiveData.CancelAsync();
                                bkWorker.ReportProgress(1, receiveBuffMessage);
                                bkWorkerReceiveData.CancelAsync();
                            }
                            else
                            {
                                Thread.Sleep(delayTime);
                            }
                        }
                        else if (Components.Message.SendMessage(buffMessage, commPort) == true)    //正式
                        {
                            bkworkTime++;
                            if (bkworkTime > 1)
                            {
                                bkWorkerReceiveData.CancelAsync();
                                bkworkTime = 0;
                                break;
                            }
                            //延时
                            Thread.Sleep(100);
                            receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                            //延时
                            Thread.Sleep(1000);
                            //触发向主线程返回下位机上传数据事件
                            bkWorker.ReportProgress(bkworkTime, receiveBuffMessage);
                        }

                        break;
                    case MessageType.pSet:

                        break;
                    case MessageType.cRead:
                        //向下位机下发“C”指令码
                        buffMessage[0] = Convert.ToByte('C');
                        //判断串口是否打开，打开则用传输数据，否则用模拟数据

                        if (Components.Message.SendMessage(buffMessage, commPort) == true)    //正式
                                                                                              //if (HFM.Components.Message.SendMessage(buffMessage, commPort) != true)      //测试使用
                        {
                            //延时
                            Thread.Sleep(100);
                            receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                            //延时
                            Thread.Sleep(500);
                            //触发向主线程返回下位机上传数据事件
                            bkWorker.ReportProgress(bkworkTime, receiveBuffMessage);
                        }

                        break;
                }
            }
        }
        #endregion

        #region ProgressChanged
        private void bkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象                        
            bool isFirstBackGround = true;//进入等待测量状态后的本底测量计时标志
            string pollutionRecord = null;//记录测量污染详细数据
            if (e.UserState is byte[])
            {
                receiveBufferMessage = (byte[])e.UserState;
            }
            //接收报文数据为空
            //if (receiveBufferMessage.Length < messageBufferLength)
            //{
            //    //数据接收出现错误次数超限
            //    if (errNumber >= 2)
            //    {
            //        if (isEnglish == true)
            //        {
            //            MessageBox.Show("Communication error! Please check whether the communication is normal.");
            //            return;
            //        }
            //        else
            //        {
            //            MessageBox.Show("通讯错误！请检查通讯是否正常。");
            //            return;
            //        }

            //    }
            //    else
            //    {
            //        errNumber++;
            //    }

            //    return;
            //}
            try
            {
                if (receiveBufferMessage[0] == Convert.ToByte('P'))
                {
                    IList<ChannelParameter> channelParameters = new List<ChannelParameter>();
                    //解析报文
                    channelParameters = Message.ExplainMessage<ChannelParameter>(receiveBufferMessage);
                    numForaech = 0;//
                    foreach (var itemParameter in channelParameters)
                    {
                        if (CmbChannelSelection.Text==itemParameter.Channel.ChannelName_English||CmbChannelSelection.Text==itemParameter.Channel.ChannelName)
                        {
                            TxtHV.Text = itemParameter.PresetHV.ToString();
                            Txtα.Text = itemParameter.AlphaThreshold.ToString();
                            Txtβ.Text = itemParameter.BetaThreshold.ToString();

                        }
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("", "Message");
                throw;
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
            //判断串口是否打开
            if (commPort.Opened!=false)
            {
                tools.PrompMessage(1);
                return;
            }
            //判断高压、alpha、Beta阈值是否为空
            if (TxtHV.Text=="")
            {
                
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

        }

        #endregion

        #endregion

        private void TxtMeasuringTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                tools.PrompMessage(14);
            }
            
        }
    }
}

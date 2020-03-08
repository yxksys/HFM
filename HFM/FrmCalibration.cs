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
        /// <summary>
        /// 当前通道道盒参数数据对象
        /// </summary>
        ChannelParameter setChannelParameter = new ChannelParameter();
        Channel channel = new Channel();

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

            var lisChanneList = channelList.Where(n => n.ChannelName.ToString() == CmbChannelSelection.Text|| n.ChannelName_English.ToString() == CmbChannelSelection.Text).ToList();
            foreach (var b in lisChanneList)
            {
                channel = b;
            }
            //if (commPort.Opened==true)
            //{
            //开启异步线程
            //if (bkWorkerReceiveData.IsBusy != true)
            //{
                bkWorkerReceiveData.RunWorkerAsync();
            //}
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
                    #region P读取指令下发并接收数据上传
                    case MessageType.pRead:

                        //向下位机下发“p”指令码
                        buffMessage[0] = Convert.ToByte('P');
                        if (Components.Message.SendMessage(buffMessage, commPort) != true)
                        {
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                //#region 模拟数据

                                //receiveBuffMessage[0] = Convert.ToByte('P');
                                //receiveBuffMessage[1] = Convert.ToByte(1);
                                //receiveBuffMessage[16] = Convert.ToByte(2);
                                //receiveBuffMessage[31] = Convert.ToByte(3);
                                //receiveBuffMessage[46] = Convert.ToByte(4);
                                //receiveBuffMessage[63] = Convert.ToByte(5);
                                //receiveBuffMessage[78] = Convert.ToByte(6);
                                //receiveBuffMessage[93] = Convert.ToByte(7);


                                //#endregion
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
                    #endregion

                    #region P写入指令下发
                    case MessageType.pSet:
                        //实例化道盒列表
                        IList<ChannelParameter> setChannelParameters = new List<ChannelParameter>();
                        //添加数据对象到列表
                        setChannelParameters.Add(setChannelParameter);
                        //生成报文
                        buffMessage = Message.BuildMessage(setChannelParameters);
                        //成功则关闭线程
                        if (Components.Message.SendMessage(buffMessage,commPort)==true)
                        {
                            //写入成功,返回p指令读取当前高压以确认更改成功
                            Thread.Sleep(300);
                            messageType = MessageType.pRead;
                        }
                        //发送失败次数大于5次,提示错误并挂起线程
                        else
                        {
                            errorNumber++;
                            if (errorNumber > 5)
                            {
                                tools.PrompMessage(2);
                                bkWorkerReceiveData.CancelAsync();
                            }
                            Thread.Sleep(200);
                            
                        }
                        break;
                    #endregion

                    #region C读取指令下发并接收数据上传
                    case MessageType.cRead:
                        
                        //向下位机下发“C”指令码
                        buffMessage[0] = Convert.ToByte('C');
                        if (Components.Message.SendMessage(buffMessage, commPort) == true)    //正式
                        {
                            bkworkTime++;
                            if (bkworkTime > (Convert.ToInt32(TxtMeasuringTime)*(Convert.ToInt32( TxtCount))))
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
                        else
                        {
                            bkworkTime++;
                            if (bkworkTime > (Convert.ToInt32(TxtMeasuringTime.Text) * (Convert.ToInt32(TxtCount.Text)*2)))
                            {
                                bkWorkerReceiveData.CancelAsync();
                                bkworkTime = 0;
                                break;
                            }

                            //延时
                            Thread.Sleep(100);
                            receiveBuffMessage[0] = Convert.ToByte('C');
                            receiveBuffMessage[1] = Convert.ToByte(1);
                            receiveBuffMessage[16] = Convert.ToByte(2);
                            receiveBuffMessage[31] = Convert.ToByte(3);
                            receiveBuffMessage[46] = Convert.ToByte(4);
                            receiveBuffMessage[63] = Convert.ToByte(5);
                            receiveBuffMessage[78] = Convert.ToByte(6);
                            receiveBuffMessage[93] = Convert.ToByte(7);
                            //延时
                            Thread.Sleep(1000);
                            //触发向主线程返回下位机上传数据事件
                            bkWorker.ReportProgress(bkworkTime, receiveBuffMessage);



                            //errorNumber++;
                            ////判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            //if (errorNumber > 5)
                            //{
                            //    bkWorker.ReportProgress(1, receiveBuffMessage);
                            //    bkWorkerReceiveData.CancelAsync();
                            //}
                            //else
                            //{
                            //    Thread.Sleep(delayTime);
                            //}
                        }

                        break; 

                        #endregion
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
            //try
            //{
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

                            setChannelParameter = itemParameter;
                        }
                    }
                }
                else if (receiveBufferMessage[0]==Convert.ToByte('C'))
                {
                    IList<MeasureData> measureDatas=new List<MeasureData>();
                    #region 刻度需要的字段
                    float hv = 0;//高压
                    float alphacps = 0;//alpha单次时间计数
                    float betacps = 0;//beta单次时间计数
                    float alphacnt = 0;//alpha总计数
                    float betacnt = 0;//Beta总计数
                    float alphaNB = 0;//alpha本地平均值
                    float betaNB = 0;//Beta本地平均
                    float alphaNR = 0;//alpha带源平均
                    float betaNR = 0;//beite带源平均
                    float eff = 0;//效率
                    float mDER = 0;//探测下限
                    float p = 0.05f;//为5%误报率所要求的标准差数
                    float area = channel.ProbeArea;//探测面积
                    string [] addInformation=new string[6];
                    measureDatas = Message.ExplainMessage<MeasureData>(receiveBufferMessage);
                    #endregion

                    for (int i = 0; i < Convert.ToInt32(TxtMeasuringTime.Text); i++)
                    {
                        foreach (var item in measureDatas)
                        {
                            if (channel.ChannelID==item.Channel.ChannelID)
                            {
                                alphacps += item.Alpha;
                                betacps += item.Beta;
                                alphacnt = alphacnt+item.Alpha;
                                betacnt = betacnt + item.Beta;
                                hv = hv + item.HV;
                            }
                        }

                        if (i==Convert.ToInt32(TxtMeasuringTime.Text))
                        {
                            addInformation[0] = "本地测量";
                            addInformation[1] = CmbChannelSelection.Text;
                            addInformation[2] = area.ToString();
                            addInformation[3] = alphacps.ToString();
                            addInformation[4] = betacps.ToString();
                            addInformation[5] = (hv/Convert.ToInt32(TxtMeasuringTime.Text)).ToString();
                        }

                        DgvInformation.Rows.Add(addInformation);
                    }
                }
            //}
            //catch (Exception eerror)
            //{
            //    MessageBox.Show(eerror.Message);
            //    bkWorkerReceiveData.CancelAsync();
            //    return;
            //}
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
                tools.PrompMessage(9);
                return;
            }
            //判断高压、alpha、Beta阈值是否为空
            if (TxtHV.Text == "" || TxtHV.Text == null || Txtα.Text == "" || Txtα.Text == null || Txtβ.Text == "" || Txtβ.Text == null)
            {
                tools.PrompMessage(6);
                return;
            }
            //高压>1000提示
            else if (Convert.ToInt32(TxtHV.Text) > 1000)
            {
                tools.PrompMessage(15);
                return;
            }
            //alpha阈值大于2000提示
            else if (Convert.ToInt32(Txtα.Text) > 2000)
            {
                tools.PrompMessage(7);
                return;
            }
            //Beta阈值大于2000提示
            else if (Convert.ToInt32(Txtβ.Text) > 2000)
            {
                tools.PrompMessage(7);
                return;
            } 
            #endregion
            //当前发送报文类型换成p写入
            messageType = MessageType.pSet;
            //当前的高压和阈值信息写入对象
            setChannelParameter.PresetHV = (float) Convert.ToDouble(TxtHV.Text);
            setChannelParameter.AlphaThreshold = (float)Convert.ToDouble(Txtα.Text);
            setChannelParameter.BetaThreshold = (float)Convert.ToDouble(Txtβ.Text);
            //判断串口是否打开
            if (commPort.Opened == true)
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
                tools.PrompMessage(2);
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
                tools.PrompMessage(9);
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
                tools.PrompMessage(10);
                return;
            }

            if (TxtSFR.Text=="")
            {
               tools.PrompMessage(11);
               return;
            }
            #endregion

            tools.PrompMessage(12);

            messageType = MessageType.cRead;
            if (bkWorkerReceiveData.IsBusy == false)
            {
                bkWorkerReceiveData.RunWorkerAsync();
            }
            else
            {
                bkWorkerReceiveData.CancelAsync();
                bkWorkerReceiveData.RunWorkerAsync();
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
                tools.PrompMessage(14);
            }
        }
        //次数
        private void TxtCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                tools.PrompMessage(14);
            }
        }
        //表面发射率
        private void TxtSFR_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                tools.PrompMessage(14);
            }
        }
        //高压
        private void TxtHV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                tools.PrompMessage(14);
            }
        }
        //alpha阈值
        private void Txtα_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                tools.PrompMessage(14);
            }
        }
        //Beta阈值
        private void Txtβ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar))
            {
                e.Handled = true;
                tools.PrompMessage(14);
            }
        } 
        #endregion
    }
}

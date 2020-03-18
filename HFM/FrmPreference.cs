/**
 * ________________________________________________________________________________ 
 *
 *  描述：参数设置窗体
 *  作者：邢家宁
 *  版本：
 *  创建时间：2020年2月25日
 *  类名：参数设置
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
using System.Data.OleDb;
using System.Collections;
using HFM.Components;
using System.Threading;


namespace HFM
{
    public partial class FrmPreference : Form
    {
        public FrmPreference()
        {
            InitializeComponent();
        }

        //运行参数设置
        private ChannelParameter channelParameter = new ChannelParameter();//道盒信息(α阈值等)
        private HFM.Components.SystemParameter system = new HFM.Components.SystemParameter();//(自检时间、单位等)
        private FactoryParameter factoryParameter = new FactoryParameter();//仪器设备信息(IP地址、软件名称、是否双手探测器等)
        private ProbeParameter probeParameter = new ProbeParameter();//系统参数(各类型的本底上限等参数)
        private Nuclide nuclide = new Nuclide();//核素选择(U_235等)
        private EfficiencyParameter efficiencyParameter = new EfficiencyParameter();//探测效率(各类型的探测效率)
        private CommPort commPort = new CommPort();//串口通讯
        private IList<MeasureData> measureDataS = new List<MeasureData>();//解析报文
        private int bkworkTime = 0;// 异步线程初始化化时间,ReportProgress百分比数值
        private Tools tools = new Tools();//工具类
        //通讯类型
        enum MessageType
        {
            pSet,// P写入类型
            pRead,// P读取类型
        }
        private MessageType messageType;



        /// <summary>
        /// 加载参数(初始化)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPreference_Load(object sender, EventArgs e)
        {
            //线程支持异步取消
            backgroundWorker_Preference.WorkerSupportsCancellation = true;

            GetProferenceData();
            GetAlphaData();
            GetBetaData();
            GetClothesData();
            GetMainProferenceData();
            GetFacilityData();
        }
        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabPresence_SelectedIndexChanged(object sender, EventArgs e)
        {
            //根据页面索引更新当前页面值
            switch (TabPresence.SelectedIndex)
            {
                case 0:
                    GetProferenceData();
                    break;
                case 1:
                    GetAlphaData();
                    break;
                case 2:
                    GetBetaData();
                    break;
                case 3:
                    GetClothesData();
                    break;
                case 4:
                    GetMainProferenceData();
                    break;
                case 5:
                    GetFacilityData();
                    break;
                default:
                    MessageBox.Show("选择有误，请重新选择");
                    break;
            }

        }

        #region 获得数据库数据并显示出来
        /// <summary>
        /// 获得系统页面参数
        /// </summary>
        private void GetProferenceData()
        {
            system = new HFM.Components.SystemParameter().GetParameter();//获得自检时间、单位等参数
            factoryParameter.GetParameter();//获得仪器设备信息参数

            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//获得探测面积参数
            probeParameters = probeParameter.GetParameter();

            #region 系统参数
            TxtSelfCheckTime.Text = system.SelfCheckTime.ToString();
            TxtSmoothingTime.Text = system.SmoothingTime.ToString();
            TxtMeasuringTime.Text = system.MeasuringTime.ToString();
            CmbMeasurementUnit.Text = system.MeasurementUnit.ToString();
            TxtAlarmTime.Text = system.AlarmTime.ToString();
            TxtBKGUpdate.Text = system.BkgUpdate.ToString();
            #endregion

            #region 工厂参数
            TxtSmoothingFactor.Text = factoryParameter.SmoothingFactor.ToString();
            TxtInstrumentNum.Text = factoryParameter.InstrumentNum.ToString();
            TxtSoftName.Text = factoryParameter.SoftName;
            TxtPortNumber.Text = factoryParameter.PortNumber;

            string[] k = factoryParameter.IpAddress.Split('.');//分割地址
            TxtIPAddressOne.Text = k[0];
            TxtIPAddressTwo.Text = k[1];
            TxtIPAddressThree.Text = k[2];
            TxtIPAddressFour.Text = k[3];

            TxtPortNumber.Text = factoryParameter.PortNumber;
            ChkIsConnectedAuto.Checked = factoryParameter.IsConnectedAuto;
            CmbUnclideType.Text = factoryParameter.MeasureType;
            TxtPortNumber.Text = factoryParameter.PortNumber;  
            #endregion

            #region 探测面积
            ArrayList a = new ArrayList(7);//channel测量面积数组
            a.Add(TxtLeftInProbeArea);
            a.Add(TxtLeftOutProbeArea);
            a.Add(TxtRightInProbeArea);
            a.Add(TxtRightOutProbeArea);
            a.Add(TxtLeftFootProbeArea);
            a.Add(TxtRightFootProbeArea);
            a.Add(TxtCloseProbeArea);

            ArrayList label = new ArrayList(7);//channel名字标签数组
            label.Add(LblLeftInProbeArea);
            label.Add(LblLeftOutProbeArea);
            label.Add(LblRightInProbeArea);
            label.Add(LblRightOutProbeArea);
            label.Add(LblLeftFootProbeArea);
            label.Add(LblRightFootProbeArea);
            label.Add(LblClothesProbeArea);

            //获得探测面积
            for (int i = 0; i < probeParameters.Count; i++)
            {
                if (probeParameters[i].ProbeChannel.IsEnabled)
                {
                    //根据channelID来修改数据
                    ((TextBox)a[probeParameters[i].ProbeChannel.ChannelID - 1]).Text = probeParameters[i].ProbeChannel.ProbeArea.ToString();
                }
                else
                {
                    //根据channelID来修改数据
                    ((TextBox)a[probeParameters[i].ProbeChannel.ChannelID - 1]).Enabled = false;
                    ((Label)label[probeParameters[i].ProbeChannel.ChannelID - 1]).Enabled = false;
                }
            }
            //未启用则数据修改只能对左手进行
            if (!factoryParameter.IsDoubleProbe)
            {
                //使外手心同步与内手心
                ((TextBox)a[1]).Text = ((TextBox)a[0]).Text;
                ((TextBox)a[3]).Text = ((TextBox)a[2]).Text;
                ((TextBox)a[1]).Enabled = false;
                ((TextBox)a[3]).Enabled = false;
                ((Label)label[1]).Enabled = false;
                ((Label)label[3]).Enabled = false;
            }
            #endregion

        }
        /// <summary>
        /// 获得α数据
        /// </summary>
        private void GetAlphaData()
        {

            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//获得α参数
            probeParameters = probeParameter.GetParameter("α");

            #region 核素选择

            //获得当前核素选择
            string nowNuclideName = nuclide.GetAlphaNuclideUser();//获得当前α核素名称
            IList<EfficiencyParameter> efficiency = new List<EfficiencyParameter>();
            efficiency = efficiencyParameter.GetParameter("α", nowNuclideName);//获得当前核素效率
            IList<RadioButton> button = new List<RadioButton>();//核素选择数组
            button.Add(RdoAlpha235);
            button.Add(RdoAlpha239);
            button.Add(RdoAlphaDefine1);
            button.Add(RdoAlpha238);
            button.Add(RdoAlpha241);
            button.Add(RdoAlphaDefine2);
            for (int i = 0; i < button.Count; i++)
            {
                if (nowNuclideName == button[i].Text)
                {
                    button[i].Checked = true;
                    break;
                }
            }

            #endregion

            //把当前所选核素效率保存到ProbeParameter当前效率数据库中
            for (int i = 0; i < efficiency.Count; i++)
            {
                //根据channelID来匹配
                for (int j = 0; j < probeParameters.Count; j++)
                {
                    if (probeParameters[j].ProbeChannel.ChannelID == efficiency[i].Channel.ChannelID)
                    {
                        probeParameters[j].Efficiency = efficiency[i].Efficiency;//把得到效率传送给当前效率
                        probeParameter.SetParameter(probeParameters[j]);//保存到数据库
                    }
                }
            }

            #region α参数
            //清除所有行(因为每次切换页面都会增加相应的行)
            for (int i = 0; i < DgvAlphaSet.Rows.Count; i++)
            {
                DgvAlphaSet.Rows.Remove(DgvAlphaSet.Rows[i]);
                i--;
            }
            
            //选出启用的设备
            for (int i = 0; i < probeParameters.Count; i++)
            {
                //设备启用且核素类型为α并除去衣物参数
                if (probeParameters[i].ProbeChannel.IsEnabled && probeParameters[i].NuclideType == "α" &&probeParameters[i].ProbeChannel.ChannelID != 7)
                {
                    int index = this.DgvAlphaSet.Rows.Add();
                    DgvAlphaSet.Rows[index].Cells[0].Value = probeParameters[i].ProbeChannel.ChannelName;
                    DgvAlphaSet.Rows[index].Cells[1].Value = probeParameters[i].HBackground;
                    DgvAlphaSet.Rows[index].Cells[2].Value = probeParameters[i].LBackground;
                    DgvAlphaSet.Rows[index].Cells[3].Value = probeParameters[i].Alarm_1;
                    DgvAlphaSet.Rows[index].Cells[4].Value = probeParameters[i].Alarm_2;
                    DgvAlphaSet.Rows[index].Cells[5].Value = probeParameters[i].Efficiency;
                }
                //设备未启用(暂时不显示)
                else
                {
                }
            }
            #endregion

        }
        /// <summary>
        /// 获得β数据
        /// </summary>
        private void GetBetaData()
        {
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//获得β参数
            probeParameters = probeParameter.GetParameter("β");

            #region 核素选择

            //获得当前核素选择
            string nowNuclideName = nuclide.GetBetaNuclideUser();//获得当前β核素名称
            IList<EfficiencyParameter> efficiency = new List<EfficiencyParameter>();
            efficiency = efficiencyParameter.GetParameter("β", nowNuclideName);//获得当前核素效率
            IList<RadioButton> button = new List<RadioButton>();//核素选择数组
            button.Add(RdoBeta14);
            button.Add(RdoBeta58);
            button.Add(RdoBeta131);
            button.Add(RdoBeta204);
            button.Add(RdoBeta32);
            button.Add(RdoBeta60);
            button.Add(RdoBeta137);
            button.Add(RdoBetaDefine1);
            button.Add(RdoBeta36);
            button.Add(RdoBeta90);
            button.Add(RdoBeta192);
            button.Add(RdoBetaDefine2);
            for (int i = 0; i < button.Count; i++)
            {
                if (nowNuclideName == button[i].Text)
                {
                    button[i].Checked = true;
                    break;
                }
            }

            #endregion

            //把当前所选核素效率保存到ProbeParameter当前效率数据库中
            for (int i = 0; i < efficiency.Count; i++)
            {
                //根据channelID来匹配
                for (int j = 0; j < probeParameters.Count; j++)
                {
                    if (probeParameters[j].ProbeChannel.ChannelID == efficiency[i].Channel.ChannelID)
                    {
                        probeParameters[j].Efficiency = efficiency[i].Efficiency;//把得到效率传送给当前效率
                        probeParameter.SetParameter(probeParameters[j]);//保存到数据库
                    }
                }
            }


            #region β参数

            //清除所有行(因为每次切换页面都会增加相应的行)
            for (int i = 0; i < DgvBetaSet.Rows.Count; i++)
            {
                DgvBetaSet.Rows.Remove(DgvBetaSet.Rows[i]);
                i--;
            }

            //选出启用的设备
            for (int i = 0; i < probeParameters.Count; i++)
            {
                //设备启用且核素类型为α并除去衣物参数
                if (probeParameters[i].ProbeChannel.IsEnabled && probeParameters[i].NuclideType == "β" && probeParameters[i].ProbeChannel.ChannelID != 7)
                {
                    int index = this.DgvBetaSet.Rows.Add();
                    DgvBetaSet.Rows[index].Cells[0].Value = probeParameters[i].ProbeChannel.ChannelName;
                    DgvBetaSet.Rows[index].Cells[1].Value = probeParameters[i].HBackground;
                    DgvBetaSet.Rows[index].Cells[2].Value = probeParameters[i].LBackground;
                    DgvBetaSet.Rows[index].Cells[3].Value = probeParameters[i].Alarm_1;
                    DgvBetaSet.Rows[index].Cells[4].Value = probeParameters[i].Alarm_2;
                    DgvBetaSet.Rows[index].Cells[5].Value = probeParameters[i].Efficiency;
                }
                //设备未启用(暂时不显示)
                else
                {
                }

                #endregion
            
            }
        }
        /// <summary>
        /// 获得衣物参数
        /// </summary>
        private void GetClothesData()
        {
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//获得C参数
            probeParameters = probeParameter.GetParameter();

            #region 核素选择

            string nowNuclideName = nuclide.GetClothesNuclideUser();//获得当前衣物核素名称
            IList<EfficiencyParameter> efficiency = new List<EfficiencyParameter>();
            efficiency = efficiencyParameter.GetParameter("C", nowNuclideName);//获得当前核素效率
            IList<RadioButton> button = new List<RadioButton>();//核素选择数组

            #region 添加核素
            //α核素
            button.Add(RdoClothesAlpha235);
            button.Add(RdoClothesAlpha238);
            button.Add(RdoClothesAlpha239);
            button.Add(RdoClothesAlpha241);
            button.Add(RdoClothesAlphaDefine1);
            //β核素
            button.Add(RdoClothesBeta14);
            button.Add(RdoClothesBeta32);
            button.Add(RdoClothesBeta36);
            button.Add(RdoClothesBeta58);
            button.Add(RdoClothesBeta60);
            button.Add(RdoClothesBeta90);
            button.Add(RdoClothesBeta131);
            button.Add(RdoClothesBeta137);
            button.Add(RdoClothesBeta192);
            button.Add(RdoClothesBeta204);
            button.Add(RdoClothesBetaDefine1);
            #endregion

            for (int i = 0; i < button.Count; i++)
            {
                if (nowNuclideName == button[i].Text)
                {
                    button[i].Checked = true;
                    break;
                }
            }

            #endregion

            //把当前所选核素效率保存到ProbeParameter当前效率数据库中
            for (int i = 0; i < efficiency.Count; i++)
            {
                //根据channelID来匹配
                for (int j = 0; j < probeParameters.Count; j++)
                {
                    if (probeParameters[j].ProbeChannel.ChannelID == efficiency[i].Channel.ChannelID)
                    {
                        probeParameters[j].Efficiency = efficiency[i].Efficiency;//把得到效率传送给当前效率
                        probeParameter.SetParameter(probeParameters[j]);//保存到数据库
                        probeParameter = probeParameters[j];
                    }
                }
            }

            #region 衣物探头

            system = system.GetParameter();//获得衣物离线自检时间
            TxtClothesHBackground.Text = probeParameter.HBackground.ToString();
            TxtClothesLBackground.Text = probeParameter.LBackground.ToString();
            TxtClothesAlarm_1.Text = probeParameter.Alarm_1.ToString();
            TxtClothesAlarm_2.Text = probeParameter.Alarm_2.ToString();
            TxtClothesEfficiency.Text = probeParameter.Efficiency.ToString();
            TxtClothOfflineTime.Text = system.ClothOfflineTime.ToString();

            #endregion

        }
        /// <summary>
        /// 获得道盒参数(数据库)
        /// </summary>
        private void GetMainProferenceData()
        {
            IList<ChannelParameter> channelParameters = new List<ChannelParameter>();//获得道盒参数
            channelParameters = channelParameter.GetParameter();
            //清除所有行(因为每次切换页面都会增加相应的行)
            for (int i = 0; i < DgvMainPreferenceSet.Rows.Count; i++)
            {
                DgvMainPreferenceSet.Rows.Remove(DgvMainPreferenceSet.Rows[i]);
                i--;
            }
            //选出启用的设备
            for (int i = 0; i < channelParameters.Count; i++)
            {
                //设备启用设备
                if (channelParameters[i].Channel.IsEnabled)
                {
                    int index = this.DgvMainPreferenceSet.Rows.Add();
                    DgvMainPreferenceSet.Rows[index].Cells[0].Value = channelParameters[i].Channel.ChannelName;
                    DgvMainPreferenceSet.Rows[index].Cells[1].Value = channelParameters[i].AlphaThreshold;
                    DgvMainPreferenceSet.Rows[index].Cells[2].Value = channelParameters[i].BetaThreshold;
                    DgvMainPreferenceSet.Rows[index].Cells[3].Value = channelParameters[i].PresetHV;
                    DgvMainPreferenceSet.Rows[index].Cells[4].Value = channelParameters[i].ADCFactor;
                    DgvMainPreferenceSet.Rows[index].Cells[5].Value = channelParameters[i].DACFactor;
                    DgvMainPreferenceSet.Rows[index].Cells[6].Value = channelParameters[i].HVFactor;
                    DgvMainPreferenceSet.Rows[index].Cells[7].Value = channelParameters[i].WorkTime;
                    DgvMainPreferenceSet.Rows[index].Cells[8].Value = channelParameters[i].HVRatio;
                }
                //设备未启用(暂时不显示)
                else
                {
                }
            }

        }
        //获得仪器选择参数
        private void GetFacilityData()
        {
            Channel channel = new Channel();
            //判断是否启用手部
            channel = channel.GetChannel(1);
            if(channel.IsEnabled)
            {
                ChkHand.Checked = true;
                //判断是否启用双探头
                factoryParameter = factoryParameter.GetParameter();
                //如果未启用则关闭手背设备,以手心为主
                if (!factoryParameter.IsDoubleProbe)
                {
                    channel.SetEnabledByID(2, false);
                    channel.SetEnabledByID(4, false);
                    RdoSingleHand.Checked = true;
                }
                else
                {
                    RdoDoubleHand.Checked = true;
                }
            }
            else
            {
                ChkHand.Checked = false;
                RdoSingleHand.Checked = false;
                RdoDoubleHand.Checked = false;
            }
            //判断是否启用脚步
            channel = channel.GetChannel(5);
            if (channel.IsEnabled)
            {
                ChkFoot.Checked = true;
            }
            else
            {
                ChkFoot.Checked = false;
            }
            //判断是否启用衣物探头
            channel = channel.GetChannel(7);
            if (channel.IsEnabled)
            {
                ChkClothes.Checked = true;
            }
            else
            {
                ChkClothes.Checked = false;
            }
        }



        #endregion

        #region 串口通信
        /// <summary>
        /// 异步线程DoWork事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_Preference_DoWork(object sender, DoWorkEventArgs e)
        {
            //如果没有取消异步线程
            if (backgroundWorker_Preference.CancellationPending == false)
            {
                //在异步线程上执行串口读操作ReadDataFromSerialPort方法
                BackgroundWorker bkWorker = sender as BackgroundWorker;
                e.Result = ReadDataFromSerialPort(bkWorker, e);
            }
            e.Cancel = true;
        }

        /// <summary>
        /// 通过串口读取下位机上传数据
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private byte[] ReadDataFromSerialPort(BackgroundWorker worker, DoWorkEventArgs e)
        {
            int errorNumber = 0; //下发自检报文出现错误计数器
            int delayTime = 200;//下发自检报文延时时间
            byte[] receiveBuffMessage = new byte[200];//接受的报文
            byte[] buffMessage = new byte[62];//报文长度
            while (true)
            {
                //请求进程中断读取数据
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }

                switch (messageType)
                {
                    #region P读取指令下发并接收数据上传
                    case MessageType.pRead:
                        //向下位机下发“P”指令码
                        buffMessage[0] = Convert.ToByte('P');
                        if (Components.Message.SendMessage(buffMessage, commPort) != true)
                        {
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {

                                MessageBox.Show("发送超时~", "提示");
                                //bkWorkerReceiveData.CancelAsync();
                                worker.ReportProgress(1, receiveBuffMessage);
                                backgroundWorker_Preference.CancelAsync();
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(delayTime);
                            }
                        }
                        else if (Components.Message.SendMessage(buffMessage, commPort) == true)//正式
                        {
                            bkworkTime++;
                            if (bkworkTime > 1)
                            {
                                backgroundWorker_Preference.CancelAsync();
                                bkworkTime = 0;
                                break;
                            }
                            //延时
                            System.Threading.Thread.Sleep(100);
                            receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                            //延时
                            System.Threading.Thread.Sleep(1000);
                            //触发向主线程返回下位机上传数据事件
                            worker.ReportProgress(bkworkTime, receiveBuffMessage);
                        }
                        break;
                    #endregion

                    #region P写入指令下发
                    case MessageType.pSet:
                        //实例化道盒列表
                        IList<ChannelParameter> setChannelParameters = new List<ChannelParameter>();
                        //添加数据对象到列表
                        setChannelParameters.Add(channelParameter);
                        //生成报文
                        buffMessage = HFM.Components.Message.BuildMessage(setChannelParameters);
                        //成功则关闭线程
                        if (Components.Message.SendMessage(buffMessage, commPort) == true)
                        {
                            //写入成功,返回p指令读取当前高压以确认更改成功
                            System.Threading.Thread.Sleep(300);
                            messageType = MessageType.pRead;
                        }
                        else
                        {
                            errorNumber++;
                            if (errorNumber > 5)
                            {
                                tools.PrompMessage(2);//提示
                                backgroundWorker_Preference.CancelAsync();
                            }
                            System.Threading.Thread.Sleep(200);
                        }
                        break;
                    #endregion
                }

            }
        }

        /// <summary>
        /// 异步线程读取串口数据后的ReportProgress事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker_Preference_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
            //IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象                        
            if (e.UserState is byte[])
            {
                receiveBufferMessage = (byte[])e.UserState;
            }

            //接收报文数据为空
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 5)
                {
                    //界面提示“通讯错误”
                    MessageBox.Show("通讯错误");
                    return;

                }
                else
                {
                    errNumber++;
                }
                return;
            }
            //接收报文无误，进行报文解析，并将解析后的道盒数据存储到channelParameters中 
            try
            {
                if (receiveBufferMessage[0] == Convert.ToByte('P'))
                {
                    IList<ChannelParameter> channelParameters = new List<ChannelParameter>();
                    //解析报文
                    channelParameters = HFM.Components.Message.ExplainMessage<ChannelParameter>(receiveBufferMessage);
                    foreach (var itemParameter in channelParameters)
                    {
                        //显示内容
                        int index = this.DgvMainPreferenceSet.Rows.Add();
                        DgvMainPreferenceSet.Rows[index].Cells[0].Value = itemParameter.Channel.ChannelName;
                        DgvMainPreferenceSet.Rows[index].Cells[1].Value = itemParameter.AlphaThreshold;
                        DgvMainPreferenceSet.Rows[index].Cells[2].Value = itemParameter.BetaThreshold;
                        DgvMainPreferenceSet.Rows[index].Cells[3].Value = itemParameter.PresetHV;
                        DgvMainPreferenceSet.Rows[index].Cells[4].Value = itemParameter.ADCFactor;
                        DgvMainPreferenceSet.Rows[index].Cells[5].Value = itemParameter.DACFactor;
                        DgvMainPreferenceSet.Rows[index].Cells[6].Value = itemParameter.HVFactor;
                        DgvMainPreferenceSet.Rows[index].Cells[7].Value = itemParameter.WorkTime;
                        DgvMainPreferenceSet.Rows[index].Cells[8].Value = itemParameter.HVRatio;
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("", "Message");
                throw;
            }
        }

    


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
                MessageBox.Show(@"端口打开错误！请检查通讯是否正常。");
            }
        }

        #endregion

        #region 按钮(确定、取消)

        #region 系统参数页面
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPreferenceOk_Click(object sender, EventArgs e)
        {
            #region 系统参数
            //首先获得默认参数,通过对原始数据赋值来实现更新
            HFM.Components.SystemParameter system = new HFM.Components.SystemParameter();
            system = system.GetParameter();
            system.SelfCheckTime = int.Parse(TxtSelfCheckTime.Text);
            system.SmoothingTime = Convert.ToInt32(TxtSmoothingTime.Text);
            system.MeasuringTime = int.Parse(TxtMeasuringTime.Text);
            system.MeasurementUnit = CmbMeasurementUnit.Text;
            system.AlarmTime = int.Parse(TxtAlarmTime.Text);
            system.BkgUpdate = int.Parse(TxtBKGUpdate.Text);
            #endregion

            #region 探测面积
            IList<Channel> channels = new Channel().GetChannel();//获得探测面积参数
            IList<TextBox> a = new List<TextBox>(7);//channel测量面积数组
            a.Add(TxtLeftInProbeArea);
            a.Add(TxtLeftOutProbeArea);
            a.Add(TxtRightInProbeArea);
            a.Add(TxtRightOutProbeArea);
            a.Add(TxtLeftFootProbeArea);
            a.Add(TxtRightFootProbeArea);
            a.Add(TxtCloseProbeArea);
            for (int i = 0; i < a.Count; i++)
            {
                //写入已启用设备面积
                if (a[i].Enabled)
                {
                    //根据通道id修改数据
                    for (int j = 0; j < channels.Count; j++)
                    {
                        if (channels[j].ChannelID == i+1)
                        {
                            channels[j].ProbeArea = Convert.ToSingle(a[i].Text);
                        }
                    }
                    
                }
            }
            #endregion

            #region 工厂参数
            FactoryParameter factoryParameter = new FactoryParameter().GetParameter();//获得仪器设备信息参数
            factoryParameter.SmoothingFactor = int.Parse(TxtSmoothingFactor.Text);
            factoryParameter.InstrumentNum = TxtInstrumentNum.Text;
            factoryParameter.SoftName = TxtSoftName.Text;
            factoryParameter.PortNumber = TxtPortNumber.Text;
            factoryParameter.IsConnectedAuto = ChkIsConnectedAuto.Checked;
            factoryParameter.MeasureType = CmbUnclideType.Text;
            factoryParameter.IpAddress = TxtIPAddressOne.Text + '.' + TxtIPAddressTwo.Text + '.'
                                         + TxtIPAddressThree.Text + '.' + TxtIPAddressFour.Text;
            #endregion

            #region 存储数据库
            for (int i = 0; i < channels.Count; i++)
            {
                if (new Channel().SetProbeAreaByID(channels[i].ChannelID, channels[i].ProbeArea))
                {
                }
                else
                {
                    MessageBox.Show("更新失败");
                    return;
                }
            }
            if (new HFM.Components.SystemParameter().SetParameter(system) && new FactoryParameter().SetParameter(factoryParameter))
            {
                MessageBox.Show("更新成功");
            }
            else
            {
                MessageBox.Show("更新失败");
                return;
            }
                
            
            
            #endregion
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPreferenceNo_Click(object sender, EventArgs e)
        {
            //重新获得数据库数据
            GetProferenceData();
        }

        #endregion

        #region α参数界面
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAlphaOk_Click(object sender, EventArgs e)
        {
            //注：α参数和α核素选择顺序不可互换

            #region α核素选择
            string nuclidename = "";//修改核素选择
            IList<RadioButton> button = new List<RadioButton>();//核素选择数组
            button.Add(RdoAlpha235);
            button.Add(RdoAlpha239);
            button.Add(RdoAlphaDefine1);
            button.Add(RdoAlpha238);
            button.Add(RdoAlpha241);
            button.Add(RdoAlphaDefine2);
            for (int i = 0; i < button.Count; i++)
            {
                if (button[i].Checked)
                {
                    nuclidename = button[i].Text;
                    break;
                }
            }
            
            #endregion

            #region α参数
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//更新α参数
            IList<HFM.Components.EfficiencyParameter> efficiencyParameters = new List<HFM.Components.EfficiencyParameter>();//更新效率
            for (int i = 0; i < DgvAlphaSet.RowCount; i++)
            {
                ProbeParameter p = new ProbeParameter();
                HFM.Components.EfficiencyParameter efficiency = new HFM.Components.EfficiencyParameter();
                efficiency.Channel = new Channel().GetChannel(DgvAlphaSet.Rows[i].Cells[0].Value.ToString());
                efficiency.NuclideType = "α";
                efficiency.NuclideName = nuclidename;
                efficiency.Efficiency = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[5].Value);
                efficiencyParameters.Add(efficiency);

                p.ProbeChannel = new Channel().GetChannel(DgvAlphaSet.Rows[i].Cells[0].Value.ToString());
                p.NuclideType = "α";
                p.ProbeType = "闪烁体";
                p.HBackground = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[1].Value);
                p.LBackground = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[2].Value);
                p.Alarm_1 = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[3].Value);
                p.Alarm_2 = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[4].Value);
                p.Efficiency = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[5].Value);
                probeParameters.Add(p);
            }
            #endregion

            #region 更新数据库
            for (int i = 0; i < probeParameters.Count; i++)
            {
                bool k = new ProbeParameter().SetParameter(probeParameters[i]);
                bool l = new HFM.Components.EfficiencyParameter().SetParameter(efficiencyParameters[i]);
                if (k && l)
                {
                }
                else
                {
                    MessageBox.Show("更新失败");
                    return;
                }
            }
            if (new Nuclide().SetAlphaNuclideUser(nuclidename))
            {
                MessageBox.Show("更新成功");
            }
            else
            {
                MessageBox.Show("更新失败");
            }
            #endregion
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAlphaNo_Click(object sender, EventArgs e)
        {
            //重新获得数据库数据
            GetAlphaData();
        }


        #endregion

        #region β参数界面
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBetaOk_Click(object sender, EventArgs e)
        {
            #region β核素选择
            string nuclidename = "";//修改核素选择
            IList<RadioButton> button = new List<RadioButton>();//核素选择数组
            button.Add(RdoBeta14);
            button.Add(RdoBeta58);
            button.Add(RdoBeta131);
            button.Add(RdoBeta204);
            button.Add(RdoBeta32);
            button.Add(RdoBeta60);
            button.Add(RdoBeta137);
            button.Add(RdoBetaDefine1);
            button.Add(RdoBeta36);
            button.Add(RdoBeta90);
            button.Add(RdoBeta192);
            button.Add(RdoBetaDefine2);
            for (int i = 0; i < button.Count; i++)
            {
                if (button[i].Checked)
                {
                    nuclidename = button[i].Text;
                    break;
                }
            }
            #endregion

            #region β参数
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//更新β参数
            IList<HFM.Components.EfficiencyParameter> efficiencyParameters = new List<HFM.Components.EfficiencyParameter>();//更新效率
            for (int i = 0; i < DgvBetaSet.RowCount; i++)
            {
                ProbeParameter p = new ProbeParameter();
                HFM.Components.EfficiencyParameter efficiency = new HFM.Components.EfficiencyParameter();
                efficiency.Channel = new Channel().GetChannel(DgvBetaSet.Rows[i].Cells[0].Value.ToString());
                efficiency.NuclideType = "β";
                efficiency.NuclideName = nuclidename;
                efficiency.Efficiency = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[5].Value);
                efficiencyParameters.Add(efficiency);

                p.ProbeChannel = new Channel().GetChannel(DgvBetaSet.Rows[i].Cells[0].Value.ToString());
                p.NuclideType = "β";
                p.ProbeType = "闪烁体";
                p.HBackground = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[1].Value);
                p.LBackground = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[2].Value);
                p.Alarm_1 = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[3].Value);
                p.Alarm_2 = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[4].Value);
                p.Efficiency = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[5].Value);
                probeParameters.Add(p);
            }
            #endregion

            #region 更新数据库
            for (int i = 0; i < probeParameters.Count; i++)
            {
                bool k = new ProbeParameter().SetParameter(probeParameters[i]);
                bool l = new HFM.Components.EfficiencyParameter().SetParameter(efficiencyParameters[i]);
                if (k && l)
                {
                }
                else
                {
                    MessageBox.Show("更新失败");
                    return;
                }
            }
            if (new Nuclide().SetBetaNuclideUser(nuclidename))
            {
                MessageBox.Show("更新成功");
            }
            else
            {
                MessageBox.Show("更新失败");
            }
            #endregion
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBetaNo_Click(object sender, EventArgs e)
        {
            //重新获得数据库数据
            GetBetaData();
        }


        #endregion

        #region 衣物探头界面
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClothesOk_Click(object sender, EventArgs e)
        {
            //注：核算选择和衣物探头选择顺序不可互换
            #region 核素选择
            string nuclidename = "";//修改核素选择
            IList<RadioButton> button = new List<RadioButton>();//核素选择数组

            #region 添加核素
            //0-4为α核素，5-15为β核素
            //α核素
            button.Add(RdoClothesAlpha235);
            button.Add(RdoClothesAlpha238);
            button.Add(RdoClothesAlpha239);
            button.Add(RdoClothesAlpha241);
            button.Add(RdoClothesAlphaDefine1);
            //β核素
            button.Add(RdoClothesBeta14);
            button.Add(RdoClothesBeta32);
            button.Add(RdoClothesBeta36);
            button.Add(RdoClothesBeta58);
            button.Add(RdoClothesBeta60);
            button.Add(RdoClothesBeta90);
            button.Add(RdoClothesBeta131);
            button.Add(RdoClothesBeta137);
            button.Add(RdoClothesBeta192);
            button.Add(RdoClothesBeta204);
            button.Add(RdoClothesBetaDefine1);

            #endregion

            //获得当前核素，同时记录核素编号
            int number = 0;
            for (int i = 0; i < button.Count; i++)
            {
                if (button[i].Checked)
                {
                    nuclidename = button[i].Text;
                    number = i;
                    break;
                }
            }
            #endregion

            #region 衣物探头
            //衣物离线自检时间数据在SystemParameter中，故需要单独存储
            Components.SystemParameter systemParameter = new Components.SystemParameter();
            systemParameter.GetParameter();//或当当前数据
            systemParameter.ClothOfflineTime = Convert.ToInt32(TxtClothOfflineTime.Text);

            ProbeParameter probeParameter = new ProbeParameter();//更新衣物参数
            Components.EfficiencyParameter effciency = new Components.EfficiencyParameter(); //更新效率
            effciency.Channel = new Channel().GetChannel(7);
            effciency.NuclideName = nuclidename;
            effciency.Efficiency = Convert.ToSingle(TxtClothesEfficiency.Text);
            //0-4为α核素，5-15为β核素
            if (number < 5 && number > 0)
            {
                effciency.NuclideType = "C";
            }
            else
            {
                effciency.NuclideType = "C";
            }

            probeParameter.ProbeChannel = effciency.Channel;
            probeParameter.NuclideType = effciency.NuclideType;
            probeParameter.ProbeType = "GM管";
            probeParameter.HBackground = Convert.ToSingle(TxtClothesHBackground.Text);
            probeParameter.LBackground = Convert.ToSingle(TxtClothesLBackground.Text);
            probeParameter.Alarm_1 = Convert.ToSingle(TxtClothesAlarm_1.Text);
            probeParameter.Alarm_2 = Convert.ToSingle(TxtClothesAlarm_2.Text);
            #endregion

            #region 更新数据库
            if (new Nuclide().SetClothesNuclideUser(nuclidename) && new Components.EfficiencyParameter().SetParameter(effciency) && new ProbeParameter().SetParameter(probeParameter))
            {
                MessageBox.Show("更新成功");
            }
            else
            {
                MessageBox.Show("更新失败");
            }
            #endregion
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClothesNo_Click(object sender, EventArgs e)
        {
            //重新获得数据库数据
            GetClothesData();
        }

        #endregion

        #region 道盒参数界面
        /// <summary>
        /// 恢复默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceRetuen_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 设置默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceSet_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 读参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceRead_Click(object sender, EventArgs e)
        {
                    
        }
        /// <summary>
        /// 写参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceWrite_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 设备设置界面
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFacilityOk_Click(object sender, EventArgs e)
        {
            //获得是否启用
            bool hand,foot,clothes,two = false;//(手部、脚部、衣物、双手)

            #region 手部启用
            //手部启用
            if (ChkHand.Checked)
            {
                hand = true;
                //启用双探头
                if (RdoSingleHand.Checked)
                {
                    two = false;
                }
                else
                {
                    two = true;
                }
            }
            else
            {
                hand = false;
            }
            #endregion

            #region 脚步启用

            if (ChkFoot.Checked)
            {
                foot = true;
            }
            else
            {
                foot = false;
            }

            #endregion

            #region 衣物启用

            if(ChkClothes.Checked)
            {
                clothes = true;
            }
            else
            {
                clothes = false;
            }

            #endregion

            #region 写入数据库

            Channel channel = new Channel();

            //手部启用
            if (hand)
            {
                //双手启用
                if (two)
                {
                    channel.SetEnabledByID(1, true);
                    channel.SetEnabledByID(2, true);
                    channel.SetEnabledByID(3, true);
                    channel.SetEnabledByID(4, true);
                }
                //单手启用
                else
                {
                    channel.SetEnabledByID(1, true);
                    channel.SetEnabledByID(2, false);
                    channel.SetEnabledByID(3, true);
                    channel.SetEnabledByID(4, false);
                }
            }
            else
            {
                channel.SetEnabledByID(1, false);
                channel.SetEnabledByID(2, false);
                channel.SetEnabledByID(3, false);
                channel.SetEnabledByID(4, false);
            }

            #endregion

        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFacilityNo_Click(object sender, EventArgs e)
        {
            //重新获得数据库数据
            GetFacilityData();
        }
        #endregion

        #endregion


    }
}

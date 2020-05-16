/**
 * ________________________________________________________________________________ 
 *
 *  描述：参数设置窗体
 *  作者：杨旭锴 邢家宁
 *  版本：
 *  创建时间：2020年2月25日
 *  类名：参数设置
 *  更新:杨旭锴 数字键盘添加,2020年4月1日
 *  更新:杨旭锴 优化更新 2020年4月5日
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
using System.Configuration;
using HFM.Components;
using System.Threading;
using Message = HFM.Components.Message;


namespace HFM
{
    public partial class FrmPreference : Form
    {
        public FrmPreference()
        {
            InitializeComponent();
        }

        #region 实例字段
        //运行参数设置
        private ChannelParameter channelParameter = new ChannelParameter();//道盒信息(α阈值等)
        private HFM.Components.SystemParameter system = new HFM.Components.SystemParameter();//(自检时间、单位等)
        private FactoryParameter factoryParameter = new FactoryParameter();//仪器设备信息(IP地址、软件名称、是否双手探测器等)
        private ProbeParameter probeParameter = new ProbeParameter();//系统参数(各类型的本底上限等参数)
        private Nuclide nuclide = new Nuclide();//核素选择(U_235等)
        private EfficiencyParameter efficiencyParameter = new EfficiencyParameter();//探测效率(各类型的探测效率)
        private IList<MeasureData> measureDataS = new List<MeasureData>();//解析报文
        private int bkworkTime = 0;// 异步线程初始化化时间,ReportProgress百分比数值
        private Tools _tools = new Tools();//工具类
        /// <summary>
        /// 异步线程初始化化时间,ReportProgress百分比数值
        /// </summary>
        private int _bkworkTime;
        /// <summary>
        /// 串口实例
        /// </summary>
        private CommPort _commPort = new CommPort();
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool _isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        /// <summary>
        /// 当前发送报文的类型
        /// </summary>
        private MessageType _messageType;

        
        /// <summary>
        /// 读取数据
        /// </summary>
        private IList<ChannelParameter> _channelParameters = new List<ChannelParameter>();
        /// <summary>
        /// 获取全部通道信息
        /// </summary>
        private IList<Channel> Channels = new Channel().GetChannel();
        //通讯类型
        enum MessageType
        {
            PSet,// P写入类型
            PRead,// P读取类型
        }
        private MessageType messageType;


        #endregion

        #region 端口字符串创建
        /// <summary>
        /// 端口字符串创建
        /// </summary>
        /// <param name="comportSet">0:commportSet   1:commportSetOfReport</param>
        /// <param name="portNum">端口</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        /// <param name="parity">校验</param>
        /// <returns></returns>
        private string StringComport(int comportSet, string portNum, string baudRate, string dataBits, string stopBits, string parity,string isEnabled)
        {
            isEnabled=isEnabled == "是" ? "true" : "false";

            if (comportSet == 1)
            {
                return $"PortNum={portNum};BaudRate={baudRate};DataBits={dataBits};Parity={parity};StopBits={stopBits};IsEnabled={isEnabled}";
            }
            else
            {
                return $"PortNum={portNum};BaudRate={baudRate};DataBits={dataBits};Parity={parity};StopBits={stopBits}";
            }

            
        }
        #endregion
        
        #region 加载界面
        /// <summary>
        /// 加载参数(初始化)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPreference_Load(object sender, EventArgs e)
        {
            OpenPort();
            //线程支持异步取消
            backgroundWorker_Preference.WorkerSupportsCancellation = true;
            //线程支持报告进度
            backgroundWorker_Preference.WorkerReportsProgress = true;
            GetProferenceData();
            //权限判断
            if (User.LandingUser.Role != 1)
            {
                switch (factoryParameter.GetParameter().MeasureType)
                {
                    case "α":
                        GrpPresence.Visible = false;
                        GrpFacilityData.Visible = false;
                        TabPresence.TabPages[2].Parent = null;
                        TabPresence.TabPages[3].Parent = null;
                        TabPresence.TabPages[3].Parent = null;
                        break;
                    case "β":
                        GrpPresence.Visible = false;
                        GrpFacilityData.Visible = false;
                        TabPresence.TabPages[1].Parent = null;
                        TabPresence.TabPages[3].Parent = null;
                        TabPresence.TabPages[3].Parent = null;
                        break;
                    case "α/β":
                        GrpPresence.Visible = false;
                        GrpFacilityData.Visible = false;
                        //TabPresence.TabPages[1].Parent = null;
                        TabPresence.TabPages[4].Parent = null;
                        TabPresence.TabPages[4].Parent = null;
                        break;
                    default:
                        break;
                }
            }

        } 
        #endregion

        #region 页面切换
        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabPresence_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (User.LandingUser.Role != 1)
            {
                switch (factoryParameter.GetParameter().MeasureType)
                {
                    case "α":
                        switch (TabPresence.SelectedIndex)
                        {
                            case 0:
                                GetProferenceData();
                                break;
                            case 1:
                                GetAlphaData();
                                break;
                            case 2:
                                GetClothesData();
                                break;
                        }
                        break;
                    case "β":
                        switch (TabPresence.SelectedIndex)
                        {
                            case 0:
                                GetProferenceData();
                                break;
                            case 1:
                                GetBetaData();
                                break;
                            case 2:
                                GetClothesData();
                                break;
                        }
                        break;
                    case "α/β":
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
                        }
                        break;
                    default:
                        break;
                }

            }
            else
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
                        GetPortConfiguration();
                        break;
                    default:
                        MessageBox.Show("选择有误，请重新选择");
                        break;
                }

            }

        } 
        #endregion

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
            //平滑因子
            TxtSmoothingFactor.Text = factoryParameter.SmoothingFactor.ToString();
            //仪器编号
            TxtInstrumentNum.Text = factoryParameter.InstrumentNum.ToString();
            //软件名称
            CmbSoftName.Text = factoryParameter.SoftName;
            //探测类型
            CmbUnclideType.Text = factoryParameter.MeasureType;


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
                    //根据channelID来修改数据
                    ((TextBox)a[probeParameters[i].ProbeChannel.ChannelID - 1]).Enabled = true;
                    ((Label)label[probeParameters[i].ProbeChannel.ChannelID - 1]).Enabled = true;
                    
                }
                else
                {
                    //根据channelID来修改数据
                    ((TextBox)a[probeParameters[i].ProbeChannel.ChannelID - 1]).Enabled = false;
                    ((Label)label[probeParameters[i].ProbeChannel.ChannelID - 1]).Enabled = false;
                }
            }
            //未启用则数据修改只能对左手进行
            if (factoryParameter.IsDoubleProbe!=true)
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

            #region 设备配置

            IList<Channel> channellList = new Channel().GetChannel();
            //判断是否启用手部
            
            if (channellList[0].IsEnabled && channellList[1].IsEnabled)
            {
                ChkHand.Checked = true;//手部
                
            }
            else if (channellList[0].IsEnabled && channellList[1].IsEnabled==false)
            {
                ChkHand.Checked = true;//手部
                
            }
            //判断单双探测器
            if (factoryParameter.IsDoubleProbe==true)
            {
                RdoDoubleHand.Checked = true;//双探测器
            }
            else
            {
                RdoSingleHand.Checked = true;//单探测器
            }
            //else
            //{
            //    ChkHand.Checked = false;
            //    RdoSingleHand.Checked = false;
            //    RdoDoubleHand.Checked = false;
            //}
            //判断是否启用脚步
            if (channellList[4].IsEnabled &&channellList[5].IsEnabled)
            {
                ChkFoot.Checked = true;
            }
            else
            {
                ChkFoot.Checked = false;
            }
            //判断是否启用衣物探头
            if (channellList[6].IsEnabled)
            {
                ChkClothes.Checked = true;
            }
            else
            {
                ChkClothes.Checked = false;
            }

            #endregion

        }
        /// <summary>
        /// 获得α数据
        /// </summary>
        private void GetAlphaData()
        {

            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//获得α参数
            

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

            //按核素名称检索效率值
            var eff = efficiency.First(x => x.NuclideName == nowNuclideName).Efficiency;
            //获得所有Alpha系统参数(各类型的本底上限等参数)
            probeParameters = probeParameter.GetParameter("α");
            //把当前显示的效率值更换到该核素的效率值
            foreach (var item in probeParameters)
            {
                //根据通道id赋值当前效率值
                item.Efficiency = efficiency.First(x => x.Channel.ChannelID == item.ProbeChannel.ChannelID).Efficiency;
            }
                       
            //列表按id排序
            probeParameters = probeParameters.OrderBy(o => o.PreferenceID).ToList();

            #region α参数

            DgvAlphaSet.Rows.Clear();
            if (_isEnglish == true)
            {
                //污染警报标题加测量单位名称
                DgvBetaSet.Columns[3].HeaderText = $"Alarm Threshold({system.MeasurementUnit})";
                //高阶警报标题加测量单位名称
                DgvBetaSet.Columns[4].HeaderText = $"High Level Alarm({system.MeasurementUnit})";
            }
            else
            {
                //污染警报标题加测量单位名称
                DgvAlphaSet.Columns[3].HeaderText = $"污染警报({system.MeasurementUnit})";
                //高阶警报标题加测量单位名称
                DgvAlphaSet.Columns[4].HeaderText = $"高阶警报({system.MeasurementUnit})";
            }
            //选出启用的设备
            for (int i = 0; i < probeParameters.Count; i++)
            {
                //设备启用且核素类型为α并除去衣物参数
                if (probeParameters[i].ProbeChannel.IsEnabled && probeParameters[i].NuclideType == "α" && probeParameters[i].ProbeChannel.ChannelID != 7)
                {
                    int index = this.DgvAlphaSet.Rows.Add();
                    if (_isEnglish)
                    {
                        DgvAlphaSet.Rows[index].Cells[0].Value = probeParameters[i].ProbeChannel.ChannelName_English;
                    }
                    else
                    {
                        DgvAlphaSet.Rows[index].Cells[0].Value = probeParameters[i].ProbeChannel.ChannelName;
                    }
                    DgvAlphaSet.Rows[index].Cells[1].Value = probeParameters[i].HBackground;
                    DgvAlphaSet.Rows[index].Cells[2].Value = probeParameters[i].LBackground;

                    //污染警报根据系统测量参数中设定的测量单位显示数值
                    DgvAlphaSet.Rows[index].Cells[3].Value = Tools.UnitConvertCPSTo(probeParameters[i].Alarm_1, system.MeasurementUnit, efficiency[i].Efficiency, probeParameters[i].ProbeChannel.ProbeArea);
                    //高阶警报根据系统测量参数中设定的测量单位显示数值
                    DgvAlphaSet.Rows[index].Cells[4].Value = Tools.UnitConvertCPSTo(probeParameters[i].Alarm_2, system.MeasurementUnit, efficiency[i].Efficiency, probeParameters[i].ProbeChannel.ProbeArea);
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
            

            #region 核素选择

            //获得当前核素选择
            string nowNuclideName = nuclide.GetBetaNuclideUser();//获得当前β核素名称
            //IList<EfficiencyParameter> efficiency = new List<EfficiencyParameter>();
            IList<EfficiencyParameter> efficiency = new EfficiencyParameter().GetParameter();
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

            //按核素名称检索效率值
            //efficiency = efficiency.Where(x=>x.NuclideName==nowNuclideName).ToList;
            //获得所有beta系统参数(各类型的本底上限等参数)
            probeParameters = probeParameter.GetParameter("β");
            //把当前显示的效率值更换到该核素的效率值
            foreach (var item in probeParameters)
            {
                item.Efficiency = efficiency.First(x=>x.Channel.ChannelID==item.ProbeChannel.ChannelID).Efficiency;
            }
            //列表按id排序
            probeParameters = probeParameters.OrderBy(o => o.PreferenceID).ToList();
            #region β参数
            //清空列表
            DgvBetaSet.Rows.Clear();
            if (_isEnglish == true)
            {
                //污染警报标题加测量单位名称
                DgvBetaSet.Columns[3].HeaderText = $"Alarm Threshold({system.MeasurementUnit})";
                //高阶警报标题加测量单位名称
                DgvBetaSet.Columns[4].HeaderText = $"High Level Alarm({system.MeasurementUnit})";
            }
            else
            {
                //污染警报标题加测量单位名称
                DgvBetaSet.Columns[3].HeaderText = $"污染警报({system.MeasurementUnit})";
                //高阶警报标题加测量单位名称
                DgvBetaSet.Columns[4].HeaderText = $"高阶警报({system.MeasurementUnit})";
            }
            

            //选出启用的设备
            for (int i = 0; i < probeParameters.Count; i++)
            {
                //设备启用且核素类型为α并除去衣物参数
                if (probeParameters[i].ProbeChannel.IsEnabled && probeParameters[i].NuclideType == "β" && probeParameters[i].ProbeChannel.ChannelID != 7)
                {
                    int index = this.DgvBetaSet.Rows.Add();
                    if (_isEnglish)
                    {
                        DgvBetaSet.Rows[index].Cells[0].Value = probeParameters[i].ProbeChannel.ChannelName_English;
                    }
                    else
                    {
                        DgvBetaSet.Rows[index].Cells[0].Value = probeParameters[i].ProbeChannel.ChannelName;
                    }
                    DgvBetaSet.Rows[index].Cells[1].Value = probeParameters[i].HBackground;
                    DgvBetaSet.Rows[index].Cells[2].Value = probeParameters[i].LBackground;
                    DgvBetaSet.Rows[index].Cells[3].Value = Tools.UnitConvertCPSTo(probeParameters[i].Alarm_1, system.MeasurementUnit, efficiency[i].Efficiency, probeParameters[i].ProbeChannel.ProbeArea);
                    DgvBetaSet.Rows[index].Cells[4].Value = Tools.UnitConvertCPSTo(probeParameters[i].Alarm_2, system.MeasurementUnit, efficiency[i].Efficiency, probeParameters[i].ProbeChannel.ProbeArea);
                    DgvBetaSet.Rows[index].Cells[5].Value = probeParameters[i].Efficiency;
                }
                //设备未启用(暂时不显示)
                else
                {
                }
            }
            #endregion

        }
        /// <summary>
        /// 获得衣物参数
        /// </summary>
        private void GetClothesData()
        {
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//获得C参数
            probeParameters = probeParameter.GetParameter("c");

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

            ////把当前所选核素效率保存到ProbeParameter当前效率数据库中
            //for (int i = 0; i < efficiency.Count; i++)
            //{
            //    //根据channelID来匹配

            //    if (probeParameters[0].ProbeChannel.ChannelID == efficiency[i].Channel.ChannelID)
            //    {
            //        probeParameters[0].Efficiency = efficiency[i].Efficiency;//把得到效率传送给当前效率
            //        probeParameter.SetParameter(probeParameters[0]);//保存到数据库
            //        probeParameter = probeParameters[0];
            //    }

            //}
            //按核素名称检索效率值
            var eff = efficiency.First(x => x.NuclideName == nowNuclideName).Efficiency;
            //获得所有beta系统参数(各类型的本底上限等参数)
            probeParameters = probeParameter.GetParameter("c");
            //把当前显示的效率值更换到该核素的效率值
            foreach (var item in probeParameters)
            {
                item.Efficiency = eff;
            }
            #region 衣物探头

            //system = system.GetParameter();//获得衣物离线自检时间
            //TxtClothesHBackground.Text = probeParameter.HBackground.ToString();
            //TxtClothesLBackground.Text = probeParameter.LBackground.ToString();
            //TxtClothesAlarm_1.Text = probeParameter.Alarm_1.ToString();
            //TxtClothesAlarm_2.Text = probeParameter.Alarm_2.ToString();
            //TxtClothesEfficiency.Text = probeParameter.Efficiency.ToString();
            //TxtClothOfflineTime.Text = system.ClothOfflineTime.ToString();
            system = system.GetParameter();//获得衣物离线自检时间
            TxtClothesHBackground.Text = probeParameters[0].HBackground.ToString();
            TxtClothesLBackground.Text = probeParameters[0].LBackground.ToString();
            TxtClothesAlarm_1.Text = probeParameters[0].Alarm_1.ToString();
            TxtClothesAlarm_2.Text = probeParameters[0].Alarm_2.ToString();
            TxtClothesEfficiency.Text = probeParameters[0].Efficiency.ToString();
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
            //DgvMainPreferenceSet.Columns[0].DataPropertyName = Channel.ChannelName;
            //DgvMainPreferenceSet.AutoGenerateColumns = false;
            //DgvMainPreferenceSet.DataSource = channelParameters;


            DgvMainPreferenceSet.Rows.Clear();
            //选出所有设备
            for (int i = 0; i < channelParameters.Count; i++)
            {
                int index = this.DgvMainPreferenceSet.Rows.Add();
                if (_isEnglish)
                {
                    DgvMainPreferenceSet.Rows[index].Cells[0].Value = channelParameters[i].Channel.ChannelName_English;
                }
                else
                {
                    DgvMainPreferenceSet.Rows[index].Cells[0].Value = channelParameters[i].Channel.ChannelName;
                }
                DgvMainPreferenceSet.Rows[index].Cells[1].Value = channelParameters[i].AlphaThreshold;
                DgvMainPreferenceSet.Rows[index].Cells[2].Value = channelParameters[i].BetaThreshold;
                DgvMainPreferenceSet.Rows[index].Cells[3].Value = channelParameters[i].PresetHV;
                DgvMainPreferenceSet.Rows[index].Cells[4].Value = channelParameters[i].ADCFactor;
                DgvMainPreferenceSet.Rows[index].Cells[5].Value = channelParameters[i].DACFactor;
                DgvMainPreferenceSet.Rows[index].Cells[6].Value = channelParameters[i].HVFactor;
                DgvMainPreferenceSet.Rows[index].Cells[7].Value = channelParameters[i].WorkTime;
                DgvMainPreferenceSet.Rows[index].Cells[8].Value = channelParameters[i].HVRatio;
            }

        }
        /// <summary>
        /// 获得端口配置参数(数据库,配置文件)
        /// </summary>
        private void GetPortConfiguration()
        {
            #region 网络配置
            //查询数据库工厂参数(网络配置信息在工厂参数中)
            factoryParameter.GetParameter();
            //IP地址
            string[] k = factoryParameter.IpAddress.Split('.');//分割地址
            TxtIPAddressOne.Text = k[0];
            TxtIPAddressTwo.Text = k[1];
            TxtIPAddressThree.Text = k[2];
            TxtIPAddressFour.Text = k[3];
            //通信端口
            TxtPortNumber.Text = factoryParameter.PortNumber;
            //是否自动连接
            ChkIsConnectedAuto.Checked = factoryParameter.IsConnectedAuto;
            //设备地址
            TxtDeviceAddress.Text = factoryParameter.DeviceAddress;
            //上报时间间隔
            TxtReportingTime.Text = factoryParameter.ReportingTime;
            #endregion

            #region 端口配置
            CommPort commPort = new CommPort();
            string[] commportSet=new string[6];
            commportSet = commPort.GetCommPortSetForParameter("commportSet");
            TxtcommportSetPortNum.Text = commportSet[0];
            TxtcommportSetBaudRate.Text = commportSet[1];
            TxtcommportSetDataBits.Text = commportSet[2];
            TxtcommportSetStopBits.Text= commportSet[4];
            TxtcommportSetParity.Text = commportSet[3];
            commportSet=commPort.GetCommPortSetForParameter("commportSetOfReport");
            TxtcommportSetOfReportPortNum.Text = commportSet[0];
            TxtcommportSetOfReportBaudRate.Text = commportSet[1];
            TxtcommportSetOfReportDataBits.Text = commportSet[2];
            TxtcommportSetOfReportStopBits.Text = commportSet[4];
            TxtcommportSetOfReportParity.Text = commportSet[3];
            CmbIsEnabled.SelectedIndex = commportSet[5]=="true"?0:1;


            #endregion
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
        private byte[] ReadDataFromSerialPort(BackgroundWorker bkWorker, DoWorkEventArgs e)
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

                switch (_messageType)
                {
                    #region P读取指令下发并接收数据上传
                    case MessageType.PRead:

                        //向下位机下发“p”指令码
                        buffMessage[0] = Convert.ToByte('P');
                        //buffMessage[61] = Convert.ToByte(0);
                        _bkworkTime++;
                        if (_bkworkTime > 3)
                        {
                            backgroundWorker_Preference.CancelAsync();
                            _bkworkTime = 0;
                            if (_isEnglish)
                            {
                                MessageBox.Show("Read over.");
                            }
                            else
                            {
                                MessageBox.Show("读取完成.");
                            }
                            break;
                        }
                        if (Message.SendMessage(buffMessage, _commPort))    //正式
                        {

                            //延时
                            Thread.Sleep(100);
                            receiveBuffMessage = Message.ReceiveMessage(_commPort);
                            //延时
                            Thread.Sleep(200);
                            //触发向主线程返回下位机上传数据事件
                            bkWorker.ReportProgress(_bkworkTime, receiveBuffMessage);
                        }
                        else
                        {
                            errorNumber++;
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            if (errorNumber > 5)
                            {
                                bkWorker.ReportProgress(1, null);
                                backgroundWorker_Preference.CancelAsync();
                            }
                            else
                            {
                                Thread.Sleep(delayTime);
                            }
                        }
                        break;
                    #endregion

                    #region P写入指令下发
                    case MessageType.PSet:
                        //道盒1-4通道解析原数据
                        IList<ChannelParameter> _first_setChannelP = new List<ChannelParameter>();
                        //道盒5-7通道解析原数据
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
                        //道盒5-7通道列表加一个空对象,(3个对象解析会报错,必须四个为一组解析)
                        _second_setChanelP.Add(_first_setChannelP[0]);

                        // _second_setChanelP.RemoveAt(3);
                        // _second_setChanelP.Add(_setChannelParameter);
                        //生成报文
                        //道盒1-4通道解析完成的数据
                        byte[] buffMessagePset1 = Message.BuildMessage(_first_setChannelP);
                        //道盒5-7通道解析完成的数据,
                        byte[] buffMessagePset2 = Message.BuildMessage(_second_setChanelP);
                        //成功则关闭线程
                        try
                        {
                            if (Message.SendMessage(buffMessagePset1, _commPort) && Message.SendMessage(buffMessagePset2, _commPort))
                            {
                                //写入成功,返回p指令读取当前高压以确认更改成功
                                backgroundWorker_Preference.CancelAsync();
                                if (_isEnglish)
                                {
                                    MessageBox.Show("Data has been distributed!", "Message");
                                }
                                else
                                {
                                    MessageBox.Show("数据已经下发!", "提示");
                                }
                            }
                            //发送失败次数大于5次,提示错误并挂起线程
                            else
                            {
                                errorNumber++;
                                if (errorNumber > 5)
                                {
                                    _tools.PrompMessage(2);
                                    backgroundWorker_Preference.CancelAsync();
                                }
                                Thread.Sleep(200);

                            }
                        }
                        catch
                        {
                            MessageBox.Show("设置失败,请重新尝试!");
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
            //接收报文无误，进行报文解析，并将解析后的道盒数据存储到channelParameters中 
            try
            {
                if (receiveBufferMessage[0] == Convert.ToByte('P'))
                {
                    DgvMainPreferenceSet.Rows.Clear();
                    //解析报文
                    _channelParameters = HFM.Components.Message.ExplainMessage<ChannelParameter>(receiveBufferMessage);
                    if (_channelParameters.Count==8)
                    {
                        _channelParameters.RemoveAt(7);
                    }
                    foreach (var itemParameter in _channelParameters)
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
                    //DgvMainPreferenceSet.AutoGenerateColumns = false;
                    //DgvMainPreferenceSet.DataSource = _channelParameters;
                }

            }
            catch (Exception EX_NAME)
            {
                Tools.ErrorLog(EX_NAME.ToString());
                throw;
            }
        }
        /// <summary>
        /// 开启串口封装的方法
        /// </summary>
        private void OpenPort()
        {
            //从配置文件获得当前串口配置
            if (_commPort.Opened)
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
                _tools.PrompMessage(1);

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
            HFM.Components.SystemParameter system = new HFM.Components.SystemParameter().GetParameter();
            //system = system.GetParameter();
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
            FactoryParameter factoryParameterBtn = new FactoryParameter().GetParameter();//获得仪器设备信息参数
            factoryParameterBtn.SmoothingFactor = int.Parse(TxtSmoothingFactor.Text);
            factoryParameterBtn.InstrumentNum = TxtInstrumentNum.Text;
            factoryParameterBtn.SoftName = CmbSoftName.Text;
            
            factoryParameterBtn.MeasureType = CmbUnclideType.Text;
            
            #endregion

            #region 设备配置
            //获得是否启用
            Channel channelBtnOk = new Channel();
            #region 手部启用
            //手部启用
            if (ChkHand.Checked)
            {
                channelBtnOk.SetEnabledByType(0, true);
            }
            else
            {
                channelBtnOk.SetEnabledByType(0, false);
            }
            //启用单双探测器
            if (RdoSingleHand.Checked)
            {
                //channelBtnOk.SetEnabledByID(1, true);
                //channelBtnOk.SetEnabledByID(2, false);
                //channelBtnOk.SetEnabledByID(3, true);
                //channelBtnOk.SetEnabledByID(4, false);
                //单探测器
                factoryParameterBtn.IsDoubleProbe = false;
            }
            else if (RdoDoubleHand.Checked)
            {
                ////根据类型全部启用首部探测器
                //channelBtnOk.SetEnabledByType(0, true);
                //双探测器
                factoryParameterBtn.IsDoubleProbe = true;

            }
            #endregion

            #region 脚步启用

            if (ChkFoot.Checked)
            {
                channelBtnOk.SetEnabledByType(1, true);
            }
            else
            {
                channelBtnOk.SetEnabledByType(1, false);
            }

            #endregion

            #region 衣物启用

            if (ChkClothes.Checked)
            {
                channelBtnOk.SetEnabledByID(7, true);
            }
            else
            {
                channelBtnOk.SetEnabledByID(7, false);
            }

            #endregion
            
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
            if (system.SetParameter(system) && factoryParameterBtn.SetParameter(factoryParameterBtn))
            {
                if (_isEnglish)
                {
                    MessageBox.Show("Update successful");
                }
                else
                {
                    MessageBox.Show("更新成功");
                }
                
            }
            else
            {
                if (_isEnglish)
                {
                    MessageBox.Show("Update failed");
                }
                else
                {
                    MessageBox.Show("更新失败");
                }
                
                return;
            }

            #endregion

            GetProferenceData();
            
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
            int setDataCount = 0;//更新成功的信息条数
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//更新α参数
            IList<EfficiencyParameter> efficiencyParameters = new List<HFM.Components.EfficiencyParameter>();//更新效率
            ProbeParameter p = new ProbeParameter();
            EfficiencyParameter efficiency = new EfficiencyParameter();
            for (int i = 0; i < DgvAlphaSet.RowCount; i++)
            {
                float alarm_1 = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[3].Value);//污染警报
                float alarm_2 = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[4].Value);//高阶警报


                //efficiency.Channel = new Channel().GetChannel(DgvAlphaSet.Rows[i].Cells[0].Value.ToString());
                efficiency.Channel = Channels.First(x => x.ChannelName == DgvAlphaSet.Rows[i].Cells[0].Value.ToString());
                efficiency.NuclideType = "α";
                efficiency.NuclideName = nuclidename;
                efficiency.Efficiency = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[5].Value);
                //efficiencyParameters.Add(efficiency);

                //p.ProbeChannel = new Channel().GetChannel(DgvAlphaSet.Rows[i].Cells[0].Value.ToString());
                p.ProbeChannel= Channels.First(x => x.ChannelName == DgvAlphaSet.Rows[i].Cells[0].Value.ToString());
                p.NuclideType = "α";
                p.ProbeType = "闪烁体";
                p.HBackground = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[1].Value);
                p.LBackground = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[2].Value);
                //按测量单位转换成cps
                p.Alarm_1 = Tools.UnitConvertToCPS(alarm_1, system.MeasurementUnit, efficiency.Efficiency,
                    p.ProbeChannel.ProbeArea);
                //按测量单位转换成cps
                p.Alarm_2 = Tools.UnitConvertToCPS(alarm_2, system.MeasurementUnit, efficiency.Efficiency,
                    p.ProbeChannel.ProbeArea);
                p.Efficiency = Convert.ToSingle(DgvAlphaSet.Rows[i].Cells[5].Value);
                //probeParameters.Add(p);
                if (efficiency.SetParameter(efficiency)==true && p.SetParameter(p)==true)
                {
                    setDataCount++;
                }
            }
            #endregion

            #region 更新数据库
            
            bool isUpDatanuclidename = false;//是否更新成功核素名称
            string upDate="";
            
            if (nuclide.SetAlphaNuclideUser(nuclidename))
            {
                isUpDatanuclidename = true; //更新成功核素名称
                if (isUpDatanuclidename==true)
                {
                    if (_isEnglish)
                    {
                        upDate = $"Currently selected nuclides are:{nuclidename}";
                    }
                    else
                    {
                        upDate = $"当前选择核素为:{nuclidename}";
                    }
                }
            }
            if (_isEnglish)
            {
                MessageBox.Show($"{setDataCount} update completed {upDate}");
            }
            else
            {
                MessageBox.Show($"更新完成{setDataCount}条信息 {upDate}");
            }
            #endregion

            #region 更新数据后重新读取数据
            GetAlphaData();
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
            int setDataCount = 0;//更新成功的信息条数
            IList<ProbeParameter> probeParameters = new List<ProbeParameter>();//更新β参数
            IList<HFM.Components.EfficiencyParameter> efficiencyParameters = new List<HFM.Components.EfficiencyParameter>();//更新效率
            ProbeParameter p = new ProbeParameter();
            HFM.Components.EfficiencyParameter efficiency = new HFM.Components.EfficiencyParameter();
            for (int i = 0; i < DgvBetaSet.RowCount; i++)
            {
                float alarm_1 = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[3].Value);//污染警报
                float alarm_2 = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[4].Value);//高阶警报
                var listChannels= Channels.First(x => x.ChannelName == DgvBetaSet.Rows[i].Cells[0].Value.ToString()|| x.ChannelName_English== DgvBetaSet.Rows[i].Cells[0].Value.ToString());
                //efficiency.Channel = new Channel().GetChannel(DgvBetaSet.Rows[i].Cells[0].Value.ToString());
                efficiency.Channel = listChannels;
                efficiency.NuclideType = "β";
                efficiency.NuclideName = nuclidename;
                efficiency.Efficiency = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[5].Value);
                //efficiencyParameters.Add(efficiency);

                //p.ProbeChannel = new Channel().GetChannel(DgvBetaSet.Rows[i].Cells[0].Value.ToString());
                p.ProbeChannel = listChannels;
                p.NuclideType = "β";
                p.ProbeType = "闪烁体";
                p.HBackground = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[1].Value);
                p.LBackground = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[2].Value);
                //按测量单位转换成cps
                p.Alarm_1 = Tools.UnitConvertToCPS(alarm_1, system.MeasurementUnit, efficiency.Efficiency,p.ProbeChannel.ProbeArea);
                //按测量单位转换成cps
                p.Alarm_2 = Tools.UnitConvertToCPS(alarm_2, system.MeasurementUnit, efficiency.Efficiency,p.ProbeChannel.ProbeArea);
                p.Efficiency = Convert.ToSingle(DgvBetaSet.Rows[i].Cells[5].Value);
                //probeParameters.Add(p);
                if (efficiency.SetParameter(efficiency)&&p.SetParameter(p))
                {
                    setDataCount++;
                }
            }
            #endregion

            #region 更新数据库
           
            
            bool isUpDatanuclidename = false;//是否更新成功核素名称
            string upDate = "";
            
            if (nuclide.SetBetaNuclideUser(nuclidename))
            {
                isUpDatanuclidename = true; //更新成功核素名称
                if (isUpDatanuclidename == true)
                {
                    if (_isEnglish)
                    {
                        upDate = $"Currently selected nuclides are:{nuclidename}";
                    }
                    else
                    {
                        upDate = $"当前选择核素为:{nuclidename}";
                    }
                }
            }
            if (_isEnglish)
            {
                MessageBox.Show($"{setDataCount} update completed {upDate}");
            }
            else
            {
                MessageBox.Show($"更新完成{setDataCount}条信息 {upDate}");
            }
            #endregion

            #region 更新数据后重新读取数据

            GetBetaData();

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
            systemParameter.SetParameter(systemParameter);
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
            if (nuclide.SetClothesNuclideUser(nuclidename) && efficiencyParameter.SetParameter(effciency) && probeParameter.SetParameter(probeParameter))
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
            //重新获得数据库数据
            GetMainProferenceData();
            MessageBox.Show("恢复成功");
        }
        /// <summary>
        /// 设置默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceSet_Click(object sender, EventArgs e)
        {
            #region 获得数据

            IList<ChannelParameter> channelParameters = new List<ChannelParameter>();//更新道盒参数
            //循环每一行
            for (int i = 0; i < DgvMainPreferenceSet.RowCount; i++) 
            {
                ChannelParameter channelParameter = new ChannelParameter();
                channelParameter.Channel = new Channel();
                channelParameter.Channel.ChannelName = Convert.ToString(DgvMainPreferenceSet.Rows[i].Cells[0].Value);
                channelParameter.AlphaThreshold = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[1].Value);
                channelParameter.BetaThreshold = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[2].Value);
                channelParameter.PresetHV = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[3].Value);
                channelParameter.ADCFactor = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[4].Value);
                channelParameter.DACFactor = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[5].Value);
                channelParameter.HVFactor = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[6].Value);
                channelParameter.WorkTime = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[7].Value);
                channelParameter.HVRatio = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[8].Value);
                channelParameters.Add(channelParameter);
            }
            //获得channelID
            for (int i = 0; i < channelParameters.Count; i++)
            {
                if (channelParameters[i].Channel.ChannelName == "左手心")
                {
                    channelParameters[i].Channel.ChannelID = 1;
                }
                if (channelParameters[i].Channel.ChannelName == "左手背")
                {
                    channelParameters[i].Channel.ChannelID = 2;
                }
                if (channelParameters[i].Channel.ChannelName == "右手心")
                {
                    channelParameters[i].Channel.ChannelID = 3;
                }
                if (channelParameters[i].Channel.ChannelName == "右手背")
                {
                    channelParameters[i].Channel.ChannelID = 4;
                }
                if (channelParameters[i].Channel.ChannelName == "左脚")
                {
                    channelParameters[i].Channel.ChannelID = 5;
                }
                if (channelParameters[i].Channel.ChannelName == "右脚")
                {
                    channelParameters[i].Channel.ChannelID = 6;
                }
                if (channelParameters[i].Channel.ChannelName == "衣物探头")
                {
                    channelParameters[i].Channel.ChannelID = 7;
                }
            }

            #endregion

            #region 数据库更新

            for (int i = 0; i < channelParameters.Count; i++)
            {
                if(new ChannelParameter().SetParameter(channelParameters[i]))
                {
                }
                else
                {
                    MessageBox.Show("更新失败");
                    return;
                }
            }
            MessageBox.Show("更新成功");


            #endregion
        }
        /// <summary>
        /// 读参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceRead_Click(object sender, EventArgs e)
        {
            
            //当前发送报文类型换成p写入
            _messageType = MessageType.PRead;
            
            //判断串口是否打开
            if (_commPort.Opened == true)
            {
                if (backgroundWorker_Preference.IsBusy==true)
                {
                    backgroundWorker_Preference.CancelAsync();
                    Thread.Sleep(100);
                }
                //判断线程是否运行
                if (backgroundWorker_Preference.IsBusy == false)
                {
                    backgroundWorker_Preference.RunWorkerAsync();
                }
            }
            else
            {
                //错误提示
                _tools.PrompMessage(2);
                return;
            }
        }
        /// <summary>
        /// 写参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMainPreferenceWrite_Click(object sender, EventArgs e)
        {
            //_channelParameters.Clear();
            #region 读取数据到列表
            for (int i = 0; i < DgvMainPreferenceSet.RowCount; i++)
            {
                ChannelParameter channelParameter = new ChannelParameter();
                channelParameter.Channel = new Channel();
                channelParameter.Channel.ChannelName = Convert.ToString(DgvMainPreferenceSet.Rows[i].Cells[0].Value);
                channelParameter.AlphaThreshold = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[1].Value);
                channelParameter.BetaThreshold = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[2].Value);
                channelParameter.PresetHV = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[3].Value);
                channelParameter.ADCFactor = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[4].Value);
                channelParameter.DACFactor = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[5].Value);
                channelParameter.HVFactor = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[6].Value);
                channelParameter.WorkTime = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[7].Value);
                channelParameter.HVRatio = Convert.ToSingle(DgvMainPreferenceSet.Rows[i].Cells[8].Value);
                _channelParameters.Add(channelParameter);
            }
            //获得channelID
            for (int i = 0; i < _channelParameters.Count; i++)
            {
                if (_channelParameters[i].Channel.ChannelName == "左手心")
                {
                    _channelParameters[i].Channel.ChannelID = 1;
                }
                if (_channelParameters[i].Channel.ChannelName == "左手背")
                {
                    _channelParameters[i].Channel.ChannelID = 2;
                }
                if (_channelParameters[i].Channel.ChannelName == "右手心")
                {
                    _channelParameters[i].Channel.ChannelID = 3;
                }
                if (_channelParameters[i].Channel.ChannelName == "右手背")
                {
                    _channelParameters[i].Channel.ChannelID = 4;
                }
                if (_channelParameters[i].Channel.ChannelName == "左脚")
                {
                    _channelParameters[i].Channel.ChannelID = 5;
                }
                if (_channelParameters[i].Channel.ChannelName == "右脚")
                {
                    _channelParameters[i].Channel.ChannelID = 6;
                }
                if (_channelParameters[i].Channel.ChannelName == "衣物探头")
                {
                    _channelParameters[i].Channel.ChannelID = 7;
                }
            }
            #endregion

            //当前发送报文类型换成p写入
            _messageType = MessageType.PSet;

            //判断串口是否打开
            if (_commPort.Opened == true)
            {
                //判断线程是否运行
                if (backgroundWorker_Preference.IsBusy == false)
                {
                    backgroundWorker_Preference.RunWorkerAsync();
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

        #region 端口配置
        /// <summary>
        /// 端口配置确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPorSave_Click(object sender, EventArgs e)
        {
            string _commportSetString = StringComport(0, TxtcommportSetPortNum.Text, TxtcommportSetBaudRate.Text,
                TxtcommportSetDataBits.Text, TxtcommportSetStopBits.Text, TxtcommportSetParity.Text,"是");
            string _commportSetOfReportSetString = StringComport(1, TxtcommportSetOfReportPortNum.Text, TxtcommportSetOfReportBaudRate.Text,
                TxtcommportSetOfReportDataBits.Text, TxtcommportSetOfReportStopBits.Text, TxtcommportSetOfReportParity.Text,CmbIsEnabled.Text);
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["commportSet"].Value = _commportSetString;
            config.AppSettings.Settings["commportSetOfReport"].Value = _commportSetOfReportSetString;
            //保存
            config.Save();


            //网络保存
            FactoryParameter factoryParameterBtn = new FactoryParameter().GetParameter();//获得仪器设备信息参数
            //IP地址
            factoryParameterBtn.IpAddress = TxtIPAddressOne.Text + '.' + TxtIPAddressTwo.Text + '.'
                                            + TxtIPAddressThree.Text + '.' + TxtIPAddressFour.Text;
            //设备地址
            factoryParameterBtn.DeviceAddress = TxtDeviceAddress.Text;
            //通信端口
            factoryParameterBtn.PortNumber = TxtPortNumber.Text;
            //是否自动连接
            factoryParameterBtn.IsConnectedAuto = ChkIsConnectedAuto.Checked;
            //上报时间间隔
            factoryParameterBtn.ReportingTime = TxtReportingTime.Text;
            factoryParameterBtn.SetParameter(factoryParameterBtn);
            if (_isEnglish)
            {
                if (MessageBox.Show("Restart the program to apply the new configuration!", "Reminder", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    _commPort.Close();
                    if (backgroundWorker_Preference.IsBusy == true)
                    {
                        backgroundWorker_Preference.Dispose();
                    }
                    Application.Restart();
                }
            }
            else
            {
                if (MessageBox.Show("重新启动程序以应用新配置！", "提醒", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    _commPort.Close();
                    if (backgroundWorker_Preference.IsBusy == true)
                    {
                        backgroundWorker_Preference.Dispose();
                    }
                    Application.Restart();
                }
            }

        }
        /// <summary>
        /// 端口配置恢复配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPorRestoreDefault_Click(object sender, EventArgs e)
        {
            TxtcommportSetPortNum.Text = "COM5";
            TxtcommportSetBaudRate.Text = "115200";
            TxtcommportSetDataBits.Text = "8";
            TxtcommportSetStopBits.Text = "1";
            TxtcommportSetParity.Text = "无";
            
            TxtcommportSetOfReportPortNum.Text = "COM3";
            TxtcommportSetOfReportBaudRate.Text = "115200";
            TxtcommportSetOfReportDataBits.Text = "8";
            TxtcommportSetOfReportStopBits.Text = "1";
            TxtcommportSetOfReportParity.Text = "无";
            CmbIsEnabled.SelectedIndex = 0;
            //TxtcommportSet.Text = "PortNum=COM1;BaudRate=115200;DataBits=8;Parity=无;StopBits=1";
            //TxtcommportSetOfReport.Text = "PortNum=COM1;BaudRate=115200;DataBits=8;Parit=无;StopBits=1;IsEnabled=false";
        }
        /// <summary>
        /// 端口配置取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            GetPortConfiguration();
        }


        #endregion

        #endregion

        #region 数字键盘
        #region alpha,beta,道盒参数标签页Dgv,数字键盘
        /// <summary>
        /// alpha参数标签页,小键盘事件添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvAlphaSet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInDGV(DgvAlphaSet);
        }
        /// <summary>
        /// beta参数标签页,小键盘事件添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvBetaSet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInDGV(DgvBetaSet);
        }
        /// <summary>
        /// 道盒参数标签页,小键盘事件添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvMainPreferenceSet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FrmKeyIn.DelegatesKeyInDGV(DgvMainPreferenceSet);
        }
        #endregion

        #region 系统参数数字键盘
        /// <summary>
        /// 自检时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSelfCheckTime_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtSelfCheckTime);
        }
        /// <summary>
        /// 平滑时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSmoothingTime_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtSmoothingTime);
        }
        /// <summary>
        /// 测量时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtMeasuringTime_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtMeasuringTime);
        }
        /// <summary>
        /// 警报时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtAlarmTime_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtAlarmTime);
        }
        /// <summary>
        /// 强制本地次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBKGUpdate_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtBKGUpdate);
        }
        /// <summary>
        /// 左手心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLeftInProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtLeftInProbeArea);
        }
        /// <summary>
        /// 右手心
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtRightInProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtRightInProbeArea);
        }
        /// <summary>
        /// 左手背
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLeftOutProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtLeftOutProbeArea);
        }
        /// <summary>
        /// 右手背
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtRightOutProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtRightOutProbeArea);
        }
        /// <summary>
        /// 左脚
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLeftFootProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtLeftFootProbeArea);
        }
        /// <summary>
        /// 右脚
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtRightFootProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtRightFootProbeArea);
        }
        /// <summary>
        /// 衣物探头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtCloseProbeArea_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtCloseProbeArea);
        }
        /// <summary>
        /// 平滑因子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSmoothingFactor_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtSmoothingFactor);
        }
        #endregion

        #region 衣物探头
        /// <summary>
        /// 本地上限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtClothesHBackground_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtClothesHBackground);
        }
        /// <summary>
        /// 本地下限
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtClothesLBackground_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtClothesLBackground);
        }
        /// <summary>
        /// 污染警报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtClothesAlarm_1_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtClothesAlarm_1);
        }
        /// <summary>
        /// 高阶警报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtClothesAlarm_2_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtClothesAlarm_2);
        }
        /// <summary>
        /// 参测效率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtClothesEfficiency_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtClothesEfficiency);
        }
        /// <summary>
        /// 离线自检
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtClothOfflineTime_Enter(object sender, EventArgs e)
        {
            FrmKeyIn.DelegatesKeyInTextBox(TxtClothOfflineTime);
        }
        #endregion

        #endregion
        
        #region 单选按钮通用选择事件
        /// <summary>
        /// beta单选按钮通用点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdoBeta_CheckedChanged(object sender, EventArgs e)
        {
            #region β核素选择
            string nuclidename = "";//修改核素选择
            IList<RadioButton> buttonBeta = new List<RadioButton>();//核素选择数组
            buttonBeta.Add(RdoBeta14);
            buttonBeta.Add(RdoBeta58);
            buttonBeta.Add(RdoBeta131);
            buttonBeta.Add(RdoBeta204);
            buttonBeta.Add(RdoBeta32);
            buttonBeta.Add(RdoBeta60);
            buttonBeta.Add(RdoBeta137);
            buttonBeta.Add(RdoBetaDefine1);
            buttonBeta.Add(RdoBeta36);
            buttonBeta.Add(RdoBeta90);
            buttonBeta.Add(RdoBeta192);
            buttonBeta.Add(RdoBetaDefine2);
            for (int i = 0; i < buttonBeta.Count; i++)
            {
                if (buttonBeta[i].Checked)
                {
                    nuclidename = buttonBeta[i].Text;
                    break;
                }
            }
            nuclide.SetBetaNuclideUser(nuclidename);
            GetBetaData();
            #endregion
        }
        /// <summary>
        /// Alpha单选按钮通用点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdoAlpha_CheckedChanged(object sender, EventArgs e)
        {
            #region 核素选择

            //获得当前核素选择
            string nuclidename = "";//修改核素选择
            IList<RadioButton> buttonAlpha = new List<RadioButton>();//核素选择数组
            buttonAlpha.Add(RdoAlpha235);
            buttonAlpha.Add(RdoAlpha239);
            buttonAlpha.Add(RdoAlphaDefine1);
            buttonAlpha.Add(RdoAlpha238);
            buttonAlpha.Add(RdoAlpha241);
            buttonAlpha.Add(RdoAlphaDefine2);
            for (int i = 0; i < buttonAlpha.Count; i++)
            {
                if (buttonAlpha[i].Checked)
                {
                    nuclidename = buttonAlpha[i].Text;
                    break;
                }
            }
            nuclide.SetAlphaNuclideUser(nuclidename);
            GetAlphaData();
            #endregion
        }

        /// <summary>
        /// 衣物单选按钮通用点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdoClothes_CheckedChanged(object sender, EventArgs e)
        {
            #region 核素选择

            //获得当前核素选择
            string nuclidename = "";//修改核素选择
            IList<RadioButton> buttonFrisker = new List<RadioButton>();//核素选择数组
            #region 添加核素
            //α核素
            buttonFrisker.Add(RdoClothesAlpha235);
            buttonFrisker.Add(RdoClothesAlpha238);
            buttonFrisker.Add(RdoClothesAlpha239);
            buttonFrisker.Add(RdoClothesAlpha241);
            buttonFrisker.Add(RdoClothesAlphaDefine1);
            //β核素
            buttonFrisker.Add(RdoClothesBeta14);
            buttonFrisker.Add(RdoClothesBeta32);
            buttonFrisker.Add(RdoClothesBeta36);
            buttonFrisker.Add(RdoClothesBeta58);
            buttonFrisker.Add(RdoClothesBeta60);
            buttonFrisker.Add(RdoClothesBeta90);
            buttonFrisker.Add(RdoClothesBeta131);
            buttonFrisker.Add(RdoClothesBeta137);
            buttonFrisker.Add(RdoClothesBeta192);
            buttonFrisker.Add(RdoClothesBeta204);
            buttonFrisker.Add(RdoClothesBetaDefine1);
            #endregion
            for (int i = 0; i < buttonFrisker.Count; i++)
            {
                if (buttonFrisker[i].Checked)
                {
                    nuclidename = buttonFrisker[i].Text;
                    break;
                }
            }
            nuclide.SetClothesNuclideUser(nuclidename);
            GetClothesData();
            #endregion
        }
        #endregion

        #region 软件名称
        /// <summary>
        /// 软件名称下拉列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbSoftName_DropDown(object sender, EventArgs e)
        {
            CmbSoftName.Items.Clear();
            CmbSoftName.Items.Add("HFM100手脚表面污染监测仪");
            CmbSoftName.Items.Add("HFM100TS手脚表面污染监测仪");
            CmbSoftName.Items.Add("HM100手部表面污染监测仪");
            CmbSoftName.Items.Add("HM100TS手部表面污染监测仪");
            CmbSoftName.Items.Add("CRM100壁挂式污染监测仪");
            CmbSoftName.Items.Add("FM100脚部表面污染监测仪");
        }
        #endregion

        #region 窗口关闭事件
        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPreference_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (backgroundWorker_Preference.IsBusy == true)
            {
                backgroundWorker_Preference.Dispose();
                Thread.Sleep(200);
            }
            _commPort.Close();
            Thread.Sleep(200);
        }


        #endregion

        
    }
}

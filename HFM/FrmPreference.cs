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

namespace HFM.Components
{
    public partial class FrmPreference : Form
    {
        public FrmPreference()
        {
            InitializeComponent();
            //OleDbConnection oleDbConnection = new OleDbConnection(DbHelperAccess.connectionString);
            //string SQL = "SELECT * FROM HFM_Preference";
            //OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(SQL, oleDbConnection);
            //System.Data.DataSet thisDataSet = new System.Data.DataSet();
            //oleDbDataAdapter.Fill(thisDataSet, "HFM_Preference");
            //DataTable dt = thisDataSet.Tables["HFM_Preference"];
            //this.DgvAlphaSet.DataSource = dt;
            //oleDbConnection.Close();

        }

        //运行参数设置
        //private ProbeParameter ProbeParameter = new ProbeParameter();//系统参数(各类型的本底上限等参数)
        
        
        
        //private ChannelParameter ChannelParameter = new ChannelParameter();//道盒信息(α阈值等)
        private SystemParameter system = new SystemParameter();//(自检时间、单位等)
        private FactoryParameter factoryParameter = new FactoryParameter();//仪器设备信息(IP地址、软件名称、是否双手探测器等)
        private ProbeParameter probeParameter = new ProbeParameter();//系统参数(各类型的本底上限等参数)
        private Nuclide nuclide = new Nuclide();//核素选择(U_235等)
        private EfficiencyParameter efficiencyParameter = new EfficiencyParameter();//探测效率(各类型的探测效率)

        //加载参数(初始化)
        private void FrmPreference_Load(object sender, EventArgs e)
        {
            GetProferenceData();
            GetAlphaData();
            GetBetaData();
            GetClothesData();
            GetMainProferenceData();
        }
        //页面切换
        private void TabPresence_SelectedIndexChanged(object sender, EventArgs e)
        {
            //根据页面索引更新当前页面值
            switch (TabPresence.SelectedIndex)
            {
                case 0: GetProferenceData();
                    break;
                case 1: GetAlphaData();
                    break;
                case 2: GetBetaData();
                    break;
                case 3: GetClothesData();
                    break;
                case 4: GetMainProferenceData();
                    break;
                case 5: GetFacilityData();
                    break;
                default: MessageBox.Show("选择有误，请重新选择");
                    break;
            }

        }

        #region 获得数据库数据并显示出来
        /// <summary>
        /// 获得系统页面参数
        /// </summary>
        private void GetProferenceData()
        {
            system = system.GetParameter();//获得自检时间、单位等参数
            factoryParameter = factoryParameter.GetParameter();//获得仪器设备信息参数

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
        //获得β数据
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
        //获得衣物参数
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
        //获得道盒参数
        private void GetMainProferenceData()
        {

        }
        //获得仪器选择参数
        private void GetFacilityData()
        {

        }
        #endregion




    }
}

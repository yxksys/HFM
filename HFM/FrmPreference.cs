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
        //private Nuclide Nuclide = new Nuclide();//核素选择(U_235等)
        
        //private EfficiencyParameter EfficiencyParameter = new EfficiencyParameter();//探测效率(各类型的探测效率)
        //private ChannelParameter ChannelParameter = new ChannelParameter();//道盒信息(α阈值等)
        private SystemParameter system = new SystemParameter();//(自检时间、单位等)
        private FactoryParameter factoryParameter = new FactoryParameter();//仪器设备信息(IP地址、软件名称、是否双手探测器等)
        private ProbeParameter probeParameter = new ProbeParameter();//系统参数(各类型的本底上限等参数)

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

        #region 获得数据库数据
        //获得系统页面参数
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
            RdoIsConnectedAuto.Checked = factoryParameter.IsConnectedAuto;
            //CmbUnclideType.Text = factoryParameter    
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
                    ((TextBox)a[probeParameters[i].PreferenceID - 1]).Text = probeParameters[i].ProbeChannel.ProbeArea.ToString();
                }
                else
                {
                    //根据channelID来修改数据
                    ((TextBox)a[probeParameters[i].PreferenceID - 1]).Enabled = false;
                    ((Label)label[probeParameters[i].PreferenceID - 1]).Enabled = false;
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
        //获得α数据
        private void GetAlphaData()
        {
            
        }
        //获得β数据
        private void GetBetaData()
        {

        }
        //获得衣物参数
        private void GetClothesData()
        {

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmWelcome : Form
    {
        public FrmWelcome()
        {
            InitializeComponent();
        }

        private void BtnTestRorW_Click(object sender, EventArgs e)
        {
            
            byte[] by = new byte[62];
            
            by = HFM.Components.Message.BuildMessage(0);
            by = HFM.Components.Message.BuildMessage(300, 20, 30, 40);
            IList<ChannelParameter> channelParameters = new List<ChannelParameter>();
            for(int i=0;i<7;i++)
            {
                //channelParameters.Add(new ChannelParameter(i + 1, i + 2, i + 3, i + 4, i + 5, i + 6, i + 7, i + 8, i + 9));             
                ChannelParameter channelParameter = new ChannelParameter();
                Channel channel = new Channel();
                channel.ChannelID = i + 1;
                channelParameter.Channel = channel;
                channelParameter.AlphaThreshold = i + 2;
                channelParameter.BetaThreshold = i + 3;
                channelParameter.PresetHV = i + 4;
                channelParameter.ADCFactor = i + 5;
                channelParameter.DACFactor = i + 6;
                channelParameter.HVFactor = i + 7;
                channelParameter.WorkTime = i + 8;
                channelParameter.HVRatio = i + 9;
                channelParameters.Add(channelParameter);
            }
            by = HFM.Components.Message.BuildMessage(channelParameters);
            MessageBox.Show(by.ToString());
            //测试CalibrationGetData方法
            Calibration calibration = new Calibration();
            //不带参数查询测试
            IList<Calibration> Icalibrations = calibration.GetData();
            //带参数查询测试
            Icalibrations = calibration.GetData(2);
            if(calibration.AddData(new Calibration(DateTime.Now,2,3,4,5,6,7))==true)
            {
                MessageBox.Show("添加成功");
            }
        }

        private void BtnCommport_Click(object sender, EventArgs e)
        {
            //实例化相关对象
            Calibration calibration = new Calibration();
            CommPort commPort = new CommPort();
            //打开串口
            if (commPort.Opened == false)
            {
                try
                {
                    commPort.Open();
                }
                catch
                {
                    MessageBox.Show("端口打开错误，请检查通讯是否正常！");
                }
            }
           
        }
    }
}

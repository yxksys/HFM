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
            byte[] mess = new byte[4];
            mess[0] =Convert.ToByte( 'p');
            mess[1] = 48;
            mess[2] = 49;
            mess[3] = 125;
            string head = Convert.ToChar(mess[0]).ToString();
            MessageBox.Show("hello word!...");
            int x = mess[1];
            int y = mess[2];
            int z = mess[3];
            MessageBox.Show("123");
            MessageBox.Show("456");
            Channel channel = new Channel();
            //SystemParameter systemParameter = new SystemParameter();
            string str = "112";
            x = 1110;
            int ii = Convert.ToInt32(str);
            HexCon hexcon = new HexCon();
            str = hexcon.ByteToString(new byte[] { 48 });
            byte[] oo = new byte[1];
            oo = hexcon.StringToByte(str);
            ii = Convert.ToInt32(oo[0]);
            ProbeParameter probeParameter = new ProbeParameter();
            ProbeParameter text = new ProbeParameter();
            probeParameter.ProbeType = "闪烁体";
            probeParameter.NuclideType = "α";
            probeParameter.ProbeChannel = new Channel(2, "", "", 20, "", false);
            probeParameter.HBackground = 61;
            probeParameter.LBackground = 21;
            probeParameter.Efficiency = 91;
            probeParameter.Alarm_1 = 60;
            probeParameter.Alarm_2 = 87;
            bool rows=text.SetParameter(probeParameter);
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
            commPort.GetCommPortSet();
            //打开串口
            if (commPort.Opened == false)
            {
                try
                {
                    commPort.Open();
                    byte[] messageData = new byte[62];
                    messageData = HFM.Components.Message.BuildMessage(0);

                    if(HFM.Components.Message.SendMessage(messageData,commPort)==true)
{
                        string messageStr = HFM.Components.Message.ReceiveMessage(commPort);
                    }
                    
                    commPort.Close();
                }
                catch
                {
                    MessageBox.Show("端口打开错误，请检查通讯是否正常！");
                }
            }
           
        }
    }
}

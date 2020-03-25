using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;
using Message = HFM.Components.Message;

namespace HFM
{
    public partial class FrmWelcome : Form
    {
        enum HardwarePlatformState
        {
            /// <summary>
            /// 默认状态
            /// </summary>
            Default = 0,
            /// <summary>
            /// Alpha自检
            /// </summary>
            AlphaCheck = 1,
            /// <summary>
            /// Beta自检
            /// </summary>
            BetaCheck = 2,
            /// <summary>
            /// 自检
            /// </summary>
            SelfTest = 3
        }
        FrmTest frmTest = null;
        //运行状态标志
        HardwarePlatformState platformState;
        public FrmWelcome()
        {
            InitializeComponent();

        }

        string[] _hv = new string[6];
        //alpha计数
        string[] _alphacps = new string[6];
        //alpha总计数
        string[] _alphacnt = new string[6];
        //Beta计数
        string[] _betacps = new string[6];
        //Beta总计数
        string[] _betacnt = new string[6];
        //通道状态
        string[] _strat = new string[6];
        //衣物计数
        string frisker = "";
        private void BtnTestRorW_Click(object sender, EventArgs e)
        {

            //FactoryParameter factoryParameter = new FactoryParameter();
            //factoryParameter.GetParameter();
            //factoryParameter=new FactoryParameter("1001", "HFM", "110.110.110.110", "100", false, "a", 100, true, "address");
            //factoryParameter.GetParameter();
            //byte[] mess = new byte[4];
            //mess[0] =Convert.ToByte( 'p');
            //mess[1] = 48;
            //mess[2] = 49;
            //mess[3] = 125;
            //string head = Convert.ToChar(mess[0]).ToString();
            //MessageBox.Show("hello word!...");
            //int x = mess[1];
            //int y = mess[2];
            //int z = mess[3];
            //MessageBox.Show("123");
            //MessageBox.Show("456");
            //Channel channel = new Channel();
            ////SystemParameter systemParameter = new SystemParameter();
            //string str = "112";
            //x = 1110;
            //int ii = Convert.ToInt32(str);
            //HexCon hexcon = new HexCon();
            //str = hexcon.ByteToString(new byte[] { 48 });
            //byte[] oo = new byte[1];
            //oo = hexcon.StringToByte(str);
            //ii = Convert.ToInt32(oo[0]);
            //ProbeParameter probeParameter = new ProbeParameter();
            //ProbeParameter text = new ProbeParameter();
            //probeParameter.ProbeType = "闪烁体";
            //probeParameter.NuclideType = "α";
            //probeParameter.ProbeChannel = new Channel(2, "", "", 20, "", false);
            //probeParameter.HBackground = 61;
            //probeParameter.LBackground = 21;
            //probeParameter.Efficiency = 91;
            //probeParameter.Alarm_1 = 60;
            //probeParameter.Alarm_2 = 87;
            //bool rows=text.SetParameter(probeParameter);
            ////测试CalibrationGetData方法
            //Calibration calibration = new Calibration();
            ////不带参数查询测试
            //IList<Calibration> Icalibrations = calibration.GetData();

            //带参数查询测试
            //Icalibrations = calibration.GetData(1);
            //if (calibration.AddData(new Calibration(DateTime.Now, 2, 3, 4, 5, 6, 7)) == true)
            //{
            //    MessageBox.Show("添加成功");
            //}    

            ////测试通道类查询    
            //Channel channel = new Channel();    

            //测试系统参数查询
            //Components.SystemParameter system = new Components.SystemParameter();
            //测试检测完成后检查次数+1
            //system.UpdateMeasuredCount();
            //system.ClearMeasuredCount();
            //system.GetParameter();
            //if(system.SetParameter(new Components.SystemParameter("1", 1, 1, 1, 1, 1, 1, false)) == true)
            //{
            //    MessageBox.Show("更新成功");
            //}


            //测试ChannelParameter
            ChannelParameter channelParameter = new ChannelParameter();
            ChannelParameter p = new ChannelParameter();
            channelParameter = p.GetParameter(1);
            if (channelParameter.CheckingID != 0)
            {
                MessageBox.Show("ok");
            }
        }

        private void BtnCommport_Click(object sender, EventArgs e)
        {
            byte[] messageData = new byte[62];
            messageData[0] = Convert.ToByte('C');         

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
                    int i = 0;
                    while (true)
                    {
                        i++;
                        if (HFM.Components.Message.SendMessage(messageData, commPort) == true)
                        {

                            if (i > 100) return;
                            System.Threading.Thread.Sleep(100);
                            byte[] receiveBuffMessage = new byte[200];
                            receiveBuffMessage = HFM.Components.Message.ReceiveMessage(commPort);
                            textBox1.Text += receiveBuffMessage[61].ToString()+";";
                        }
                    }
                    

                    messageData=HFM.Components.Message.BuildMessage(0);

                    if(HFM.Components.Message.SendMessage(messageData,commPort)==true)
{
                        byte[] messageStr = HFM.Components.Message.ReceiveMessage(commPort);
                    }
                    
                    commPort.Close();
                }
                catch
                {
                    MessageBox.Show("端口打开错误，请检查通讯是否正常！");
                }
            }
           
        }
        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        private static extern bool SetSystemTime(ref SYSTEMTIME t);
        
        [StructLayout(LayoutKind.Sequential)]
        struct SYSTEMTIME
        {
            public short Year;
            public short Month;
            public short DayofWeek;
            public short Day;
            public short Hour;
            public short Minute;
            public short Second;
            public short MiliSecond;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            short time = 13;
            SYSTEMTIME t = new SYSTEMTIME();
            t.Year = 2020;
            t.Month = 3;
            t.Day = 6;
            t.Hour =(short)(time - 8)<=0? (short)(time - 8+24): (short)(time - 8);
            t.Minute = 35;
            t.Second = 0;
            t.MiliSecond = 20;
            bool v = SetSystemTime(ref t);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            frmTest.MdiParent = this;
            frmTest.Controls["textBox1"].Text = (Convert.ToInt32(frmTest.Controls["textBox1"].Text)+1).ToString();
            frmTest.Show();
        }

        private void FrmWelcome_Load(object sender, EventArgs e)
        {
            frmTest = new FrmTest();
        }
    }
}

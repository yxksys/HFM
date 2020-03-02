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
            List<MeasureData> measure = new List<MeasureData>();
            MeasureData one = new MeasureData(1, DateTime.Now, 1001, 1007, 24, 24, 500);
            MeasureData two = new MeasureData(2, DateTime.Now, 1002, 1006, 24, 24, 500);
            MeasureData three = new MeasureData(3, DateTime.Now, 0, 0, 24, 24, 0);
            MeasureData four = new MeasureData(4, DateTime.Now, 1004, 1004, 24, 24, 500);
            MeasureData five = new MeasureData(5, DateTime.Now, 1005, 0, 24, 24, 500);
            MeasureData six = new MeasureData(6, DateTime.Now, 0, 0, 24, 24, 500);
            MeasureData seven = new MeasureData(7, DateTime.Now, 100, 220, 24, 24, 500);
            measure.Add(one);
            measure.Add(two);
            measure.Add(three);
            measure.Add(four);
            measure.Add(five);
            measure.Add(six);
            measure.Add(seven);
            //运行状态标志
            

           
            //float[] _hv=new float[6];
            //float[] _alpha=new float[6];
            //float[] _alphacnt = new float[6];
            //float[] _betacnt = new float[6];
            //float[] _beta = new float[6];
            //定义dgv高压
            
            //临时变量
            int i = 0;
            //从列表中取出数据
            platformState = HardwarePlatformState.BetaCheck;
            foreach (var item in measure)
            {
                //通过运行状态判断衣物探头是alpha还是beta，默认alpha通道计数
                if (i == 6)
                {
                    if (platformState == HardwarePlatformState.AlphaCheck)
                    {
                        frisker = Convert.ToString(item.Alpha);
                    }
                    else if (platformState == HardwarePlatformState.BetaCheck)
                    {
                        frisker = Convert.ToString(item.Beta);
                    }
                    else
                    {
                        frisker = Convert.ToString(item.Alpha);
                    }
               
                    break;
                }
                _hv[i]=Convert.ToString(item.HV);
                _alphacps[i] = Convert.ToString(item.Alpha);
                _betacps[i] = Convert.ToString(item.Beta);
                i++;
            }
            //赋值alpha和Beta总计数并且判断赋值通道状态
            for (i = 0; i < 6; i++)
            {
                //alpha总计数
                _alphacnt[i] = Convert.ToString(Convert.ToInt32(_alphacnt[i]) + Convert .ToInt32(_alphacps[i]));
                //beta总计数
                _betacnt[i] = Convert.ToString(Convert.ToInt32( _betacnt[i]) + Convert.ToInt32(_betacps[i]));
                //判断通道状态
                if (Convert.ToInt32(_hv[i]) == 0 && (Convert.ToInt32(_alphacnt[i]) == 0 || Convert.ToInt32(_betacnt[i]) == 0 ))
                {
                    _strat[i] = "通讯故障";
                }
                else if (Convert.ToInt32(_alphacnt[i]) == 0 && Convert.ToInt32(_betacnt[i]) == 0)
                {
                    _strat[i] = "探头故障";
                }
                else
                {
                    _strat[i] = "正常工作";
                }
            }

            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Insert(0,_hv);
            dataGridView1.Rows.Insert(1,_alphacps);
            dataGridView1.Rows.Insert(2,_alphacnt);
            dataGridView1.Rows.Insert(3,_betacps);
            dataGridView1.Rows.Insert(4,_betacnt);
            dataGridView1.Rows.Insert(5,_strat);
            textBox1.Text = frisker;
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
        }

        private void BtnCommport_Click(object sender, EventArgs e)
        {
            byte[] messageData = new byte[62];
            messageData[0] = Convert.ToByte('P');         

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
                   
                    while (true)
                    {
                        if (HFM.Components.Message.SendMessage(messageData, commPort) == true)
                        {


                            System.Threading.Thread.Sleep(100);
                            byte[] receiveBuffMessage = new byte[200];
                            receiveBuffMessage = HFM.Components.Message.ReceiveMessage(commPort);

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
    }
}

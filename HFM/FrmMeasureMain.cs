using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using HFM.Components;

namespace HFM
{
    public partial class FrmMeasureMain : Form
    {       
        int ix = 0;
        CommPort commPort = new CommPort();
        //工厂参数
        FactoryParameter factoryParameter = new FactoryParameter();
        //系统参数
        Components.SystemParameter systemParameter = new Components.SystemParameter();        
        //监测时间
        int checkTime = 0;
        //系统报警时间长度设置
        int alarmTimeSet = 0;
        //系统报警时间计时
        DateTime alarmTimeStart = DateTime.Now;
        int stateTimeSet = 0;//系统当前运行状态的检测时间设置
        int stateTimeRemain = 0;//系统当前运行状态剩余时间
        int errNumber = 0; //报文接收出现错误计数器   
        bool isPlayed = false;//是否已经进行语音播报
        //当前可使用的检测通道,即全部启用的监测通道
        IList<Channel> channelS = new List<Channel>();
        //运行状态枚举类型
        enum PlatformState
        {
            ReadyToRun = 0,
            SelfTest = 1,
            BackGrouneMeasure = 2,
            ReadyToMeasure = 3,
            Measuring = 4,
            Result = 5
        }
        //运行状态标志
        PlatformState platformState;
        enum DeviceStatus
        {
            OperatingNormally=1,
            OperatingFaulted=2,
            OperatingContaminated=4
        }
        byte deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
        const int BASE_DATA = 1000;
        //存储各个通道最终计算检测值的List
        IList<MeasureData> calculatedMeasureDataS = new List<MeasureData>();        
        DateTime stateTimeStart = DateTime.Now;//系统当前运行状态的开始计时变量
        //存储本底计算结果，用例对测量数据进行校正
        IList<MeasureData> baseData = new List<MeasureData>();
        public FrmMeasureMain()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 初始化显示界面
        /// </summary>
        private void DisplayInit()
        {            
            //在界面中显示当前系统时间
            LblTime.Text = DateTime.Now.ToLongTimeString();
            //获得系统参数信息            
            factoryParameter.GetParameter();
            //在界面中显示“仪器名称”、“仪器编号”、“IP地址及端口”等信息
            LblName.Text = factoryParameter.SoftName;
            LblIP.Text += factoryParameter.IpAddress + " " + factoryParameter.PortNumber;
            LblSN.Text += factoryParameter.InstrumentNum;
            //获得当前系统应用路径
            string appPath = Application.StartupPath;
            //加载相关图片资源（设备厂家图标、手部脚步图片、衣物图片等）
            PicLHB.Image = Image.FromFile(appPath + "\\Images\\LHB.png");
            PicLHP.Image = Image.FromFile(appPath + "\\Images\\LHP.png");
            PicRHB.Image = Image.FromFile(appPath + "\\Images\\RHB.png");
            PicRHP.Image = Image.FromFile(appPath + "\\Images\\RHP.png");
            PicLF.Image = Image.FromFile(appPath + "\\Images\\LF.png");
            PicRF.Image = Image.FromFile(appPath + "\\Images\\RF.png");
            PicFrisker.Image = Image.FromFile(appPath + "\\Images\\clothes.png");
            PicLogo.Image = Image.FromFile(appPath + "\\Images\\logo.png");

            //设置label的容器为其对应的PictureBox，以使其透明背景有效   
            //左手状态显示区域
            LblLeft.Parent = PicLeftBackground;
            //重新设置label的位置，是想对于容器PictureBox的位置而不是窗体的位置
            LblLeft.Location = new Point(3, 20);
            //右手状态显示区域
            LblRight.Parent = PicRightBackground;
            LblRight.Location = new Point(3, 20);
            //衣物显示区域
            LblFrisker.Parent = PicFrisker;
            //label在PictureBox中的位置为右对齐
            LblFrisker.Location = new Point(PicFrisker.Width - LblFrisker.Width, 15);
            //监测过程状态显示区域
            LblShowStutas.Parent = PicShowStatus;//监测状态显示
            LblTimeRemain.Parent = PicShowStatus; //剩余时间显示
            LblShowStutas.Location = new Point(3, 50);
            LblTimeRemain.Location = new Point(PicShowStatus.Width - LblTimeRemain.Width, 0);                        
        }
        private void ChannelDisplayControl(Channel channel,bool enabled,Color backColor)
        {
            PictureBox picture = null;
            TextBox textBox = null;
            Panel panel = null;
            Label label = null;
            //界面中对应通道的Enabled属性设置为True，该通道相关元素背景色设置为正常
            picture = (PictureBox)(this.Controls[string.Format("Pic{0}", channel.ChannelName_English)]);
            if (picture != null)
            {
                picture.Enabled = enabled;
                picture.BackColor = backColor;
            }
            textBox = (TextBox)(this.Controls[string.Format("Txt{0}", channel.ChannelName_English)]);
            if (textBox != null)
            {
                textBox.Enabled = enabled;
                //textBox.BackColor = backColor;
            }
            //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名                    
            panel = (Panel)(this.Controls[string.Format("Pnl{0}", channel.ChannelName_English)]);
            if (panel != null)
            {
                panel.Enabled = enabled;
                //panel.BackColor = backColor;
                //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名                    
                label = (Label)(panel.Controls[string.Format("Lbl{0}", channel.ChannelName_English)]);
            }
            else
            {
                label = (Label)(this.Controls[string.Format("Lbl{0}", channel.ChannelName_English)]);
            }
            if (label != null)
            {
                label.Enabled = enabled;
                //label.BackColor = backColor;
            }
        }
        /// <summary>
        /// 显示测量数据
        /// </summary>
        private void DisplayMeasureData(IList<MeasureData> measureDataS,string unit)
        {
            PictureBox pictureBox = null;
            Panel panel = null;
            Label label = null;
            PnlLHB.Parent = PicLHB;
            PnlLHP.Parent = PicLHP;
            PnlRHP.Parent = PicRHP;
            PnlRHB.Parent = PicRHB;
            PnlLF.Parent = PicLF;
            PnlRF.Parent = PicRF;
            foreach (MeasureData measureData in measureDataS)
            {
                if (measureData.Channel.ChannelName_English == "Frisker")
                {
                    //显示衣物测量结果
                    continue;
                }                
                switch (factoryParameter.MeasureType)
                {
                    case "α":
                        //找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                        pictureBox=(PictureBox)(this.Controls[string.Format("Pic{0}",measureData.Channel.ChannelName_English)]);
                        //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                        panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                        label = (Label)(panel.Controls[string.Format("Lbl{0}", measureData.Channel.ChannelName_English)]);
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F1") + unit;
                        break;
                    case "β":                        
                        //找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                        pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", measureData.Channel.ChannelName_English)]);
                        //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                        panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                        label= (Label)(panel.Controls[string.Format("Lbl{0}", measureData.Channel.ChannelName_English)]);
                        //Label控件显示测量值
                        label.Text = "β" + measureData.Beta.ToString("F1") + unit;                        
                        break;
                    case "α/β":
                        //找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                        pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", measureData.Channel.ChannelName_English)]);
                        //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                        panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                        label = (Label)(panel.Controls[string.Format("Lbl{0}", measureData.Channel.ChannelName_English)]);
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F1") + unit+ "\r\nβ" + measureData.Beta.ToString() + unit;
                        break;
                }
            }               
            //左手背监测结果显示控制，Panel的高度和Label的高度一致。
            PnlLHB.Height = LblLHB.Height + 1;
            //控制Panel显示在PictureBox的底部            
            PnlLHB.Location = new Point(0, PicLHB.Height - PnlLHB.Height - 5);
            //左手心监测结果显示控制
            PnlLHP.Height = LblLHP.Height + 1;            
            PnlLHP.Location = new Point(0, PicLHP.Height - PnlLHP.Height - 5);
            //右手心监测结果显示控制
            PnlRHP.Height = LblRHP.Height + 1;            
            PnlRHP.Location = new Point(0, PicRHP.Height - PnlRHP.Height - 5);
            //右手背监测结果显示控制
            PnlRHB.Height = LblRHB.Height + 1;            
            PnlRHB.Location = new Point(0, PicRHB.Height - PnlRHB.Height - 5);
            //左脚监测结果显示控制
            PnlLF.Height = LblLF.Height + 1;            
            PnlLF.Location = new Point(0, PicLF.Height - PnlLF.Height - 5);
            //右脚监测结果显示控制
            PnlRF.Height = LblRF.Height + 1;            
            PnlRF.Location = new Point(0, PicRF.Height - PnlRF.Height - 5);
        }
        private void FrmMeasureMain_Load(object sender, EventArgs e)
        {
            //初始化显示界面
            DisplayInit();
            //获得系统参数设置
            systemParameter.GetParameter();
            checkTime = systemParameter.SelfCheckTime;
            alarmTimeSet = systemParameter.AlarmTime;                                
            Channel channel = new Channel();
            //在界面中将启用通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，其它通道（通道未启用或探测面积为0）的控件Enabled无效；           
            //获得全部通道信息
            channelS = channel.GetChannel();
            for(int i=0;i<channelS.Count;i++)
            {                
                if(channelS[i].IsEnabled==true && channelS[i].ProbeArea!=0)//通道被启用且探测面积不为0
                {
                    //界面中相关通道控件Enabled设置为true，背景色设置为正常
                    ChannelDisplayControl(channelS[i], true, PlatForm.ColorStatus.CORLOR_NORMAL);
                }
                else
                {
                    //界面中相关通道控件Enabled属性设置为false，背景色设置为屏蔽
                    ChannelDisplayControl(channelS[i], false, PlatForm.ColorStatus.COLOC_PINGBI);
                }
            }
            //获得全部启用通道
            channelS = channel.GetChannel(true);
            //将检测面积为0的通道剔除
            for (int i = 0; i < channelS.Count; i++)
            {
                if (channelS[i].ProbeArea == 0)
                {
                    channelS.RemoveAt(i);
                    i--;
                }
            }
            //根据有效通道对象初始化用来存储最终监测数据的列表
            foreach (Channel usedChannel in channelS)
            {
                MeasureData measureData = new MeasureData();
                measureData.Channel = usedChannel;
                calculatedMeasureDataS.Add(measureData);
            }                                     
            //正常使用的通道测量数值显示（初始值为0cps）
            DisplayMeasureData(calculatedMeasureDataS,"cps");            
            //打开串口
            if (commPort.Opened == true)
            {
                commPort.Close();
            }
            //从配置文件获得当前串口配置
            commPort.GetCommPortSet();
            //打开串口
            try
            {
                commPort.Open();
            }
            catch
            {
                MessageBox.Show("端口打开错误！请检查通讯是否正常。");
                TxtShowResult.Text = "串口打开失败";
                return;
            }            
            //将运行状态标志设置为“运行准备”
            platformState = PlatformState.ReadyToRun;
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
            //线程支持报告进度
            bkWorkerReceiveData.WorkerReportsProgress = true;
            //启动异步线程,响应DoWork事件
            bkWorkerReceiveData.RunWorkerAsync();
            bkWorkerReportStatus.RunWorkerAsync();
        }
        /// <summary>
        /// 异步线程DoWork事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 通过串口读取下位机上传数据
        /// </summary>
        /// <param name="worker"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private byte[] ReadDataFromSerialPort(BackgroundWorker worker, DoWorkEventArgs e)
        {
            bool isSelfCheckSended = false;
            int errorNumber = 0; //下发自检报文出现错误计数器
            int delayTime = 200;//下发自检报文延时时间
            //DateTime stateTimeStart = DateTime.Now.AddSeconds(-3);//初始化计时开始时间
            while (true)
            {
                //请求进程中断读取数据
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }
                //如果当前运行状态为“仪器自检”，则根据不同探测类型向下位机下发相应的自检指令
                if (platformState == PlatformState.SelfTest && isSelfCheckSended==false)
                {
                    byte[] messageDate = null;
                    switch (factoryParameter.MeasureType)
                    {                                                                          
                        case "α":
                            //生成Alpha自检指令报文，包含参数：自检时间/2-2 
                            messageDate = Components.Message.BuildMessage(0, checkTime / 2-2);
                            //下发Alpha自检指令，如果不成功则：
                            if (Components.Message.SendMessage(messageDate, commPort) != true)
                            {
                                //错误计数器errorNumber+1
                                errorNumber++;
                                //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                                if (errorNumber > 5)
                                {
                                    worker.ReportProgress(1, null);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(delayTime);
                                    continue;
                                }
                            }
                            ////下发成功，置报文已经发送标志
                            //isSelfCheckSended = true;
                            break;
                        case "β":
                            //生成Beta自检指令报文，包含参数：自检时间/2-2
                            messageDate = Components.Message.BuildMessage(1, checkTime / 2 - 2);
                            //下发Beta自检指令,如果不成功则
                            if(Components.Message.SendMessage(messageDate,commPort)!=true)
                            {
                                //错误计数器errorNumber+1
                                errorNumber++;
                                //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                                if (errorNumber > 5)
                                {
                                    worker.ReportProgress(1, null);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(delayTime);
                                    continue;
                                }
                            }
                            
                            ////下发成功，置报文已经发送标志
                            //isSelfCheckSended = true;                            
                            break;
                        case "α/β":
                            //先下发Alpha自检指令
                            //生成Alpha自检指令报文，包含参数：自检时间/2-2 
                            messageDate = Components.Message.BuildMessage(0, checkTime / 2 - 2);
                            //下发Alpha自检指令，如果不成功则：
                            if (Components.Message.SendMessage(messageDate, commPort) != true)
                            {
                                //错误计数器errorNumber+1
                                errorNumber++;
                                //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                                if (errorNumber > 5)
                                {
                                    worker.ReportProgress(1, null);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(delayTime);
                                    continue;
                                }
                            }
                            //后下发Beta自检指令
                            //生成Beta自检指令报文，包含参数：自检时间/2-2
                            messageDate = Components.Message.BuildMessage(1, checkTime / 2 - 2);
                            //下发Beta自检指令,如果不成功则
                            if (Components.Message.SendMessage(messageDate, commPort) != true)
                            {
                                //错误计数器errorNumber+1
                                errorNumber++;
                                //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                                if (errorNumber > 5)
                                {
                                    worker.ReportProgress(1, null);
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(delayTime);
                                    continue;
                                }
                            }
                            ////下发成功，置报文已经发送标志
                            //isSelfCheckSended = true;
                            break;
                    }
                    //下发成功，置报文已经发送标志
                    isSelfCheckSended = true;
                    //延时100毫秒
                    Thread.Sleep(100);
                    //启动自检计时 
                    //stateTimeStart = System.DateTime.Now.AddSeconds(-3);
                }
                //向下位机下发“C”指令码
                byte[] buffMessage = new byte[62];
                buffMessage[0] = Convert.ToByte('C');
                //将当前监测状态打包到报文最后一个字节
                buffMessage[61] = Convert.ToByte(deviceStatus);
                if (HFM.Components.Message.SendMessage(buffMessage, commPort) == true)
                {
                    //延时
                    Thread.Sleep(100);
                    //读取串口回传数据并赋值给receiveBuffMessage
                    byte[] receiveBuffMessage = new byte[200];
                    receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                    //延时
                    Thread.Sleep(700);
                    //触发向主线程返回下位机上传数据事件
                    worker.ReportProgress(1, receiveBuffMessage);
                }
            }
        }

        private void bkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {            
            string appPath = Application.StartupPath;
            int messageBufferLength = 62; //最短报文长度            
            byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象                        
            bool isFirstBackGround = true;//进入等待测量状态后的本底测量计时标志
            string pollutionRecord = null;//记录测量污染详细数据
            //创建音频播放对象
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            if (e.UserState is byte[])
            {
                receiveBufferMessage = (byte[])e.UserState;
            }
            //报警时间超过系统参数设置的报警时间长度，则监测状态恢复为正常状态
            if ((DateTime.Now - alarmTimeStart).Seconds >= alarmTimeSet)
            {
                deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
            }                
            ////更新当前显示时间
            //LblTime.Text = DateTime.Now.ToLongTimeString();
            //接收报文数据为空
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 2)
                {
                    //界面提示“通讯错误”
                    TxtShowResult.Text = "通讯错误！";
                }
                else
                {
                    errNumber++;
                }
                return;
            }
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            measureDataS = Components.Message.ExplainMessage<MeasureData>(receiveBufferMessage);
            //textBox1.Text += measureDataS.ToString();
            //return;
            //如果当前运行状态为“运行准备”
            if (platformState == PlatformState.ReadyToRun)
            {
                //textBox1.Text = platformState.ToString();
                //判断衣物探头被拿起
                if (measureDataS[6].InfraredStatus == 1)
                {
                    //跳转至衣物检测界面
                    //
                }
                //如果手部探测器为启用状态，则判断左右手是否到位，到位则相应指示框背景变为绿色，否则为橙色，同时进行文字信息提示（具体操作可参考老版本源代码）
                if(channelS.FirstOrDefault(channel => channel.ChannelName_English == "LHP")!=null)//左手心启用,左右手全部启用（手心手背四个通道）
                {                    
                    if(measureDataS[0].InfraredStatus!=1)//左手红外不到位（左手心和左手背红外状态一致，所以判断左手心即可）
                    {
                        PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;                        
                        LblLeft.Text = "左手不到位";                        
                    }
                    else
                    {
                        PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;                        
                        LblLeft.Text = "左手到位";                        
                    }
                    if(measureDataS[2].InfraredStatus!=1)//右手红外不到位（有手心和右手背红外状态一致，所以判断右手心即可）
                    {
                        PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        LblRight.Text = "右手不到位";
                    }
                    else
                    {
                        PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                        LblRight.Text = "右手到位";
                    }
                }
                //当前运行状态设置为“仪器自检”
                platformState = PlatformState.SelfTest;
                LblShowStutas.Text = "仪器自检";
                TxtShowResult.Text += "仪器自检\r\n";
                player.SoundLocation = appPath + "\\Audio\\Chinese_Self_checking.wav";
                player.Play();
                stateTimeStart = System.DateTime.Now.AddSeconds(1);
                return;
            }
            //如果当前运行状态为“仪器自检”
            if (platformState == PlatformState.SelfTest)
            {
                //textBox1.Text += platformState.ToString();
                //获得当前系统参数设置中的的自检时间并赋值给stateTimeSet
                stateTimeSet=systemParameter.SelfCheckTime;               
                //更新剩余时间：系统自检设置时间-已经用时
                stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                //更新当前系统运行状态剩余时间
                LblTimeRemain.Text = stateTimeRemain<0?"0": stateTimeRemain.ToString();
                // IList<int> channelIDS=channelS.Select(channel => channel.ChannelID).ToList();               
                for (int i = 0; i < channelS.Count; i++)
                {
                    /*因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    *所以，需要从measureDataS中找到对应通道的监测数据赋值给calculatedMeasureDataS，但是本地值Alpha和Beta需要累加*/
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    //计算每个通道上传的Alpha和Beta计数累加(是指全部启用的通道)
                    calculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    calculatedMeasureDataS[i].Beta += list[0].Beta;
                    //将当前通道本次的其它测量数据赋值给calculatedMeasureDataS
                    calculatedMeasureDataS[i].AnalogV = list[0].AnalogV;
                    calculatedMeasureDataS[i].DigitalV = list[0].DigitalV;
                    calculatedMeasureDataS[i].HV = list[0].HV;
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    calculatedMeasureDataS[i].IsEnglish = list[0].IsEnglish;
                    calculatedMeasureDataS[i].MeasureDate = DateTime.Now;                                        
                }
                //界面中对应通道显示当前测量值
                DisplayMeasureData(calculatedMeasureDataS,"cps");
                //自检时间到
                if (stateTimeRemain < 0)
                {
                    isPlayed = false;
                    string[] errRecordS = new string[2];
                    errRecordS = BaseCheck();
                    if (errRecordS == null)//自检通过
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        // 运行状态标志设置为“本底测量”
                        platformState = PlatformState.BackGrouneMeasure;
                        //启动本底测量计时 
                        stateTimeStart = System.DateTime.Now;
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        //系统状态显示区域显示本底测量
                        LblShowStutas.Text = "本底测量";
                        //测试结果区域显示本底测量
                        TxtShowResult.Text += "本底测量\r\n";                       
                        //系统提示本底测量
                        player.SoundLocation = appPath + "\\Audio\\Chinese_Background_measure.wav";
                        player.Play();
                    }
                    else
                    {
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库                       
                        ErrorData errorData = new ErrorData
                        {
                            //中文描述
                            Record = errRecordS[0],
                            IsEnglish = false
                        };
                        errorData.AddData(errorData);
                        //英文描述
                        errorData.Record = errRecordS[1];
                        errorData.IsEnglish = true;
                        errorData.AddData(errorData);
                        //语音提示故障
                        player.SoundLocation = appPath + "\\Audio\\Chinese_Self-checking_fault.wav";
                        player.Play();
                        Thread.Sleep(3000);
                        //测量数据存储全部清零
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                            //启动故障报警计时
                            alarmTimeStart = System.DateTime.Now;
                        //重新启动自检计时
                        stateTimeStart = System.DateTime.Now;
                    }
                    return;
                }
            }
            //运行状态为本底测量
            if (platformState == PlatformState.BackGrouneMeasure)
            {
                //textBox1.Text += platformState.ToString();
                //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                stateTimeSet = systemParameter.SmoothingTime;                        
                //在系统界面中显示本底测量倒计时时间（s）:系统平滑设置时间-已经用时
                stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                LblTimeRemain.Text = stateTimeRemain < 0 ? "0" : stateTimeRemain.ToString();
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)：
                    //第k次计算本底值=第k-1次计算本底值*平滑因子/（平滑因子+1）+第k次测量值/（平滑因子+1）               
                    calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Alpha / (factoryParameter.SmoothingFactor + 1);
                    calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Beta / (factoryParameter.SmoothingFactor + 1);
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;                    
                    //当前通道红外到位
                    if (calculatedMeasureDataS[i].InfraredStatus == 1)
                    {
                        if (isPlayed != true)//保证在一个测量周期只播报和显示一次
                        {
                            isPlayed = true;
                            //显示当前calculatedMeasureDataS[i].Channel.ChannelName对应的红外到位
                            if (calculatedMeasureDataS[i].Channel.ChannelName_English == "LHP" || calculatedMeasureDataS[i].Channel.ChannelName_English == "LHB")
                            {
                                //系统状态显示区域显示测量中断
                                LblShowStutas.Text = "测量中断";
                                //测量结果显示区域显示左手到位重新测量
                                TxtShowResult.Text += "左手到位，重新测量\r\n";
                                //语音提示
                                player.SoundLocation = appPath + "\\Audio\\Chinese_Left_hand_in_place_please_measure_again.wav";
                                player.Play();
                            }
                            if (calculatedMeasureDataS[i].Channel.ChannelName_English == "RHP" || calculatedMeasureDataS[i].Channel.ChannelName_English == "RHB")
                            {
                                //系统状态显示区域显示测量中断
                                LblShowStutas.Text = "测量中断";
                                //测量结果显示区域显示右手到位重新测量
                                TxtShowResult.Text += "右手手到位，重新测量\r\n";
                                PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                                LblRight.Text = "右手到位，重新测量";
                                //语音提示
                                player.SoundLocation = appPath + "\\Audio\\Chinese_right_hand_in_place_please_measure_again.wav";
                                player.Play();
                            }
                            if (calculatedMeasureDataS[i].Channel.ChannelName_English == "Frisker")
                            {
                                //系统状态显示区域显示测量中断
                                LblShowStutas.Text = "测量中断";
                                //测量结果显示区域显示衣物探头到位重新测量
                                TxtShowResult.Text += "衣物探头到位，重新测量\r\n";
                                //语音提示
                                player.SoundLocation = appPath + "\\Audio\\Chinese_frisker_In_place_measure_again.wav";
                                player.Play();
                            }
                        }
                        //重新启动本底测量（本底测量时间重新开始计时）
                        stateTimeStart = System.DateTime.Now;
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for (int j = 0; j < channelS.Count; j++)
                        {
                            calculatedMeasureDataS[j].Alpha = 0;
                            calculatedMeasureDataS[j].Beta = 0;
                        }
                        return;
                    }
                }
                //本底测量时间到
                if (stateTimeRemain < 0)
                {
                    isPlayed = false;
                    //各个手部和脚部通道显示当前测量本底值（cps）
                    string[] errRecordS = BaseCheck();
                    //本底测量判断
                    if (errRecordS == null)//本底测量通过
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //系统参数中，将上次本底测量后已测量人数清零
                        Components.SystemParameter systemParameter = new Components.SystemParameter();
                        systemParameter.ClearMeasuredCount();
                        // 运行状态标志设置为“等待测量”
                        platformState = PlatformState.ReadyToMeasure;
                        //启动本底测量计时(因为在等待测量过程中也要进行本底测量和计算) 
                        stateTimeStart = System.DateTime.Now;
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                            baseData.Add(calculatedMeasureDataS[i]);
                            //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为测量时计算做准备
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                    }
                    else//本底测量未通过
                    {
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库
                        //
                        //界面显示“本底测量未通过”同时进行语音提示
                        //
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now;
                        platformState = PlatformState.ReadyToRun;
                        //stateTimeSet = systemParameter.SelfCheckTime;
                        //重新启动自检计时
                        //stateTimeStart = System.DateTime.Now;
                    }
                    return;
                }
            }
            //运行状态为等待测量
            if (platformState == PlatformState.ReadyToMeasure)
            {
                //textBox1.Text += platformState.ToString();
                //所有手部红外到位标志，默认全部到位
                bool isHandInfraredStatus = true;
                //系统语音提示“等待测量”，并在设备运行状态区域显示“等待测量”
                //                
                //
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if (list[0].Channel.ChannelID != 7)//只对手部红外状态进行判断
                    {
                        if (list[0].InfraredStatus == 0)//手部红外状态不到位
                        {
                            //第一次判断红外状态
                            if (isFirstBackGround == true)
                            {
                                //重新启动本底计时
                                stateTimeStart = System.DateTime.Now;
                                isFirstBackGround = false;
                            }
                            //手部红外状态到位标志置false，说明手部不到位
                            isHandInfraredStatus = false;
                            //继续计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)：
                            //第k次计算本底值=第k-1次计算本底值*平滑因子/（平滑因子+1）+第k次测量值/（平滑因子+1）               
                            calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Alpha / (factoryParameter.SmoothingFactor + 1);
                            calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Beta / (factoryParameter.SmoothingFactor + 1); ;
                            calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                        }
                    }
                }
                //所有通道手部红外状态全部到位
                if (isHandInfraredStatus == true)
                {
                    //将运行状态修改为“开始测量”
                    platformState = PlatformState.Measuring;
                    //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为测量时计算做准备
                    for (int i = 0; i < channelS.Count; i++)
                    {
                        calculatedMeasureDataS[i].Alpha = 0;
                        calculatedMeasureDataS[i].Beta = 0;
                    }
                    //重新启动计时，为开始测量及时准备
                    stateTimeStart = System.DateTime.Now;
                    return;
                }
                //本底测量时间到，进行本底判断
                if (stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds <= 0)
                {                    
                    string[] errRecordS = BaseCheck();
                    //本底测量判断
                    if (errRecordS == null)//本底检测通过
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //下次如果还进行本底计算，则需重新计时，所以置标志为True
                        isFirstBackGround = true;
                        //重新启动本底测量计时
                        stateTimeStart = System.DateTime.Now;
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                            baseData.Add(calculatedMeasureDataS[i]);
                            //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次计算做准备
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                    }
                    else//本底检测未通过
                    {
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库
                        //
                        //界面显示“本底测量出现故障”同时进行语音提示
                        //
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now;
                    }
                }
            }
            //运行状态为开始测量
            if (platformState == PlatformState.Measuring)
            {
                //textBox1.Text += platformState.ToString();
                float conversionData = 0;
                //获得当前系统参数设置中的的测量时间并赋值给stateTimeSet
                //    
                //系统语音提示“正在测量”，并在设备运行状态区域显示“正在测量”
                //
                //在系统界面中显示正在测量倒计时时间（s）:系统设置测量时间-已经用时
                stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据经过平滑运算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if (list[0].Channel.ChannelID != 7)//只对手部红外状态进行判断
                    {
                        if (list[0].InfraredStatus == 0)//手部红外状态不到位
                        {
                            //进行文字和语音提示，该通道channelS[i].ChannelName不到位
                            //设定当前运行状态为“等待测量”
                            platformState = PlatformState.ReadyToRun;
                            return;
                        }
                    }
                    //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)进行累加：                               
                    calculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    calculatedMeasureDataS[i].Beta += list[0].Beta;
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    //进行语音提示
                }
                //测量时间到
                if (stateTimeRemain < 0)
                {                    
                    Components.SystemParameter systemParameter = new Components.SystemParameter();
                    //计算每个通道的计数平均值,然后减去本底值
                    for (int i = 0; i < calculatedMeasureDataS.Count; i++)
                    {
                        calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha / stateTimeSet - baseData[i].Alpha;
                        if (calculatedMeasureDataS[i].Alpha < 0)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                        }
                        ProbeParameter probeParameter = new ProbeParameter();
                        //获得当前通道的一级和二级报警阈值
                        probeParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID, "α");
                        //判断当前通道Alpha测量值是否超过报警阈值
                        if (calculatedMeasureDataS[i].Alpha > probeParameter.Alarm_1 || calculatedMeasureDataS[i].Alpha > probeParameter.Alarm_2)
                        {
                            //通过语音和文字提示当前通道Alpha测量有污染，请去污。将污染信息添加进pollutionRecord字符串
                        }
                        //获得当前系统参数设置中的测量单位                        
                        systemParameter.GetParameter();
                        //将当前通道的最终测量数据转换为系统参数设置的目标测量单位
                        conversionData = UnitConver(calculatedMeasureDataS[i].Alpha, systemParameter.MeasurementUnit);
                        //按照系统参数单位要求显示该通道最终Alpha测量结果
                        //
                        //
                        calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta / stateTimeSet - baseData[i].Beta;
                        if (calculatedMeasureDataS[i].Beta < 0)
                        {
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        probeParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID, "β");
                        if (calculatedMeasureDataS[i].Beta > probeParameter.Alarm_1 || calculatedMeasureDataS[i].Beta > probeParameter.Alarm_2)
                        {
                            //通过语音和文字提示当前通道Beta测量有污染，请去污。将污染信息添加进pollutionRecord字符串
                        }
                        conversionData = UnitConver(calculatedMeasureDataS[i].Beta, systemParameter.MeasurementUnit);
                        //按照系统参数单位要求显示该通道最终Beta测量结果
                    }
                    if (pollutionRecord == null)//说明本次测量无污染
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //语音和文字提示“无污染”
                        //
                    }
                    else
                    {
                        //将设备监测状态设置为“污染”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated);                       
                        //启动报警计时
                        alarmTimeStart = System.DateTime.Now;
                    }
                    //将本次测量数据和污染描述字符串pollutionRecord保存到数据库
                    //
                    //更新本底测量次数（+1）
                    systemParameter = new Components.SystemParameter();
                    systemParameter.UpdateMeasuredCount();
                    //运行状态设置为“测量结束”
                    platformState = PlatformState.Result;
                    return;
                }
            }
            //运行状态为“测量结束”
            if (platformState == PlatformState.Result)
            {
                //textBox1.Text += platformState.ToString();
                //本次测量无污染
                if (pollutionRecord == null)
                {
                    //获得系统参数设置中的强制本地次数
                    Components.SystemParameter systemParameter = new Components.SystemParameter();
                    systemParameter.GetParameter();
                    //获得测量总人数（从上次本底测量开始）
                    //如果测量人数大于系统设置的强制本底次数则将系统运行状态设定为“本底测量”后返回
                    if (systemParameter.MeasuredCount >= systemParameter.BkgUpdate)
                    {
                        platformState = PlatformState.BackGrouneMeasure;
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < calculatedMeasureDataS.Count; i++)
                        {
                            //衣物
                            if (calculatedMeasureDataS[i].Channel.ChannelID == 7)
                            {
                                if (calculatedMeasureDataS[i].InfraredStatus == 1)
                                {
                                    //转到衣物探测
                                    //
                                    return;
                                }
                            }
                        }
                        //设置运行状态为等待测量
                        platformState = PlatformState.ReadyToRun;
                        return;
                    }
                }
                else//有污染
                {
                    //设置运行状态为本底测量
                    platformState = PlatformState.BackGrouneMeasure;
                    return;
                }
            }
        }

        /// <summary>
        /// 本底检测
        /// </summary>
        /// <returns></returns>
        private string[] BaseCheck()
        {
            //自检是否通过标志，默认true
            bool isCheck = true;
            //故障记录字符串
            string errRecord = null;//中文
            string errRecord_E = null;//英文
            for (int i = 0; i < channelS.Count; i++)
            {
                //获得当前通道的道盒参数
                ChannelParameter channelParameter = new ChannelParameter();
                channelParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID);
                //判断当前高压值：当前高压值若为系统设定值的0.8~1.2倍之内，则该通道高压正常，否则高压故障,将故障信息添加到errRecord字符串，isCheck = false;
                if (calculatedMeasureDataS[i].HV<channelParameter.PresetHV * (1 - PlatForm.ErrorRange.HV_ERROR) || calculatedMeasureDataS[i].HV > channelParameter.PresetHV * (1 + PlatForm.ErrorRange.HV_ERROR))
                {
                    //高压故障,将故障信息添加到errRecord字符串
                    errRecord += string.Format("高压故障,设置值:{0}V,实测值:{1}V;", channelParameter.PresetHV.ToString(), calculatedMeasureDataS[i].HV.ToString());
                    errRecord_E += string.Format("HV Fault,Preset:{0}V,Actual:{1}V;", channelParameter.PresetHV.ToString(), calculatedMeasureDataS[i].HV.ToString());
                    //设置isCheck为false
                    isCheck = false;
                }
                if (platformState == PlatformState.SelfTest)
                {
                    //根据仪器检测类型计算最终自检数据
                    switch (factoryParameter.MeasureType)
                    {
                        case "α":
                            //计算Alpha自检数据值
                            calculatedMeasureDataS[i].Alpha = (calculatedMeasureDataS[i].Alpha + BASE_DATA * 2) / stateTimeSet * 2;
                            break;
                        case "β":
                            //计算Beta自检数据
                            calculatedMeasureDataS[i].Beta = (calculatedMeasureDataS[i].Beta + BASE_DATA * 2) / stateTimeSet * 2;
                            break;
                        case "α/β":
                            //同时计算Alpha和Beta自检数据
                            calculatedMeasureDataS[i].Alpha = (calculatedMeasureDataS[i].Alpha + BASE_DATA * 2) / stateTimeSet * 2;
                            calculatedMeasureDataS[i].Beta = (calculatedMeasureDataS[i].Beta + BASE_DATA * 2) / stateTimeSet * 2;
                            break;
                    }                    
                    //判断电子线路故障：如果本底值在设定值的0.7~1.3倍之内，则显示电子线路正常，否则显示电子线路故障
                    if (factoryParameter.MeasureType != "β")
                    {
                        //对Alpha本底值(BASE_DATA)进行判断，如果故障提示“α线路故障”同时将故障信息添加到errRecord字符串，isCheck = false;
                        if(calculatedMeasureDataS[i].Alpha< channelParameter.AlphaThreshold* (1-PlatForm.ErrorRange.BASE_ERROR)||calculatedMeasureDataS[i].Alpha> channelParameter.AlphaThreshold* (1+PlatForm.ErrorRange.BASE_ERROR))
                        {
                            //将故障信息添加到error字符串
                            errRecord += string.Format("α电子线路故障;");
                            errRecord_E += string.Format("Alpha Channel Fault;");
                            //设置isCheck为false
                            isCheck = false;
                        }
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        //对Beta本底值(BASE_DATA)进行判断，如果故障提示“β线路故障”同时将故障信息添加到errRecord字符串,isCheck = false;
                        if (calculatedMeasureDataS[i].Beta < channelParameter.BetaThreshold * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataS[i].Beta > channelParameter.BetaThreshold * (1 + PlatForm.ErrorRange.BASE_ERROR))
                        {
                            //将故障信息添加到error字符串
                            errRecord += string.Format("β电子线路故障;");
                            errRecord_E+= string.Format("Beta Channel Fault;");
                            //设置isCheck为false
                            isCheck = false;
                        }
                    }
                    //判断根据红外状态判断该通道红外是否故障，自检时无检查体但提示到位，说明红外故障
                    if (calculatedMeasureDataS[i].InfraredStatus == 1)//红外状态到位
                    {
                        //红外故障,将故障信息添加到errRecord字符串
                        errRecord += string.Format("红外故障;");
                        errRecord_E += string.Format("Sensor fault;");
                        isCheck = false;
                    }                                
                }
                if (platformState == PlatformState.BackGrouneMeasure)
                {
                    ProbeParameter probeParameter = new ProbeParameter();
                    if (factoryParameter.MeasureType != "β")
                    {
                        //查询当前通道的α本底上限、本底下限
                        probeParameter.GetParameter(channelS[i].ChannelID, "α");
                        //进行α本底测量判断
                        if (calculatedMeasureDataS[i].Alpha < probeParameter.LBackground) //超过当前通道的本底下限
                        {
                            //该通道channelS[i].ChannelName的本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("α本底下限值:{0}cps,当前本底值:{1}cps;",probeParameter.LBackground.ToString(),calculatedMeasureDataS[i].Alpha.ToString());
                            isCheck = false;
                        }
                        if (calculatedMeasureDataS[i].Alpha >= probeParameter.HBackground)//超过当前通道的本底上限
                        {
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("α本底上限值:{0}cps,当前本底值:{1}cps;", probeParameter.HBackground.ToString(), calculatedMeasureDataS[i].Alpha.ToString());
                            isCheck = false;
                        }
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        //查询当前通道的β本底上限、本底下限
                        probeParameter.GetParameter(channelS[i].ChannelID, "β");
                        if (calculatedMeasureDataS[i].Beta < probeParameter.LBackground)//超过当前通道的本底下限
                        {
                            //该通道channelS[i].ChannelName本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("β本底下限值:{0}cps,当前本底值:{1}cps;", probeParameter.LBackground.ToString(), calculatedMeasureDataS[i].Beta.ToString());
                            isCheck = false;
                        }
                        if (calculatedMeasureDataS[i].Beta >= probeParameter.HBackground)//超过当前通道的本底上限
                        {
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("β本底上限值:{0}cps,当前本底值:{1}cps;", probeParameter.HBackground.ToString(), calculatedMeasureDataS[i].Beta.ToString());
                            isCheck = false;
                        }
                    }
                }
                //根据是否检测通过设置该通道的背景颜色                
                if (errRecord != null)//故障信息不为空
                {
                    //显示故障信息,故障点为：calculatedMeasureDataS[i].Channel.ChannelName
                    PicShowStatus.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                    if (platformState == PlatformState.SelfTest)
                    {
                        LblShowStutas.Text = "仪器故障";
                    }
                    TxtShowResult.Text += calculatedMeasureDataS[i].Channel.ChannelName + errRecord + "\r\n";
                    //对应通道名字文本框背景色显示为ERROR
                    ((TextBox)(this.Controls[string.Format("Txt{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                }
                else
                {
                    //设备状态区域背景色显示为NORMAL
                    PicShowStatus.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                    if (platformState == PlatformState.SelfTest)
                    {
                        //设备状态区域文字提示“仪器正常”
                        LblShowStutas.Text = "仪器正常";
                    }
                    //对应通道名字文本框背景色显示为NORMAL
                    ((TextBox)(this.Controls[string.Format("Txt{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                }
            }
            if (isCheck == false)//未通过
            {
                string[] errRecordS = new string[2];
                errRecordS[0] = errRecord;
                errRecordS[1] = errRecord_E;
                return errRecordS;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 测量数据单位换算，将cps单位数据data换算为目标单位后返回
        /// </summary>
        /// <param name="data">需换算的测量数据值(cps)</param>
        /// <param name="unit">要换算的目标单位</param>
        /// <returns>目标单位值</returns>
        private float UnitConver(float data, string unit)
        {
            //将data（单位为cps）换算为目标单位unit后返回
            //cpm：最终测量计数平均值(cpm) = 60 * 计算平均值(cps)
            //Bq: 最终测量计数平均值(Bq) = 200 * 计算平均值(cps) /探测效率
            //Bq / cm2 : 最终测量计数平均值(Bq / cm2) = 200 * 计算平均值(cps) /探测效率/ 该通道测量面积
            //KBq / cm2:最终测量计数平均值(KBq / cm2) = 200 * 计算平均值(cps) /探测效率/ 该通道测量面积 / 1000
            //dpm:最终测量计数平均值(dpm) = 12000 * 计算平均值(cps) /探测效率
            //nCi : 最终测量计数平均值(nCi) = 200 * 计算平均值(cps) /探测效率*0.027
            return 0;
        }

        private void bkWorkerReportStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            //如果没有取消异步线程
            if (bkWorkerReportStatus.CancellationPending == false)
            {
                //在异步线程上执行串口读操作ReadDataFromSerialPort方法
                BackgroundWorker bkWorkerReport = sender as BackgroundWorker;
                ReadDataFromSerialPortReport(bkWorkerReport, e);
            }
            e.Cancel = true;
        }
        private void ReadDataFromSerialPortReport(BackgroundWorker worker, DoWorkEventArgs e)
        {

        }

        private void TmrDispTime_Tick(object sender, EventArgs e)
        {
            //更新当前显示时间
            LblTime.Text = DateTime.Now.ToLongTimeString();
            //更新剩余时间：系统自检设置时间-已经用时
            //stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
            ////更新当前系统运行状态剩余时间
            //LblTimeRemain.Text = stateTimeRemain.ToString();
        }
    }
}

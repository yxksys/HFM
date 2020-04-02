using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using HFM.Components;

namespace HFM
{
    public partial class FrmMeasureMain : Form
    {       
        bool isEnglish = false;//当前语言，默认中文
        //监测时间
        int checkTime = 0;
        //系统报警时间长度设置
        int alarmTimeSet = 0;
        //系统报警时间计时
        DateTime alarmTimeStart = DateTime.Now;
        int stateTimeSet = 0;//系统当前运行状态的检测时间设置
        int stateTimeRemain = 0;//系统当前运行状态剩余时间
        int errNumber = 0; //报文接收出现错误计数器   
        int throwDataCount = 0;//准备检测阶段预读取数据扔掉次数
        int clothesTimeCount = 0;//衣物离线时间计数器，每秒计数一次
        /// <summary>
        /// clothesStatus配合衣物探头红外状态来进行判断
        /// 衣物探头红外状态-clothesStatus：
        /// 00：红外未到位，衣物探头处于还未被拿起状态，说明衣物探头已经被放下一段时间（至少1s）
        /// 01：红外未到位，衣物探头处于已经被拿起状态，说明衣物探头刚刚被放下
        /// 10：红外到位，衣物探头处于还为被拿起状态，说明衣物探头刚被拿起
        /// 11：红外到位，衣物探头处于已经被拿起状态，说明衣物探头已经被拿起一段时间（至少1s）
        /// </summary>
        int clothesStatus = 0;//衣物探头状态。0：衣物探头还未被拿起，1：衣物探头已经被拿起
        int[] lastInfraredStatus = new int[3];//记录上一个数据包红外状态，分别为“左手、右手、衣物”，本底测量中，如果本次红外到位而上次不到位则进行语音播报，如果上次红外到位本次也红外到位，则不需要重复播报提示
        bool isSelfCheckSended = false;
        bool isFirstBackGround = true;//进入等待测量状态后的本底测量计时标志
        string pollutionRecord = null;//记录测量污染详细数据
        string pollutionRecord_E = null;//记录测量污染详细数据(英文)        
        //衣物探测界面
        FrmClothes frmClothes = null;
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
        /// <summary>
        /// 设备状态：1个字节，0001 0000监测正常，0010 0000监测失败，0100 0000监测污染
        /// </summary>
        enum DeviceStatus 
        {
            OperatingNormally=16,
            OperatingFaulted=32,
            OperatingContaminated=64
        }
        byte deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
        const int BASE_DATA = 1000;       
        DateTime stateTimeStart = DateTime.Now;//系统当前运行状态的开始计时变量        
        //创建音频播放对象
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        CommPort commPort = new CommPort();//监测端口
        CommPort commPort_Supervisory = new CommPort();//和管理机通信端口
        Thread IOThread = null;
        float smoothedDataOfClothes = 0;//平滑处理后的衣物测量值
        float baseDataOfClothes = 0;//衣物探头本底值
        int alarmCountOfClothes = 0;//衣物检测报警次数
        string clotheseNuclideUsed = "U_235";//衣物检测核素选择,默认U_235
        string alphaNuclideUsed = "U_235";//Alpha核素选择，默认U_235
        string betaNuclideUsed = "U_235";//Beta核素选择，默认U_235
        //float clotheseEfficiency = 0;//衣物检测探测效率    
        FactoryParameter factoryParameter = new FactoryParameter();//工厂参数        
        Components.SystemParameter systemParameter = new Components.SystemParameter();//系统参数
        Channel[] channelsAll=new Channel[7];//全部通道
        IList<Channel> channelS = new List<Channel>();//当前可使用的检测通道,即全部启用的监测通道
        IList<MeasureData> baseData = new List<MeasureData>(); //存储本底计算结果，用例对测量数据进行校正
        IList<MeasureData> calculatedMeasureDataS = new List<MeasureData>();//存储各个通道最终计算检测值的列表
        IList<EfficiencyParameter> efficiencyParameterS = new List<EfficiencyParameter>();//存储探测效率参数列表
        IList<ProbeParameter> probeParameterS = new List<ProbeParameter>();//存储探测参数的列表  
        IList<ChannelParameter> channelParameterS = new List<ChannelParameter>();//存储道盒参数列表
        const int TEAM_LENGTH= 240;
        struct SMOOTHINGDATA
        {
            //平滑数组            
            public UInt32[] team;
            //平滑滑动位置
            public UInt32 team_i;
            //滑动 0：没满 1：超过120组
            public UInt32 team_Full;
            //均值
            public UInt32 average;
            public UInt32 count_to;
            //数据累加和
            public ulong sum;
            //计数120s，判断有没数据产生
            public UInt32 lingCycle;
            //连续多次很短时间产生数据
            public char cout_tu;
            //5s本次和值
            public UInt32 s5_Sum;
            //5s上次和值
            public UInt32 s5_Sum_Last;
            //5s位置
            public UInt32 s5_Sum_i;
            //10s本次和值
            public UInt32 s10_Sum;
            //10s上次和值
            public UInt32 s10_Sum_Last;
            //10s位置
            public UInt32 s10_Sum_i;
            public char lengthLocal;
            public char cycle_Length_i;
            public char team_Length;
            public ulong sumAll;
            public ulong sumLength;
            public UInt32 com_Count;
            public UInt32[] baseTime;
            public UInt32 baseData_i;
            public UInt32 baseData_Full;
            public UInt32 baseSum;
        }
        SMOOTHINGDATA smoothingData;
        [StructLayout(LayoutKind.Sequential)]
        struct SYSTEMTIME //系统时间结构体
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
        float[] poissonTable_1 =new float[50]{ 0, 3, 0, 4, 0, 6, 0, 7, 1, 9,
                                             1,10,2,11,3,13,3,14,4,15,
                                             5,16,6,18,6,19,7,20,8,21,
                                             9,23,10,24,10,25,11,26,12,27,
                                            17,34,18,36,19,37,19,38,20,39 };
        float[] poissonTable = new float[50]{ 0, 4, 0, 6, 0, 8, 0, 10, 0, 11,
                                             1,13,1,14,2,16,2,17,3,18,
                                             4,20,4,21,5,22,6,24,6,25,
                                             7,26,8,28,8,29,9,30,10,31,
                                            11,33,11,34,12,35,13,36,14,38 };
        int[] cycleLength = new int[8] { 16, 25, 36, 49, 64, 81, 100, 121 };
        const double POISSONUA_2 = 1.658;
        const double POISSONUA = 2.5758;
        const double POISSONUA2_4 = 0.676;
        const int LONGTIME = 60;
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
            PnlFrisker.Parent = PicFrisker;
            foreach (MeasureData measureData in measureDataS)
            {
                //找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", measureData.Channel.ChannelName_English)]);
                //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                label = (Label)(panel.Controls[string.Format("Lbl{0}", measureData.Channel.ChannelName_English)]);
                if (measureData.Channel.ChannelName_English == "Frisker")
                {
                    //显示衣物测量结果
                    continue;
                }                
                switch (factoryParameter.MeasureType)
                {
                    case "α":                       
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F1") + unit;
                        break;
                    case "β":                        
                        ////找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                        //pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", measureData.Channel.ChannelName_English)]);
                        ////找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                        //panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                        ////找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                        //label= (Label)(panel.Controls[string.Format("Lbl{0}", measureData.Channel.ChannelName_English)]);
                        //Label控件显示测量值
                        label.Text = "β" + measureData.Beta.ToString("F1") + unit;                        
                        break;
                    case "α/β":
                        ////找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                        //pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", measureData.Channel.ChannelName_English)]);
                        ////找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                        //panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                        ////找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                        //label = (Label)(panel.Controls[string.Format("Lbl{0}", measureData.Channel.ChannelName_English)]);
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
        /// <summary>
        /// 将中英文故障记录添加到数据库中
        /// </summary>
        /// <param name="errRecordS">中文和英文故障记录数据</param>
        private void AddErrorData(string[] errRecordS)
        {
            ErrorData errorData = new ErrorData
            {
                //中文描述
                Record = errRecordS[0],
                IsEnglish = false
            };
            errorData.ErrTime = DateTime.Now;
            errorData.AddData(errorData);
            //英文描述
            errorData.Record = errRecordS[1];
            errorData.IsEnglish = true;
            errorData.AddData(errorData);
        }        
        private void FrmMeasureMain_Load(object sender, EventArgs e)
        {
            BtnChinese.Enabled = false;
            smoothingData.team = new UInt32[TEAM_LENGTH];
            //获得工厂参数设置信息           
            factoryParameter.GetParameter();
            //获得系统参数设置信息
            systemParameter.GetParameter();            
            Nuclide nuclide = new Nuclide();
            clotheseNuclideUsed = nuclide.GetClothesNuclideUser(); //获得用户衣物探测核素选择
            alphaNuclideUsed = nuclide.GetAlphaNuclideUser();//获得用户Alpha探测核素选择
            betaNuclideUsed = nuclide.GetBetaNuclideUser();//获得用户Beta探测核素选择
            EfficiencyParameter efficiencyParameter = new EfficiencyParameter();
            efficiencyParameterS = efficiencyParameter.GetParameter();//获得全部效率参数            
            //获得所有探测参数
            ProbeParameter probeParameter = new ProbeParameter();           
            probeParameterS = probeParameter.GetParameter();
            //获得所有道盒参数
            ChannelParameter channelParameter = new ChannelParameter();
            channelParameterS = channelParameter.GetParameter();
            //获得全部通道信息
            Channel channel = new Channel();            
            channelS = channel.GetChannel();
            channelS.CopyTo(channelsAll,0);//将全部通道信息复制到channelsAll中
            //初始化显示界面
            DisplayInit();
            //实例化衣物探测界面
            frmClothes = new FrmClothes();                      
            checkTime = systemParameter.SelfCheckTime;//检测时间 
            alarmTimeSet = systemParameter.AlarmTime;//报警时间                                  
            //在界面中将启用通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，其它通道（通道未启用或探测面积为0）的控件Enabled无效；                       
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
            //初始化左手、右手和衣物的红外状态，默认都不到位
            for(int i=0;i<3;i++)
            {
                lastInfraredStatus[i] = 0;
            }
            //正常使用的通道测量数值显示（初始值为0cps）
            DisplayMeasureData(calculatedMeasureDataS,"cps");            
            //打开串口
            if (commPort.Opened == true)
            {
                commPort.Close();
            }
            //从配置文件获得当前监测串口配置
            commPort.GetCommPortSet("commportSet");
            //打开监测通信串口
            try
            {
                commPort.Open();
            }
            catch
            {
                if (isEnglish)
                {
                    MessageBox.Show("Measuring Comm open fault！Please check the serial Port!");
                    TxtShowResult.Text = "Measuring Comm open fault";
                }
                else
                {
                    MessageBox.Show("监测端口打开错误！请检查通讯是否正常。");
                    TxtShowResult.Text = "串口打开失败";
                }
                return;
            }
            //从配置文件获得当前管理机串口通信配置
            commPort_Supervisory.GetCommPortSet("commportSetOfReport");
            //打开管理机通信串口
            try
            {
                commPort_Supervisory.Open();
            }
            catch
            {
                if (isEnglish)
                {
                    MessageBox.Show("Supervisor Comm open fault！Please check the serial Port!");
                    TxtShowResult.Text = "Supervisor Comm open fault";
                }
                else
                {
                    MessageBox.Show("管理机端口打开错误！请检查通讯是否正常。");
                    TxtShowResult.Text = "管理机串口打开失败";
                }
                return;
            }
            MeasureIO measureIO = new MeasureIO();
            IOThread = new Thread(measureIO.HFMeasureIO);
            IOThread.IsBackground = true;
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
            int errorNumber = 0; //下发自检报文出现错误计数器
            int delayTime = 200;//下发自检报文延时时间            
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
                    byte[] receiveBuffMessage = new byte[124];
                    receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                    //延时
                    if (platformState == PlatformState.SelfTest && isSelfCheckSended == false)
                    {
                        Thread.Sleep(900-delayTime);
                    }
                    else
                    {
                        Thread.Sleep(900);
                    }
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
                    if (isEnglish)
                    {
                        TxtShowResult.Text = "Communication Error！";
                    }
                    else
                    {
                        TxtShowResult.Text = "通讯错误！";
                    }
                }
                else
                {
                    errNumber++;
                }
                return;
            }
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            measureDataS = Components.Message.ExplainMessage<MeasureData>(receiveBufferMessage); 
            if(measureDataS==null)//解析失败
            {
                return;
            }
            for(int i=0;i<channelsAll.Count();i++)
            {
                measureDataS[i].Channel = channelsAll[i];
            }
            //衣物探头被启用
            if (measureDataS[6].Channel.IsEnabled == true)
            {
                //衣物探头已经被拿起（红外状态为到位)
                if (measureDataS[6].InfraredStatus == 1)
                {
                    //衣物探头已经被拿起（不是刚被拿起）
                    smoothedDataOfClothes = SmoothData((UInt32)measureDataS[6].Alpha);
                    //设置衣物探测结果显示区域背景色为STATUS
                    PicFrisker.BackColor = PlatForm.ColorStatus.COLOR_STATUS;
                    //如果当前状态为等待测量或开始测量则,说明衣物探测界面已经加载
                    if (platformState == PlatformState.ReadyToMeasure || platformState == PlatformState.Measuring)
                    {
                        //获得当前衣物检测通道的探测参数
                        IList<ProbeParameter> clothesProbeParmeter = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == 7).ToList();
                        //衣物离线时间计数器+1（1s）
                        clothesTimeCount++;
                        //衣物探头刚刚被拿起（红外状态为到位，衣物探头状态clothesStatus为0（还未被拿起），说明衣物探头是刚被拿起）  
                        if (clothesStatus == 0)
                        {
                            //置衣物探头状态为：已经被拿起
                            clothesStatus = 1;
                            //重新开始衣物离线时间计数
                            clothesTimeCount = 0;
                            if(frmClothes.IsDisposed)//如果窗体已经被释放，则重新创建
                            {
                                frmClothes = new FrmClothes();                               
                            }
                            //设置窗体进度条状态
                            frmClothes.loadingCircle.Active = true;
                            frmClothes.PrgClothAlarm_2.Maximum = (int)clothesProbeParmeter[0].Alarm_2;
                            frmClothes.PrgClothAlarm_1.Maximum = (int)clothesProbeParmeter[0].Alarm_1;
                            //frmClothes.PrgClothAlarm_1.Width = (int)(frmClothes.PrgClothAlarm_2.Width * clothesProbeParmeter[0].Alarm_1 / clothesProbeParmeter[0].Alarm_2);
                            //显示衣物探头监测窗口                    
                            frmClothes.Show();
                            return;
                        }
                        //衣物探头已经被拿起（红外状态为到位，衣物探头状态clothesStatus为1（已被拿起），说明衣物探头已经被拿起一段时间（至少1s））
                        if (clothesStatus == 1)
                        {                            
                            //衣物探测界面显示本底值(单位cps)
                            frmClothes.TxtBackgroundValue.Text = string.Format("{0}cps", baseDataOfClothes.ToString("F1"));
                            //从当前探测值中减去本底值
                            smoothedDataOfClothes = (smoothedDataOfClothes - baseDataOfClothes) < 0 ? 0 : smoothedDataOfClothes - baseDataOfClothes;
                            //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                            IList<EfficiencyParameter> efficiencyParameterNow= efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "C" && efficiencyParameter.Channel.ChannelID == 7 && efficiencyParameter.NuclideName == clotheseNuclideUsed).ToList();
                            //根据系统参数中设置的检测单位，对减去本底值后的测量值进行单位变换并在衣物探测界面中进行显示
                            float converedData = UnitConver(smoothedDataOfClothes, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, measureDataS[6].Channel.ProbeArea);
                            frmClothes.TxtMeasureValue.Text = string.Format("{0}{1}", converedData.ToString("F1"), systemParameter.MeasurementUnit);
                            #region 如果减去本底值后的测量值大于一级报警，说明有污染  
                            ////获得当前衣物检测通道的探测参数
                            //IList<ProbeParameter> clothesProbeParmeter = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == 7).ToList();
                            if (smoothedDataOfClothes > clothesProbeParmeter[0].Alarm_1)
                            {
                                //大于二级报警，衣物探测界面测量结果显示文本框背景色设置为ALATM2
                                if (smoothedDataOfClothes > clothesProbeParmeter[0].Alarm_2)
                                {
                                    frmClothes.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                                    frmClothes.PrgClothAlarm_2.Value = 100;
                                }
                                else
                                {
                                    //大于一级报警，衣物探测界面测量结果显示文本框背景色设置为ALATM1
                                    frmClothes.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                                    frmClothes.PrgClothAlarm_1.Value = 100;
                                }
                                //衣物探测界面进度条设置为100%
                                //frmClothes.PrgClothAlarm_1.Value = 100;
                                frmClothes.loadingCircle.Active = false;
                                //报警次数+1
                                alarmCountOfClothes++;
                                //如果连续三次出现污染报警（污染报警计数器超过3）
                                if (alarmCountOfClothes > 2)
                                {
                                    //探测主界面的探测结果显示区域显示衣物污染
                                    if (isEnglish)
                                    {
                                        TxtShowResult.Text += "Clothing Contaminated\r\n";
                                        player.SoundLocation = appPath + "\\Audio\\English_Decontaminate_please.wav";
                                    }
                                    else
                                    {
                                        TxtShowResult.Text += "衣物污染\r\n";
                                        //语音提示被测人员污染
                                        player.SoundLocation = appPath + "\\Audio\\Chinese_Decontaminate_please.wav";
                                    }
                                    player.Play();
                                    //Thread.Sleep(2000);
                                    //将测量时间（当前时间）、状态（“污染”）、详细信息（“衣物探头”+测量值）写入数据库                                
                                    MeasureData measureData = new MeasureData();
                                    measureData.MeasureDate = DateTime.Now;
                                    measureData.MeasureStatus = "污染";
                                    measureData.DetailedInfo = string.Format("衣物探头{0}{1}", converedData.ToString("F1"), systemParameter.MeasurementUnit);
                                    measureData.IsEnglish = false;
                                    measureData.AddData(measureData);
                                    measureData.MeasureStatus = "Contaminated";
                                    measureData.DetailedInfo = string.Format("Frisker{0}{1}", converedData.ToString("F1"), systemParameter.MeasurementUnit);
                                    measureData.IsEnglish = true;
                                    measureData.AddData(measureData);
                                    //启动报警计时
                                    alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                                }
                            }
                            #endregion
                            #region 如果减去本底值后的测量值未大于一级报警，说明没有污染
                            else
                            {
                                //设置衣物探测界面进度条变化百分比
                                int prgBarValue = (int)(smoothedDataOfClothes / efficiencyParameterNow[0].Efficiency);
                                frmClothes.PrgClothAlarm_1.Value = prgBarValue>=frmClothes.PrgClothAlarm_1.Maximum?frmClothes.PrgClothAlarm_1.Maximum: prgBarValue;
                                frmClothes.PrgClothAlarm_2.Value= prgBarValue >= frmClothes.PrgClothAlarm_2.Maximum ? frmClothes.PrgClothAlarm_2.Maximum : prgBarValue;
                                //衣物探测界面测量结果显示文本框背景色设置为SYSTEM
                                frmClothes.TxtMeasureValue.BackColor = PlatForm.ColorStatus.COLOR_SYSTEM;
                                //报警计数器归0
                                alarmCountOfClothes = 0;
                                //语音提示dida2.wav                    
                                player.SoundLocation = appPath + "\\Audio\\dida2.wav";
                                player.Play();
                                //Thread.Sleep(200);
                            }
                            #endregion
                            return;
                        }
                    }
                }
                //衣物探头红外未到位
                if (measureDataS[6].InfraredStatus == 0)
                {                    
                    //衣物探测显示区域背景色为ERROR
                    PicFrisker.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                    if (clothesStatus == 1)//探头当前状态为已经被拿起，说明衣物探头刚刚被放下
                    {
                        //关闭衣物探头监测界面
                        frmClothes.Close();
                        //语音提示dida1.wav
                        player.SoundLocation = appPath + "\\Audio\\dida1.wav";
                        player.Play();
                        //Thread.Sleep(200);
                        //设置衣物探头状态为0，已经被放下
                        clothesStatus = 0;
                        #region 如果衣物离线时间大于系统设置的离线自检时间
                        if (clothesTimeCount > systemParameter.ClothOfflineTime)
                        {
                            //重置衣物探测实践次数
                            clothesTimeCount = 0;
                            //监测结果显示区域显示“仪器本底测量”
                            if (isEnglish)
                            {
                                TxtShowResult.Text += "Updating background\r\n";
                                player.SoundLocation = appPath + "\\Audio\\English_Updating_background.wav";
                            }
                            else
                            {
                                TxtShowResult.Text += "仪器本底测量\r\n";
                                //语音提示本底测量
                                player.SoundLocation = appPath + "\\Audio\\Chinese_Background_measure.wav";
                            }
                            player.Play();
                            //Thread.Sleep(1000);
                            //将当前系统状态设置为本底测量
                            platformState = PlatformState.BackGrouneMeasure;
                            return;
                        }
                        return;
                        #endregion
                    }
                    //对衣物探头测量数据进行平滑处理
                    smoothedDataOfClothes = SmoothData((UInt32)measureDataS[6].Alpha);
                    if (platformState == PlatformState.BackGrouneMeasure || platformState == PlatformState.Measuring)
                    {
                        //将当前平滑处理后的检测值作为本底值
                        baseDataOfClothes = smoothedDataOfClothes;
                        //在衣物检测结果显示区域显示当前测量值，根据数据大小控制显示字体大小
                        if (smoothedDataOfClothes < 10)
                        {
                            LblFrisker.Font = new Font("宋体", 36, FontStyle.Bold);
                            LblFrisker.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F1"));
                        }
                        else if (smoothedDataOfClothes < 1000)
                        {
                            LblFrisker.Font = new Font("宋体", 36, FontStyle.Bold);
                            LblFrisker.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F0"));
                        }
                        else
                        {
                            LblFrisker.Font = new Font("宋体", 26, FontStyle.Bold);
                            LblFrisker.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F0"));
                        }
                    }
                }
            }
            //如果当前运行状态为“运行准备”
            if (platformState == PlatformState.ReadyToRun)
            {                            
                //如果手部探测器为启用状态，则判断左右手是否到位，到位则相应指示框背景变为绿色，否则为橙色，同时进行文字信息提示（具体操作可参考老版本源代码）
                if (channelS.FirstOrDefault(channel => channel.ChannelName_English == "LHP")!=null)//左手心启用,左右手全部启用（手心手背四个通道）
                {                    
                    if(measureDataS[0].InfraredStatus!=1)//左手红外不到位（左手心和左手背红外状态一致，所以判断左手心即可）
                    {
                        PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        if (isEnglish)
                        {
                            LblLeft.Text = "LH not in place";
                        }
                        else
                        {
                            LblLeft.Text = "左手不到位";
                        }
                    }
                    else
                    {
                        PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                        if (isEnglish)
                        {
                            LblLeft.Text = "LH in place";
                        }
                        else
                        {
                            LblLeft.Text = "左手到位";
                        }
                    }
                    if(measureDataS[2].InfraredStatus!=1)//右手红外不到位（有手心和右手背红外状态一致，所以判断右手心即可）
                    {
                        PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        if (isEnglish)
                        {
                            LblRight.Text = "RH not in place";
                        }
                        else
                        {
                            LblRight.Text = "右手不到位";
                        }
                    }
                    else
                    {
                        PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                        if (isEnglish)
                        {
                            LblRight.Text = "RH in place";
                        }
                        else
                        {
                            LblRight.Text = "右手到位";
                        }
                    }
                }
                //当前运行状态设置为“仪器自检”
                platformState = PlatformState.SelfTest;
                if (isEnglish)
                {
                    LblShowStutas.Font = new Font("宋体", 28, FontStyle.Bold);
                    LblShowStutas.Text = "Self-checking";
                    TxtShowResult.Text += "Self-checking\r\n";                    
                    player.SoundLocation = appPath + "\\Audio\\English_Self_checking.wav";
                }
                else
                {
                    LblShowStutas.Font = new Font("宋体", 48,FontStyle.Bold);
                    LblShowStutas.Text = "仪器自检";
                    TxtShowResult.Text += "仪器自检\r\n";
                    //IOThread.Start("SelfTest");                
                    //IOThread.Suspend();
                    player.SoundLocation = appPath + "\\Audio\\Chinese_Self_checking.wav";
                }
                player.Play();
                stateTimeStart = System.DateTime.Now.AddSeconds(1);
                //Thread.Sleep(1000);
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
                LblTimeRemain.Text = stateTimeRemain < 0 ? "0" : stateTimeRemain.ToString();                        
                for (int i = 0; i < channelS.Count; i++) //遍历全部启用的检测通道
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
                    //判断红外状态，在手部状态区域进行相应提示
                    if (calculatedMeasureDataS[i].InfraredStatus == 1)//红外到位
                    {
                        //左手
                        if (calculatedMeasureDataS[i].Channel.ChannelID == 1 || calculatedMeasureDataS[i].Channel.ChannelID ==2)
                        {
                            if (isEnglish)
                            {
                                LblLeft.Text = "LH in place";
                            }
                            else
                            {
                                LblLeft.Text = "左手到位";
                            }
                            PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                        }
                        //右手
                        if((calculatedMeasureDataS[i].Channel.ChannelID == 3 || calculatedMeasureDataS[i].Channel.ChannelID ==4))
                        {
                            if (isEnglish)
                            {
                                LblRight.Text = "RH in place";
                            }
                            else
                            {
                                LblRight.Text = "右手到位";
                            }
                            PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                        }
                    }
                    else//红外不到位
                    {
                        //左手
                        if (calculatedMeasureDataS[i].Channel.ChannelID ==1 || calculatedMeasureDataS[i].Channel.ChannelID ==2)
                        {
                            if (isEnglish)
                            {
                                LblLeft.Text = "LH not in place";
                            }
                            else
                            {
                                LblLeft.Text = "左手不到位";
                            }
                            PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        }
                        //右手
                        if ((calculatedMeasureDataS[i].Channel.ChannelID == 3 || calculatedMeasureDataS[i].Channel.ChannelID ==4))
                        {
                            if (isEnglish)
                            {
                                LblRight.Text = "RH not in place";
                            }
                            else {
                                LblRight.Text = "右手不到位";
                            }
                            PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        }
                    }
                }
                //界面中对应通道显示当前测量值
                DisplayMeasureData(calculatedMeasureDataS,"cps");
                //自检时间到
                if (stateTimeRemain < 0)
                {                  
                    string[] errRecordS = new string[2];
                    errRecordS = BaseCheck();
                    if (errRecordS == null)//自检通过
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        // 运行状态标志设置为“本底测量”
                        platformState = PlatformState.BackGrouneMeasure;
                        //启动本底测量计时 
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        //系统状态显示区域显示本底测量
                        if (isEnglish)
                        {
                            LblShowStutas.Font = new Font("宋体", 28, FontStyle.Bold);
                            LblShowStutas.Text = "Updating Background";
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "Updating Background\r\n";
                            //系统提示本底测量                                                
                            player.SoundLocation = appPath + "\\Audio\\English_Updating_background.wav";
                        }
                        else
                        {
                            LblShowStutas.Font = new Font("宋体", 48, FontStyle.Bold);
                            LblShowStutas.Text = "本底测量";
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "本底测量\r\n";
                            //系统提示本底测量                                                
                            player.SoundLocation = appPath + "\\Audio\\Chinese_Background_measure.wav";
                        }
                        player.Play();
                    }
                    else
                    {
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库    
                        AddErrorData(errRecordS);
                        //语音提示故障
                        if (isEnglish)
                        {
                            player.SoundLocation = appPath + "\\Audio\\English_Self-checking_fault.wav";
                        }
                        else
                        {
                            player.SoundLocation = appPath + "\\Audio\\Chinese_Self-checking_fault.wav";
                        }
                        player.Play();
                        //Thread.Sleep(3000);
                        //测量数据存储全部清零
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        isSelfCheckSended = false;
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        //重新启动自检计时
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        return;
                    }                   
                }                
                return;
            }
            //运行状态为本底测量
            if (platformState == PlatformState.BackGrouneMeasure)
            {
                bool isReDisplay = false; //是否需要重新显示"本底测量"提示信息,默认false
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
                         //显示当前calculatedMeasureDataS[i].Channel.ChannelName对应的红外到位
                         if (calculatedMeasureDataS[i].Channel.ChannelID ==1 || calculatedMeasureDataS[i].Channel.ChannelID ==2)
                         {                                                        
                            if (lastInfraredStatus[0] == 0) //左手上次为红外不到位，本次为红外到位，说明需要重新进行语音提示
                            {
                                //系统状态显示区域显示测量中断
                                if (isEnglish)
                                {
                                    LblShowStutas.Text = "Interrupted";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "Left hand in place,Please measure again!\r\n";
                                    player.SoundLocation = appPath + "\\Audio\\English_Left_hand_in_place_please_measure_again.wav";
                                }
                                else
                                {
                                    LblShowStutas.Text = "测量中断";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "左手到位，重新测量\r\n";
                                    player.SoundLocation = appPath + "\\Audio\\Chinese_Left_hand_in_place_please_measure_again.wav";
                                }
                                player.Play();
                                //重新启动测量计时
                                stateTimeStart = System.DateTime.Now.AddSeconds(1);
                                //Thread.Sleep(3000);
                            }
                            //记录当前红外状态
                            lastInfraredStatus[0] = 1;
                        }
                         if (calculatedMeasureDataS[i].Channel.ChannelID ==3 || calculatedMeasureDataS[i].Channel.ChannelID ==4)
                         {
                            if (lastInfraredStatus[1] == 0)//右手上次为红外不到位，本次为红外到位，说明需要重新进行语音提示
                            {
                                //系统状态显示区域显示测量中断
                                if (isEnglish)
                                {
                                    LblShowStutas.Text = "Interrupted";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "Right hand in place,Please measure again!\r\n";
                                    player.SoundLocation = appPath + "\\Audio\\English_right_hand_in_place_please_measure_again.wav";
                                }
                                else
                                {
                                    LblShowStutas.Text = "测量中断";
                                    //测量结果显示区域显示右手到位重新测量
                                    TxtShowResult.Text += "右手手到位，重新测量\r\n";
                                    //PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                                    LblRight.Text = "右手到位，重新测量";
                                    player.SoundLocation = appPath + "\\Audio\\Chinese_right_hand_in_place_please_measure_again.wav";
                                }
                                player.Play();
                                //重新启动测量计时
                                stateTimeStart = System.DateTime.Now.AddSeconds(1);
                                //Thread.Sleep(3000);
                            }
                            //记录当前红外状态
                            lastInfraredStatus[1] = 1;
                        }
                        if (calculatedMeasureDataS[i].Channel.ChannelID ==7)
                         {                            
                            if (lastInfraredStatus[2] == 0)//衣物上次为红外不到位，本次为红外到位，说明需要重新进行本地测量，进行语音提示
                            {
                                //系统状态显示区域显示测量中断
                                if (isEnglish)
                                {
                                    LblShowStutas.Text = "Interrupted";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "Frisker in place,Please measure again!\r\n";
                                    player.SoundLocation = appPath + "\\Audio\\English_frisker_In_place.wav";
                                }
                                else
                                {
                                    LblShowStutas.Text = "测量中断";
                                    //测量结果显示区域显示衣物探头到位重新测量
                                    TxtShowResult.Text += "衣物探头到位，重新测量\r\n";
                                    player.SoundLocation = appPath + "\\Audio\\Chinese_frisker_In_place_measure_again.wav";
                                }
                                player.Play();
                                //重新启动测量计时
                                stateTimeStart = System.DateTime.Now.AddSeconds(1);
                                //Thread.Sleep(3000);
                            }
                            //记录当前红外状态
                            lastInfraredStatus[2] = 1;
                        }                        
                        //重新启动本底测量（本底测量时间重新开始计时）
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for (int j = 0; j < channelS.Count; j++)
                        {
                            calculatedMeasureDataS[j].Alpha = 0;
                            calculatedMeasureDataS[j].Beta = 0;
                        }
                        return;
                    }
                    else//当前通道红外不到位
                    {
                        //左右手和衣物,只要有一个之前出现过红外到位，而现在不到位，说明测量被中断过，就需要重新显示“本底测量”提示信息，重新开始计时
                        if (lastInfraredStatus[0] == 1|| lastInfraredStatus[1] == 1|| lastInfraredStatus[2] == 1)//上次为红外到位，说明刚从到位变为不到位，需提示开始本底测量。
                        {
                            isReDisplay = true;                            
                        }                              
                        //记录本次红外状态
                        switch (calculatedMeasureDataS[i].Channel.ChannelID)
                        {
                            case 1:
                            case 2:
                                lastInfraredStatus[0] = 0;
                                break;
                            case 3:
                            case 4:
                                lastInfraredStatus[1] = 0;
                                break;
                            case 7:
                                lastInfraredStatus[2] = 0;
                                break;
                        }
                    }                    
                }
                //之前测量被中断过，需要重新显示提示信息
                if (isReDisplay== true)
                {
                    if (isEnglish)
                    {
                        LblShowStutas.Font = new Font("宋体", 28, FontStyle.Bold);
                        LblShowStutas.Text = "Updating Background";
                    }
                    else
                    {
                        LblShowStutas.Font = new Font("宋体", 48, FontStyle.Bold);
                        LblShowStutas.Text = "本底测量";
                    }
                    //重新启动本底测量（本底测量时间重新开始计时）
                    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    isReDisplay =false;
                    return;
                }
                //本底测量时间到
                if (stateTimeRemain < 0)
                {
                    //isPlayed = false;
                    //各个手部和脚部通道显示当前测量本底值（cps）
                    string[] errRecordS = BaseCheck();
                    //本底测量判断
                    if (errRecordS == null)//本底测量通过
                    {
                        //系统状态显示区域显示等待测量
                        if (isEnglish)
                        {
                            LblShowStutas.Text = "Ready";
                            //测试结果区域显示仪器正常等待测量
                            TxtShowResult.Text += "Ready\r\n";
                            //系统语音提示仪器正常等待测量
                            player.SoundLocation = appPath + "\\Audio\\English_Ready.wav";
                        }
                        else
                        {
                            LblShowStutas.Text = "等待测量";
                            //测试结果区域显示仪器正常等待测量
                            TxtShowResult.Text += "仪器正常 等待测量\r\n";
                            //系统语音提示仪器正常等待测量
                            player.SoundLocation = appPath + "\\Audio\\Chinese_Ready.wav";
                        }
                        player.Play();
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //系统参数中，将上次本底测量后已测量人数清零                        
                        systemParameter.ClearMeasuredCount();                       
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                            baseData.Add(calculatedMeasureDataS[i]);
                            //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为测量时计算做准备
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        // 运行状态标志设置为“等待测量”
                        platformState = PlatformState.ReadyToMeasure;
                        //启动本底测量计时(因为在等待测量过程中也要进行本底测量和计算) 
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    }
                    else//本底测量未通过
                    {
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库
                        AddErrorData(errRecordS);                        
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        platformState = PlatformState.ReadyToRun;                        
                    }
                    return;
                }
                return;
            }
            //运行状态为等待测量
            if (platformState == PlatformState.ReadyToMeasure)
            {                                
                //所有手部红外到位标志，默认全部到位
                bool isHandInfraredStatus = true;                
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if (list[0].Channel.ChannelID>=1&& list[0].Channel.ChannelID<=4 && list[0].InfraredStatus == 0)//当前通道为手部脚步通道且红外不到位
                    {                        
                            //第一次判断红外状态
                            if (isFirstBackGround == true)
                            {
                                //重新启动本底计时
                                stateTimeStart = System.DateTime.Now.AddSeconds(1);
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
                //所有通道手部红外状态全部到位
                if (isHandInfraredStatus == true)
                {
                    //系统语音提示仪器正常开始测量
                    if(isEnglish)
                    {
                        //系统状态显示区域显示开始测量
                        LblShowStutas.Text = "Start counting";
                        //测试结果区域显示开始测量
                        TxtShowResult.Text += "Start counting\r\n";
                        player.SoundLocation = appPath + "\\Audio\\English_Start_counting.wav";
                    }
                    else
                    {
                        //系统状态显示区域显示开始测量
                        LblShowStutas.Text = "开始测量";
                        //测试结果区域显示开始测量
                        TxtShowResult.Text += "开始测量\r\n";
                        player.SoundLocation = appPath + "\\Audio\\Chinese_Start_counting.wav";
                    }                    
                    player.Play();
                    //左右手状态区域显示正常
                    if (isEnglish)
                    {
                        LblLeft.Text = "Left hand in place";
                        LblRight.Text = "Left hand in place";
                    }
                    else
                    {
                        LblLeft.Text = "左手到位";
                        LblRight.Text = "右手到位";
                    }
                    PicLeftBackground.BackColor = PlatForm.ColorStatus.COLOR_STATUS;
                    PicRightBackground.BackColor = PlatForm.ColorStatus.COLOR_STATUS;                   
                    // Thread.Sleep(2000);                   
                    //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为测量时计算做准备
                    for (int i = 0; i < channelS.Count; i++)
                    {
                        calculatedMeasureDataS[i].Alpha = 0;
                        calculatedMeasureDataS[i].Beta = 0;
                    }
                    //将运行状态修改为“开始测量”
                    platformState = PlatformState.Measuring;
                    //重新启动计时，为开始测量及时准备
                    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    return;
                }
                //本底测量时间到，进行本底判断
                if (stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds <= 0)
                {
                    //下次如果还进行本底计算，则需重新计时，所以置标志为True
                    isFirstBackGround = true;                   
                    string[] errRecordS = BaseCheck();
                    //本底测量判断
                    if (errRecordS == null)//本底检测通过
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //下次如果还进行本底计算，则需重新计时，所以置标志为True
                        isFirstBackGround = true;                       
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
                        AddErrorData(errRecordS);
                        //界面显示“本底测量出现故障”同时进行语音提示                        
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                    }
                    //重新启动本底测量计时
                    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                }
                return;
            }
            //运行状态为开始测量
            if (platformState == PlatformState.Measuring)
            {                
                //textBox1.Text += platformState.ToString();               
                MeasureData conversionData = new MeasureData();
                IList<MeasureData> conversionDataS=new List<MeasureData>();
                //获得当前系统参数设置中的的测量时间并赋值给stateTimeSet
                stateTimeSet = systemParameter.MeasuringTime;                    
                //在系统界面中显示正在测量倒计时时间（s）:系统设置测量时间-已经用时
                stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                LblTimeRemain.Text = stateTimeRemain < 0 ? "0" : stateTimeRemain.ToString();                
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据经过平滑运算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if (list[0].Channel.ChannelID>=1 && list[0].Channel.ChannelID<=4 && list[0].InfraredStatus == 0)//手部红外状态未到位
                    {                          
                        //进行文字和语音提示，该通道channelS[i].ChannelName不到位
                        if(list[0].Channel.ChannelID==1|| list[0].Channel.ChannelID==2)//左手
                        {
                            if(isEnglish)
                            {
                                TxtShowResult.Text += "Left hand moving, please measure again";
                                //系统语音提示左手移动重新测量
                                player.SoundLocation = appPath + "\\Audio\\English_Left_hand_moved_please_measure_again.wav";
                            }
                            else
                            {
                                TxtShowResult.Text += "左手移动,重新测量";
                                //系统语音提示左手移动重新测量
                                player.SoundLocation = appPath + "\\Audio\\Chinese_Left_hand_moved_please_measure_again.wav";
                            }                            
                            player.Play();
                            //Thread.Sleep(2000);
                        }
                        if (list[0].Channel.ChannelID == 3 || list[0].Channel.ChannelID ==4)
                        {
                            if (isEnglish)
                            {
                                TxtShowResult.Text += "Right hand moving, please measure again";
                                //系统语音提示左手移动重新测量
                                player.SoundLocation = appPath + "\\Audio\\English_right_hand_moved_please_measure_again.wav";                                
                            }
                            else
                            {                                
                                TxtShowResult.Text += "右手移动,重新测量";
                                //系统语音提示右手移动重新测量
                                player.SoundLocation = appPath + "\\Audio\\Chinese_right_hand_moved_please_measure_again.wav";
                            }
                            player.Play();                            
                            if(isEnglish)
                            {
                                //切换到等待测量阶段，进行必要的显示
                                LblShowStutas.Text = "Ready";
                            }
                            else
                            {
                                //切换到等待测量阶段，进行必要的显示
                                LblShowStutas.Text = "等待测量";
                            }
                            //Thread.Sleep(2000);
                        }                        
                        //设定当前运行状态为“等待测量”
                        platformState = PlatformState.ReadyToMeasure;
                        //重新启动测量计时
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        return;                        
                    }
                    //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)进行累加：                               
                    calculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    calculatedMeasureDataS[i].Beta += list[0].Beta;
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;                    
                }
                //进行语音提示
                player.SoundLocation = appPath + "\\Audio\\dida1.wav";
                player.Play();
                if (stateTimeRemain == 1)
                {
                    player.SoundLocation = appPath + "\\Audio\\dida2.wav";
                    player.Play();
                }
                //测量时间到
                if (stateTimeRemain < 0)
                {
                    PictureBox pictureBox;
                    Panel panel;
                    Label label;                    
                    //计算每个通道的计数平均值,然后减去本底值
                    for (int i = 0; i < calculatedMeasureDataS.Count; i++)
                    {
                        if(calculatedMeasureDataS[i].Channel.ChannelID==7)//如果是衣物探头，则跳过继续
                        {
                            continue;
                        }
                        //找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                        pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                        //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                        panel = (Panel)(pictureBox.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名
                        label = (Label)(panel.Controls[string.Format("Lbl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                        //获得当前系统参数设置中的测量单位                        
                        //systemParameter.GetParameter();
                        //将当前通道的最终测量数据转换为系统参数设置的目标测量单位
                        //Nuclide nuclide = new Nuclide();
                        //string nuclideUser=nuclide.GetAlphaNuclideUser();
                        //EfficiencyParameter efficiencyParameter = new EfficiencyParameter();
                        //efficiencyParameter.GetParameter("α", nuclideUser,calculatedMeasureDataS[i].Channel.ChannelID);
                        //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                        IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == alphaNuclideUsed).ToList();
                        conversionData.Alpha = UnitConver(calculatedMeasureDataS[i].Alpha, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency,calculatedMeasureDataS[i].Channel.ProbeArea);
                        //nuclideUser = nuclide.GetBetaNuclideUser();
                       // efficiencyParameter.GetParameter("β", nuclideUser, calculatedMeasureDataS[i].Channel.ChannelID);
                        efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == betaNuclideUsed).ToList();
                        conversionData.Beta = UnitConver(calculatedMeasureDataS[i].Beta, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency,calculatedMeasureDataS[i].Channel.ProbeArea);
                        conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        //将单位转换后的测量数据添加进IList列表
                        conversionDataS.Add(conversionData);
                        calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha / stateTimeSet - baseData[i].Alpha;
                        if (calculatedMeasureDataS[i].Alpha < 0)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                        }
                        //ProbeParameter probeParameter = new ProbeParameter();
                        //获得当前通道的一级和二级报警阈值 
                        //probeParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID, "α");
                        //获得当前检测通道的Alpha探测参数
                        IList<ProbeParameter> probeParmeterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && probeParmeter.NuclideType== "α").ToList();                       
                        //判断当前通道Alpha测量值是否超过报警阈值
                        if (calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_1 || calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_2)
                        {
                            //将当前通道Alpha测量污染信息添加进pollutionRecord字符串
                            pollutionRecord += string.Format("{0}:α值{1}cps;", calculatedMeasureDataS[i].Channel.ChannelName,calculatedMeasureDataS[i].Alpha);
                            pollutionRecord_E += string.Format("{0}:Alpha Value{1}cps;",calculatedMeasureDataS[i].Channel.ChannelName_English,calculatedMeasureDataS[i].Alpha);
                            if(calculatedMeasureDataS[i].Alpha> probeParmeterNow[0].Alarm_2)
                            {
                                //当前通道测量结果显示文本框背景设置为Alarm2
                                label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                            }
                            else
                            {
                                label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                            }
                        }                        
                        calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta / stateTimeSet - baseData[i].Beta;
                        if (calculatedMeasureDataS[i].Beta < 0)
                        {
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        //probeParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID, "β");
                        //获得当前检测通道的Beta探测参数
                        probeParmeterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && probeParmeter.NuclideType == "β").ToList();
                        if (calculatedMeasureDataS[i].Beta > probeParmeterNow[0].Alarm_1 || calculatedMeasureDataS[i].Beta > probeParmeterNow[0].Alarm_2)
                        {
                            //将当前通道Beta测量污染信息添加进pollutionRecord字符串
                            pollutionRecord += string.Format("{0}:β值{1}cps;",calculatedMeasureDataS[i].Channel.ChannelName,calculatedMeasureDataS[i].Beta);
                            pollutionRecord_E += string.Format("{0}:Beta Value}{1}cps;",calculatedMeasureDataS[i].Channel.ChannelName_English,calculatedMeasureDataS[i].Beta);
                            if (calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_2)
                            {
                                //当前通道测量结果显示文本框背景设置为Alarm2
                                label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                            }
                            else
                            {
                                label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                            }
                        }                        
                    }
                    //按照系统参数单位要求显示最终测量结果,级显示单位转换后的conversionDataS列表值
                    DisplayMeasureData(conversionDataS, systemParameter.MeasurementUnit);
                    if (pollutionRecord == null)//说明本次测量无污染
                    {
                        //语音和文字提示“没有污染”
                        if (isEnglish)
                        {
                            player.SoundLocation = appPath + "\\Audio\\English_NoContamination_please_frisker.wav";
                        }
                        else
                        {
                            player.SoundLocation = appPath + "\\Audio\\Chinese_NoContamination_please_frisker.wav";
                        }
                        player.Play();
                        //设备状态区域显示无污染
                        if (isEnglish)
                        {
                            LblShowStutas.Text = "No Contamination";                            
                            //测量结果显示区域提示没有污染，请进行衣物测量
                            TxtShowResult.Text += "No Contamination,Please measure the clothing!\r\n";
                        }
                        else
                        {
                            LblShowStutas.Text = "无污染";                            
                            //测量结果显示区域提示没有污染，请进行衣物测量
                            TxtShowResult.Text += "没有污染，请进行衣物测量\r\n";
                        }
                        PicShowStatus.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);                        
                        //Thread.Sleep(3000);
                    }
                    else
                    {
                        // 设备状态区域显示人员污染
                        if (isEnglish)
                        {
                            LblShowStutas.Text = "Contaminated";
                            //测量结果显示区域提示被测人员污染，请去污
                            TxtShowResult.Text += "Decontaminate, please!\r\n";
                            //语音提示被测人员污染
                            player.SoundLocation = appPath + "\\Audio\\English_Decontaminate_please.wav";
                        }
                        else
                        {
                            LblShowStutas.Text = "人员污染";
                            //测量结果显示区域提示被测人员污染，请去污
                            TxtShowResult.Text += "被测人员污染，请去污！\r\n";
                            //语音提示被测人员污染
                            player.SoundLocation = appPath + "\\Audio\\Chinese_Decontaminate_please.wav";
                        }
                        //设置状态显示区域背景色
                        PicShowStatus.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;                                               
                        //将设备监测状态设置为“污染”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated);                       
                        player.Play();
                        //Thread.Sleep(3000);
                        //将本次测量数据和污染描述字符串pollutionRecord保存到数据库
                        MeasureData measureData = new MeasureData();
                        measureData.MeasureDate = DateTime.Now;
                        measureData.MeasureStatus = "污染";
                        measureData.DetailedInfo = pollutionRecord;
                        measureData.IsEnglish = false;
                        measureData.AddData(measureData);
                        measureData.MeasureStatus ="Contaminated";
                        measureData.DetailedInfo =pollutionRecord_E;
                        measureData.IsEnglish = true;
                        measureData.AddData(measureData);
                        //启动报警计时
                        alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                    }                    
                    //更新本底测量次数（+1）                    
                    systemParameter.UpdateMeasuredCount();
                    //运行状态设置为“测量结束”
                    platformState = PlatformState.Result;
                    return;
                }
                return;
            }
            //运行状态为“测量结束”
            if (platformState == PlatformState.Result)
            {
                //扔掉5次预读取的数据
                if(throwDataCount<5)
                {
                    for(int i=0;i<measureDataS.Count;i++)
                    {
                        measureDataS[i].Alpha = 0;
                        measureDataS[i].Beta = 0;
                        measureDataS[i].InfraredStatus = 0;
                    }
                    throwDataCount++;
                    commPort.ClearPortData();
                    return;
                }
                throwDataCount = 0;                
                //本次测量无污染
                if (pollutionRecord == null)
                {
                    //获得系统参数设置中的强制本地次数                    
                    //systemParameter.GetParameter();
                    //获得测量总人数（从上次本底测量开始）
                    //如果测量人数大于系统设置的强制本底次数则，转到“本底测量”状态
                    if (systemParameter.MeasuredCount >= systemParameter.BkgUpdate) 
                    {
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        // 运行状态标志设置为“本底测量”
                        platformState = PlatformState.BackGrouneMeasure;                        
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        //系统状态显示区域显示本底测量
                        if (isEnglish)
                        {
                            LblShowStutas.Font = new Font("宋体", 28, FontStyle.Bold);
                            LblShowStutas.Text = "Updating Background";
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "Updating Background\r\n";
                            //系统提示本底测量
                            player.SoundLocation = appPath + "\\Audio\\English_Updating_background.wav";
                        }
                        else
                        {
                            LblShowStutas.Font = new Font("宋体", 48, FontStyle.Bold);
                            LblShowStutas.Text = "本底测量";
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "本底测量\r\n";
                            //系统提示本底测量
                            player.SoundLocation = appPath + "\\Audio\\Chinese_Background_measure.wav";
                        }
                        player.Play();
                        //启动本底测量计时 
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        //Thread.Sleep(1000);
                        return;
                    }
                    else
                    {
                        //系统状态显示区域显示等待测量
                        if (isEnglish)
                        {
                            LblShowStutas.Text = "Ready";
                            //测试结果区域显示等待测量
                            TxtShowResult.Text += "Ready\r\n";
                            //系统语音提示仪器正常等待测量
                            player.SoundLocation = appPath + "\\Audio\\English_Ready.wav";
                        }
                        else
                        {
                            LblShowStutas.Text = "等待测量";
                            //测试结果区域显示等待测量
                            TxtShowResult.Text += "等待测量\r\n";
                            //系统语音提示仪器正常等待测量
                            player.SoundLocation = appPath + "\\Audio\\Chinese_Ready.wav";
                        }
                        player.Play();
                        //Thread.Sleep(3000);
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);                        
                        //commPort.
                        //设置运行状态为等待测量
                        platformState = PlatformState.ReadyToMeasure;                                                
                        return;
                    }
                }
                else//有污染,转到本底测量状态
                {
                    //设备监测状态为正常
                    deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                    // 运行状态标志设置为“本底测量”
                    platformState = PlatformState.BackGrouneMeasure;                    
                    //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                    for (int i = 0; i < channelS.Count; i++)
                    {
                        calculatedMeasureDataS[i].Alpha = 0;
                        calculatedMeasureDataS[i].Beta = 0;
                    }
                    //系统状态显示区域显示本底测量
                    if (isEnglish)
                    {
                        LblShowStutas.Font = new Font("宋体", 28, FontStyle.Bold);
                        LblShowStutas.Text = "Updating Background";
                        //测试结果区域显示本底测量
                        TxtShowResult.Text += "Updating Background\r\n";
                        //系统提示本底测量
                        player.SoundLocation = appPath + "\\Audio\\English_Updating_background.wav";
                    }
                    else
                    {
                        LblShowStutas.Font = new Font("宋体", 48, FontStyle.Bold);
                        LblShowStutas.Text = "本底测量";
                        //测试结果区域显示本底测量
                        TxtShowResult.Text += "本底测量\r\n";
                        //系统提示本底测量
                        player.SoundLocation = appPath + "\\Audio\\Chinese_Background_measure.wav";
                    }
                    player.Play();
                    //启动本底测量计时 
                    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    //Thread.Sleep(1000);
                    return;
                }                
            }
        }
        private void BaseSmooth()
        {
            smoothingData.baseData_Full = 1;
            smoothingData.baseData_i = 0;
            smoothingData.baseSum = 2000;
            smoothingData.baseTime = new UInt32[10];
            for(int i=0;i<10;i++)
            {
                smoothingData.baseTime[i] = 200;
            }
        }
        /// <summary>
        /// 对衣物探头监测数据做平滑处理
        /// </summary>
        private float SmoothData(UInt32 data)
        {
            float smoothedData = 0;
            float region_L = 0;
            float region_H = 0;
            int status = 0;
            int mutation_i = 0;//突变
            if (smoothingData.average <= 1 && data < 4)//平均值小于1，下次计数小于4
            {
                if (smoothingData.team_Full == 1)
                {
                    if (smoothingData.team_i == TEAM_LENGTH)
                    {
                        smoothingData.team_i = 0;
                    }
                    smoothingData.sum = smoothingData.sum + data - smoothingData.team[smoothingData.team_i];
                    smoothingData.team[smoothingData.team_i] = data;
                    smoothedData= (float)smoothingData.sum / TEAM_LENGTH;
                    smoothingData.average = (UInt32)smoothedData;
                    smoothingData.team_i++;
                }
                else
                {
                    smoothingData.team[smoothingData.team_i] = data;
                    smoothingData.sum += data;
                    smoothingData.team_i++;
                    smoothedData= (float)smoothingData.sum / (smoothingData.team_i + 1);
                    smoothingData.average = (UInt32)smoothedData;
                    if (smoothingData.team_i == TEAM_LENGTH)
                    {
                        smoothingData.team_Full = 1;
                    }
                }
                //10s泊松分布判断
                smoothingData.s10_Sum += data;
                smoothingData.s10_Sum_i++;
                if (smoothingData.s10_Sum_i == 10)
                {
                    //10s不满足泊松分布
                    if (smoothingData.s10_Sum < poissonTable[smoothingData.s10_Sum_Last * 2] || smoothingData.s10_Sum > poissonTable[smoothingData.s10_Sum_Last * 2 + 1])
                    {
                        smoothingData.team_Full = 0;
                        smoothingData.s10_Sum_Last = smoothingData.s10_Sum;
                        //将最后10组数放入新数组队列前面，算出平均值
                        smoothingData.sum = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            mutation_i = (int)(smoothingData.team_i + TEAM_LENGTH - i) % TEAM_LENGTH;
                            smoothingData.team[9 - i] = smoothingData.team[mutation_i];
                            smoothingData.sum = smoothingData.sum + smoothingData.team[9 - i];
                        }
                        smoothedData=(float)smoothingData.sum / 10;
                        smoothingData.average = (UInt32)smoothedData;
                        smoothingData.team_i = 10;
                    }
                    smoothingData.s10_Sum_i = 0;
                    smoothingData.s10_Sum = 0;
                }
            }
            //泊松表比较
            else if (smoothingData.average < 25)
            {
                smoothingData.s10_Sum_i = 0;
                smoothingData.s10_Sum = 0;
                switch (smoothingData.count_to)
                {
                    case 0:
                        if (data > poissonTable[smoothingData.average * 2 + 1])
                        {
                            smoothingData.count_to = 0x10;
                        }
                        if (data < poissonTable[smoothingData.average * 2])
                        {
                            smoothingData.count_to = 0x20;
                        }
                        break;
                    case 0x10:
                        if (data > poissonTable[smoothingData.average * 2 + 1])
                        {
                            status = 1;
                        }
                        smoothingData.count_to = 0;
                        break;
                    case 0x20:
                        if (data < poissonTable[smoothingData.average * 2])
                        {
                            status = 1;
                        }
                        smoothingData.count_to = 0;
                        break;
                    default:
                        smoothingData.count_to = 0;
                        break;
                }
                if (status == 1)
                {
                    smoothingData.team[0] = data;
                    smoothingData.average = data;
                    smoothingData.sum = data;
                    smoothingData.team_i = 0;
                    smoothingData.team_Full = 0;
                    smoothedData= (float)data;
                    status = 0;
                }
                else
                {
                    if (smoothingData.team_Full == 1)
                    {
                        if (smoothingData.team_i == TEAM_LENGTH)
                        {
                            smoothingData.team_i = 0;
                        }
                        smoothingData.sum = smoothingData.sum + data - smoothingData.team[smoothingData.team_i];
                        smoothingData.team[smoothingData.team_i] = data;
                        smoothedData= (float)smoothingData.sum / TEAM_LENGTH;
                        smoothingData.average = (UInt32)smoothedData;
                        smoothingData.team_i++;
                    }
                    else
                    {
                        smoothingData.team[smoothingData.team_i] = data;
                        smoothingData.sum += data;
                        smoothingData.team_i++;
                        smoothedData= (float)smoothingData.sum / (smoothingData.team_i + 1);
                        smoothingData.average = (UInt32)smoothedData;
                        if (smoothingData.team_i == TEAM_LENGTH)
                        {
                            smoothingData.team_Full = 1;
                        }
                    }
                }
            }
            //泊松公式比较
            else
            {
                smoothingData.s10_Sum_i = 0;
                region_L = smoothingData.average - (float)0.5 + (float)POISSONUA_2;
                region_L = (float)Math.Sqrt(region_L);
                region_L = (float)POISSONUA - region_L * 2;
                region_L = (region_L / 2) * (region_L / 2);
                region_H = smoothingData.average + (float)0.5 + (float)POISSONUA_2;
                region_H = (float)Math.Sqrt(region_H);
                region_H = (float)POISSONUA + region_H * 2;
                region_H = (region_H / 2) * (region_H / 2);
                switch (smoothingData.count_to)
                {
                    case 0:
                        if(data>region_H)
                        {
                            smoothingData.count_to = 0x10;
                        }
                        if(data<region_L)
                        {
                            smoothingData.count_to = 0x20;
                        }
                        break;
                    case 0x10:
                        if(data>region_H)
                        {
                            status = 1;
                        }
                        smoothingData.count_to = 0;
                        break;
                    case 0x20:
                        if(data<region_L)
                        {
                            status = 1;
                        }
                        smoothingData.count_to = 0;
                        break;
                    default:
                        smoothingData.count_to = 0;
                        break;
                }
                if(status==0)
                {
                    if(smoothingData.team_Full==1)
                    {
                        if(smoothingData.team_i==TEAM_LENGTH)
                        {
                            smoothingData.team_i = 0;
                        }
                        smoothingData.sum = smoothingData.sum + data - smoothingData.team[smoothingData.team_i];
                        smoothingData.team[smoothingData.team_i] = data;
                        smoothedData = (float)smoothingData.sum / TEAM_LENGTH;
                        smoothingData.average = (UInt32)smoothedData;
                        smoothingData.team_i++;
                    }
                    else
                    {
                        smoothingData.team[smoothingData.team_i] = data;
                        smoothingData.sum += data;
                        smoothingData.team_i++;
                        smoothedData = (float)smoothingData.sum / (smoothingData.team_i + 1);
                        smoothingData.average =(UInt32) smoothedData;
                        if(smoothingData.team_i==TEAM_LENGTH)
                        {
                            smoothingData.team_Full = 1;
                        }
                    }
                }
                else
                {
                    smoothingData.team[0] = data;
                    smoothingData.average = data;
                    smoothingData.sum = data;
                    smoothedData = (float)data;
                    smoothingData.team_i = 0;
                    smoothingData.team_Full = 0;
                }
            }
            if(data==0)
            {
                smoothingData.lingCycle++;
            }
            else
            {
                smoothingData.lingCycle = 0;
            }
            if(smoothingData.lingCycle>LONGTIME)
            {
                smoothingData.lingCycle = 0;
                smoothedData = 0;
                smoothingData.baseData_i = 0;
                smoothingData.baseData_Full = 0;
                smoothedData =(float)3.3e-4;
            }
            return smoothedData;
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
            for (int i = 0; i < channelS.Count; i++)//遍历全部启用通道
            {                
                if(calculatedMeasureDataS[i].Channel.ChannelID==7)//对衣物探头不做判断
                {
                    continue;
                }
                //获得当前通道的道盒参数,从全部道盒参数列表channelParameterS中找到当前通道的道盒参数
                //ChannelParameter channelParameter = new ChannelParameter();
                //channelParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID);
                IList<ChannelParameter>channelParameterNow= channelParameterS.Where(channelParameter=>channelParameter.Channel.ChannelID== calculatedMeasureDataS[i].Channel.ChannelID).ToList();
                //判断当前高压值：当前高压值若为系统设定值的0.8~1.2倍之内，则该通道高压正常，否则高压故障,将故障信息添加到errRecord字符串，isCheck = false;
                if (calculatedMeasureDataS[i].HV< channelParameterNow[0].PresetHV * (1 - PlatForm.ErrorRange.HV_ERROR) || calculatedMeasureDataS[i].HV > channelParameterNow[0].PresetHV * (1 + PlatForm.ErrorRange.HV_ERROR))
                {
                    //高压故障,将故障信息添加到errRecord字符串
                    errRecord += string.Format("高压故障,设置值:{0}V,实测值:{1}V;", channelParameterNow[0].PresetHV.ToString(), calculatedMeasureDataS[i].HV.ToString());
                    errRecord_E += string.Format("HV Fault,Preset:{0}V,Actual:{1}V;", channelParameterNow[0].PresetHV.ToString(), calculatedMeasureDataS[i].HV.ToString());
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
                        if(calculatedMeasureDataS[i].Alpha< channelParameterNow[0].AlphaThreshold* (1-PlatForm.ErrorRange.BASE_ERROR)||calculatedMeasureDataS[i].Alpha> channelParameterNow[0].AlphaThreshold* (1+PlatForm.ErrorRange.BASE_ERROR))
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
                        if (calculatedMeasureDataS[i].Beta < channelParameterNow[0].BetaThreshold * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataS[i].Beta > channelParameterNow[0].BetaThreshold * (1 + PlatForm.ErrorRange.BASE_ERROR))
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
                   // ProbeParameter probeParameter = new ProbeParameter();
                    if (factoryParameter.MeasureType != "β")
                    {
                        //查询当前通道的α本底上限、本底下限（从探测参数列表中找到当前通道的"α"探测参数）                       
                        IList<ProbeParameter> probeParameterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channelS[i].ChannelID && probeParmeter.NuclideType== "α").ToList();
                        //probeParameter.GetParameter(channelS[i].ChannelID, "α");
                        //进行α本底测量判断
                        if (calculatedMeasureDataS[i].Alpha < probeParameterNow[0].LBackground) //超过当前通道的本底下限
                        {
                            //该通道channelS[i].ChannelName的本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("α本底下限值:{0}cps,当前本底值:{1}cps;", probeParameterNow[0].LBackground.ToString(),calculatedMeasureDataS[i].Alpha.ToString());
                            errRecord_E += string.Format("αLow Background Threshold{0}cps,Actual Background:{1}cps;", probeParameterNow[0].LBackground.ToString(), calculatedMeasureDataS[i].Alpha.ToString());
                            isCheck = false;
                        }
                        if (calculatedMeasureDataS[i].Alpha >= probeParameterNow[0].HBackground)//超过当前通道的本底上限
                        {
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("α本底上限值:{0}cps,当前本底值:{1}cps;", probeParameterNow[0].HBackground.ToString(), calculatedMeasureDataS[i].Alpha.ToString());
                            errRecord_E += string.Format("αHigh Background Threshold{0}cps,Actual Background:{1}cps;", probeParameterNow[0].HBackground.ToString(), calculatedMeasureDataS[i].Alpha.ToString());
                            isCheck = false;
                        }
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        //查询当前通道的β本底上限、本底下限
                        //probeParameter.GetParameter(channelS[i].ChannelID, "β");
                        //查询当前通道的β本底上限、本底下限（从探测参数列表中找到当前通道的"β"探测参数）                       
                        IList<ProbeParameter> probeParameterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channelS[i].ChannelID && probeParmeter.NuclideType == "α").ToList();
                        if (calculatedMeasureDataS[i].Beta < probeParameterNow[0].LBackground)//超过当前通道的本底下限
                        {
                            //该通道channelS[i].ChannelName本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("β本底下限值:{0}cps,当前本底值:{1}cps;", probeParameterNow[0].LBackground.ToString(), calculatedMeasureDataS[i].Beta.ToString());
                            errRecord_E += string.Format("βLow Background Threshold{0}cps,Actual Background:{1}cps;", probeParameterNow[0].LBackground.ToString(), calculatedMeasureDataS[i].Beta.ToString());
                            isCheck = false;
                        }
                        if (calculatedMeasureDataS[i].Beta >= probeParameterNow[0].HBackground)//超过当前通道的本底上限
                        {
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("β本底上限值:{0}cps,当前本底值:{1}cps;", probeParameterNow[0].HBackground.ToString(), calculatedMeasureDataS[i].Beta.ToString());
                            errRecord_E += string.Format("βHigh Background Threshold{0}cps,Actual Background:{1}cps;", probeParameterNow[0].HBackground.ToString(), calculatedMeasureDataS[i].Beta.ToString());
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
                        if (isEnglish)
                        {
                            LblShowStutas.Text = "Fault";
                        }
                        else
                        {
                            LblShowStutas.Text = "仪器故障";
                        }
                    }
                    TxtShowResult.Text += calculatedMeasureDataS[i].Channel.ChannelName + errRecord + "\r\n";
                    //对应通道名字文本框背景色显示为ERROR
                    if (calculatedMeasureDataS[i].Channel.ChannelID != 7)//衣物探头除外
                    {
                        ((TextBox)(this.Controls[string.Format("Txt{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                    }
                }
                else
                {
                    //设备状态区域背景色显示为NORMAL
                    PicShowStatus.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                    if (platformState == PlatformState.SelfTest)
                    {
                        //设备状态区域文字提示“仪器正常”
                        if (isEnglish)
                        {
                            LblShowStutas.Text = "Ready";
                        }
                        else
                        {
                            LblShowStutas.Text = "仪器正常";
                        }
                    }
                    //对应通道名字文本框背景色显示为NORMAL
                    if (calculatedMeasureDataS[i].Channel.ChannelID != 7)//衣物探头除外
                    {
                        ((TextBox)(this.Controls[string.Format("Txt{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                    }
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
        /// <param name="efficiency">探测效率</param>
        /// <param name="proberArea">探测面积</param>
        /// <returns>目标单位值</returns>
        private float UnitConver(float data, string unit,float efficiency,float proberArea)
        {
            float convertedData=0;
            //将data（单位为cps）换算为目标单位unit后返回
            switch (unit)
            {
                case "cps":
                    convertedData = data;
                    break;
                case "cpm": //最终测量计数平均值(cpm) = 60 * 计算平均值(cps)
                    convertedData = 60 * data;
                    break;
                case "Bq"://最终测量计数平均值(Bq) = 200 * 计算平均值(cps) /探测效率
                    convertedData = 200 * data /efficiency;
                    break;
                case "Bq/cm2"://最终测量计数平均值(Bq/cm2) = 200 * 计算平均值(cps) /探测效率/该通道测量面积
                    convertedData = 200 * data / efficiency / proberArea;
                    break;
                case "KBq/cm2"://KBq/cm2:最终测量计数平均值(KBq/cm2) = 200 * 计算平均值(cps) /探测效率/ 该通道测量面积/1000
                    convertedData = 200 * data / efficiency / proberArea / 1000;
                    break;
                case "dpm"://dpm:最终测量计数平均值(dpm) = 12000 * 计算平均值(cps)/探测效率
                    convertedData = 12000 * data / efficiency;
                    break;
                case "nCi"://nCi : 最终测量计数平均值(nCi) = 200 * 计算平均值(cps)/探测效率*0.027
                    convertedData =Convert.ToSingle(200 * data / efficiency * 0.027);
                    break;
            }                                                                       
            return convertedData;
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
        private byte[] ReadDataFromSerialPortReport(BackgroundWorker worker, DoWorkEventArgs e)
        {
            //巡检管理机下发的报文
            while (true)
            {
                //请求进程中断读取数据
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }
                //读取串口回传数据并赋值给receiveBuffMessage
                byte[] receiveBuffMessage = new byte[20];
                receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                //延时
                Thread.Sleep(1300);                
                //触发向主线程返回下位机上传数据事件
                worker.ReportProgress(1, receiveBuffMessage);
            }
        }

        private void bkWorkerReportStatus_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string appPath = Application.StartupPath;
            int messageBufferLength =8; //最短报文长度            
            byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
            int[] message = new int[7]; //解析后报文结构数据存储List对象                                                            
            if (e.UserState is byte[])
            {
                receiveBufferMessage = (byte[])e.UserState;
            }
            //接收报文数据为空
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 2)
                {
                    //界面提示“通讯错误”
                    if (isEnglish)
                    {
                        TxtShowResult.Text = "Communication Error！";
                    }
                    else
                    {
                        TxtShowResult.Text = "通讯错误！";
                    }
                }
                else
                {
                    errNumber++;
                }
                return;
            }
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            message = Components.Message.ExplainMessage(receiveBufferMessage);
            if (message == null)//解析失败
            {
                return;
            }
            //解析成功
            if(message.Count()>=7)//长度大于1，为时间同步命令
            {
                //将当前系统时间同步为管理机下发的时间
                SYSTEMTIME timeForSyn = new SYSTEMTIME();
                timeForSyn.Year =(short) message[0];
                timeForSyn.Month = (short)message[1];
                timeForSyn.Day = (short)message[2];
                timeForSyn.Hour = (short)(message[3] - 8) <= 0 ? (short)(message[3] - 8 + 24) : (short)(message[3] - 8);
                timeForSyn.Minute = (short)message[4];
                timeForSyn.Second = (short)message[5];
                timeForSyn.MiliSecond = (short)message[6];
                try
                {
                    SetSystemTime(ref timeForSyn);                                  
                    //向管理机恢复时间同步报文
                    byte[] timeSynMessage = new byte[8];
                    timeSynMessage[0] = 0x01;
                    timeSynMessage[1] = 0x10;
                    timeSynMessage[2] = 0x11;
                    timeSynMessage[3] = 0x00;
                    timeSynMessage[4] = 0x00;
                    timeSynMessage[5] = 0x04;
                    //生成CRC校验码
                    byte[] crc16 = new byte[2];
                    crc16 = Tools.CRC16(timeSynMessage, timeSynMessage.Length - 2);
                    timeSynMessage[6] = crc16[0];
                    timeSynMessage[7] = crc16[1];
                    Components.Message.SendMessage(timeSynMessage, commPort_Supervisory);                    
                    if (isEnglish)
                    {
                        TxtShowResult.Text += "Time synchronization completed!\r\n";
                    }
                    else
                    {
                        TxtShowResult.Text += "时间同步完成\r\n";
                    }                   
                }
                catch
                {
                    if (isEnglish)
                    {
                        TxtShowResult.Text += "Time synchronization failed!\r\n";
                    }
                    else
                    {
                        TxtShowResult.Text += "时间同步失败\r\n";
                    }
                }
            }
            else//是上报测试状态命令
            {
                //下发地址和当前设备地址不一致则返回
                if (factoryParameter.DeviceAddress != message[0].ToString())
                {
                    return;
                }
                else //下发地址和当前设备地址一致，则上传当前测试状态
                {
                    byte[] deviceStatusMessage;
                    //从数据库中查询最近一次记录的测量数据
                    MeasureData measureData = new MeasureData();
                    measureData.GetData();
                    //从数据库中查询最近一次记录的故障数据
                    ErrorData errorData = new ErrorData();
                    errorData.GetData();
                    //监测数据和故障数据都已经上报，说明最近仪器正常，上报时间为当前时间
                    //监测数据上报，故障数据未上报，说明最近仪器故障，上报完成后更新故障数据状态为已上报
                    //监测数据未上报，故障数据上报，说明最近状态为污染，上报完成后更新监测数据状态为已上报
                    //监测数据和故障数据都未上报，则将最近的状态进行上报，更新两条记录为已上报
                    if (measureData.MeasureDate > errorData.ErrTime)//最近一次记录为MeasureData，说明是状态为污染（因为只有污染状态才会记录，正常不记录）
                    {
                        //生成上报管理机的监测仪测试状态报文                    
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), measureData.MeasureDate, 0x04);//仪器状态为污染
                    }
                    else
                    {
                        //生成上报管理机的监测仪测试状态报文                    
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), DateTime.Now, deviceStatus);
                    }
                    //向管理机上报仪器检测状态
                    if(Components.Message.SendMessage(deviceStatusMessage, commPort_Supervisory))
                    {

                    }                    
                    else
                    {

                    }
                }
            }
        }        
        private void TmrDispTime_Tick(object sender, EventArgs e)
        {
            //更新当前显示时间
            LblTime.Text = DateTime.Now.ToLongTimeString();
            //根据当前红外状态控制左右手及衣物红外状态显示
            foreach (MeasureData usedChannelData in calculatedMeasureDataS)
            {
                switch (usedChannelData.Channel.ChannelID)
                {
                    case 1:
                    case 2:
                        if (usedChannelData.InfraredStatus == 1)//红外到位
                        {
                            if(isEnglish)
                            {
                                LblLeft.Text = "LH in place";
                            }
                            else
                            {
                                LblLeft.Text = "左手到位";
                            }                            
                            PicLeftBackground.BackColor = PlatForm.ColorStatus.COLOR_STATUS;
                        }
                        else
                        {
                            if (isEnglish)
                            {
                                LblLeft.Text = "LH not in place";
                            }
                            else
                            {
                                LblLeft.Text = "左手不到位";
                            }                            
                            PicLeftBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        }
                        break;
                    case 3:
                    case 4:
                        if (usedChannelData.InfraredStatus == 1)//红外到位
                        {
                            if (isEnglish)
                            {
                                LblRight.Text = "RH in place";
                            }
                            else
                            {
                                LblRight.Text = "右手到位";
                            }
                            PicRightBackground.BackColor = PlatForm.ColorStatus.COLOR_STATUS;
                        }
                        else
                        {
                            if (isEnglish)
                            {
                                LblRight.Text = "RH not in place";
                            }
                            else
                            {
                                LblRight.Text = "右手不到位";
                            }
                            PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        }
                        break;
                    case 7:
                        if (usedChannelData.InfraredStatus == 1)//红外到位
                        {
                            PicFrisker.BackColor = PlatForm.ColorStatus.COLOR_STATUS;
                        }
                        else
                        {
                            PicFrisker.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                        }
                        break;
                }
            }
            //更新剩余时间：系统自检设置时间-已经用时
            //stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
            ////更新当前系统运行状态剩余时间
            //LblTimeRemain.Text = stateTimeRemain.ToString();
        }

        private void BtnChinese_Click(object sender, EventArgs e)
        {
            try
            {
                isEnglish = false;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                this.BtnEnglish.Enabled = true;
                this.BtnChinese.Enabled = false;
                Tools.ApplyLanguageResource(this);
            }
            catch
            {
                MessageBox.Show("切换失败，请稍后再试");
                return;
            }
        }

        private void BtnEnglish_Click(object sender, EventArgs e)
        {
            try
            {
                isEnglish = true;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                this.BtnChinese.Enabled = true;
                this.BtnEnglish.Enabled = false;
                Tools.ApplyLanguageResource(this);
            }
            catch
            {
                MessageBox.Show("Switch Fault，please try later");
                return;
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        private static extern bool SetSystemTime(ref SYSTEMTIME t);

        private void BtnOption_Click(object sender, EventArgs e)
        {

        }
    }
}

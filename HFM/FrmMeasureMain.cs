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
using HFM.Properties;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace HFM
{
    public partial class FrmMeasureMain : Form
    {        
        ObserverDictionary<string, int> testDeviceStatus = new ObserverDictionary<string, int>();        
        bool isDeviceStatusUpdated = true;//设备状态是否更新标志，设备状态如果更新则在状态上报时应该查询数据库
        Panel[] PnlStatus;//各通道背景Panel
        Label[] LblTitle;//各通道顶部标题Label
        Label[] LblValue;//各通道测量值显示Label
        PictureBox[] PicStatus;//各通道状态图片标识
        Label[] LblStatus;//各通道状态图片下面状态文字提示Label      
        byte infraredStatusOfMessageLast = 0x3b;//上次报文信息中红外状态，默认手、脚、衣物都不到位。格式：5-7通道红外状态-1-4通道红外状态。占低6位
        byte infraredStatusOfMessageNow = 0x3b;//本次报文信息中红外状态，默认手、脚、衣物都不到位。格式：5-7通道红外状态-1-4通道红外状态。占低6位
        //const int BASE_DATA = 1000;//标准本底值
        const int FONT_SIZE_E = 12;//检测状态显示区域英文字体大小
        const int FONT_SIZE = 14;//检测状态显示区域中午字体大小                                
        string appPath = null;//应用系统安装路径
        int messageBufferLength = 0;//串口接收数据缓冲区大小
        byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
        System.Timers.Timer TmrDispTime = null;//显示系统时间、控制控件状态颜色异步Timer对象
        IList<MeasureData> measureDataS = null;//检测数据接收列表缓冲区
        bool isEnglish = false;//当前语言，默认中文                      
        DateTime alarmTimeStart = DateTime.Now;//系统报警时间计时        
        int stateTimeRemain = 0;//系统当前运行状态剩余时间        
        int errNumber = 0; //报文接收出现错误计数器           
        int[] lastInfraredStatus = new int[3];//记录上一个数据包红外状态，分别为“左手、右手、衣物”，本底测量中，如果本次红外到位而上次不到位则进行语音播报，如果上次红外到位本次也红外到位，则不需要重复播报提示        
        int backgroundCount = 0;//本底测量计数，记录本底测量阶段回传数据报文个数以便于求平均值
        bool isSelfCheckSended = false;//自检指令是否已经下发标志，因为在一个自检周期内，自检指令只需下发一次
        bool isBetaCommandToSend = false;//Beta自检指令是否应该下发，在α/β自检时，先下发α自检指令，自检时间到一半时再下发β自检指令        
        bool[] isLoadProgressPic =new bool[6] { false,false,false,false,false,false};//窗口顶部本底测量检测进度状态图片是否已经被加载                        
        bool isCommError = false;//监测端口通信错误标志
        bool isCommReportError = false;//上报端口通信错误标志     
        bool isFrmDisplayed = false;
        bool isFrmFirstShow = true;
        FrmClothes frmClothes = null;//衣物探测界面
        //运行状态枚举类型
        enum PlatformState
        {
            ReadyToRun = 0,
            SelfCheck = 1,
            BackGrouneMeasure = 2,
            ReadyToMeasure = 3,
            Measuring = 4,
            Result = 5
        }       
        /// <summary>
        /// 设备状态：1个字节，0001 0000监测正常，0010 0000监测失败，0100 0000监测污染
        /// </summary>        
        byte deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);//设备当前状态        
        DateTime stateTimeStart;//系统当前运行状态的开始计时变量                
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();//创建音频播放对象
        CommPort commPort = new CommPort();//监测端口
        CommPort commPort_Supervisory = new CommPort();//和管理机通信端口        
        System.Timers.Timer TmrUIControl = new System.Timers.Timer();//UI界面刷新定时器，每200毫秒根据当前状态刷新UI界面       
        UIEventArgs uIEventArgs = new UIEventArgs();//UI界面刷新事件参数传递类        
        int playControl = 0;//控制语音播放变量                 
        FactoryParameter factoryParameter = new FactoryParameter();//工厂参数        
        Components.SystemParameter systemParameter = new Components.SystemParameter();//系统参数
        Channel[] channelsAll=new Channel[7];//全部通道
        IList<Channel> channelS = new List<Channel>();//全部通道信息； ---//当前可使用的检测通道,即全部启用的监测通道
        IList<Channel> usedChannelS = new List<Channel>(); //当前可使用的检测通道,即全部启用的监测通道       
        IList<MeasureData> baseData = new List<MeasureData>(); //存储本底计算结果，用来对测量数据进行校正
        IList<MeasureData> calculatedMeasureDataS = new List<MeasureData>();//存储各个通道最终计算检测值的列表
        IList<EfficiencyParameter> efficiencyParameterS = new List<EfficiencyParameter>();//存储探测效率参数列表
        IList<ProbeParameter> probeParameterS = new List<ProbeParameter>();//存储探测参数的列表  
        IList<ChannelParameter> channelParameterS = new List<ChannelParameter>();//存储道盒参数列表    
        MeasureData conversionData = new MeasureData();                
        IList<MeasureData> conversionDataS = new List<MeasureData>();
        //-------------创建全部状态---------------
        StateReadyToRun stateReadyToRun= new StateReadyToRun() { Name="ReadyToRun"};
        StateSelfCheck stateSelfCheck;
        StateBackgroundMeasure stateBackgroundMeasure;
        StateReadyToMeasure stateReadyToMeasure;// 
        StateHandFootMeasuring stateHandFootMeasuring;        
        StateFriskerMeasuringIndependent stateFriskerMeasuringIndependent;//衣物独立探头
        StateFriskerMeasuringShared stateFriskerMeasuringShared;//衣物和手脚共享探头
        StateResult stateResult;
        IState stateCurrent;
        IStateFrisker stateFrisker;
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
                                            17,34,18,36,19,37,19,38,20,39 };//泊松表1        
        int[] cycleLength = new int[8] { 16, 25, 36, 49, 64, 81, 100, 121 };
        //int handTestCount=0;//手部检测计数器。由于单探测器时，手心手背必须分两次进行检测，用来手心、手背检测计数。 
        [DllImport("winmm.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int waveOutSetVolume(int uDeviceID, int dwVolume); //Call waveOutSetVolume( 0, 100 );
        public FrmMeasureMain()
        {
            InitializeComponent();
            PnlStatus = new Panel[7];
            LblTitle = new Label[7];
            LblValue = new Label[13];
            PicStatus = new PictureBox[7];
            LblStatus = new Label[7];
            PnlStatus[0] = PnlLHP;
            PnlStatus[1] = PnlLHB;
            PnlStatus[2] = PnlRHP;
            PnlStatus[3] = PnlRHB;
            PnlStatus[4] = PnlLF;
            PnlStatus[5] = PnlRF;
            PnlStatus[6] = PnlFrisker;
            LblTitle[0] = LblLHP;
            LblTitle[1] = LblLHB;
            LblTitle[2] = LblRHP;
            LblTitle[3] = LblRHB;
            LblTitle[4] = LblLF;
            LblTitle[5] = LblRF;
            LblTitle[6] = LblFrisker;
            PicStatus[0] = PicLHP;
            PicStatus[1] = PicLHB;
            PicStatus[2] = PicRHP;
            PicStatus[3] = PicRHB;
            PicStatus[4] = PicLF;
            PicStatus[5] = PicRF;
            PicStatus[6] = PicFrisker;
            LblValue[0] = LblLHPA;
            LblValue[1] = LblLHPB;
            LblValue[2] = LblLHBA;
            LblValue[3] = LblLHBB;
            LblValue[4] = LblRHPA;
            LblValue[5] = LblRHPB;
            LblValue[6] = LblRHBA;
            LblValue[7] = LblRHBB;
            LblValue[8] = LblLFA;
            LblValue[9] = LblLFB;
            LblValue[10] = LblRFA;
            LblValue[11] = LblRFB;
            LblValue[12] = LblFriskerB;
            LblStatus[0] = LblLHPStatus;
            LblStatus[1] = LblLHBStatus;
            LblStatus[2] = LblRHPStatus;
            LblStatus[3] = LblRHBStatus;
            LblStatus[4] = LblLFStatus;
            LblStatus[5] = LblRFStatus;
            LblStatus[6] = LblFriskerStatus;
            //UIEventArgs uIEventArgs = new UIEventArgs();      
            //为UI控制定时器绑定事件响应方法            
            //this.TmrUIControl.Elapsed += (sender, e) => TmrUIControl_Tick(this, uIEventArgs);
        }
        /// <summary>
        /// 初始化显示界面
        /// </summary>
        private void DisplayInit()
        {            
            //在界面中显示当前系统时间
            LblTime.Text = DateTime.Now.ToLongTimeString();
            //中英文状态显示
            if (systemParameter.IsEnglish == true)
            {
                try
                {
                    isEnglish = true;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    Tools.ApplyLanguageResource(this);
                    Tools.controls.Clear();
                    LblName.Text = Tools.EnSoftName(factoryParameter.SoftName);
                }
                catch
                {
                    MessageBox.Show("Switch Fault，please try later");
                }
            }
            else
            {
                isEnglish = false;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                Tools.ApplyLanguageResource(this);
                Tools.controls.Clear();
                LblName.Text = factoryParameter.SoftName;
            }
            //在界面中显示“仪器名称”、“仪器编号”、“IP地址及端口”等信息                                    
            LblIP.Text = factoryParameter.IpAddress + " " + factoryParameter.PortNumber;
            LblSN.Text = factoryParameter.InstrumentNum;                       
            PicLogo.Image = Resources.logo;
            //界面中所有通道测量值显示恢复初始状态            
            for (int i = 0; i < 13; i++)
            {
                LblValue[i].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                LblValue[i].Text = "0.00cps";
            }
        }

        private void StateMachineInit()
        {
            //初始化状态机
            stateSelfCheck = new StateSelfCheck() { Name = "SelfCheck" };
            stateBackgroundMeasure = new StateBackgroundMeasure() { Name = "BackGroundMeasure" };
            stateReadyToMeasure = new StateReadyToMeasure() { Name = "ReadyToMeasure" };
            stateHandFootMeasuring = new StateHandFootMeasuring() { Name = "HandFootMeasuring" };            
            stateResult = new StateResult() { Name = "Result" };
            //---------------组合各个状态间关系-------------
            //准备运行的下一个状态集为：仪器自检
            stateReadyToMeasure.Nexts = new List<IState> { stateSelfCheck };
            //仪器自检状态的下一个状态集为：本底测量、仪器自检（自检故障时重新开始自检）
            stateSelfCheck.Nexts = new List<IState> { stateBackgroundMeasure, stateSelfCheck };
            //本底测量状态的下一个状态集为：等待测量、仪器自检（本底异常时）、本底测量（本底测量过程中断时）
            stateBackgroundMeasure.Nexts = new List<IState> { stateReadyToMeasure, stateSelfCheck, stateBackgroundMeasure };
            //等待测量状态的下一个状态集为：手脚测量、衣物测量、本底测量（本底异常时）
            stateReadyToMeasure.Nexts = new List<IState> { stateHandFootMeasuring, stateFrisker, stateBackgroundMeasure };
            //手脚测量状态的下一个状态集为：测量完成、等待测量（测量中断或本次测量完成时）
            stateHandFootMeasuring.Nexts = new List<IState> { stateResult, stateReadyToMeasure };
            //手脚测量状态的下一个状态集为：测量结果、等待测量（测量中断或本次测量完成时）
            stateFrisker.Nexts = new List<IState> { stateResult, stateReadyToMeasure };
            //测量结果状态的下一个状态集为：等待测量、本底测量
            stateResult.Nexts = new List<IState> { stateBackgroundMeasure, stateReadyToMeasure };
            //设定每个状态的保持时间长度
            stateSelfCheck.SetTimes = systemParameter.SelfCheckTime;//仪器自检时间长度 
            stateBackgroundMeasure.SetTimes = systemParameter.SmoothingTime;//本底测量时间长度
            stateReadyToMeasure.SetTimes = systemParameter.SmoothingTime;//等待测量状态的平滑时间长度
            stateHandFootMeasuring.SetTimes = systemParameter.MeasuringTime;//手脚测量时间长度      
                                                                            //初始化每个状态保持时间（剩余时间）
            stateSelfCheck.HoldTimes = systemParameter.SelfCheckTime;
            stateBackgroundMeasure.HoldTimes = systemParameter.SmoothingTime;
            stateReadyToMeasure.HoldTimes = systemParameter.SmoothingTime;
            stateHandFootMeasuring.HoldTimes = systemParameter.MeasuringTime;
            //设定报警时间长度
            State.AlarmTimes = systemParameter.AlarmTime;
            //stateBackgroundMeasure.AlarmTimes = systemParameter.AlarmTime;
            //stateReadyToMeasure.AlarmTimes = systemParameter.AlarmTime;
            //stateHandFootMeasuring.AlarmTimes = systemParameter.AlarmTime;
            //stateFrisker.AlarmTimes = systemParameter.AlarmTime;
            //stateResult.AlarmTimes = systemParameter.AlarmTime;
            //设定用户探测核素选择
            Nuclide nuclide = new Nuclide();
            stateHandFootMeasuring.AlphaNuclideUsed = nuclide.GetAlphaNuclideUser();//获得用户Alpha探测核素选择
            stateHandFootMeasuring.BetaNuclideUsed = nuclide.GetBetaNuclideUser();//获得用户Beta探测核素选择            
        }

        /// <summary>
        /// 根据当前通道状态设置界面中各个通道显示效果
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="status">状态：0未启用；1启用红外未到位；2启用红外到位</param>   
        /// <param name="ctrlModel">控制方式：0初始化；1：保持</param>用于控制测量值显示标签背景色，是保持原来颜色还是重新初始化成初始状态颜色
        private void ChannelDisplayControl(Channel channel,int status,int ctrlModel)
        {
            switch (status)
            {
                case 0://通道未启用                    
                    //通道背景图片
                    if (channel.ChannelID >= 1 && channel.ChannelID <= 4)//手部
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Hand_NotInPlace;// Image.FromFile(appPath + "\\Images\\Hand_NotInPlace.png");                       
                    }
                    if(channel.ChannelID==5 || channel.ChannelID==6)//脚部
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Foot_NotInPlace;// Image.FromFile(appPath + "\\Images\\Foot_NotInPlace.png");
                    }
                    if(channel.ChannelID==7)//衣物
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.FriskerBK_NotInPlace;// Image.FromFile(appPath + "\\Images\\FriskerBK_NotInPlace.png");                        
                    }                    
                    //通道标题标签
                    LblTitle[channel.ChannelID - 1].Enabled = false;
                    LblTitle[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                    //通道状态图片
                    PicStatus[channel.ChannelID - 1].BackgroundImage =Image.FromFile(appPath + string.Format("\\Images\\{0}_Disabled.png", channel.ChannelName_English));
                    //通道测量值标签
                    if (channel.ChannelID == 7)
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                    }
                    else
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                        LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = false;
                    }
                    //通道状态标签
                    LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                    LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                    LblStatus[channel.ChannelID - 1].Text = "未启用";
                    break;
                case 1://启用红外未到位
                    //通道背景图片
                    if (channel.ChannelID >= 1 && channel.ChannelID <= 4)//手部
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Hand_InPlace;// Image.FromFile(appPath + "\\Images\\Hand_InPlace.png");                        
                    }
                    if (channel.ChannelID == 5 || channel.ChannelID == 6)//脚部
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Foot_InPlace;// Image.FromFile(appPath + "\\Images\\Foot_InPlace.png");
                    }
                    if (channel.ChannelID == 7 && measureDataS.Count>0&& measureDataS[6].InfraredStatus ==0)//衣物
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.FriskerBK_InPlace;// Image.FromFile(appPath + "\\Images\\FriskerBK_InPlace.png");
                        ////通道状态图片
                        PicStatus[channel.ChannelID - 1].BackgroundImage = Resources.Frisker_NotInPlace;// Image.FromFile(appPath + "\\Images\\Frisker_NotInPlace.png");
                        //通道状态标签
                        LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                        LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
                        LblStatus[channel.ChannelID - 1].Text = "未到位";
                    }                    
                    //通道标题标签
                    LblTitle[channel.ChannelID - 1].Enabled = true;
                    LblTitle[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.COLOR_BKENABLED;
                    LblTitle[channel.ChannelID - 1].BackColor = Color.Transparent;                    
                    //通道测量值标签
                    if (channel.ChannelID == 7)
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                        LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                    }
                    else
                    {
                        switch(factoryParameter.MeasureType)
                        {
                            case "α":
                                if (ctrlModel == 0)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = false;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                }
                                break;
                            case "β":
                                if (ctrlModel == 0)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                }
                                break;
                            case "α/β":
                                if (ctrlModel == 0)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                }
                                break;

                        }                        
                    }
                    if (channel.ChannelID != 7)
                    {
                        //通道状态图片
                        PicStatus[channel.ChannelID - 1].BackgroundImage = Image.FromFile(appPath + string.Format("\\Images\\{0}_NotInPlace.png", channel.ChannelName_English));
                        //通道状态标签
                        LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                        LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
                        LblStatus[channel.ChannelID - 1].Text = "未到位";
                    }
                    break;
                case 2://启用红外到位
                    //通道背景图片
                    if (channel.ChannelID >= 1 && channel.ChannelID <= 4)//手部
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Hand_InPlace;// Image.FromFile(appPath + "\\Images\\Hand_InPlace.png");
                    }
                    if (channel.ChannelID == 5 || channel.ChannelID == 6)//脚部
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Foot_InPlace;// Image.FromFile(appPath + "\\Images\\Foot_InPlace.png");
                    }
                    if (channel.ChannelID == 7 && measureDataS.Count > 0 && measureDataS[6].InfraredStatus == 1)//衣物
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.FriskerBK_InPlace;// Image.FromFile(appPath + "\\Images\\FriskerBK_InPlace.png");
                        ////通道状态图片
                        PicStatus[channel.ChannelID - 1].BackgroundImage = Resources.Frisker_InPlace;// Image.FromFile(appPath + "\\Images\\Frisker_InPlace.png");
                        //通道状态标签
                        LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                        LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                        LblStatus[channel.ChannelID - 1].Text = "已到位";
                    }                    
                    //通道标题标签
                    LblTitle[channel.ChannelID - 1].Enabled = true;
                    LblTitle[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.COLOR_BKENABLED;                    
                    //通道测量值标签
                    if (channel.ChannelID == 7)
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                    }
                    else
                    {                        
                        switch (factoryParameter.MeasureType)
                        {
                            case "α":
                                if (ctrlModel == 0)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = false;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                }
                                break;
                            case "β":
                                if (ctrlModel == 0)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                                    //LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                }
                                break;
                            case "α/β":
                                if (ctrlModel == 0)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                }
                                break;

                        }
                    }
                    if (channel.ChannelID != 7)
                    {
                        //通道状态图片
                        PicStatus[channel.ChannelID - 1].BackgroundImage = Image.FromFile(appPath + string.Format("\\Images\\{0}_InPlace.png", channel.ChannelName_English));
                        //通道状态标签
                        LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                        LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                        LblStatus[channel.ChannelID - 1].Text = "已到位";
                    }
                    break;
            }
        }
        /// <summary>
        /// 显示测量数据
        /// </summary>
        private void DisplayMeasureData(IList<MeasureData> measureDataS,string unit)
        {
            //PictureBox pictureBox = null;
            Panel panel = null;
            Label label = null;           
            foreach (MeasureData measureData in measureDataS)
            {
                if(measureData.Channel.ChannelName==null||measureData.Channel.IsEnabled==false)
                {
                    continue;
                }
                //找到通道测量值显示区域对应的PictureBox，其名字为Pic+通道英文名
                //pictureBox = (PictureBox)(this.Controls[string.Format("Pic{0}", measureData.Channel.ChannelName_English)]);
                //找到通道测量值显示区域对应的Panel，其名字为：Pnl+通道英文名
                panel = (Panel)(this.Controls[string.Format("Pnl{0}", measureData.Channel.ChannelName_English)]);
                if (measureData.Channel.ChannelName_English == "Frisker")
                {
                    //显示衣物测量结果
                    LblFriskerB.Text= measureData.Beta.ToString("F2") +"cps";//衣物测量结果在Beta值中存储，衣物探头测量单位全部为cps
                    continue;
                }                
                switch (factoryParameter.MeasureType)
                {
                    case "α":
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English,"A")]);
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F2") + unit;
                        break;
                    case "β":
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English, "B")]);
                        //Label控件显示测量值
                        label.Text = "β" + measureData.Beta.ToString("F2") + unit;                        
                        break;
                    case "α/β":
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English, "A")]);
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F2") + unit;
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English, "B")]);
                        //Label控件显示测量值
                        label.Text = "β" + measureData.Beta.ToString("F2") + unit;
                        break;
                }
            }                           
        }
        /// <summary>
        /// 将中英文故障记录添加到数据库中
        /// </summary>
        /// <param name="errRecordS">中文和英文故障记录数据</param>
        private void AddErrorData(Dictionary<Language,string> errRecordS)
        {
            ErrorData errorData = new ErrorData
            {
                //中文描述
                Record = errRecordS[Language.Chinese],
                IsEnglish = false
            };
            errorData.ErrTime = DateTime.Now;
            errorData.AddData(errorData);
            //英文描述
            errorData.Record = errRecordS[Language.English];
            errorData.IsEnglish = true;
            errorData.AddData(errorData);           
        }                
        private void FrmMeasureMain_Load(object sender, EventArgs e)
        {                                           
            //设定初始状态为运行准备
            stateCurrent = stateReadyToRun;               
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            appPath = Application.StartupPath;
            messageBufferLength = 62; //最短报文长度                        
            measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象                                                                        
            //获得工厂参数设置信息           
            factoryParameter.GetParameter();
            //获得系统参数设置信息
            systemParameter.GetParameter();            
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
            //实例化衣物探测界面
            frmClothes = new FrmClothes(isEnglish);
            //初始化衣物探测状态
            Nuclide nuclide = new Nuclide();
            //factoryParameter.IsFriskerIndependent = false;
            //factoryParameter.IsFootInfrared = true;
            //根据衣物探头类型（独立/共享）初始化衣物探测状态
            if (factoryParameter.IsFriskerIndependent==true)//独立衣物探头
            {
                stateFriskerMeasuringIndependent = new StateFriskerMeasuringIndependent(frmClothes) { Name = "FriskerMeasuring" };
                stateFrisker = stateFriskerMeasuringIndependent;
                stateFriskerMeasuringIndependent.NuclideUsed = nuclide.GetClothesNuclideUser(); //获得用户衣物探测核素选择
                //绑定衣物测量数据显示事件响应(在衣物测量界面)
                stateFriskerMeasuringIndependent.DisplayDataEvent += frmClothes.DisplayData;
                //绑定衣物测量数据显示及衣物测量结果状态控制事件响应(在测量主界面)
                stateFriskerMeasuringIndependent.ShowMsgEvent += ShowMsgFriskerMeasuring;
            }
            else
            {
                stateFriskerMeasuringShared= new StateFriskerMeasuringShared(frmClothes) { Name = "FriskerMeasuring" };
                stateFrisker = stateFriskerMeasuringShared;
                //获得衣物Alpha和Beta核素选择
                stateFriskerMeasuringShared.AlphaNuclideUsed = nuclide.GetAlphaNuclideUser();
                stateFriskerMeasuringShared.BetaNuclideUsed = nuclide.GetBetaNuclideUser();
                //绑定衣物测量数据显示事件响应(在衣物测量界面)
                stateFriskerMeasuringShared.DisplayDataEvent+= frmClothes.DisplayData;
                //绑定衣物测量数据显示及衣物测量结果状态控制事件响应(在测量主界面)
                stateFriskerMeasuringShared.ShowMsgEvent += ShowMsgFriskerMeasuring;
            }
            //初始化显示界面
            DisplayInit();                      
            uIEventArgs.ChannelStatus.DoOnAdd = UIChannelStatus_Paint;
            uIEventArgs.ChannelStatus.DoOnSetKeyValue = UIChannelStatus_Paint;
            uIEventArgs.MeasureStatus.DoOnAdd = UIMeasureStatus_Paint;
            uIEventArgs.MeasureStatus.DoOnSetKeyValue = UIMeasureStatus_Paint;
            //初始化设备状态为正常
            uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
            //初始化UI界面各通道状态字典：未启用/红外未到位            
            foreach (Channel cn in channelS)
            {
                if (cn.IsEnabled == false || cn.ProbeArea == 0)//通道未启用或检测面积为0
                {
                    if (!uIEventArgs.ChannelStatus.ContainsKey(cn)) //不包含当前通道
                    {
                        uIEventArgs.ChannelStatus.Add(cn, ChannelStatus.Disabled);//会触发UIChannelStatus_Paint刷新界面                        
                    }
                    else
                    {
                        uIEventArgs.ChannelStatus[cn] = ChannelStatus.Disabled;//已包含当前通道，则设置为未启用；会触发UIChannelStatus_Paint刷新界面
                    }
                    //使用通道列表中如果包含该通道，则移除
                    if (usedChannelS.Contains(cn))
                    {
                        usedChannelS.Remove(cn);
                    }
                }
                else //通道启用且检测面积不为0
                {
                    if (!uIEventArgs.ChannelStatus.ContainsKey(cn))//不含吧当前通道
                    {
                        uIEventArgs.ChannelStatus.Add(cn, ChannelStatus.NotInPlace);//会触发UIChannelStatus_Paint刷新界面
                    }
                    else
                    {
                        uIEventArgs.ChannelStatus[cn] = ChannelStatus.NotInPlace;//已包含当前通道，则设置为未到位；会触发UIChannelStatus_Paint刷新界面
                    }
                    //使用通道列表中如果不包含该通道则添加
                    if (!usedChannelS.Contains(cn))
                    {
                        usedChannelS.Add(cn);
                    }
                }
                //初始化UI界面各通道设备状态字典：设备正常
                uIEventArgs.MeasureStatus.Add(cn, DeviceStatus.OperatingNormally);
            }                        
            LblTimeRemain.Text = string.Format("{0}s", systemParameter.SelfCheckTime.ToString());                                                                           
            //打开串口
            if (commPort.Opened == true)
            {
                commPort.Close();
            }
            //从配置文件获得当前监测串口配置
            commPort.GetCommPortSet("commportSet");
            //commPort.PortNum = 6;
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
                    TxtShowResult.Text = "Measuring Comm open fault\r\n";
                }
                else
                {
                    MessageBox.Show("监测端口打开错误！请检查通讯是否正常。");
                    TxtShowResult.Text = "监测串口打开失败\r\n";
                }
                //return;
            }
            //打开管理机通讯串口
            if (commPort_Supervisory.Opened == true)
            {
                commPort_Supervisory.Close();
            }
            //从配置文件获得当前管理机串口通信配置
            commPort_Supervisory.GetCommPortSet("commportSetOfReport");
            if (commPort_Supervisory.IsEnabled == true)
            {
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
                        TxtShowResult.Text = "Supervisor Comm open fault\r\n";
                    }
                    else
                    {
                        MessageBox.Show("管理机端口打开错误！请检查通讯是否正常。");
                        TxtShowResult.Text = "管理机串口打开失败\r\n";
                    }
                }
                //线程支持异步取消
                bkWorkerReportStatus.WorkerSupportsCancellation = true;
                //线程支持报告进度
                bkWorkerReportStatus.WorkerReportsProgress = true;
                //启动异步线程,响应DoWork事件
                bkWorkerReportStatus.RunWorkerAsync();
            }                        
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
            //线程支持报告进度
            bkWorkerReceiveData.WorkerReportsProgress = true;
            //启动异步线程,响应DoWork事件
            bkWorkerReceiveData.RunWorkerAsync();
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
                TmrDispTime = new System.Timers.Timer();
                TmrDispTime.Interval = 500;
                TmrDispTime.Elapsed += TmrDispTime_Tick;
                TmrDispTime.Enabled = true;
                //在异步线程上执行串口读操作ReadDataFromSerialPort方法
                BackgroundWorker bkWorker = sender as BackgroundWorker;
                e.Result = ReadDataFromSerialPort(bkWorker, e);
            }
            else
            {
                e.Cancel = true;
            }
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
                if (stateCurrent ==stateSelfCheck && isSelfCheckSended==false)
                {
                    byte[] messageDate = null;
                    switch (factoryParameter.MeasureType)
                    {                                                                          
                        case "α":
                            //生成Alpha自检指令报文，包含参数：自检时间/2-2 
                            messageDate = Components.Message.BuildMessage(0, stateSelfCheck.SetTimes / 2-2);
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
                            messageDate = Components.Message.BuildMessage(1, stateSelfCheck.SetTimes / 2 - 2);
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
                            if (isBetaCommandToSend == false)
                            {
                                //先下发Alpha自检指令
                                //生成Alpha自检指令报文，包含参数：自检时间/2-2 
                                messageDate = Components.Message.BuildMessage(0, stateSelfCheck.SetTimes / 2 - 2);
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
                            }
                            else
                            {
                                //后下发Beta自检指令
                                //生成Beta自检指令报文，包含参数：自检时间/2-2
                                messageDate = Components.Message.BuildMessage(1, stateSelfCheck.SetTimes / 2 - 2);
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
                    if (isBetaCommandToSend == false)//如果是Alpha/Beta自检，Beta自检指令需要下发时不需要重新设置开始时间
                    {
                        stateTimeStart = System.DateTime.Now;
                    }
                }
                //向下位机下发“C”指令码
                byte[] buffMessage = new byte[62];
                buffMessage[0] = Convert.ToByte('C');

                //将当前监测状态打包到报文最后一个字节                
                buffMessage[61] = (deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_1) ? Convert.ToByte(DeviceStatus.OperatingFaulted) : deviceStatus);
                if (HFM.Components.Message.SendMessage(buffMessage, commPort) == true)
                {
                    //延时
                    Thread.Sleep(100);
                    //读取串口回传数据并赋值给receiveBuffMessage
                    byte[] receiveBuffMessage = new byte[124];
                    try
                    {
                        receiveBuffMessage = Components.Message.ReceiveMessage(commPort);                        
                    }
                    catch
                    {
                        TxtShowResult.Text += "监测端口通信错误！\r\n";
                        isCommError = true;
                    }
                    //延时
                    if (stateCurrent == stateSelfCheck && isSelfCheckSended == false)
                    {
                        Thread.Sleep(50- errorNumber* delayTime);
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                    if (receiveBuffMessage!= null&& receiveBuffMessage.Length >0)
                    {
                        //触发向主线程返回下位机上传数据事件
                        worker.ReportProgress(1, receiveBuffMessage);
                        isCommError = false;
                        errorNumber = 0;
                    }
                    else
                    {
                        errorNumber++;
                        if(errorNumber>5)
                        {
                            isCommError = true;                            
                        }
                    }
                }
                else
                {
                    errNumber++;
                    if(errNumber == 5)
                    {
                        TxtShowResult.Text += "监测端口通信错误！\r\n";
                        isCommError = true;                        
                    }
                }
            }
        }

        private void bkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {                   
            if (e.UserState is byte[])
            {
                receiveBufferMessage = (byte[])e.UserState;
            }            
            //接收报文数据为空
            if (receiveBufferMessage==null||receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 3)
                {
                    //界面提示“通讯错误”
                    if (isEnglish)
                    {
                        TxtShowResult.Text = "Communication Error！";
                    }
                    else
                    {
                        TxtShowResult.Text = "通讯错误！\r\n";
                        isCommError = true;
                    }
                }
                else
                {
                    errNumber++;
                }
                return;
            }
            isCommError = false;           
            LblTimeRemain.Text = stateCurrent.HoldTimes < 0 ? string.Format("{0,3}s", "0") : string.Format("{0,3}s", stateCurrent.HoldTimes.ToString());
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            if (receiveBufferMessage[0] =='C' || receiveBufferMessage[0] == 'c') //判断报文头是C指令
            {
                measureDataS = Components.Message.ExplainMessage<MeasureData>(receiveBufferMessage);                
            } 
            //如果脚步和躯干红外独立则更新红外状态
            if(factoryParameter.IsFootInfrared==true)
            {
                measureDataS = MessageV2.UpdateInfraredStatus(measureDataS, receiveBufferMessage);                
                File.AppendAllText(appPath + "\\log\\msg.txt", "检测端口回传原始报文：" + BitConverter.ToString(receiveBufferMessage) + "\r\n");
                File.AppendAllText(appPath + "\\log\\msg.txt", "解析后脚步红外状态---左脚：" + measureDataS[4].InfraredStatus.ToString() + "\r\n");
                File.AppendAllText(appPath + "\\log\\msg.txt", "解析后脚步红外状态---右脚：" + measureDataS[5].InfraredStatus.ToString() + "\r\n");
            }
            if(measureDataS==null||measureDataS.Count<7)//解析失败
            {
                return;
            }              
            try
            {
                //报文解析无误,将当前报文红外状态清零
                infraredStatusOfMessageNow &= 0;
                if(factoryParameter.IsFootInfrared==true)//脚步独立红外
                {
                    //加载将当前报文各通道红外状态
                    infraredStatusOfMessageNow |= (byte)(receiveBufferMessage[61] & 0x37);//红外状态屏蔽无效位后赋值            
                }
                else
                {
                    //加载将当前报文1-4通道红外状态
                    infraredStatusOfMessageNow |= (byte)(receiveBufferMessage[61] & 7);//红外状态屏蔽高位后赋值            
                    //加载当前报文5-7通道红外状态
                    infraredStatusOfMessageNow |= (byte)((receiveBufferMessage[123] & 7) << 3);
                }
                
            }
            catch (Exception)
            {
                return;
            }            
            if ((infraredStatusOfMessageNow ^ infraredStatusOfMessageLast) != 0)//红外状态发生变化
            {
                //保存当前红外状态
                infraredStatusOfMessageLast = infraredStatusOfMessageNow;
                //重新刷新控制各个通道显示状态
                foreach (Channel channel in usedChannelS)//usedChannelS中存储当前全部启用且探测面积不为0的通道
                {
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channel.ChannelID).ToList();
                    if (list.Count > 0)
                    {
                        if(uIEventArgs.ChannelStatus.ContainsKey(channel))
                        {
                            uIEventArgs.ChannelStatus[channel] = list[0].InfraredStatus == 0 ? ChannelStatus.NotInPlace : ChannelStatus.InPlace;
                        }                        
                    }
                    if ((stateCurrent == stateReadyToMeasure || stateCurrent == stateHandFootMeasuring ) && factoryParameter.IsDoubleProbe == false)//当前状态为等待测量且为单探测器
                    {
                        switch (channel.ChannelID)
                        {
                            //左右手心
                            case 1:
                            case 3:
                                if (uIEventArgs.ChannelStatus.ContainsKey(channel))
                                {
                                    //进行手部第一次测试，按照红外是否到位正常进行状态显示
                                    if (stateCurrent.HandProbeCtrl == HandProbeCtrl.StepOneReady)
                                    {
                                        uIEventArgs.ChannelStatus[channel] = list[0].InfraredStatus == 0 ? ChannelStatus.NotInPlace : ChannelStatus.InPlace;
                                    }
                                    //进行手部第二次测试，屏蔽红外状态显示，显示不到位
                                    if (stateCurrent.HandProbeCtrl == HandProbeCtrl.StepTwoReady)
                                    {
                                        uIEventArgs.ChannelStatus[channel] = ChannelStatus.NotInPlace;
                                    }
                                }
                                break;
                            //左右手背
                            case 2:
                            case 4:
                                if (uIEventArgs.ChannelStatus.ContainsKey(channel))
                                {
                                    //进行手部第一次测试，屏蔽红外状态显示，显示不到位
                                    if (stateCurrent.HandProbeCtrl == HandProbeCtrl.StepOneReady)
                                    {
                                        uIEventArgs.ChannelStatus[channel] = ChannelStatus.NotInPlace;                                        
                                    }
                                    //进行手部第二次测试，按照红外是否到位正常进行状态显示
                                    if (stateCurrent.HandProbeCtrl == HandProbeCtrl.StepTwoReady)
                                    {
                                        uIEventArgs.ChannelStatus[channel] = list[0].InfraredStatus == 0 ? ChannelStatus.NotInPlace : ChannelStatus.InPlace;
                                    }
                                }
                                break;
                        }
                    }
                }                
            }                                            
            for (int i=0;i<channelS.Count();i++)
            {
                measureDataS[i].Channel = channelS[i];
            }
            //衣物探头被启用
            if (measureDataS[6].Channel.IsEnabled == true)
            {
                //启动衣物探测处理                                  
                int runStatus = stateFrisker.Run(stateCurrent, measureDataS, probeParameterS, efficiencyParameterS);
                switch (runStatus)
                {                    
                    case -1://未到数据处理周期1s或无需做任何处理
                        break;
                    case 0://衣物探头红外到位但衣物测量窗口还未打开，加载并打开衣物探测界面
                        if (frmClothes.IsDisposed)//如果窗体已经被释放，则重新创建
                        {
                            frmClothes = new FrmClothes(isEnglish);
                            //绑定衣物测量数据显示事件响应(在衣物测量界面)
                            if (stateFriskerMeasuringIndependent != null)
                            {
                                stateFriskerMeasuringIndependent.DisplayDataEvent += frmClothes.DisplayData;
                            }
                            if (stateFriskerMeasuringShared != null)
                            {
                                stateFriskerMeasuringShared.DisplayDataEvent += frmClothes.DisplayData;
                            }
                        }
                        //设置窗体进度条状态
                        frmClothes.loadingCircle.Active = true;                        
                        //显示衣物探头监测窗口                    
                        frmClothes.Show();
                        //File.AppendAllText(appPath + "\\log\\msg.txt", "衣物监测窗口打开，当前衣物红外状态：" + measureDataS[6].InfraredStatus.ToString());                        
                        break;
                    case 1://衣物探头红外到位但探头还未放下，测量返回有污染/无污染都不做任何处理
                        break;
                    case 2: //衣物探头红外不到位，探头刚被放下                        
                        //关闭衣物探头监测界面
                        if (stateFriskerMeasuringIndependent != null)
                        {
                            stateFriskerMeasuringIndependent.DisplayDataEvent -= frmClothes.DisplayData;
                        }
                        if (stateFriskerMeasuringShared != null)
                        {
                            stateFriskerMeasuringShared.DisplayDataEvent -= frmClothes.DisplayData;
                        }
                        frmClothes.Close();
                        //衣物检测完成标志
                        stateFrisker.IsTested = true;
                        //测量值、设置阈值赋值给显示参数类并显示结果
                        FriskerEventArgs friskerEventArgs = new FriskerEventArgs();
                        friskerEventArgs.MeasureValue["α"] = stateFrisker.SmoothedDataOfAlpha;
                        friskerEventArgs.MeasureValue["β"] = stateFrisker.SmoothedDataOfBeta;
                        friskerEventArgs.MeasureValue["C"] = stateFrisker.SmoothedData;                        
                        if(stateFrisker.DeviceStatus == DeviceStatus.OperatingContaminated_1)
                        {
                            friskerEventArgs.PreSet["α"] = probeParameterS[6].Alarm_1;
                            friskerEventArgs.PreSet["β"] = probeParameterS[6].Alarm_1;
                            friskerEventArgs.PreSet["C"] = probeParameterS[6].Alarm_1;
                        }
                        if (stateFrisker.DeviceStatus == DeviceStatus.OperatingContaminated_2)
                        {
                            friskerEventArgs.PreSet["α"] = probeParameterS[6].Alarm_2;
                            friskerEventArgs.PreSet["β"] = probeParameterS[6].Alarm_2;
                            friskerEventArgs.PreSet["C"] = probeParameterS[6].Alarm_2;
                        }
                        //在测量结果区域显示测量结果，对污染状态背景色进行设置
                        ShowMsgFriskerMeasuring(this, friskerEventArgs);
                        //如果衣物污染则将衣物污染数据保存到数据库
                        if (stateFrisker.DeviceStatus == DeviceStatus.OperatingContaminated_1 || stateFrisker.DeviceStatus == DeviceStatus.OperatingContaminated_2)
                        {                            
                            //将测量时间（当前时间）、状态（“污染”）、详细信息（“衣物探头”+测量值）写入数据库                                
                            MeasureData measureData = new MeasureData();
                            measureData.MeasureDate = DateTime.Now;
                            measureData.MeasureStatus = "污染";
                            if (factoryParameter.IsFriskerIndependent)
                            {
                                measureData.DetailedInfo[Language.Chinese] = string.Format("衣物探头{0}cps", stateFriskerMeasuringIndependent.SmoothedData.ToString("F2"));
                                measureData.DetailedInfo[Language.English] = string.Format("Frisker{0}cps", stateFriskerMeasuringIndependent.SmoothedData.ToString("F2"));
                            }
                            else
                            {
                                switch(factoryParameter.MeasureType)
                                {
                                    case "α":
                                        measureData.DetailedInfo[Language.Chinese] = string.Format("衣物探头α:{0}cps", stateFriskerMeasuringShared.SmoothedDataOfAlpha.ToString("F2"));
                                        measureData.DetailedInfo[Language.English] = string.Format("Frisker α:{0}cps", stateFriskerMeasuringShared.SmoothedDataOfAlpha.ToString("F2"));
                                        break;
                                    case "β":
                                        measureData.DetailedInfo[Language.Chinese] = string.Format("衣物探头β:{0}cps", stateFriskerMeasuringShared.SmoothedDataOfBeta.ToString("F2"));
                                        measureData.DetailedInfo[Language.English] = string.Format("Frisker β:{0}cps", stateFriskerMeasuringShared.SmoothedDataOfBeta.ToString("F2"));
                                        break;
                                    case "α/β":
                                        measureData.DetailedInfo[Language.Chinese] = string.Format("衣物探头α:{0}cps;β:{1}cps", stateFriskerMeasuringShared.SmoothedDataOfAlpha.ToString("F2"),stateFriskerMeasuringShared.SmoothedDataOfBeta.ToString("F2"));
                                        measureData.DetailedInfo[Language.English] = string.Format("Frisker α:{0}cps;β:{1}cps", stateFriskerMeasuringShared.SmoothedDataOfAlpha.ToString("F2"),stateFriskerMeasuringShared.SmoothedDataOfBeta.ToString("F2"));
                                        break;
                                }
                            }
                            measureData.IsEnglish = false;
                            measureData.AddData(measureData);//添加中文记录
                            stateFrisker.ErrRecord.Add(Language.Chinese, measureData.DetailedInfo[Language.Chinese]);
                            measureData.MeasureStatus = "Contaminated";                             
                            measureData.IsEnglish = true;
                            measureData.AddData(measureData);//添加英文记录
                            stateFrisker.ErrRecord.Add(Language.English, measureData.DetailedInfo[Language.English]);
                            stateResult.ErrRecord[Language.Chinese] += stateFrisker.ErrRecord[Language.Chinese];
                            stateResult.ErrRecord[Language.English] += stateFrisker.ErrRecord[Language.English];
                            //启动故障报警计时
                            State.AlarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        }                                                
                        //转StateResult状态
                        stateCurrent = stateResult;
                        return;
                }                                            
            }            
            //如果当前运行状态为“运行准备”
            if (stateCurrent==stateReadyToRun)
            {
                //初始化状态机
                StateMachineInit();
                //设定初始语音播报状态
                stateReadyToMeasure.SoundMessage = SoundMessage.Ready;
                //绑定事件                
                stateBackgroundMeasure.ShowMsgEvent += this.ShowMsgBackGround;                
                stateReadyToMeasure.ShowMsgEvent += this.ShowMsgReady;                
                stateHandFootMeasuring.ShowMsgEvent += ShowMsgHandFootMeasuring;                
                //设定当前状态的探测参数设置
                stateCurrent.FactoryParameter = factoryParameter;
                stateCurrent.SystemParameter = systemParameter;
                stateCurrent.DeviceStatus = DeviceStatus.OperatingNormally;
                uIEventArgs.CurrentState = stateSelfCheck;
                //根据有效通道对象初始化用来存储最终监测数据的列表
                foreach (Channel usedChannel in usedChannelS)
                {
                    //MeasureData measureData = new MeasureData();
                    //measureData.Channel = usedChannel;
                    stateSelfCheck.CalculatedMeasureDataS.Add(new MeasureData());
                    stateSelfCheck.CalculatedMeasureDataS.Last().Channel = usedChannel;
                    stateBackgroundMeasure.CalculatedMeasureDataS.Add(new MeasureData());
                    stateBackgroundMeasure.CalculatedMeasureDataS.Last().Channel = usedChannel;
                    stateReadyToMeasure.CalculatedMeasureDataS.Add(new MeasureData());
                    stateReadyToMeasure.CalculatedMeasureDataS.Last().Channel = usedChannel;
                    stateHandFootMeasuring.CalculatedMeasureDataS.Add(new MeasureData());
                    stateHandFootMeasuring.CalculatedMeasureDataS.Last().Channel = usedChannel;
                    stateHandFootMeasuring.CalculatedMeasureDataS.Last().InfraredStatus = 1;//手脚开始测量默认红外状态为全部到位
                    stateFrisker.CalculatedMeasureDataS.Add(new MeasureData());
                    stateFrisker.CalculatedMeasureDataS.Last().Channel = usedChannel;
                    //calculatedMeasureDataS.Add(measureData);//---------------------------可去掉？？
                }
                //下一个状态（仪器自检）的文字提示和语音播报                
                ShowMsgSelfCheck(this, uIEventArgs);
                //切换状态到仪器自检
                stateCurrent = stateSelfCheck;
                stateCurrent.LastTime = System.DateTime.Now;
                return;
            }
            if(stateCurrent==stateSelfCheck)
            {
                uIEventArgs.CurrentState = stateSelfCheck;
                //对采集的监测数据进行处理                
                int runStatus=stateCurrent.Run(usedChannelS,measureDataS, uIEventArgs);
                //1s时间未到，直接返回
                if (runStatus == -1)
                {
                    return;
                }
                //显示当前各通道测量数据
                DisplayMeasureData(measureDataS, "cps");
                //如果是α/β自检且自检时间是否到自检时间的一半
                if (factoryParameter.MeasureType == "α/β" && stateCurrent.HoldTimes == stateCurrent.SetTimes / 2 + 2)
                {
                    isSelfCheckSended = false;
                    isBetaCommandToSend = true;
                }
                //自检时间到
                if (stateCurrent.HoldTimes<0)
                {
                    //对启用的有效通道进行本底判断
                    //string[] errRecordS = new string[2];
                    stateCurrent.ErrRecord = stateCurrent.BaseCheck(usedChannelS, channelParameterS,uIEventArgs);
                    if (stateCurrent.ErrRecord == null)//自检通过切换到本底测量
                    {
                        //当前设备状态设置为正常
                        uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
                        stateCurrent.DeviceStatus = DeviceStatus.OperatingNormally;                        
                        //下一个状态（本底）的文字提示和语音播报
                        //ShowMsg(this,uIEventArgs);
                        ShowMsgBackGround(this, uIEventArgs);
                        //重新初始化界面测量值显示（归0）
                        DisplayMeasureData(stateBackgroundMeasure.CalculatedMeasureDataS, "cps");
                        //切换状态到本底测量
                        stateCurrent = stateBackgroundMeasure;
                        stateCurrent.IsShowMsg = false;//未进行有效信息显示和语音播报
                        stateCurrent.LastTime = System.DateTime.Now;
                    }
                    else
                    {
                        //当前设备状态设置为故障
                        uIEventArgs.DeviceStatus = DeviceStatus.OperatingFaulted;
                        stateCurrent.DeviceStatus = DeviceStatus.OperatingFaulted;                                              
                        //将故障信息errRecord写入数据库    
                        AddErrorData(stateCurrent.ErrRecord);
                        isDeviceStatusUpdated = true;
                        //进行自检故障文字和语音提示,自检状态进度图片显示仪器故障颜色
                        //ShowMsg(this, uIEventArgs);
                        ShowMsgSelfCheck(this, uIEventArgs);
                        //测量数据存储全部清零
                        foreach(MeasureData data in stateCurrent.CalculatedMeasureDataS)
                        {
                            data.Alpha = 0;
                            data.Beta = 0;
                        }                        
                        isSelfCheckSended = false;
                        isBetaCommandToSend = false;
                        //启动故障报警计时
                        State.AlarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        //重新启动仪器自检计时
                        stateCurrent.HoldTimes = stateCurrent.SetTimes;
                    }
                    return;
                }                
            }
            if(stateCurrent==stateBackgroundMeasure)
            {
                uIEventArgs.CurrentState = stateBackgroundMeasure;
                backgroundCount++;//本底测量值计数+1
                //对采集的监测数据进行处理
                int runStatus=stateCurrent.Run(usedChannelS,measureDataS, uIEventArgs);
                //1s时间未到或红外异常直接返回
                if (runStatus==-1)
                {
                    return; 
                }                                
                if(runStatus==1)//本次测量数据处理正常（本次1s的数据）
                {
                    stateCurrent.IsShowMsg = false;
                    //本底测量时间到
                    if (stateCurrent.HoldTimes < 0)
                    {
                        stateCurrent.ErrRecord = stateCurrent.BaseCheck(usedChannelS, probeParameterS, uIEventArgs);
                        //显示本底测量最终结果
                        DisplayMeasureData(stateCurrent.CalculatedMeasureDataS, "cps");
                        if (stateCurrent.ErrRecord==null)//本底测量通过切换到等待测量
                        {
                            //当前设备状态设置为正常
                            uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
                            stateCurrent.DeviceStatus = DeviceStatus.OperatingNormally;                            
                            //系统参数中，将上次本底测量后已测量人数清零                        
                            systemParameter.ClearMeasuredCount();
                            systemParameter.MeasuredCount = 0;
                            for (int i = 0; i < usedChannelS.Count; i++)
                            {
                                //uIEventArgs.MeasureStatus[usedChannelS[i]] = DeviceStatus.OperatingNormally;
                                //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                                MeasureData baseDataTemp = new MeasureData();
                                Tools.Clone(stateCurrent.CalculatedMeasureDataS[i], baseDataTemp);
                                baseDataTemp.Channel = stateCurrent.CalculatedMeasureDataS[i].Channel;
                                stateCurrent.BaseData.Add(baseDataTemp);
                                //由于等待测量阶段，本底初始值为本底测量最终结果，所以需要将当前本底值拷贝过去
                                Tools.Clone(stateCurrent.CalculatedMeasureDataS[i], stateReadyToMeasure.CalculatedMeasureDataS[i]);
                            }
                            //如果是单探测器，将左手心、右手心的本底值拷贝到左手背、右手背
                            if (factoryParameter.IsDoubleProbe == false)
                            {
                                Tools.Clone(stateCurrent.BaseData[0], stateCurrent.BaseData[1]);
                                Tools.Clone(stateCurrent.BaseData[2], stateCurrent.BaseData[3]);
                            }
                            //下一个状态（等待测量）的文字提示和语音播报
                            uIEventArgs.CurrentState = stateReadyToMeasure;
                            uIEventArgs.CurrentState.SoundMessage = SoundMessage.Ready;
                            //ShowMsg(this,uIEventArgs);
                            ShowMsgReady(this, uIEventArgs);
                            //重新初始化界面测量值显示（归0）
                            //DisplayMeasureData(stateReadyToMeasure.CalculatedMeasureDataS, "cps");
                            //切换到等待测量状态
                            stateCurrent = stateReadyToMeasure;                            
                            stateCurrent.LastTime = System.DateTime.Now;
                        }
                        else
                        {
                            //当前设备状态设置为故障
                            uIEventArgs.DeviceStatus = DeviceStatus.OperatingFaulted;
                            stateCurrent.DeviceStatus = DeviceStatus.OperatingFaulted;
                            //保存故障信息
                            //将故障信息errRecord写入数据库
                            AddErrorData(stateCurrent.ErrRecord);
                            isDeviceStatusUpdated = true;
                            //进行自检故障文字和语音提示,自检状态进度图片显示仪器故障颜色
                            //ShowMsg(this, uIEventArgs);
                            ShowMsgBackGround(this, uIEventArgs);
                            //启动故障报警计时
                            State.AlarmTimeStart = System.DateTime.Now.AddSeconds(1);
                            //重新启动本底测量，求平均值需要将累加测量初始值清零
                            for (int j = 0; j < usedChannelS.Count; j++)
                            {
                                //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次计算做准备
                                stateCurrent.CalculatedMeasureDataS[j].Alpha = 0;
                                stateCurrent.CalculatedMeasureDataS[j].Beta = 0;
                            }
                            isSelfCheckSended = false;//置是否已经下发自检指令标志为false（需重新下发自检指令）
                            isBetaCommandToSend = false;//
                            stateCurrent = stateReadyToRun;                            
                        }
                    }
                }
                return;
            }
            if(stateCurrent==stateReadyToMeasure)
            {
                uIEventArgs.CurrentState = stateReadyToMeasure;
                //对采集的监测数据进行处理
                int runStatus = stateCurrent.Run(usedChannelS, measureDataS, uIEventArgs);
                //1s时间未到，直接返回
                if (runStatus == -1)
                {
                    return;
                }
                //红外全部到位，转开始测量状态
                if (runStatus==1)
                {                                                       
                    foreach(MeasureData data in stateHandFootMeasuring.CalculatedMeasureDataS)
                    {
                        //手脚测量初始值归0
                        data.Alpha = 0;
                        data.Beta = 0;
                        //开始测量时默认红外全部到位
                        data.InfraredStatus = 1;
                    }
                    uIEventArgs.CurrentState = stateHandFootMeasuring;
                    uIEventArgs.CurrentState.SoundMessage = SoundMessage.Measuring;                    
                    //下一个状态（开始测量）的文字提示和语音播报                    
                    ShowMsgHandFootMeasuring(this,uIEventArgs);                    
                    //重新初始化界面测量值显示（归0）
                    DisplayMeasureData(stateHandFootMeasuring.CalculatedMeasureDataS, "cps");
                    stateCurrent = stateHandFootMeasuring;
                    stateCurrent.LastTime= System.DateTime.Now;
                    stateCurrent.HoldTimes = stateHandFootMeasuring.SetTimes;
                    return;
                }
                //平滑时间到，则进行本底判断
                if(stateCurrent.HoldTimes < 0)
                {                    
                    //进行本底判断
                    stateCurrent.ErrRecord = stateCurrent.BaseCheck(usedChannelS, probeParameterS, uIEventArgs);
                    //显示当前本底测量结果
                    DisplayMeasureData(stateCurrent.CalculatedMeasureDataS, "cps");
                    //如果是单探测器，则将左右手背显示清零
                    if (stateCurrent.FactoryParameter.IsDoubleProbe == false)
                    {
                        LblLHBA.Text = "α0.00cps";
                        LblLHBB.Text = "β0.00cps";
                        LblRHBA.Text = "α0.00cps";
                        LblRHBB.Text = "β0.00cps";
                    }
                    //本底测量通过
                    if(stateCurrent.ErrRecord==null)
                    {
                        //当前设备状态设置为正常
                        uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
                        stateCurrent.DeviceStatus = DeviceStatus.OperatingNormally;
                        for (int i = 0; i < usedChannelS.Count; i++)
                        {
                            //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                            MeasureData baseDataTemp = new MeasureData();
                            Tools.Clone(stateCurrent.CalculatedMeasureDataS[i], stateCurrent.BaseData[i]);                                                        
                        }
                        //如果是单探测器，将左手心、右手心的本底值拷贝到左手背、右手背
                        if (factoryParameter.IsDoubleProbe == false)
                        {
                            Tools.Clone(stateCurrent.BaseData[0], stateCurrent.BaseData[1]);
                            Tools.Clone(stateCurrent.BaseData[2], stateCurrent.BaseData[3]);
                        }
                        //重新开始计时
                        stateCurrent.HoldTimes = stateCurrent.SetTimes;
                    }
                    //本底检测未通过
                    else
                    {
                        //当前设备状态设置为故障
                        uIEventArgs.DeviceStatus = DeviceStatus.OperatingFaulted;
                        stateBackgroundMeasure.DeviceStatus = DeviceStatus.OperatingFaulted;
                        //保存故障信息
                        //将故障信息errRecord写入数据库
                        AddErrorData(stateCurrent.ErrRecord);
                        isDeviceStatusUpdated = true;
                        //进行本底故障文字和语音提示,本底测量状态进度图片显示仪器故障颜色
                        //ShowMsg(this,uIEventArgs);
                        ShowMsgBackGround(this, uIEventArgs);
                        //启动故障报警计时
                        State.AlarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        //本底测量数据报文计数器清零
                        backgroundCount = 0;
                        stateCurrent = stateBackgroundMeasure;
                    }
                }                
                return;
            }
            if(stateCurrent==stateHandFootMeasuring)
            {                
                uIEventArgs.CurrentState = stateHandFootMeasuring;
                //对采集的监测数据进行处理
                int runStatus = stateCurrent.Run(usedChannelS, measureDataS,uIEventArgs);
                //1s时间未到，直接返回
                if (runStatus == -1)
                {
                    return;
                }
                //测量过程被中断，转等待测量状态
                if(runStatus==0)
                {
                    //下一个状态（等待测量）的文字提示和语音播报
                    uIEventArgs.CurrentState = stateReadyToMeasure;
                    //屏蔽语音提示（从开始测量状态进入等待测量状态时，不再进行语音提示）
                    stateReadyToMeasure.SoundMessage = SoundMessage.Mute;
                    ShowMsgReady(this, uIEventArgs);
                    //重新初始化界面测量值显示（归0）
                    DisplayMeasureData(stateReadyToMeasure.CalculatedMeasureDataS, "cps");
                    //切换到等待测量状态
                    stateCurrent = stateReadyToMeasure;
                    stateCurrent.LastTime = System.DateTime.Now;
                    stateCurrent.HoldTimes = stateReadyToMeasure.SetTimes;
                    return;
                }
                //测量数据正常处理
                if(runStatus==1)
                {                    
                    if (stateCurrent.HoldTimes<=0)
                    {
                        //测量时间到声音提示
                        uIEventArgs.CurrentState.SoundMessage = SoundMessage.DiDa_2;                        
                    }
                    else
                    {
                        //测量过程声音提示                        
                        uIEventArgs.CurrentState.SoundMessage = SoundMessage.DiDa_1;
                    }
                    ShowMsgHandFootMeasuring(this, uIEventArgs);
                }
                //测量时间到
                if(stateCurrent.HoldTimes<=0)
                {                    
                    stateCurrent.ErrRecord = stateHandFootMeasuring.BaseCheck(usedChannelS, efficiencyParameterS, probeParameterS,uIEventArgs);                    
                    //单探测器检测，则探测器接到手心/手背一个道盒，所以手心手背的检测需分两次进行
                    if (stateCurrent.FactoryParameter.IsDoubleProbe==false)
                    {
                        //手部第一次检测完成
                        if (stateCurrent.HandProbeCtrl == HandProbeCtrl.StepOneReady)
                        {
                            //置手部检测状态为手部第一次检测完成
                            stateCurrent.HandProbeCtrl = HandProbeCtrl.StepOneComplete;
                            //转换到等待测量时不需要语音提示“等待测量”，所以置声音消息为静音Mute
                            uIEventArgs.CurrentState.SoundMessage = SoundMessage.Mute;
                            //显示切换到等待测量
                            ShowMsgReady(this, uIEventArgs);
                            stateCurrent = stateReadyToMeasure;                            
                            stateCurrent.HoldTimes = stateCurrent.SetTimes;
                            stateCurrent.LastTime = DateTime.Now;
                            return;
                        }
                    }
                    //手脚检测完成，置手部第二次检测完成标志(手部检测完成标志)
                    stateCurrent.HandProbeCtrl = HandProbeCtrl.StepTwoComplete;                    
                    //按照系统参数单位要求显示最终测量结果,级显示单位转换后的conversionDataS列表值
                    DisplayMeasureData(stateHandFootMeasuring.ConversionDataS, systemParameter.MeasurementUnit);
                    //清除转换后用于显示的监测数据存储列表conversionDataS为下次监测做准备
                    stateHandFootMeasuring.ConversionDataS.Clear();
                    //更新测量次数（+1）              
                    systemParameter.MeasuredCount++;
                    systemParameter.UpdateMeasuredCount();
                    //同步测量结果信息
                    uIEventArgs.CurrentState.ErrRecord = stateCurrent.ErrRecord;
                    //同步BaseCheck返回的设备状态
                    uIEventArgs.DeviceStatus = stateCurrent.DeviceStatus;
                    //在测量结果区域显示测量结果，对污染状态背景色进行设置
                    ShowMsgHandFootMeasuring(this, uIEventArgs);
                    //无污染,衣物测量开启且未完成检测转等待测量（StateReadyToMeasure）状态，否则转StateResult状态
                    if (stateCurrent.ErrRecord==null)
                    {
                        //将本次测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次测量时计算做准备
                        for (int i = 0; i < usedChannelS.Count; i++)
                        {
                            stateCurrent.CalculatedMeasureDataS[i].Alpha = 0;
                            stateCurrent.CalculatedMeasureDataS[i].Beta = 0;
                        }                                               
                        //按照系统参数单位要求显示无污染测量结果0，因已经把stateCurrent.CalculatedMeasureDataS清零，所以显示stateCurrent.CalculatedMeasureDataSS内容即可
                        //DisplayMeasureData(stateCurrent.CalculatedMeasureDataS, stateCurrent.SystemParameter.MeasurementUnit);                                                                                                                                                                  
                    }
                    //有污染
                    else
                    {
                        //将本次测量数据和污染描述字符串pollutionRecord保存到数据库
                        MeasureData measureData = new MeasureData();
                        measureData.MeasureDate = DateTime.Now;
                        measureData.MeasureStatus = "污染";
                        measureData.DetailedInfo[Language.Chinese] = stateCurrent.ErrRecord[Language.Chinese];
                        measureData.IsEnglish = false;
                        measureData.AddData(measureData);
                        measureData.MeasureStatus = "Contaminated";
                        measureData.DetailedInfo[Language.English] = stateCurrent.ErrRecord[Language.English];
                        measureData.IsEnglish = true;
                        measureData.AddData(measureData);
                        //将当前状态污染记录保存到Result状态
                        stateResult.ErrRecord[Language.Chinese] += stateCurrent.ErrRecord[Language.Chinese];
                        stateResult.ErrRecord[Language.English] += stateCurrent.ErrRecord[Language.English];
                        //启动报警计时
                        State.AlarmTimeStart = DateTime.Now.AddSeconds(1);
                    }                    
                    //转StateResult
                    stateCurrent = stateResult;
                    return;
                }
            }
            if(stateCurrent==stateResult)
            {
                //保证1s处理一次，数据不做任何处理
                int runStatus = stateCurrent.Run(usedChannelS, measureDataS, uIEventArgs);
                //1s时间未到，直接返回
                if (runStatus==-1)
                {
                    return;
                }
                //控制测量结果显示区域文本框显示行数
                if (TxtShowResult.GetLineFromCharIndex(TxtShowResult.TextLength) > 16)
                {
                    int start = TxtShowResult.GetFirstCharIndexFromLine(0);
                    int end = TxtShowResult.GetFirstCharIndexFromLine(TxtShowResult.GetLineFromCharIndex(TxtShowResult.TextLength) - 16);
                    TxtShowResult.Select(start, end);
                    TxtShowResult.SelectedText = "";
                }                
                //本次测量无污染
                if (stateCurrent.DeviceStatus==DeviceStatus.OperatingNormally && stateFrisker.DeviceStatus==DeviceStatus.OperatingNormally)
                {
                    //手脚检测已经完成
                    if(stateHandFootMeasuring.HandProbeCtrl==HandProbeCtrl.StepTwoComplete)
                    {
                        //衣物测量启用且衣物还未完成检测
                        if (measureDataS[6].Channel.IsEnabled == true && stateFrisker.IsTested == false)
                        {                           
                            //设定衣物探头状态为未拿起
                            stateFrisker.FriskerStatus = FriskerStatus.NotPickUp;                                                                                     
                        }                                               
                    }
                    //衣物检测已经完成
                    if (stateFrisker.IsTested == true)
                    {
                        //衣物测量完成标志复位，为下一次测量准备
                        stateFrisker.IsTested = false;                                       
                    }
                    //本次测量无污染,仪器无污染状态背景色设置为无污染
                    //PnlNoContamination.BackgroundImage = Resources.NoContamination_progress;                    
                }                
                //本次测量有污染
                else
                {
                    if (playControl % 3 == 0)
                    {
                        //语音提示本次测量结果                        
                        ShowMsgResult(this, uIEventArgs);
                    }
                    //报警时间小于设定报警时间长度，则返回等待
                    if ((DateTime.Now-State.AlarmTimeStart).Seconds<State.AlarmTimes)
                    {
                        //清除预读取数据
                        for (int i = 0; i < measureDataS.Count; i++)
                        {
                            measureDataS[i].Alpha = 0;
                            measureDataS[i].Beta = 0;
                            measureDataS[i].InfraredStatus = 0;
                        }
                        commPort.ClearPortData();
                        playControl++;
                        return;
                    }                    
                    else
                    {
                        //语音控制复位
                        playControl = 0;
                        //错误和污染记录复位
                        stateHandFootMeasuring.ErrRecord = null;
                        stateFrisker.ErrRecord = null;
                        stateResult.ErrRecord = null;
                        //衣物监测离线次数复位
                        stateFrisker.OfflineTimeCount = 0;
                        //设备状态恢复为正常
                        stateHandFootMeasuring.DeviceStatus = DeviceStatus.OperatingNormally;
                        stateFrisker.DeviceStatus = DeviceStatus.OperatingNormally;
                        uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;                                                                                                
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS初始化为0//当前本底值，为本底测量时计算做准备
                        foreach (MeasureData data in stateBackgroundMeasure.CalculatedMeasureDataS)
                        {
                            data.Alpha = 0;
                            data.Beta = 0;
                        }
                        //本底测量开始时，初始值显示为0
                        DisplayMeasureData(calculatedMeasureDataS, "cps");
                        //恢复为女声播报语音
                        uIEventArgs.isAudioMan = false;
                        //下一个状态（本底）的文字提示和语音播报                        
                        ShowMsgBackGround(this, uIEventArgs);                        
                        //切换状态到本底测量
                        stateCurrent = stateBackgroundMeasure;
                        stateCurrent.IsShowMsg = false;//未进行有效信息显示和语音播报
                        stateCurrent.HoldTimes = systemParameter.SmoothingTime;
                        stateCurrent.LastTime = System.DateTime.Now;
                        return;
                    }
                }
                //检测次数大于强制本底次数、衣物离线时间大于设置时间、有污染（手脚、衣物）则强制本底
                if(systemParameter.MeasuredCount>=systemParameter.BkgUpdate || stateFrisker.OfflineTimeCount>=systemParameter.ClothOfflineTime)
                {
                    //语音控制复位
                    playControl = 0;
                    //错误和污染记录复位
                    stateHandFootMeasuring.ErrRecord = null;
                    stateFrisker.ErrRecord = null;
                    stateResult.ErrRecord = null;
                    //衣物监测离线次数复位
                    stateFrisker.OfflineTimeCount = 0;
                    //设备状态恢复为正常
                    stateHandFootMeasuring.DeviceStatus = DeviceStatus.OperatingNormally;
                    stateFrisker.DeviceStatus = DeviceStatus.OperatingNormally;
                    uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
                    //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS初始化为0//当前本底值，为本底测量时计算做准备
                    foreach (MeasureData data in stateCurrent.CalculatedMeasureDataS)
                    {
                        data.Alpha = 0;
                        data.Beta = 0;
                    }
                    //本底测量开始时，初始值显示为0
                    DisplayMeasureData(calculatedMeasureDataS, "cps");
                    //恢复为女声播报语音
                    uIEventArgs.isAudioMan = false;
                    //下一个状态（本底）的文字提示和语音播报                        
                    ShowMsgBackGround(this, uIEventArgs);
                    //切换状态到本底测量
                    stateCurrent = stateBackgroundMeasure;
                    stateCurrent.IsShowMsg = false;//未进行有效信息显示和语音播报
                    stateCurrent.HoldTimes = systemParameter.SmoothingTime;
                    stateCurrent.LastTime = System.DateTime.Now;
                }
                //由于等待测量阶段，本底初始值为本底测量最终结果，所以需要将当前本底值拷贝过去
                for(int i=0;i<usedChannelS.Count;i++)
                {
                    Tools.Clone(stateCurrent.BaseData[i], stateReadyToMeasure.CalculatedMeasureDataS[i]);
                }
                //设备状态恢复为正常
                stateHandFootMeasuring.DeviceStatus = DeviceStatus.OperatingNormally;
                stateFrisker.DeviceStatus = DeviceStatus.OperatingNormally;
                uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
                //转等待测量状态
                stateCurrent = stateReadyToMeasure;
                stateCurrent.SetTimes = systemParameter.SmoothingTime;
                stateCurrent.HoldTimes = stateCurrent.SetTimes;
                stateCurrent.LastTime = DateTime.Now;
                uIEventArgs.isAudioMan = false;
                return;
            }                                  
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
            //读取串口回传数据并赋值给receiveBuffMessage
            byte[] receiveBuffMessage = null;            
            bool isFindStatusMessage = false;//是否找到正确的状态上报报文
            bool isFindTimeSynMessage = false;//是否找到正确的时间同步报文
            int index = 0;//遍历报文索引
            //巡检管理机下发的报文
            while (true)
            {
                //遍历接收缓冲区后找到的有效报文
                byte[] effectiveMessage = new byte[1024];
                //请求进程中断读取数据
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }

                try
                {
                    //读取串口数据
                    byte[] receiveDataTemp;
                    receiveDataTemp = Components.Message.ReceiveMessage(commPort_Supervisory, 1024);
                    if (receiveDataTemp.Count() > 0)
                    {
                        File.AppendAllText(appPath + "\\log\\msg.txt", "当前串口回传信息-1：" + BitConverter.ToString(receiveDataTemp) + "\r\n");
                    }
                    if (receiveBuffMessage == null)
                    {
                        receiveBuffMessage = receiveDataTemp;
                    }
                    else
                    {
                        //将串口数据放到接收数据缓冲区末尾
                        receiveBuffMessage = Tools.CopyTo(receiveBuffMessage, receiveDataTemp);
                    }                    
                    if (receiveBuffMessage.Count() > 0)
                    {
                        File.AppendAllText(appPath + "\\log\\msg.txt", "当前接收缓冲区信息-组包-1：" + BitConverter.ToString(receiveBuffMessage) + "\r\n");
                    }
                }
                catch
                {
                    TxtShowResult.Text += "管理机端口通信错误！\r\n";
                    isCommReportError = true;
                }
                //触发向主线程返回下位机上传数据事件，如果是时间同步报文，需要读两次串口才能将17个字节数据读回来
                if (receiveBuffMessage == null || receiveBuffMessage.Count() == 0)
                {
                    continue;
                }
                while (receiveBuffMessage.Count() < 8)//串口回传数据小于最小报文长度（上报状态8字节，时间同步17字节）
                {
                    //继续读串口数据，直到最小报文长度8字节
                    byte[] receiveDataTemp;
                    receiveDataTemp = Components.Message.ReceiveMessage(commPort_Supervisory);//继续从串口读时间同步报文                                 
                    if (receiveDataTemp.Count() > 0)//串口有回传数据
                    {
                        File.AppendAllText(appPath + "\\log\\msg.txt", "当前串口回传信息-2：" + BitConverter.ToString(receiveDataTemp) + "\r\n");
                        receiveBuffMessage = Tools.CopyTo(receiveBuffMessage, receiveDataTemp);
                        File.AppendAllText(appPath + "\\log\\msg.txt", "当前接收缓冲区信息-组包-2：" + BitConverter.ToString(receiveBuffMessage) + "\r\n");
                    }
                }
                //遍历报文判断报文是否包含上报状态报文或时间同步报文标志信息
                for (index = 0; index <= receiveBuffMessage.Length - 8; index++)
                {
                    //遍历整个报文，判断是否为上报状态报文
                    if ((receiveBuffMessage[index + 1] == 0x03 && receiveBuffMessage[index + 2] == 0x00 && receiveBuffMessage[index + 3] == 0x00 && receiveBuffMessage[index + 4] == 0x00 && receiveBuffMessage[index + 5] == 0x05) || (receiveBuffMessage[index + 1] == 0x03 && receiveBuffMessage[index + 2] == 0x00 && receiveBuffMessage[index + 3] == 0x04 && receiveBuffMessage[index + 4] == 0x00 && receiveBuffMessage[index + 5] == 0x01))
                    {
                        //找到正确的状态上报报文，将报文信息保存到新的报文数组effectiveMessage中
                        //byte[] effectiveMessage = new byte[8];                                               
                        Array.Copy(receiveBuffMessage, index, effectiveMessage, 0, 8);
                        //由于effectiveMessage数组长度为1024，所以需要将多余的元素删除掉
                        List<byte> list = new List<byte>();
                        list = effectiveMessage.ToList();
                        list.RemoveRange(8, effectiveMessage.Length - 8);
                        effectiveMessage = list.ToArray();
                        File.AppendAllText(appPath + "\\log\\msg.txt", "处理后信息-遍历03指令结果：" + BitConverter.ToString(effectiveMessage) + "\r\n");
                        isFindStatusMessage = true;
                        break;
                    }
                    //遍历整个报文，判断是否为时间同步报文
                    if (receiveBuffMessage[index] == 0x00 && receiveBuffMessage[index + 1] == 0x10 && receiveBuffMessage[index + 2] == 0x11 && receiveBuffMessage[index + 3] == 0x00)
                    {
                        //找到正确的时间同步报文，将包含时间同步报文头信息的剩余串口回传数据保存到新的报文数组effectiveMessage中
                        //byte[] effectiveMessage = new byte[1024];                                            
                        //Array.Copy(receiveBuffMessage, index, effectiveMessage, 0, receiveBuffMessage.Length - index);
                        ////由于effectiveMessage数组长度为1024，所以需要将多余的元素删除掉
                        //List<byte> list = new List<byte>();
                        //list = effectiveMessage.ToList();
                        //list.RemoveRange(receiveBuffMessage.Length - index-1, effectiveMessage.Length -(receiveBuffMessage.Length - index));
                        //effectiveMessage = list.ToArray();
                        //File.AppendAllText(appPath + "\\log\\msg.txt", "处理后信息-遍历时间同步指令：" + BitConverter.ToString(effectiveMessage) + "\r\n");
                        //时间同步报文长度为17字节，如果接收缓冲区中有效报文长度小于17字节，则需要分多次读取直到读取到17个字节的串口数据
                        while ((receiveBuffMessage.Count() - index) < 17)
                        {
                            byte[] receiveDataTemp;
                            receiveDataTemp = Components.Message.ReceiveMessage(commPort_Supervisory);//继续从串口读时间同步报文     
                            if (receiveDataTemp.Count() > 0)//串口有回传数据
                            {
                                File.AppendAllText(appPath + "\\log\\msg.txt", "当前串口回传信息-3：" + BitConverter.ToString(receiveDataTemp) + "\r\n");
                                receiveBuffMessage = Tools.CopyTo(receiveBuffMessage, receiveDataTemp);//将串口回传数据拼接到有效报文后面
                                File.AppendAllText(appPath + "\\log\\msg.txt", "处理后信息-时间同步组包：" + BitConverter.ToString(receiveBuffMessage) + "\r\n");
                            }
                            Thread.Sleep(10);
                        }
                        //将遍历找到的时间同步报文信息从接收缓冲区receiveBuffMessage拷贝到有效报文数组effectiveMessage
                        Array.Copy(receiveBuffMessage, index, effectiveMessage, 0, 17);
                        //由于effectiveMessage数组长度为1024，所以需要将多余的元素删除掉
                        List<byte> list = new List<byte>();
                        list = effectiveMessage.ToList();
                        list.RemoveRange(17, effectiveMessage.Length - 17);
                        effectiveMessage = list.ToArray();
                        File.AppendAllText(appPath + "\\log\\msg.txt", "处理后信息-时间同步报文结果：" + BitConverter.ToString(effectiveMessage) + "\r\n");
                        isFindTimeSynMessage = true;
                        break;
                    }
                }
                if (isFindStatusMessage == false && isFindTimeSynMessage == false)//还未找到正确报文
                {
                    //将串口回传信息中除去已经判断的数据，将剩余报文信息保存到接收缓冲区receiveBuffMessage中
                    List<byte> list = new List<byte>(receiveBuffMessage);
                    list.RemoveRange(0, index + 1);
                    receiveBuffMessage = list.ToArray();                    
                }
                else
                {
                    //已经找到正确报文
                    isCommReportError = false;
                    if (isFindStatusMessage == true)
                    {
                        //上报状态报文,从接收缓冲区中将上报状态报文信息剔除
                        List<byte> list = new List<byte>(receiveBuffMessage);
                        list.RemoveRange(0, index+8);
                        receiveBuffMessage = list.ToArray();
                    }
                    if (isFindTimeSynMessage == true)
                    {
                        //时间同步报文，从接收缓冲区中将时间同步报文信息剔除
                        List<byte> list = new List<byte>(receiveBuffMessage);
                        list.RemoveRange(0, index+17);
                        receiveBuffMessage = list.ToArray();
                    }
                    isFindStatusMessage = false;//为下一个报文读取解析做准备
                    isFindTimeSynMessage = false;
                    //第一次启动后，进入设置，设置完成端口后，回到主线程报错误，没有开启进度显示，所以加判断
                    if (bkWorkerReportStatus.WorkerReportsProgress == false)
                    {
                        bkWorkerReportStatus.WorkerReportsProgress = true;
                    }
                    worker.ReportProgress(1, effectiveMessage);
                    //Array.Clear(effectiveMessage, 0, 1024);
                }
                Thread.Sleep(10);
            }
            
            /************************************************************    
            while(true)
            {
                //请求进程中断读取数据
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }
                //读取串口回传数据并赋值给receiveBuffMessage
                byte[] receiveBuffMessage=new byte[1024];
                try
                {
                    receiveBuffMessage = Components.Message.ReceiveMessage(commPort_Supervisory,1024);
                    //if (receiveBuffMessage.Count() > 0)
                    //{
                    //    File.AppendAllText(appPath + "\\log\\msg.txt", "串口回传信息：" +BitConverter.ToString(receiveBuffMessage) + "\r\n");
                    //}
                }
                catch
                {
                    TxtShowResult.Text += "管理机端口通信错误！\r\n";
                    isCommReportError = true;
                }
                //触发向主线程返回下位机上传数据事件，如果是时间同步报文，需要读两次串口才能将17个字节数据读回来
                if (receiveBuffMessage == null || receiveBuffMessage.Count() == 0)
                {
                    continue;
                }
                if (receiveBuffMessage.Count() >= 4)
                {
                    if (receiveBuffMessage[0] == 0x00 && receiveBuffMessage[1] == 0x10 && receiveBuffMessage[2] == 0x11 && receiveBuffMessage[3] == 0x00)//时间同步报文
                    {                                                    
                        while (receiveBuffMessage.Count()<17)//时间同步报文长度为17，可能需要分多次读取
                        {
                            byte[] receiveDataTemp = new byte[1024];
                            receiveDataTemp = Components.Message.ReceiveMessage(commPort_Supervisory);//继续从串口读时间同步报文                                 
                            receiveBuffMessage = Tools.CopyTo(receiveBuffMessage, receiveDataTemp);
                            Thread.Sleep(10);
                            //File.AppendAllText(appPath + "\\log\\msg.txt", "处理后信息：" + BitConverter.ToString(receiveBuffMessage) + "\r\n");
                        }                                        
                    }
                    isCommReportError = false;
                    //第一次启动后，进入设置，设置完成端口后，回到主线程报错误，没有开启进度显示，所以加判断
                    if (bkWorkerReportStatus.WorkerReportsProgress == false)
                    {
                        bkWorkerReportStatus.WorkerReportsProgress = true;
                    }
                    worker.ReportProgress(1, receiveBuffMessage);
                }                
                Thread.Sleep(10);  
            }
            ***************************************************************************/       
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
            else
            {
                return;
            }
            //接收报文数据为空，说明没有收到管理机下发的命令
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 20)
                {
                    errNumber = 0;
                    //界面提示“通讯错误”
                    if (isEnglish)
                    {
                        TxtShowResult.Text = "Communication with Supervisor Error！";
                    }
                    else
                    {
                        TxtShowResult.Text += "管理机通讯错误！\r\n";
                        isCommReportError = true;
                    }
                }
                else
                {
                    errNumber++;
                }
                return;
            }
            errNumber = 0;
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            message = Components.Message.ExplainMessage(receiveBufferMessage);            
            if (message == null||message.Count()<=0)//解析失败
            {
                return;
            }
            isCommReportError = false;            
            //解析成功
            if (message.Count()>=7)//长度大于7，为时间同步命令
            {
                //将当前系统时间同步为管理机下发的时间
                SYSTEMTIME timeForSyn = new SYSTEMTIME();
                timeForSyn.Year =(short) message[0];
                timeForSyn.Month = (short)message[1];
                timeForSyn.Day = (short)message[2];
                //timeForSyn.Hour = (short)(message[3] - 8) <= 0 ? (short)(message[3] - 8 + 24) : (short)(message[3] - 8);//设置系统时间时，时间需和实际时间差8小时
                timeForSyn.Hour = (short)message[3];
                timeForSyn.Minute = (short)message[4];
                timeForSyn.Second = (short)message[5];
                timeForSyn.MiliSecond = (short)message[6];
                try
                {
                    //SetSystemTime(ref timeForSyn);
                    SetLocalTime(ref timeForSyn);
                    stateTimeStart = DateTime.Now;//同步时间后重新开始检测运行状态倒计时
                    //向管理机回复时间同步报文
                    //byte[] timeSynMessage = new byte[8];
                    //try
                    //{
                    //    byte deviceAddress= Convert.ToByte(factoryParameter.DeviceAddress); //0x01;
                    //    timeSynMessage[0] = deviceAddress;
                    //}
                    //catch 
                    //{
                    //    if (isEnglish)
                    //    {
                    //        TxtShowResult.Text += "Time synchronization completed,reply failed!\r\n";
                    //    }
                    //    else
                    //    {
                    //        TxtShowResult.Text += "时间同步完成,向管理机应答失败!\r\n";
                    //    }
                    //}
                    
                    //timeSynMessage[1] = 0x10;
                    //timeSynMessage[2] = 0x11;
                    //timeSynMessage[3] = 0x00;
                    //timeSynMessage[4] = 0x00;
                    //timeSynMessage[5] = 0x04;
                    ////生成CRC校验码
                    //byte[] crc16 = new byte[2];
                    //crc16 = Tools.CRC16(timeSynMessage, timeSynMessage.Length - 2);
                    //timeSynMessage[6] = crc16[0];
                    //timeSynMessage[7] = crc16[1];
                    //Components.Message.SendMessage(timeSynMessage, commPort_Supervisory);                       
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
                //if(isAudioContaminatedPlayed==false)
                //{
                //    return;
                //}
                //下发地址和当前设备地址不一致则返回
                if (factoryParameter.DeviceAddress != message[0].ToString())
                {
                    return;
                }
                else //下发地址和当前设备地址一致，则上传当前测试状态
                {
                    byte[] deviceStatusMessage=null;
                    if (isDeviceStatusUpdated == true)//数据库监测数据和故障数据已经更新，应该进行状态查询上报
                    {
                        //从数据库中查询最近一次记录的测量数据
                        MeasureData measureData = new MeasureData();
                        measureData.GetLatestData();
                        //从数据库中查询最近一次记录的故障数据
                        ErrorData errorData = new ErrorData();
                        errorData.GetLatestData();
                        //测量数据和故障数据都为空，说明仪器正常,因为中英文故障数据描述成对出现，所以只要判断一个（英文）的详细信息即可
                        if ((measureData.MeasureID == 0 || errorData.ErrID == 0) || string.IsNullOrEmpty(measureData.DetailedInfo[Language.English]) && string.IsNullOrEmpty(errorData.Record))
                        {
                            deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), DateTime.Now, 0x01, message[1], message[2]);//0x01:仪器正常
                        }
                        //监测数据和故障数据都已经上报，说明最近仪器正常，上报时间为当前时间
                        if (measureData.IsReported == true && errorData.IsReported == true)
                        {
                            deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), DateTime.Now, 0x01, message[1], message[2]);//0x01:仪器正常
                        }
                        //监测数据上报，故障数据未上报，说明最近仪器故障，上报完成后更新故障数据状态为已上报
                        if ((errorData.ErrID != 0) && (string.IsNullOrEmpty(measureData.DetailedInfo[Language.English]) == false && measureData.IsReported == true) && errorData.IsReported == false)
                        {
                            deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), errorData.ErrTime, 0x02, message[1], message[2]);//0x02:仪器故障
                        }
                        //监测数据未上报，故障数据上报，说明最近状态为污染，上报完成后更新监测数据状态为已上报
                        if ((measureData.MeasureID != 0) && measureData.IsReported == false && (errorData.IsReported == true && string.IsNullOrEmpty(errorData.Record) == false))
                        {
                            deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), measureData.MeasureDate, 0x04, message[1], message[2]);//0x04:仪器污染
                        }
                        //监测数据和故障数据都未上报，则将最近的状态进行上报，更新两条记录为已上报
                        if ((measureData.MeasureID != 0 || errorData.ErrID != 0) && measureData.IsReported == false && errorData.IsReported == false)
                        {
                            if (measureData.MeasureDate > errorData.ErrTime)//最近一次记录为MeasureData，说明是状态为污染（因为只有污染状态才会记录，正常不记录）
                            {
                                //生成上报管理机的监测仪测试状态(污染)报文                    
                                deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), measureData.MeasureDate, 0x04, message[1], message[2]);//仪器状态为污染
                            }
                            else
                            {
                                //生成上报管理机的监测仪测试状态（故障）报文                    
                                deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), errorData.ErrTime, 0x02, message[1], message[2]);//仪器状态为故障
                            }
                        }
                        //向管理机上报仪器检测状态
                        if (Components.Message.SendMessage(deviceStatusMessage, commPort_Supervisory))//上报成功
                        {
                            isDeviceStatusUpdated = false;
                            //数据库中更新上报标志
                            if (string.IsNullOrEmpty(measureData.DetailedInfo[Language.English]) == false && measureData.IsReported == false)
                            {
                                //更新上报标志
                                measureData.UpdataReported(true, measureData.MeasureID);
                                measureData.UpdataReported(true, measureData.MeasureID - 1);
                            }
                            if (string.IsNullOrEmpty(errorData.Record) == false && errorData.IsReported == false)
                            {
                                //更新上报标志
                                errorData.UpdateReported(true, errorData.ErrID);
                                errorData.UpdateReported(true, errorData.ErrID - 1);
                            }
                        }
                        else
                        {
                            if (isEnglish)
                            {
                                TxtShowResult.Text += "Report status failed!\r\n";
                            }
                            else
                            {
                                TxtShowResult.Text += "上报状态失败\r\n";
                            }
                        }
                    }
                    else//设备状态未更新，则上报设备状态为正常
                    {
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), DateTime.Now, 0x01, message[1], message[2]);//0x01:仪器正常                                                                                                                                                                          //向管理机上报仪器检测状态
                        if (Components.Message.SendMessage(deviceStatusMessage, commPort_Supervisory)==false)//上报失败
                        {
                            if (isEnglish)
                            {
                                TxtShowResult.Text += "Report status failed!\r\n";
                            }
                            else
                            {
                                TxtShowResult.Text += "上报状态失败\r\n";
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// UI定时刷新
        /// 1、根据当前检测进度，刷新显示当前检测进度标志
        /// 2、根据串口回传各个通道红外状态刷新显示各个通道是否到位（200ms周期）
        /// 3、根据仪器状态刷新显示各个通道状态（正常、故障、一级污染、二级污染）、测量值
        /// 4、显示当前时间
        /// 5、显示当前检测状态倒计时        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void TmrUIControl_Tick(object sender,UIEventArgs e)
        //{            
        //    Type type = typeof(StateSelfCheck);
        //    switch(e.CurrentState.GetType().Name)
        //    {
        //        case "StateSelfCheck":
        //            if (e.MeasureStatus.ContainsValue(DeviceStatus.OperatingFaulted))//仪器出现自检故障
        //            {
        //                //仪器自检状态标签设置为故障状态图片
        //                PnlSelfCheck.BackgroundImage = Resources.Fault_progress;
        //                //根据当前语言设置，进度状态标签显示“仪器故障”
        //                if (isEnglish)
        //                {
        //                    LblSelfCheck.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Bold);
        //                    LblSelfCheck.Text = "Fault";
        //                }
        //                else
        //                {
        //                    LblSelfCheck.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Bold);
        //                    LblSelfCheck.Text = "仪器故障";
        //                }
        //                //将自检出现故障的通道名字标签背景色设置为ERROR                        
        //                var list = (from d in e.MeasureStatus where d.Value == DeviceStatus.OperatingFaulted select d.Key).ToList();
        //                //foreach (Channel channel in list)
        //                //{
        //                //    Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", channel.ChannelName_English)]);
        //                //    ((Label)(panel.Controls[string.Format("Lbl{0}", channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
        //                //}
        //            }
        //            else //自检正常
        //            {
        //                //仪器自检状态标签设置为当前状态图片
        //                PnlSelfCheck.BackgroundImage = Resources.progress;
        //                //取消其它测量状态图片
        //                PnlBackground.BackgroundImage = null;
        //                PnlReady.BackgroundImage = null;
        //                PnlMeasuring.BackgroundImage = null;
        //                if (isEnglish)
        //                {
        //                    TxtShowResult.Text += "Self-checking\r\n";
        //                }
        //                else
        //                {
        //                    TxtShowResult.Text += "仪器自检\r\n";
        //                }                        
        //            }
        //            //文字显示标签置于顶部
        //            LblSelfCheck.BringToFront();
        //            break;
        //        case "StateBackgroundMeasure":
        //            //本底测量状态标签设置为当前状态图片
        //            PnlBackground.BackgroundImage = Resources.progress;
        //            //取消其它测量状态图片
        //            PnlSelfCheck.BackgroundImage = null;
        //            PnlReady.BackgroundImage = null;
        //            PnlMeasuring.BackgroundImage = null;
        //            //文字显示标签置于顶部
        //            LblBackground.BringToFront();
        //            break;
        //        case "StateReadyToMeasure":
        //            //等待测量状态标签设置为当前状态图片
        //            PnlReady.BackgroundImage = Resources.progress;
        //            //取消其它测量状态图片
        //            PnlSelfCheck.BackgroundImage = null;
        //            PnlBackground.BackgroundImage = null;
        //            PnlMeasuring.BackgroundImage = null;
        //            //文字显示标签置于顶部
        //            LblReady.BringToFront();
        //            break;
        //        case "StateMeasuring":
        //            //开始测量状态标签设置为当前状态图片
        //            PnlMeasuring.BackgroundImage = Resources.progress;
        //            //取消其它测量状态图片
        //            PnlSelfCheck.BackgroundImage = null;
        //            PnlBackground.BackgroundImage = null;
        //            PnlReady.BackgroundImage = null;
        //            //文字显示标签置于顶部
        //            LblMeasuring.BringToFront();
        //            break;
        //    }           
        //}
        /// <summary>
        /// 当前设备通道（道盒）设备状态显示背景颜色（图片）
        /// </summary>
        /// <param name="ChannelName_E">通道（道盒）名称</param>
        /// <param name="deviceStatus">设备状态（16：正常；32：故障；63：一级污染；64：二级污染</param>
        private void UIMeasureStatus_Paint(Channel channel,DeviceStatus deviceStatus)
        {
            Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", channel.ChannelName_English)]);
            Label lblChannel = ((Label)(panel.Controls[string.Format("Lbl{0}", channel.ChannelName_English)]));
            Label lblChannelAValue=null;
            Label lblChannelBValue=null;
            if (stateCurrent.FactoryParameter != null)
            {
                if (stateCurrent.FactoryParameter.MeasureType == "α" || stateCurrent.FactoryParameter.MeasureType == "α/β")
                {
                    //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"A"
                    lblChannelAValue = ((Label)(panel.Controls[string.Format("Lbl{0}{1}", channel.ChannelName_English, "A")]));
                }
                if (stateCurrent.FactoryParameter.MeasureType == "β" || stateCurrent.FactoryParameter.MeasureType == "α/β")
                {
                    lblChannelBValue = ((Label)(panel.Controls[string.Format("Lbl{0}{1}", channel.ChannelName_English, "B")]));
                }
            }
            //设置当前道盒（通道）测量值标签文字颜色为正常
            //根据当前设备状态，界面刷新当前通道设备状态显示颜色（图片）
            switch (deviceStatus)
            {                
                case DeviceStatus.OperatingNormally: //正常                                        
                    //刷新当前道盒（通道）名称显示的Label控件背景颜色为正常
                    lblChannel.BackColor = PlatForm.ColorStatus.COLOR_BKNORMAL;
                    if (lblChannelAValue != null)
                    {
                        lblChannelAValue.BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                    }
                    if (lblChannelBValue != null)
                    {
                        lblChannelBValue.BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                    }
                    break;
                case DeviceStatus.OperatingFaulted: //故障
                    //刷新当前道盒（通道）名称显示的Label控件背景颜色为故障
                    lblChannel.BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                    break;
                case DeviceStatus.OperatingContaminated_1: //一级污染
                    if (stateCurrent.FactoryParameter.MeasureType == "α" || stateCurrent.FactoryParameter.MeasureType == "α/β")
                    {
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"A"
                        //Label lblChannelValue = ((Label)(panel.Controls[string.Format("Lbl{0}{1}", channel.ChannelName_English, "A")]));
                        //刷新当前道盒（通道）名称显示的Label控件背景颜色为Alpha检测一级污染
                        lblChannel.BackColor = PlatForm.ColorStatus.COLOR_ALARM_3;
                        lblChannelAValue.BackColor= PlatForm.ColorStatus.COLOR_ALARM_3;
                    }
                    if (stateCurrent.FactoryParameter.MeasureType == "β" || stateCurrent.FactoryParameter.MeasureType == "α/β")
                    {
                        //Label lblChannelValue = ((Label)(panel.Controls[string.Format("Lbl{0}{1}", channel.ChannelName_English, "B")]));
                        //刷新当前道盒（通道）名称显示的Label控件背景颜色为Alpha检测高阶污染
                        lblChannel.BackColor = PlatForm.ColorStatus.COLOR_ALARM_4;
                        lblChannelBValue.BackColor = PlatForm.ColorStatus.COLOR_ALARM_4;
                    }                                      
                    break;
                case DeviceStatus.OperatingContaminated_2: //二级污染
                    //刷新当前道盒（通道）名称显示的Label控件背景颜色为二级污染                    
                    if (stateCurrent.FactoryParameter.MeasureType == "α" || stateCurrent.FactoryParameter.MeasureType == "α/β")
                    {
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"A"
                        //Label lblChannelValue = ((Label)(panel.Controls[string.Format("Lbl{0}{1}", channel.ChannelName_English, "A")]));
                        //刷新当前道盒（通道）名称显示的Label控件背景颜色为Beta检测一级污染
                        lblChannel.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                        lblChannelAValue.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                    }
                    if (stateCurrent.FactoryParameter.MeasureType == "β" || stateCurrent.FactoryParameter.MeasureType == "α/β")
                    {
                        //Label lblChannelValue = ((Label)(panel.Controls[string.Format("Lbl{0}{1}", channel.ChannelName_English, "B")]));
                        //刷新当前道盒（通道）名称显示的Label控件背景颜色为Beta检测高阶污染
                        lblChannel.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                        lblChannelBValue.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                    }
                    break;
            }                        
        }
        /// <summary>
        /// 更新当前设备通道（道盒）红外状态显示背景颜色（图片）
        /// </summary>
        /// <param name="channel">通道（道盒）</param>
        /// <param name="infraredStatus">红外状态（-1:通道未启用；0：红外未到位；1：红外到位）</param>
        private void UIChannelStatus_Paint(Channel channel,ChannelStatus channelStatus)
        {                    
            switch(channelStatus)               
            {
                case ChannelStatus.Disabled: //通道未启用
                    //通道背景Panel图片
                    if (channel.ChannelID >= 1 && channel.ChannelID <= 4)//手部背景Panel为未启用
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Hand_NotInPlace;                      
                    }
                    if (channel.ChannelID == 5 || channel.ChannelID == 6)//脚部背景Panel为未启用
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Foot_NotInPlace;
                    }
                    if (channel.ChannelID == 7)//衣物背景Panel为未启用
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.FriskerBK_NotInPlace;
                    }
                    //通道名称标题Label为Disabled
                    LblTitle[channel.ChannelID - 1].Enabled = false;
                    LblTitle[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                    //每个通道右边标识图片为未启用
                    PicStatus[channel.ChannelID - 1].BackgroundImage =Image.FromFile(appPath + string.Format("\\Images\\{0}_Disabled.png", channel.ChannelName_English));
                    //每个通道测量值显示标签为Disabled
                    if (channel.ChannelID == 7)
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                    }
                    else
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                        LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = false;
                    }
                    //每个通道右边标识图片下方状态标签为未启用
                    LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                    LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                    LblStatus[channel.ChannelID - 1].Text =isEnglish?"Disabled":"未启用";                    
                    break;
                case ChannelStatus.NotInPlace://红外未到位
                case ChannelStatus.InPlace://红外到位
                    //通道背景Panel图片
                    if (channel.ChannelID >= 1 && channel.ChannelID <= 4)//手部背景Panel图片为启用
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Hand_InPlace;                       
                    }
                    if (channel.ChannelID == 5 || channel.ChannelID == 6)//脚部背景Panel图片为启用
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.Foot_InPlace;
                    }
                    if (channel.ChannelID == 7 )//衣物背景Panel图片为启用
                    {
                        PnlStatus[channel.ChannelID - 1].BackgroundImage = Resources.FriskerBK_InPlace; //衣物背景Panel图片为启用                        
                    }
                    //设置通道标题标签状态
                    LblTitle[channel.ChannelID - 1].Enabled = true;
                    LblTitle[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.COLOR_BKENABLED;
                    LblTitle[channel.ChannelID - 1].BackColor = Color.Transparent;
                    //设置通道测量值显示标签
                    if (channel.ChannelID == 7)
                    {
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                        LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                    }
                    else
                    {
                        switch (factoryParameter.MeasureType)
                        {
                            case "α":
                                if (uIEventArgs.DeviceStatus== DeviceStatus.OperatingNormally)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                    //LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = false;
                                    //LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                }
                                break;
                            case "β":
                                if (uIEventArgs.DeviceStatus == DeviceStatus.OperatingNormally)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = false;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                   // LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    //LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                }
                                break;
                            case "α/β":
                                if (uIEventArgs.DeviceStatus == DeviceStatus.OperatingNormally)
                                {
                                    LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    //LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                    LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                    //LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                }
                                break;

                        }
                    }
                    //设置通道标识图片状态
                    if (channelStatus == ChannelStatus.NotInPlace)
                    {
                        PicStatus[channel.ChannelID - 1].BackgroundImage = Image.FromFile(appPath + string.Format("\\Images\\{0}_NotInPlace.png", channel.ChannelName_English));
                        //通道标识图片下面状态提示标签
                        LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                        LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
                        LblStatus[channel.ChannelID - 1].Text = isEnglish ? "NotInPlace" : "未到位";
                    }
                    if (channelStatus == ChannelStatus.InPlace)
                    {
                        //设置通道标识图片状态
                        PicStatus[channel.ChannelID - 1].BackgroundImage = Image.FromFile(appPath + string.Format("\\Images\\{0}_InPlace.png", channel.ChannelName_English));
                        //通道标识图片下面状态提示标签
                        LblStatus[channel.ChannelID - 1].BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                        LblStatus[channel.ChannelID - 1].ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                        LblStatus[channel.ChannelID - 1].Text = isEnglish ? "InPlace" : "已到位";
                    }
                    break;                
            }
            //设置当前通道手部是否到位提示
            switch (channel.ChannelID)
            {
                case 1:
                case 2:
                    if (channelStatus == ChannelStatus.InPlace)//红外到位
                    {
                        if (isEnglish)
                        {
                            LblLeft.Text = "LH in place";
                        }
                        else
                        {
                            LblLeft.Text = "左手到位";
                        }
                        LblLeft.BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                        LblLeft.ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
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
                        LblLeft.BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                        LblLeft.ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
                    }
                    break;
                case 3:
                case 4:
                    if (channelStatus == ChannelStatus.InPlace)//红外到位
                    {
                        if (isEnglish)
                        {
                            LblRight.Text = "RH in place";
                        }
                        else
                        {
                            LblRight.Text = "右手到位";
                        }
                        LblRight.BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                        LblRight.ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
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
                        LblRight.BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                        LblRight.ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
                    }
                    break;                
            }            
        }
        private void TmrDispTime_Tick(object sender, EventArgs e)
        {
            //更新当前显示时间
            LblTime.Text = DateTime.Now.ToLongTimeString();            
            //监测串口状态，如果串口关闭则打开
            if (bkWorkerReceiveData.IsBusy)
            {
                if (isCommError||commPort.Opened==false)//监测端口通讯错误或串口未打开
                {                    
                    try
                    {
                        commPort.Close();
                        if (commPort.Opened == false)
                        {
                            commPort.Open();
                            Thread.Sleep(100);
                            isCommError = false;
                        }
                    }
                    catch(Exception ex)
                    {
                            return;
                    }                    
                }
            }
            if (bkWorkerReportStatus.IsBusy)
            {
                if (isCommReportError||commPort_Supervisory.Opened==false)//上报状态端口通讯错误或串口未打开
                {
                    try
                    {
                        commPort_Supervisory.Close();
                        if (commPort_Supervisory.Opened == false)
                        {
                            commPort_Supervisory.Open();
                            isCommReportError = false;
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }               
        }            
        //private void BtnChinese_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        systemParameter.IsEnglish = false;
        //        systemParameter.SetParameter(systemParameter);
        //        isEnglish = false;
        //        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
        //        this.BtnEnglish.Enabled = true;
        //        this.BtnChinese.Enabled = false;                
        //        Tools.ApplyLanguageResource(this);
        //        DisplayInit();
        //    }
        //    catch
        //    {
        //        MessageBox.Show("切换失败，请稍后再试");
        //        return;
        //    }
        //}

        //private void BtnEnglish_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        systemParameter.IsEnglish = true;
        //        systemParameter.SetParameter(systemParameter);
        //        isEnglish = true;
        //        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        //        this.BtnChinese.Enabled = true;
        //        this.BtnEnglish.Enabled = false;                
        //        Tools.ApplyLanguageResource(this);
        //        DisplayInit();
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Switch Fault，please try later");
        //        return;
        //    }
        //}

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        private static extern bool SetSystemTime(ref SYSTEMTIME t);

        [DllImport("kernel32", CharSet = CharSet.Ansi)]
        private static extern bool SetLocalTime(ref SYSTEMTIME t);
        private void BtnOption_Click(object sender, EventArgs e)
        {            
            FrmEnterPassword frmEnterPassword = new FrmEnterPassword();           
            if (frmEnterPassword.ShowDialog()==DialogResult.OK)
            {                
                if(bkWorkerReportStatus.IsBusy)
                {
                    bkWorkerReportStatus.CancelAsync();
                    Thread.Sleep(100);
                }
                if(bkWorkerReceiveData.IsBusy)
                {
                    bkWorkerReceiveData.CancelAsync();                    
                    Thread.Sleep(100);
                }                
                #region 打开窗体操作                               
                FrmMain frmMain = new FrmMain(commPort, commPort_Supervisory);
                frmMain.Show();
                this.Hide();                
                #endregion
            }
        }

        private void FrmMeasureMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            while (bkWorkerReportStatus.IsBusy)
            {
                bkWorkerReportStatus.CancelAsync();
                Thread.Sleep(100);
            }
            while(bkWorkerReceiveData.IsBusy)
            {
                bkWorkerReceiveData.CancelAsync();
                Thread.Sleep(100);
            }
            bkWorkerReceiveData.Dispose();           
            bkWorkerReportStatus.Dispose();
            this.TmrDispTime.Enabled = false;
            while (this.commPort.Opened == true)
            {
                this.commPort.Close();
                Thread.Sleep(100);
            }
            while (this.commPort_Supervisory.Opened == true)
            {
                this.commPort_Supervisory.Close();
                Thread.Sleep(100);
            }
            this.Controls.Clear();
        }

        private void BtExit_Click(object sender, EventArgs e)
        {
            if (bkWorkerReportStatus.IsBusy)
            {
                bkWorkerReportStatus.CancelAsync();
                Thread.Sleep(200);
            }
            if (bkWorkerReceiveData.IsBusy)
            {
                bkWorkerReceiveData.CancelAsync();
                Thread.Sleep(200);
            }
            bkWorkerReceiveData.Dispose();
            bkWorkerReportStatus.Dispose();
            this.TmrDispTime.Enabled = false;
            if (this.commPort.Opened == true)
            {
                this.commPort.Close();
                Thread.Sleep(200);
            }

            if (this.commPort_Supervisory.Opened == true)
            {
                this.commPort_Supervisory.Close();
                Thread.Sleep(200);
            }
            Application.Exit();
        }

        private void BtnLanguage_Click(object sender, EventArgs e) 
        {
            systemParameter.IsEnglish = !systemParameter.IsEnglish;
            isEnglish = systemParameter.IsEnglish;
            frmClothes.isEnglish = !frmClothes.isEnglish;
            try
            {
                systemParameter.SetParameter(systemParameter);
                if (isEnglish == true)
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    this.Text = "Chinese";
                }
                else
                {
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                    this.Text = "英文";
                }
                Tools.ApplyLanguageResource(this);
                Tools.controls.Clear();
                //在界面中将启用通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，其它通道（通道未启用或探测面积为0）的控件Enabled无效；                       
                for (int i = 0; i < channelsAll.Count(); i++)
                {
                    if (channelsAll[i].IsEnabled == true && channelsAll[i].ProbeArea != 0)//通道被启用且探测面积不为0
                    {
                        //界面中相关通道控件Enabled设置为true，背景色设置为正常
                        ChannelDisplayControl(channelsAll[i], 1,1);//需要保持原来测量状态显示
                    }
                    else
                    {
                        //界面中相关通道控件Enabled属性设置为false，背景色设置为屏蔽
                        ChannelDisplayControl(channelsAll[i], 0,1);//需要保持原来测量状态显示
                    }
                }
                DisplayInit();
                //显示当前剩余时间
                LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s", "0") : string.Format("{0,3}s", stateTimeRemain.ToString());
                //根据当前运行状态设置顶部进度状态图片                                                                          
                if(stateCurrent==stateSelfCheck)
                {
                    //仪器自检状态标签设置为当前状态图片
                    PnlSelfCheck.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                                                                      //取消本底测量状态图片
                    PnlBackground.BackgroundImage = null;
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                }
                if(stateCurrent==stateBackgroundMeasure)
                {
                    PnlSelfCheck.BackgroundImage = null;
                    PnlBackground.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                }
                if(stateCurrent==stateReadyToMeasure)
                {
                    PnlSelfCheck.BackgroundImage = null;
                    PnlBackground.BackgroundImage = null;
                    PnlReady.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    PnlMeasuring.BackgroundImage = null;
                }
                if(stateCurrent==stateHandFootMeasuring)
                {
                    PnlSelfCheck.BackgroundImage = null;
                    PnlBackground.BackgroundImage = null;
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = Resources.progress;
                }
                if(stateCurrent==stateResult)
                {
                    PnlSelfCheck.BackgroundImage = null;
                    PnlBackground.BackgroundImage = null;
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                }                                 
            }
            catch
            {
                if (isEnglish == true)
                {
                    MessageBox.Show("Switch Fault，please try later");
                }
                else
                {
                    MessageBox.Show("切换失败，请稍后再试");
                }
                return;
            }
        }        
        private void FrmMeasureMain_VisibleChanged(object sender, EventArgs e)
        {               
            commPort.ClearPortData();
            //窗体第一次加载直接返回
            if(isFrmFirstShow)
            {
                isFrmDisplayed = !isFrmDisplayed;
                isFrmFirstShow = false;
                return;
            }
            if (isFrmDisplayed == false)
            {
                errNumber = 0;
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
                TxtShowResult.Text = "";                
                //获得工厂参数设置信息           
                factoryParameter.GetParameter();
                //从配置文件获得当前监测串口配置
                commPort.GetCommPortSet("commportSet");
                //从配置文件获得当前管理机串口通信配置
                commPort_Supervisory.GetCommPortSet("commportSetOfReport");
                //获得系统参数设置信息
                systemParameter.GetParameter();
                //Nuclide nuclide = new Nuclide();
                //clotheseNuclideUsed = nuclide.GetClothesNuclideUser(); //获得用户衣物探测核素选择
                //alphaNuclideUsed = nuclide.GetAlphaNuclideUser();//获得用户Alpha探测核素选择
                //betaNuclideUsed = nuclide.GetBetaNuclideUser();//获得用户Beta探测核素选择
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
                IList<Channel> list = channel.GetChannel();
                for (int i = 0; i < list.Count; i++)
                {
                    Tools.Clone(list[i], channelS[i]);
                }
                //实例化衣物探测界面
                frmClothes = new FrmClothes(isEnglish);
                //初始化衣物探测状态
                Nuclide nuclide = new Nuclide();
                //根据衣物探头类型（独立/共享）初始化衣物探测状态
                if (factoryParameter.IsFriskerIndependent == true)//独立衣物探头
                {
                    stateFriskerMeasuringIndependent = new StateFriskerMeasuringIndependent(frmClothes) { Name = "FriskerMeasuring" };
                    stateFrisker = stateFriskerMeasuringIndependent;
                    stateFriskerMeasuringIndependent.NuclideUsed = nuclide.GetClothesNuclideUser(); //获得用户衣物探测核素选择
                                                                                                    //绑定衣物测量数据显示事件响应(在衣物测量界面)
                    stateFriskerMeasuringIndependent.DisplayDataEvent += frmClothes.DisplayData;
                    //绑定衣物测量数据显示及衣物测量结果状态控制事件响应(在测量主界面)
                    stateFriskerMeasuringIndependent.ShowMsgEvent += ShowMsgFriskerMeasuring;
                }
                else
                {
                    stateFriskerMeasuringShared = new StateFriskerMeasuringShared(frmClothes) { Name = "FriskerMeasuring" };
                    stateFrisker = stateFriskerMeasuringShared;
                    //获得衣物Alpha和Beta核素选择
                    stateFriskerMeasuringShared.AlphaNuclideUsed = nuclide.GetAlphaNuclideUser();
                    stateFriskerMeasuringShared.BetaNuclideUsed = nuclide.GetBetaNuclideUser();
                    //绑定衣物测量数据显示事件响应(在衣物测量界面)
                    stateFriskerMeasuringShared.DisplayDataEvent += frmClothes.DisplayData;
                    //绑定衣物测量数据显示及衣物测量结果状态控制事件响应(在测量主界面)
                    stateFriskerMeasuringShared.ShowMsgEvent += ShowMsgFriskerMeasuring;
                }
                //初始化显示界面
                DisplayInit();                
                uIEventArgs.ChannelStatus.DoOnAdd = UIChannelStatus_Paint;
                uIEventArgs.ChannelStatus.DoOnSetKeyValue = UIChannelStatus_Paint;
                uIEventArgs.MeasureStatus.DoOnAdd = UIMeasureStatus_Paint;
                uIEventArgs.MeasureStatus.DoOnSetKeyValue = UIMeasureStatus_Paint;
                //初始化设备状态为正常
                uIEventArgs.DeviceStatus = DeviceStatus.OperatingNormally;
                //初始化UI界面各通道状态字典：未启用/红外未到位            
                foreach (Channel cn in channelS)
                {
                    if (cn.IsEnabled == false || cn.ProbeArea == 0)//通道未启用或检测面积为0
                    {
                        if (!uIEventArgs.ChannelStatus.ContainsKey(cn)) //不包含当前通道
                        {
                            uIEventArgs.ChannelStatus.Add(cn, ChannelStatus.Disabled);//会触发UIChannelStatus_Paint刷新界面                        
                        }
                        else
                        {
                            uIEventArgs.ChannelStatus[cn] = ChannelStatus.Disabled;//已包含当前通道，则设置为未启用；会触发UIChannelStatus_Paint刷新界面
                        }
                        //使用通道列表中如果包含该通道，则移除
                        if (usedChannelS.Contains(cn))
                        {
                            usedChannelS.Remove(cn);
                        }
                    }
                    else //通道启用且检测面积不为0
                    {
                        if (!uIEventArgs.ChannelStatus.ContainsKey(cn))//不含吧当前通道
                        {
                            uIEventArgs.ChannelStatus.Add(cn, ChannelStatus.NotInPlace);//会触发UIChannelStatus_Paint刷新界面
                        }
                        else
                        {
                            uIEventArgs.ChannelStatus[cn] = ChannelStatus.NotInPlace;//已包含当前通道，则设置为未到位；会触发UIChannelStatus_Paint刷新界面
                        }
                        //使用通道列表中如果不包含该通道则添加
                        if (!usedChannelS.Contains(cn))
                        {
                            usedChannelS.Add(cn);
                        }
                    }
                    //初始化UI界面各通道设备状态字典：设备正常
                    if (uIEventArgs.MeasureStatus.ContainsKey(cn))
                    {
                        uIEventArgs.MeasureStatus[cn] = DeviceStatus.OperatingNormally;
                    }
                    else
                    {
                        uIEventArgs.MeasureStatus.Add(cn, DeviceStatus.OperatingNormally);
                    }
                }

                //LblTimeRemain.Text = string.Format("{0}s", systemParameter.SelfCheckTime.ToString());
                //在界面中将启用通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，其它通道（通道未启用或探测面积为0）的控件Enabled无效；                       
                //for (int i = 0; i < channelS.Count; i++)
                //{
                //    if (channelS[i].IsEnabled == true && channelS[i].ProbeArea != 0)//通道被启用且探测面积不为0
                //    {
                //        //界面中相关通道控件Enabled设置为true，背景色设置为正常
                //        ChannelDisplayControl(channelS[i], 1,0);//所有通道状态全部初始化
                //    }
                //    else
                //    {
                //        //界面中相关通道控件Enabled属性设置为false，背景色设置为屏蔽
                //        ChannelDisplayControl(channelS[i], 0,0);//所有通道状态全部初始化
                //    }
                //}
                //获得全部启用通道
                //channelS = channel.GetChannel(true);
                ////将检测面积为0的通道剔除
                //for (int i = 0; i < channelS.Count; i++)
                //{
                //    if (channelS[i].ProbeArea == 0)
                //    {
                //        channelS.RemoveAt(i);
                //        i--;
                //    }
                //}
                //calculatedMeasureDataS.Clear();
                //根据有效通道对象初始化用来存储最终监测数据的列表
                //foreach (Channel usedChannel in channelS)
                //{
                //    MeasureData measureData = new MeasureData();
                //    measureData.Channel = usedChannel;
                //    calculatedMeasureDataS.Add(measureData);
                //}
                //初始化左手、右手和衣物的红外状态，默认都不到位
                //for (int i = 0; i < 3; i++)
                //{
                //    lastInfraredStatus[i] = 0;
                //}
                //正常使用的通道测量数值显示（初始值为0cps）
                //DisplayMeasureData(calculatedMeasureDataS, "cps");

                isSelfCheckSended = false;
                isBetaCommandToSend = false;
                //将运行状态标志设置为“运行准备”
                stateCurrent = stateReadyToRun;
                //当前系统状态时间为自检时间
                //stateCurrent.HoldTimes = systemParameter.SelfCheckTime;
                //stateCurrent.LastTime = DateTime.Now;
                //stateTimeStart = DateTime.Now.AddSeconds(1);
                //stateTimeRemain_Last = stateTimeSet;
                //isPlatformStateSwitched = true;//置状态切换标志
                if (this.bkWorkerReceiveData.IsBusy == false)
                {
                    this.bkWorkerReceiveData.RunWorkerAsync();
                }
                if (commPort_Supervisory.IsEnabled == true && this.bkWorkerReportStatus.IsBusy == false)
                {
                    this.bkWorkerReportStatus.RunWorkerAsync();
                }                
                deviceStatus =Convert.ToByte(DeviceStatus.OperatingNormally);
                //恢复仪器自检状态背景
                //PnlSelfCheck.BackColor = Color.Transparent;                
                //if (isLoadProgressPic[0] == false||LblSelfCheck.Text=="仪器故障")
                //{
                //    //SetProgressPicFlag(0);//仪器自检进度图片已经被加载标志设置为true，其它为false
                //    //恢复当前标签文字提示（自检过程中可能有故障提示，所以自检通过后需要重新恢复）
                //    if (isEnglish == false)
                //    {
                //        LblSelfCheck.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                //        LblSelfCheck.Text = "仪器自检";
                //    }
                //    else
                //    {
                //        LblSelfCheck.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                //        LblSelfCheck.Text = "Self-Checking";
                //    }
                //    //仪器自检状态标签设置为进度图片
                //    PnlSelfCheck.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                //    //取消本底测量状态标签设置
                //    PnlBackground.BackgroundImage = null;
                //    //取消等待测量状态标签设置
                //    PnlReady.BackgroundImage = null;
                //    PnlMeasuring.BackgroundImage = null;
                //    PnlNoContamination.BackgroundImage = null;
                //    PnlContaminated.BackgroundImage = null;                    
                //    LblTimeRemain.Parent = PnlSelfCheck;//控制剩余时间标签显示位置                                
                //    LblBackground.BringToFront();
                //    LblTimeRemain.BringToFront();
                //    LblTimeRemain.BackColor = Color.Transparent;                    
                //}
                //stateTimeStart = DateTime.Now;//重新启动计时
                //序列化上报设备数据
                if (factoryParameter.IsConnectedAuto == true)
                {
                    if (Serialize(factoryParameter.IpAddress, Convert.ToInt32(factoryParameter.PortNumber)) == false)
                    {
                        TxtShowResult.Text += "序列化上报状态失败!\r\n";
                    }
                }                
            }
            isFrmDisplayed = !isFrmDisplayed;
        }

        private void TxtShowResult_TextChanged(object sender, EventArgs e)
        {
            if (TxtShowResult.GetLineFromCharIndex(TxtShowResult.TextLength) > 16)
            {
                int start = TxtShowResult.GetFirstCharIndexFromLine(0);
                int end = TxtShowResult.GetFirstCharIndexFromLine(TxtShowResult.GetLineFromCharIndex(TxtShowResult.TextLength) - 16);
                TxtShowResult.Select(start, end);
                TxtShowResult.SelectedText = "";
            }
            TxtShowResult.SelectionStart = TxtShowResult.Text.Length;
            TxtShowResult.ScrollToCaret();
        }
        private bool Serialize(string ipAddress, int port)
        {
            Serializable serializable = new Serializable();
            //仪器编号
            serializable.DeviceNumber =factoryParameter.InstrumentNum;
            //仪器状态
            serializable.Status = "正常";
            //仪器测试标志
            serializable.Test_Flag = "--";
            //参数更新标志
            serializable.Update_Flag = "--";
            //故障信息
            serializable.Fault = "--";
            //报警信息
            serializable.Alarm = "--";
            //测量单位
            serializable.TestUnit = systemParameter.MeasurementUnit;
            //测量时间
            serializable.TestTime = systemParameter.MeasuringTime;
            //当前使用核素
            serializable.NuclideUsed = new NuclideUsed(stateHandFootMeasuring.AlphaNuclideUsed,stateHandFootMeasuring.BetaNuclideUsed, stateFriskerMeasuringIndependent.NuclideUsed);
            //高压
            serializable.HighVoltage = new HighVoltage(channelParameterS[0].PresetHV.ToString(), channelParameterS[1].PresetHV.ToString(), channelParameterS[2].PresetHV.ToString(), channelParameterS[3].PresetHV.ToString(), channelParameterS[4].PresetHV.ToString(), channelParameterS[5].PresetHV.ToString(), channelParameterS[6].PresetHV.ToString());
            //Alpha本底数据
            serializable.AlphaBackground = new AlphaBackground();
            //Beta本底数据
            serializable.BetaBackground = new BetaBackground();
            //Alpha测量数据
            serializable.AlphaTestData = new AlphaTestData();
            //Beta测量数据
            serializable.BetaTestData = new BetaTestData();
            //获得当前Alpha探测参数
            List<ProbeParameter> probeParmeterAlphaS= probeParameterS.Where(probeParmeter => probeParmeter.NuclideType == "α").ToList();
            //Alpha本底上限
            serializable.AlphaBackgroundUp = new AlphaBackgroundUp(probeParmeterAlphaS[0].HBackground.ToString(),probeParmeterAlphaS[1].HBackground.ToString(),probeParmeterAlphaS[2].HBackground.ToString(),probeParmeterAlphaS[3].HBackground.ToString(),probeParmeterAlphaS[4].HBackground.ToString(),probeParmeterAlphaS[5].HBackground.ToString());
            //Alpha本底下限
            serializable.AlphaBackgroundDown=new AlphaBackgroundDown(probeParmeterAlphaS[0].LBackground.ToString(), probeParmeterAlphaS[1].LBackground.ToString(), probeParmeterAlphaS[2].LBackground.ToString(), probeParmeterAlphaS[3].LBackground.ToString(), probeParmeterAlphaS[4].LBackground.ToString(), probeParmeterAlphaS[5].LBackground.ToString());
            //Alpha一级报警阈值
            serializable.AlphaAlarmThread = new AlphaAlarmThread(probeParmeterAlphaS[0].Alarm_1.ToString(), probeParmeterAlphaS[1].Alarm_1.ToString(), probeParmeterAlphaS[2].Alarm_1.ToString(), probeParmeterAlphaS[3].Alarm_1.ToString(), probeParmeterAlphaS[4].Alarm_1.ToString(), probeParmeterAlphaS[5].Alarm_1.ToString());
            //Alpha二级报警阈值
            serializable.AlphaHighAlarmThread = new AlphaHighAlarmThread(probeParmeterAlphaS[0].Alarm_2.ToString(), probeParmeterAlphaS[1].Alarm_2.ToString(), probeParmeterAlphaS[2].Alarm_2.ToString(), probeParmeterAlphaS[3].Alarm_2.ToString(), probeParmeterAlphaS[4].Alarm_2.ToString(), probeParmeterAlphaS[5].Alarm_2.ToString());
            //Alpha探测效率
            serializable.AlphaEfficiency = new AlphaEfficiency(probeParmeterAlphaS[0].Efficiency.ToString(), probeParmeterAlphaS[1].Efficiency.ToString(), probeParmeterAlphaS[2].Efficiency.ToString(), probeParmeterAlphaS[3].Efficiency.ToString(), probeParmeterAlphaS[4].Efficiency.ToString(), probeParmeterAlphaS[5].Efficiency.ToString());
            //获得当前Beta探测参数
            List<ProbeParameter> probeParmeterBetaS = probeParameterS.Where(probeParmeter => probeParmeter.NuclideType == "β").ToList();
            //Beta本底上限
            serializable.BetaBackgroundUp = new BetaBackgroundUp(probeParmeterBetaS[0].HBackground.ToString(), probeParmeterBetaS[1].HBackground.ToString(), probeParmeterBetaS[2].HBackground.ToString(), probeParmeterBetaS[3].HBackground.ToString(), probeParmeterBetaS[4].HBackground.ToString(), probeParmeterBetaS[5].HBackground.ToString());
            //Beta本底下限
            serializable.BetaBackgroundDown = new BetaBackgroundDown(probeParmeterBetaS[0].LBackground.ToString(), probeParmeterBetaS[1].LBackground.ToString(), probeParmeterBetaS[2].LBackground.ToString(), probeParmeterBetaS[3].LBackground.ToString(), probeParmeterBetaS[4].LBackground.ToString(), probeParmeterBetaS[5].LBackground.ToString());
            //Beta一级报警阈值
            serializable.BetaAlarmThread = new BetaAlarmThread(probeParmeterAlphaS[0].Alarm_1.ToString(), probeParmeterAlphaS[1].Alarm_1.ToString(), probeParmeterAlphaS[2].Alarm_1.ToString(), probeParmeterAlphaS[3].Alarm_1.ToString(), probeParmeterAlphaS[4].Alarm_1.ToString(), probeParmeterAlphaS[5].Alarm_1.ToString());
            //Beta二级报警阈值
            serializable.BetaHighAlarmThread = new BetaHighAlarmThread(probeParmeterAlphaS[0].Alarm_2.ToString(), probeParmeterAlphaS[1].Alarm_2.ToString(), probeParmeterAlphaS[2].Alarm_2.ToString(), probeParmeterAlphaS[3].Alarm_2.ToString(), probeParmeterAlphaS[4].Alarm_2.ToString(), probeParmeterAlphaS[5].Alarm_2.ToString());
            //Beta探测效率
            serializable.BetaEfficiency = new BetaEfficiency(probeParmeterAlphaS[0].Efficiency.ToString(), probeParmeterAlphaS[1].Efficiency.ToString(), probeParmeterAlphaS[2].Efficiency.ToString(), probeParmeterAlphaS[3].Efficiency.ToString(), probeParmeterAlphaS[4].Efficiency.ToString(), probeParmeterAlphaS[5].Efficiency.ToString());
            //获得当前衣物探测参数
            List<ProbeParameter> probeParmeterClothesS = probeParameterS.Where(probeParmeter => probeParmeter.NuclideType == "C").ToList();
            //衣物本底上限
            serializable.ClothesBackgroundUP = probeParmeterClothesS[0].HBackground.ToString();
            //衣物本底下限
            serializable.ClothesBackgroundDown = probeParmeterClothesS[0].LBackground.ToString();
            //衣物一级报警阈值
            serializable.ClothesAlarmThread = probeParmeterClothesS[0].Alarm_1.ToString();
            //衣物二级报警阈值
            serializable.ClothesHighAlarmThread= probeParmeterClothesS[0].Alarm_2.ToString();
            //衣物探测效率
            serializable.ClothesEfficiency = probeParmeterClothesS[0].Efficiency.ToString();
            //Alpha阈值
            serializable.AlphaThread = new AlphaThread(channelParameterS[0].AlphaThreshold.ToString(), channelParameterS[1].AlphaThreshold.ToString(), channelParameterS[2].AlphaThreshold.ToString(), channelParameterS[3].AlphaThreshold.ToString(), channelParameterS[4].AlphaThreshold.ToString(), channelParameterS[5].AlphaThreshold.ToString(), channelParameterS[6].AlphaThreshold.ToString());
            //Beta阈值
            serializable.BetaThread = new BetaThread(channelParameterS[0].BetaThreshold.ToString(), channelParameterS[1].BetaThreshold.ToString(), channelParameterS[2].BetaThreshold.ToString(), channelParameterS[3].BetaThreshold.ToString(), channelParameterS[4].BetaThreshold.ToString(), channelParameterS[5].BetaThreshold.ToString(), channelParameterS[6].BetaThreshold.ToString());
            //Alpha报警标志
            serializable.AlphaAlarmFlag = new AlphaAlarmFlag();
            //Beta报警标志
            serializable.BetaAlarmFlag = new BetaAlarmFlag();
            //Alpha故障标志
            serializable.AlphaFailedFlag = new AlphaFailedFlag();
            //Beta故障标志
            serializable.BetaFailedFlag = new BetaFailedFlag();
            string postXml = SerializeHelper.Serialize(typeof(Serializable), serializable);
            //将序列化后的结果转换为字节码
            Byte[] byteDateLine = Encoding.ASCII.GetBytes(postXml.ToCharArray());
            //声明发送对象
            //MessageAgreement message = new MessageAgreement(1, 2, byteDateLine);
            //创建Socket连接
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(ipAddress);
                socket.Connect(address, port);
                //发送数据
                //byte[] sendBytes = message.ToBytes();
                socket.Send(byteDateLine, byteDateLine.Length, 0);
                socket.Close();
                return true;
            }
            catch
            {
                if (socket != null)
                {
                    if (socket.Connected == true)
                    {
                        socket.Close();
                    }
                    socket.Dispose();
                }
                return false;
            }
        }
        /// <summary>
        /// 根据语言要求设定检测过程文字和语音提示信息
        /// 根据语言要求设定UI事件参数类错误信息记录
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="eventAtgs">UI事件参数类</param>
        /// <returns>检测过程文字和语音提示信息类</returns>
        private MessageShow SetMsgLanguage(Language language, UIEventArgs eventArgs)
        {
            MessageShow message;
            if(language==Language.English)
            {
                message = new MessageEnglish();
            }
            else
            {
                message = new MessageChinese();
            }
            if (eventArgs.CurrentState.ErrRecord != null && eventArgs.CurrentState.ErrRecord.Count > 0)
            {
                //ErrRecord属性在自检、本底测量阶段存储故障信息，在测量阶段存储污染信息
                message.TxtErrorRecordOfChannel = eventArgs.CurrentState.ErrRecord[language];
                message.TxtPollutionRecerd = eventArgs.CurrentState.ErrRecord[language];
            }
            return message;
        }
        /// <summary>
        /// 仪器自检过程状态文字和语音提示事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventAtgs"></param>
        private void ShowMsgSelfCheck(object sender,UIEventArgs eventArgs)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            MessageShow message;
            if(isEnglish)
            {
                message = SetMsgLanguage(Language.English, eventArgs);
            }
            else
            {
                message = SetMsgLanguage(Language.Chinese, eventArgs);
            }
            if (eventArgs.DeviceStatus == DeviceStatus.OperatingNormally)
            {
                //仪器自检状态标签设置为当前状态图片
                PnlSelfCheck.BackgroundImage = Resources.progress;
                //取消其它状态进度图片设置
                PnlBackground.BackgroundImage = null;
                PnlReady.BackgroundImage = null;
                PnlMeasuring.BackgroundImage = null;
                //控制剩余时间显示标签位置
                LblTimeRemain.Parent = PnlSelfCheck;
                LblTimeRemain.Location = new Point(84, 0);//控制剩余时间标签显示位置
                LblTimeRemain.BringToFront();
                //监测状态文本框显示“仪器自检”
                TxtShowResult.Text += message.TxtSelfCheck;
                //状态信息语音播报
                player.Stream = message.StreamSelfCheck;
                player.Play();
                return;
            }
            if (eventArgs.DeviceStatus == DeviceStatus.OperatingFaulted)
            {
                //仪器自检状态背景色设置为故障
                PnlSelfCheck.BackgroundImage = Resources.Fault_progress;
                //监测状态文本框显示故障信息
                TxtShowResult.Text += message.TxtErrorRecordOfChannel;
                //语音提示故障
                player.Stream = message.StreamSelfCheckFault;
                player.Play();
                return;
            }
        }
        /// <summary>
        /// 本底测量过程状态文字和语音提示事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventAtgs"></param>
        private void ShowMsgBackGround(object sender, UIEventArgs eventArgs)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            MessageShow message;
            if (isEnglish)
            {
                message = SetMsgLanguage(Language.English, eventArgs);
            }
            else
            {
                message = SetMsgLanguage(Language.Chinese, eventArgs);
            }
            if (stateCurrent.GetType() == typeof(StateBackgroundMeasure))
            {
                foreach (MeasureData data in eventArgs.CurrentState.CalculatedMeasureDataS)
                {
                    if (data.InfraredStatus == 1)
                    {
                        //顶部检测进度标签提示测量中断
                        LblBackground.Font = message.FontSet;
                        LblBackground.Text = message.TxtBackGroundInterrupted;
                        //测量结果显示区域文本框显示到位,同时进行语音提示
                        switch (data.Channel.ChannelID)
                        {
                            case 1:
                            case 2:
                                //左手到位                                
                                TxtShowResult.Text += message.TxtLeftHandInPlace;
                                player.Stream = message.StreamLeftHandInPlace;
                                break;
                            case 3:
                            case 4:
                                //右手到位                                
                                TxtShowResult.Text += message.TxtRightHandInPlace;
                                player.Stream = message.StreamRightHandInPlace;
                                break;
                            case 7:
                                //衣物到位
                                //未进行语音提示                                
                                TxtShowResult.Text += message.TxtFriskerInPlace;
                                player.Stream = message.StreamFriskerInPlace;
                                break;
                        }
                        //播放语音提示
                        player.Play();
                        //中断本次显示
                        return;
                    }
                }
            }
            if (eventArgs.DeviceStatus == DeviceStatus.OperatingNormally)
            {
                //本底测量状态标签设置为当前状态图片
                PnlBackground.BackgroundImage = Resources.progress;
                LblBackground.BringToFront();
                //取消其它状态进度图片设置
                PnlSelfCheck.BackgroundImage = null;
                PnlReady.BackgroundImage = null;
                PnlMeasuring.BackgroundImage = null;
                PnlNoContamination.BackgroundImage = null;
                PnlContaminated.BackgroundImage = null;
                //控制剩余时间标签显示位置
                LblTimeRemain.Parent = PnlBackground;
                LblTimeRemain.BringToFront();
                LblTimeRemain.BackColor = Color.Transparent;
                //取消测量值显示状态背景颜色（测量人员污染时，可能会有报警颜色）

                //监测状态文本框显示“本底测量”
                TxtShowResult.Text += message.TxtBackGroundMeasure;
                //状态信息语音播报
                player.Stream = message.StreamBackGroundMeasure;
                player.Play();
                return;
            }
            if (eventArgs.CurrentState.DeviceStatus == DeviceStatus.OperatingFaulted)
            {
                //仪器本底状态背景色设置为故障
                PnlBackground.BackgroundImage = Resources.Fault_progress;
                //监测状态文本框显示故障信息
                TxtShowResult.Text += message.TxtErrorRecordOfChannel;
                //语音提示故障
                player.Stream = message.StreamBackGroundError;
                player.Play();
            }
        }
        /// <summary>
        /// 等待测量过程状态文字和语音提示事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventAtgs"></param>
        private void ShowMsgReady(object sender, UIEventArgs eventArgs)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            MessageShow message;
            if (isEnglish)
            {
                message = SetMsgLanguage(Language.English, eventArgs);
            }
            else
            {
                message = SetMsgLanguage(Language.Chinese, eventArgs);
            }
            switch (eventArgs.CurrentState.SoundMessage)
            {
                case SoundMessage.FlipPalm:
                    //翻转手掌进行检测
                    player.Stream = message.StreamFlipPalm;
                    break;
                case SoundMessage.FriskerMeasure:
                    //请进行衣物测量
                    player.Stream = message.StreamFriskerMeasure;
                    TxtShowResult.Text += message.TxtFriskerMeasure;
                    break;
                case SoundMessage.Leave:
                    //没有污染，请离开
                    player.Stream = message.StreamNoContamination;
                    TxtShowResult.Text += message.TxtNoContamination;
                    break;
                case SoundMessage.Ready:
                    //等待测量
                    player.Stream = message.StreamReadyToMeasure;
                    break;                                         
            }
            if (eventArgs.CurrentState.SoundMessage != SoundMessage.Mute)//不是初始屏蔽语音播报状态（第一次进入等待测量节点不播放上述语音信息）
            {
                player.Play();                
            }
            //等待测量状态标签设置为当前状态图片
            PnlReady.BackgroundImage = Resources.progress;
            LblReady.BringToFront();
            //取消其它状态进度图片设置
            PnlBackground.BackgroundImage = null;
            PnlMeasuring.BackgroundImage = null;
            PnlNoContamination.BackgroundImage = null;
            PnlContaminated.BackgroundImage = null;
            //控制剩余时间标签显示位置
            LblTimeRemain.Parent = PnlReady;
            LblTimeRemain.BringToFront();
            if(eventArgs.CurrentState.SoundMessage == SoundMessage.Ready)
            {
                //监测状态文本框显示“等待测量”
                TxtShowResult.Text += message.TxtReadyToMeasure;
            }            
            return;
        }
        /// <summary>
        /// 手脚开始测量过程状态文字和语音提示事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventAtgs"></param>
        private void ShowMsgHandFootMeasuring(object sender, UIEventArgs eventArgs)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            MessageShow message;
            if (isEnglish)
            {
                message = SetMsgLanguage(Language.English, eventArgs);
            }
            else
            {
                message = SetMsgLanguage(Language.Chinese, eventArgs);
            }
            if(eventArgs.CurrentState.HandProbeCtrl==HandProbeCtrl.StepTwoComplete)
            {
                if(eventArgs.DeviceStatus==DeviceStatus.OperatingNormally)
                {                    
                    //仪器无污染状态背景色设置为无污染
                    PnlNoContamination.BackgroundImage = Resources.NoContamination_progress;
                    //取消开始测量状态进度显示
                    PnlMeasuring.BackColor = Color.Transparent;
                    PnlMeasuring.BackgroundImage = null;                   
                }
                else
                {
                    //显示测量结果信息
                    TxtShowResult.Text += message.TxtDecontamination;
                    //设置人员污染状态显示区域背景色
                    PnlContaminated.BackgroundImage = Resources.Contaminated_progress;
                    //取消开始测量状态进度显示
                    PnlMeasuring.BackColor = Color.Transparent;
                    PnlMeasuring.BackgroundImage = null;                    
                }
                return;
            }
            //根据红外状态提示手部移动
            foreach (MeasureData data in eventArgs.CurrentState.CalculatedMeasureDataS)
            {
                //红外不到位，提示后返回
                if (data.InfraredStatus == 0)
                {
                    //测量结果显示区域文本框显示手部移动重新测量,同时进行语音提示
                    switch (data.Channel.ChannelID)
                    {
                        case 1:
                        case 2:
                            //左手移动                                
                            TxtShowResult.Text += message.TxtLeftHandMoved;
                            player.Stream = message.StreamLeftHandMoved;
                            break;
                        case 3:
                        case 4:
                            //右手移动                                
                            TxtShowResult.Text += message.TxtRightHandMoved;
                            player.Stream = message.StreamRightHandMoved;
                            break;
                            //左脚移动
                        case 5:
                            TxtShowResult.Text += message.TxtLeftFootMoved;
                            player.Stream = message.StreamLeftFootMoved;
                            break;
                            //右脚移动
                        case 6:
                            TxtShowResult.Text += message.TxtRightFootMoved;
                            player.Stream = message.StreamRightFootMoved;
                            break;
                        default:
                            continue;
                    }
                    //播放语音提示
                    player.Play();
                    //中断本次显示
                    return;
                }
            }
            //测试过程和开始测量语音提示
            switch (eventArgs.CurrentState.SoundMessage)
            {
                case SoundMessage.DiDa_1:
                    player.Stream = message.StreamDiDa_1;
                    player.Play();
                    return;
                case SoundMessage.DiDa_2:
                    player.Stream = message.StreamDiDa_2;
                    player.PlaySync();
                    return;
                case SoundMessage.Measuring:
                    //状态信息语音播报
                    player.Stream = message.StreamMeasuring;
                    player.PlaySync();
                    break;
            }
            //开始测量状态标签设置为当前状态图片
            PnlMeasuring.BackgroundImage = Resources.progress;
            //取消等待测量状态进度图片设置
            PnlReady.BackgroundImage = null;
            //控制剩余时间标签显示位置
            LblTimeRemain.Parent = PnlMeasuring;
            LblTimeRemain.BringToFront();
            //监测状态文本框显示“开始测量”
            TxtShowResult.Text += message.TxtMeasuring;                                   
            return;
        }
        /// <summary>
        /// 衣物测量过程状态文字和语音提示事件响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ShowMsgFriskerMeasuring(object sender, FriskerEventArgs eventArgs)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            MessageShow message;

            if (isEnglish)
            {
                message = new MessageEnglish();
            }
            else
            {
                message = new MessageChinese();
            }
            if(stateFrisker.IsTested==true)
            {
                //语音提示dida1.wav
                player.Stream = Resources.dida1;
                player.Play();                
            }            
            //在衣物检测结果显示区域显示当前测量值，根据数据大小控制显示字体大小
            if (eventArgs.MeasureValue["C"] < 10)
            {
                LblFriskerB.Font = new Font("宋体", 12, FontStyle.Bold);
                
            }
            else if (eventArgs.MeasureValue["C"] < 1000)
            {
                LblFriskerB.Font = new Font("宋体", 12, FontStyle.Bold);                
            }
            else
            {
                LblFriskerB.Font = new Font("宋体", 10, FontStyle.Bold);                
            }
            if (factoryParameter.IsFriskerIndependent)
            {
                LblFriskerB.Text = string.Format("{0}cps", eventArgs.MeasureValue["C"].ToString("F2"));
            }
            else
            {
                switch(factoryParameter.MeasureType)
                {
                    case "α":
                        LblFriskerB.Text = string.Format("α:{0}cps", eventArgs.MeasureValue["α"].ToString("F2"));
                        break;
                    case "β":
                        LblFriskerB.Text = string.Format("β:{0}cps", eventArgs.MeasureValue["β"].ToString("F2"));
                        break;
                    case "α/β":
                        LblFriskerB.Text = string.Format("α:{0}cps;β:{1}cps", eventArgs.MeasureValue["α"].ToString("F2"), eventArgs.MeasureValue["β"].ToString("F2"));
                        break;
                }
            }
            switch (eventArgs.DeviceStatus)
            {
                case DeviceStatus.OperatingNormally:                    
                    break;
                case DeviceStatus.OperatingContaminated_1:                    
                case DeviceStatus.OperatingContaminated_2:
                    //仪器人员污染状态背景色设置为污染
                    PnlContaminated.BackgroundImage = Resources.Contaminated_progress;
                    //衣物探测结果显示区域背景色设置为污染
                    LblFriskerB.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;                    
                    //在测量结果显示文本框显示衣物污染相关信息（设定值和测量值）
                    message.TxtPreset = eventArgs.PreSet.ToString();
                    message.TxtActual = eventArgs.MeasureValue.ToString();
                    TxtShowResult.Text += message.TxtFriskerContamination;
                    //播放报警语音
                    player.Stream = Resources.dida3;//报警音提示                    
                    player.Play();
                    break;                    
            }
        }
        private void ShowMsgResult(object sender,UIEventArgs eventArgs)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            MessageShow message;
            if (isEnglish)
            {
                message = SetMsgLanguage(Language.English, eventArgs);
            }
            else
            {
                message = SetMsgLanguage(Language.Chinese, eventArgs);
            }
            //本次测量有污染，进行语音提示，同时测量结果区域显示
            if(eventArgs.DeviceStatus==DeviceStatus.OperatingContaminated_1||eventArgs.DeviceStatus==DeviceStatus.OperatingContaminated_2)
            {                
                if (uIEventArgs.isAudioMan)
                {
                    player.Stream = message.StreamDecontamination_Man;
                }
                else
                {
                    player.Stream = message.StreamDecontamination;
                }
                player.Play();                
            }
        }        
    }    
}

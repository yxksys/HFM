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

namespace HFM
{
    public partial class FrmMeasureMain : Form
    {
        //int measureCount = 0;
        Panel[] PnlStatus;
        Label[] LblTitle;
        Label[] LblValue;
        PictureBox[] PicStatus;
        Label[] LblStatus;
       // string listS = "";
        //string timetemp = "";
        byte infraredStatusOfMessageLast = 0x1b;//上次报文信息中红外状态，默认手和衣物都不到位。格式：5-7通道红外状态-1-4通道红外状态。占低6位
        byte infraredStatusOfMessageNow = 0x1b;//本次报文信息中红外状态，默认手和衣物都不到位。格式：5-7通道红外状态-1-4通道红外状态。占低6位
        const int BASE_DATA = 1000;//标准本底值
        const int FONT_SIZE_E = 12;//检测状态显示区域英文字体大小
        const int FONT_SIZE = 14;//检测状态显示区域中午字体大小
        //const int FONT_SIZE_RESULT =18 ;//检测结果显示字体
        const int TEAM_LENGTH = 240;//
        //泊松参数
        const double POISSONUA_2 = 1.658;
        const double POISSONUA = 2.5758;
        const double POISSONUA2_4 = 0.676;
        const int LONGTIME = 60;
        string appPath = null;//应用系统安装路径
        int messageBufferLength = 0;//串口接收数据缓冲区大小
        byte[] receiveBufferMessage = null; //存储接收报文信息缓冲区
        System.Timers.Timer TmrDispTime = null;//显示系统时间、控制控件状态颜色异步Timer对象
        IList<MeasureData> measureDataS = null;//检测数据接收列表缓冲区
        bool isEnglish = false;//当前语言，默认中文        
        int checkTime = 0;//监测时间       
        int alarmTimeSet = 0; //系统报警时间长度设置        
        DateTime alarmTimeStart = DateTime.Now;//系统报警时间计时
        int stateTimeSet = 0;//系统当前运行状态的检测时间设置
        int stateTimeRemain = 0;//系统当前运行状态剩余时间
        int errNumber = 0; //报文接收出现错误计数器   
        int throwDataCount = 0;//准备检测阶段预读取数据扔掉次数
        int clothesTimeCount = 0;//衣物离线时间计数器，每秒计数一次     
        int isHandTested = 0;//0：手部第一次检测还未开始；1：手部第一次检测完成；2：手部第二次检测完成。由于单探测器时，手心手背必须分两次进行检测，手部第一次检测完成后，根据该标志进行语音提示，然后进该标志置为1，第二次检测完成后置2。
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
        bool isSelfCheckSended = false;//自检指令是否已经下发标志，因为在一个自检周期内，自检指令只需下发一次
        bool isBetaCommandToSend = false;//Beta自检指令是否应该下发，在α/β自检时，先下发α自检指令，自检时间到一半时再下发β自检指令
        bool isFirstBackGround = true;//进入等待测量状态后的本底测量计时标志
        bool isFirstBackGroundData = true;//串口回传的第一个本底数据
        bool[] isLoadProgressPic =new bool[6] { false,false,false,false,false,false};//窗口顶部本底测量检测进度状态图片是否已经被加载                
        bool isHandSecondEnabled = false;//是否允许启动手部翻转后测量，当手部第一次测量结束后，用户必须翻转手掌（红外出现至少一次不到位），才能启动第二次手部检测
        bool isClothesContaminated=false;//衣物探测是否有污染
        bool isAudioPlayed = false;//语音播报是否结束标志
        bool isTestedEnd = false;//监测是否结束标志
        bool isFrmDisplayed = false;
        bool isCommError = false;//监测端口通信错误标志
        bool isCommReportError = false;//上报端口通信错误标志
        string pollutionRecord = null;//记录测量污染详细数据
        string pollutionRecord_E = null;//记录测量污染详细数据(英文)                
        FrmClothes frmClothes = null;//衣物探测界面
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
            OperatingContaminated_1=63,
            OperatingContaminated_2=64
        }
        byte deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);//设备当前状态        
        DateTime stateTimeStart;//系统当前运行状态的开始计时变量                
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();//创建音频播放对象
        CommPort commPort = new CommPort();//监测端口
        CommPort commPort_Supervisory = new CommPort();//和管理机通信端口
        Thread IOThread = null;
        float smoothedDataOfClothes = 0;//平滑处理后的衣物测量值
        float baseDataOfClothes = 0;//衣物探头本底值
        int alarmCountOfClothes = 0;//衣物检测报警次数
        int playControl = 1;//控制语音播放变量
        string clotheseNuclideUsed = "U_235";//衣物检测核素选择,默认U_235
        string alphaNuclideUsed = "U_235";//Alpha核素选择，默认U_235
        string betaNuclideUsed = "U_235";//Beta核素选择，默认U_235          
        FactoryParameter factoryParameter = new FactoryParameter();//工厂参数        
        Components.SystemParameter systemParameter = new Components.SystemParameter();//系统参数
        Channel[] channelsAll=new Channel[7];//全部通道
        IList<Channel> channelS = new List<Channel>();//当前可使用的检测通道,即全部启用的监测通道
        IList<MeasureData> baseData = new List<MeasureData>(); //存储本底计算结果，用例对测量数据进行校正
        IList<MeasureData> calculatedMeasureDataS = new List<MeasureData>();//存储各个通道最终计算检测值的列表
        IList<EfficiencyParameter> efficiencyParameterS = new List<EfficiencyParameter>();//存储探测效率参数列表
        IList<ProbeParameter> probeParameterS = new List<ProbeParameter>();//存储探测参数的列表  
        IList<ChannelParameter> channelParameterS = new List<ChannelParameter>();//存储道盒参数列表    
        MeasureData conversionData = new MeasureData();
        //conversionData.Channel = new Channel();
        IList<MeasureData> conversionDataS = new List<MeasureData>();
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
                                            17,34,18,36,19,37,19,38,20,39 };//泊松表1
        float[] poissonTable = new float[50]{ 0, 4, 0, 6, 0, 8, 0, 10, 0, 11,
                                             1,13,1,14,2,16,2,17,3,18,
                                             4,20,4,21,5,22,6,24,6,25,
                                             7,26,8,28,8,29,9,30,10,31,
                                            11,33,11,34,12,35,13,36,14,38 };//泊松表2
        int[] cycleLength = new int[8] { 16, 25, 36, 49, 64, 81, 100, 121 };       
        //int handTestCount=0;//手部检测计数器。由于单探测器时，手心手背必须分两次进行检测，用来手心、手背检测计数。        
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
        }
        /// <summary>
        /// 初始化显示界面
        /// </summary>
        private void DisplayInit()
        {            
            //在界面中显示当前系统时间
            LblTime.Text = DateTime.Now.ToLongTimeString();
            //在界面中显示“仪器名称”、“仪器编号”、“IP地址及端口”等信息
            //Tools tools = new Tools();//实例化工具类，中英文切换需要
            if (isEnglish == true)
            {
                LblName.Text = Tools.EnSoftName(factoryParameter.SoftName);
            }
            else
            {
                LblName.Text = factoryParameter.SoftName;
            }
            //LblName.Text = factoryParameter.SoftName;
            LblIP.Text = factoryParameter.IpAddress + " " + factoryParameter.PortNumber;//yxk  。。。
            LblSN.Text = factoryParameter.InstrumentNum;
            //获得当前系统应用路径
            string appPath = Application.StartupPath;
            PicLogo.Image = Resources.logo;// Image.FromFile(appPath + "\\Images\\logo.png");   
            //界面中所有控件恢复初始状态
            for(int i=0;i<7;i++)
            {
                LblTitle[i].BackColor = Color.Transparent;
                LblStatus[i].BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
            }
            for(int i=0;i<13;i++)
            {
                LblValue[i].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL ;
                LblValue[i].Text ="0.0cps";
            }
        }
        
        /// <summary>
        /// 根据当前通道状态设置界面中各个通道显示效果
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="status">状态：0未启用；1启用红外未到位；2启用红外到位</param>
        private void ChannelDisplayControl(Channel channel,int status)
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
                                LblValue[(channel.ChannelID - 1) * 2].Enabled = true;                                                               
                                LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = false;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                break;
                            case "β":
                                LblValue[(channel.ChannelID - 1) * 2].Enabled = false;                              
                                LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                                LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.COLOR_BKDISABLED;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                                break;
                            case "α/β":
                                LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                                LblValue[(channel.ChannelID - 1) * 2].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                LblValue[(channel.ChannelID - 1) * 2].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;                                
                                LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].ForeColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
                                LblValue[(channel.ChannelID - 1) * 2 + 1].BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
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
                        LblValue[(channel.ChannelID - 1) * 2].Enabled = true;
                        LblValue[(channel.ChannelID - 1) * 2 + 1].Enabled = true;
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
                    LblFriskerB.Text= measureData.Beta.ToString("F1") +"cps";//衣物测量结果在Beta值中存储，衣物探头测量单位全部为cps
                    continue;
                }                
                switch (factoryParameter.MeasureType)
                {
                    case "α":
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English,"A")]);
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F1") + unit;
                        break;
                    case "β":
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English, "B")]);
                        //Label控件显示测量值
                        label.Text = "β" + measureData.Beta.ToString("F1") + unit;                        
                        break;
                    case "α/β":
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English, "A")]);
                        //Label控件显示测量值
                        label.Text = "α" + measureData.Alpha.ToString("F1") + unit;
                        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+测量类型（A/B）
                        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", measureData.Channel.ChannelName_English, "B")]);
                        //Label控件显示测量值
                        label.Text = "β" + measureData.Beta.ToString("F1") + unit;
                        break;
                }
            }                           
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

        /// <summary>
        /// 设置窗体顶部检测index索引项的进度背景图片加载标志为True,其他设置为false
        /// </summary>
        /// <param name="index">窗体顶部检测进度索引0：仪器自检；1：本底测量；2：等待测量；3：开始测量；4：无污染；5：人员污染</param>
        private void SetProgressPicFlag(int index)
        {            
            for(int i=0;i<6;i++)
            {   
                isLoadProgressPic[i] = false;               
            }
            isLoadProgressPic[index] = true;
        }
        /// <summary>
        /// 清除窗体顶部检测进度背景图片加载标志，将窗口顶部所有检测监督背景图片加载标志设置为false
        /// </summary>
        private void ClearProgressPicFlag()
        {
            for (int i = 0; i < 6; i++)
            {
                isLoadProgressPic[i] = false;
            }
        }
        private void FrmMeasureMain_Load(object sender, EventArgs e)
        {            
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            appPath = Application.StartupPath;
            messageBufferLength = 62; //最短报文长度                        
            measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象                                                            
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
            channelS.CopyTo(channelsAll, 0);//将全部通道信息复制到channelsAll中
            if (systemParameter.IsEnglish == true)
            {
                try
                {
                    isEnglish = true;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    Tools.ApplyLanguageResource(this);
                    Tools.controls.Clear();
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
            }            
            //初始化显示界面
            DisplayInit();
            LblTimeRemain.Text = string.Format("{0}s",systemParameter.SelfCheckTime.ToString());
            //实例化衣物探测界面
            frmClothes = new FrmClothes(isEnglish);
            checkTime = systemParameter.SelfCheckTime;//检测时间 
            alarmTimeSet = systemParameter.AlarmTime;//报警时间                                  
            //在界面中将启用通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，其它通道（通道未启用或探测面积为0）的控件Enabled无效；                       
            for (int i = 0; i < channelS.Count; i++)
            {
                if (channelS[i].IsEnabled == true && channelS[i].ProbeArea != 0)//通道被启用且探测面积不为0
                {
                    //界面中相关通道控件Enabled设置为true，背景色设置为正常
                    ChannelDisplayControl(channelS[i], 1);
                }
                else
                {
                    //界面中相关通道控件Enabled属性设置为false，背景色设置为屏蔽
                    ChannelDisplayControl(channelS[i], 0);
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
            for (int i = 0; i < 3; i++)
            {
                lastInfraredStatus[i] = 0;
            }
            //正常使用的通道测量数值显示（初始值为0cps）
            DisplayMeasureData(calculatedMeasureDataS, "cps");
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
            //将运行状态标志设置为“运行准备”
            platformState = PlatformState.ReadyToRun;
            //当前系统状态时间为自检时间
            stateTimeSet = systemParameter.SelfCheckTime;
            stateTimeStart = DateTime.Now;
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
                            if (isBetaCommandToSend == false)
                            {
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
                            }
                            else
                            {
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
                            }
                            ////下发成功，置报文已经发送标志
                            //isSelfCheckSended = true;
                            break;
                    }
                    //下发成功，置报文已经发送标志
                    isSelfCheckSended = true;
                    //延时200毫秒
                    Thread.Sleep(200);
                    //启动自检计时 
                    if (isBetaCommandToSend == false)//如果时Alpha/Beta自检，Beta自检指令需要下发时不需要重新设置开始时间
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
                        TxtShowResult.Text += "监测测端口通信错误！\r\n";
                        isCommError = true;
                    }
                    //延时
                    if (platformState == PlatformState.SelfTest && isSelfCheckSended == false)
                    {
                        Thread.Sleep(800- errorNumber* delayTime);
                    }
                    else
                    {
                        Thread.Sleep(800);
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
                        TxtShowResult.Text += "监测测端口通信错误！\r\n";
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
                if (errNumber >= 2)
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
            stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
            LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s", "0") : string.Format("{0,3}s", stateTimeRemain.ToString());
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            if (receiveBufferMessage[0] =='C' || receiveBufferMessage[0] == 'c') //判断报文头是C指令
            {
                measureDataS = Components.Message.ExplainMessage<MeasureData>(receiveBufferMessage);
            }            
            if(measureDataS==null||measureDataS.Count<7)//解析失败
            {
                return;
            }
            //if (platformState == PlatformState.ReadyToMeasure)
            //{
            //    TxtShowResult.Text += measureDataS[3].Beta.ToString()+"   ";
            //}

                //报文解析无误,将当前报文红外状态清零
                infraredStatusOfMessageNow &= 0;
            //加载将当前报文1-4通道红外状态
            infraredStatusOfMessageNow |= (byte)(receiveBufferMessage[61] & 7);//红外状态屏蔽高位后赋值            
            //加载当前报文5-7通道红外状态
            infraredStatusOfMessageNow |= (byte)((receiveBufferMessage[123] & 7) << 3);
            if ((infraredStatusOfMessageNow ^ infraredStatusOfMessageLast) != 0)//红外状态发生变化
            {
                //重新刷新控制各个通道显示状态
                foreach (Channel usedChannel in channelS)//channelS中存储当前全部启用且探测面积不为0的通道
                {
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == usedChannel.ChannelID).ToList();
                    if (list.Count > 0)
                    {
                        ChannelDisplayControl(usedChannel, list[0].InfraredStatus == 0 ? 1 : 2);
                        //根据当前红外状态控制左右手及衣物红外状态显示
                        switch (usedChannel.ChannelID)
                        {
                            case 1:
                            case 2:
                                if (list[0].InfraredStatus == 1)//红外到位
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
                                if (list[0].InfraredStatus == 1)//红外到位
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
                            case 7:
                                if (list[0].InfraredStatus == 1)//红外到位
                                {
                                    PicFrisker.BackgroundImage = Resources.Frisker_InPlace;// Image.FromFile(appPath + "\\Images\\Frisker_InPlace.png");
                                }
                                else
                                {
                                    PicFrisker.BackgroundImage = Resources.Frisker_NotInPlace;// Image.FromFile(appPath + "\\Images\\Frisker_NotInPlace.png");
                                }
                                break;
                        }
                    }
                    if (platformState == PlatformState.ReadyToMeasure && factoryParameter.IsDoubleProbe == false)//当前状态为开始测量且为单探测器
                    {
                        if (isHandTested == 0)//进行手部第一次测试
                        {
                            if (usedChannel.ChannelID == 2 || usedChannel.ChannelID == 4)//屏蔽左右手背
                            {
                                ChannelDisplayControl(usedChannel, 1);
                            }
                        }
                        if (isHandTested == 1)//手部第二次测试
                        {
                            if (usedChannel.ChannelID == 1 || usedChannel.ChannelID == 3)//屏蔽左右手心
                            {
                                ChannelDisplayControl(usedChannel, 1);
                            }
                        }
                    }                    
                }                
            }
            infraredStatusOfMessageLast = infraredStatusOfMessageNow;//保存当前红外状态
            for (int i=0;i<channelsAll.Count();i++)
            {
                measureDataS[i].Channel = channelsAll[i];
            }
            //衣物探头被启用
            if (measureDataS[6].Channel.IsEnabled == true)
            {                
                //衣物探头已经被拿起（红外状态为到位)
                if (measureDataS[6].InfraredStatus == 1)
                {
                    // ChannelDisplayControl(measureDataS[6].Channel, 2);     修正注释               
                    //衣物探头已经被拿起（不是刚被拿起）
                    smoothedDataOfClothes = SmoothData((UInt32)measureDataS[6].Beta);                                        
                    //如果当前状态为等待测量或开始测量则,说明衣物探测界面已经加载
                    if (platformState == PlatformState.ReadyToMeasure || platformState == PlatformState.Measuring)
                    {
                        //获得当前衣物检测通道的探测参数
                        IList<ProbeParameter> clothesProbeParmeter = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == 7).ToList();
                        if(clothesProbeParmeter.Count<=0)//无数据返回
                        {
                            return;
                        }
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
                                frmClothes = new FrmClothes(isEnglish);                               
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
                            //根据系统参数中设置的检测单位，对减去本底值后的测量值进行单位变换并在衣物探测界面中进行显示
                            //float converedData = Tools.UnitConvertCPSTo(smoothedDataOfClothes, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, measureDataS[6].Channel.ProbeArea);
                            frmClothes.TxtMeasureValue.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F1"));
                            #region 如果减去本底值后的测量值大于一级报警，说明有污染  
                            ////获得当前衣物检测通道的探测参数
                            //IList<ProbeParameter> clothesProbeParmeter = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == 7).ToList();
                            if (smoothedDataOfClothes > clothesProbeParmeter[0].Alarm_1)
                            {
                                //大于二级报警，衣物探测界面测量结果显示文本框背景色设置为ALATM2
                                if (smoothedDataOfClothes > clothesProbeParmeter[0].Alarm_2)
                                {
                                    frmClothes.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                                    frmClothes.PrgClothAlarm_2.Value = frmClothes.PrgClothAlarm_2.Maximum;                                    
                                }
                                else
                                {
                                    //大于一级报警，衣物探测界面测量结果显示文本框背景色设置为ALATM1
                                    frmClothes.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                                    frmClothes.PrgClothAlarm_1.Value = frmClothes.PrgClothAlarm_1.Maximum;                                    
                                }
                                //衣物探测界面进度条设置为100%
                                //frmClothes.PrgClothAlarm_1.Value = 100;
                                frmClothes.loadingCircle.Active = false;
                                //报警次数+1
                                alarmCountOfClothes++;
                                //如果连续三次出现污染报警（污染报警计数器超过3）
                                if (alarmCountOfClothes > 2 && isClothesContaminated == false)
                                {
                                    //将设备监测状态设置为“污染”
                                    if (smoothedDataOfClothes > clothesProbeParmeter[0].Alarm_2)
                                    {
                                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated_2);
                                    }
                                    else
                                    {
                                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated_1);
                                    }
                                    //仪器人员污染状态背景色设置为污染
                                    PnlContaminated.BackgroundImage = Resources.Contaminated_progress;// Image.FromFile(appPath + "\\Images\\Contaminated_progress.png");                                    
                                    //衣物探测结果显示区域背景色设置为污染
                                    LblFriskerB.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                                    //衣物探测结果显示区域显示当前衣物探测值
                                    LblFriskerB.Text= string.Format("{0}cps", smoothedDataOfClothes.ToString("F1"));
                                    //探测主界面的探测结果显示区域显示衣物污染
                                    if (isEnglish)
                                    {
                                        TxtShowResult.Text +=string.Format("Clothing Contaminated,Preset:{0}Actual:{1}\r\n", smoothedDataOfClothes > clothesProbeParmeter[0].Alarm_2 ? clothesProbeParmeter[0].Alarm_2 : clothesProbeParmeter[0].Alarm_1, smoothedDataOfClothes.ToString("F1"));
                                        player.Stream = Resources.English_Decontaminate_please;// appPath + "\\Audio\\English_Decontaminate_please.wav";
                                    }
                                    else
                                    {
                                        TxtShowResult.Text +=string.Format("衣物污染，设置值：{0}测量值：{0}\r\n", smoothedDataOfClothes> clothesProbeParmeter[0].Alarm_2? clothesProbeParmeter[0].Alarm_2: clothesProbeParmeter[0].Alarm_1, smoothedDataOfClothes.ToString("F1"));
                                        //语音提示被测人员污染
                                        player.Stream = Resources.Chinese_Decontaminate_please;// appPath + "\\Audio\\Chinese_Decontaminate_please.wav";
                                    }
                                    player.LoadAsync();
                                    player.PlaySync();
                                    //Thread.Sleep(2000);
                                    //将测量时间（当前时间）、状态（“污染”）、详细信息（“衣物探头”+测量值）写入数据库                                
                                    MeasureData measureData = new MeasureData();
                                    measureData.MeasureDate = DateTime.Now;
                                    measureData.MeasureStatus = "污染";
                                    //string temp = converedData.ToString("F1");
                                    measureData.DetailedInfo = string.Format("衣物探头{0}cps", smoothedDataOfClothes.ToString("F1"));
                                    measureData.IsEnglish = false;
                                    measureData.AddData(measureData);
                                    measureData.MeasureStatus = "Contaminated";
                                    measureData.DetailedInfo = string.Format("Frisker{0}cps", smoothedDataOfClothes.ToString("F1"));
                                    measureData.IsEnglish = true;
                                    measureData.AddData(measureData);
                                    //启动报警计时
                                    alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                                    isClothesContaminated = true;                                        
                                    return;
                                }
                            }
                            #endregion
                            #region 如果减去本底值后的测量值未大于一级报警，说明没有污染
                            else
                            {
                                //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                                IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "C" && efficiencyParameter.Channel.ChannelID == 7 && efficiencyParameter.NuclideName == clotheseNuclideUsed).ToList();
                                //设置衣物探测界面进度条变化百分比
                                int prgBarValue = (int)(smoothedDataOfClothes / efficiencyParameterNow[0].Efficiency);
                                frmClothes.PrgClothAlarm_1.Value = prgBarValue>=frmClothes.PrgClothAlarm_1.Maximum?frmClothes.PrgClothAlarm_1.Maximum: prgBarValue;
                                frmClothes.PrgClothAlarm_2.Value= prgBarValue >= frmClothes.PrgClothAlarm_2.Maximum ? frmClothes.PrgClothAlarm_2.Maximum : prgBarValue;
                                //衣物探测界面测量结果显示文本框背景色设置为SYSTEM
                                frmClothes.TxtMeasureValue.BackColor = PlatForm.ColorStatus.COLOR_SYSTEM;
                                //报警计数器归0
                                alarmCountOfClothes = 0;
                                //语音提示dida2.wav                    
                                player.Stream = Resources.dida2;// appPath + "\\Audio\\dida2.wav";
                                player.Load();
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
                    //ChannelDisplayControl(measureDataS[6].Channel, 1);修正注释
                    ////设置衣物探测区域状态图片为到位
                    //PicFrisker.Image = Image.FromFile(appPath + "\\Images\\Frisker_NotInPlace.png");
                    ////设置衣物探测区域状态标签为已到位
                    //LblFriskerStatus.BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
                    //LblFriskerStatus.ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
                    //衣物探测结果显示区域背景色设置为正常
                    LblFriskerB.BackColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
                    //衣物探测结果显示区域显示当前衣物探测值为0cps
                    //LblFriskerB.Text = string.Format("{0}{1}",0,"cps");
                    if (clothesStatus == 1)//探头当前状态为已经被拿起，说明衣物探头刚刚被放下
                    {                        
                        //关闭衣物探头监测界面
                        frmClothes.Close();
                        //语音提示dida1.wav
                        player.Stream = Resources.dida1;// appPath + "\\Audio\\dida1.wav";
                        player.Load();
                        player.Play();    
                        if(isClothesContaminated==false)
                        {
                            // 仪器无污染状态背景色设置为无污染
                            PnlNoContamination.BackgroundImage = Resources.NoContamination_progress;// Image.FromFile(appPath + "\\Images\\NoContamination_progress.png");                        
                            PnlMeasuring.BackColor = Color.Transparent;
                            ////设备监测状态为正常
                            //deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        }
                        //设置衣物探头状态为0，已经被放下
                        clothesStatus = 0;
                        //设置检测完成标志为true
                        isTestedEnd = true;
                        ////一次检测周期完成，恢复手部检测状态标志为0，手部未开始检测
                        //isHandTested = 0;
                        //运行状态设置为“测量结束”
                        platformState = PlatformState.Result;
                        //#region 如果衣物离线时间大于系统设置的离线自检时间，或者衣物污染，则启动本底测量
                        //if (clothesTimeCount > systemParameter.ClothOfflineTime||isClothesContaminated==true)
                        //{                            
                        //    clothesTimeCount = 0;
                        //    //监测结果显示区域显示“仪器本底测量”
                        //    if (isEnglish)
                        //    {
                        //        TxtShowResult.Text += "Updating background\r\n";
                        //        player.SoundLocation = appPath + "\\Audio\\English_Updating_background.wav";
                        //    }
                        //    else
                        //    {
                        //        TxtShowResult.Text += "仪器本底测量\r\n";
                        //        //语音提示本底测量
                        //        player.SoundLocation = appPath + "\\Audio\\Chinese_Background_measure.wav";
                        //    }
                        //    player.PlaySync();
                        //    //Thread.Sleep(1000);                            
                        //    //将当前系统状态设置为本底测量
                        //    platformState = PlatformState.BackGrouneMeasure;
                        //    //重新设置本底测量开始时间
                        //    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        //    return;
                        //}
                        //#endregion                            
                        return;                                            
                    }
                    //对衣物探头测量数据进行平滑处理
                    smoothedDataOfClothes = SmoothData((UInt32)measureDataS[6].Beta);
                    if (platformState == PlatformState.BackGrouneMeasure || platformState == PlatformState.ReadyToMeasure || platformState == PlatformState.Measuring)
                    {
                        //将当前平滑处理后的检测值作为本底值
                        baseDataOfClothes = smoothedDataOfClothes;
                        //在衣物检测结果显示区域显示当前测量值，根据数据大小控制显示字体大小
                        if (smoothedDataOfClothes < 10)
                        {
                            LblFriskerB.Font = new Font("宋体", 16, FontStyle.Bold);
                            LblFriskerB.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F1"));
                        }
                        else if (smoothedDataOfClothes < 1000)
                        {
                            LblFriskerB.Font = new Font("宋体", 16, FontStyle.Bold);
                            LblFriskerB.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F0"));
                        }
                        else
                        {
                            LblFriskerB.Font = new Font("宋体", 10, FontStyle.Bold);
                            LblFriskerB.Text = string.Format("{0}cps", smoothedDataOfClothes.ToString("F0"));
                        }
                    }
                }
                //if(isClothesContaminated == true)//衣物污染
                //{
                //    if ((DateTime.Now - alarmTimeStart).Seconds < alarmTimeSet)//未到报警时间长度
                //    {
                //        return;
                //    }
                //    else  //强制本底测量
                //    {
                //        //恢复检测状态为正常
                //        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                //        // 运行状态标志设置为“本底测量”
                //        platformState = PlatformState.BackGrouneMeasure;
                //        //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                //        stateTimeSet = systemParameter.SmoothingTime;
                //        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                //        for (int i = 0; i < channelS.Count; i++)
                //        {
                //            calculatedMeasureDataS[i].Alpha = 0;
                //            calculatedMeasureDataS[i].Beta = 0;
                //        }
                //        //系统状态显示区域显示本底测量
                //        if (isEnglish)
                //        {
                //            //测试结果区域显示本底测量
                //            TxtShowResult.Text += "Updating Background\r\n";
                //            //系统提示本底测量
                //            player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
                //        }
                //        else
                //        {
                //            //测试结果区域显示本底测量
                //            TxtShowResult.Text += "本底测量\r\n";
                //            //系统提示本底测量
                //            player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
                //        }
                //        player.LoadAsync();
                //        player.PlaySync();
                //        //测量值显示标签背景恢复为默认状态（如果检查结果为人员污染，则会将测量值显示标签背景色变为污染报警，所以需要恢复）
                //        for (int i = 0; i < channelS.Count; i++)
                //        {
                //            //通道测量值标签
                //            if (channelS[i].ChannelID == 7)
                //            {
                //                LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
                //            }
                //            else
                //            {
                //                LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
                //                LblValue[(channelS[i].ChannelID - 1) * 2 + 1].BackColor = Color.White;
                //            }
                //        }
                //        //启动本底测量计时 
                //        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                //        return;
                //    }
                //}                
            }
            //人员污染同时报警时间小于系统参数设置的报警时间长度，则直接返回等待
            //if (((deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_1)) || (deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_2)) || isClothesContaminated == true) && ((DateTime.Now - alarmTimeStart).Seconds < alarmTimeSet))
            //{
            //    return;
            //}
            //else//报警时间超过系统参数设置的报警时间长度，则监测状态恢复为正常状态                       
            //{
            //    //如果人员污染同时报警时间大于系统参数设置的报警时间长度，则进行本底测量
            //    if (((deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_1))|| (deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_2))) && ((DateTime.Now - alarmTimeStart).Seconds >= alarmTimeSet))
            //    {
            //        //恢复检测状态为正常
            //        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
            //        // 运行状态标志设置为“本底测量”
            //        platformState = PlatformState.BackGrouneMeasure;
            //        //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
            //        stateTimeSet = systemParameter.SmoothingTime;                    
            //        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
            //        for (int i = 0; i < channelS.Count; i++)
            //        {
            //            calculatedMeasureDataS[i].Alpha = 0;
            //            calculatedMeasureDataS[i].Beta = 0;
            //        }
            //        //系统状态显示区域显示本底测量
            //        if (isEnglish)
            //        {
            //            //测试结果区域显示本底测量
            //            TxtShowResult.Text += "Updating Background\r\n";
            //            //系统提示本底测量
            //            player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
            //        }
            //        else
            //        {
            //            //测试结果区域显示本底测量
            //            TxtShowResult.Text += "本底测量\r\n";
            //            //系统提示本底测量
            //            player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
            //        }
            //        player.LoadAsync();
            //        player.PlaySync();
            //        //测量值显示标签背景恢复为默认状态（如果检查结果为人员污染，则会将测量值显示标签背景色变为污染报警，所以需要恢复）
            //        for (int i = 0; i < channelS.Count; i++)
            //        {
            //            //通道测量值标签
            //            if (channelS[i].ChannelID == 7)
            //            {
            //                LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
            //            }
            //            else
            //            {
            //                LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
            //                LblValue[(channelS[i].ChannelID - 1) * 2 + 1].BackColor = Color.White;
            //            }
            //        }
            //        //启动本底测量计时 
            //        stateTimeStart = System.DateTime.Now.AddSeconds(1);
            //        alarmTimeStart = DateTime.MaxValue;//报警时间设置为一个大值，避免时间计算出现错误从而无法进入Result状态
            //        //Thread.Sleep(1000);
            //        return;
            //    }
            //    //恢复检测状态为正常
            //    deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);                
            //}
            //如果当前运行状态为“运行准备”
            if (platformState == PlatformState.ReadyToRun)
            {                            
                //如果手部探测器为启用状态，则判断左右手是否到位，到位则相应指示框背景变为绿色，否则为橙色，同时进行文字信息提示（具体操作可参考老版本源代码）
                //if (channelS.FirstOrDefault(channel => channel.ChannelName_English == "LHP")!=null)//左手心启用,左右手全部启用（手心手背四个通道）
                //{                    
                //    if(measureDataS[0].InfraredStatus!=1)//左手红外不到位（左手心和左手背红外状态一致，所以判断左手心即可）
                //    {                                                
                //        if (isEnglish)
                //        {
                //            LblLeft.Text = "LH not in place";                            
                //        }
                //        else
                //        {
                //            LblLeft.Text = "左手不到位";
                //        }
                //    }
                //    else
                //    {                       
                //        if (isEnglish)
                //        {
                //            LblLeft.Text = "LH in place";
                //        }
                //        else
                //        {
                //            LblLeft.Text = "左手到位";
                //        }
                //    }
                //    if(measureDataS[2].InfraredStatus!=1)//右手红外不到位（有手心和右手背红外状态一致，所以判断右手心即可）
                //    {                       
                //        if (isEnglish)
                //        {
                //            LblRight.Text = "RH not in place";
                //        }
                //        else
                //        {
                //            LblRight.Text = "右手不到位";
                //        }
                //    }
                //    else
                //    {                        
                //        if (isEnglish)
                //        {
                //            LblRight.Text = "RH in place";
                //        }
                //        else
                //        {
                //            LblRight.Text = "右手到位";
                //        }
                //    }
                //}                
                if (isEnglish)
                {                   
                    TxtShowResult.Text += "Self-checking\r\n";
                    player.Stream = Resources.English_Self_checking;// appPath + "\\Audio\\English_Self_checking.wav";
                }
                else
                {                   
                    TxtShowResult.Text += "仪器自检\r\n";
                    //IOThread.Start("SelfTest");                
                    //IOThread.Suspend();
                    player.Stream = Resources.Chinese_Self_checking;// appPath + "\\Audio\\Chinese_Self_checking.wav";
                }
                player.LoadAsync();
                player.PlaySync();
                //当前运行状态设置为“仪器自检”
                platformState = PlatformState.SelfTest;
                //获得当前系统参数设置中的的自检时间并赋值给stateTimeSet
                stateTimeSet = systemParameter.SelfCheckTime;
                stateTimeStart = System.DateTime.Now;                
                return;
            }
            //如果当前运行状态为“仪器自检”
            if (platformState == PlatformState.SelfTest)
            {                                
                string[] errRecordS = new string[2];
                if (errRecordS == null)//无自检故障信息时设置背景为正常状态背景
                {
                    //仪器自检状态标签设置为当前状态图片
                    PnlSelfCheck.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    LblCheck.BringToFront();
                }
                if (isLoadProgressPic[0] == false)
                {
                    SetProgressPicFlag(0);//仪器自检进度图片已经被加载标志设置为true，其它为false
                    //仪器自检状态标签设置为当前状态图片
                    PnlSelfCheck.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    //取消本底测量状态图片
                    PnlBackground.BackgroundImage = null;
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                    LblTimeRemain.Parent = PnlSelfCheck;
                    LblTimeRemain.Location = new Point(84, 0);//控制剩余时间标签显示位置
                    LblTimeRemain.BringToFront();
                }
                ////获得当前系统参数设置中的的自检时间并赋值给stateTimeSet
                //stateTimeSet =systemParameter.SelfCheckTime;               
                //更新剩余时间：系统自检设置时间-已经用时
                if(stateTimeStart.Year<DateTime.Now.Year)//说明还未开始计时
                {
                    return;
                }
                //stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;                
                //更新当前系统运行状态剩余时间
                //LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s","0"):string.Format("{0,3}s",stateTimeRemain.ToString());                
                for (int i = 0; i < channelS.Count; i++) //遍历全部启用的检测通道
                {                    
                    /*因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    *所以，需要从measureDataS中找到对应通道的监测数据赋值给calculatedMeasureDataS，但是本地值Alpha和Beta需要累加*/
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if (list.Count > 0)
                    {
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
                    //判断红外状态，在手部状态区域进行相应提示
                    //if (calculatedMeasureDataS[i].InfraredStatus == 1)//红外到位
                    //{
                    //    //界面相应通道显示状态控制-红外到位
                    //    // ChannelDisplayControl(calculatedMeasureDataS[i].Channel, 2);修正注释
                    //    //左手到位
                    //    if (calculatedMeasureDataS[i].Channel.ChannelID == 1 || calculatedMeasureDataS[i].Channel.ChannelID ==2)
                    //    {
                    //        if (isEnglish)
                    //        {
                    //            LblLeft.Text = "LH in place";
                    //        }
                    //        else
                    //        {
                    //            LblLeft.Text = "左手到位";
                    //        }                            
                    //    }
                    //    //右手到位
                    //    if((calculatedMeasureDataS[i].Channel.ChannelID == 3 || calculatedMeasureDataS[i].Channel.ChannelID ==4))
                    //    {
                    //        if (isEnglish)
                    //        {
                    //            LblRight.Text = "RH in place";
                    //        }
                    //        else
                    //        {
                    //            LblRight.Text = "右手到位";
                    //        }                                                        
                    //    }
                    //}
                    //else//红外不到位
                    //{
                    //    //界面相应通道显示状态控制-红外不到位
                    //    // ChannelDisplayControl(calculatedMeasureDataS[i].Channel, 1);修正注释
                    //    //左手不到位
                    //    if (calculatedMeasureDataS[i].Channel.ChannelID ==1 || calculatedMeasureDataS[i].Channel.ChannelID ==2)
                    //    {
                    //        if (isEnglish)
                    //        {
                    //            LblLeft.Text = "LH not in place";
                    //        }
                    //        else
                    //        {
                    //            LblLeft.Text = "左手不到位";
                    //        }                            
                    //    }
                    //    //右手不到位
                    //    if ((calculatedMeasureDataS[i].Channel.ChannelID == 3 || calculatedMeasureDataS[i].Channel.ChannelID ==4))
                    //    {
                    //        if (isEnglish)
                    //        {
                    //            LblRight.Text = "RH not in place";
                    //        }
                    //        else {
                    //            LblRight.Text = "右手不到位";
                    //        }                                                        
                    //    }
                    //}
                }
                //界面中对应通道显示当前测量值
                DisplayMeasureData(measureDataS, "cps");
                //如果是α/β自检且自检时间是否到自检时间的一半
                if(factoryParameter.MeasureType== "α/β"&& stateTimeRemain== stateTimeSet/2+2)
                {
                    isSelfCheckSended = false;
                    isBetaCommandToSend = true;
                }
                //自检时间到
                if (stateTimeRemain < 0)
                {                                      
                    errRecordS = BaseCheck();                    
                    if (errRecordS == null)//自检通过
                    {
                        //恢复当前标签文字提示（自检过程中可能有故障提示，所以自检通过后需要重新恢复）
                        if (isEnglish==false)
                        {
                            LblCheck.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                            LblCheck.Text = "仪器自检";
                        }
                        else
                        {
                            LblCheck.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                            LblCheck.Text = "Self-Checking";
                        }
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        // 运行状态标志设置为“本底测量”
                        platformState = PlatformState.BackGrouneMeasure;
                        //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                        stateTimeSet = systemParameter.SmoothingTime;
                        //启动本底测量计时 
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS初始化为当前本底值，为本底测量时计算做准备
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha =0;
                            calculatedMeasureDataS[i].Beta = 0 ;
                        }                        
                        //系统状态显示区域显示本底测量
                        if (isEnglish)
                        {                            
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "Updating Background\r\n";
                            //系统提示本底测量                                                
                            player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
                        }
                        else
                        {                            
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "本底测量\r\n";
                            //系统提示本底测量                                                
                            player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
                        }
                        player.LoadAsync();
                        player.PlaySync();
                    }
                    else
                    {
                        //仪器自检状态背景色设置为故障
                        PnlSelfCheck.BackgroundImage = Resources.Fault_progress;// Image.FromFile(appPath + "\\Images\\Fault_progress.png");
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库    
                        AddErrorData(errRecordS);
                        //语音提示故障
                        if (isEnglish)
                        {
                            player.Stream = Resources.English_Self_checking_fault;// appPath + "\\Audio\\English_Self-checking_fault.wav";
                        }
                        else
                        {
                            player.Stream = Resources.Chinese_Self_checking_fault;// appPath + "\\Audio\\Chinese_Self-checking_fault.wav";
                        }
                        player.LoadAsync();
                        player.PlaySync();
                        //测量数据存储全部清零
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        isSelfCheckSended = false;
                        isBetaCommandToSend = false;
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
                if (isLoadProgressPic[1] == false)
                {
                    SetProgressPicFlag(1);//本底测量进度图片已经被加载标志设置为true，其它为false
                    //取消仪器自检状态标签设置
                    PnlSelfCheck.BackgroundImage = null;
                    //取消等待测量状态标签设置
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                    PnlNoContamination.BackgroundImage = null;
                    PnlContaminated.BackgroundImage = null;
                    //本底测量状态标签设置为当前状态图片
                    PnlBackground.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    LblTimeRemain.Parent = PnlBackground;//控制剩余时间标签显示位置                                
                    LblBackground.BringToFront();
                    LblTimeRemain.BringToFront();
                    LblTimeRemain.BackColor = Color.Transparent;
                }
                bool isReDisplay = false; //是否需要重新显示"本底测量"提示信息,默认false
                //textBox1.Text += platformState.ToString();
                ////获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                //stateTimeSet = systemParameter.SmoothingTime;                        
                //在系统界面中显示本底测量倒计时时间（s）:系统平滑设置时间-已经用时
                //stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;                
                //LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s","0"):string.Format("{0,3}s",stateTimeRemain.ToString());                
                for (int i = 0; i < channelS.Count; i++)
                {                    
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if (list.Count > 0)
                    {
                        if (isFirstBackGroundData)
                        {
                            calculatedMeasureDataS[i].Alpha = list[0].Alpha;
                            calculatedMeasureDataS[i].Beta = list[0].Beta;
                            
                        }
                        else
                        {
                            //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)：
                            //第k次计算本底值=第k-1次计算本底值*平滑因子/（平滑因子+1）+第k次测量值/（平滑因子+1）               
                            calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Alpha / (factoryParameter.SmoothingFactor + 1);
                            calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Beta / (factoryParameter.SmoothingFactor + 1);
                        }
                        calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    }
                    //当前通道红外到位
                    if (calculatedMeasureDataS[i].InfraredStatus == 1)
                    {
                        //更新当前通道显示状态为红外到位
                        // ChannelDisplayControl(calculatedMeasureDataS[i].Channel,2);修正注释
                        //显示当前calculatedMeasureDataS[i].Channel.ChannelName对应的红外到位
                        if (calculatedMeasureDataS[i].Channel.ChannelID ==1 || calculatedMeasureDataS[i].Channel.ChannelID ==2)
                         {                                                        
                            if (lastInfraredStatus[0] == 0) //左手上次为红外不到位，本次为红外到位，说明需要重新进行语音提示
                            {                                
                                //系统状态显示区域显示测量中断
                                if (isEnglish)
                                {
                                    LblBackground.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                                    LblBackground.Text = "Interrupted";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "Left hand in place,Please measure again!\r\n";
                                    player.Stream = Resources.English_Left_hand_in_place_please_measure_again;// appPath + "\\Audio\\English_Left_hand_in_place_please_measure_again.wav";
                                    //LblLeft.Text = "LH in place";
                                }
                                else
                                {
                                    LblBackground.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                                    LblBackground.Text = "测量中断";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "左手到位，重新测量\r\n";
                                    player.Stream = Resources.Chinese_Left_hand_in_place_please_measure_again;// appPath + "\\Audio\\Chinese_Left_hand_in_place_please_measure_again.wav";
                                    //LblLeft.Text = "左手到位";
                                }
                                player.Load();
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
                                    LblBackground.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                                    LblBackground.Text = "Interrupted";
                                    //测量结果显示区域显示右手到位重新测量
                                    TxtShowResult.Text += "Right hand in place,Please measure again!\r\n";
                                    player.Stream = Resources.English_right_hand_in_place_please_measure_again;// appPath + "\\Audio\\English_right_hand_in_place_please_measure_again.wav";
                                    //LblRight.Text = "LH in place";
                                }
                                else
                                {
                                    LblBackground.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                                    LblBackground.Text = "测量中断";
                                    //测量结果显示区域显示右手到位重新测量
                                    TxtShowResult.Text += "右手到位，重新测量\r\n";
                                    //PicRightBackground.BackColor = PlatForm.ColorStatus.CORLOR_NORMAL;
                                    //LblRight.Text = "右手到位，重新测量";
                                    player.Stream = Resources.Chinese_right_hand_in_place_please_measure_again;// appPath + "\\Audio\\Chinese_right_hand_in_place_please_measure_again.wav";
                                    //LblRight.Text = "右手到位";
                                }
                                player.Load();
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
                            if (lastInfraredStatus[2] == 0)//衣物上次为红外不到位，本次为红外到位，说明需要重新进行本底测量，进行语音提示
                            {
                                //PicFrisker.Image= Image.FromFile(appPath + "\\Images\\CLO_InPlace.png");
                                //系统状态显示区域显示测量中断
                                if (isEnglish)
                                {
                                    LblBackground.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                                    LblBackground.Text = "Interrupted";
                                    //测量结果显示区域显示左手到位重新测量
                                    TxtShowResult.Text += "Frisker in place,Please measure again!\r\n";
                                    player.Stream = Resources.English_frisker_In_place;// appPath + "\\Audio\\English_frisker_In_place.wav";
                                }
                                else
                                {
                                    LblBackground.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                                    LblBackground.Text = "测量中断";
                                    //测量结果显示区域显示衣物探头到位重新测量
                                    TxtShowResult.Text += "衣物探头到位，重新测量\r\n";
                                    player.Stream = Resources.Chinese_frisker_In_place_measure_again;// appPath + "\\Audio\\Chinese_frisker_In_place_measure_again.wav";
                                }
                                player.Load();
                                player.Play();
                                //重新启动测量计时
                                stateTimeStart = System.DateTime.Now.AddSeconds(1);
                                //Thread.Sleep(3000);
                            }
                            //记录当前红外状态
                            lastInfraredStatus[2] = 1;
                        }                        
                        //重新启动本底测量（本底测量时间重新开始计时）
                        stateTimeStart = System.DateTime.Now;
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for (int j = 0; j < channelS.Count; j++)
                        {
                            calculatedMeasureDataS[j].Alpha = 0;
                            calculatedMeasureDataS[j].Beta =0;                            
                        }
                        isFirstBackGroundData = true;
                        return;
                    }
                    else//当前通道红外不到位
                    {
                        //更新当前通道显示状态为不到位
                        // ChannelDisplayControl(calculatedMeasureDataS[i].Channel, 1);修正注释
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
                isFirstBackGroundData = false;
                DisplayMeasureData(calculatedMeasureDataS,"cps");//yxk,修改,显示数据
                //之前测量被中断过，需要重新显示提示信息
                if (isReDisplay== true)
                {
                    if (isEnglish)
                    {
                        LblBackground.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                        LblBackground.Text = "Updating Background";
                    }
                    else
                    {
                        LblBackground.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                        LblBackground.Text = "本底测量";
                    }
                    //重新启动本底测量（本底测量时间重新开始计时）
                    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    isReDisplay =false;
                    return;
                }
                //本底测量时间到
                if (stateTimeRemain < 0)
                {
                    isFirstBackGroundData = true;
                    //isPlayed = false;
                    //各个手部和脚部通道显示当前测量本底值（cps）
                    string[] errRecordS = BaseCheck();
                    //本底测量判断
                    if (errRecordS == null)//本底测量通过
                    {                        
                        //系统状态显示区域显示等待测量
                        if (isEnglish)
                        {                            
                            //测试结果区域显示仪器正常等待测量
                            TxtShowResult.Text += "Ready\r\n";
                            //系统语音提示仪器正常等待测量
                            player.Stream = Resources.English_Ready;// appPath + "\\Audio\\English_Ready.wav";
                        }
                        else
                        {                            
                            //测试结果区域显示仪器正常等待测量
                            TxtShowResult.Text += "仪器正常 等待测量\r\n";
                            //系统语音提示仪器正常等待测量
                            player.Stream = Resources.Chinese_Ready;// appPath + "\\Audio\\Chinese_Ready.wav";
                        }
                        player.LoadAsync();
                        player.PlaySync();
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //系统参数中，将上次本底测量后已测量人数清零                        
                        systemParameter.ClearMeasuredCount();
                        systemParameter.MeasuredCount = 0;
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                            MeasureData baseDataTemp = new MeasureData();
                            Tools.Clone(calculatedMeasureDataS[i], baseDataTemp);
                            baseData.Add(baseDataTemp);
                            //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为测量时计算做准备
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        // 运行状态标志设置为“等待测量”
                        platformState = PlatformState.ReadyToMeasure;
                        //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                        stateTimeSet = systemParameter.SmoothingTime;
                        //启动等待测量计时(因为在等待测量过程中也要进行本底测量和计算) 
                        stateTimeStart = System.DateTime.Now;
                    }
                    else//本底测量未通过
                    {
                        //仪器本底状态背景色设置为故障
                        PnlBackground.BackgroundImage = Resources.Fault_progress;// Image.FromFile(appPath + "\\Images\\Fault_progress.png");
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //语音提示故障
                        if (isEnglish)
                        {
                            player.Stream = Resources.English_Background_abnomal;// appPath + "\\Audio\\English_Background_abnomal.wav";
                        }
                        else
                        {
                            player.Stream = Resources.Chinese_Background_abnomal;// appPath + "\\Audio\\Chinese_Background_abnomal.wav";
                        }
                        player.LoadAsync();
                        player.PlaySync();
                        //将故障信息errRecord写入数据库
                        AddErrorData(errRecordS);                        
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        stateTimeStart = new DateTime(1900, 01, 01);//重新初始化开始计时对象
                        isSelfCheckSended = false;//置是否已经下发自检指令标志为false（需重新下发自检指令）
                        isBetaCommandToSend = false;//
                        platformState = PlatformState.ReadyToRun;                        
                    }
                    return;
                }
                return;
            }
            //运行状态为等待测量
            if (platformState == PlatformState.ReadyToMeasure)
            {
                if (isLoadProgressPic[2] == false)
                {
                    SetProgressPicFlag(2);////等待测量进度图片已经被加载标志设置为true，其它为false
                    PnlBackground.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                    PnlNoContamination.BackgroundImage = null;
                    PnlContaminated.BackgroundImage = null;
                    //等待测量状态标签设置为当前状态图片
                    PnlReady.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    LblReady.BringToFront();
                    LblTimeRemain.Parent = PnlReady;//控制剩余时间标签显示位置
                    LblTimeRemain.BringToFront();
                    return;
                }
                //获得当前系统参数设置中的的测量时间并赋值给stateTimeSet
                //stateTimeSet = systemParameter.SmoothingTime;
                //在系统界面中显示正在测量倒计时时间（s）:系统设置测量时间-已经用时
                //stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                //LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s", "0") : string.Format("{0,3}s", stateTimeRemain.ToString());
                MeasureData conversionData = new MeasureData();
                IList<MeasureData> conversionDataS = new List<MeasureData>();
                //所有手部红外到位标志，默认全部到位
                bool isHandInfraredStatus = true;
                //控制每个通道的红外状态显示
                //for (int i = 0; i < channelS.Count; i++)修正注释
                //{
                //    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                //    if (list[0].InfraredStatus == 0)
                //    {
                //        ChannelDisplayControl(list[0].Channel, 1);
                //    }
                //    if (list[0].InfraredStatus == 1)
                //    {
                //        ChannelDisplayControl(list[0].Channel, 2);
                //    }
                //} 
                //手部测量完成，但还未恢复为初始状态标志，说明衣物探头启用且还未进行测量
                //注：如果衣物探头启用，则恢复状态标志0在衣物测量完成后进行
                //如果衣物探头未启用，则恢复状态标志0在Result状态中进行，则不会出现满足下面条件的情况                
                //如果是单探测器，且手部第一次检查完成，则需要扔掉翻转手掌过程中预读取的数据
                if (factoryParameter.IsDoubleProbe==false && isHandTested==1)//isHandTested为true，说明手部第一次检测已经完成现在的等待检测状态为第二次手部检测
                {                    
                    if (stateTimeRemain <= 0)
                    {
                        //重新启动本底计时
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    }
                    if (measureDataS[1].InfraredStatus == 0 && measureDataS[3].InfraredStatus == 0 && isHandSecondEnabled == false)
                    {
                        
                        isHandSecondEnabled = true;                        
                        return;
                    }
                    if (isHandSecondEnabled == false)
                    {
                        if (playControl % 5 == 0)
                        {
                            //提示翻转手掌进行检测
                            if (isEnglish == false)
                            {
                                //提示翻转手掌
                                player.Stream = Resources.Chinese_Please_Flip_Palm_for_Measuring;// appPath + "\\Audio\\Chinese_Please_Flip_Palm_for_Measuring.wav";
                            }
                            else
                            {
                                player.Stream = Resources.English_please_flip_palm_for_measuring;// appPath + "\\Audio\\English_Please_Flip_Palm_for_Measuring.wav";
                            }
                            player.Load();
                            player.Play();
                        }
                        playControl++;
                        return;
                    }                   
                }
                if(isHandTested==2)//手部检测已经完成
                {
                    if (stateTimeRemain <= 0)
                    {
                        //重新启动本底计时
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    }
                    if (measureDataS[2].InfraredStatus == 0 && measureDataS[4].InfraredStatus == 0)//说明手部检测完成后，红外不到位，手部已经离开监测仪
                    {
                        //重置手部检测标志为0（未开始检测）
                        isHandTested = 0;
                        return;
                    }
                    else
                    {
                        if (playControl % 5 == 0)
                        {
                            //衣物探头未启用，提示离开
                            if (measureDataS[6].Channel.IsEnabled == false)
                            {
                                if (isEnglish)
                                {
                                    //提示翻转手掌
                                    player.Stream = Resources.English_NoContamination_please_leave;// appPath + "\\Audio\\English_NoContamination_please_leave.wav";
                                }
                                else
                                {
                                    player.Stream = Resources.Chinese_NoContamination_please_leave;// appPath + "\\Audio\\Chinese_NoContamination_please_leave.wav";
                                }
                            }
                            else//衣物探头启用，提示进行衣物检测
                            {
                                if (isEnglish)
                                {
                                    //提示进行衣物检测
                                    player.Stream = Resources.English_NoContamination_please_frisker;// appPath + "\\Audio\\English_NoContamination_please_frisker.wav";
                                }
                                else
                                {
                                    player.Stream = Resources.Chinese_NoContamination_please_frisker;// appPath + "\\Audio\\Chinese_NoContamination_please_frisker.wav";
                                }
                            }
                            player.Load();
                            player.Play();
                        }
                        playControl++;
                        return;
                    }
                }
                //只有衣物检测被启用
                if (channelS.Count==1 && channelS[0].ChannelID==7)
                {
                    isHandInfraredStatus = false;
                }                
                for (int i = 0; i < channelS.Count; i++)
                {                    
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();  
                    if(list.Count<=0)
                    {
                        continue;
                    }
                    if (list[0].Channel.ChannelID>=1&& list[0].Channel.ChannelID<=7 && list[0].InfraredStatus == 0)//当前通道为手部脚步通道且红外不到位  ///yxk测试修改 衣物探头在等待测量界面的时候显示值有数，提示框测量值为零
                    {                        
                            //第一次判断红外状态
                            if (isFirstBackGround == true)
                            {
                                //重新启动本底计时
                                stateTimeStart = System.DateTime.Now;
                                isFirstBackGround = false;
                            }
                        //手部红外状态到位标志置false，说明手部不到位
                        if (list[0].Channel.ChannelID != 7)
                        {
                            isHandInfraredStatus = false;
                        }
                        if (isFirstBackGroundData)
                        {
                            calculatedMeasureDataS[i].Alpha = list[0].Alpha;
                            calculatedMeasureDataS[i].Beta = list[0].Beta;                            
                        }
                        else
                        {
                            //继续计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)：
                            //第k次计算本底值=第k-1次计算本底值*平滑因子/（平滑因子+1）+第k次测量值/（平滑因子+1）                                       
                            calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Alpha / (factoryParameter.SmoothingFactor + 1);
                            calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Beta / (factoryParameter.SmoothingFactor + 1); ;
                        }
                            calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                        //获得当前系统参数设置中的测量单位                                                
                        //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                        //IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == alphaNuclideUsed).ToList();
                        //conversionData.Alpha = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Alpha, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                        //efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == betaNuclideUsed).ToList();
                        //conversionData.Beta = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Beta, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                        //conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        ////将单位转换后的测量数据添加进IList列表
                        //MeasureData conversionDataTemp = new MeasureData();
                        //Tools.Clone(conversionData, conversionDataTemp);
                        //Tools.Clone(conversionData.Channel, conversionDataTemp.Channel = new Channel());
                        //conversionDataS.Add(conversionDataTemp);
                    }                      
                }
                isFirstBackGroundData = false;
                //DisplayMeasureData(conversionDataS, systemParameter.MeasurementUnit);//yxk,修改,实时显示数据
                //所有通道手部红外状态全部到位
                if (isHandInfraredStatus == true && isHandTested!=2)
                {                    
                    //系统语音提示仪器正常开始测量
                    if (isEnglish)
                    {                        
                        //测试结果区域显示开始测量
                        TxtShowResult.Text += "Start counting\r\n";
                       // player.Stream = Resources.English_Start_counting;// appPath + "\\Audio\\English_Start_counting.wav";
                    }
                    else
                    {                        
                        //测试结果区域显示开始测量
                        TxtShowResult.Text += "开始测量\r\n";
                        //player.Stream = Resources.Chinese_Start_counting;// appPath + "\\Audio\\Chinese_Start_counting.wav";
                    }
                    //player.Load();
                    //player.PlaySync();
                    //Thread.Sleep(100);
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
                    //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为测量时计算做准备
                    for (int i = 0; i < channelS.Count; i++)
                    {
                        calculatedMeasureDataS[i].Alpha = 0;
                        calculatedMeasureDataS[i].Beta = 0;
                        //通道状态修改为红外到位
                        //ChannelDisplayControl(calculatedMeasureDataS[i].Channel, 2);修正注释
                    }
                    DisplayMeasureData(calculatedMeasureDataS, systemParameter.MeasurementUnit);//yxk,修改,清零
                    //将运行状态修改为“开始测量”
                    platformState = PlatformState.Measuring;
                    //获得当前系统参数设置中的的测量时间并赋值给stateTimeSet
                    stateTimeSet = systemParameter.MeasuringTime;
                    //重新启动计时，为开始测量及时准备
                    stateTimeStart = System.DateTime.Now;
                    return;
                }
                //本底测量时间到，进行本底判断
                if (stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds <= 0)
                {
                    //下次如果还进行本底计算，则需重新计时，所以置标志为True
                    isFirstBackGround = true;
                    isFirstBackGroundData = true;
                    string[] errRecordS = BaseCheck();
                    //本底测量判断
                    if (errRecordS == null)//本底检测通过
                    {
                        DisplayMeasureData(measureDataS, "cps");//yxk,修改显示当前本底值
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        //下次如果还进行本底计算，则需重新计时，所以置标志为True
                        isFirstBackGround = true;                       
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            //将最终的本底计算结果保存，以用于检测时对测量结果进行校正
                            Tools.Clone(calculatedMeasureDataS[i], baseData[i]);
                            //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次计算做准备
                            //calculatedMeasureDataS[i].Alpha = 0;
                            //calculatedMeasureDataS[i].Beta = 0;
                        }
                    }
                    else//本底检测未通过
                    {
                        //仪器本底测量状态背景色设置为故障
                        PnlBackground.BackgroundImage = Resources.Fault_progress;
                        //将设备监测状态设置为“故障”
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingFaulted);
                        //将故障信息errRecord写入数据库
                        AddErrorData(errRecordS);
                        //界面显示“本底测量出现故障”同时进行语音提示   
                        TxtShowResult.Text += "本底测量故障\r\n";                        
                        //语音提示故障
                        if (isEnglish)
                        {
                            player.Stream= Resources.English_Background_abnomal;
                        }
                        else
                        {
                            player.Stream =Resources.Chinese_Background_abnomal;
                        }
                        player.LoadAsync();
                        player.PlaySync();
                        //将故障信息errRecord写入数据库
                        AddErrorData(errRecordS);
                        //启动故障报警计时
                        alarmTimeStart = System.DateTime.Now.AddSeconds(1);
                        platformState = PlatformState.BackGrouneMeasure;
                    }
                    for (int i = 0; i < channelS.Count; i++)
                    {
                        //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次计算做准备
                        calculatedMeasureDataS[i].Alpha = 0;
                        calculatedMeasureDataS[i].Beta =0;
                    }
                    //重新启动本底测量计时
                    stateTimeStart = System.DateTime.Now.AddSeconds(1);
                }
                return;
            }
            //运行状态为开始测量
            if (platformState == PlatformState.Measuring)
            {
                if (isLoadProgressPic[3] == false)
                {
                    SetProgressPicFlag(3);
                    PnlReady.BackgroundImage = null;
                    //开始测量状态标签设置为当前状态图片
                    PnlMeasuring.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    LblTimeRemain.Parent = PnlMeasuring;//控制剩余时间标签显示位置
                    LblTimeRemain.BringToFront();
                    return;
                }
                if (isAudioPlayed == false)
                {
                    isAudioPlayed = true;
                    //系统语音提示仪器正常开始测量
                    if (isEnglish)
                    {
                        //测试结果区域显示开始测量
                        //TxtShowResult.Text += "Start counting\r\n";
                        player.Stream = Resources.English_Start_counting;// appPath + "\\Audio\\English_Start_counting.wav";
                    }
                    else
                    {
                        //测试结果区域显示开始测量
                        //TxtShowResult.Text += "开始测量\r\n";
                        player.Stream = Resources.Chinese_Start_counting;// appPath + "\\Audio\\Chinese_Start_counting.wav";
                    }
                    player.Load();
                    player.Play();
                    //重新启动计时，为开始测量及时准备
                    stateTimeStart = System.DateTime.Now.AddSeconds(-1);
                    return;
                    //Thread.Sleep(100);
                }
                //MeasureData conversionData = new MeasureData();
                //conversionData.Channel = new Channel();
                //IList<MeasureData> conversionDataS=new List<MeasureData>();
                //获得当前系统参数设置中的的测量时间并赋值给stateTimeSet
                //stateTimeSet = systemParameter.MeasuringTime;
                //在系统界面中显示正在测量倒计时时间（s）:系统设置测量时间-已经用时
                //stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                //LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s","0"): string.Format("{0,3}s",stateTimeRemain.ToString());                
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据经过平滑运算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();  
                    if(list.Count<=0)
                    {
                        continue;
                    }
                    if (list[0].Channel.ChannelID>=1 && list[0].Channel.ChannelID<=4 && list[0].InfraredStatus == 0)//手部红外状态未到位
                    {
                        //控制当前通道状态显示为未到位
                        //ChannelDisplayControl(list[0].Channel,1);修正注释
                        //进行文字和语音提示，该通道channelS[i].ChannelName不到位
                        if (list[0].Channel.ChannelID==1|| list[0].Channel.ChannelID==2)//左手
                        {
                            if(isEnglish)
                            {
                                TxtShowResult.Text += "Left hand moving, please measure again";
                                //系统语音提示左手移动重新测量
                                player.Stream = Resources.English_Left_hand_moved_please_measure_again;// appPath + "\\Audio\\English_Left_hand_moved_please_measure_again.wav";
                            }
                            else
                            {
                                TxtShowResult.Text += "左手移动,重新测量";
                                //系统语音提示左手移动重新测量
                                player.Stream = Resources.Chinese_Left_hand_moved_please_measure_again;// appPath + "\\Audio\\Chinese_Left_hand_moved_please_measure_again.wav";
                            }
                            player.Load();                            
                            player.PlaySync();
                            //Thread.Sleep(2000);
                        }
                        if (list[0].Channel.ChannelID == 3 || list[0].Channel.ChannelID ==4)
                        {
                            if (isEnglish)
                            {
                                TxtShowResult.Text += "Right hand moving, please measure again";
                                //系统语音提示左手移动重新测量
                                player.Stream = Resources.English_right_hand_moved_please_measure_again;// appPath + "\\Audio\\English_right_hand_moved_please_measure_again.wav";                                
                            }
                            else
                            {                                
                                TxtShowResult.Text += "右手移动,重新测量";
                                //系统语音提示右手移动重新测量
                                player.Stream = Resources.Chinese_right_hand_moved_please_measure_again;// appPath + "\\Audio\\Chinese_right_hand_moved_please_measure_again.wav";
                            }
                            player.LoadAsync();
                            player.PlaySync();                            
                        }
                        //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为等待测量过程中进行本底测量做准备
                        for (int j = 0; j < channelS.Count; j++)
                        {
                            calculatedMeasureDataS[j].Alpha = 0;
                            calculatedMeasureDataS[j].Beta = 0;                           
                        }
                        DisplayMeasureData(calculatedMeasureDataS,"cps");//各个通道显示单位恢复为cps
                        //设定当前运行状态为“等待测量”
                        platformState = PlatformState.ReadyToMeasure;
                        // 获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                        stateTimeSet = systemParameter.SmoothingTime;
                        //重新启动测量计时
                        stateTimeStart = System.DateTime.Now;
                        return;                        
                    }
                    //measureCount++;
                    //TxtShowResult.Text += measureCount.ToString()+"   ";
                    //控制每个通道状态显示为到位
                    //ChannelDisplayControl(list[0].Channel, 2);修正注释
                    //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)进行累加：                    
                    calculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    calculatedMeasureDataS[i].Beta += list[0].Beta;
                    //listS += list[0].Beta.ToString() + ";";
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                }
                //进行语音提示
                player.Stream = Resources.dida1;// appPath + "\\Audio\\dida1.wav";
                //player.LoadAsync();
                player.Play();
                if (stateTimeRemain == 0)
                {
                    player.Stream = Resources.dida2;// appPath + "\\Audio\\dida2.wav";
                    player.LoadAsync();
                    player.Play();
                }
                //测量时间到
                if (stateTimeRemain < 0)
                {
                    isAudioPlayed = false;
                    //PictureBox pictureBox;
                    //Panel panel;
                    Label label=null;
                    //计算每个通道的计数平均值,然后减去本底值
                    for (int i = 0; i < calculatedMeasureDataS.Count; i++)
                    {                        
                        if (calculatedMeasureDataS[i].Channel.ChannelID==7)//如果是衣物探头，则跳过继续
                        {
                            continue;
                        }
                        conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        //如果是单探测器且是第二次测量，则把手心数据赋值给手背,然后不做任何报警判断。
                        if (factoryParameter.IsDoubleProbe == false && isHandTested == 1)
                        {                            
                            List<MeasureData> list = null;
                            if (calculatedMeasureDataS[i].Channel.ChannelID == 5 || calculatedMeasureDataS[i].Channel.ChannelID == 6)//单探测器如果第二次测量跳过脚步判断
                            {
                                continue;
                            }
                            if (calculatedMeasureDataS[i].Channel.ChannelID == 1)//左手心道盒数据，存储到左手背
                            {
                                //找到calculatedMeasureDataS的左手背通道                                
                                list=calculatedMeasureDataS.Where(data => data.Channel.ChannelID == 2).ToList();                                                                                    
                            }
                            if (calculatedMeasureDataS[i].Channel.ChannelID == 3)//右手心道盒数据，存储到右手背
                            {
                                //找到calculatedMeasureDataS的右手背通道                                
                                list = calculatedMeasureDataS.Where(data => data.Channel.ChannelID == 4).ToList();                                
                            }
                            if (list!=null && list.Count > 0)
                            {
                                //找到手背通道对应的索引号
                                int index = calculatedMeasureDataS.IndexOf(list[0]);
                                //将手心通道数据拷贝到手背通道
                                Tools.Clone(calculatedMeasureDataS[i], calculatedMeasureDataS[index]);
                                continue;
                            }
                        }
                        //获得当前系统参数设置中的测量单位                                                
                        //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                        //IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == alphaNuclideUsed).ToList();
                        //conversionData.Alpha = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Alpha, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                        //efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == betaNuclideUsed).ToList();
                        //conversionData.Beta = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Beta, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                        //conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        //将单位转换后的测量数据添加进IList列表
                        //MeasureData conversionDataTemp = new MeasureData();
                        //Tools.Clone(conversionData, conversionDataTemp);
                        //Tools.Clone(conversionData.Channel, conversionDataTemp.Channel = new Channel());
                        //conversionDataS.Add(conversionDataTemp);
                        if (factoryParameter.MeasureType == "α" || factoryParameter.MeasureType == "α/β")
                        {
                            calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha / (stateTimeSet+1) - baseData[i].Alpha;
                            if (calculatedMeasureDataS[i].Alpha < 0)
                            {
                                calculatedMeasureDataS[i].Alpha = 0;
                            }
                            //测量单位转换
                            IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == alphaNuclideUsed).ToList();
                            conversionData.Alpha = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Alpha, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                            //获得当前检测通道的Alpha探测参数
                            IList<ProbeParameter> probeParmeterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && probeParmeter.NuclideType== "α").ToList();
                            //判断当前通道Alpha测量值是否超过报警阈值                       
                            if (calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_1 || calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_2)
                            {                                                                
                                float tempValue = calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_2 ? probeParmeterNow[0].Alarm_2 : probeParmeterNow[0].Alarm_1;
                                tempValue = Tools.UnitConvertCPSTo(tempValue, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                                //if (factoryParameter.IsDoubleProbe == false)//单探测器
                                //{
                                //    if (isHandTested == 0)//手部第一次测量（手心）、左脚、右脚
                                //    {                                        
                                //        pollutionRecord += string.Format("{0}测量值:α值{1}{2}设置值:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName, conversionData.Alpha.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //        pollutionRecord_E += string.Format("{0}Actual:Alpha Value{1}{2}Perset:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName_English, conversionData.Alpha.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"A"
                                //        Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                                //        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", calculatedMeasureDataS[i].Channel.ChannelName_English, "A")]);
                                //    }
                                //    if(isHandTested==1)//手部第二次检测，数据在手心，实际测量是手背
                                //    {
                                        
                                //        //将当前通道Alpha测量污染信息添加进pollutionRecord字符串
                                //        switch (calculatedMeasureDataS[i].Channel.ChannelID)
                                //        {
                                //            case 1://探测器接手心，所以左手心的值是对应是左手背                                                
                                //                pollutionRecord += string.Format("{0}测量值:α值{1}{2}设置值:{3}{4}\r\n", "左手背", conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit, tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                pollutionRecord_E += string.Format("{0}Actual:Alpha Value{1}{2}Preset:{3}{4}\r\n", "LHB", conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                //找到通道测量值显示Label控件
                                //                Panel panel = (Panel)(this.Controls["PnlLHB"]);
                                //                label = (Label)(panel.Controls["LblLHBA"]);
                                //                break;
                                //            case 3://探测器接手心，所以右手心的值是对应是右手背
                                //                pollutionRecord += string.Format("{0}测量值:α值{1}{2}设置值:{3}{4}\r\n", "右手背", conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                pollutionRecord_E += string.Format("{0}Actual:Alpha Value{1}{2}Preset:{3}{4}\r\n", "RHB", conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit, tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                //找到通道测量值显示Label控件
                                //                panel = (Panel)(this.Controls["PnlRHB"]);
                                //                label = (Label)(panel.Controls["LblRHBA"]);
                                //                break;
                                //            case 5://左脚右脚
                                //            case 6:
                                //                //将当前通道Alpha测量污染信息添加进pollutionRecord字符串
                                //                pollutionRecord += string.Format("{0}测量值:α值{1}{2}设置值:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName, conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                pollutionRecord_E += string.Format("{0}Actual:Alpha Value{1}{2}Preset:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName_English, conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"A"
                                //                panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                                //                label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", calculatedMeasureDataS[i].Channel.ChannelName_English, "A")]);
                                //                break;
                                //            case 2:
                                //            case 4:
                                //                continue;                                                
                                //        }                                        
                                        
                                //    }
                                //}
                                //else//双探测器
                                //{
                                    //将当前通道Alpha测量污染信息添加进pollutionRecord字符串
                                pollutionRecord += string.Format("{0}测量值:α值{1}{2}设置值:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName, conversionData.Alpha.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                pollutionRecord_E += string.Format("{0}Actual:Alpha Value{1}{2}Preset:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName_English, conversionData.Alpha.ToString("F1"), systemParameter.MeasurementUnit, tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"A"
                                Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                                label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", calculatedMeasureDataS[i].Channel.ChannelName_English, "A")]);
                                //}
                                if (calculatedMeasureDataS[i].Alpha > probeParmeterNow[0].Alarm_2)
                                {
                                    //当前通道测量结果显示文本框背景设置为Alarm2
                                    label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                                    //将设备监测状态设置为“污染”
                                    deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated_2);
                                }
                                else
                                {
                                    label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                                    //将设备监测状态设置为“污染”
                                    deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated_1);
                                }
                            }
                        }
                        if (factoryParameter.MeasureType == "β" || factoryParameter.MeasureType == "α/β")
                        {
                            //TxtShowResult.Text += calculatedMeasureDataS[i].Beta.ToString() + "\r\n";
                            //TxtShowResult.Text += "本："+baseData[i].Beta.ToString() + "\r\n";
                            calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta / (stateTimeSet+1) - baseData[i].Beta;
                            if (calculatedMeasureDataS[i].Beta < 0)
                            {
                                calculatedMeasureDataS[i].Beta = 0;
                            }
                            //测量单位转换
                            IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == betaNuclideUsed).ToList();
                            conversionData.Beta = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Beta, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                            //获得当前检测通道的Beta探测参数
                            IList<ProbeParameter> probeParmeterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && probeParmeter.NuclideType == "β").ToList();                            
                            if (calculatedMeasureDataS[i].Beta > probeParmeterNow[0].Alarm_1 || calculatedMeasureDataS[i].Beta > probeParmeterNow[0].Alarm_2)
                            {
                                float tempValue = calculatedMeasureDataS[i].Beta > probeParmeterNow[0].Alarm_2 ? probeParmeterNow[0].Alarm_2 : probeParmeterNow[0].Alarm_1;
                                tempValue = Tools.UnitConvertCPSTo(tempValue, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                                //if (factoryParameter.IsDoubleProbe == false)//单探测器
                                //{
                                //    if (isHandTested == 0)//手部第一次测量（手心）
                                //    {
                                //        //将当前通道Beta测量污染信息添加进pollutionRecord字符串
                                //        pollutionRecord += string.Format("{0}测量值:β值{1}{2}设置值:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName, conversionData.Beta.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //        pollutionRecord_E += string.Format("{0}Actual:Beta Value{1}{2}Preset:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName_English, conversionData.Beta.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //        //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"B"
                                //        Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                                //        label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", calculatedMeasureDataS[i].Channel.ChannelName_English, "B")]);
                                //        //Tools.Clone(calculatedMeasureDataS[i].Channel, conversionData.Channel);
                                //    }
                                //    if(isHandTested==1)//手部第二次检测，数据在手心，实际测量是手背
                                //    {
                                //        //将当前通道Beta测量污染信息添加进pollutionRecord字符串
                                //        switch(calculatedMeasureDataS[i].Channel.ChannelID)
                                //        {
                                //            case 1:
                                //                pollutionRecord += string.Format("{0}测量值:β值{1}{2}设置值:{3}{4}\r\n", "左手背", conversionData.Beta.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                pollutionRecord_E += string.Format("{0}Actual:Beta Value{1}{2}Preset:{3}{4}\r\n","LHB", conversionData.Beta.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                //找到通道测量值显示Label控件
                                //                Panel panel = (Panel)(this.Controls["PnlLHB"]);
                                //                label = (Label)(panel.Controls["LblLHBB"]);
                                //                //Tools.Clone(calculatedMeasureDataS[2].Channel, conversionData.Channel);//左手背道盒
                                //                break;
                                //            case 3:
                                //                pollutionRecord += string.Format("{0}测量值:β值{1}{2}设置值:{3}{4}\r\n", "右手背", conversionData.Beta.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                pollutionRecord_E += string.Format("{0}Actual:Beta Value{1}{2}Preset:{3}{4}\r\n", "RHB", conversionData.Beta.ToString("F1"), systemParameter.MeasurementUnit, tempValue.ToString("F1"),systemParameter.MeasurementUnit);
                                //                //找到通道测量值显示Label控件
                                //                panel = (Panel)(this.Controls["PnlRHB"]);
                                //                label = (Label)(panel.Controls["LblRHBB"]);
                                //                //Tools.Clone(calculatedMeasureDataS[4].Channel, conversionData.Channel);//右手背道盒
                                //                break;
                                //            case 5:
                                //            case 6:
                                //                //将当前通道Beta测量污染信息添加进pollutionRecord字符串
                                //                pollutionRecord += string.Format("{0}测量值:β值{1}{2}设置值:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName, conversionData.Beta.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                pollutionRecord_E += string.Format("{0}Actual:Beta Value{1}{2}Preset:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName_English, conversionData.Beta.ToString("F1"), systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                //                //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"B"
                                //                panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                                //                label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", calculatedMeasureDataS[i].Channel.ChannelName_English, "B")]);
                                //                //Tools.Clone(calculatedMeasureDataS[i].Channel, conversionData.Channel);
                                //                break;
                                //        }                                        
                                //    }
                                //}
                                //else//双探测器
                                //{
                                    
                                    //将当前通道Beta测量污染信息添加进pollutionRecord字符串
                                pollutionRecord += string.Format("{0}测量值:β值{1}{2}设置值:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName, conversionData.Beta.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"), systemParameter.MeasurementUnit);
                                pollutionRecord_E += string.Format("{0}Actual:Beta Value{1}{2}Preset:{3}{4}\r\n", calculatedMeasureDataS[i].Channel.ChannelName_English, conversionData.Beta.ToString("F1"),systemParameter.MeasurementUnit,tempValue.ToString("F1"),systemParameter.MeasurementUnit);
                                //找到通道测量值显示Label控件，其名字为：Lbl+通道英文名+"B"
                                Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                                label = (Label)(panel.Controls[string.Format("Lbl{0}{1}", calculatedMeasureDataS[i].Channel.ChannelName_English, "B")]);
                                    //Tools.Clone(calculatedMeasureDataS[i].Channel, conversionData.Channel);
                                //}
                                if (calculatedMeasureDataS[i].Beta > probeParmeterNow[0].Alarm_2)
                                {
                                    //当前通道测量结果显示文本框背景设置为Alarm2
                                    label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                                    deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated_2);
                                }
                                else
                                {
                                    label.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                                    deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated_1);
                                }
                            }
                        }
                        //if (factoryParameter.IsDoubleProbe == false&& isHandTested == 1)//单探测器，第二次检测，测试手背数据，但检测数据在手心，所以将手心道盒数据保存到手背通道
                        //{                            
                        //    if (calculatedMeasureDataS[i].Channel.ChannelID==1)//左手心道盒数据，存储到左手背
                        //    {
                        //        //conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        //        conversionDataS = conversionDataS.Where(data => data.Channel.ChannelID == 2).ToList();
                        //        Tools.Clone(calculatedMeasureDataS[i], conversionDataS[0]);
                        //        //Tools.Clone(conversionData.Channel, conversionDataS[0].Channel);
                        //    }
                        //    if(calculatedMeasureDataS[i].Channel.ChannelID==3)//右手心道盒数据，存储到右手背
                        //    {
                        //        //conversionData.Channel = calculatedMeasureDataS[3].Channel;
                        //        conversionDataS = conversionDataS.Where(data => data.Channel.ChannelID == 4).ToList();
                        //        Tools.Clone(calculatedMeasureDataS[i], conversionDataS[0]);
                        //        //Tools.Clone(conversionData.Channel, conversionDataS[3].Channel);
                        //    }  
                        //    if(calculatedMeasureDataS[i].Channel.ChannelID == 5||calculatedMeasureDataS[i].Channel.ChannelID == 6)
                        //    {
                        //        //conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        //        conversionDataS = conversionDataS.Where(data => data.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID).ToList();
                        //        Tools.Clone(calculatedMeasureDataS[i], conversionDataS[0]);
                        //        //Tools.Clone(conversionData.Channel, conversionDataS[i].Channel);
                        //    }                            
                        //}
                        //else
                        //{                        
                        //}
                        //双探测器或者是单探测器第一次测量除去手背或者是单探测器第二次测量的手背数据，则将当前单位转换后的探测数据添加进转换单位探测数据列表
                        if (factoryParameter.IsDoubleProbe == true || (factoryParameter.IsDoubleProbe == false && isHandTested == 0&& (conversionData.Channel.ChannelID != 2 && conversionData.Channel.ChannelID != 4))|| (factoryParameter.IsDoubleProbe == false && isHandTested == 1 && (conversionData.Channel.ChannelID==2||conversionData.Channel.ChannelID==4)))
                        {                            
                            //将单位转换后的测量数据添加进IList列表
                            MeasureData conversionDataTemp = new MeasureData();
                            Tools.Clone(conversionData, conversionDataTemp);
                            Tools.Clone(conversionData.Channel, conversionDataTemp.Channel = new Channel());
                            conversionDataS.Add(conversionDataTemp);                         
                        }                        
                        ////获得当前系统参数设置中的测量单位                                                
                        ////从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                        //IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == alphaNuclideUsed).ToList();
                        //conversionData.Alpha = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Alpha, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                        //efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == calculatedMeasureDataS[i].Channel.ChannelID && efficiencyParameter.NuclideName == betaNuclideUsed).ToList();
                        //conversionData.Beta = Tools.UnitConvertCPSTo(calculatedMeasureDataS[i].Beta, systemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedMeasureDataS[i].Channel.ProbeArea);
                        //conversionData.Channel = calculatedMeasureDataS[i].Channel;
                        ////将单位转换后的测量数据添加进IList列表
                        //MeasureData conversionDataTemp = new MeasureData();

                        //Tools.Clone(conversionData, conversionDataTemp);
                        //Tools.Clone(conversionData.Channel, conversionDataTemp.Channel=new Channel());
                        //conversionDataS.Add(conversionDataTemp);
                    }
                    ////按照系统参数单位要求显示最终测量结果,级显示单位转换后的conversionDataS列表值
                    //DisplayMeasureData(conversionDataS, systemParameter.MeasurementUnit);
                    if (factoryParameter.IsDoubleProbe == false)//单探测器检测，则探测器接到手心/手背一个道盒，所以手心手背的检测需分两次进行
                    {
                        //当前为手手部第一次检测完成
                        if (isHandTested == 0)//说明手部第一次检测刚刚完成
                        {
                            isHandTested = 1;//设置手部第一次检测完成，设置手部第二次检测标志1                               
                            //语音提示翻转手掌进行检测
                            if (isEnglish==false)
                            {
                                //提示翻转手掌
                                player.Stream = Resources.Chinese_Please_Flip_Palm_for_Measuring;// appPath + "\\Audio\\Chinese_Please_Flip_Palm_for_Measuring.wav";
                            }
                            else
                            {
                                player.Stream = Resources.English_please_flip_palm_for_measuring;// appPath + "\\Audio\\English_Please_Flip_Palm_for_Measuring.wav";
                            }
                            player.LoadAsync();
                            player.PlaySync();
                            if (isEnglish)
                            {
                                //测试结果区域显示等待测量
                                TxtShowResult.Text += "Ready\r\n";
                            }
                            else
                            {
                                //测试结果区域显示等待测量
                                TxtShowResult.Text += "等待测量\r\n";
                            }
                            //设备监测状态为正常
                            //deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                            //commPort.
                            //设置运行状态为等待测量
                            platformState = PlatformState.ReadyToMeasure;
                            // 获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                            stateTimeSet = systemParameter.SmoothingTime;
                            //重新启动测量计时 
                            stateTimeStart = System.DateTime.Now;
                            //返回
                            return;
                        }
                    }
                    if (pollutionRecord == null)//说明本次手脚测量无污染
                    {
                        //将本次测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次测量时计算做准备
                        for (int j = 0; j < channelS.Count; j++)
                        {
                            calculatedMeasureDataS[j].Alpha = 0;
                            calculatedMeasureDataS[j].Beta = 0;
                        }
                        if (measureDataS[6].Channel.IsEnabled == true)//衣物探头被启用
                        {
                            //    //检测完成，语音和文字提示“没有污染”
                            //    if (isEnglish)
                            //    {
                            //        //设备状态区域显示无污染
                            //        //测量结果显示区域提示没有污染，请进行衣物测量
                            //        TxtShowResult.Text += "No Contamination,Please measure the clothing!\r\n";
                            //        //player.Stream = Resources.English_NoContamination_please_frisker;// appPath + "\\Audio\\English_NoContamination_please_frisker.wav";
                            //    }
                            //    else
                            //    {
                            //        //测量结果显示区域提示没有污染，请进行衣物测量
                            //        TxtShowResult.Text += "没有污染，请进行衣物测量\r\n";
                            //        //player.Stream = Resources.Chinese_NoContamination_please_frisker;// appPath + "\\Audio\\Chinese_NoContamination_please_frisker.wav";
                            //    }
                        }
                        else//衣物探头未启用
                        {
                            //if(isEnglish==false)
                            //{
                            //    //设备状态区域显示无污染
                            //    //测量结果显示区域提示没有污染，请离开
                            //    TxtShowResult.Text += "没有污染，请离开!\r\n";
                            //    player.Stream = Resources.Chinese_NoContamination_please_leave;// appPath + "\\Audio\\Chinese_NoContamination_please_leave.wav";
                            //}
                            //else
                            //{
                            //    TxtShowResult.Text += "No Contamination,Please Leave!\r\n";
                            //    player.Stream = Resources.English_NoContamination_please_leave;// appPath + "\\Audio\\English_NoContamination_please_leave.wav";
                            //}
                            isTestedEnd = true;//检测已经完成
                            //运行状态设置为“测量结束”
                            platformState = PlatformState.Result;
                        }
                        //player.LoadAsync();
                        //player.PlaySync();
                        //仪器无污染状态背景色设置为无污染
                        PnlNoContamination.BackgroundImage = Resources.NoContamination_progress;// Image.FromFile(appPath + "\\Images\\NoContamination_progress.png");                        
                        PnlMeasuring.BackColor = Color.Transparent;
                        //设备监测状态为正常
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);                        
                        //Thread.Sleep(3000);
                    }
                    else //本次手脚测量有污染
                    {
                        //// 设备状态区域显示人员污染
                        //if (isEnglish)
                        //{                            
                        //    //测量结果显示区域提示被测人员污染，请去污
                        //    TxtShowResult.Text +=string.Format("Decontaminate, please!{0}\r\n", pollutionRecord_E);
                        //    //语音提示被测人员污染
                        //    player.Stream = Resources.English_Decontaminate_please;// appPath + "\\Audio\\English_Decontaminate_please.wav";
                        //}
                        //else
                        //{                            
                        //    //测量结果显示区域提示被测人员污染，请去污
                        //    TxtShowResult.Text +=string.Format("被测人员污染，请去污！{0}\r\n", pollutionRecord);
                        //    //语音提示被测人员污染
                        //    player.Stream = Resources.Chinese_Decontaminate_please;// appPath + "\\Audio\\Chinese_Decontaminate_please.wav";                            
                        //}
                        //设置状态显示区域背景色
                        PnlContaminated.BackgroundImage = Resources.Contaminated_progress;// Image.FromFile(appPath + "\\Images\\Contaminated_progress.png");
                        PnlMeasuring.BackColor = Color.Transparent;
                        PnlMeasuring.BackgroundImage = null;
                        ////将设备监测状态设置为“污染”
                        //deviceStatus = Convert.ToByte(DeviceStatus.OperatingContaminated);       
                        //player.LoadAsync();
                        //player.PlayLooping();
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
                    //按照系统参数单位要求显示最终测量结果,级显示单位转换后的conversionDataS列表值
                    DisplayMeasureData(conversionDataS, systemParameter.MeasurementUnit);
                    //清除转换后用于显示的监测数据存储列表conversionDataS为下次监测做准备
                    conversionDataS.Clear();
                    //本次检测完成，设置手部监测完成标志2
                    isHandTested = 2;
                    isHandSecondEnabled = false;
                    playControl = 0;
                    //更新测量次数（+1）              
                    systemParameter.MeasuredCount++;
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
                if (TxtShowResult.GetLineFromCharIndex(TxtShowResult.TextLength) > 16)
                {
                    int start = TxtShowResult.GetFirstCharIndexFromLine(0);
                    int end = TxtShowResult.GetFirstCharIndexFromLine(TxtShowResult.GetLineFromCharIndex(TxtShowResult.TextLength) - 16);
                    TxtShowResult.Select(start, end);
                    TxtShowResult.SelectedText = "";                   
                }                
                ClearProgressPicFlag();//检测完成，将窗体顶部状态图片加载标志全部设置为false                
                //扔掉5次预读取的数据
                if (throwDataCount < 1)
                {
                    for (int i = 0; i < measureDataS.Count; i++)
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
                if (pollutionRecord == null && isClothesContaminated == false)//手脚、衣物无污染
                {
                    if (isTestedEnd!=true)//监测未完成
                    {
                        //检测完成，语音和文字提示“没有污染”进行衣物测量
                        if (isEnglish)
                        {
                            player.Stream = Resources.English_NoContamination_please_frisker;// appPath + "\\Audio\\English_NoContamination_please_frisker.wav";
                        }
                        else
                        {
                            player.Stream = Resources.Chinese_NoContamination_please_frisker;// appPath + "\\Audio\\Chinese_NoContamination_please_frisker.wav";
                        }
                    }
                    else//监测完成，离开
                    {
                        if (isEnglish == false)
                        {                            
                            player.Stream = Resources.Chinese_NoContamination_please_leave;// appPath + "\\Audio\\Chinese_NoContamination_please_leave.wav";
                        }
                        else
                        {                            
                            player.Stream = Resources.English_NoContamination_please_leave;// appPath + "\\Audio\\English_NoContamination_please_leave.wav";
                        }                                                                                       
                    }
                    player.LoadAsync();
                    player.PlaySync();
                }
                if (deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_1) || deviceStatus == Convert.ToByte(DeviceStatus.OperatingContaminated_2))
                {
                    // 设备状态区域显示人员污染
                    if (isEnglish)
                    {
                        //语音提示被测人员污染
                        player.Stream = Resources.English_Decontaminate_please;// appPath + "\\Audio\\English_Decontaminate_please.wav";
                    }
                    else
                    {
                        //语音提示被测人员污染
                        player.Stream = Resources.Chinese_Decontaminate_please;// appPath + "\\Audio\\Chinese_Decontaminate_please.wav";                            
                    }
                    player.LoadAsync();
                    player.PlaySync();
                    //Thread.Sleep(200);
                }
                //检测次数大于强制本底次数、衣物离线时间大于设置时间、有污染（手脚、衣物）则强制本底
                if (systemParameter.MeasuredCount >= systemParameter.BkgUpdate || clothesTimeCount > systemParameter.ClothOfflineTime||pollutionRecord != null || isClothesContaminated == true)
                {                    
                    //如果有污染且报警时间长度小于设定报警时间长度，则返回等待
                    if ((pollutionRecord != null || isClothesContaminated == true) && ((DateTime.Now - alarmTimeStart).Seconds< alarmTimeSet)) 
                    {                        
                        return;
                    }
                    //转本底测量
                    else
                    {                        
                        pollutionRecord = null;//清空本次污染记录信息，为下一次测量做准备                                              
                        isClothesContaminated = false;                       
                        clothesTimeCount = 0;//重置衣物检测离线次数
                        deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                        // 运行状态标志设置为“本底测量”
                        platformState = PlatformState.BackGrouneMeasure;
                        //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                        stateTimeSet = systemParameter.SmoothingTime;
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS初始化为当前本底值，为本底测量时计算做准备
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        //系统状态显示区域显示本底测量
                        if (isEnglish)
                        {
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "Updating Background\r\n";
                            //系统提示本底测量
                            player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
                        }
                        else
                        {
                            //测试结果区域显示本底测量
                            TxtShowResult.Text += "本底测量\r\n";
                            //系统提示本底测量
                            player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
                        }
                        player.LoadAsync();
                        player.PlaySync();
                        //测量值显示标签背景恢复为默认状态（如果检查结果为人员污染，则会将测量值显示标签背景色变为污染报警，所以需要恢复）
                        for (int i = 0; i < channelS.Count; i++)
                        {
                            //通道测量值标签
                            if (channelS[i].ChannelID == 7)
                            {
                                LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
                            }
                            else
                            {
                                LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
                                LblValue[(channelS[i].ChannelID - 1) * 2 + 1].BackColor = Color.White;
                            }
                        }
                        ////启动本底测量计时 
                        stateTimeStart = System.DateTime.Now.AddSeconds(1);
                        //Thread.Sleep(1000);
                        return;
                    }                                                         
                }
                //if (isTestedEnd != true )//检测还未完成且未达到本底测量条件，应该进入等待测量状态。
                //{                    
                    //无污染状态设置为透明
                    //PnlNoContamination.BackColor = Color.Transparent;
                    //如果检测次数大于本底上限次数，或者存在污染，则强制进行本底测量。
                    //if (systemParameter.MeasuredCount >= systemParameter.BkgUpdate || pollutionRecord != null || isClothesContaminated == true)                        
                    //{
                    //    //设备监测状态为正常
                    //    //deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                    //    // 运行状态标志设置为“本底测量”
                    //    platformState = PlatformState.BackGrouneMeasure;
                    //    //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                    //    stateTimeSet = systemParameter.SmoothingTime;
                    //    //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                    //    for (int i = 0; i < channelS.Count; i++)
                    //    {
                    //        calculatedMeasureDataS[i].Alpha = 0;
                    //        calculatedMeasureDataS[i].Beta = 0;
                    //    }
                    //    //系统状态显示区域显示本底测量
                    //    if (isEnglish)
                    //    {
                    //        //LblShowStutas.Font = new Font("宋体", FONT_SIZE_E, FontStyle.Bold);
                    //        //LblShowStutas.Text = "Updating Background";
                    //        //测试结果区域显示本底测量
                    //        TxtShowResult.Text += "Updating Background\r\n";
                    //        //系统提示本底测量
                    //        player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
                    //    }
                    //    else
                    //    {
                    //        //测试结果区域显示本底测量
                    //        TxtShowResult.Text += "本底测量\r\n";
                    //        //系统提示本底测量
                    //        player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
                    //    }
                    //    player.LoadAsync();
                    //    player.PlaySync();
                    //    //启动本底测量计时 
                    //    stateTimeStart = System.DateTime.Now;
                    //    //Thread.Sleep(1000);
                    //    return;
                    //}                       
                    //系统状态显示区域显示等待测量
                  
                //if (isEnglish)
                //{
                //    //测试结果区域显示等待测量
                //    TxtShowResult.Text += "Ready\r\n";
                //    //系统语音提示仪器正常等待测量
                //    player.Stream = Resources.English_Ready;// appPath + "\\Audio\\English_Ready.wav";
                //}
                //else
                //{
                //    //测试结果区域显示等待测量
                //    TxtShowResult.Text += "等待测量\r\n";
                //    //系统语音提示仪器正常等待测量
                //    player.Stream = Resources.Chinese_Ready;// appPath + "\\Audio\\Chinese_Ready.wav";
                //}
                //player.LoadAsync();
                //player.PlaySync();
                //将本次测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次测量时计算做准备
                for (int j = 0; j < channelS.Count; j++)
                {
                    calculatedMeasureDataS[j].Alpha = 0;
                    calculatedMeasureDataS[j].Beta = 0;
                }                
                conversionDataS.Clear();
                isTestedEnd = false;//恢复检测完成状态标志为false，为下次检测做准备
                //设备监测状态为正常
                deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                //设置运行状态为等待测量
                platformState = PlatformState.ReadyToMeasure;
                // 获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                stateTimeSet = systemParameter.SmoothingTime;
                //重新启动测量计时 
                stateTimeStart = System.DateTime.Now;
                return;
                //}
                //else//检测完成
                //{                                        
                ////本次测量无污染
                //if (pollutionRecord == null && isClothesContaminated == false)//手脚、衣物无污染
                //{

                //}
                    //{                        
                    //无污染状态设置为透明
                    //PnlNoContamination.BackColor = Color.Transparent;
                    //如果测量人数大于系统设置的强制本底次数则，转到“本底测量”状态
                    //if (systemParameter.MeasuredCount >= systemParameter.BkgUpdate || clothesTimeCount > systemParameter.ClothOfflineTime)
                    //{

                    //    ////设备监测状态为正常
                    //    //deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                    //    // 运行状态标志设置为“本底测量”
                    //    //platformState = PlatformState.BackGrouneMeasure;
                    //    //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                    //    //stateTimeSet = systemParameter.SmoothingTime;
                    //    //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                    //    //for (int i = 0; i < channelS.Count; i++)
                    //    //{
                    //    //    calculatedMeasureDataS[i].Alpha = 0;
                    //    //    calculatedMeasureDataS[i].Beta = 0;
                    //    //}
                    //    //系统状态显示区域显示本底测量
                    //    //if (isEnglish)
                    //    //{
                    //    //    //LblShowStutas.Font = new Font("宋体", FONT_SIZE_E, FontStyle.Bold);
                    //    //    //LblShowStutas.Text = "Updating Background";
                    //    //    //测试结果区域显示本底测量
                    //    //    TxtShowResult.Text += "Updating Background\r\n";
                    //    //    //系统提示本底测量
                    //    //    player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
                    //    //}
                    //    //else
                    //    //{
                    //    //    //测试结果区域显示本底测量
                    //    //    TxtShowResult.Text += "本底测量\r\n";
                    //    //    //系统提示本底测量
                    //    //    player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
                    //    //}
                    //    //player.LoadAsync();
                    //    //player.PlaySync();
                    //    //启动本底测量计时 
                    //    //stateTimeStart = System.DateTime.Now;
                    //    //Thread.Sleep(1000);
                    //    //return;
                    //}
                    //else
                    //{
                    //系统状态显示区域显示等待测量
                    //if (isEnglish)
                    //{
                    //    //测试结果区域显示等待测量
                    //    TxtShowResult.Text += "Ready\r\n";
                    //    //系统语音提示仪器正常等待测量
                    //    player.Stream = Resources.English_Ready;// appPath + "\\Audio\\English_Ready.wav";
                    //}
                    //else
                    //{
                    //    //测试结果区域显示等待测量
                    //    TxtShowResult.Text += "等待测量\r\n";
                    //    //系统语音提示仪器正常等待测量
                    //    player.Stream = Resources.Chinese_Ready;// appPath + "\\Audio\\Chinese_Ready.wav";
                    //}
                    //player.LoadAsync();
                    //player.PlaySync();
                    ////将本次测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为下次测量时计算做准备
                    //for (int j = 0; j < channelS.Count; j++)
                    //{
                    //    calculatedMeasureDataS[j].Alpha = 0;
                    //    calculatedMeasureDataS[j].Beta = 0;
                    //}
                    //Thread.Sleep(3000);
                    //设备监测状态为正常
                    //deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                    //commPort.
                    //设置运行状态为等待测量
                    //platformState = PlatformState.ReadyToMeasure;
                    // 获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                    //stateTimeSet = systemParameter.SmoothingTime;
                    //重新启动测量计时 
                    //stateTimeStart = System.DateTime.Now;
                    //return;
                    //}
                    //}
                    //else//有污染
                    //{
                    //if ((DateTime.Now - alarmTimeStart).Seconds < alarmTimeSet) //达到报警时间长度
                    //{                            
                    //pollutionRecord = null;//清空本次污染记录信息，为下一次测量做准备
                    //人员污染状态设置为透明
                    //PnlContaminated.BackColor = Color.Transparent;
                    //设备监测状态为正常
                    //本次测量完成，恢复衣物探头污染状态标志为false无污染
                    //isClothesContaminated = false;
                    //deviceStatus = Convert.ToByte(DeviceStatus.OperatingNormally);
                    //// 运行状态标志设置为“本底测量”
                    //platformState = PlatformState.BackGrouneMeasure;
                    ////获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                    //stateTimeSet = systemParameter.SmoothingTime;
                    ////将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                    //for (int i = 0; i < channelS.Count; i++)
                    //{
                    //    calculatedMeasureDataS[i].Alpha = 0;
                    //    calculatedMeasureDataS[i].Beta = 0;
                    //}
                    ////系统状态显示区域显示本底测量
                    //if (isEnglish)
                    //{
                    //    //测试结果区域显示本底测量
                    //    TxtShowResult.Text += "Updating Background\r\n";
                    //    //系统提示本底测量
                    //    player.Stream = Resources.English_Updating_background;// appPath + "\\Audio\\English_Updating_background.wav";
                    //}
                    //else
                    //{
                    //    //测试结果区域显示本底测量
                    //    TxtShowResult.Text += "本底测量\r\n";
                    //    //系统提示本底测量
                    //    player.Stream = Resources.Chinese_Background_measure;// appPath + "\\Audio\\Chinese_Background_measure.wav";
                    //}
                    //player.LoadAsync();
                    //player.PlaySync();
                    ////测量值显示标签背景恢复为默认状态（如果检查结果为人员污染，则会将测量值显示标签背景色变为污染报警，所以需要恢复）
                    //for (int i = 0; i < channelS.Count; i++)
                    //{
                    //    //通道测量值标签
                    //    if (channelS[i].ChannelID == 7)
                    //    {
                    //        LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
                    //    }
                    //    else
                    //    {
                    //        LblValue[(channelS[i].ChannelID - 1) * 2].BackColor = Color.White;
                    //        LblValue[(channelS[i].ChannelID - 1) * 2 + 1].BackColor = Color.White;
                    //    }
                    //}
                    //////启动本底测量计时 
                    //stateTimeStart = System.DateTime.Now.AddSeconds(1);
                    //Thread.Sleep(1000);
                    //    return;
                    //}
                    //}                      
                    //}
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
                    status = 0;
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
            bool isSelfCheckFault = false;//仪器自检是否出现故障标志
            //故障记录字符串
            string errRecord = null;//中文
            string errRecord_E = null;//英文  
            string errRecordOfChannel = null;
            string errRecordOfChannel_E = null;          
            for (int i = 0; i < channelS.Count; i++)//遍历全部启用通道
            {                               
                //如果是单探测器，则将左右手背跳过不进行判断
                if(factoryParameter.IsDoubleProbe==false)
                {
                    if(channelS[i].ChannelID==2||channelS[i].ChannelID==4)
                    {
                        continue;
                    }
                }
                //获得当前通道的道盒参数,从全部道盒参数列表channelParameterS中找到当前通道的道盒参数
                //ChannelParameter channelParameter = new ChannelParameter();
                //channelParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID);
                if (platformState == PlatformState.SelfTest)
                {
                    if (calculatedMeasureDataS[i].Channel.ChannelID == 7)//对衣物探头不做判断
                    {
                        continue;
                    }
                    IList<ChannelParameter>channelParameterNow= channelParameterS.Where(channelParameter=>channelParameter.Channel.ChannelID== calculatedMeasureDataS[i].Channel.ChannelID).ToList();                
                    //判断当前高压值：当前高压值若为系统设定值的0.8~1.2倍之内，则该通道高压正常，否则高压故障,将故障信息添加到errRecord字符串，isCheck = false;
                    if (calculatedMeasureDataS[i].HV< channelParameterNow[0].PresetHV * (1 - PlatForm.ErrorRange.HV_ERROR) || calculatedMeasureDataS[i].HV > channelParameterNow[0].PresetHV * (1 + PlatForm.ErrorRange.HV_ERROR))
                    {
                    //高压故障,将故障信息添加到errRecord字符串
                    errRecordOfChannel += string.Format("{0}高压故障,设置值:{1}V,实测值:{2}V;", calculatedMeasureDataS[i].Channel.ChannelName,channelParameterNow[0].PresetHV.ToString("F1"), calculatedMeasureDataS[i].HV.ToString("F1"));
                    errRecordOfChannel_E += string.Format("{0}HV Fault,Preset:{1}V,Actual:{2}V;",calculatedMeasureDataS[i].Channel.ChannelName_English,channelParameterNow[0].PresetHV.ToString("F1"), calculatedMeasureDataS[i].HV.ToString("F1"));
                    //设置isCheck为false
                    isCheck = false;
                    }
                
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
                        if(calculatedMeasureDataS[i].Alpha< BASE_DATA * (1-PlatForm.ErrorRange.BASE_ERROR)||calculatedMeasureDataS[i].Alpha> BASE_DATA * (1+PlatForm.ErrorRange.BASE_ERROR))
                        {
                            //将故障信息添加到error字符串
                            errRecordOfChannel += string.Format("{0}α电子线路故障;",calculatedMeasureDataS[i].Channel.ChannelName);
                            errRecordOfChannel_E += string.Format("{0}Alpha Channel Fault;", calculatedMeasureDataS[i].Channel.ChannelName_English);
                            //设置isCheck为false
                            isCheck = false;
                        }
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        if (calculatedMeasureDataS[i].Beta < BASE_DATA * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataS[i].Beta > BASE_DATA * (1 + PlatForm.ErrorRange.BASE_ERROR))
                            //对Beta本底值(BASE_DATA)进行判断，如果故障提示“β线路故障”同时将故障信息添加到errRecord字符串,isCheck = false;
                           // if (calculatedMeasureDataS[i].Beta < channelParameterNow[0].BetaThreshold * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataS[i].Beta > channelParameterNow[0].BetaThreshold * (1 + PlatForm.ErrorRange.BASE_ERROR))
                        {
                            //将故障信息添加到error字符串
                            errRecordOfChannel += string.Format("{0}β电子线路故障;", calculatedMeasureDataS[i].Channel.ChannelName);
                            errRecordOfChannel_E += string.Format("{0}Beta Channel Fault;", calculatedMeasureDataS[i].Channel.ChannelName_English);
                            //设置isCheck为false
                            isCheck = false;
                        }
                    }
                    //判断根据红外状态判断该通道红外是否故障，自检时无检查体但提示到位，说明红外故障
                    if (calculatedMeasureDataS[i].InfraredStatus == 1)//红外状态到位
                    {
                        //红外故障,将故障信息添加到errRecord字符串
                        errRecordOfChannel += string.Format("{0}红外故障;", calculatedMeasureDataS[i].Channel.ChannelName);
                        errRecordOfChannel_E += string.Format("{0}Sensor fault;", calculatedMeasureDataS[i].Channel.ChannelName_English);
                        isCheck = false;
                    }                                
                }
                if (platformState == PlatformState.BackGrouneMeasure|| platformState==PlatformState.ReadyToMeasure)
                {
                   // ProbeParameter probeParameter = new ProbeParameter();
                    if (factoryParameter.MeasureType != "β"&& channelS[i].ChannelID!=7)
                    {
                        //查询当前通道的α本底上限、本底下限（从探测参数列表中找到当前通道的"α"探测参数）                       
                        IList<ProbeParameter> probeParameterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channelS[i].ChannelID && probeParmeter.NuclideType== "α").ToList();
                        //probeParameter.GetParameter(channelS[i].ChannelID, "α");
                        //进行α本底测量判断
                        if (calculatedMeasureDataS[i].Alpha < probeParameterNow[0].LBackground) //超过当前通道的本底下限
                        {
                            //该通道channelS[i].ChannelName的本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecordOfChannel += string.Format("{0}α本底下限值:{1}cps,当前本底值:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName, probeParameterNow[0].LBackground.ToString("F1"),calculatedMeasureDataS[i].Alpha.ToString("F1"));
                            errRecordOfChannel_E += string.Format("{0} α Low Background Threshold{1}cps,Actual Background:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName_English, probeParameterNow[0].LBackground.ToString("F1"), calculatedMeasureDataS[i].Alpha.ToString("F1"));
                            isCheck = false;
                        }
                        if (calculatedMeasureDataS[i].Alpha >= probeParameterNow[0].HBackground)//超过当前通道的本底上限
                        {
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecordOfChannel += string.Format("{0}α本底上限值:{1}cps,当前本底值:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName, probeParameterNow[0].HBackground.ToString("F1"), calculatedMeasureDataS[i].Alpha.ToString("F1"));
                            errRecordOfChannel_E += string.Format("{0} α High Background Threshold{1}cps,Actual Background:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName_English, probeParameterNow[0].HBackground.ToString("F1"), calculatedMeasureDataS[i].Alpha.ToString("F1"));
                            isCheck = false;
                        }
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        //查询当前通道的β本底上限、本底下限
                        //probeParameter.GetParameter(channelS[i].ChannelID, "β");
                        //查询当前通道的β本底上限、本底下限（从探测参数列表中找到当前通道的"β"探测参数）                       
                        IList<ProbeParameter> probeParameterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channelS[i].ChannelID && (probeParmeter.NuclideType == "β"|| probeParmeter.NuclideType == "C")).ToList();
                        if (calculatedMeasureDataS[i].Beta < probeParameterNow[0].LBackground)//超过当前通道的本底下限
                        {
                            //该通道channelS[i].ChannelName本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecordOfChannel += string.Format("{0} β本底下限值:{1}cps,当前本底值:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName, probeParameterNow[0].LBackground.ToString("F1"), calculatedMeasureDataS[i].Beta.ToString("F1"));
                            errRecordOfChannel_E += string.Format("{0} β Low Background Threshold{1}cps,Actual Background:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName_English, probeParameterNow[0].LBackground.ToString("F1"), calculatedMeasureDataS[i].Beta.ToString("F1"));
                            isCheck = false;
                        }
                        if (calculatedMeasureDataS[i].Beta >= probeParameterNow[0].HBackground)//超过当前通道的本底上限
                        {
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecordOfChannel += string.Format("{0}β本底上限值:{1}cps,当前本底值:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName, probeParameterNow[0].HBackground.ToString("F1"), calculatedMeasureDataS[i].Beta.ToString("F1"));
                            errRecordOfChannel_E += string.Format("{0} β High Background Threshold{1}cps,Actual Background:{2}cps;", calculatedMeasureDataS[i].Channel.ChannelName_English, probeParameterNow[0].HBackground.ToString("F1"), calculatedMeasureDataS[i].Beta.ToString("F1"));
                            isCheck = false;
                        }
                    }
                }
                //根据是否检测通过设置该通道的背景颜色                
                if (errRecordOfChannel != null)//故障信息不为空
                {                    
                    if (platformState == PlatformState.SelfTest)//当前为仪器自检状态
                    {                        
                        isSelfCheckFault = true;//当前通道自检故障标志
                        if (isEnglish)
                        {
                            LblCheck.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Bold);
                            LblCheck.Text = "Fault";
                        }
                        else
                        {
                            LblCheck.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Bold);
                            LblCheck.Text = "仪器故障";
                        }
                    }
                    if (isEnglish != true)
                    {
                        TxtShowResult.Text += errRecordOfChannel + "\r\n";
                    }
                    else
                    {
                        TxtShowResult.Text +=errRecordOfChannel_E + "\r\n";
                    }
                    //对应通道名字文本框背景色显示为ERROR
                    if (calculatedMeasureDataS[i].Channel.ChannelID != 7)//衣物探头除外
                    {
                        Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                        ((Label)(panel.Controls[string.Format("Lbl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.CORLOR_ERROR;
                    }
                }
                else
                {
                    //恢复仪器自检状态背景
                    PnlSelfCheck.BackColor = Color.Transparent;
                    //对应通道名字文本框背景色显示为不到位                                  修正注释
                    //if (calculatedMeasureDataS[i].Channel.ChannelID != 7)//衣物探头除外
                    //{
                    //    ChannelDisplayControl(calculatedMeasureDataS[i].Channel, 1);                        
                    //}
                    //对应通道名字文本框背景色显示为正常
                    if (calculatedMeasureDataS[i].Channel.ChannelID != 7)//衣物探头除外
                    {
                        Panel panel = (Panel)(this.Controls[string.Format("Pnl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)]);
                        ((Label)(panel.Controls[string.Format("Lbl{0}", calculatedMeasureDataS[i].Channel.ChannelName_English)])).BackColor = PlatForm.ColorStatus.COLOC_BKNORMAL;
                    }
                }
                errRecord += errRecordOfChannel;//将当前通道错误记录添加到整体错误记录字符串
                errRecord_E += errRecordOfChannel_E;
                errRecordOfChannel = null;//清空当前通道错误记录字符串
                errRecordOfChannel_E = null;
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
                try
                {
                    receiveBuffMessage = Components.Message.ReceiveMessage(commPort_Supervisory);
                }
                catch
                {
                    TxtShowResult.Text += "管理机端口通信错误！\r\n";
                    isCommReportError = true;
                }
                //延时
                Thread.Sleep(1000);
                //触发向主线程返回下位机上传数据事件
                if (receiveBuffMessage.Count() >= 8)//报文长度大于最小报文长度
                {
                    isCommReportError = false;
                    worker.ReportProgress(1, receiveBuffMessage);
                }
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
            //接收报文数据为空，说明没有收到管理机下发的命令
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 2)
                {
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
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            message = Components.Message.ExplainMessage(receiveBufferMessage);            
            if (message == null||message.Length<=0)//解析失败
            {
                return;
            }
            isCommReportError = false;
            //解析成功
            if (message.Count()>=7)//长度大于1，为时间同步命令
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
                    byte[] deviceStatusMessage=null;
                    //从数据库中查询最近一次记录的测量数据
                    MeasureData measureData = new MeasureData();
                    measureData.GetLatestData();
                    //从数据库中查询最近一次记录的故障数据
                    ErrorData errorData = new ErrorData();
                    errorData.GetLatestData();
                    //测量数据和故障数据都为空，说明仪器正常
                    if(measureData==null && errorData==null)
                    {
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), DateTime.Now, 0x01);//0x01:仪器正常
                    }                    
                    //监测数据和故障数据都已经上报，说明最近仪器正常，上报时间为当前时间
                    if(measureData.IsReported==true && errorData.IsReported==true)
                    {
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), DateTime.Now, 0x01);//0x01:仪器正常
                    }
                    //监测数据上报，故障数据未上报，说明最近仪器故障，上报完成后更新故障数据状态为已上报
                    if((measureData==null||measureData.IsReported==true) && errorData.IsReported==false)
                    {
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), errorData.ErrTime, 0x02);//0x02:仪器故障
                    }
                    //监测数据未上报，故障数据上报，说明最近状态为污染，上报完成后更新监测数据状态为已上报
                    if(measureData.IsReported==false && (errorData.IsReported==true||errorData==null))
                    {
                        deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress),measureData.MeasureDate, 0x04);//0x04:仪器污染
                    }
                    //监测数据和故障数据都未上报，则将最近的状态进行上报，更新两条记录为已上报
                    if (measureData.IsReported == false && errorData.IsReported == false)
                    {
                        if (measureData.MeasureDate > errorData.ErrTime)//最近一次记录为MeasureData，说明是状态为污染（因为只有污染状态才会记录，正常不记录）
                        {
                            //生成上报管理机的监测仪测试状态(污染)报文                    
                            deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress), measureData.MeasureDate, 0x04);//仪器状态为污染
                        }
                        else
                        {
                            //生成上报管理机的监测仪测试状态（故障）报文                    
                            deviceStatusMessage = Components.Message.BuildMessage(Convert.ToInt32(factoryParameter.DeviceAddress),errorData.ErrTime, 0x02);//仪器状态为故障
                        }
                    }
                    //向管理机上报仪器检测状态
                    if(Components.Message.SendMessage(deviceStatusMessage, commPort_Supervisory))//上报成功
                    {
                        //数据库中更新上报标志
                        if(measureData!=null)
                        {
                            //更新上报标志
                            measureData.UpdataReported(true, measureData.MeasureID);
                        }
                        if(errorData!=null)
                        {
                            //更新上报标志
                            errorData.UpdateReported(true, errorData.ErrID);
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
            }
        }        
        private void TmrDispTime_Tick(object sender, EventArgs e)
        {
            //更新当前显示时间
            LblTime.Text = DateTime.Now.ToLongTimeString();
            //监测串口状态，如果串口关闭则打开
            if (bkWorkerReceiveData.IsBusy)
            {
                if (isCommError||commPort.Opened==false)//监测端口通讯错误
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
                    catch
                    {
                            return;
                    }                    
                }
            }
            if (bkWorkerReportStatus.IsBusy)
            {
                if (isCommReportError)//上报状态端口通讯错误
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
            ////根据当前红外状态控制左右手及衣物红外状态显示
            //foreach (MeasureData usedChannelData in calculatedMeasureDataS)
            //{
            //    switch (usedChannelData.Channel.ChannelID)
            //    {
            //        case 1:
            //        case 2:
            //            if (usedChannelData.InfraredStatus == 1)//红外到位
            //            {
            //                if(isEnglish)
            //                {
            //                    LblLeft.Text = "LH in place";
            //                }
            //                else
            //                {
            //                    LblLeft.Text = "左手到位";
            //                }                            
            //                LblLeft.BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
            //                LblLeft.ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
            //            }
            //            else
            //            {
            //                if (isEnglish)
            //                {
            //                    LblLeft.Text = "LH not in place";
            //                }
            //                else
            //                {
            //                    LblLeft.Text = "左手不到位";
            //                }
            //                LblLeft.BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
            //                LblLeft.ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
            //            }
            //            break;
            //        case 3:
            //        case 4:
            //            if (usedChannelData.InfraredStatus == 1)//红外到位
            //            {
            //                if (isEnglish)
            //                {
            //                    LblRight.Text = "RH in place";
            //                }
            //                else
            //                {
            //                    LblRight.Text = "右手到位";
            //                }
            //                LblRight.BackColor = PlatForm.ColorStatus.CORLOR_BKINPLACE;
            //                LblRight.ForeColor = PlatForm.ColorStatus.CORLOR_FRNORMAL;
            //            }
            //            else
            //            {
            //                if (isEnglish)
            //                {
            //                    LblRight.Text = "RH not in place";
            //                }
            //                else
            //                {
            //                    LblRight.Text = "右手不到位";
            //                }
            //                LblRight.BackColor = PlatForm.ColorStatus.CORLOR_BKNOTINPLACE;
            //                LblRight.ForeColor = PlatForm.ColorStatus.CORLOR_FRNOTINPLACE;
            //            }
            //            break;
            //        case 7:
            //            if (usedChannelData.InfraredStatus == 1)//红外到位
            //            {
            //                PicFrisker.BackgroundImage = Image.FromFile(appPath + "\\Images\\Frisker_InPlace.png");
            //            }
            //            else
            //            {
            //                PicFrisker.BackgroundImage = Image.FromFile(appPath + "\\Images\\Frisker_NotInPlace.png");
            //            }
            //            break;
            //    }
            //}            
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

        private void BtnOption_Click(object sender, EventArgs e)
        {            
            FrmEnterPassword frmEnterPassword = new FrmEnterPassword();           
            if (frmEnterPassword.ShowDialog()==DialogResult.OK)
            {
                bool isOpened = false;
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
                //bkWorkerReportStatus.Dispose();
                //Thread.Sleep(100);
                //bkWorkerReceiveData.Dispose();
                //Thread.Sleep(100);
                //while (this.TmrDispTime.Enabled)
                //{
                //    this.TmrDispTime.Stop();
                //}
                //while (this.commPort.Opened == true)
                //{
                //    this.commPort.Close();                    
                //    Thread.Sleep(100);                    
                //}
                //while (this.commPort_Supervisory.Opened == true)
                //{
                //    this.commPort_Supervisory.Close();
                //    Thread.Sleep(100);
                //}
                #region 打开窗体操作                               
                FrmMain frmMain = new FrmMain(commPort);
                frmMain.Show();
                this.Hide();
                //try
                //{
                //    FrmMain frmMain = new FrmMain();
                //}
                //catch (Exception exception)
                //{
                //    Console.WriteLine(exception);
                //    throw;
                //}
                //if (isEnglish)
                //{
                //    MessageBox.Show("Login Successful!", "Success");
                //}
                //else
                //{
                //    MessageBox.Show("用户登录成功！", "成功");
                //}
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
                        ChannelDisplayControl(channelsAll[i], 1);
                    }
                    else
                    {
                        //界面中相关通道控件Enabled属性设置为false，背景色设置为屏蔽
                        ChannelDisplayControl(channelsAll[i], 0);
                    }
                }
                DisplayInit();
                //显示当前剩余时间
                LblTimeRemain.Text = stateTimeRemain < 0 ? string.Format("{0,3}s", "0") : string.Format("{0,3}s", stateTimeRemain.ToString());
                //根据当前运行状态设置顶部进度状态图片
                switch (platformState)
                {                                                            
                    case PlatformState.SelfTest:
                        //仪器自检状态标签设置为当前状态图片
                        PnlSelfCheck.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                        //取消本底测量状态图片
                        PnlBackground.BackgroundImage = null;
                        PnlReady.BackgroundImage = null;
                        PnlMeasuring.BackgroundImage = null;
                        break;
                    case PlatformState.BackGrouneMeasure:                        
                        PnlSelfCheck.BackgroundImage = null;
                        PnlBackground.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                        PnlReady.BackgroundImage = null;
                        PnlMeasuring.BackgroundImage = null;
                        break;
                    case PlatformState.ReadyToMeasure:
                        PnlSelfCheck.BackgroundImage = null;
                        PnlBackground.BackgroundImage = null;
                        PnlReady.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                        PnlMeasuring.BackgroundImage = null;
                        break;
                    case PlatformState.Measuring:
                        PnlSelfCheck.BackgroundImage = null;
                        PnlBackground.BackgroundImage = null;
                        PnlReady.BackgroundImage = null;
                        PnlMeasuring.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                        break;
                    case PlatformState.Result:
                        PnlSelfCheck.BackgroundImage = null;
                        PnlBackground.BackgroundImage = null;
                        PnlReady.BackgroundImage = null;
                        PnlMeasuring.BackgroundImage = null ;
                        break;
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
        //protected override void OnVisibleChanged(EventArgs e)
        //{
        //    base.OnVisibleChanged(e);
        //    if (!IsHandleCreated)
        //    {
        //        this.Close();
        //    }
        //}

        private void FrmMeasureMain_VisibleChanged(object sender, EventArgs e)
        {            
            commPort.ClearPortData();
            if (isFrmDisplayed == false)
            {
                errNumber = 0;
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
                TxtShowResult.Text = "";                
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
                channelS.CopyTo(channelsAll, 0);//将全部通道信息复制到channelsAll中
                
                //初始化显示界面
                DisplayInit();
                LblTimeRemain.Text = string.Format("{0}s", systemParameter.SelfCheckTime.ToString());
               
                checkTime = systemParameter.SelfCheckTime;//检测时间 
                alarmTimeSet = systemParameter.AlarmTime;//报警时间                                  
                //在界面中将启用通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，其它通道（通道未启用或探测面积为0）的控件Enabled无效；                       
                for (int i = 0; i < channelS.Count; i++)
                {
                    if (channelS[i].IsEnabled == true && channelS[i].ProbeArea != 0)//通道被启用且探测面积不为0
                    {
                        //界面中相关通道控件Enabled设置为true，背景色设置为正常
                        ChannelDisplayControl(channelS[i], 1);
                    }
                    else
                    {
                        //界面中相关通道控件Enabled属性设置为false，背景色设置为屏蔽
                        ChannelDisplayControl(channelS[i], 0);
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
                calculatedMeasureDataS.Clear();
                //根据有效通道对象初始化用来存储最终监测数据的列表
                foreach (Channel usedChannel in channelS)
                {
                    MeasureData measureData = new MeasureData();
                    measureData.Channel = usedChannel;
                    calculatedMeasureDataS.Add(measureData);
                }
                //初始化左手、右手和衣物的红外状态，默认都不到位
                for (int i = 0; i < 3; i++)
                {
                    lastInfraredStatus[i] = 0;
                }
                //正常使用的通道测量数值显示（初始值为0cps）
                DisplayMeasureData(calculatedMeasureDataS, "cps");

                isSelfCheckSended = false;
                isBetaCommandToSend = false;
                //将运行状态标志设置为“运行准备”
                platformState = PlatformState.ReadyToRun;
                //当前系统状态时间为自检时间
                stateTimeSet = systemParameter.SelfCheckTime;
                stateTimeStart = DateTime.Now.AddSeconds(1);               
                if (this.bkWorkerReceiveData.IsBusy == false)
                {
                    this.bkWorkerReceiveData.RunWorkerAsync();
                }
                if (commPort_Supervisory.IsEnabled == true && this.bkWorkerReportStatus.IsBusy == false)
                {
                    this.bkWorkerReportStatus.RunWorkerAsync();
                }
                //将存储各个通道测量计算结果的列表calculatedMeasureDataS清零，重新计数做准备
                //for (int i = 0; i < channelS.Count; i++)
                //{
                //    calculatedMeasureDataS[i].Alpha = 0;
                //    calculatedMeasureDataS[i].Beta = 0;
                //}
                deviceStatus =Convert.ToByte(DeviceStatus.OperatingNormally);
                //恢复仪器自检状态背景
                PnlSelfCheck.BackColor = Color.Transparent;                
                if (isLoadProgressPic[0] == false||LblCheck.Text=="仪器故障")
                {
                    SetProgressPicFlag(0);//仪器自检进度图片已经被加载标志设置为true，其它为false
                    //恢复当前标签文字提示（自检过程中可能有故障提示，所以自检通过后需要重新恢复）
                    if (isEnglish == false)
                    {
                        LblCheck.Font = new Font("微软雅黑", FONT_SIZE, FontStyle.Italic);
                        LblCheck.Text = "仪器自检";
                    }
                    else
                    {
                        LblCheck.Font = new Font("微软雅黑", FONT_SIZE_E, FontStyle.Italic);
                        LblCheck.Text = "Self-Checking";
                    }
                    //仪器自检状态标签设置为进度图片
                    PnlSelfCheck.BackgroundImage = Resources.progress;// Image.FromFile(appPath + "\\Images\\progress.png");
                    //取消本底测量状态标签设置
                    PnlBackground.BackgroundImage = null;
                    //取消等待测量状态标签设置
                    PnlReady.BackgroundImage = null;
                    PnlMeasuring.BackgroundImage = null;
                    PnlNoContamination.BackgroundImage = null;
                    PnlContaminated.BackgroundImage = null;                    
                    LblTimeRemain.Parent = PnlSelfCheck;//控制剩余时间标签显示位置                                
                    LblBackground.BringToFront();
                    LblTimeRemain.BringToFront();
                    LblTimeRemain.BackColor = Color.Transparent;                    
                }
                //stateTimeStart = DateTime.Now;//重新启动计时
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
    }
}

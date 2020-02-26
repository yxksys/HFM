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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using HFM.Components;


namespace HFM
{
    public partial class FrmMeasure : Form
    {
        CommPort commPort = new CommPort();
        //工厂参数
        FactoryParameter factoryParameter = new FactoryParameter();
        //当前可使用的检测通道
        IList<Channel> channelS = new List<Channel>();
        //运行状态枚举类型
        enum PlatformState
        {
            ReadyToRun=0,
            SelfTest=1,
            BackGrouneMeasure=2,
            ReadyToMeasure=3,
            Measuring=4,
            Result=5
        }
        //运行状态标志
        PlatformState platformState;
        const int BASE_DATA= 1000;
        //存储各个通道最终计算检测值的List
        IList<MeasureData> calculatedMeasureDataS=new List<MeasureData>();
        int stateTimeSet = 0;//系统当前运行状态的检测时间设置
        //存储本底计算结果，用例对测量数据进行校正
        IList<MeasureData> baseData = new List<MeasureData>();
        public FrmMeasure()
        {
            InitializeComponent();
        }

        private void FrmMeasure_Load(object sender, EventArgs e)
        {           
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
            //从配置文件获得当前串口配置
            if(commPort.Opened==true)
            {
                commPort.Close();
            }
            commPort.GetCommPortSet();
            //打开串口
            try
            {
                commPort.Open();
            }
            catch
            {                
                MessageBox.Show("端口打开错误！请检查通讯是否正常。");
                return;
            }
            //加载相关图片资源（设备厂家图标、手部脚步图片、衣物图片等）
            // 
            //
            //在界面中显示当前系统时间
            //
            //
            //获得系统参数信息            
            factoryParameter.GetParameter();
            //在界面中显示“仪器名称”、“仪器编号”、“IP地址及端口”等信息
            //
            //
            //根据探测类型控制显示界面
            switch (factoryParameter.MeasureType)
            {
                case "α":                    
                    //设置显示界面状态
                    //
                    //
                    break;
                case "β":                   
                    //设置显示界面状态
                    //
                    //
                    break;
                case "α/β":                    
                    //设置显示界面状态
                    //
                    //
                    break;
            }
            //获得全部启用通道
            Channel channel = new Channel();
            channelS=channel.GetChannel(true);
            //将检测面积为0的通道剔除
            for(int i=0;i<channelS.Count;i++)
            {
                if(channelS[i].ProbeArea==0)
                {
                    channelS.RemoveAt(i);
                }
            }
            //根据有效通道对象初始化用来存储最终监测数据的列表
            foreach (Channel usedChannel in channelS)
            {
                MeasureData measureData = new MeasureData();
                measureData.Channel = usedChannel;
                calculatedMeasureDataS.Add(measureData);
            }
            //在界面中将channelS列表中的通道（通道处于启用状态同时探测面积不为0）的控件Enabled有效，将其探测数值显示为0；其它通道（通道未启用或探测面积为0）的控件Enabled无效；
            //
            //
            //将运行状态标志设置为“运行准备”
            platformState = PlatformState.ReadyToRun;
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
            if(bkWorkerReceiveData.CancellationPending==false)
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
        private byte[] ReadDataFromSerialPort(BackgroundWorker worker,DoWorkEventArgs e)
        {
            int errorNumber = 0; //下发自检报文出现错误计数器
            int delayTime = 200;//下发自检报文延时时间
            while (true)
            {
                //请求进程中断读取数据
                if(worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }
                //如果当前运行状态为“仪器自检”，则根据不同探测类型向下位机下发相应的自检指令
                if (platformState == PlatformState.SelfTest)
                {
                    //获得系统自检时间,并计算下发时间参数：自检时间/2-2一并下发
                    //
                    //
                    switch (factoryParameter.MeasureType)
                    {
                        //生成Alpha自检指令报文，包含参数：自检时间/2-2
                        //生成Beta自检指令报文，包含参数：自检时间/2-2
                        case "α":
                            //下发Alpha自检指令
                            //如果不成功则：
                            //{ 
                            //错误计数器errorNumber+1，
                            //判断错误计数器errorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            //错误计数器errorNumber未超过3次，延时delayTime(200)毫秒后continue继续下发
                            //}                          
                            break;
                        case "β":
                            //下发Beta自检指令
                            //判断逻辑同Alpha自检
                            //
                            //
                            break;
                        case "α/β":
                            //下发Alpha自检指令
                            //判断逻辑同Alpha自检
                            //
                            //下发Beta自检指令
                            //判断逻辑同Alpha自检
                            //
                            break;
                    }
                    //延时1000毫秒
                    Thread.Sleep(1000);
                }
                //向下位机下发“C”指令码
                byte[] buffMessage = new byte[62];
                buffMessage[0] = Convert.ToByte('C');
                if(HFM.Components.Message.SendMessage(buffMessage, commPort) == true)
                {
                    //延时
                    Thread.Sleep(100);
                    //读取串口回传数据并赋值给receiveBuffMessage
                    byte[] receiveBuffMessage = new byte[200];
                    //
                    //
                    //延时
                    Thread.Sleep(1000);
                    //触发向主线程返回下位机上传数据事件
                    worker.ReportProgress(1, receiveBuffMessage);
                }

            }
        }

        //后台线程读取串口数据后的ReportProgress事件响应
        private void bkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS=new List<MeasureData>(); //解析后报文结构数据存储List对象            
            DateTime stateTimeStart=DateTime.Now;//系统当前运行状态的开始计时变量
            bool isFirstBackGround = true;//进入等待测量状态后的本底测量计时标志
            string pollutionRecord = null;//记录测量污染详细数据
            //对事件参数类中的数据对象序列化为byte[]
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, e.UserState);
                receiveBufferMessage = ms.GetBuffer();
            }
            //更新当前显示时间
            //
            //接收报文数据为空
            if(receiveBufferMessage.Length< messageBufferLength)
            {
                //数据接收出现错误次数超限
                if(errNumber>=2)
                {
                    //界面提示“通讯错误”
                    //
                }
                else
                {
                    errNumber++;
                }
                return;
            }
            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中
            //
            //            
            //如果当前运行状态为“运行准备”
            if (platformState==PlatformState.ReadyToRun)
            {
                //判断衣物探头被拿起
                if(measureDataS[6].InfraredStatus==1)
                {
                    //跳转至衣物检测界面
                    //
                }
                //判断左右手是否到位，到位则相应指示框背景变为绿色，否则为橙色，同时进行文字信息提示（具体操作可参考老版本源代码）
                //
                //当前运行状态设置为“仪器自检”
                platformState = PlatformState.SelfTest;
                //启动自检计时 
                stateTimeStart = System.DateTime.Now;
                return;
            }
            //如果当前运行状态为“仪器自检”
            if(platformState==PlatformState.SelfTest)
            {
                //获得当前系统参数设置中的的自检时间并赋值给stateTimeSet
                //           
                //系统语音提示“仪器自检”，并在设备运行状态区域显示“仪器自检”
                //
                //在系统界面中显示自检倒计时时间（s）:系统自检设置时间-已经用时
                int stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                //
                //
                // IList<int> channelIDS=channelS.Select(channel => channel.ChannelID).ToList();               
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据赋值给calculatedMeasureDataS，但是本地值Alpha和Beta需要累加
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    //计算每个通道上传的Alpha和Beta计数累加(是指全部启用的通道)
                    calculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    calculatedMeasureDataS[i].Beta += list[0].Alpha;
                    //将当前通道本次的其它测量数据赋值给calculatedMeasureDataS
                    calculatedMeasureDataS[i].AnalogV = list[0].AnalogV;
                    calculatedMeasureDataS[i].DigitalV = list[0].DigitalV;
                    calculatedMeasureDataS[i].HV = list[0].HV;
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    calculatedMeasureDataS[i].IsEnglish = list[0].IsEnglish;
                    calculatedMeasureDataS[i].MeasureDate = DateTime.Now;
                    //界面中对应通道显示当前测量值（list[0]的值）
                    //
                    //
                }
                //自检时间到
                if (stateTimeRemain == 0)
                {
                    string errRecord=BaseCheck();
                    if (errRecord == null)//自检通过
                    {
                        // 运行状态标志设置为“本底测量”
                        platformState = PlatformState.BackGrouneMeasure;
                        //启动本底测量计时 
                        stateTimeStart = System.DateTime.Now;
                        //将本底测量中存储各个通道测量计算结果的列表calculatedMeasureDataS清零，为本底测量时计算做准备
                        for(int i=0;i<channelS.Count;i++)
                        {
                            calculatedMeasureDataS[i].Alpha= 0;
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                    }
                    else
                    {
                        //将故障信息errRecord写入数据库
                        //
                        //界面显示“仪器故障”同时进行语音提示
                        //
                    }
                    return;
                }
            }
            //运行状态为本底测量
            if(platformState==PlatformState.BackGrouneMeasure)
            {
                //获得当前系统参数设置中的平滑时间并赋值给stateTimeSet
                //           
                //系统语音提示“本底测量”，并在设备运行状态区域显示“本底测量”
                //
                //在系统界面中显示本底测量倒计时时间（s）:系统平滑设置时间-已经用时
                int stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                for (int i = 0; i < channelS.Count; i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)：
                    //第k次计算本底值=第k-1次计算本底值*平滑因子/（平滑因子+1）+第k次测量值/（平滑因子+1）               
                    calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha*factoryParameter.SmoothingFactor/(factoryParameter.SmoothingFactor+1)+ list[0].Alpha/(factoryParameter.SmoothingFactor+1);
                    calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Beta / (factoryParameter.SmoothingFactor + 1); 
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    //当前通道红外到位
                    if(calculatedMeasureDataS[i].InfraredStatus==1)
                    {
                        //显示当前calculatedMeasureDataS[i].Channel.ChannelName对应的红外到位
                        //
                        //语音提示
                        //
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
                    string errRecord = BaseCheck();
                    //本底测量判断
                    if (errRecord == null)//本底测量通过
                    {
                        //系统参数中，将上次本底测量后已测量人数清零
                        Components.SystemParameter systemParameter = new Components.SystemParameter();
                        systemParameter.ClearMeasuredCount();
                        // 运行状态标志设置为“等待测量”
                        platformState = PlatformState.ReadyToRun;
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
                        //将故障信息errRecord写入数据库
                        //
                        //界面显示“本底测量未通过”同时进行语音提示
                        //
                    }
                }
            }
            //运行状态为等待测量
            if(platformState==PlatformState.ReadyToRun)
            {
                //所有手部红外到位标志，默认全部到位
                bool isHandInfraredStatus = true;
                //系统语音提示“等待测量”，并在设备运行状态区域显示“等待测量”
                //                
                //
                for(int i=0;i<channelS.Count;i++)
                {
                    //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                    //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    if(list[0].Channel.ChannelID != 7)//只对手部红外状态进行判断
                    {
                        if (list[0].InfraredStatus == 0)//手部红外状态不到位
                        {
                            //第一次判断红外状态
                            if(isFirstBackGround==true)
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
                if (stateTimeSet-(System.DateTime.Now-stateTimeStart).Seconds==0)
                {
                    string errRecord = BaseCheck();
                    //本底测量判断
                    if(errRecord==null)//本底检测通过
                    {                        
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
                        //将故障信息errRecord写入数据库
                        //
                        //界面显示“本底测量出现故障”同时进行语音提示
                        //
                    }
                }
            }            
            //运行状态为开始测量
            if(platformState==PlatformState.Measuring)
            {                
                float conversionData=0;
                //获得当前系统参数设置中的的测量时间并赋值给stateTimeSet
                //    
                //系统语音提示“正在测量”，并在设备运行状态区域显示“正在测量”
                //
                //在系统界面中显示正在测量倒计时时间（s）:系统设置测量时间-已经用时
                int stateTimeRemain = stateTimeSet - (System.DateTime.Now - stateTimeStart).Seconds;
                for(int i=0;i<channelS.Count;i++)
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
                    calculatedMeasureDataS[i].Alpha +=list[0].Alpha ;
                    calculatedMeasureDataS[i].Beta +=list[0].Beta ;
                    calculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;   
                    //进行语音提示
                }
                //测量时间到
                if(stateTimeRemain<0)
                {
                    Components.SystemParameter systemParameter = new Components.SystemParameter();
                    //计算每个通道的计数平均值,然后减去本底值
                    for (int i = 0; i < calculatedMeasureDataS.Count; i++)
                    {
                        calculatedMeasureDataS[i].Alpha = calculatedMeasureDataS[i].Alpha / stateTimeSet-baseData[i].Alpha;
                        if(calculatedMeasureDataS[i].Alpha<0)
                        {
                            calculatedMeasureDataS[i].Alpha = 0;
                        }                        
                        ProbeParameter probeParameter = new ProbeParameter();
                        //获得当前通道的一级和二级报警阈值
                        probeParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID, "α");
                        //判断当前通道Alpha测量值是否超过报警阈值
                        if (calculatedMeasureDataS[i].Alpha>probeParameter.Alarm_1||calculatedMeasureDataS[i].Alpha>probeParameter.Alarm_2)
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
                        if(calculatedMeasureDataS[i].Beta<0)
                        {
                            calculatedMeasureDataS[i].Beta = 0;
                        }
                        probeParameter.GetParameter(calculatedMeasureDataS[i].Channel.ChannelID, "β");
                        if(calculatedMeasureDataS[i].Beta>probeParameter.Alarm_1||calculatedMeasureDataS[i].Beta>probeParameter.Alarm_2)
                        {
                            //通过语音和文字提示当前通道Beta测量有污染，请去污。将污染信息添加进pollutionRecord字符串
                        }
                        conversionData =UnitConver(calculatedMeasureDataS[i].Beta,systemParameter.MeasurementUnit);
                        //按照系统参数单位要求显示该通道最终Beta测量结果
                    }
                    if(pollutionRecord == null)//说明本次测量无污染
                    {
                        //语音和文字提示“无污染”
                        //
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
            if(platformState==PlatformState.Result)
            {
                //本次测量无污染
                if(pollutionRecord==null)
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
                            if (calculatedMeasureDataS[i].Channel.ChannelID==7)
                            {
                                if(calculatedMeasureDataS[i].InfraredStatus==1)
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

        private string BaseCheck()
        {
            //自检是否通过标志，默认true
            bool isCheck = true;
            //故障记录字符串
            string errRecord = null;
            for (int i = 0; i < channelS.Count; i++)
            {
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

                            break;
                        case "α/β":
                            //同时计算Alpha和Beta自检数据

                            break;
                    }
                    //判断当前高压值：当前高压值若为系统设定值的0.8~1.2倍之内，则显示该通道高压正常，否则提示高压故障同时将故障信息添加到errRecord字符串，isCheck = false;
                    //
                    //
                    //判断电子线路故障：如果本底值在设定值的0.7~1.3倍之内，则显示电子线路正常，否则显示电子线路故障
                    if (factoryParameter.MeasureType != "β")
                    {
                        //对Alpha本底值(BASE_DATA)进行判断，如果故障提示“α线路故障”同时将故障信息添加到errRecord字符串，isCheck = false;
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        //对Beta本底值(BASE_DATA)进行判断，如果故障提示“β线路故障”同时将故障信息添加到errRecord字符串,isCheck = false;
                    }
                    //判断根据红外状态判断该通道红外是否故障，自检时无检查体但提示到位，说明红外故障
                    if (calculatedMeasureDataS[i].InfraredStatus == 1)//红外状态到位
                    {
                        //软件界面提示红外故障。故障点为：calculatedMeasureDataS[i].Channel.ChannelName，将故障信息添加到errRecord字符串
                        isCheck = false;
                    }
                }
                if(platformState==PlatformState.BackGrouneMeasure)
                {                    
                    ProbeParameter probeParameter = new ProbeParameter();
                    if(factoryParameter.MeasureType!= "β")
                    {
                        //查询当前通道的α本底上限、本底下限
                        probeParameter.GetParameter(channelS[i].ChannelID, "α");
                        //进行α本底测量判断
                        if (calculatedMeasureDataS[i].Alpha > probeParameter.LBackground) //超过当前通道的本底下限
                        {
                            //显示该该通道channelS[i].ChannelName本底下限值，当前本底值.同时将故障信息添加到错误信息串errRecord。置isCheck=false

                        }
                        if (calculatedMeasureDataS[i].Alpha<probeParameter.HBackground)//超过当前通道的本底上限
                        {
                            //显示该该通道channelS[i].ChannelName本底上限值，当前本底值.同时将故障信息添加到错误信息串errRecord。置isCheck=false
                        }
                    }
                    if (factoryParameter.MeasureType != "α")
                    {
                        //查询当前通道的β本底上限、本底下限
                        probeParameter.GetParameter(channelS[i].ChannelID, "β");
                        if(calculatedMeasureDataS[i].Beta>probeParameter.LBackground)//超过当前通道的本底下限
                        {
                            //显示该该通道channelS[i].ChannelName本底下限值，当前本底值.同时将故障信息添加到错误信息串errRecord。置isCheck=false
                        }
                        if(calculatedMeasureDataS[i].Beta<probeParameter.HBackground)//超过当前通道的本底上限
                        {
                            //显示该该通道channelS[i].ChannelName本底上限值，当前本底值.同时将故障信息添加到错误信息串errRecord。置isCheck=false
                        }
                    }                    
                }
                //根据是否检测通过设置该通道的背景颜色
            }
            if (isCheck == false)//未通过
            {                    
                return errRecord;
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
        private float UnitConver(float data,string unit)
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
    }
}

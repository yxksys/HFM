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
            //据有效通道对象初始化用来存储最终监测数据的列表
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
                            //错误计数器ErrorNumber+1，
                            //判断错误计数器ErrorNumber是否超过5次，超过则触发向主线程返回下位机上传数据事件：worker.ReportProgress(1, null);
                            //错误计数器ErrorNumber未超过3次，延时delayTime(200)毫秒后continue继续下发
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
            int selfCheckTimeSet = 0;//系统自检时间设置
            DateTime selfCheckTimeRun;//自检运行时间
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
                    //调整至衣物检测界面
                    //
                }
                //判断左右手是否到位，到位则相应指示框背景变为绿色，否则为橙色，同时进行文字信息提示（具体操作可参考老版本源代码）
                //
                //当前运行状态设置为“仪器自检”
                platformState = PlatformState.SelfTest;
                return;
            }
            //如果当前运行状态为“仪器自检”
            if(platformState==PlatformState.SelfTest)
            {
                //获得当前系统参数设置中的的自检时间并赋值给selfCheckTimeSet
                //
                //启动自检计时 
                selfCheckTimeRun = System.DateTime.Now;
                //系统语音提示“仪器自检”，并在设备运行状态区域显示“仪器自检”
                //
                //在系统界面中显示自检倒计时时间（s）:系统自检设置时间-已经用时
                int selfCheckTimeRemain=selfCheckTimeSet-(System.DateTime.Now - selfCheckTimeRun).Seconds;
                //
                //
                //查询未启用的
               // IList<int> channelIDS=channelS.Select(channel => channel.ChannelID).ToList();               
                for(int i=0;i<channelS.Count;i++)
                {
                    //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                    List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                    //计算每个通道上传的Alpha和Beta计数累加(是指全部启用的通道)
                    calculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    calculatedMeasureDataS[i].Beta += measureDataS[i].Beta;
                    //界面中对应通道显示当前测量值（list[0]的值）
                    //
                    //
                }
                //自检时间到
                if(selfCheckTimeRemain==0)
                {
                    for (int i = 0; i < channelS.Count; i++)
                    {
                        //根据仪器检测类型计算最终自检数据
                        switch (factoryParameter.MeasureType)
                        {
                            case "α":
                                //计算Alpha自检数据值
                                calculatedMeasureDataS[i].Alpha = (calculatedMeasureDataS[i].Alpha + BASE_DATA * 2) / selfCheckTimeSet * 2;

                                break;
                            case "β":
                                //计算Beta自检数据
                               
                                break;
                            case "α/β":
                                //同时计算Alpha和Beta自检数据

                                break;
                        }
                        //判断当前高压值：当前高压值若为系统设定值的0.8~1.2倍之内，则显示该通道高压正常，否则提示高压故障

                    }
                    
                }
            }


        }        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmTestHardware : Form
    {
        //实例化串口
        CommPort commPort = new CommPort();
        //存储各个通道最终计算检测值的List
        IList<MeasureData> calculatedMeasureDataS = new List<MeasureData>();
        /// <summary>
        /// 测量时间
        /// </summary>
        private int measuringTime;

        /// <summary>
        /// 运行状态
        /// </summary>
        enum HardwarePlatformState
        {
            /// <summary>
            /// 默认状态
            /// </summary>
            Default=0,
            /// <summary>
            /// Alpha自检
            /// </summary>
            AlphaCheck=1,
            /// <summary>
            /// Beta自检
            /// </summary>
            BetaCheck=2,
            /// <summary>
            /// 自检
            /// </summary>
            SelfTest=3
        }
        //运行状态标志
        HardwarePlatformState platformState;
        public FrmTestHardware()
        {
            InitializeComponent();
        }

        private void FrmTestHardware_Load(object sender, EventArgs e)
        {
            
            //初始化运行状态为默认状态
            platformState = HardwarePlatformState.Default;
            //初始化测量时间为系统参数时间
            measuringTime = (new HFM.Components.SystemParameter().GetParameter().MeasuringTime);
            //从配置文件获得当前串口配置
            if (commPort.Opened == true)
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
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
            //开启异步线程
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
                //如果当前运行状态为不是“默认状态
                //”，则根据不同探测类型向下位机下发相应的自检指令
                if (platformState != HardwarePlatformState.Default)
                {
                    byte[] messageDate = null;
                    switch (platformState)
                    {
                        //生成Alpha、Beta和自检指令报文,自检时间固定10秒                        
                        case HardwarePlatformState.AlphaCheck:
                            //下发Alpha自检指令
                            messageDate = Components.Message.BuildMessage(0);
                            //如果不成功则：
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
                            break;
                        case HardwarePlatformState.BetaCheck:
                            //下发Beta自检指令
                            messageDate = Components.Message.BuildMessage(1);
                            //如果不成功则：
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
                            break;
                        case HardwarePlatformState.SelfTest:
                            //下发自检指令
                            messageDate = Components.Message.BuildMessage(1);
                            //如果不成功则：
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
                            break;
                    }
                    //延时1000毫秒
                    Thread.Sleep(1000);
                }
                //向下位机下发“C”指令码
                byte[] buffMessage = new byte[62];
                buffMessage[0] = Convert.ToByte('C');
                if (HFM.Components.Message.SendMessage(buffMessage, commPort) == true)
                {
                    //延时
                    Thread.Sleep(100);
                    //读取串口回传数据并赋值给receiveBuffMessage
                    byte[] receiveBuffMessage = new byte[200];
                    receiveBuffMessage = Components.Message.ReceiveMessage(commPort);
                    //延时
                    Thread.Sleep(1000);
                    //触发向主线程返回下位机上传数据事件
                    worker.ReportProgress(1, receiveBuffMessage);
                }

            }
        }

        //异步线程读取串口数据后的ReportProgress事件响应
        private void bkWorkerReceiveData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int messageBufferLength = 62; //最短报文长度
            int errNumber = 0; //报文接收出现错误计数器
            byte[] receiveBufferMessage; //存储接收报文信息缓冲区
            IList<MeasureData> measureDataS = new List<MeasureData>(); //解析后报文结构数据存储List对象            
            DateTime stateTimeStart = DateTime.Now; //系统当前运行状态的开始计时变量
            bool isFirstBackGround = true; //进入等待测量状态后的本底测量计时标志
            string pollutionRecord = null; //记录测量污染详细数据
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
            if (receiveBufferMessage.Length < messageBufferLength)
            {
                //数据接收出现错误次数超限
                if (errNumber >= 2)
                {
                    MessageBox.Show("通讯错误！请检查通讯是否正常。");
                    return;
                }
                else
                {
                    errNumber++;
                }

                return;
            }

            //接收报文无误，进行报文解析，并将解析后的监测数据存储到measureDataS中 
            measureDataS = Components.Message.ExplainMessage<MeasureData>(receiveBufferMessage);


            
        }
    }
}

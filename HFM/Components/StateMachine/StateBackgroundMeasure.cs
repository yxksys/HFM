using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    class StateBackgroundMeasure:State
    {        
        //状态处理过程中用于语音或文字信息提示的事件委托
        public delegate void ShowMsgHander(object sender, UIEventArgs eventAtgs);
        //状态下进行语音或文字提示事件
        public event ShowMsgHander ShowMsgEvent;
        string appPath = System.Windows.Forms.Application.StartupPath;
        //事件参数类
        //UIEventArgs uIEventArgs = new UIEventArgs();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <param name="previous"></param>
        /// <param name="Args"></param>
        /// <returns></returns>
        public override bool Run<T>(IState current, IState previous, ref T Args)
        {
            return false;
        }
        /// <summary>
        /// 本底测量数据处理，每1s处理一组数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Args1"></param>
        /// <param name="Args2"></param>
        /// <returns>-1：未到1s或红外异常1：数据处理正常返回</returns>
        public override int Run<T1, T2, T3>(T1 Args1, T2 Args2, T3 Args3)
        {
            //判断时间是否过了1s
            if ((System.DateTime.Now - this.LastTime).Seconds <= 0)
            {
                return -1;
            }
            //已经过了1s，记录当前时间
            this.LastTime = System.DateTime.Now;
            //当前状态保持时间-1s
            this.HoldTimes--;
            //base.Run(Args1, Args2);            
            //测量数据计数+1
            dataCount++;
            IList<Channel> channelS = (IList<Channel>)Args1;
            IList<MeasureData> measureDataS = (IList<MeasureData>)Args2;
            UIEventArgs uIEventArgs = Args3 as UIEventArgs;
            for (int i=0;i<channelS.Count;i++)//遍历全部启用的检测通道
            {
                //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                if (list.Count > 0)
                {
                    this.CalculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    //该通道红外到位
                    if (this.CalculatedMeasureDataS[i].InfraredStatus == 1)
                    {
                        System.IO.File.AppendAllText(appPath + "\\log\\msg.txt", "本底测量当前红外状态----" + CalculatedMeasureDataS[i].Channel.ChannelName+":"+ CalculatedMeasureDataS[i].InfraredStatus.ToString() + "\r\n");
                        this.DeviceStatus = DeviceStatus.OperatingFaulted;
                        //uIEventArgs.CurrentState = this;
                        if (this.IsShowMsg == false)//未进行有效信息显示和语音播报
                        {
                            //显示异常信息并语音提示                            
                            ShowMsgEvent?.Invoke(this, uIEventArgs);
                            this.IsShowMsg = true;
                        }
                        //测量数据存储全部清零
                        foreach (MeasureData data in this.CalculatedMeasureDataS)
                        {
                            data.Alpha = 0;
                            data.Beta = 0;
                        }
                        //重新启动本底测量计时
                        this.HoldTimes = this.SetTimes;
                        return -1;
                    }
                    //本底测量平滑算法修改为平均值算法。此处为平滑算法
                    /***********************************************
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
                    *****************************************************/
                    /***************************************************
                     * 此处为修改后的平均值算法，测量值累加并计数
                     * **************************************************/
                    this.CalculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    this.CalculatedMeasureDataS[i].Beta += list[0].Beta;
                    //记录当前本底平滑值
                    //File.AppendAllText(appPath + "\\log\\background.txt", "本底测量累加值，通道编号：" + calculatedMeasureDataS[i].Channel.ChannelID.ToString() + ";Alpha:" + calculatedMeasureDataS[i].Alpha.ToString() + ";Beta:" + calculatedMeasureDataS[i].Beta.ToString() + "\r\n");                    
                }
            }            
            return 1;
        }
    }
}

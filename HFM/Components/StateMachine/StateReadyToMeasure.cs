using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    class StateReadyToMeasure:State
    {
        //状态处理过程中用于语音或文字信息提示的事件委托
        public delegate void ShowMsgHander(object sender,UIEventArgs eventAtgs);
        //状态下进行语音或文字提示事件
        public event ShowMsgHander ShowMsgEvent;
        //事件参数类
        //UIEventArgs uIEventArgs = new UIEventArgs();
        int playControl = -1;//控制语音播放变量
        int timesOutCount = 0;//两步式探测（单探测器）超时时间计算次数
        /// <summary>
        /// 等待测量数据处理，每1s处理一组数据
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Args1"></param>
        /// <param name="Args2"></param>
        /// <returns>-1：未到1s无需进行任何处理返回；0：数据正常处理；1：转手脚开始测量状态;</returns>
        public override int Run<T1, T2, T3>(T1 Args1, T2 Args2, T3 Args3)
        {                                    
            //bool isHandSecondEnabled = false;//是否允许启动手部翻转后测量，当手部第一次测量结束后，用户必须翻转手掌（红外出现至少一次不到位），才能启动第二次手部检测
            //判断时间是否过了1s
            if ((System.DateTime.Now - this.LastTime).Seconds <= 0)
            {
                return -1;
            }           
            //已经过了1s，记录当前时间
            this.LastTime = System.DateTime.Now;
            //当前状态保持时间-1s
            this.HoldTimes--;
            playControl++;
            IList<Channel> channelS = (IList<Channel>)Args1;
            IList<MeasureData> measureDataS = (IList<MeasureData>)Args2;
            UIEventArgs uIEventArgs = Args3 as UIEventArgs;
            //uIEventArgs.CurrentState = this;
            //所有手部红外到位标志，默认全部到位
            bool isHandInfraredStatus = true;
            //HandProbeCtrl为StepOneComplete，说明手部第一次检测已经完成现在的等待检测状态为第二次手部检测
            if (this.FactoryParameter.IsDoubleProbe == false && this.HandProbeCtrl == HandProbeCtrl.StepOneComplete)
            {
                //计时时间到，重新启动等待测量计时
                if (this.HoldTimes < 0)
                {                    
                    this.HoldTimes = this.SetTimes;
                }
                //每4秒进行一次反转手掌语音提示判断，超过提示次数限制重新启动一次新的测量，否则红外到位说明手还未离开提示翻转手掌检测
                if (playControl % 4 == 0)
                {
                    timesOutCount++;                    
                    //提示次数达到超时计数次数（1次4s共计次数为系统设置超时时间除以4），则重置手心检测状态，恢复到等待测量阶段
                    if (timesOutCount >= this.SystemParameter.TimeOut / 4)
                    {
                        //重置手部检测标志为StepOneReady（未开始检测）
                        this.HandProbeCtrl = HandProbeCtrl.StepOneReady;
                        timesOutCount = 0;
                        //重置语音播报控制计数
                        playControl = -1;
                    }
                    //提示次数未达到计数次数上限，且手部还未离开探测器（isHandSecondEnabled为false）
                    else
                    {
                        //一个通道红外到位，说明手部还未全部离开探测器
                        if (measureDataS[1].InfraredStatus == 1 || measureDataS[3].InfraredStatus == 1)
                        {
                            //设置语音信息标志为翻转手掌进行检测                            
                            this.SoundMessage = SoundMessage.FlipPalm;                            
                            //按照SoundMessage状态，语音请翻转手掌进行检测
                            ShowMsgEvent?.Invoke(this,uIEventArgs);
                        }
                    }
                }
                //手部红外未到位，说明手部已经离开探测器，且是第一次检测完成，则置this.HandProbeCtrl为StepTwoReady（可以进行第二次手部检测）
                if (measureDataS[1].InfraredStatus == 0 && measureDataS[3].InfraredStatus == 0 )
                {
                    timesOutCount = 0;
                    this.HandProbeCtrl=HandProbeCtrl.StepTwoReady;
                }
            }
            if(this.HandProbeCtrl==HandProbeCtrl.StepTwoReady)
            {
                //手部红外不到位
                if (measureDataS[1].InfraredStatus == 0 && measureDataS[3].InfraredStatus == 0)
                {
                    timesOutCount++;
                    if(timesOutCount >= this.SystemParameter.TimeOut / 4)
                    {
                        //重置手部检测标志为StepOneReady（未开始检测）
                        this.HandProbeCtrl = HandProbeCtrl.StepOneReady;
                        timesOutCount = 0;
                    }
                }
            }
            //手部检测已经完成，红外不到位说明手部已经离开手部检测状态复位；红外到位提示进行衣物检测或离开
            if (this.HandProbeCtrl == HandProbeCtrl.StepTwoComplete)//手部检测已经完成
            {
                if (this.HoldTimes <= 0)
                {
                    //重新启动等待测量计时
                    this.HoldTimes = this.SetTimes;
                }
                //手部检测完成后，红外不到位，说明手部已经离开监测仪
                if (measureDataS[1].InfraredStatus == 0 && measureDataS[3].InfraredStatus == 0)//说明手部检测完成后，红外不到位，手部已经离开监测仪
                {
                    //重置手部检测标志为StepOneReady（第一次检测准备）
                    this.HandProbeCtrl = HandProbeCtrl.StepOneReady;
                    //重置语音控制计数
                    playControl = -1;
                    return 0;
                }
                //有红外到位，提示离开或进行衣物测量
                else
                {
                    if (playControl % 4 == 0)
                    {
                        //衣物探头未启用，提示离开
                        if (measureDataS[6].Channel.IsEnabled == false)
                        {                            
                            //设置语音信息标志为没有污染请离开
                            this.SoundMessage = SoundMessage.Leave;
                            //按照SoundMessage状态，语音提示没有污染请离开
                            ShowMsgEvent?.Invoke(this,uIEventArgs);
                        }
                        //衣物探头启用，提示进行衣物检测
                        else
                        {
                            //设置语音信息标志为没有污染请离开
                            this.SoundMessage = SoundMessage.FriskerMeasure;
                            //按照SoundMessage状态，语音提示没有污染进行以为测量
                            ShowMsgEvent?.Invoke(this,uIEventArgs);
                        }                        
                    }                    
                    return  0;
                }
            }
            //只有衣物检测被启用，则置所有手部到位标志为false
            if (channelS.Count == 1 && channelS[0].ChannelID == 7)
            {
                isHandInfraredStatus = false;
            }
            //通道红外不到位，则对每个通道本底数据进行处理
            for (int i = 0; i < channelS.Count; i++)
            {
                //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                //所以，需要从measureDataS中找到对应通道的监测数据通过平滑计算后赋值给calculatedMeasureDataS
                //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();                
                if (list.Count <= 0)
                {
                    continue;
                }
                if (list[0].Channel.ChannelID >= 1 && list[0].Channel.ChannelID <= 7 && list[0].InfraredStatus == 0)//当前通道为手部脚步通道且红外不到位 
                {                        
                    //手部红外状态到位标志置false，说明手部不到位
                    if (list[0].Channel.ChannelID != 7)
                    {
                        isHandInfraredStatus = false;
                    }
                    /*****************************************************************
                        * 本底平滑初始值为接收到第一个报文的测量值，修改为本底结果后注释
                         
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
                        calculatedMeasureDataS[i].Beta = calculatedMeasureDataS[i].Beta * factoryParameter.SmoothingFactor / (factoryParameter.SmoothingFactor + 1) + list[0].Beta / (factoryParameter.SmoothingFactor + 1);                            
                    }
                    * *****************************************************************/
                    /*******************************************************************
                        * 本底平滑初始值修改为：本底测量后平滑结果，所以不需要对初始值进行设置
                        ******************************************************************/
                    this.CalculatedMeasureDataS[i].Alpha = this.CalculatedMeasureDataS[i].Alpha * this.FactoryParameter.SmoothingFactor / (this.FactoryParameter.SmoothingFactor + 1) + list[0].Alpha / (FactoryParameter.SmoothingFactor + 1);
                    this.CalculatedMeasureDataS[i].Beta = this.CalculatedMeasureDataS[i].Beta * this.FactoryParameter.SmoothingFactor / (this.FactoryParameter.SmoothingFactor + 1) + list[0].Beta / (FactoryParameter.SmoothingFactor + 1);

                    //记录当前本底平滑值
                    //File.AppendAllText(appPath + "\\log\\background.txt", "等待测量-本底平滑值，通道编号：" + calculatedMeasureDataS[i].Channel.ChannelID.ToString() + ";Alpha:" + calculatedMeasureDataS[i].Alpha.ToString() + ";Beta:" + calculatedMeasureDataS[i].Beta.ToString() + "\r\n");

                    this.CalculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;                    
                }
            }                        
            //所有通道手部红外状态全部到位，且手部检测未完成，转开始测量状态
            if (isHandInfraredStatus == true && this.HandProbeCtrl != HandProbeCtrl.StepTwoComplete && this.HandProbeCtrl!=HandProbeCtrl.StepOneComplete)
            {
                //重置测量超时计数器
                timesOutCount = 0;
                //重置语音提示控制计数器
                playControl = -1;
                //返回转开始测量状态
                return 1;
            }
            return 0;
        }        
    }
}

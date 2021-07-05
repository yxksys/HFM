using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace HFM.Components
{
    class StateHandFootMeasuring:State
    {        
        //状态处理过程中用于语音或文字信息提示的事件委托
        public delegate void ShowMsgHander(object sender, UIEventArgs eventAtgs);
        //状态下进行语音或文字提示事件
        public event ShowMsgHander ShowMsgEvent;        
        public string AlphaNuclideUsed { get; set; }= "U-235";//Alpha核素选择，默认U-235
        public string BetaNuclideUsed { get; set; } = "U-235";//Beta核素选择，默认U-235                   
        MeasureData conversionData = new MeasureData();
        public IList<MeasureData> ConversionDataS = new List<MeasureData>();
        string pollutionRecord = null;
        string pollutionRecord_E = null;
        string appPath = Application.StartupPath;//应用系统安装路径
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
            IList<Channel> channelS = (IList<Channel>)Args1;
            IList<MeasureData> measureDataS = (IList<MeasureData>)Args2;
            UIEventArgs uIEventArgs = Args3 as UIEventArgs;            
            //遍历全部启用的检测通道
            for (int i = 0; i < channelS.Count; i++)
            {
                //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                //所以，需要从measureDataS中找到对应通道的监测数据经过平滑运算后赋值给calculatedMeasureDataS
                //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
               // File.AppendAllText(appPath + "\\log\\errorLog.txt", "measureData:Alpha=" + list[0].Alpha.ToString() + "; Beta=" + list[0].Beta.ToString() + ";\r\n");
                if (list.Count <= 0)
                {
                    continue;
                }
                //同步当前检测红外状态
                this.CalculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;                                
                if (list[0].Channel.ChannelID >= 1 && list[0].Channel.ChannelID <= 6 && list[0].InfraredStatus == 0)//手部红外状态未到位
                {                    
                    //显示异常信息并语音提示(左/右手、脚移动重新测量)
                    ShowMsgEvent?.Invoke(this, uIEventArgs);                                                              
                    return 0;
                }                
                //计算每个通道上传的Alpha和Beta本底值(是指全部启用的通道)进行累加：                    
                this.CalculatedMeasureDataS[i].Alpha += list[0].Alpha;
                this.CalculatedMeasureDataS[i].Beta += list[0].Beta;
               // File.AppendAllText(appPath + "\\log\\errorLog.txt", "calcuData:Alpha=" + CalculatedMeasureDataS[i].Alpha.ToString() + "; Beta=" + CalculatedMeasureDataS[i].Beta.ToString() + ";\r\n\r\n");                                
            }
           // File.AppendAllText(appPath + "\\log\\errorLog.txt","---------"+"\r\n");
            return 1;//数据正常处理返回
        }
        public Dictionary<Language, string> BaseCheck<T1, T2, T3, T4>(T1 Args1, T2 Args2, T3 Args3,T4 Args4)
        {
            IList<Channel> channelS = (IList<Channel>)Args1;
            IList<EfficiencyParameter> efficiencyParameterS = (IList<EfficiencyParameter>)Args2;
            IList<ProbeParameter> probeParameterS = (IList<ProbeParameter>)Args3;
            //事件参数类
            UIEventArgs uIEventArgs =Args4 as UIEventArgs;
            //计算每个通道的计数平均值,然后减去本底值
            foreach (Channel channel in  channelS)
            {
                if (channel.ChannelID == 7)//如果是衣物探头，则跳过继续
                {
                    continue;
                }
                //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                IList<MeasureData> calculatedDataOfChannel = CalculatedMeasureDataS.Where(measureData => measureData.Channel.ChannelID == channel.ChannelID).ToList();
                if(calculatedDataOfChannel.Count<=0)
                {
                    continue;
                }
                IList<MeasureData> baseDataOfChannel = BaseData.Where(baseData => baseData.Channel.ChannelID == channel.ChannelID).ToList();
                conversionData.Channel = channel;
                //对于双探测器（一步式），则每个通道循环处理判断是否污染即可
                //对于单探测器（两步式），则第一次检测时每个通道都循环判断是否污染，但是因为手背数据没有采集到全为0所以不受影响
                //如果单探测器（两步式），如果是第二次测量，则需要把手心数据赋值给手背,而对于手心通道跳过不做是否污染判断；当通道是手背时再做是否污染判断
                if (this.FactoryParameter.IsDoubleProbe==false && this.HandProbeCtrl==HandProbeCtrl.StepTwoReady)
                {
                    List<MeasureData> list = null;
                    //单探测器如果第二次测量跳过脚部判断（因为脚部是否污染判断在第一次测量时已经做过了）
                    if (channel.ChannelID == 5 || channel.ChannelID == 6)
                    {
                        continue;
                    }
                    //如果是左手心道盒数据，需要存储到左手背，所以找到左手背通道
                    if (channel.ChannelID == 1)
                    {
                        //找到calculatedMeasureDataS的左手背通道                                
                        list = CalculatedMeasureDataS.Where(data => data.Channel.ChannelID == 2).ToList();
                    }
                    //如果是右手心道盒数据，需要存储到右手背，所以找到右手背通道
                    if (channel.ChannelID == 3)
                    {
                        //找到calculatedMeasureDataS的右手背通道                                
                        list = CalculatedMeasureDataS.Where(data => data.Channel.ChannelID == 4).ToList();
                    }
                    //将左/右手心道盒数据拷贝到找到的左/右手背通道，然后直接返回处理下一通道（不做污染判断）
                    if (list != null && list.Count > 0)
                    {
                        //找到手背通道对应的索引号
                        int index = CalculatedMeasureDataS.IndexOf(list[0]);
                        //将当前（手心）通道数据拷贝到手背通道
                        Tools.Clone(calculatedDataOfChannel[0], CalculatedMeasureDataS[index]);
                        continue;
                    }
                }
                //根据测量时间、本底值进行测量结果校正,并进行是否污染判断
                if (FactoryParameter.MeasureType == "α" || FactoryParameter.MeasureType == "α/β")
                {
                    calculatedDataOfChannel[0].Alpha = calculatedDataOfChannel[0].Alpha / (SetTimes + 1) - baseDataOfChannel[0].Alpha;
                    if (calculatedDataOfChannel[0].Alpha < 0)
                    {
                        calculatedDataOfChannel[0].Alpha = 0;
                    }
                    //测量单位转换
                    IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == channel.ChannelID && efficiencyParameter.NuclideName == this.AlphaNuclideUsed).ToList();
                    conversionData.Alpha = Tools.UnitConvertCPSTo(calculatedDataOfChannel[0].Alpha, SystemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedDataOfChannel[0].Channel.ProbeArea);
                    //获得当前检测通道的Alpha探测参数
                    IList<ProbeParameter> probeParmeterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channel.ChannelID && probeParmeter.NuclideType == "α").ToList();
                    //判断当前通道Alpha测量值是否超过报警阈值                       
                    if (calculatedDataOfChannel[0].Alpha > probeParmeterNow[0].Alarm_1 || calculatedDataOfChannel[0].Alpha > probeParmeterNow[0].Alarm_2)
                    {
                        uIEventArgs.isAudioMan = true;
                        float tempValue = calculatedDataOfChannel[0].Alpha > probeParmeterNow[0].Alarm_2 ? probeParmeterNow[0].Alarm_2 : probeParmeterNow[0].Alarm_1;
                        tempValue = Tools.UnitConvertCPSTo(tempValue, SystemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedDataOfChannel[0].Channel.ProbeArea);
                        //将当前通道Alpha测量污染信息添加进pollutionRecord字符串
                        pollutionRecord += string.Format("{0}测量值:α值{1}{2}设置值:{3}{4}\r\n", calculatedDataOfChannel[0].Channel.ChannelName, conversionData.Alpha.ToString("F2"), SystemParameter.MeasurementUnit, tempValue.ToString("F2"), SystemParameter.MeasurementUnit);
                        pollutionRecord_E += string.Format("{0}Actual:Alpha Value{1}{2}Preset:{3}{4}\r\n", calculatedDataOfChannel[0].Channel.ChannelName_English, conversionData.Alpha.ToString("F2"), SystemParameter.MeasurementUnit, tempValue.ToString("F2"), SystemParameter.MeasurementUnit);                                                
                        if (calculatedDataOfChannel[0].Alpha > probeParmeterNow[0].Alarm_2)
                        {                            
                            //设置当前通道显示状态为Alpha高阶污染
                            uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingContaminated_2;
                            //设置当前设备状态为Alpha高阶污染
                            this.DeviceStatus = DeviceStatus.OperatingContaminated_2;                            
                        }
                        else
                        {                            
                            //本次污染状态为一级报警，当前设备状态比一级报警低（正常或故障），则当前设备状态设置为一级报警，否则状态不变
                            if (this.DeviceStatus <= DeviceStatus.OperatingContaminated_1)
                            {
                                //设置当前通道显示状态为Alpha低阶污染
                                uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingContaminated_1;
                                //将设备监测状态设置为Alpha“低阶污染”
                                this.DeviceStatus = DeviceStatus.OperatingContaminated_1;
                            }
                        }
                    }
                    else//当前通道数据未超过报警上限，则将检测数据归0，用于显示
                    {
                        conversionData.Alpha = 0;
                    }
                }
                if (FactoryParameter.MeasureType == "β" || FactoryParameter.MeasureType == "α/β")
                {
                    //TxtShowResult.Text += calculatedMeasureDataS[i].Beta.ToString() + "\r\n";
                    //TxtShowResult.Text += "本："+baseData[i].Beta.ToString() + "\r\n";
                    calculatedDataOfChannel[0].Beta = calculatedDataOfChannel[0].Beta / (SetTimes + 1) - baseDataOfChannel[0].Beta;
                    if (calculatedDataOfChannel[0].Beta < 0)
                    {
                        calculatedDataOfChannel[0].Beta = 0;
                    }
                    //测量单位转换
                    IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == channel.ChannelID && efficiencyParameter.NuclideName == BetaNuclideUsed).ToList();
                    conversionData.Beta = Tools.UnitConvertCPSTo(calculatedDataOfChannel[0].Beta, SystemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedDataOfChannel[0].Channel.ProbeArea);
                    //获得当前检测通道的Beta探测参数
                    IList<ProbeParameter> probeParmeterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channel.ChannelID && probeParmeter.NuclideType == "β").ToList();
                    if (calculatedDataOfChannel[0].Beta > probeParmeterNow[0].Alarm_1 || calculatedDataOfChannel[0].Beta > probeParmeterNow[0].Alarm_2)
                    {
                        float tempValue = calculatedDataOfChannel[0].Beta > probeParmeterNow[0].Alarm_2 ? probeParmeterNow[0].Alarm_2 : probeParmeterNow[0].Alarm_1;
                        tempValue = Tools.UnitConvertCPSTo(tempValue, SystemParameter.MeasurementUnit, efficiencyParameterNow[0].Efficiency, calculatedDataOfChannel[0].Channel.ProbeArea);
                        //将当前通道Beta测量污染信息添加进pollutionRecord字符串
                        pollutionRecord += string.Format("{0}测量值:β值{1}{2}设置值:{3}{4}\r\n", channel.ChannelName, conversionData.Beta.ToString("F2"), SystemParameter.MeasurementUnit, tempValue.ToString("F2"), SystemParameter.MeasurementUnit);
                        pollutionRecord_E += string.Format("{0}Actual:Beta Value{1}{2}Preset:{3}{4}\r\n", channel.ChannelName_English, conversionData.Beta.ToString("F2"), SystemParameter.MeasurementUnit, tempValue.ToString("F2"), SystemParameter.MeasurementUnit);                        
                        if (calculatedDataOfChannel[0].Beta > probeParmeterNow[0].Alarm_2)
                        {
                            //设置当前通道显示状态为Beta高阶污染
                            uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingContaminated_2;
                            //设置当前设备状态为Beta高阶污染
                            this.DeviceStatus = DeviceStatus.OperatingContaminated_2;                                                                                    
                        }
                        else
                        {
                            //设置当前通道显示状态为Alpha低阶污染
                            uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingContaminated_1;
                            //本次污染状态为一级报警，当前设备状态比一级报警低（正常或故障），则当前设备状态设置为一级报警，否则状态不变
                            if (this.DeviceStatus <= DeviceStatus.OperatingContaminated_1)
                            {                                
                                //将设备监测状态设置为“污染”
                                this.DeviceStatus = DeviceStatus.OperatingContaminated_1;
                            }
                        }
                    }
                    else//当前通道数据未超过报警上限，则将检测数据归0，用于显示
                    {
                        conversionData.Beta = 0;
                    }
                }
                //双探测器或者是单探测器第一次测量除去手背或者是单探测器第二次测量的手背数据，则将当前单位转换后的探测数据添加进转换单位探测数据列表
                if (FactoryParameter.IsDoubleProbe == true || (FactoryParameter.IsDoubleProbe == false && this.HandProbeCtrl == HandProbeCtrl.StepOneReady && (conversionData.Channel.ChannelID != 2 && conversionData.Channel.ChannelID != 4)) || (FactoryParameter.IsDoubleProbe == false && this.HandProbeCtrl == HandProbeCtrl.StepTwoReady && (conversionData.Channel.ChannelID == 2 || conversionData.Channel.ChannelID == 4)))
                {
                    //将单位转换后的测量数据添加进IList列表
                    MeasureData conversionDataTemp = new MeasureData();
                    Tools.Clone(conversionData, conversionDataTemp);
                    Tools.Clone(conversionData.Channel, conversionDataTemp.Channel = new Channel());
                    ConversionDataS.Add(conversionDataTemp);
                }
            }
            if(pollutionRecord!=null)
            {
                Dictionary<Language, string> errRecordS = new Dictionary<Language, string>();
                errRecordS.Add(Language.Chinese, pollutionRecord);
                errRecordS.Add(Language.English, pollutionRecord_E);                
                return errRecordS;
            }
            else
            {
                this.DeviceStatus = DeviceStatus.OperatingNormally;
                return null;
            }            
        }
    }
}

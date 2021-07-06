using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HFM.Components
{
    class StateFriskerMeasuringShared:StateFriskerMeasuringIndependent,IStateFrisker
    {
        //状态处理过程中用于在衣物探测界面进行测量信息显示的事件委托
        public new delegate void DisplayDataHander(object sender, FriskerEventArgs eventAtgs);
        //状态处理过程中用于在衣物探测界面进行测量信息显示的事件
        public new event DisplayDataHander DisplayDataEvent;
        //状态处理过程中用于在主探测界面进行语音或文字提示事件
        public new event DisplayDataHander ShowMsgEvent;
        //探头本底值
        float _baseDataOfAlpha =0;
        float _baseDataOfBeta = 0;
        //平滑处理后的衣物测量值
        //public float SmoothedDataOfAlpha { get; set; } = 0;
        //public float SmoothedDataOfBeta { get; set; } = 0;
        public StateFriskerMeasuringShared(FrmClothes frmClothes)
        {
            TEAM_LENGTH = 120;
            _smoothingData.team = new UInt32[TEAM_LENGTH];
        }
        public StateFriskerMeasuringShared()
        { }
        public string AlphaNuclideUsed { get; set; }
        public string BetaNuclideUsed { get; set; }
                       
        public override int Run<T1, T2, T3, T4>(T1 Args1, T2 Args2, T3 Args3, T4 Args4)
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
            State stateCurrent = Args1 as State;            
            IList<MeasureData> measureDataS = (IList<MeasureData>)Args2;
            IList<ProbeParameter> probeParameterS = (IList<ProbeParameter>)Args3;
            IList<EfficiencyParameter> efficiencyParameterS = (IList<EfficiencyParameter>)Args4;
            //因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
            //所以，需要从measureDataS中找到对应通道的监测数据经过平滑运算后赋值给calculatedMeasureDataS
            //从解析的7个通道的measureDataS监测数据中找到当前衣物通道的测量数据
            //由于衣物共享右手心探头，所以查询右手心数据（ChannelID=3）
            List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == 3).ToList();                                      
            if (list.Count <= 0)
            {
                return -1;
            }
            //将手心数据拷贝到衣物通道            
            measureDataS[6].Alpha = list[0].Alpha;
            measureDataS[6].Beta = list[0].Beta;
            //对采集到的衣物数据做平滑处理
            try
            {
                SmoothedDataOfAlpha = SmoothData((UInt32)measureDataS[6].Alpha);
            }
            catch (Exception ex)
            {
                using (StreamWriter fs = new StreamWriter(appPath + "\\log\\errorLog.txt", true))
                {
                    fs.WriteLine(ex.ToString() + "\r\n");
                }
            }
            try
            {
                SmoothedDataOfBeta = SmoothData((UInt32)measureDataS[6].Beta);
            }
            catch (Exception ex)
            {
                using (StreamWriter fs = new StreamWriter(appPath + "\\log\\errorLog.txt", true))
                {
                    fs.WriteLine(ex.ToString() + "\r\n");
                }
            }
            //衣物探头已经被拿起（红外状态为到位)
            if (measureDataS[6].InfraredStatus == 1)
            {
                //衣物红外到位前提下，如果当前状态为等待测量,则可以进行测量。其它状态（仪器自检、本底测量）等则应提示探头到位错误
                if (stateCurrent.GetType() == typeof(StateReadyToMeasure))
                {
                    //获得当前衣物检测通道的探测参数
                    IList<ProbeParameter> clothesProbeParmeter = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == 3).ToList();
                    //无数据返回后不做任何处理
                    if (clothesProbeParmeter.Count <= 0)
                    {
                        return -1;
                    }
                    eventArgs.ProbeParmeter = clothesProbeParmeter;
                    //衣物离线时间计数器+1（1s）
                    OfflineTimeCount++;
                    //衣物探头刚刚被拿起--------衣物探头状态之前为未拿起，现在为红外到位，说明衣物探头刚刚被拿起  
                    if (FriskerStatus == FriskerStatus.NotPickUp)
                    {
                        //置衣物探头状态为到位-------探头已经被拿起
                        FriskerStatus = FriskerStatus.PickUP;
                        //重新开始衣物离线时间计数
                        OfflineTimeCount = 0;
                        //返回后加载衣物测量界面
                        return 0;
                    }
                    //衣物探头已经被拿起--------衣物探头状态之前已经被拿起，现在也为红外到位，说明衣物探头已经被拿起一段时间（至少1s）
                    if (FriskerStatus == FriskerStatus.PickUP)
                    {
                        //从当前探测值中减去本底值
                        SmoothedDataOfAlpha = (SmoothedDataOfAlpha - _baseDataOfAlpha) < 0 ? 0 : SmoothedDataOfAlpha - _baseDataOfAlpha;
                        SmoothedDataOfBeta = (SmoothedDataOfBeta - _baseDataOfBeta) < 0 ? 0 : SmoothedDataOfBeta - _baseDataOfBeta;
                        //为衣物探测界面显示本底值(单位cps)和测量值，将当前本底值、测量值赋值给事件参数类                             
                        eventArgs.BackgroundValue["α"] = SmoothedDataOfAlpha;
                        eventArgs.BackgroundValue["β"] = SmoothedDataOfBeta;
                        eventArgs.MeasureValue["α"] = SmoothedDataOfAlpha;
                        eventArgs.MeasureValue["β"] = SmoothedDataOfBeta;                         
                        if (FactoryParameter.MeasureType == "α" || FactoryParameter.MeasureType == "α/β")
                        {
                            IList<ProbeParameter> probeParmeterOfAlpha = clothesProbeParmeter.Where(probeParmeter => probeParmeter.NuclideType == "α").ToList();
                            #region 如果减去本底值后的测量值大于报警阈值，说明有污染
                            if (SmoothedDataOfAlpha > probeParmeterOfAlpha[0].Alarm_1 || SmoothedDataOfAlpha > probeParmeterOfAlpha[0].Alarm_2)
                            {
                                //报警次数+1
                                AlarmCount++;
                                //如果连续三次出现污染报警（污染报警计数器超过3）
                                //2020-07-31修改为出现1次即报警，且可以重复多次报警
                                //if (alarmCountOfClothes > 0 && isClothesContaminated == false)
                                if (AlarmCount > 0)
                                {
                                    AlarmCount = 0;//报警计数器归零
                                                   //将设备监测状态设置为“污染”
                                    if (SmoothedDataOfAlpha > probeParmeterOfAlpha[0].Alarm_2)
                                    {
                                        DeviceStatus = DeviceStatus.OperatingContaminated_2;
                                    }
                                    else
                                    {
                                        DeviceStatus = DeviceStatus.OperatingContaminated_1;
                                    }                                    
                                }
                                eventArgs.PreSet["α"] = SmoothedDataOfAlpha > probeParmeterOfAlpha[0].Alarm_2 ? probeParmeterOfAlpha[0].Alarm_2 : probeParmeterOfAlpha[0].Alarm_1;
                            }
                            #endregion
                            #region 如果减去本底值后的测量值未大于报警阈值，说明没有污染
                            else
                            {
                                UnAlarmCount++;//衣物检测正常计数器+1
                                if (UnAlarmCount > 2)//超过3次，界面背景设置为正常状态
                                {
                                    this.DeviceStatus = DeviceStatus.OperatingNormally;
                                }
                                //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                                IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "α" && efficiencyParameter.Channel.ChannelID == 3 && efficiencyParameter.NuclideName == AlphaNuclideUsed).ToList();
                                if (efficiencyParameterNow.Count > 0)
                                {
                                    eventArgs.Efficiency["α"] = efficiencyParameterNow[0].Efficiency;
                                }
                                //报警计数器归0
                                AlarmCount = 0;
                            }
                            #endregion
                        }
                        if (FactoryParameter.MeasureType == "β" || FactoryParameter.MeasureType == "α/β")
                        {
                            IList<ProbeParameter> probeParmeterOfBeta = clothesProbeParmeter.Where(probeParmeter => probeParmeter.NuclideType == "β").ToList();
                            #region 如果减去本底值后的测量值大于报警阈值，说明有污染
                            if (SmoothedDataOfBeta > probeParmeterOfBeta[0].Alarm_1 || SmoothedDataOfBeta > probeParmeterOfBeta[0].Alarm_2)
                            {
                                //报警次数+1
                                AlarmCount++;
                                //如果连续三次出现污染报警（污染报警计数器超过3）
                                //2020-07-31修改为出现1次即报警，且可以重复多次报警
                                //if (alarmCountOfClothes > 0 && isClothesContaminated == false)
                                if (AlarmCount > 0)
                                {
                                    AlarmCount = 0;//报警计数器归零
                                                   //将设备监测状态设置为“污染”
                                    if (SmoothedDataOfBeta > probeParmeterOfBeta[0].Alarm_2)
                                    {
                                        DeviceStatus = DeviceStatus.OperatingContaminated_2;
                                    }
                                    else
                                    {
                                        DeviceStatus = DeviceStatus.OperatingContaminated_1;
                                    }
                                }
                                eventArgs.PreSet["β"] = SmoothedDataOfBeta > probeParmeterOfBeta[0].Alarm_2 ? probeParmeterOfBeta[0].Alarm_2 : probeParmeterOfBeta[0].Alarm_1;
                            }
                            #endregion
                            #region 如果减去本底值后的测量值未大于报警阈值，说明没有污染
                            else
                            {
                                UnAlarmCount++;//衣物检测正常计数器+1
                                if (UnAlarmCount > 2)//超过3次，界面背景设置为正常状态
                                {
                                    this.DeviceStatus = DeviceStatus.OperatingNormally;
                                }
                                //从探测效率参数列表中查找当前用户选择的的衣物探测核素的探测效率参数
                                IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "β" && efficiencyParameter.Channel.ChannelID == 3 && efficiencyParameter.NuclideName == BetaNuclideUsed).ToList();
                                if (efficiencyParameterNow.Count > 0)
                                {
                                    eventArgs.Efficiency["β"] = efficiencyParameterNow[0].Efficiency;
                                }
                                //报警计数器归0
                                AlarmCount = 0;
                            }
                            #endregion
                        }
                        eventArgs.DeviceStatus = DeviceStatus;
                        eventArgs.IsFriskerIndependent = false;
                        eventArgs.MeasureType = FactoryParameter.MeasureType;
                        //通过事件响应在衣物探测界面显示实时测量值并设置界面背景颜色、检测进度百分比                        
                        DisplayDataEvent?.Invoke(this, eventArgs);
                        //在测量主界面显示当前测量值及进行测量状态控制
                        //ShowMsgEvent?.Invoke(this, eventArgs);
                        return 1;
                    }
                }
            }
            //衣物探头还未拿起或刚刚放下（红外状态为未到位）
            if (measureDataS[6].InfraredStatus == 0)
            {
                //衣物探头刚刚被放下---------衣物探头状态之前为已经被拿，现在为红外不到位，说明衣物探头刚刚被放下
                if (FriskerStatus == FriskerStatus.PickUP)
                {
                    //设置衣物探头状态为已经被放下
                    FriskerStatus = FriskerStatus.NotPickUp;
                    //设置衣物检测完成标志为true
                    IsTested = true;
                    eventArgs.MeasureValue["α"] = SmoothedDataOfAlpha;
                    eventArgs.MeasureValue["β"] = SmoothedDataOfBeta;
                    //在测量主界面显示当前测量值及进行测量状态控制
                    ShowMsgEvent?.Invoke(this, eventArgs);
                    return 2;
                }
                //衣物探头已经被放下一段时间
                if (stateCurrent.GetType() == typeof(StateBackgroundMeasure) || stateCurrent.GetType() == typeof(StateReadyToMeasure) || stateCurrent.GetType() == typeof(StateHandFootMeasuring))
                {
                    //将当前平滑处理后的检测值作为本底值
                    _baseDataOfAlpha = SmoothedDataOfAlpha;
                    _baseDataOfBeta = SmoothedDataOfBeta;
                    eventArgs.MeasureValue["α"] = SmoothedDataOfAlpha;
                    eventArgs.MeasureValue["β"] = SmoothedDataOfBeta;
                    //在测量主界面显示当前测量值及进行测量状态控制
                    ShowMsgEvent?.Invoke(this, eventArgs);
                    return 1;
                }
            }
            return 1;            
        }
    }
}

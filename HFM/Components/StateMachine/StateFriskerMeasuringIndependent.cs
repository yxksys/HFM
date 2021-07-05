using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using HFM.Components;

namespace HFM.Components
{
    class StateFriskerMeasuringIndependent:State,IStateFrisker
    {
        //状态处理过程中用于在衣物探测界面进行测量信息显示的事件委托
        public delegate void DisplayDataHander(object sender, FriskerEventArgs eventAtgs);
        //状态处理过程中用于在衣物探测界面进行测量信息显示的事件
        public event DisplayDataHander DisplayDataEvent;        
        //状态处理过程中用于在主探测界面进行语音或文字提示事件
        public event DisplayDataHander ShowMsgEvent;
        protected int TEAM_LENGTH = 240;
        //泊松参数
        protected const double POISSONUA_2 = 1.658;
        protected const double POISSONUA = 2.5758;
        protected const double POISSONUA2_4 = 0.676;
        protected const int LONGTIME = 60;
        protected FriskerEventArgs eventArgs = new FriskerEventArgs();
        protected struct SMOOTHINGDATA
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
        protected SMOOTHINGDATA _smoothingData = new SMOOTHINGDATA();
        protected float[] poissonTable = new float[50]{ 0, 4, 0, 6, 0, 8, 0,10, 0,11,
                                              1,13, 1,14, 2,16, 2,17, 3,18,
                                              4,20, 4,21, 5,22, 6,24, 6,25,
                                              7,26, 8,28, 8,29, 9,30,10,31,
                                             11,33,11,34,12,35,13,36,14,38 };//泊松表2
        float _baseData = 0;//衣物探头本底值
        public float SmoothedData { get; set; } = 0;//平滑处理后的衣物测量值
        public float SmoothedDataOfAlpha { get; set; } = 0;
        public float SmoothedDataOfBeta { get; set; } = 0;
        protected string appPath = Application.StartupPath;//应用系统安装路径
        public StateFriskerMeasuringIndependent(FrmClothes frmClothes)
        {                        
            _smoothingData.team = new UInt32[TEAM_LENGTH];
        }
        public StateFriskerMeasuringIndependent()
        { }
        public string NuclideUsed { get; set; }="U-235";//衣物检测核素选择,默认U-235
        public bool IsTested { get; set; } = false;
        public FriskerStatus FriskerStatus { get; set; } = FriskerStatus.NotPickUp;
        public int OfflineTimeCount { get; set; } = 0;
        public int AlarmCount { get; set; } = 0;
        public int UnAlarmCount { get; set; } = 0;        

        private void BaseSmooth()
        {
            _smoothingData.baseData_Full = 1;
            _smoothingData.baseData_i = 0;
            _smoothingData.baseSum = 2000;
            _smoothingData.baseTime = new UInt32[10];
            for (int i = 0; i < 10; i++)
            {
                _smoothingData.baseTime[i] = 200;
            }
        }
        /// <summary>
        /// 对衣物探头监测数据做平滑处理
        /// </summary>
        protected float SmoothData(UInt32 data)
        {
            float smoothedData = 0;
            float region_L = 0;
            float region_H = 0;
            int status = 0;
            int mutation_i = 0;//突变
            if (_smoothingData.average <= 1 && data < 4)//平均值小于1，下次计数小于4
            {
                if (_smoothingData.team_Full == 1)
                {
                    if (_smoothingData.team_i >= TEAM_LENGTH)
                    {
                        _smoothingData.team_i = 0;
                    }
                    _smoothingData.sum = _smoothingData.sum + data - _smoothingData.team[_smoothingData.team_i];
                    _smoothingData.team[_smoothingData.team_i] = data;
                    smoothedData = (float)_smoothingData.sum / TEAM_LENGTH;
                    _smoothingData.average = (UInt32)smoothedData;
                    _smoothingData.team_i++;
                }
                else
                {
                    _smoothingData.team[_smoothingData.team_i] = data;
                    _smoothingData.sum += data;
                    _smoothingData.team_i++;
                    smoothedData = (float)_smoothingData.sum / (_smoothingData.team_i + 1);
                    _smoothingData.average = (UInt32)smoothedData;
                    if (_smoothingData.team_i == TEAM_LENGTH)
                    {
                        _smoothingData.team_Full = 1;
                    }
                }
                //10s泊松分布判断
                _smoothingData.s10_Sum += data;
                _smoothingData.s10_Sum_i++;
                if (_smoothingData.s10_Sum_i == 10)
                {
                    //10s不满足泊松分布
                    if (_smoothingData.s10_Sum < poissonTable[_smoothingData.s10_Sum_Last * 2] || _smoothingData.s10_Sum > poissonTable[_smoothingData.s10_Sum_Last * 2 + 1])
                    {
                        _smoothingData.team_Full = 0;
                        _smoothingData.s10_Sum_Last = _smoothingData.s10_Sum;
                        //将最后10组数放入新数组队列前面，算出平均值
                        _smoothingData.sum = 0;
                        for (int i = 0; i < 9; i++)
                        {
                            mutation_i = (int)(_smoothingData.team_i + TEAM_LENGTH - i) % TEAM_LENGTH;
                            _smoothingData.team[9 - i] = _smoothingData.team[mutation_i];
                            _smoothingData.sum = _smoothingData.sum + _smoothingData.team[9 - i];
                        }
                        smoothedData = (float)_smoothingData.sum / 10;
                        _smoothingData.average = (UInt32)smoothedData;
                        _smoothingData.team_i = 10;
                    }
                    _smoothingData.s10_Sum_i = 0;
                    _smoothingData.s10_Sum = 0;
                }
            }
            //泊松表比较
            else if (_smoothingData.average < 25)
            {
                _smoothingData.s10_Sum_i = 0;
                _smoothingData.s10_Sum = 0;
                switch (_smoothingData.count_to)
                {
                    case 0:
                        if (data > poissonTable[_smoothingData.average * 2 + 1])
                        {
                            _smoothingData.count_to = 0x10;
                        }
                        if (data < poissonTable[_smoothingData.average * 2])
                        {
                            _smoothingData.count_to = 0x20;
                        }
                        break;
                    case 0x10:
                        if (data > poissonTable[_smoothingData.average * 2 + 1])
                        {
                            status = 1;
                        }
                        _smoothingData.count_to = 0;
                        break;
                    case 0x20:
                        if (data < poissonTable[_smoothingData.average * 2])
                        {
                            status = 1;
                        }
                        _smoothingData.count_to = 0;
                        break;
                    default:
                        _smoothingData.count_to = 0;
                        break;
                }
                if (status == 1)
                {
                    _smoothingData.team[0] = data;
                    _smoothingData.average = data;
                    _smoothingData.sum = data;
                    _smoothingData.team_i = 0;
                    _smoothingData.team_Full = 0;
                    smoothedData = (float)data;
                    status = 0;
                }
                else
                {
                    if (_smoothingData.team_Full == 1)
                    {
                        if (_smoothingData.team_i == TEAM_LENGTH)
                        {
                            _smoothingData.team_i = 0;
                        }
                        _smoothingData.sum = _smoothingData.sum + data - _smoothingData.team[_smoothingData.team_i];
                        _smoothingData.team[_smoothingData.team_i] = data;
                        smoothedData = (float)_smoothingData.sum / TEAM_LENGTH;
                        _smoothingData.average = (UInt32)smoothedData;
                        _smoothingData.team_i++;
                    }
                    else
                    {
                        _smoothingData.team[_smoothingData.team_i] = data;
                        _smoothingData.sum += data;
                        _smoothingData.team_i++;
                        smoothedData = (float)_smoothingData.sum / (_smoothingData.team_i + 1);
                        _smoothingData.average = (UInt32)smoothedData;
                        if (_smoothingData.team_i == TEAM_LENGTH)
                        {
                            _smoothingData.team_Full = 1;
                        }
                    }
                }
            }
            //泊松公式比较
            else
            {
                _smoothingData.s10_Sum_i = 0;
                region_L = _smoothingData.average - (float)0.5 + (float)POISSONUA_2;
                region_L = (float)Math.Sqrt(region_L);
                region_L = (float)POISSONUA - region_L * 2;
                region_L = (region_L / 2) * (region_L / 2);
                region_H = _smoothingData.average + (float)0.5 + (float)POISSONUA_2;
                region_H = (float)Math.Sqrt(region_H);
                region_H = (float)POISSONUA + region_H * 2;
                region_H = (region_H / 2) * (region_H / 2);
                switch (_smoothingData.count_to)
                {
                    case 0:
                        if (data > region_H)
                        {
                            _smoothingData.count_to = 0x10;
                        }
                        if (data < region_L)
                        {
                            _smoothingData.count_to = 0x20;
                        }
                        break;
                    case 0x10:
                        if (data > region_H)
                        {
                            status = 1;
                        }
                        _smoothingData.count_to = 0;
                        break;
                    case 0x20:
                        if (data < region_L)
                        {
                            status = 1;
                        }
                        _smoothingData.count_to = 0;
                        break;
                    default:
                        _smoothingData.count_to = 0;
                        break;
                }
                if (status == 0)
                {
                    if (_smoothingData.team_Full == 1)
                    {
                        if (_smoothingData.team_i == TEAM_LENGTH)
                        {
                            _smoothingData.team_i = 0;
                        }
                        _smoothingData.sum = _smoothingData.sum + data - _smoothingData.team[_smoothingData.team_i];
                        _smoothingData.team[_smoothingData.team_i] = data;
                        smoothedData = (float)_smoothingData.sum / TEAM_LENGTH;
                        _smoothingData.average = (UInt32)smoothedData;
                        _smoothingData.team_i++;
                    }
                    else
                    {
                        _smoothingData.team[_smoothingData.team_i] = data;
                        _smoothingData.sum += data;
                        _smoothingData.team_i++;
                        smoothedData = (float)_smoothingData.sum / (_smoothingData.team_i + 1);
                        _smoothingData.average = (UInt32)smoothedData;
                        if (_smoothingData.team_i == TEAM_LENGTH)
                        {
                            _smoothingData.team_Full = 1;
                        }
                    }
                }
                else
                {
                    _smoothingData.team[0] = data;
                    _smoothingData.average = data;
                    _smoothingData.sum = data;
                    smoothedData = (float)data;
                    _smoothingData.team_i = 0;
                    _smoothingData.team_Full = 0;
                    status = 0;
                }
            }
            if (data == 0)
            {
                _smoothingData.lingCycle++;
            }
            else
            {
                _smoothingData.lingCycle = 0;
            }
            if (_smoothingData.lingCycle > LONGTIME)
            {
                _smoothingData.lingCycle = 0;
                smoothedData = 0;
                _smoothingData.baseData_i = 0;
                _smoothingData.baseData_Full = 0;
                smoothedData = (float)3.3e-4;
            }
            if (smoothedData > 100000)
            {
                File.AppendAllText(appPath + "\\log\\errorLog.txt", "sum=" + _smoothingData.sum + "; team_i =" + _smoothingData.team_i + ";");
            }
            return smoothedData;
        }
        public virtual int Run<T1, T2, T3, T4>(T1 Args1, T2 Args2, T3 Args3, T4 Args4)
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
            IList <EfficiencyParameter> efficiencyParameterS= (IList<EfficiencyParameter>)Args4;           
            //对采集到的衣物数据做平滑处理
            try
            {
                SmoothedData = SmoothData((UInt32)measureDataS[6].Beta);
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
                    IList<ProbeParameter> clothesProbeParmeter = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == 7 && probeParmeter.NuclideType=="C" ).ToList();
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
                        SmoothedData = (SmoothedData - _baseData) < 0 ? 0 : SmoothedData - _baseData;
                        //为衣物探测界面显示本底值(单位cps)和测量值，将当前本底值、测量值赋值给事件参数类                            
                        eventArgs.BackgroundValue["C"] = _baseData;
                        eventArgs.MeasureValue["C"] = SmoothedData;
                        #region 如果减去本底值后的测量值大于报警阈值，说明有污染                          
                        if (SmoothedData > clothesProbeParmeter[0].Alarm_1 || SmoothedData > clothesProbeParmeter[0].Alarm_2)
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
                                if (SmoothedData > clothesProbeParmeter[0].Alarm_2)
                                {
                                    DeviceStatus = DeviceStatus.OperatingContaminated_2;
                                }
                                else
                                {
                                    DeviceStatus = DeviceStatus.OperatingContaminated_1;
                                }                                                                
                                return 1;
                            }                            
                            eventArgs.PreSet["C"] = SmoothedData > clothesProbeParmeter[0].Alarm_2 ? clothesProbeParmeter[0].Alarm_2 : clothesProbeParmeter[0].Alarm_1;                                             
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
                            IList<EfficiencyParameter> efficiencyParameterNow = efficiencyParameterS.Where(efficiencyParameter => efficiencyParameter.NuclideType == "C" && efficiencyParameter.Channel.ChannelID == 7 && efficiencyParameter.NuclideName == NuclideUsed).ToList();
                            eventArgs.Efficiency["C"] = efficiencyParameterNow[0].Efficiency;                                                                                                                                           
                            //报警计数器归0
                            AlarmCount = 0;                                                                             
                        }
                        #endregion
                        eventArgs.DeviceStatus = DeviceStatus;
                        eventArgs.IsFriskerIndependent = true;
                        //通过事件响应在衣物探测界面显示实时测量值并设置界面背景颜色、检测进度百分比                        
                        DisplayDataEvent?.Invoke(this, eventArgs);
                        //在测量主界面显示当前测量值及进行测量状态控制
                        //ShowMsgEvent?.Invoke(this, eventArgs);
                        return 1;
                        
                    }                    
                }
            }
            //衣物探头还未拿起或刚刚放下（红外状态为未到位）
            if(measureDataS[6].InfraredStatus == 0)
            {
                //衣物探头刚刚被放下---------衣物探头状态之前为已经被拿，现在为红外不到位，说明衣物探头刚刚被放下
                if (FriskerStatus==FriskerStatus.PickUP)
                {
                    //设置衣物探头状态为已经被放下
                    FriskerStatus = FriskerStatus.NotPickUp;
                    //设置衣物检测完成标志为true
                    IsTested = true;
                    eventArgs.MeasureValue["C"] = SmoothedData;
                    //在测量主界面显示当前测量值及进行测量状态控制
                    ShowMsgEvent?.Invoke(this, eventArgs);
                    return 2;
                }
                //衣物探头已经被放下一段时间
                if (stateCurrent.GetType()==typeof(StateBackgroundMeasure)|| stateCurrent.GetType() == typeof(StateReadyToMeasure)|| stateCurrent.GetType() == typeof(StateHandFootMeasuring))
                {
                    //将当前平滑处理后的检测值作为本底值
                    _baseData = SmoothedData;
                    eventArgs.MeasureValue["C"] = SmoothedData;
                    //在测量主界面显示当前测量值及进行测量状态控制
                    ShowMsgEvent?.Invoke(this, eventArgs);
                    return 1;
                }                
            }
            return 1;
        }
    }
}

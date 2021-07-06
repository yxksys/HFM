using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    public class State : IState
    {        
        protected int dataCount = 0;
        int _setTimes;//状态保持时间长度设置
        DateTime _lastTime;//状态上次运行时间
        int _holdTimes;//状态保持时间长度，用来记录各个状态剩余时间        
        static int _alarmTimes;//状态报警时间长度   
        static DateTime _alarmTimeStart; //状态报警开始时间
        static DeviceStatus _deviceStatus;//当前状态：正常、故障、一级污染、高阶污染
        static Dictionary<Language,string> _errRecord=new Dictionary<Language, string>();//错误信息
        static bool _isShowMsg;//当前状态是否进行了有效的信息提示（语音播报）
        static SystemParameter _systemParameter;
        static FactoryParameter _factoryParameter;
        //手部探测器检测进度0：还未开始检测；1：手部第一次检测完成；2：手部第二次检测完成；3：手部探测器进行衣物检测
        //由于单探测器时，手心手背必须分两次进行检测，手部第一次检测完成后，根据该标志进行语音提示，然后进该标志置为1，第二次检测完成后置2。
        static HandProbeCtrl _handProbeCtrl = 0;
        static IList<MeasureData> _baseData = new List<MeasureData>();//本底数据
        public string Name { get; set; } = "ReadyToRun";

        public IList<IState> Nexts { get; set; } = new List<IState>();
        public Func<IState, IState> Selector { get; set; }
        public SoundMessage SoundMessage { get; set; } = SoundMessage.Mute;
        //状态保持时间
        public int HoldTimes { get => _holdTimes; set => _holdTimes = value; }
        public static int AlarmTimes { get => _alarmTimes; set => _alarmTimes = value; }
        public static DateTime AlarmTimeStart { get => _alarmTimeStart; set => _alarmTimeStart = value; }
        public int SetTimes { get => _setTimes; set => _setTimes = value; }
        public DateTime LastTime { get => _lastTime; set => _lastTime = value; }
        public bool IsShowMsg { get=> _isShowMsg; set=> _isShowMsg = value; }
        //系统参数
        public SystemParameter SystemParameter { get=> _systemParameter; set=> _systemParameter=value; }
        //工厂参数
        public FactoryParameter FactoryParameter { get=> _factoryParameter; set=> _factoryParameter=value; }
        //经过计算处理后的数据
        public IList<MeasureData> CalculatedMeasureDataS { get; set; } = new List<MeasureData>();
        
        public DeviceStatus DeviceStatus { get => _deviceStatus; set => _deviceStatus = value; }
        //手部探测器检测进度0：还未开始检测；1：手部第一次检测完成手部还未翻转（不能进行第二次手部检测）；2：手部第一次检测完成手部已经翻转（可以进行第二次手部检测）；3：手部第二次检测完成；4：手部探测器进行衣物检测
        //由于单探测器时，手心手背必须分两次进行检测，手部第一次检测完成后，根据该标志进行语音提示，然后进该标志置为1，第二次检测完成后置2。
        public HandProbeCtrl HandProbeCtrl { get=> _handProbeCtrl; set=> _handProbeCtrl = value; }
        public IList<MeasureData> BaseData { get => _baseData; set => _baseData = value; } //存储本底计算结果，用来对测量数据进行校正
        public Dictionary<Language,string> ErrRecord { get => _errRecord; set => _errRecord = value; }

        public virtual bool Run<T>(IState current, IState previous,ref T Args)
        {
            return false;
        }
        public virtual int Run<T1,T2,T3>(T1 Args1,T2 Args2,T3 Args3)
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
            return 1;
        }
        //本底判断
        public virtual Dictionary<Language,string> BaseCheck<T1,T2,T3>(T1 Args1,T2 Args2,T3 Args3)
        {
            bool isCheck = true;
            string errRecord = null;
            string errRecord_E = null;
            IList<Channel> channelS = (IList<Channel>)Args1;
            IList<ProbeParameter> probeParameterS = (IList<ProbeParameter>)Args2;
            UIEventArgs uIEventArgs = Args3 as UIEventArgs;
            if (this.Name == "BackGroundMeasure")
            {
                //求测量本底的平均值
                foreach (MeasureData data in CalculatedMeasureDataS)
                {
                    data.Alpha /= dataCount;
                    data.Beta /= dataCount;
                }
                dataCount = 0;
            }
            foreach (Channel channel in channelS)
            {
                //先设置当前通道状态为正常
                uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingNormally;
                IList<MeasureData> calculatedMeasureDataNow = this.CalculatedMeasureDataS.Where(calculatedMeasureData => calculatedMeasureData.Channel.ChannelID == channel.ChannelID).ToList();
                if (this.FactoryParameter.MeasureType!= "β")
                {
                    if(channel.ChannelID!=7)
                    {
                        //查询当前通道的α本底上限、本底下限（从探测参数列表中找到当前通道的"α"探测参数）                       
                        IList<ProbeParameter> probeParameterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channel.ChannelID && probeParmeter.NuclideType == "α").ToList();                                               
                        //进行α本底测量判断
                        if (calculatedMeasureDataNow[0].Alpha < probeParameterNow[0].LBackground) //超过当前通道的本底下限
                        {
                            //设置当前通道状态为故障
                            uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                            //该通道channelS[i].ChannelName的本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("{0}α本底下限值:{1}cps,当前本底值:{2}cps;\r\n", channel.ChannelName, probeParameterNow[0].LBackground.ToString("F2"), calculatedMeasureDataNow[0].Alpha.ToString("F2"));
                            errRecord_E += string.Format("{0} α Low Background Threshold{1}cps,Actual Background:{2}cps;\r\n", channel.ChannelName_English, probeParameterNow[0].LBackground.ToString("F2"), calculatedMeasureDataNow[0].Alpha.ToString("F2"));
                            isCheck = false;
                        }                        
                        if (calculatedMeasureDataNow[0].Alpha >= probeParameterNow[0].HBackground)//超过当前通道的本底上限
                        {
                            //设置当前通道状态为故障
                            uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                            //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                            errRecord += string.Format("{0}α本底上限值:{1}cps,当前本底值:{2}cps;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName, probeParameterNow[0].HBackground.ToString("F2"), calculatedMeasureDataNow[0].Alpha.ToString("F2"));
                            errRecord_E += string.Format("{0} α High Background Threshold{1}cps,Actual Background:{2}cps;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName_English, probeParameterNow[0].HBackground.ToString("F2"), calculatedMeasureDataNow[0].Alpha.ToString("F2"));
                            isCheck = false;
                        }
                    }
                }
                if (this.FactoryParameter.MeasureType != "α")
                {
                    /*****************************************************************************
                     * 由于衣物探头本底值并不是使用平滑因子算法进行计算，是使用泊松分布算法进行计算
                     * 所以如果当前通道是衣物探头，则将衣物探头当前本底值赋值给通道进行本底判断
                    // *****************************************************************************/
                    //if (channelS[i].ChannelID == 7)
                    //{
                    //    calculatedMeasureDataS[i].Beta = baseDataOfClothes;
                    //    File.AppendAllText(appPath + "\\log\\background.txt", "本底判断，衣物通道：" + calculatedMeasureDataS[i].Channel.ChannelID.ToString() + ";Beta:" + calculatedMeasureDataS[i].Beta.ToString() + "\r\n");
                    //}                       
                    //查询当前通道的β本底上限、本底下限
                    //probeParameter.GetParameter(channelS[i].ChannelID, "β");
                    //查询当前通道的β本底上限、本底下限（从探测参数列表中找到当前通道的"β"探测参数）                       
                    IList<ProbeParameter> probeParameterNow = probeParameterS.Where(probeParmeter => probeParmeter.ProbeChannel.ChannelID == channel.ChannelID && (probeParmeter.NuclideType == "β" || probeParmeter.NuclideType == "C")).ToList();
                    if (calculatedMeasureDataNow[0].Beta < probeParameterNow[0].LBackground)//超过当前通道的本底下限
                    {
                        //设置当前通道状态为故障
                        uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                        //该通道channelS[i].ChannelName本底下限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                        errRecord += string.Format("{0} β本底下限值:{1}cps,当前本底值:{2}cps;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName, probeParameterNow[0].LBackground.ToString("F2"), calculatedMeasureDataNow[0].Beta.ToString("F2"));
                        errRecord_E += string.Format("{0} β Low Background Threshold{1}cps,Actual Background:{2}cps;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName_English, probeParameterNow[0].LBackground.ToString("F2"), calculatedMeasureDataNow[0].Beta.ToString("F2"));
                        isCheck = false;
                    }
                    if (calculatedMeasureDataNow[0].Beta >= probeParameterNow[0].HBackground)//超过当前通道的本底上限
                    {
                        //设置当前通道状态为故障
                        uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                        //该通道channelS[i].ChannelName本底上限值，当前本底值添加到错误信息串errRecord。置isCheck=false
                        errRecord += string.Format("{0}β本底上限值:{1}cps,当前本底值:{2}cps;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName, probeParameterNow[0].HBackground.ToString("F2"), calculatedMeasureDataNow[0].Beta.ToString("F2"));
                        errRecord_E += string.Format("{0} β High Background Threshold{1}cps,Actual Background:{2}cps;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName_English, probeParameterNow[0].HBackground.ToString("F2"), calculatedMeasureDataNow[0].Beta.ToString("F2"));
                        isCheck = false;
                    }
                }
            }
            if (isCheck == false)//未通过
            {
                Dictionary<Language, string> errRecordS = new Dictionary<Language, string>();
                errRecordS.Add(Language.Chinese, errRecord);
                errRecordS.Add(Language.English, errRecord_E);
                return errRecordS;
            }
            else
            {
                return null;
            }
        }        
    }
}

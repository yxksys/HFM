using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    class StateSelfCheck:State
    {
        const int BASE_DATA = 1000;//标准本底值
        /// <summary>
        /// 数据处理，每1s对采集的数据进行1次处理
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
            //仪器自检的过程实现
            IList<Channel> channelS=(IList<Channel>)Args1;
            IList<MeasureData> measureDataS = (IList<MeasureData>)Args2;
            for (int i = 0; i< channelS.Count; i++) //遍历全部启用的检测通道
            {
                /*因为measureDataS中是从报文协议中解析的全部7个通道的监测数据，但是calculatedMeasureDataS只是存储当前在用的通道信息
                *所以，需要从measureDataS中找到对应通道的监测数据赋值给calculatedMeasureDataS，但是本地值Alpha和Beta需要累加*/
                //从解析的7个通道的measureDataS监测数据中找到当前通道的测量数据
                List<MeasureData> list = measureDataS.Where(measureData => measureData.Channel.ChannelID == channelS[i].ChannelID).ToList();
                if (list.Count > 0)
                {                    
                    //计算每个通道上传的Alpha和Beta计数累加(是指全部启用的通道)                    
                    this.CalculatedMeasureDataS[i].Alpha += list[0].Alpha;
                    this.CalculatedMeasureDataS[i].Beta += list[0].Beta;
                    //将当前通道本次的其它测量数据赋值给calculatedMeasureDataS
                    this.CalculatedMeasureDataS[i].AnalogV = list[0].AnalogV;
                    this.CalculatedMeasureDataS[i].DigitalV = list[0].DigitalV;
                    this.CalculatedMeasureDataS[i].HV = list[0].HV;
                    this.CalculatedMeasureDataS[i].InfraredStatus = list[0].InfraredStatus;
                    this.CalculatedMeasureDataS[i].IsEnglish = list[0].IsEnglish;
                    this.CalculatedMeasureDataS[i].MeasureDate = DateTime.Now;
                }
            }            
            return 1;
        }
        public override Dictionary<Language,string> BaseCheck<T1,T2,T3>(T1 Args1,T2 Args2,T3 Args3)
        {
            bool isCheck = true;
            string errRecord = null;
            string errRecord_E = null;            
            IList<Channel> channelS = (IList<Channel>)Args1;
            IList<ChannelParameter> channelParameterS = (IList<ChannelParameter>)Args2;
            UIEventArgs uIEventArgs = Args3 as UIEventArgs;
            foreach(Channel channel in channelS)
            {
                //如果是单探测器，则将左右手背跳过不进行判断
                if (this.FactoryParameter.IsDoubleProbe == false)
                {
                    if (channel.ChannelID == 2 || channel.ChannelID == 4)
                    {
                        continue;
                    }
                }
                if (channel.ChannelID == 7)//对衣物探头不做判断
                {
                    continue;
                }
                IList<ChannelParameter> channelParameterNow = channelParameterS.Where(channelParameter => channelParameter.Channel.ChannelID == channel.ChannelID).ToList();
                IList<MeasureData> calculatedMeasureDataNow = this.CalculatedMeasureDataS.Where(calculatedMeasureData => calculatedMeasureData.Channel.ChannelID == channel.ChannelID).ToList();
                //判断当前高压值：当前高压值若为系统设定值的0.8~1.2倍之内，则该通道高压正常，否则高压故障,将故障信息添加到errRecord字符串，isCheck = false;
                if (calculatedMeasureDataNow[0].HV < channelParameterNow[0].PresetHV * (1 - PlatForm.ErrorRange.HV_ERROR) || calculatedMeasureDataNow[0].HV > channelParameterNow[0].PresetHV * (1 + PlatForm.ErrorRange.HV_ERROR))
                {
                    //设置当前通道状态为故障
                    uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                    //高压故障,将故障信息添加到errRecord字符串
                    errRecord += string.Format("{0}高压故障,设置值:{1}V,实测值:{2}V;\r\n", channel.ChannelName, channelParameterNow[0].PresetHV.ToString("F2"), calculatedMeasureDataNow[0].HV.ToString("F2"));
                    errRecord_E += string.Format("{0}HV Fault,Preset:{1}V,Actual:{2}V;\r\n", channel.ChannelName_English, channelParameterNow[0].PresetHV.ToString("F2"), calculatedMeasureDataNow[0].HV.ToString("F2"));
                    //设置isCheck为false
                    isCheck = false;
                }
                else
                {
                    //设置当前通道状态为正常
                    uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingNormally;
                }
                //根据仪器检测类型计算最终自检数据
                switch (this.FactoryParameter.MeasureType)
                {
                    case "α":
                        //计算Alpha自检数据值
                        calculatedMeasureDataNow[0].Alpha = (calculatedMeasureDataNow[0].Alpha + BASE_DATA * 2) / SetTimes * 2;
                        break;
                    case "β":
                        //计算Beta自检数据
                        calculatedMeasureDataNow[0].Beta = (calculatedMeasureDataNow[0].Beta + BASE_DATA * 2) / SetTimes * 2;
                        break;
                    case "α/β":
                        //同时计算Alpha和Beta自检数据
                        calculatedMeasureDataNow[0].Alpha = (calculatedMeasureDataNow[0].Alpha + BASE_DATA * 2) / SetTimes * 2;
                        calculatedMeasureDataNow[0].Beta = (calculatedMeasureDataNow[0].Beta + BASE_DATA * 2) / SetTimes * 2;
                        break;
                }
                //判断电子线路故障：如果本底值在设定值的0.7~1.3倍之内，则显示电子线路正常，否则显示电子线路故障
                if (this.FactoryParameter.MeasureType != "β")
                {
                    //对Alpha本底值(BASE_DATA)进行判断，如果故障提示“α线路故障”同时将故障信息添加到errRecord字符串，isCheck = false;
                    if (calculatedMeasureDataNow[0].Alpha < BASE_DATA * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataNow[0].Alpha > BASE_DATA * (1 + PlatForm.ErrorRange.BASE_ERROR))
                    {
                        //设置当前通道状态为故障
                        uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                        //将故障信息添加到error字符串
                        errRecord += string.Format("{0}α电子线路故障;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName);
                        errRecord_E += string.Format("{0}Alpha Channel Fault;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName_English);
                        //设置isCheck为false
                        isCheck = false;
                    }                    
                }
                if (this.FactoryParameter.MeasureType != "α")
                {
                    if (calculatedMeasureDataNow[0].Beta < BASE_DATA * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataNow[0].Beta > BASE_DATA * (1 + PlatForm.ErrorRange.BASE_ERROR))
                    //对Beta本底值(BASE_DATA)进行判断，如果故障提示“β线路故障”同时将故障信息添加到errRecord字符串,isCheck = false;
                    // if (calculatedMeasureDataS[i].Beta < channelParameterNow[0].BetaThreshold * (1 - PlatForm.ErrorRange.BASE_ERROR) || calculatedMeasureDataS[i].Beta > channelParameterNow[0].BetaThreshold * (1 + PlatForm.ErrorRange.BASE_ERROR))
                    {
                        //设置当前通道状态为故障
                        uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                        //将故障信息添加到error字符串
                        errRecord += string.Format("{0}β电子线路故障;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName);
                        errRecord_E += string.Format("{0}Beta Channel Fault;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName_English);
                        //设置isCheck为false
                        isCheck = false;
                    }                    
                }
                //判断根据红外状态判断该通道红外是否故障，自检时无检查提示但红外到位，说明红外故障
                if (calculatedMeasureDataNow[0].InfraredStatus == 1)//红外状态到位
                {
                    //设置当前通道状态为故障
                    uIEventArgs.MeasureStatus[channel] = DeviceStatus.OperatingFaulted;
                    //红外故障,将故障信息添加到errRecord字符串
                    errRecord += string.Format("{0}红外故障;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName);
                    errRecord_E += string.Format("{0}Sensor fault;\r\n", calculatedMeasureDataNow[0].Channel.ChannelName_English);
                    isCheck = false;
                }
            }
            if (isCheck == false)//未通过
            {
                Dictionary<Language,string> errRecordS = new Dictionary<Language, string>();
                errRecordS.Add(Language.Chinese,errRecord);
                errRecordS.Add(Language.English,errRecord_E);                
                return errRecordS;
            }
            else
            {
                return null;
            }
        }        
    }
}

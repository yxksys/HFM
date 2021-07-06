using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HFM.Components
{
    public class UIEventArgs:EventArgs
    {        
        //当前检测进度状态
        public IState CurrentState { get; set; }
        //当前设备状态：正常、故障、一级污染、高阶污染
        public DeviceStatus DeviceStatus { get; set; }        
        /// <summary>
        /// 用于控制界面各通道红外是否到位显示。key(Channel):通道，value(ChannelStatus)通道状态：-1通道未启用；0红外不到位；1红外到位
        /// </summary>
        public ObserverDictionary<Channel, ChannelStatus> ChannelStatus { get; set; } = new ObserverDictionary<Channel, ChannelStatus>();
        
        /// <summary>
        /// 检测状态，Key(Channel)通道,Value(DeviceStatus)设备状态：16：正常；32：故障；63：一级污染；64：二级污染
        /// </summary>
        public ObserverDictionary<Channel , DeviceStatus> MeasureStatus { get; set; } = new ObserverDictionary<Channel, DeviceStatus>();        
        //是否男生语音提示
        public bool isAudioMan { get; set; }               
    }
}

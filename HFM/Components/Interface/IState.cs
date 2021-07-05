using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{    
    public interface IState
    {           
        string Name { get; set; }
        //状态保持时间长度设置
        int SetTimes { get; set; }
        //状态上次运行时间
        DateTime LastTime { get; set; }
        //状态保持时间长度，用来记录各个状态剩余时间 
        int HoldTimes { get; set; }
        //报警时间长度
        //int AlarmTimes { get; set; }
        //状态报警开始时间
        //DateTime AlarmTimeStart { get; set; }
        //计算处理后的测量数据
        IList<MeasureData> CalculatedMeasureDataS { get; set; }
        SystemParameter SystemParameter { get; set; }
        FactoryParameter FactoryParameter { get; set; }
        HandProbeCtrl HandProbeCtrl { get; set; }
        //存储本底计算结果，用来对测量数据进行校正
        IList<MeasureData> BaseData { get; set; } 
        DeviceStatus DeviceStatus { get; set; }
        //当前状态是否进行了有效信息显示（语音播报）
        bool IsShowMsg { get; set; }
        SoundMessage SoundMessage { get; set; }
        //故障信息、污染信息
        Dictionary<Language,string> ErrRecord { get; set; }
        IList<IState> Nexts { get; set; }
        Func<IState, IState> Selector { get; set; }
        bool Run<T>(IState current, IState previous,ref T Args);
        /// <summary>
        /// 状态运行数据处理
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Args1"></param>
        /// <param name="Args2"></param>
        /// <returns>-1：未进行任何数据处理；0：数据处理异常；1：数据处理正常</returns>
        int Run<T1,T2,T3>(T1 Args1,T2 Args2,T3 Args3);
        Dictionary<Language,string> BaseCheck<T1,T2,T3>(T1 Args1,T2 Args2,T3 Args3);
    }
}

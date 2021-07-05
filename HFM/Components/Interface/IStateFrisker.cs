using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    interface IStateFrisker:IState
    {        
        float SmoothedData { get; set; }//平滑处理后的衣物测量值
        float SmoothedDataOfAlpha { get; set; }
        float SmoothedDataOfBeta { get; set; }
        //string NuclideUsed { get; set; }//核素使用（独立探头时有效）
        //string AlphaNuclideUsed { get; set; }//Alpha核素选择(和衣物共享探头时有效)
        //string BetaNuclideUsed { get; set; }//Beta核素选择(和衣物共享探头时有效)
        bool IsTested { get; set; }//检测是否完成
        FriskerStatus FriskerStatus { get; set; }//探头状态是否拿起
        int OfflineTimeCount { get; set; }//离线时间计数
        int AlarmCount { get; set; }//报警计数
        int UnAlarmCount { get; set; }//非报警计数
        int Run<T1, T2, T3, T4>(T1 Args1, T2 Args2, T3 Args3, T4 Args4);//状态运行数据处理
    }
}

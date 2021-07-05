using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    class StateResult:State
    {
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
            return 1;
        }
    }
}

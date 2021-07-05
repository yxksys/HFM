using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HFM.Components
{
    public class FriskerEventArgs : EventArgs
    {
        //是否独立探测器
        public bool IsFriskerIndependent { get; set; } = false;
        //探测类型
        public string MeasureType { get; set; } = "β";
        //探测参数
        public IList<ProbeParameter> ProbeParmeter { get; set; }
        //本底值
        public Dictionary<string,float> BackgroundValue { get; set; } =new Dictionary<string, float> { { "α", 0 }, { "β", 0 },{"C",0 } };
        //设定阈值
        public Dictionary<string,float> PreSet { get; set; } = new Dictionary<string, float> { { "α", 0 }, { "β", 0 }, { "C", 0 } };
        //测量值
        public Dictionary<string,float> MeasureValue { get; set; } = new Dictionary<string, float> { { "α", 0 }, { "β", 0 }, { "C", 0 } };
        //设备状态
        public DeviceStatus DeviceStatus { get; set; } = DeviceStatus.OperatingNormally;
        //探测效率参数
        public Dictionary<string,float> Efficiency { get; set; } = new Dictionary<string, float> { { "α", 0 }, { "β", 0 }, { "C", 0 } };
    }
}

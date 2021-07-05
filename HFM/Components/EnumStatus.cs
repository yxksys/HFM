using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    /// <summary>
    /// 设备状态：16正常；32故障；63一级污染；64高阶污染
    /// </summary>
    public enum DeviceStatus
    {
        OperatingNormally = 16, //正常
        OperatingFaulted = 32, //故障
        OperatingContaminated_1 = 63,  //一级污染
        OperatingContaminated_2 = 64  //高阶污染
    }
    /// <summary>
    /// 通道状态：-1通道未启用；0红外未到位；1红外到位
    /// </summary>
    public enum ChannelStatus
    {
        Disabled=-1, //通道未启用
        NotInPlace=0, //红外未到位
        InPlace=1  //红外到位
    }
    public enum Language
    {
        Chinese=0,
        English=1
    }    
    public enum SoundMessage
    {
        Mute=0,//不做任何提示
        Ready=1,//等待测量
        FlipPalm=2, //翻转手掌检测
        Leave=3,   //没有污染离开
        FriskerMeasure=4,  //衣物检测
        Measuring=5, //开始测量
        DiDa_1=6,//测量过程声音提示
        DiDa_2=7//测量结束声音提示
    }
    public enum HandProbeCtrl
    {
        StepOneReady=0,//手部第一次检测准备
        StepOneComplete=1,//手部第一次检测完成（手部还未翻转不能进行第二次手部检测）
        StepTwoReady =2,//手部第二次检测准备
        StepTwoComplete=3,//手部第二次检测完成
        FriskerReady=4//手部探测器用作衣物检测准备
    }
    /// <summary>
    /// 衣物探头状态
    /// </summary>
    public enum FriskerStatus
    {
        NotPickUp=0,//衣物探头还未拿起
        PickUP=1 //衣物探头已经拿起
    }
}

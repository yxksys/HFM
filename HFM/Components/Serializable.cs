using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HFM.Components
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot("root", IsNullable = false)]
    public class Serializable
    {
        /// <summary>
        /// 仪器编号
        /// </summary>
        public string DeviceNumber { get; set; }
        /// <summary>
        /// 仪器状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 仪器测试标志
        /// </summary>
        public string Test_Flag { get; set; }
        /// <summary>
        /// 参数更新标志
        /// </summary>
        public string Update_Flag { get; set; }
        /// <summary>
        /// 故障信息
        /// </summary>
        public string Fault { get; set; }
        /// <summary>
        /// 报警信息
        /// </summary>
        public string Alarm { get; set; }
        /// <summary>
        /// 测量单位
        /// </summary>
        public string TestUnit { get; set; }
        /// <summary>
        /// 测量时间
        /// </summary>
        public DateTime TestTime { get; set; }
        //当前核素信息
        [System.Xml.Serialization.XmlElementAttribute("NuclideUsed")]
        public NuclideUsed NuclideUsed { get; set; }
        //当前高压信息
        [System.Xml.Serialization.XmlElementAttribute("HighVoltage")]
        public HighVoltage HighVoltage { get; set; }
        //当前Alpha本底信息
        [System.Xml.Serialization.XmlElementAttribute("AlphaBackground")]
        public AlphaBackground AlphaBackground { get; set; }
        //当前Beta本底信息
        [System.Xml.Serialization.XmlElementAttribute("BetaBackground")]
        public BetaBackground BetaBackground { get; set; }
        //当前Alpha本底上限
        [System.Xml.Serialization.XmlElementAttribute("AlphaBackgroundUp")]
        public AlphaBackgroundUp AlphaBackgroundUp { get; set; }
        //当前Beta本底上限
        [System.Xml.Serialization.XmlElementAttribute("BetaBackgroundUp")]
        public BetaBackgroundUp BetaBackgroundUp { get; set; }
        //当前Alpha本底下限
        [System.Xml.Serialization.XmlElementAttribute("AlphaBackgroundDown")]
        public AlphaBackgroundDown AlphaBackgroundDown { get; set; }
        //当前Beta本底下限
        [System.Xml.Serialization.XmlElementAttribute("BetaBackgroundDown")]
        public BetaBackgroundDown BetaBackgroundDown { get; set; }
        //当前Alpha测量数据
        [System.Xml.Serialization.XmlElementAttribute("AlphaTestData")]
        public AlphaTestData AlphaTestData { get; set; }
        //当前Beta测量数据
        [System.Xml.Serialization.XmlElementAttribute("BetaTestData")]
        public BetaTestData BetaTestData { get; set; }
        //当前Alpha一级报警阈值
        [System.Xml.Serialization.XmlElementAttribute("AlphaAlarmThread")]
        public AlphaAlarmThread AlphaAlarmThread { get; set; }
        //当前Beta一级报警阈值
        [System.Xml.Serialization.XmlElementAttribute("BetaAlarmThread")]
        public BetaAlarmThread BetaAlarmThread { get; set; }
        //当前Alpha二级报警阈值
        [System.Xml.Serialization.XmlElementAttribute("AlphaHighAlarmThread")]
        public AlphaHighAlarmThread AlphaHighAlarmThread { get; set; }
        //当前Beta二级报警阈值
        [System.Xml.Serialization.XmlElementAttribute("BetaHighAlarmThread")]
        public BetaHighAlarmThread BetaHighAlarmThread { get; set; }
        //当前Alpha效率
        [System.Xml.Serialization.XmlElementAttribute("AlphaEfficiency")]
        public AlphaEfficiency AlphaEfficiency { get; set; }
        //当前Beta效率
        [System.Xml.Serialization.XmlElementAttribute("BetaEfficiency")]
        public BetaEfficiency BetaEfficiency { get; set; }
        //当前Alpha阈值
        [System.Xml.Serialization.XmlElementAttribute("AlphaThread")]
        public AlphaThread AlphaThread { get; set; }
        //当前Beta阈值
        [System.Xml.Serialization.XmlElementAttribute("BetaThread")]
        public BetaThread BetaThread { get; set; }
        //当前Alpha报警标志
        [System.Xml.Serialization.XmlElementAttribute("AlphaAlarmFlag")]
        public AlphaAlarmFlag AlphaAlarmFlag { get; set; }
        //当前Beta报警标志
        [System.Xml.Serialization.XmlElementAttribute("BetaAlarmFlag")]
        public BetaAlarmFlag BetaAlarmFlag { get; set; }
        //当前Alpha故障标志
        [System.Xml.Serialization.XmlElementAttribute("AlphaFailedFlag")]
        public AlphaFailedFlag AlphaFailedFlag { get; set; }
        //当前Beta故障标志
        [System.Xml.Serialization.XmlElementAttribute("BetaFailedFlag")]
        public BetaFailedFlag BetaFailedFlag { get; set; }
    }
    /// <summary>
    /// 当前核素信息
    /// </summary>
    public class NuclideUsed
    {
        public NuclideUsed()
        { }
        public NuclideUsed(string alphaNuclide="--",string betaNuclide="--",string clothNuclide="--")
        {
            AlphaNuclide = alphaNuclide;
            BetaNuclide = betaNuclide;
            ClothNuclide = clothNuclide;
        }
        /// <summary>
        /// Alpha核素
        /// </summary>
        public string AlphaNuclide { get; set; }
        /// <summary>
        /// Beta核素
        /// </summary>
        public string BetaNuclide { get; set; }
        /// <summary>
        /// 衣物核素
        /// </summary>
        public string ClothNuclide { get; set; }

    }
    /// <summary>
    /// 当前高压信息
    /// </summary>
    public class HighVoltage
    {
        public HighVoltage()
        { }
        public HighVoltage(string leftInsideHV="--",string leftOutsideHV="--",string rightInsideHV="--",string rightOutsideHV="--",string leftFootHV="--",string rightFootHV="--",string clothesHV="--")
        {
            LeftInsideHV = leftInsideHV;
            LeftOutsideHV = leftOutsideHV;
            RightInsideHV = rightInsideHV;
            RightOutsideHV = rightOutsideHV;
            LeftFootHV = leftFootHV;
            RightFootHV = rightFootHV;
            ClothesHV = clothesHV;
        }
        /// <summary>
        /// 左手心高压值
        /// </summary>
        public string LeftInsideHV { get; set; }
        /// <summary>
        /// 左手背高压值
        /// </summary>
        public string LeftOutsideHV { get; set; }
        /// <summary>
        /// 有手心高压值
        /// </summary>
        public string RightInsideHV { get; set; }
        /// <summary>
        /// 右手背高压值
        /// </summary>
        public string RightOutsideHV { get; set; }
        /// <summary>
        /// 左脚高压值
        /// </summary>
        public string LeftFootHV { get; set; }
        /// <summary>
        /// 右脚高压值
        /// </summary>
        public string RightFootHV { get; set; }
        /// <summary>
        /// 衣物高压值
        /// </summary>
        public string ClothesHV { get; set; }
    }
    /// <summary>
    /// 当前Alpha本底信息
    /// </summary>
    public class AlphaBackground
    {
        public AlphaBackground()
        {}
        public AlphaBackground(string alphaLeftInside="--",string alphaLeftOutside="--",string alphaRightInside="--",string alphaRightOutside="--",string alphaLeftFoot="--",string alphaRightFoot="--",string alphaClothes="--")
        {
            AlphaLeftInside = alphaLeftInside;
            AlphaLeftOutside = alphaLeftOutside;
            AlphaRightInside = alphaRightInside;
            AlphaRightOutside = alphaRightOutside;
            AlphaLeftFoot = alphaLeftFoot;
            AlphaRightFoot = alphaRightFoot;
            AlphaClothes = alphaClothes;
        }
        /// <summary>
        /// 左手心Alpha本底值
        /// </summary>
        public string AlphaLeftInside { get; set; }
        /// <summary>
        /// 左手背Alpha本底值
        /// </summary>
        public string AlphaLeftOutside { get; set; }
        /// <summary>
        /// 右手心Alpha本底值
        /// </summary>
        public string AlphaRightInside { get; set; }
        /// <summary>
        /// 右手背Alpha本底值
        /// </summary>
        public string AlphaRightOutside { get; set; }
        /// <summary>
        /// 左脚Alpha本底值
        /// </summary>
        public string AlphaLeftFoot { get; set; }
        /// <summary>
        /// 右脚Alpha本底值
        /// </summary>
        public string AlphaRightFoot { get; set; }
        /// <summary>
        /// 衣物Alpha本底值
        /// </summary>
        public string AlphaClothes { get; set; }
    }
    /// <summary>
    /// 当前Beta本底信息
    /// </summary>
    public class BetaBackground
    {
        public BetaBackground()
        { }
        public BetaBackground(string betaLeftInside="--",string betaLeftOutside="--",string betaRightInside="--",string betaRightOutside="--",string betaLeftFoot="--",string betaRightFoot="--",string betaClothes="--")
        {
            BetaLeftInside = betaLeftInside;
            BetaLeftOutside = betaLeftInside;
            BetaRightInside = betaRightInside;
            BetaRightOutside = betaRightOutside;
            BetaLeftFoot = betaLeftFoot;
            BetaRightFoot = betaRightFoot;
            BetaClothes = betaClothes;
        }
        /// <summary>
        /// 左手心Beta本底值
        /// </summary>
        public string BetaLeftInside { get; set; }
        /// <summary>
        /// 左手背Beta本底值
        /// </summary>
        public string BetaLeftOutside { get; set; }
        /// <summary>
        /// 右手心Beta本底值
        /// </summary>
        public string BetaRightInside { get; set; }
        /// <summary>
        /// 右手背Beta本底值
        /// </summary>
        public string BetaRightOutside { get; set; }
        /// <summary>
        /// 左脚Beta本底值
        /// </summary>
        public string BetaLeftFoot { get; set; }
        /// <summary>
        /// 右脚Beta本底值
        /// </summary>
        public string BetaRightFoot { get; set; }
        /// <summary>
        /// 衣物Beta本底值
        /// </summary>
        public string BetaClothes { get; set; }
    }

    /// <summary>
    /// 当前Alpha本底上限
    /// </summary>
    public class AlphaBackgroundUp
    {
        public AlphaBackgroundUp()
        { }
        public AlphaBackgroundUp(string alphaLeftInsideUp="--",string alphaLeftOutsideUp="--",string alphaRightInsideUp="--",string alphaRightOutsideUp="--",string alphaLeftFootUp="--",string alphaRightFootUp="--",string alphaClothesUp="--")
        {
            AlphaLeftInsideUp = alphaLeftInsideUp;
            AlphaLeftOutsideUp = alphaLeftOutsideUp;
            AlphaRightInsideUp = alphaRightInsideUp;
            AlphaRightOutsideUp = alphaRightOutsideUp;
            AlphaLeftFootUp = alphaLeftFootUp;
            AlphaRightFootUp = alphaRightFootUp;
            alphaClothesUp = AlphaClothesUp;
        }
        /// <summary>
        /// 左手心Alpha本底上限
        /// </summary>
        public string AlphaLeftInsideUp { get; set; }
        /// <summary>
        /// 左手背Alpha本底上限
        /// </summary>
        public string AlphaLeftOutsideUp { get; set; }
        /// <summary>
        /// 右手心Alpha本底上限
        /// </summary>
        public string AlphaRightInsideUp { get; set; }
        /// <summary>
        /// 右手背Alpha本底上限
        /// </summary>
        public string AlphaRightOutsideUp { get; set; }
        /// <summary>
        /// 左脚Alpha本底上限
        /// </summary>
        public string AlphaLeftFootUp { get; set; }
        /// <summary>
        /// 右脚Alpha本底上限
        /// </summary>
        public string AlphaRightFootUp { get; set; }
        /// <summary>
        /// 衣物Alpha本底上限
        /// </summary>
        public string AlphaClothesUp { get; set; }
    }

    /// <summary>
    /// 当前Beta本底上限
    /// </summary>
    public class BetaBackgroundUp
    {
        public BetaBackgroundUp()
        { }
        public BetaBackgroundUp(string betaLeftInsideUp="--",string betaLeftOutsideUp="--",string betaRightInsideUp="--",string betaRightOutsideUp="--",string betaLeftFootUp="--",string betaRightFootUp="--",string betaClothedUp="--")
        {
            BetaLeftInsideUp = betaLeftInsideUp;
            BetaLeftOutsideUp = betaLeftOutsideUp;
            BetaRightInsideUp = betaRightInsideUp;
            BetaRightOutsideUp = betaRightOutsideUp;
            BetaLeftFootUp = betaLeftFootUp;
            BetaRightFootUp = betaRightFootUp;
            BetaClothesUp = betaClothedUp;
        }
        /// <summary>
        /// 左手心Beta本底上限
        /// </summary>
        public string BetaLeftInsideUp { get; set; }
        /// <summary>
        /// 左手背Beta本底上限
        /// </summary>
        public string BetaLeftOutsideUp { get; set; }
        /// <summary>
        /// 右手心Beta本底上限
        /// </summary>
        public string BetaRightInsideUp { get; set; }
        /// <summary>
        /// 右手背Beta本底上限
        /// </summary>
        public string BetaRightOutsideUp { get; set; }
        /// <summary>
        /// 左脚Beta本底上限
        /// </summary>
        public string BetaLeftFootUp { get; set; }
        /// <summary>
        /// 右脚Beta本底上限
        /// </summary>
        public string BetaRightFootUp { get; set; }
        /// <summary>
        /// 衣物Beta本底上限
        /// </summary>
        public string BetaClothesUp { get; set; }
    }

    /// <summary>
    /// 当前Alpha本底下限
    /// </summary>
    public class AlphaBackgroundDown
    {
        public AlphaBackgroundDown()
        { }
        public AlphaBackgroundDown(string alphaLeftInsideDown="--",string alphaLeftOutsideDown="--",string alphaRightInsideDown="--",string alphaRightOutsideDown="--",string alphaLeftFootDown="--",string alphaRightFootDown="--",string alphaClothesDown="--")
        {
            AlphaLeftInsideDown = alphaLeftInsideDown;
            AlphaLeftOutsideDown = alphaLeftOutsideDown;
            AlphaRightInsideDown = alphaRightOutsideDown;
            AlphaRightOutsideDown = alphaRightOutsideDown;
            AlphaLeftFootDown = alphaLeftFootDown;
            AlphaRightFootDown = alphaRightFootDown;
            AlphaClothesDown = alphaClothesDown;
        }
        /// <summary>
        /// 左手心Alpha本底下限
        /// </summary>
        public string AlphaLeftInsideDown { get; set; }
        /// <summary>
        /// 左手背Alpha本底下限
        /// </summary>
        public string AlphaLeftOutsideDown { get; set; }
        /// <summary>
        /// 右手心Alpha本底下限
        /// </summary>
        public string AlphaRightInsideDown { get; set; }
        /// <summary>
        /// 右手背Alpha本底下限
        /// </summary>
        public string AlphaRightOutsideDown { get; set; }
        /// <summary>
        /// 左脚Alpha本底下限
        /// </summary>
        public string AlphaLeftFootDown { get; set; }
        /// <summary>
        /// 右脚Alpha本底下限
        /// </summary>
        public string AlphaRightFootDown { get; set; }
        /// <summary>
        /// 衣物Alpha本底下限
        /// </summary>
        public string AlphaClothesDown { get; set; }
    }
    /// <summary>
    /// 当前Beta本底下限
    /// </summary>
    public class BetaBackgroundDown
    {
        public BetaBackgroundDown()
        { }
        public BetaBackgroundDown(string betaLeftInsideDown="--",string betaLeftOutsideDown="--",string betaRightInsideDown="--",string betaRightOutsideDown="--",string betaLeftFootDown="--",string betaRightFootDown="--",string betaClothesDown="--")
        {
            BetaLeftInsideDown = betaLeftInsideDown;
            BetaLeftOutsideDown = betaLeftOutsideDown;
            BetaRightInsideDown = betaRightInsideDown;
            BetaRightOutsideDown = betaRightOutsideDown;
            BetaLeftFootDown = betaLeftFootDown;
            BetaRightFootDown = betaRightFootDown;
            BetaClothesDown = betaClothesDown;
        }
        /// <summary>
        /// 左手心Beta本底下限
        /// </summary>
        public string BetaLeftInsideDown { get; set; }
        /// <summary>
        /// 左手背Beta本底下限
        /// </summary>
        public string BetaLeftOutsideDown { get; set; }
        /// <summary>
        /// 右手心Beta本底下限
        /// </summary>
        public string BetaRightInsideDown { get; set; }
        /// <summary>
        /// 右手背Beta本底下限
        /// </summary>
        public string BetaRightOutsideDown { get; set; }
        /// <summary>
        /// 左脚Beta本底下限
        /// </summary>
        public string BetaLeftFootDown { get; set; }
        /// <summary>
        /// 右脚Beta本底下限
        /// </summary>
        public string BetaRightFootDown { get; set; }
        /// <summary>
        /// 衣物Beta本底下限
        /// </summary>
        public string BetaClothesDown { get; set; }
    }
    /// <summary>
    /// 当前Alpha测量数据
    /// </summary>
    public class AlphaTestData
    {
        /// <summary>
        /// 左手心Alpha测量值
        /// </summary>
        public string AlphaLeftInsideTest { get; set; } = "--";
        /// <summary>
        /// 左手背Alpha测量值
        /// </summary>
        public string AlphaLeftOutsideTest { get; set; } = "--";
        /// <summary>
        /// 右手心Alpha测量值
        /// </summary>
        public string AlphaRightInsideTest { get; set; } = "--";
        /// <summary>
        /// 右手背Alpha测量值
        /// </summary>
        public string AlphaRightOutsideTest { get; set; } = "--";
        /// <summary>
        /// 左脚Alpha测量值
        /// </summary>
        public string AlphaLeftFootTest { get; set; } = "--";
        /// <summary>
        /// 右脚Alpha测量值
        /// </summary>
        public string AlphaRightFootTest { get; set; } = "--";
        /// <summary>
        /// 衣物Alpha测量值
        /// </summary>
        public string AlphaClothesTest { get; set; } = "--";
    }
    /// <summary>
    /// 当前Beta测量数据
    /// </summary>
    public class BetaTestData
    {
        /// <summary>
        /// 左手心Beta测量值
        /// </summary>
        public string BetaLeftInsideTest { get; set; } = "--";
        /// <summary>
        /// 左手背Beta测量值
        /// </summary>
        public string BetaLeftOutsideTest { get; set; } = "--";
        /// <summary>
        /// 右手心Beta测量值
        /// </summary>
        public string BetaRightInsideTest { get; set; } = "--";
        /// <summary>
        /// 右手背Beta测量值
        /// </summary>
        public string BetaRightOutsideTest { get; set; } = "--";
        /// <summary>
        /// 左脚Beta测量值
        /// </summary>
        public string BetaLeftFootTest { get; set; } = "--";
        /// <summary>
        /// 右脚Beta测量值
        /// </summary>
        public string BetaRightFootTest { get; set; } = "--";
        /// <summary>
        /// 衣物Beta测量值
        /// </summary>
        public string BetaClothesTest { get; set; } = "--";
    }
    /// <summary>
    /// 当前Alpha一级报警阈值
    /// </summary>
    public class AlphaAlarmThread
    {
        /// <summary>
        /// 左手心Alpha一级报警阈值
        /// </summary>
        public string AlphaLeftInsideAlarm { get; set; }
        /// <summary>
        /// 左手背Alpha一级报警阈值
        /// </summary>
        public string AlphaLeftOutsideAlarm { get; set; }
        /// <summary>
        /// 右手心Alpha一级报警阈值
        /// </summary>
        public string AlphaRightInsideAlarm { get; set; }
        /// <summary>
        /// 右手背Alpha一级报警阈值
        /// </summary>
        public string AlphaRightOutsideAlarm { get; set; }
        /// <summary>
        /// 左脚Alpha一级报警阈值
        /// </summary>
        public string AlphaLeftFootAlarm { get; set; }
        /// <summary>
        /// 右脚Alpha一级报警阈值
        /// </summary>
        public string AlphaRightFootAlarm { get; set; }
        /// <summary>
        /// 衣物Alpha一级报警阈值
        /// </summary>
        public string AlphaClothesAlarm { get; set; }
    }

    /// <summary>
    /// 当前Beta一级报警阈值
    /// </summary>
    public class BetaAlarmThread
    {
        /// <summary>
        /// 左手心Beta一级报警阈值
        /// </summary>
        public string BetaLeftInsideAlarm { get; set; }
        /// <summary>
        /// 左手背Beta一级报警阈值
        /// </summary>
        public string BetaLeftOutsideAlarm { get; set; }
        /// <summary>
        /// 右手心Beta一级报警阈值
        /// </summary>
        public string BetaRightInsideAlarm { get; set; }
        /// <summary>
        /// 右手背Beta一级报警阈值
        /// </summary>
        public string BetaRightOutsideAlarm { get; set; }
        /// <summary>
        /// 左脚Beta一级报警阈值
        /// </summary>
        public string BetaLeftFootAlarm { get; set; }
        /// <summary>
        /// 右脚Beta一级报警阈值
        /// </summary>
        public string BetaRightFootAlarm { get; set; }
        /// <summary>
        /// 衣物Beta一级报警阈值
        /// </summary>
        public string BetaClothesAlarm { get; set; }
    }

    /// <summary>
    /// 当前Alpha二级报警阈值
    /// </summary>
    public class AlphaHighAlarmThread
    {
        /// <summary>
        /// 左手心Alpha二级报警阈值
        /// </summary>
        public string AlphaLeftInsideHAlarm { get; set; }
        /// <summary>
        /// 左手背Alpha二级报警阈值
        /// </summary>
        public string AlphaLeftOutsideHAlarm { get; set; }
        /// <summary>
        /// 右手心Alpha二级报警阈值
        /// </summary>
        public string AlphaRightInsideHAlarm { get; set; }
        /// <summary>
        /// 右手背Alpha二级报警阈值
        /// </summary>
        public string AlphaRightOutsideHAlarm { get; set; }
        /// <summary>
        /// 左脚Alpha二级报警阈值
        /// </summary>
        public string AlphaLeftFootHAlarm { get; set; }
        /// <summary>
        /// 右脚Alpha二级报警阈值
        /// </summary>
        public string AlphaRightFootHAlarm { get; set; }
        /// <summary>
        /// 衣物Alpha二级报警阈值
        /// </summary>
        public string AlphaClothesHAlarm { get; set; }
    }

    /// <summary>
    /// 当前Beta二级报警阈值
    /// </summary>
    public class BetaHighAlarmThread
    {
        /// <summary>
        /// 左手心Beta二级报警阈值
        /// </summary>
        public string BetaLeftInsideHAlarm { get; set; }
        /// <summary>
        /// 左手背Beta二级报警阈值
        /// </summary>
        public string BetaLeftOutsideHAlarm { get; set; }
        /// <summary>
        /// 右手心Beta二级报警阈值
        /// </summary>
        public string BetaRightInsideHAlarm { get; set; }
        /// <summary>
        /// 右手背Beta二级报警阈值
        /// </summary>
        public string BetaRightOutsideHAlarm { get; set; }
        /// <summary>
        /// 左脚Beta二级报警阈值
        /// </summary>
        public string BetaLeftFootHAlarm { get; set; }
        /// <summary>
        /// 右脚Beta二级报警阈值
        /// </summary>
        public string BetaRightFootHAlarm { get; set; }
        /// <summary>
        /// 衣物Beta二级报警阈值
        /// </summary>
        public string BetaClothesHAlarm { get; set; }        
    }
    /// <summary>
    /// 当前Alpha探测效率
    /// </summary>
    public class AlphaEfficiency
    {
        public string AlphaLeftInsideEff { get; set; }
        public string AlphaLeftOutsideEff { get; set; }
        public string AlphaRightInsideEff { get; set; }
        public string AlphaRightOutSideEff { get; set; }
        public string AlphaLeftFootEff { get; set; }
        public string AlphaRightFootEff { get; set; }
        public string AlphaClothesEff { get; set; }
    }
    /// <summary>
    /// 当前Beta探测效率
    /// </summary>
    public class BetaEfficiency
    {
        public string BetaLeftInsideEff { get; set; }
        public string BetaLeftOutsideEff { get; set; }
        public string BetaRightInsideEff { get; set; }
        public string BetaRightOutSideEff { get; set; }
        public string BetaLeftFootEff { get; set; }
        public string BetaRightFootEff { get; set; }
        public string BetaClothesEff { get; set; }
    }

    /// <summary>
    /// 当前Alpha阈值
    /// </summary>
    public class AlphaThread
    {
        /// <summary>
        /// 左手心Alpha阈值
        /// </summary>
        public string AlphaLeftInsideThread { get; set; }
        /// <summary>
        /// 左手背Alpha阈值
        /// </summary>
        public string AlphaLeftOutsideThread { get; set; }
        /// <summary>
        /// 右手心Alpha阈值
        /// </summary>
        public string AlphaRightInsideThread { get; set; }
        /// <summary>
        /// 右手背Alpha阈值
        /// </summary>
        public string AlphaRightOutsideThread { get; set; }
        /// <summary>
        /// 左脚Alpha阈值
        /// </summary>
        public string AlphaLeftFootThread { get; set; }
        /// <summary>
        /// 右脚Alpha阈值
        /// </summary>
        public string AlphaRightFootThread { get; set; }
        /// <summary>
        /// 衣物Alpha阈值
        /// </summary>
        public string AlphaClothesThread { get; set; }
    }

    /// <summary>
    /// 当前Beta阈值
    /// </summary>
    public class BetaThread
    {
        /// <summary>
        /// 左手心Beta阈值
        /// </summary>
        public string BetaLeftInsideThread { get; set; }
        /// <summary>
        /// 左手背Beta阈值
        /// </summary>
        public string BetaLeftOutsideThread { get; set; }
        /// <summary>
        /// 右手心Beta阈值
        /// </summary>
        public string BetaRightInsideThread { get; set; }
        /// <summary>
        /// 右手背Beta阈值
        /// </summary>
        public string BetaRightOutsideThread { get; set; }
        /// <summary>
        /// 左脚Beta阈值
        /// </summary>
        public string BetaLeftFootThread { get; set; }
        /// <summary>
        /// 右脚Beta阈值
        /// </summary>
        public string BetaRightFootThread { get; set; }
        /// <summary>
        /// 衣物Beta阈值
        /// </summary>
        public string BetaClothesThread { get; set; }
    }

    /// <summary>
    /// 当前Alpha报警标志
    /// </summary>
    public class AlphaAlarmFlag
    {
        /// <summary>
        /// 左手心Alpha报警标志
        /// </summary>
        public string AlphaLeftInsideAlarmFlag { get; set; }
        /// <summary>
        /// 左手背Alpha报警标志
        /// </summary>
        public string AlphaLeftOutsideAlarmFlag { get; set; }
        /// <summary>
        /// 右手心Alpha报警标志
        /// </summary>
        public string AlphaRightInsideAlarmFlag { get; set; }
        /// <summary>
        /// 右手背Alpha报警标志
        /// </summary>
        public string AlphaRightOutsideAlarmFlag { get; set; }
        /// <summary>
        /// 左脚Alpha报警标志
        /// </summary>
        public string AlphaLeftFootAlarmFlag { get; set; }
        /// <summary>
        /// 右脚Alpha报警标志
        /// </summary>
        public string AlphaRightFootAlarmFlag { get; set; }
        /// <summary>
        /// 衣物Alpha报警标志
        /// </summary>
        public string AlphaClothesAlarmFlag { get; set; }
    }

    /// <summary>
    /// 当前Beta报警标志
    /// </summary>
    public class BetaAlarmFlag
    {
        /// <summary>
        /// 左手心Beta报警标志
        /// </summary>
        public string BetaLeftInsideAlarmFlag { get; set; }
        /// <summary>
        /// 左手背Beta报警标志
        /// </summary>
        public string BetaLeftOutsideAlarmFlag { get; set; }
        /// <summary>
        /// 右手心Beta报警标志
        /// </summary>
        public string BetaRightInsideAlarmFlag { get; set; }
        /// <summary>
        /// 右手背Beta报警标志
        /// </summary>
        public string BetaRightOutsideAlarmFlag { get; set; }
        /// <summary>
        /// 左脚报警标志
        /// </summary>
        public string BetaLeftFootAlarmFlag { get; set; }
        /// <summary>
        /// 右脚Beta报警标志
        /// </summary>
        public string BetaRightFootAlarmFlag { get; set; }
        /// <summary>
        /// 衣物Beta报警标志
        /// </summary>
        public string BetaClothesAlarmFlag { get; set; }
    }

    /// <summary>
    /// 当前Alpha故障标志
    /// </summary>
    public class AlphaFailedFlag
    {
        /// <summary>
        /// 左手心Alpha故障标志
        /// </summary>
        public string AlphaLeftInsideFailedFlag { get; set; }
        /// <summary>
        /// 左手背Alpha故障标志
        /// </summary>
        public string AlphaLeftOutsideFailedFlag { get; set; }
        /// <summary>
        /// 右手心Alpha故障标志
        /// </summary>
        public string AlphaRightInsideFailedFlag { get; set; }
        /// <summary>
        /// 右手背Alpha故障标志
        /// </summary>
        public string AlphaRightOutsideFailedFlag { get; set; }
        /// <summary>
        /// 左脚Alpha故障标志
        /// </summary>
        public string AlphaLeftFootFailedFlag { get; set; }
        /// <summary>
        /// 右脚Alpha故障标志
        /// </summary>
        public string AlphaRightFootFailedFlag { get; set; }
        /// <summary>
        /// 衣物Alpha故障标志
        /// </summary>
        public string AlphaClothesFailedFlag { get; set; }
    }

    /// <summary>
    /// 当前Beta故障标志
    /// </summary>
    public class BetaFailedFlag
    {
        /// <summary>
        /// 左手心Beta故障标志
        /// </summary>
        public string BetaLeftInsideFailedFlag { get; set; }
        /// <summary>
        /// 左手背Beta故障标志
        /// </summary>
        public string BetaLeftOutsideFailedFlag { get; set; }
        /// <summary>
        /// 右手心Beta故障标志
        /// </summary>
        public string BetaRightInsideFailedFlag { get; set; }
        /// <summary>
        /// 右手背Beta故障标志
        /// </summary>
        public string BetaRightOutsideFailedFlag { get; set; }
        /// <summary>
        /// 左脚Beta故障标志
        /// </summary>
        public string BetaLeftFootFailedFlag { get; set; }
        /// <summary>
        /// 右脚Beta故障标志
        /// </summary>
        public string BetaRightFootFailedFlag { get; set; }
        /// <summary>
        /// 衣物Beta故障标志
        /// </summary>
        public string BetaClothesFailedFlag { get; set; }
    }
}

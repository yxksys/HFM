using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HFM.Properties;
using System.Drawing;

namespace HFM.Components
{
    public class MessageShow
    {
        protected static string _txtPreset;
        protected static string _txtActual;
        protected static string _txtPollutionRecerd;
        #region 监测状态文字提示
        public string TxtPreset { get=> _txtPreset; set=> _txtPreset =value; }//设置值
        public string TxtActual { get=> _txtActual; set=> _txtActual=value; }//测量值
        public string TxtPollutionRecerd { get=> _txtPollutionRecerd; set=> _txtPollutionRecerd=value; }//污染记录
        public string TxtErrorRecordOfChannel { get; set; }//仪器故障记录
        public virtual string TxtCommFault { get; set; }= "监测串口打开失败\r\n";
        public virtual string TxtCommSupervisoryFault { get; set; } = "管理机串口打开失败\r\n";
        public virtual string TxtCommError { get; set; } = "通讯错误\r\n";
        public virtual string TxtCommSupervisoryError { get; set; } = "管理机通讯错误\r\n";
        public virtual string TxtSelfCheck { get; set; } = "仪器自检\r\n";
        public virtual string TxtBackGroundMeasure { get; set; } = "本底测量\r\n";
        public virtual string TxtBackGroundInterrupted { get; set; } = "测量中断";
        public virtual string TxtLeftHandInPlace { get; set; } = "左手到位，重新测量\r\n";        
        public virtual string TxtRightHandInPlace { get; set; } = "右手到位，重新测量\r\n";
        public virtual string TxtLeftHandMoved { get; set; } = "左手移动，重新测量\r\n";
        public virtual string TxtRightHandMoved { get; set; } = "右手移动，重新测量\r\n";
        public virtual string TxtLeftFootMoved { get; set; } = "左脚移动，重新测量\r\n";
        public virtual string TxtRightFootMoved { get; set; } = "右脚移动，重新测量\r\n";
        public virtual string TxtFriskerInPlace { get; set; } = "衣物探头到位，重新测量\r\n";
        public virtual string TxtBackGroundError { get; set; } = "本底测量故障\r\n";
        public virtual string TxtReadyToMeasure { get; set; } = "仪器正常，等待测量\r\n";
        public virtual string TxtMeasuring { get; set; } = "开始测量\r\n";
        public virtual string TxtFriskerMeasure { get; set; } = "没有污染，请进行衣物测量\r\n";
        public virtual string TxtNoContamination { get; set; } = "没有污染，请离开\r\n";
        public virtual string TxtFriskerContamination { get; set; } = string.Format("衣物污染，设置值{0};测量值{1}", _txtPreset, _txtActual);
        public virtual string TxtDecontamination { get; set; } = string.Format("被测人员污染，请去污\r\n{0}\r\n", _txtPollutionRecerd);        
        public virtual string TxtTimeSynCompleted { get; set; } = "时间同步完成\r\n";
        public virtual string TxtTimeSynFailed { get; set; } = "时间同步失败\r\n";
        public virtual string TxtReportStatusFailed { get; set; } = "上报状态失败\r\n";
        #endregion
        #region 监测状态语音提示
        public Stream StreamDiDa_1 { get; set; } = Resources.dida1;
        public Stream StreamDiDa_2 { get; set; } = Resources.dida2;
        public Stream StreamDiDa_3 { get; set; } = Resources.dida3;
        public virtual Stream StreamSelfCheck { get; set; } = Resources.Chinese_Self_checking;
        public virtual Stream StreamBackGroundMeasure { get; set; } = Resources.Chinese_Updating_background;
        public virtual Stream StreamSelfCheckFault { get; set; } = Resources.Chinese_Self_checking_fault;
        public virtual Stream StreamLeftHandInPlace { get; set; } = Resources.Chinese_Left_hand_in_place_please_measure_again;
        public virtual Stream StreamRightHandInPlace { get; set; } = Resources.Chinese_right_hand_in_place_please_measure_again;
        public virtual Stream StreamFriskerInPlace { get; set; } = Resources.Chinese_frisker_In_place_measure_again;
        public virtual Stream StreamReadyToMeasure { get; set; } = Resources.Chinese_Ready;
        public virtual Stream StreamBackGroundError { get; set; } = Resources.Chinese_Background_abnomal;
        public virtual Stream StreamFlipPalm { get; set; } = Resources.Chinese_Please_Flip_Palm_for_Measuring;
        public virtual Stream StreamNoContamination { get; set; } = Resources.Chinese_NoContamination_please_leave;
        public virtual Stream StreamFriskerMeasure { get; set; } = Resources.Chinese_NoContamination_please_frisker;
        public virtual Stream StreamMeasuring { get; set; } = Resources.Chinese_Start_counting;
        public virtual Stream StreamLeftHandMoved { get; set; } = Resources.Chinese_Left_hand_moved_please_measure_again;
        public virtual Stream StreamRightHandMoved { get; set; } = Resources.Chinese_right_hand_moved_please_measure_again;
        public virtual Stream StreamLeftFootMoved { get; set; } = Resources.Chinese_Left_Foot_Moved_Please_Measure_Again;
        public virtual Stream StreamRightFootMoved { get; set; } = Resources.Chinese_Right_Foot_Moved_Please_Measure_Again;
        public virtual Stream StreamDecontamination { get; set; } = Resources.Chinese_Decontaminate_please;
        public virtual Stream StreamDecontamination_Man { get; set; } = Resources.Chinese_Decontaminate_please_man;
        #endregion
        #region 字体设置
        public virtual Font FontSet { get; set; } = new Font("微软雅黑", 14, FontStyle.Italic);
        #endregion

    }
}

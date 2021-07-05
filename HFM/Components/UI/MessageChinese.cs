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
    class MessageChinese:MessageShow
    {
        #region 监测状态文字提示
        public override string TxtCommFault { get; set; } = "监测串口打开失败\r\n";
        public override string TxtCommSupervisoryFault { get; set; } = "管理机串口打开失败\r\n";
        public override string TxtCommError { get; set; } = "通讯错误\r\n";
        public override string TxtCommSupervisoryError { get; set; } = "管理机通讯错误\r\n";
        public override string TxtSelfCheck { get; set; } = "仪器自检\r\n";
        public override string TxtBackGroundMeasure { get; set; } = "本底测量\r\n";
        public override string TxtBackGroundInterrupted { get; set; } = "测量中断";
        public override string TxtLeftHandInPlace { get; set; } = "左手到位，重新测量\r\n";
        public override string TxtRightHandInPlace { get; set; } = "右手到位，重新测量\r\n";
        public override string TxtLeftHandMoved { get; set; } = "左手移动，重新测量\r\n";
        public override  string TxtRightHandMoved { get; set; } = "右手移动，重新测量\r\n";
        public override string TxtLeftFootMoved { get; set; } = "左脚移动，重新测量\r\n";
        public override string TxtRightFootMoved { get; set; } = "右脚移动，重新测量\r\n";
        public override string TxtFriskerInPlace { get; set; } = "衣物探头到位，重新测量\r\n";
        public override string TxtBackGroundError { get; set; } = "本底测量故障\r\n";
        public override string TxtReadyToMeasure { get; set; } = "仪器正常，等待测量\r\n";
        public override string TxtMeasuring { get; set; } = "开始测量\r\n";
        public override string TxtFriskerMeasure { get; set; } = "没有污染，请进行衣物测量\r\n";
        public override string TxtNoContamination { get; set; } = "没有污染，请离开\r\n";
        public override string TxtFriskerContamination { get; set; } = string.Format("衣物污染，设置值{0};测量值{1}", _txtPreset, _txtActual);
        public override string TxtDecontamination { get; set; } = string.Format("被测人员污染，请去污\r\n{0}\r\n", _txtPollutionRecerd);        
        public override string TxtTimeSynCompleted { get; set; } = "时间同步完成\r\n";
        public override string TxtTimeSynFailed { get; set; } = "时间同步失败\r\n";
        public override string TxtReportStatusFailed { get; set; } = "上报状态失败\r\n";
        #endregion
        #region 监测状态语音提示
        public override Stream StreamSelfCheck { get; set; } = Resources.Chinese_Self_checking;
        public override Stream StreamBackGroundMeasure { get; set; } = Resources.Chinese_Updating_background;
        public override Stream StreamSelfCheckFault { get; set; } = Resources.Chinese_Self_checking_fault;
        public override Stream StreamLeftHandInPlace { get; set; } = Resources.Chinese_Left_hand_in_place_please_measure_again;
        public override Stream StreamRightHandInPlace { get; set; } = Resources.Chinese_right_hand_in_place_please_measure_again;
        public override Stream StreamFriskerInPlace { get; set; } = Resources.Chinese_frisker_In_place_measure_again;
        public override Stream StreamReadyToMeasure { get; set; } = Resources.Chinese_Ready;
        public override Stream StreamBackGroundError { get; set; } = Resources.Chinese_Background_abnomal;
        public override Stream StreamFlipPalm { get; set; } = Resources.Chinese_Please_Flip_Palm_for_Measuring;
        public override Stream StreamNoContamination { get; set; } = Resources.Chinese_NoContamination_please_leave;
        public override Stream StreamFriskerMeasure { get; set; } = Resources.Chinese_NoContamination_please_frisker;
        public override Stream StreamMeasuring { get; set; } = Resources.Chinese_Start_counting;
        public override Stream StreamLeftHandMoved { get; set; } = Resources.Chinese_Left_hand_moved_please_measure_again;
        public override Stream StreamRightHandMoved { get; set; } = Resources.Chinese_right_hand_moved_please_measure_again;
        public override Stream StreamLeftFootMoved { get; set; } = Resources.Chinese_Left_Foot_Moved_Please_Measure_Again;
        public override  Stream StreamRightFootMoved { get; set; } = Resources.Chinese_Right_Foot_Moved_Please_Measure_Again;
        public override Stream StreamDecontamination { get; set; } = Resources.Chinese_Decontaminate_please;
        public override Stream StreamDecontamination_Man { get; set; } = Resources.Chinese_Decontaminate_please_man;
        #endregion
        #region 字体设置
        public override Font FontSet { get; set; } = new Font("微软雅黑", 14, FontStyle.Italic);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using HFM.Properties;

namespace HFM.Components
{
    class MessageEnglish:MessageShow
    {
        #region 监测状态文字提示
        public override string TxtCommFault { get; set; } = "Measuring Comm Open Fault\r\n";
        public override string TxtCommSupervisoryFault { get; set; } = "Supervisor Comm Open Fault\r\n";
        public override string TxtCommError { get; set; } = "Communication Error\r\n";
        public override string TxtCommSupervisoryError { get; set; } = "Communication With Supervisor Error\r\n";
        public override string TxtSelfCheck { get; set; } = "Self-Checking\r\n";
        public override string TxtBackGroundMeasure { get; set; } = "Updating Background\r\n";
        public override string TxtBackGroundInterrupted { get; set; } = "Interrupetd";
        public override string TxtLeftHandInPlace { get; set; } = "Left Hand In Place，Please Measure Again\r\n";
        public override string TxtRightHandInPlace { get; set; } = "Right Hand In Place，Please Measure Again\r\n";
        public override  string TxtLeftHandMoved { get; set; } = "Left Hand Moving，Please Measure Again\r\n";
        public override  string TxtRightHandMoved { get; set; } = "Right Hand Moving，Please Measure Again\r\n";
        public override string TxtLeftFootMoved { get; set; } = "Left Foot Moving，Please Measure Again\r\n";
        public override string TxtRightFootMoved { get; set; } = "Right Foot Moving, Please Measure Again\r\n";
        public override string TxtFriskerInPlace { get; set; } = "Frisker In Place，Please Measure Again\r\n";
        public override string TxtBackGroundError { get; set; } = "Background Error\r\n";
        public override string TxtReadyToMeasure { get; set; } = "Ready\r\n";
        public override string TxtMeasuring { get; set; } = "Start Counting\r\n";
        public override string TxtFriskerMeasure { get; set; } = "No Contamination，Please Measure The Clothing\r\n";
        public override string TxtNoContamination { get; set; } = "No Contamination，Please Leave!\r\\n";
        public override string TxtFriskerContamination { get; set; } = string.Format("Clothing Contaminated，Preset{0};Actual{1}", _txtPreset, _txtActual);
        public override string TxtDecontamination { get; set; } = string.Format("Decontaminate Please!\r\n{0}\r\n", _txtPollutionRecerd);        
        public override string TxtTimeSynCompleted { get; set; } = "Time Synchronization Completed\r\n";
        public override string TxtTimeSynFailed { get; set; } = "Time Synchronization Failed\r\n";
        public override string TxtReportStatusFailed { get; set; } = "Report Status Failed\r\n";
        #endregion
        #region 监测状态语音提示
        public override Stream StreamSelfCheck { get; set; } = Resources.English_Self_checking;
        public override Stream StreamBackGroundMeasure { get; set; } = Resources.English_Updating_background;
        public override Stream StreamSelfCheckFault { get; set; } = Resources.English_Self_checking_fault;
        public override Stream StreamLeftHandInPlace { get; set; } = Resources.English_Left_hand_in_place_please_measure_again;
        public override Stream StreamRightHandInPlace { get; set; } = Resources.English_right_hand_in_place_please_measure_again;       
        public override Stream StreamFriskerInPlace { get; set; } = Resources.English_frisker_In_place;
        public override Stream StreamReadyToMeasure { get; set; } = Resources.English_Ready;
        public override Stream StreamBackGroundError { get; set; } = Resources.English_Background_abnomal;
        public override Stream StreamFlipPalm { get; set; } = Resources.English_please_flip_palm_for_measuring;
        public override Stream StreamNoContamination { get; set; } = Resources.English_NoContamination_please_leave;
        public override Stream StreamFriskerMeasure { get; set; } = Resources.English_NoContamination_please_frisker;
        public override Stream StreamMeasuring { get; set; } = Resources.English_Start_counting;
        public override Stream StreamLeftHandMoved { get; set; } = Resources.English_Left_hand_moved_please_measure_again;
        public override Stream StreamRightHandMoved { get; set; } = Resources.English_right_hand_moved_please_measure_again;
        public override Stream StreamLeftFootMoved { get; set; } = Resources.English_Left_Foot_Moved_Please_Measure_Again;
        public override Stream StreamRightFootMoved { get; set; } = Resources.English_Right_Foot_Moved_Please_Measure_Again;
        public override Stream StreamDecontamination { get; set; } = Resources.English_Decontaminate_please;
        public override Stream StreamDecontamination_Man { get; set; } = Resources.English_Decontaminate_please;
        #endregion
        #region 字体设置
        public override Font FontSet { get; set; } = new Font("微软雅黑", 12, FontStyle.Italic);
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;
using System.Threading;
using System.Globalization;
using System.IO;
using HFM.Properties;

namespace HFM
{
    public partial class FrmClothes : Form
    {

        bool isAudioDida1 = true;//测量滴答播放控制，每2秒播放一次
        public bool isEnglish = false;
        public FrmClothes()
        {
            InitializeComponent();
        }
        public FrmClothes(bool isEnglish)
        {
            this.isEnglish = isEnglish;                   
            InitializeComponent();
        }
        private void FrmClothes_Load(object sender, EventArgs e)
        {            
            loadingCircle.OuterCircleRadius = 20;
            loadingCircle.InnerCircleRadius = 14;
            loadingCircle.NumberSpoke = 20;
            loadingCircle.SpokeThickness = 3;
            loadingCircle.Active = true;
            if (isEnglish)
            {
                try
                {
                    isEnglish = true;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    Tools.ApplyLanguageResource(this);
                    Tools.controls.Clear();
                }
                catch
                {
                    return;
                }
            }
            else
            {
                try
                {
                    isEnglish = false;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                    Tools.ApplyLanguageResource(this);
                    Tools.controls.Clear();
                }
                catch
                {
                    return;
                }
            }
        }
        public void DisplayData(object sender, FriskerEventArgs e)
        {
            int prgBarValue = 0;
            if (e.IsFriskerIndependent)
            {
                TxtBackgroundValue.Text = string.Format("{0}cps", e.BackgroundValue["C"].ToString("F2"));
                TxtMeasureValue.Text = string.Format("{0}cps", e.MeasureValue["C"].ToString("F2"));
                prgBarValue = (int)(e.MeasureValue["C"] / e.Efficiency["C"]);
            }
            else
            {                
                switch (e.MeasureType)
                {
                    case "α":
                        TxtBackgroundValue.Text = string.Format("α:{0}cps", e.BackgroundValue["α"].ToString("F2"));
                        TxtMeasureValue.Text = string.Format("α:{0}cps", e.MeasureValue["α"].ToString("F2"));
                        prgBarValue = (int)(e.MeasureValue["α"] / e.Efficiency["α"]);
                        break;
                    case "β":
                        TxtBackgroundValue.Text = string.Format("β:{0}cps", e.BackgroundValue["β"].ToString("F2"));
                        TxtMeasureValue.Text = string.Format("β:{0}cps", e.MeasureValue["β"].ToString("F2"));
                        prgBarValue = (int)(e.MeasureValue["β"] / e.Efficiency["β"]);
                        break;
                    case "α/β":
                        TxtBackgroundValue.Text = string.Format("α:{0}cps;β:{1}cps", e.BackgroundValue["α"].ToString("F2"), e.BackgroundValue["β"].ToString("F2"));
                        TxtMeasureValue.Text = string.Format("α:{0}cps;β:{1}cps", e.MeasureValue["α"].ToString("F2"),e.MeasureValue["β"].ToString("F2"));
                        prgBarValue = (int)(e.MeasureValue["β"] / e.Efficiency["β"]);
                        break;
                }                
            }
            switch (e.DeviceStatus)
            {
                case DeviceStatus.OperatingNormally:
                    this.BackColor = Color.SpringGreen;
                    //设置监测进度百分比
                    PrgClothAlarm_1.Value = prgBarValue >= PrgClothAlarm_1.Maximum ? PrgClothAlarm_1.Maximum : prgBarValue;
                    PrgClothAlarm_2.Value = prgBarValue >= PrgClothAlarm_2.Maximum ? PrgClothAlarm_2.Maximum : prgBarValue;
                    //测量结果显示文本框背景色设置为SYSTEM
                    TxtMeasureValue.BackColor = PlatForm.ColorStatus.COLOR_SYSTEM;
                    if (isAudioDida1)
                    {
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer();//创建音频播放对象
                        //语音提示dida2.wav                    
                        player.Stream = Resources.dida2;
                        player.Load();
                        player.Play();
                    }
                    isAudioDida1 = !isAudioDida1;
                    break;
                case DeviceStatus.OperatingContaminated_1:
                    this.BackColor = PlatForm.ColorStatus.COLOR_ALARM_1;
                    PrgClothAlarm_1.Value = PrgClothAlarm_1.Maximum;
                    break;
                case DeviceStatus.OperatingContaminated_2:
                    this.BackColor = PlatForm.ColorStatus.COLOR_ALARM_2;
                    PrgClothAlarm_2.Value = PrgClothAlarm_2.Maximum;
                    break;
            }
        }
    }
}

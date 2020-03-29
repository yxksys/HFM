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

namespace HFM
{
    public partial class FrmHistory : Form
    {
        #region 字段
        /// <summary>
        /// 检测数组
        /// </summary>
        private string[] _measurArray=new string[4];
        /// <summary>
        /// 刻度数组
        /// </summary>
        private string[] _calibrationArray=new string[7];

        private string[] _errorDataArray=new string[3];
        #endregion

        #region 实例
        /// <summary>
        /// 系统参数
        /// </summary>
        private Components.SystemParameter _systemParameter=new Components.SystemParameter().GetParameter();
        /// <summary>
        /// 检测数据(英文)数据库查询所得
        /// </summary>
        private IList<MeasureData> _measureDataEnglish =new MeasureData().GetData(true);
        /// <summary>
        /// 检测数据(中文)数据库查询所得
        /// </summary>
        private IList<MeasureData> _measureDataChinese = new MeasureData().GetData();
        /// <summary>
        /// 刻度记录-数据库查询所得
        /// </summary>
        private IList<Calibration> _calibrations = new Calibration().GetData();
        /// <summary>
        /// 故障记录(英文)数据库查询所得
        /// </summary>
        private IList<ErrorData> _errorDatasEnglish=new ErrorData().GetData(true);
        /// <summary>
        /// 故障记录(中文)数据库查询所得
        /// </summary>
        private IList<ErrorData> _errorDatasChinese = new ErrorData().GetData(false);
        #endregion
        public FrmHistory()
        {
            InitializeComponent();
        }

        #region 页面切换
        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //根据页面索引更新当前页面值
            switch (TabHistory.SelectedIndex)
            {
                case 0:
                    DgvMeasure.Rows.Clear();        //清理Dgv数据表
                    if (_systemParameter.IsEnglish)
                    {
                        //遍历列表对象,取出数据按字段,添加到Dgv中
                        foreach (var measureData in _measureDataEnglish)
                        {
                            _measurArray[0] = measureData.MeasureDate.ToString();
                            _measurArray[1] = measureData.MeasureStatus;
                            _measurArray[2] = measureData.DetailedInfo;
                            _measurArray[3] = measureData.IsEnglish.ToString();
                            DgvMeasure.Rows.Add(_measurArray);
                        }
                    }
                    else
                    {
                        //遍历列表对象,取出数据按字段,添加到Dgv中
                        foreach (var measureData in _measureDataChinese)
                        {
                            _measurArray[0] = measureData.MeasureDate.ToString();
                            _measurArray[1] = measureData.MeasureStatus;
                            _measurArray[2] = measureData.DetailedInfo;
                            _measurArray[3] = measureData.IsEnglish.ToString();
                            DgvMeasure.Rows.Add(_measurArray);
                        }
                    }
                    break;
                case 1:
                    DgvCalibration.Rows.Clear();        //清理Dgv数据表
                    //遍历列表对象,取出数据按字段,添加到Dgv中
                    foreach (var calibration in _calibrations)
                    {
                        _calibrationArray[0] = calibration.CalibrationTime.ToString();
                        _calibrationArray[1] = calibration.Channel.ChannelID.ToString();
                        _calibrationArray[2] = calibration.HighVoltage.ToString();
                        _calibrationArray[3] = calibration.Threshold;
                        _calibrationArray[4] = calibration.Efficiency.ToString();
                        _calibrationArray[5] = calibration.MDA.ToString();
                        _calibrationArray[6] = calibration.AlphaBetaPercent.ToString();
                        DgvCalibration.Rows.Add(_calibrationArray);
                    }
                    break;
                case 2:
                    DgvError.Rows.Clear();
                    if (_systemParameter.IsEnglish)
                    {
                        foreach (var errorData in _errorDatasEnglish)
                        {
                            _errorDataArray[0] = errorData.ErrTime.ToString();
                            _errorDataArray[1] = errorData.Record;
                            _errorDataArray[2] = errorData.IsEnglish.ToString();
                            DgvError.Rows.Add(_errorDataArray);
                        }

                    }
                    else
                    {
                        foreach (var errorData in _errorDatasChinese)
                        {
                            _errorDataArray[0] = errorData.ErrTime.ToString();
                            _errorDataArray[1] = errorData.Record;
                            _errorDataArray[2] = errorData.IsEnglish.ToString();
                            DgvError.Rows.Add(_errorDataArray);
                        }
                    }
                    break;
                default:
                    MessageBox.Show("选择有误，请重新选择");
                    break;
            }

        } 
        #endregion
    }

    
}

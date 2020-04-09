using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private string[] _measurArray = new string[4];

        /// <summary>
        /// 刻度数组
        /// </summary>
        private string[] _calibrationArray = new string[7];

        private string[] _errorDataArray = new string[3];

        #endregion

        #region 实例

        /// <summary>
        /// 系统参数
        /// </summary>
        private Components.SystemParameter _systemParameter = new Components.SystemParameter().GetParameter();



        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmHistory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 启动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmHistory_Load(object sender, EventArgs e)
        {
            /// <summary>
            /// 检测数据(英文)数据库查询所得
            /// </summary>
            IList<MeasureData> _measureDataEnglish = new MeasureData().GetData(true);
            /// <summary>
            /// 检测数据(中文)数据库查询所得
            /// </summary>
            IList<MeasureData> _measureDataChinese = new MeasureData().GetData(false);


            DgvMeasure.Rows.Clear(); //清理Dgv数据表
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

            if (User.LandingUser.Role!=1)
            {
                BtnDeleteCalibration.Visible = false;
                BtnDeleteError.Visible = false;
            }
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
                    DgvMeasure.Rows.Clear(); //清理Dgv数据表
                    /// <summary>
                    /// 检测数据(英文)数据库查询所得
                    /// </summary>
                    IList<MeasureData> _measureDataEnglish = new MeasureData().GetData(true);
                    /// <summary>
                    /// 检测数据(中文)数据库查询所得
                    /// </summary>
                    IList<MeasureData> _measureDataChinese = new MeasureData().GetData(false);
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
                    DgvCalibration.Rows.Clear(); //清理Dgv数据表
                    /// <summary>
                    /// 刻度记录-数据库查询所得
                    /// </summary>
                    IList<Calibration> _calibrations = new Calibration().GetData();
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
                    /// <summary>
                    /// 故障记录(英文)数据库查询所得
                    /// </summary>
                    IList<ErrorData> _errorDatasEnglish = new ErrorData().GetData(true);
                    /// <summary>
                    /// 故障记录(中文)数据库查询所得
                    /// </summary>
                    IList<ErrorData> _errorDatasChinese = new ErrorData().GetData(false);
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

        /// <summary>
        /// 导出故障记录文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeriveError_Click(object sender, EventArgs e)
        {
            string localFilePath = "", fileNameExt = "", FilePath = "";
            //设置文件类型
            //书写规则例如：txt files(*.txt)|*.txt
            //设置默认文件名（可以不设置）
            //主设置默认文件extension（可以不设置）
            //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
            //设置默认文件类型显示顺序（可以不设置）
            //保存对话框是否记忆上次打开的目录
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files(*.txt)|*.txt|xls files(*.xls)|*.xls|All files(*.*)|*.*",
                FileName = DateTime.Now.ToString("yyyy MMMM dd") + "FaultLog",
                DefaultExt = "txt",
                AddExtension = true,
                FilterIndex = 2,
                RestoreDirectory = true
            };
            
            // Show save file dialog box
            DialogResult result = saveFileDialog.ShowDialog();
            //点了保存按钮进入
            if (result == DialogResult.OK)
            {
                //获得文件路径
                localFilePath = saveFileDialog.FileName.ToString();
                //获取文件名，不带路径
                fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                //获取文件路径，不带文件名
                FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\")); //在文件名里加字符
                //saveFileDialog.FileName.Insert(1,"dameng");
                //为用户使用 SaveFileDialog 选定的文件名创建读/写文件流。
                //System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();//输出文件
                //fs可以用于其他要写入的操作
                System.IO.FileStream fs = (System.IO.FileStream) saveFileDialog.OpenFile(); //输出文件
                //fs可以用于其他要写入的操作
                AddText(fs, "故障时间\t故障记录\t是否英文\n");
                string str = "";
                for (int i = 0; i < DgvError.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < DgvError.Columns.Count; j++)
                    {
                        str = DgvError.Rows[i].Cells[j].Value.ToString().Trim();
                        str = str + "\t";
                        AddText(fs, str);
                    }

                    AddText(fs, "\n");
                }

                fs.Close();
            }
        }

        /// <summary>
        /// 导出文件写入操作
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="value"></param>
        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        /// <summary>
        /// 导出刻度记录文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeriveCalibration_Click(object sender, EventArgs e)
        {
            string localFilePath = "", fileNameExt = "",FilePath = "";
            //设置文件类型
            //书写规则例如：txt files(*.txt)|*.txt
            //设置默认文件名（可以不设置）
            //主设置默认文件extension（可以不设置）
            //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
            //设置默认文件类型显示顺序（可以不设置）
            //保存对话框是否记忆上次打开的目录
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files(*.txt)|*.txt|xls files(*.xls)|*.xls|All files(*.*)|*.*",
                FileName = DateTime.Now.ToString("yyyy MMMM dd") + "CalibrationLog",
                DefaultExt = "txt",
                AddExtension = true,
                FilterIndex = 2,
                RestoreDirectory = true
            };
            

            // Show save file dialog box
            DialogResult result = saveFileDialog.ShowDialog();
            //点了保存按钮进入
            if (result == DialogResult.OK)
            {
                //获得文件路径
                localFilePath = saveFileDialog.FileName.ToString();
                //获取文件名，不带路径
                fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                //获取文件路径，不带文件名
                FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\")); //在文件名里加字符
                //saveFileDialog.FileName.Insert(1,"dameng");
                //为用户使用 SaveFileDialog 选定的文件名创建读/写文件流。
                //System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();//输出文件
                //fs可以用于其他要写入的操作
                System.IO.FileStream fs = (System.IO.FileStream) saveFileDialog.OpenFile(); //输出文件
                //fs可以用于其他要写入的操作
                AddText(fs, "刻度时间\t刻度通道\t高压值\t阈值\t效率\t探测下限\t串道比\n");
                string str = "";
                for (int i = 0; i < DgvCalibration.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < DgvCalibration.Columns.Count; j++)
                    {
                        str = DgvCalibration.Rows[i].Cells[j].Value.ToString().Trim();
                        str = str + "\t";
                        AddText(fs, str);
                    }

                    AddText(fs, "\n");
                }

                fs.Close();
            }
        }

        /// <summary>
        /// 故障日志,删除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteError_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除所有故障日记记录么?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int count = new ErrorData().DeleteData();
                MessageBox.Show($"您成功删除{count}条记录");
                DgvError.Rows.Clear();
                /// <summary>
                /// 故障记录(英文)数据库查询所得
                /// </summary>
                IList<ErrorData> _errorDatasEnglish = new ErrorData().GetData(true);
                /// <summary>
                /// 故障记录(中文)数据库查询所得
                /// </summary>
                IList<ErrorData> _errorDatasChinese = new ErrorData().GetData(false);
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
            }
        }

        /// <summary>
        /// 刻度日志,删除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteCalibration_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除所有故障日记记录么?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                int count = new Calibration().DeleteData();
                MessageBox.Show($"您成功删除{count}条记录");
                DgvCalibration.Rows.Clear(); //清理Dgv数据表
                /// <summary>
                /// 刻度记录-数据库查询所得
                /// </summary>
                IList<Calibration> _calibrations = new Calibration().GetData();
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
            }
        }
    }


}

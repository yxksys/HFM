/**
 * ________________________________________________________________________________ 
 *
 *  描述：通用工具类
 *  作者：* 、杨旭锴
 *  版本：
 *  创建时间：
 *  类名：工具类
 *  更新：2020年3月6日 新增方法，窗口错误提示消息,中英文提示框
 *        2020-03-07增加了16位CRC校验方法
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using HFM.Components;
using System.Collections.Generic;

namespace HFM.Components
{
	/// <summary>
	/// Tools 的摘要说明。
	/// </summary>
	public class Tools
	{
        static List<Control> controls = new List<Control>();
		#region 将DataReader 转为 DataTable
		/// <summary>
		/// 将DataReader 转为 DataTable
		/// </summary>
		/// <param name="DataReader">DataReader</param>
		public static DataTable ConvertDataReaderToDataTable(SqlDataReader dataReader)
		{
			DataTable datatable = new DataTable();
			DataTable schemaTable = dataReader.GetSchemaTable();
			//动态添加列
            try
            {
			
				foreach(DataRow myRow in schemaTable.Rows)
				{
					DataColumn myDataColumn = new DataColumn();
					//myDataColumn.DataType	= myRow.GetType();
                    myDataColumn.DataType = System.Type.GetType("System.String"); 
					myDataColumn.ColumnName = myRow[0].ToString();
					datatable.Columns.Add(myDataColumn);
				}
				//添加数据
				while(dataReader.Read())
				{
					DataRow myDataRow = datatable.NewRow();                   
					for(int i=0;i<schemaTable.Rows.Count;i++)
					{
                        myDataRow[i] =dataReader[i].ToString();
					}
					datatable.Rows.Add(myDataRow);
					myDataRow = null;
				}
				schemaTable = null;
				dataReader.Close();
				return datatable;
            }
            catch (Exception ex)
            {
                //Error.Log(ex.ToString());
                throw new Exception("转换出错出错!", ex);
            }
			
		}

		#endregion

		#region 将英文的星期几转为中文
		public static string ConvertDayOfWeekToZh(System.DayOfWeek dw)
		{
			string DayOfWeekZh="";
			switch (dw.ToString ("D"))
			{
				case "0":
					DayOfWeekZh="日";
					break;
				case "1":
					DayOfWeekZh="一";
					break;
				case "2":
					DayOfWeekZh="二";
					break;
				case "3":
					DayOfWeekZh="三";
					break;
				case "4":
					DayOfWeekZh="四";
					break;
				case "5":
					DayOfWeekZh="五";
					break;
				case "6":
					DayOfWeekZh="六";
					break;
			}
			
			return DayOfWeekZh;
		}
        #endregion

        #region 窗口错误提示消息,中英文提示框
        /// <summary>
        /// 窗口错误提示消息
        /// </summary>
        /// <param name="num">1:端口打开错误！请检查通讯是否正常。
        /// 2:通讯错误！请检查通讯是否正常。
        /// 3:通信故障,无法读取数据
        /// 4:登录失败，无法进行操作！
        /// 5:没有选择通道！
        /// 6:高压或者阈值数据没有输入
        /// 7:Alpha阈值范围-2000mV,α阈值输入有误
        /// 8:Beta阈值范围-2000mV,β阈值输入有误
        /// 9:请选择相应通道。
        /// 10:请选择核素类型！
        /// 11:请输入发射率！
        /// 12：进行本底测量，确认远离放射源？
        /// 13：请放入放射源！
        /// 14：请输入数字！
        /// 15:高压输入范围-1000V,高压输入有误！
        /// </param>
        public void PrompMessage(int num)
        {
            //从数据库中查看当前中英文状态
            bool isEnglish=new Components.SystemParameter().GetParameter().IsEnglish;

            switch (num)
            {
                case 1:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Port open error! Please check whether the communication is normal！", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"端口打开错误！请检查通讯是否正常。！", @"提示");
                    }
                    break;
                case 2:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Communication error! Please check whether the communication is normal.", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"通讯错误！请检查通讯是否正常！", @"提示");
                    }
                    break;
                case 3:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"COM Fault!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"通信故障,无法读取数据!", @"提示");
                    }
                    break;
                case 4:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Login Failed！", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"登录失败，无法进行操作！", @"提示");
                    }
                    break;
                case 5:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No Channel Selected！", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"没有选择通道！", @"提示");
                    }
                    break;
                case 6:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No High Voltage or threshold Inputed!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"高压或者阈值数据没有输入。", @"提示");
                    }
                    break;
                case 7:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Alpha Threshold range 0-1000mV,Input Error!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"阈值范围-2000mV,α阈值输入有误", @"提示");
                    }
                    break;
                case 8:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Beta Threshold range 0-1000mV,Input Error!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"阈值范围-2000mV,β阈值输入有误", @"提示");
                    }
                    break;
                case 9:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please select corresponding channel!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"请选择相应通道。", @"提示");
                    }
                    break;
                case 10:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please select nuclide!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"请选择核素类型！", @"提示");
                    }
                    break;
                case 11:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please enter the emissivity!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"请输入发射率！", @"提示");
                    }
                    break;
                case 12:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Background measuring, confirm away from source?", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"进行本底测量，确认远离放射源？", @"提示");
                    }
                    break;
                case 13:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please insert the source!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"请放入放射源！", @"提示");
                    }
                    break;
                case 14:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please enter a number！", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"请输入数字！", @"提示");
                    }
                    break;
                case 15:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"HV range 0-1000V,Input Error!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"高压输入范围-1000V,高压输入有误！", @"提示");
                    }
                    break;
                default:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Error", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"错误，请检查！", @"提示");
                    }
                    break;
            }
        }
        #endregion

        #region 生成16位CRC校验码
        /// <summary>
        /// 对输入参数data的前length个元素求CRC16检验值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns>输入参数data前length个元素的CRC16校验值</returns>
        public static byte[] CRC16(byte[] data, int length)
        {
            int len = data.Length;
            if (len > 0 && len>=length)
            {
                ushort crc = 0xFFFF;
                for (int i = 0; i < length; i++)
                {
                    crc = (ushort)(crc ^ (data[i]));
                    for (int j = 0; j < 8; j++)
                    {
                        crc = (crc & 1) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                    }
                }
                byte hi = (byte)((crc & 0xFF00) >> 8);  //高位置
                byte lo = (byte)(crc & 0x00FF);         //低位置

                return new byte[] { hi, lo };
            }
            return new byte[] { 0, 0 };
        }
        #endregion

        #region 加密方法

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }

        #endregion

        #region 错误日志记录
        /// <summary>
        /// 错误日志文件创建
        /// </summary>
        /// <param name="error"></param>
        public static void ErrorLog(string error)
        {
            //错误日志文件创建位置
            string path = $@"ErrorLog\{DateTime.Now.ToString("yyyyMMddTHHmmss")}.txt";
            if (!File.Exists(path))
            {
                // 创建要写入的文件。
                FileInfo fi1 = new FileInfo(path);
                try
                {
                    using (StreamWriter sw = fi1.CreateText())
                    {
                        sw.WriteLine(error);

                        sw.Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("程序中找不到错误日志文件目录,请手动在程序目录中创建日志目录'ErrorLog'");
                    throw;
                }
            }
        }

        #endregion

        #region 中英文界面转换时加载语言资源
        public static void ApplyLanguageResource(Form form)
        {
            System.ComponentModel.ComponentResourceManager res = new System.ComponentModel.ComponentResourceManager(form.GetType());
            GetControls(form);
            foreach (Control ctl in controls)
            {
                res.ApplyResources(ctl, ctl.Name);
            }
            form.ResumeLayout(false);
            form.PerformLayout();
            res.ApplyResources(form, "$form");
        }
        #endregion

        #region 主窗体底部状态栏

        /// <summary>
        /// 主窗体底部状态栏--串口是否打开,通信是否正常  false:通信故障   true:通信正常
        /// </summary>
        public static bool FormBottomPortStatus = false;

        #endregion

        #region 测量单位换算
        /// <summary>
        /// 测量数据单位换算，将cps单位数据data换算为目标单位后返回
        /// </summary>
        /// <param name="data">需换算的测量数据值(cps)</param>
        /// <param name="unit">要换算的目标单位</param>
        /// <param name="efficiency">探测效率</param>
        /// <param name="proberArea">探测面积</param>
        /// <returns>目标单位值</returns>
        public static float UnitConvertCPSTo(float data, string unit, float efficiency, float proberArea)
        {
            float convertedData = 0;
            //将data（单位为cps）换算为目标单位unit后返回
            switch (unit)
            {
                case "cps":
                    convertedData = data;
                    break;
                case "cpm": //最终测量计数平均值(cpm) = 60 * 计算平均值(cps)
                    convertedData = 60 * data;
                    break;
                case "Bq"://最终测量计数平均值(Bq) = 200 * 计算平均值(cps) /探测效率
                    convertedData = 200 * data / efficiency;
                    break;
                case "Bq/cm2"://最终测量计数平均值(Bq/cm2) = 200 * 计算平均值(cps) /探测效率/该通道测量面积
                    convertedData = 200 * data / efficiency / proberArea;
                    break;
                case "KBq/cm2"://KBq/cm2:最终测量计数平均值(KBq/cm2) = 200 * 计算平均值(cps) /探测效率/ 该通道测量面积/1000
                    convertedData = 200 * data / efficiency / proberArea / 1000;
                    break;
                case "dpm"://dpm:最终测量计数平均值(dpm) = 12000 * 计算平均值(cps)/探测效率
                    convertedData = 12000 * data / efficiency;
                    break;
                case "nCi"://nCi : 最终测量计数平均值(nCi) = 200 * 计算平均值(cps)/探测效率*0.027
                    convertedData = Convert.ToSingle(200 * data / efficiency * 0.027);
                    break;
            }
            return convertedData;
        }
        /// <summary>
        /// 测量数据单位换算，将其它单位(unit)数据data换算为cps单位后返回
        /// </summary>
        /// <param name="data">需转换的数据</param>
        /// <param name="unit">需转换的数据单位</param>
        /// <param name="efficiency">探测效率</param>
        /// <param name="proberArea">探测面积</param>
        /// <returns></returns>
        public static float UnitConvertToCPS(float data, string unit, float efficiency, float proberArea)
        {
            float convertedData = 0;
            //将data（单位为unit）换算为目标单位cps后返回
            switch (unit)
            {
                case "cps":
                    convertedData = data;
                    break;
                case "cpm": //最终测量计数平均值(cpm) = 60 * 计算平均值(cps)
                    convertedData = data / 60;
                    break;
                case "Bq"://最终测量计数平均值(Bq) = 200 * 计算平均值(cps) /探测效率
                    convertedData = data / efficiency * 200;
                    break;
                case "Bq/cm2"://最终测量计数平均值(Bq/cm2) = 200 * 计算平均值(cps) /探测效率/该通道测量面积
                    //convertedData = 200 * data / efficiency / proberArea;
                    convertedData = proberArea* efficiency* data/ 200;
                    break;
                case "KBq/cm2"://KBq/cm2:最终测量计数平均值(KBq/cm2) = 200 * 计算平均值(cps) /探测效率/ 该通道测量面积/1000
                    convertedData = 1000 * proberArea * efficiency * data / 200;
                    break;
                case "dpm"://dpm:最终测量计数平均值(dpm) = 12000 * 计算平均值(cps)/探测效率
                    convertedData = efficiency * data / 12000;
                    break;
                case "nCi"://nCi : 最终测量计数平均值(nCi) = 200 * 计算平均值(cps)/探测效率*0.027
                    convertedData = Convert.ToSingle(data * efficiency / 0.027 / 200);
                    break;
            }

            return convertedData;
        }
        #endregion

        #region 获得窗体中所有控件
        private static List<Control> GetControls(Control fatherControl)
        {
            Control.ControlCollection sonControls = fatherControl.Controls;
            //遍历所有控件
            foreach(Control control in sonControls)
            {
                controls.Add(control);
                if(control.Controls!=null)
                {
                    GetControls(control);
                }
            }
            return controls;
        }
        #endregion
    }
}

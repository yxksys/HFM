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
using System.Windows.Forms;
using HFM.Components;
namespace HFM.Components
{
	/// <summary>
	/// Tools 的摘要说明。
	/// </summary>
	public class Tools
	{
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
            if (len > 0)
            {
                ushort crc = 0xFFFF;
                for (int i = 0; i < len; i++)
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


    }
}

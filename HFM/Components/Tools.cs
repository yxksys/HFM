/**
 * ________________________________________________________________________________ 
 *
 *  描述：通用工具类
 *  作者：杨慧炯
 *  版本：V1.0
 *  创建时间：
 *  类名：Tools
 *  更新记录：2020-03-07增加了16位CRC校验方法
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
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

        #region CRC16校验
        /// <summary>
        /// 对输入参数data的前length个元素求CRC16检验值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns>输入参数data前length个元素的CRC16校验值</returns>
        public static byte[] CRC16(byte[] data,int length)
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

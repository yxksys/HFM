using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using HFM.Components;
namespace HFM.Components
{
	/// <summary>
	/// Tools ��ժҪ˵����
	/// </summary>
	public class Tools
	{
		#region ��DataReader תΪ DataTable
		/// <summary>
		/// ��DataReader תΪ DataTable
		/// </summary>
		/// <param name="DataReader">DataReader</param>
		public static DataTable ConvertDataReaderToDataTable(SqlDataReader dataReader)
		{
			DataTable datatable = new DataTable();
			DataTable schemaTable = dataReader.GetSchemaTable();
			//��̬�����
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
				//�������
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
                throw new Exception("ת���������!", ex);
            }
			
		}

		#endregion

		#region ��Ӣ�ĵ����ڼ�תΪ����
		public static string ConvertDayOfWeekToZh(System.DayOfWeek dw)
		{
			string DayOfWeekZh="";
			switch (dw.ToString ("D"))
			{
				case "0":
					DayOfWeekZh="��";
					break;
				case "1":
					DayOfWeekZh="һ";
					break;
				case "2":
					DayOfWeekZh="��";
					break;
				case "3":
					DayOfWeekZh="��";
					break;
				case "4":
					DayOfWeekZh="��";
					break;
				case "5":
					DayOfWeekZh="��";
					break;
				case "6":
					DayOfWeekZh="��";
					break;
			}
			
			return DayOfWeekZh;
		}
		#endregion
	}
}

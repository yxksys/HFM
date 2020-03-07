/**
 * ________________________________________________________________________________ 
 *
 *  ������ͨ�ù�����
 *  ���ߣ�* ��������
 *  �汾��
 *  ����ʱ�䣺
 *  ������������
 *  ���£�2020��3��6�� �������������ڴ�����ʾ��Ϣ,��Ӣ����ʾ��
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

        #region ���ڴ�����ʾ��Ϣ,��Ӣ����ʾ��
        /// <summary>
        /// ���ڴ�����ʾ��Ϣ
        /// </summary>
        /// <param name="num">1:�˿ڴ򿪴�������ͨѶ�Ƿ�������
        /// 2:ͨѶ��������ͨѶ�Ƿ�������
        /// 3:ͨ�Ź���,�޷���ȡ����
        /// 4:��¼ʧ�ܣ��޷����в�����
        /// 5:û��ѡ��ͨ����
        /// 6:��ѹ������ֵ����û������
        /// 7:Alpha��ֵ��Χ-2000mV,����ֵ��������
        /// 8:Beta��ֵ��Χ-2000mV,����ֵ��������
        /// 9:��ѡ����Ӧͨ����
        /// 10:��ѡ��������ͣ�
        /// 11:�����뷢���ʣ�
        /// 12�����б��ײ�����ȷ��Զ�����Դ��
        /// 13����������Դ��
        /// 14�����������֣�
        /// 15:��ѹ���뷶Χ-1000V,��ѹ��������
        /// </param>
        public void PrompMessage(int num)
        {
            //�����ݿ��в鿴��ǰ��Ӣ��״̬
            bool isEnglish=new Components.SystemParameter().GetParameter().IsEnglish;

            switch (num)
            {
                case 1:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Port open error! Please check whether the communication is normal��", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"�˿ڴ򿪴�������ͨѶ�Ƿ���������", @"��ʾ");
                    }
                    break;
                case 2:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Communication error! Please check whether the communication is normal.", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"ͨѶ��������ͨѶ�Ƿ�������", @"��ʾ");
                    }
                    break;
                case 3:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"COM Fault!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"ͨ�Ź���,�޷���ȡ����!", @"��ʾ");
                    }
                    break;
                case 4:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Login Failed��", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��¼ʧ�ܣ��޷����в�����", @"��ʾ");
                    }
                    break;
                case 5:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No Channel Selected��", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"û��ѡ��ͨ����", @"��ʾ");
                    }
                    break;
                case 6:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No High Voltage or threshold Inputed!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��ѹ������ֵ����û�����롣", @"��ʾ");
                    }
                    break;
                case 7:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Alpha Threshold range 0-1000mV,Input Error!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��ֵ��Χ-2000mV,����ֵ��������", @"��ʾ");
                    }
                    break;
                case 8:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Beta Threshold range 0-1000mV,Input Error!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��ֵ��Χ-2000mV,����ֵ��������", @"��ʾ");
                    }
                    break;
                case 9:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please select corresponding channel!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��ѡ����Ӧͨ����", @"��ʾ");
                    }
                    break;
                case 10:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please select nuclide!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��ѡ��������ͣ�", @"��ʾ");
                    }
                    break;
                case 11:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please enter the emissivity!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"�����뷢���ʣ�", @"��ʾ");
                    }
                    break;
                case 12:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Background measuring, confirm away from source?", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"���б��ײ�����ȷ��Զ�����Դ��", @"��ʾ");
                    }
                    break;
                case 13:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please insert the source!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��������Դ��", @"��ʾ");
                    }
                    break;
                case 14:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please enter a number��", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"���������֣�", @"��ʾ");
                    }
                    break;
                case 15:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"HV range 0-1000V,Input Error!", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"��ѹ���뷶Χ-1000V,��ѹ��������", @"��ʾ");
                    }
                    break;
                default:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Error", @"Message");
                    }
                    else
                    {
                        MessageBox.Show(@"�������飡", @"��ʾ");
                    }
                    break;
            }
        }
        #endregion


    }
}

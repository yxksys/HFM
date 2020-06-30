/**
 * ________________________________________________________________________________ 
 *
 *  ������ͨ�ù�����
 *  ���ߣ�* ��������
 *  �汾��
 *  ����ʱ�䣺
 *  ������������
 *  ���£�2020��3��6�� �������������ڴ�����ʾ��Ϣ,��Ӣ����ʾ��
 *        2020-03-07������16λCRCУ�鷽��
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
	/// Tools ��ժҪ˵����
	/// </summary>
	public class Tools
	{
        public static List<Control> controls = new List<Control>();
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
        /// 16:δ������Ч���ݣ�����������
        /// 17���������ݴ���!�볢�����²�����
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
                        MessageBox.Show(@"Port open error! Please check whether the communication is normal��", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"�˿ڴ򿪴�������ͨѶ�Ƿ���������", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 2:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Communication error! Please check whether the communication is normal.", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"ͨѶ��������ͨѶ�Ƿ�������", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 3:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"COM Fault!", @"Message",MessageBoxButtons.OK, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"ͨ�Ź���,�޷���ȡ����!", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 4:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Login Failed��", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��¼ʧ�ܣ��޷����в�����", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 5:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No Channel Selected��", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"û��ѡ��ͨ����", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 6:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No High Voltage or threshold Inputed!", @"Message",MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��ѹ������ֵ����û�����롣", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 7:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Alpha Threshold range 0-1000mV,Input Error!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��ֵ��Χ-2000mV,����ֵ��������", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 8:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Beta Threshold range 0-1000mV,Input Error!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��ֵ��Χ-2000mV,����ֵ��������", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 9:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please select corresponding channel!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��ѡ����Ӧͨ����", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 10:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please select nuclide!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��ѡ��������ͣ�", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 11:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please enter the emissivity,And not equal to 0!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"�����뷢����,���Ҳ�����0��", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 12:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Background measuring, confirm away from source?", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"���б��ײ�����ȷ��Զ�����Դ��", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 13:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please insert the source!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��������Դ��", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 14:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Please enter a number��", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"���������֣�", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 15:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"HV range 0-1000V,Input Error!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"��ѹ���뷶Χ-1000V,��ѹ��������", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 16:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"No valid data has been entered, please re-enter!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"δ������Ч���ݣ����������룡", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                case 17:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Return data error! Please try again!", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"�������ݴ���!�볢�����²�����", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
                default:
                    if (isEnglish == true)
                    {
                        MessageBox.Show(@"Error", @"Message", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        MessageBox.Show(@"�������飡", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    break;
            }
        }
        #endregion

        #region ����16λCRCУ����
        /// <summary>
        /// ���������data��ǰlength��Ԫ����CRC16����ֵ
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns>�������dataǰlength��Ԫ�ص�CRC16У��ֵ</returns>
        public static byte[] CRC16(byte[] data, int length)
        {
            int len = data.Length;                 
            if (len > 0 && len >= length)
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
                byte hi = (byte)((crc & 0xFF00) >> 8);  //��λ��
                byte lo = (byte)(crc & 0x00FF);         //��λ��

                return new byte[] { hi, lo };
            }
            return new byte[] { 0, 0 };
        }
        #endregion

        #region ���ܷ���

        /// <summary>
        /// 32λMD5����
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //ʵ����һ��md5����
            // ���ܺ���һ���ֽ����͵����飬����Ҫע�����UTF8/Unicode�ȵ�ѡ��
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // ͨ��ʹ��ѭ�������ֽ����͵�����ת��Ϊ�ַ��������ַ����ǳ����ַ���ʽ������
            for (int i = 0; i < s.Length; i++)
            {
                // ���õ����ַ���ʹ��ʮ���������͸�ʽ����ʽ����ַ���Сд����ĸ�����ʹ�ô�д��X�����ʽ����ַ��Ǵ�д�ַ� 
                pwd = pwd + s[i].ToString("X");
            }
            return pwd;
        }

        #endregion

        #region ������־��¼
        /// <summary>
        /// ������־�ļ�����
        /// </summary>
        /// <param name="error"></param>
        public static void ErrorLog(string error)
        {
            //������־�ļ�����λ��
            string path = $@"ErrorLog\{DateTime.Now.ToString("yyyyMMddTHHmmss")}.txt";
            if (!File.Exists(path))
            {
                // ����Ҫд����ļ���
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
                    MessageBox.Show("�������Ҳ���������־�ļ�Ŀ¼,���ֶ��ڳ���Ŀ¼�д�����־Ŀ¼'ErrorLog'");
                    throw;
                }
            }
        }

        #endregion

        #region ��Ӣ�Ľ���ת��ʱ����������Դ
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

        #region ������ײ�״̬��

        /// <summary>
        /// ������ײ�״̬��--�����Ƿ��,ͨ���Ƿ�����  false:ͨ�Ź���   true:ͨ������
        /// </summary>
        public static bool FormBottomPortStatus = false;

        #endregion

        #region ������λ����
        /// <summary>
        /// �������ݵ�λ���㣬��cps��λ����data����ΪĿ�굥λ�󷵻�
        /// </summary>
        /// <param name="data">�軻��Ĳ�������ֵ(cps)</param>
        /// <param name="unit">Ҫ�����Ŀ�굥λ</param>
        /// <param name="efficiency">̽��Ч��</param>
        /// <param name="proberArea">̽�����</param>
        /// <returns>Ŀ�굥λֵ</returns>
        public static float UnitConvertCPSTo(float data, string unit, float efficiency, float proberArea)
        {
            float convertedData = 0;
            //��data����λΪcps������ΪĿ�굥λunit�󷵻�
            switch (unit)
            {
                case "cps":
                    convertedData = data;
                    break;
                case "cpm": //���ղ�������ƽ��ֵ(cpm) = 60 * ����ƽ��ֵ(cps)
                    convertedData = 60 * data;
                    break;
                case "Bq"://���ղ�������ƽ��ֵ(Bq) = 200 * ����ƽ��ֵ(cps) /̽��Ч��
                    convertedData = 200 * data / efficiency;
                    break;
                case "Bq/cm2"://���ղ�������ƽ��ֵ(Bq/cm2) = 200 * ����ƽ��ֵ(cps) /̽��Ч��/��ͨ���������
                    convertedData = 200 * data / efficiency / proberArea;
                    break;
                case "KBq/cm2"://KBq/cm2:���ղ�������ƽ��ֵ(KBq/cm2) = 200 * ����ƽ��ֵ(cps) /̽��Ч��/ ��ͨ���������/1000
                    convertedData = 200 * data / efficiency / proberArea / 1000;
                    break;
                case "dpm"://dpm:���ղ�������ƽ��ֵ(dpm) = 12000 * ����ƽ��ֵ(cps)/̽��Ч��
                    convertedData = 12000 * data / efficiency;
                    break;
                case "nCi"://nCi : ���ղ�������ƽ��ֵ(nCi) = 200 * ����ƽ��ֵ(cps)/̽��Ч��*0.027
                    convertedData = Convert.ToSingle(200 * data / efficiency * 0.027);
                    break;
            }
            return convertedData;
        }
        /// <summary>
        /// �������ݵ�λ���㣬��������λ(unit)����data����Ϊcps��λ�󷵻�
        /// </summary>
        /// <param name="data">��ת��������</param>
        /// <param name="unit">��ת�������ݵ�λ</param>
        /// <param name="efficiency">̽��Ч��</param>
        /// <param name="proberArea">̽�����</param>
        /// <returns></returns>
        public static float UnitConvertToCPS(float data, string unit, float efficiency, float proberArea)
        {
            float convertedData = 0;
            //��data����λΪunit������ΪĿ�굥λcps�󷵻�
            switch (unit)
            {
                case "cps":
                    convertedData = data;
                    break;
                case "cpm": //���ղ�������ƽ��ֵ(cpm) = 60 * ����ƽ��ֵ(cps)
                    convertedData = data / 60;
                    break;
                case "Bq"://���ղ�������ƽ��ֵ(Bq) = 200 * ����ƽ��ֵ(cps) /̽��Ч��
                    convertedData = data * efficiency / 200;
                    
                    //convertedData = 200 * data / efficiency;//cps תbq
                    break;
                case "Bq/cm2"://���ղ�������ƽ��ֵ(Bq/cm2) = 200 * ����ƽ��ֵ(cps) /̽��Ч��/��ͨ���������
                    //convertedData = 200 * data / efficiency / proberArea;
                    convertedData = proberArea* efficiency* data/ 200;
                    break;
                case "KBq/cm2"://KBq/cm2:���ղ�������ƽ��ֵ(KBq/cm2) = 200 * ����ƽ��ֵ(cps) /̽��Ч��/ ��ͨ���������/1000
                    convertedData = 1000 * proberArea * efficiency * data / 200;
                    break;
                case "dpm"://dpm:���ղ�������ƽ��ֵ(dpm) = 12000 * ����ƽ��ֵ(cps)/̽��Ч��
                    convertedData = efficiency * data / 12000;
                    break;
                case "nCi"://nCi : ���ղ�������ƽ��ֵ(nCi) = 200 * ����ƽ��ֵ(cps)/̽��Ч��*0.027
                    convertedData = Convert.ToSingle(data * efficiency / 0.027 / 200);
                    break;
            }

            return convertedData;
        }
        #endregion

        #region ��ô��������пؼ�
        private static List<Control> GetControls(Control fatherControl)
        {
            Control.ControlCollection sonControls = fatherControl.Controls;
            //�������пؼ�
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

        #region ��������֮����п���
        public static void Clone(object objSource,object objDetection)
        {
            Type typeSource = objSource.GetType();
            Type typeDetection = objDetection.GetType();
            if(typeSource.Equals(typeDetection))
            {
                foreach(System.Reflection.PropertyInfo p in typeSource.GetProperties())
                {
                    System.Reflection.PropertyInfo p1=typeDetection.GetProperty(p.Name.ToString());
                    p1.SetValue(objDetection, p.GetValue(objSource));
                }
            }
        }
        #endregion

        #region ����������Ӣ��ת��
        /// <summary>
        /// ����������Ӣ��ʹ��
        /// </summary>
        /// <param name="cnSoftname">��������</param>
        /// <returns>Ӣ������</returns>
        public static string EnSoftName(string cnSoftname)
        {
            switch (cnSoftname)
            {
                case "HFM100�ֽű�����Ⱦ�����":
                    return "HFM100 Hand-Foot Monitor";
                case "HFM100TS�ֽű�����Ⱦ�����":
                    return "HFM100TS Two-Step Hand-Foot Monitor";
                case "HM100�ֲ���Ⱦ�����":
                    return "HM100 Hand Monitor";
                case "HM100TS�ֲ���Ⱦ�����":
                    return "HFM100 Two-Step Hand Monitor";
                case "CRM100�ڹ�ʽ��Ⱦ�����":
                    return "CRM100 Radiation Monitor";
                case "FM100�Ų���Ⱦ�����":
                    return "FM100 Foot Monitor";
                case "RM-AP������":
                    return "RM-AP Alarm Panel";
                default:
                    return "";
            }
        }
        public static string CnSoftName(string enSoftname)
        {
            switch (enSoftname)
            {
                case "HFM100 Hand-Foot Monitor":
                    return "HFM100�ֽű�����Ⱦ�����";
                case "HFM100TS Two-Step Hand-Foot Monitor":
                    return "HFM100TS�ֽű�����Ⱦ�����";
                case "HM100 Hand Monitor":
                    return "HM100�ֲ���Ⱦ�����";
                case "HFM100 Two-Step Hand Monitor":
                    return "HM100TS�ֲ���Ⱦ�����";
                case "CRM100 Radiation Monitor":
                    return "HM100TS�ֲ���Ⱦ�����";
                case "FM100 Foot Monitor":
                    return "FM100�Ų���Ⱦ�����";
                case "RM-AP Alarm Panel":
                    return "RM-AP������";
                default:
                    return "";
            }
        }
        #endregion

    }
}

using System;
using System.Windows.Forms;
using HFM.Components;
namespace HFM.Components
{
	/// <summary>
	/// SendData ��ժҪ˵����
	/// ͨ����������λ�����ͱ�����Ϣ
	/// </summary>
	public class SendData
	{
		public SendData()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#region ���ͱ�����Ϣ
		/// <summary>
		///  ͨ����������λ�����ͱ�����Ϣ
		///  </summary>
		/// <param name="BuffMessage">�洢������Ϣ������</param>
		/// <param name="commport">���䱨�Ķ˿�</param>
		/// <returns>true  ���ͳɹ�
		///          false ����ʧ��  </returns>
		public bool SendMessage(byte[] BuffMessage,CommPort commport) 
		{
			//�����Ѵ�
						
			//��õ�ǰϵͳʱ��
			System.DateTime Start_Time=new System.DateTime();  
			Start_Time= System.DateTime.Now;
			while (true)
			{
				System.DateTime Now_Time=new System.DateTime();
				Now_Time= System.DateTime.Now;
				//����ʱ�����20������ʧ��
				TimeSpan Space_Time=Now_Time.Subtract(Start_Time);     
				if(Space_Time.Seconds>20)
					return false;
				else
				{
					try
					{
						commport.Write(BuffMessage);
						return true;
					}
					catch
					{
						MessageBox.Show("�����·�ʧ�ܣ�����ͨѶ�Ƿ�������");
						return false;
					}
				}
			}
		}
							
		#endregion
		
	}
}

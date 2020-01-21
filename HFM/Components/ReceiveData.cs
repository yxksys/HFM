using System;
using System.Windows.Forms;
namespace HFM.Components
{
	/// <summary>
	/// ReceiveData ��ժҪ˵����
	/// </summary>
    public class ReceiveData
    {
        public ReceiveData()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
        }
        #region ���ձ�����Ϣ
        /// <summary>
        ///  ͨ�����ڽ�����λ���ϴ��Ĳɼ����ݱ���		
        /// </summary>
        /// <param name="commport"> �Ѵ򿪵Ľ��ձ�����Ϣ�Ĵ���</param>
        /// <returns>PosID  ���ճɹ��������յ��Ĳɼ���Ϣ
        ///                 ����ʧ�ܣ�����""  </returns>
        public string ReceiveMessage(CommPort commport)
        {
            //�����Ѵ�

            int NumBytes;
            HexCon hexcon = new HexCon();

            NumBytes = 54;
            byte[] RecBuf = new byte[54];
            //��õ�ǰϵͳʱ��
            System.DateTime Start_Time = new System.DateTime();
            Start_Time = System.DateTime.Now;
            while (true)
            {
                System.DateTime Now_Time = new System.DateTime();
                Now_Time = System.DateTime.Now;
                //����ʱ�����20������ʧ��
                TimeSpan Space_Time = Now_Time.Subtract(Start_Time);
                if (Space_Time.Seconds > 20)
                    return "";
                else
                {
                    //���������ݵ�RecBuf
                    try
                    {
                        //������λ���ϴ��Ĳɼ����ݱ��ģ������byte��ת��Ϊstring����(ʮ������)������
                        RecBuf = commport.Read(NumBytes);
                        string RecMessage = hexcon.ByteToString(RecBuf);
                        return RecMessage;
                    }
                    catch
                    {
                        MessageBox.Show("�������ݹ����з����쳣�������������ɼ�ϵͳ��");
                        return "";
                    }

                }
            }
        }
        #endregion
    }
}
		
	

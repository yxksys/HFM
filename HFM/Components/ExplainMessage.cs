using System;
using System.Windows.Forms;

namespace HFM.Components
{
	/// <summary>
	/// ExplainMessage ��ժҪ˵����
	/// /// <summary>
	/// ��������
	/// �ɼ����ݱ���
	///
    /// ���Ľṹ����54�ֽڣ�����ַ ���� Ronv1H Ronv1L Ronv2H Ronv2L��Ronv12H Ronv12L ǰ6·���������־ ��6·���������־ ����  �� ���� У��ͣ�Ϊǰ53���ֽڵ��ۼӺͣ��� ����16���ơ�
    /// ��ַ��12����·Ϊһ�飺 
    ///                        A1~A12��000B��00H��
    ///                        A13~A24��0001B��01H��
    ///                        B1~B12��0010B��02H��
    ///                        B13~B24��0011B��03H��
    ///                        C1~C12��0100B��04H��
    ///                        C13~C24��0101B��05H��
    ///                        D1~D12��0110B��06H��
    ///                        D13~D24��0111B��07H�� 
    /// Ronv1H��Ronv1L�ֱ�Ϊ��һ����·�ĵ�����ֽ�λ�͵��ֽ�λֵ���û�·�ĵ���ֵ���㹫ʽΪ��Ronv1H*256+Ronv1L����ֵ����Ϊ������������ֵΪ��������Ϊ0��������·��ͬ��
    /// ������У�6����·Ϊһ�飬��ǰ6·���������־����6·���������־�������仯ʱ�����������־��0��1��仯������ʾ��Ӧ��ַ��ǰ6����·����6����·������������μ�1��
	///  ���һ���ֽ�ΪУ��λ�������յ�������ǰ53���ֽڵĺ�����յ������һ���ֽ����ʱ����ʾ���������������ʾ���ݴ����쳣��
	/// </summary>
	/// 
	/// </summary>
	public class ExplainMessage
	{
		public ExplainMessage()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

        #region ����
        //��·��ַ  
        private int LoopAddress;

        //ĳһ��·ֵRonv
        private float[] ronv=new float[12];

        //ĳһ��·��λֵRonvH
        private int RonvH;

        //ĳһ��·��λֵRonvL
        private int RonvL;

        //////Ronv2
        ////private int Ronv2;

        ////Ronv2H
        //private int Ronv2H;

        ////Ronv2L
        //private int Ronv2L;

        //////Ronv3
        ////private int Ronv3;

        ////Ronv3H
        //private int Ronv3H;

        ////Ronv3L
        //private int Ronv3L;

        //////Ronv4
        ////private int Ronv4;

        ////Ronv4H
        //private int Ronv4H;

        ////Ronv4L
        //private int Ronv4L;

        //////Ronv5
        ////private int Ronv5;

        ////Ronv5H
        //private int Ronv5H;

        ////Ronv5L
        //private int Ronv5L;

        //////Ronv6
        ////private int Ronv6;

        ////Ronv6H
        //private int Ronv6H;
        
        ////Ronv6L
        //private int Ronv6L;

        //////Ronv7
        ////private int Ronv7;

        ////Ronv7H
        //private int Ronv7H;
        
        ////Ronv7L
        //private int Ronv7L;

        //////Ronv8
        ////private int Ronv8;

        ////Ronv8H
        //private int Ronv8H;
        
        ////Ronv8L
        //private int Ronv8L;

        //////Ronv9
        ////private int Ronv9;

        ////Ronv9H
        //private int Ronv9H;

        ////Ronv9L
        //private int Ronv9L;

        //////Ronv10
        ////private int Ronv10;

        ////Ronv10H
        //private int Ronv10H;

        ////Ronv10L
        //private int Ronv10L;

        //////Ronv11
        ////private int Ronv11;

        ////Ronv11H
        //private int Ronv11H;
        
        ////Ronv11L
        //private int Ronv11L;

        //////Ronv12
        ////private int Ronv12;

        ////Ronv12H
        //private int Ronv12H;
        
        ////Ronv12L
        //private int Ronv12L;

        //ǰ6·���������־
        private int ForeFlag;

        //��6·���������־
        private int AftFlag;


        //У���Ƿ�ͨ��
        private bool ISChecked=false;


        public int Loop_Address
        {
            //set
            //{
            //    LoopAddress = value;
            //}
            get { return LoopAddress; }
        }

        public float[] Ronv
        {
            //set
            //{
            //    Ronv1 = value;
            //}
            get { return ronv; }
        }

        //public int Ronv_2
        //{
        //    //set
        //    //{
        //    //    Ronv2 = value;
        //    //}
        //    get { return Ronv2; }
        //}

        //public int Ronv_3
        //{
        //    //set
        //    //{
        //    //    Ronv3 = value;
        //    //}
        //    get { return Ronv3; }
        //}

        //public int Ronv_4
        //{
        //    //set
        //    //{
        //    //    Ronv4 = value;
        //    //}
        //    get { return Ronv4; }
        //}

        //public int Ronv_5
        //{
        //    //set
        //    //{
        //    //    Ronv5 = value;
        //    //}
        //    get { return Ronv5; }
        //}

        //public int Ronv_6
        //{
        ////    set
        ////    {
        ////        Ronv6 = value;
        ////    }
        //    get { return Ronv6; }
        //}

        //public int Ronv_7
        //{
        //    //set
        //    //{
        //    //    Ronv7 = value;
        //    //}
        //    get { return Ronv7; }
        //}

        //public int Ronv_8
        //{
        //    //set
        //    //{
        //    //    Ronv8 = value;
        //    //}
        //    get { return Ronv8; }
        //}

        //public int Ronv_9
        //{
        //    //set
        //    //{
        //    //    Ronv9 = value;
        //    //}
        //    get { return Ronv9; }
        //}

        //public int Ronv_10
        //{
        //    //set
        //    //{
        //    //    Ronv10 = value;
        //    //}
        //    get { return Ronv10; }
        //}

        //public int Ronv_11
        //{
        //    //set
        //    //{
        //    //    Ronv11 = value;
        //    //}
        //    get { return Ronv11; }
        //}

        //public int Ronv_12
        //{
        //    //set
        //    //{
        //    //    Ronv12 = value;
        //    //}
        //    get { return Ronv12; }
        //}

        public int Fore_Flag
        {
            //set
            //{
            //    ForeFlag = value;
            //}
            get { return ForeFlag; }
        }
        
        public int Aft_Flag
        {
            //set
            //{
            //    AftFlag = value;

            //}
            get { return AftFlag; }
        }

        public bool IS_Checked
        {
            //set
            //{
            //    ISChecked = value;

            //}
            get { return ISChecked; }
        }
      
        #endregion

		#region ������Ϣ����
		/// <summary>
		///  �յ�����λ���ϴ��Ĳɼ���Ϣ�������н���
		/// </summary>
		/// <param name="Str_RecMessage ">���յ��Ĳɼ�������Ϣ���Ĺ���54���ֽڣ�</param>
		/// 
		/// <returns>void</returns>
		public void DAcquireData(string Str_RecMessage) 
		{
            int[] RecMessage=new int[54];
            int CheckSum=0;
            if (Str_RecMessage.Length == 108)
            {
                for (int i = 0; i < 54; i++)
                {
                    RecMessage[i] = (Str_RecMessage.Substring(i * 2, 2) == "" ? 0 : Convert.ToInt16(Str_RecMessage.Substring(i * 2, 2), 16));
                    //����յ���Ϣ��У���
                    if (i < 53)
                    {
                        CheckSum += RecMessage[i];
                    }
                }
                LoopAddress = RecMessage[0];

                for (int i = 1; i <= 12; i++)
                {
                    //��i��·��λֵ
                    RonvH = RecMessage[2 * i];
                    //��i��·��λֵ
                    RonvL = RecMessage[2 * i + 1];
                    //��i��·ֵ
                    ronv[i - 1] = ((RonvH * 256 + RonvL) / 1000) < 0 ? 0 : (RonvH * 256 + RonvL) / 1000;
                }
                ForeFlag = RecMessage[26];
                AftFlag = RecMessage[27];

                //���һ����Ϊ���յ���У�����Ϣ���жϽ��յ���У�����ʵ�ʼ�����Ƿ�һ�£�һ�±�ʾ������������ISCheckedΪtrue��������ISCheckedΪfalse
                //if (CheckSum == RecMessage[53])
                //{
                //    ISChecked = true;
                //}
                //else
                //{
                //    ISChecked = false;
                //}
                ISChecked = true;
            }
            else
            {
                ISChecked = false;
            }
		}
		#endregion
	}
}

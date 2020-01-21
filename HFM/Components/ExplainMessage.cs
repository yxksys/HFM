using System;
using System.Windows.Forms;

namespace HFM.Components
{
	/// <summary>
	/// ExplainMessage 的摘要说明。
	/// /// <summary>
	/// 解析报文
	/// 采集数据报文
	///
    /// 报文结构（共54字节）：地址 保留 Ronv1H Ronv1L Ronv2H Ronv2L…Ronv12H Ronv12L 前6路试验次数标志 后6路试验次数标志 保留  … 保留 校验和（为前53个字节的累加和）。 编码16进制。
    /// 地址以12个回路为一组： 
    ///                        A1~A12：000B（00H）
    ///                        A13~A24：0001B（01H）
    ///                        B1~B12：0010B（02H）
    ///                        B13~B24：0011B（03H）
    ///                        C1~C12：0100B（04H）
    ///                        C13~C24：0101B（05H）
    ///                        D1~D12：0110B（06H）
    ///                        D13~D24：0111B（07H） 
    /// Ronv1H和Ronv1L分别为第一个回路的电阻高字节位和低字节位值，该回路的电阻值计算公式为：Ronv1H*256+Ronv1L。阻值不能为负数，若所求值为负数令其为0。其它回路相同。
    /// 检测箱中，6个回路为一组，当前6路试验次数标志（后6路试验次数标志）发生变化时（试验次数标志在0和1间变化），表示对应地址的前6个回路（后6个回路）的样件试验次加1。
	///  最后一个字节为校验位，当接收到的数据前53个字节的和与接收到的最后一个字节相等时，表示接收正常，否则表示数据传输异常。
	/// </summary>
	/// 
	/// </summary>
	public class ExplainMessage
	{
		public ExplainMessage()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

        #region 属性
        //回路地址  
        private int LoopAddress;

        //某一回路值Ronv
        private float[] ronv=new float[12];

        //某一回路高位值RonvH
        private int RonvH;

        //某一回路低位值RonvL
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

        //前6路试验次数标志
        private int ForeFlag;

        //后6路试验次数标志
        private int AftFlag;


        //校验是否通过
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

		#region 报文信息解析
		/// <summary>
		///  收到从下位机上传的采集信息后对其进行解析
		/// </summary>
		/// <param name="Str_RecMessage ">接收到的采集数据信息报文共（54个字节）</param>
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
                    //求接收到信息的校验和
                    if (i < 53)
                    {
                        CheckSum += RecMessage[i];
                    }
                }
                LoopAddress = RecMessage[0];

                for (int i = 1; i <= 12; i++)
                {
                    //第i回路高位值
                    RonvH = RecMessage[2 * i];
                    //第i回路低位值
                    RonvL = RecMessage[2 * i + 1];
                    //第i回路值
                    ronv[i - 1] = ((RonvH * 256 + RonvL) / 1000) < 0 ? 0 : (RonvH * 256 + RonvL) / 1000;
                }
                ForeFlag = RecMessage[26];
                AftFlag = RecMessage[27];

                //最后一个数为接收到的校验和信息，判断接收到的校验和与实际计算的是否一致，一致表示接收正常，置ISChecked为true，否则置ISChecked为false
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

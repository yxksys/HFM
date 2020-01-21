using System;
using System.Windows.Forms;
namespace HFM.Components
{
	/// <summary>
	/// ReceiveData 的摘要说明。
	/// </summary>
    public class ReceiveData
    {
        public ReceiveData()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        #region 接收报文信息
        /// <summary>
        ///  通过串口接收下位机上传的采集数据报文		
        /// </summary>
        /// <param name="commport"> 已打开的接收报文信息的串口</param>
        /// <returns>PosID  接收成功：返回收到的采集信息
        ///                 接收失败：返回""  </returns>
        public string ReceiveMessage(CommPort commport)
        {
            //串口已打开

            int NumBytes;
            HexCon hexcon = new HexCon();

            NumBytes = 54;
            byte[] RecBuf = new byte[54];
            //获得当前系统时间
            System.DateTime Start_Time = new System.DateTime();
            Start_Time = System.DateTime.Now;
            while (true)
            {
                System.DateTime Now_Time = new System.DateTime();
                Now_Time = System.DateTime.Now;
                //传输时间大于20秒则传输失败
                TimeSpan Space_Time = Now_Time.Subtract(Start_Time);
                if (Space_Time.Seconds > 20)
                    return "";
                else
                {
                    //读串口数据到RecBuf
                    try
                    {
                        //接收下位机上传的采集数据报文，将其从byte型转换为string类型(十六进制)并返回
                        RecBuf = commport.Read(NumBytes);
                        string RecMessage = hexcon.ByteToString(RecBuf);
                        return RecMessage;
                    }
                    catch
                    {
                        MessageBox.Show("接收数据过程中发生异常，请重新启动采集系统！");
                        return "";
                    }

                }
            }
        }
        #endregion
    }
}
		
	

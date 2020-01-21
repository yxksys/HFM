using System;
using System.Windows.Forms;
using HFM.Components;
namespace HFM.Components
{
	/// <summary>
	/// SendData 的摘要说明。
	/// 通过串口向下位机传送报文信息
	/// </summary>
	public class SendData
	{
		public SendData()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#region 发送报文信息
		/// <summary>
		///  通过串口向下位机发送报文信息
		///  </summary>
		/// <param name="BuffMessage">存储报文信息缓冲区</param>
		/// <param name="commport">传输报文端口</param>
		/// <returns>true  发送成功
		///          false 发送失败  </returns>
		public bool SendMessage(byte[] BuffMessage,CommPort commport) 
		{
			//串口已打开
						
			//获得当前系统时间
			System.DateTime Start_Time=new System.DateTime();  
			Start_Time= System.DateTime.Now;
			while (true)
			{
				System.DateTime Now_Time=new System.DateTime();
				Now_Time= System.DateTime.Now;
				//传输时间大于20秒则传输失败
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
						MessageBox.Show("命令下发失败！请检查通讯是否正常！");
						return false;
					}
				}
			}
		}
							
		#endregion
		
	}
}

using System;
using HFM.Components;
namespace HFM.Components
{
	/// <summary>
	
	/// </summary>
	public class BuildMessage
	{
		public BuildMessage()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		
		#region 下发命令信息报文
		/// <summary>
        ///  生成下发命令信息报文
		/// </summary>
		/// 要求生成的报文信息格式为十六进制，报文：E2 地址 Kout。
		/// 地址为Kout控制的回路，以12个回路为一组： 
        ///                        A1~A12：000B（00H）
        ///                        A13~A24：0001B（01H）
        ///                        B1~B12：0010B（02H）
        ///                        B13~B24：0011B（03H）
        ///                        C1~C12：0100B（04H）
        ///                        C13~C24：0101B（05H）
        ///                        D1~D12：0110B（06H）
        ///                        D13~D24：0111B（07H）
        /// Kout为控制回路状态的命令：
        ///                        00B（00H）：A、B、C、D四个控制箱都运行
        ///                        01B（01H）：前6回路对应控制箱停止
        ///                        10B（02H）：后6回路对应控制箱停止
        ///                        11B（03H）：12回路对应控制箱都停止
        /// <returns>返回下发命令信息报文的十六进制数组；</returns>
		public byte[] BCtrlCommand(int CtrlAddress,int CtrlCommand)
		{
            string CtrlCommandHead = "E2";
            byte[] CtrlCommandMessage = new byte[3];
			HexCon hexcon=new HexCon();
            string TempCtrlAddressString = String.Format("{0:X}", CtrlAddress);
            string TempCtrlCommandString = String.Format("{0:X}", CtrlCommand);
            for (int i = TempCtrlAddressString.Length + 1; i <= 2; i++)
			{
                TempCtrlAddressString = "0" + TempCtrlAddressString;
			}
            for (int i = TempCtrlCommandString.Length + 1; i <= 2; i++)
            {
                TempCtrlCommandString = "0" + TempCtrlCommandString;
            }
            CtrlCommandMessage = hexcon.StringToByte(CtrlCommandHead + TempCtrlAddressString + TempCtrlCommandString);
            return CtrlCommandMessage;
		}
		#endregion		
		
	}
}

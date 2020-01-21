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
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		
		#region �·�������Ϣ����
		/// <summary>
        ///  �����·�������Ϣ����
		/// </summary>
		/// Ҫ�����ɵı�����Ϣ��ʽΪʮ�����ƣ����ģ�E2 ��ַ Kout��
		/// ��ַΪKout���ƵĻ�·����12����·Ϊһ�飺 
        ///                        A1~A12��000B��00H��
        ///                        A13~A24��0001B��01H��
        ///                        B1~B12��0010B��02H��
        ///                        B13~B24��0011B��03H��
        ///                        C1~C12��0100B��04H��
        ///                        C13~C24��0101B��05H��
        ///                        D1~D12��0110B��06H��
        ///                        D13~D24��0111B��07H��
        /// KoutΪ���ƻ�·״̬�����
        ///                        00B��00H����A��B��C��D�ĸ������䶼����
        ///                        01B��01H����ǰ6��·��Ӧ������ֹͣ
        ///                        10B��02H������6��·��Ӧ������ֹͣ
        ///                        11B��03H����12��·��Ӧ�����䶼ֹͣ
        /// <returns>�����·�������Ϣ���ĵ�ʮ���������飻</returns>
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

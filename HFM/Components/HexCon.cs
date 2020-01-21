using System;

namespace HFM.Components
{
	/// <summary>
	/// HexCon 的摘要说明。
	/// </summary>
	public class HexCon
	{
		public HexCon()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		public  string ByteToString(byte[] InBytes)  
		{ 
			string StringOut=""; 
			foreach (byte InByte in InBytes)  
			{ 
				StringOut=StringOut + String.Format("{0:X2}",InByte); 
			} 
			return StringOut;  
		} 

		public  byte[] StringToByte(string InString)  
		{ 
			string[] ByteStrings=new string[InString.Length/2]; 
				
			// char [] Byte; 
			for(int i=0;i<(InString.Length /2);i++)
			{
				ByteStrings[i]=InString.Substring (i*2,2);
			}
				
			byte[] ByteOut; 
			ByteOut = new byte[ByteStrings.Length]; 
			for (int i = 0;i<ByteStrings.Length;i++)  
			{    
				//  ByteStrings.to 
				ByteOut[i] = System.Convert .ToByte(ByteStrings[i],16); 

			}  
			return ByteOut; 
			
		}
	}
}

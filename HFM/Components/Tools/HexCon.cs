/**
 * ________________________________________________________________________________ 
 *
 *  ������
 *  ���ߣ�
 *  �汾��
 *  ����ʱ�䣺
 *  ������
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;

namespace HFM.Components
{
    /// <summary>
    /// HexCon ��ժҪ˵����
    /// </summary>
    public class HexCon
    {
        public HexCon()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
            //
        }
        public string ByteToString(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2}", InByte);
            }
            return StringOut;
        }

        public byte[] StringToByte(string InString)
        {
            string[] ByteStrings = new string[InString.Length / 2];

            // char [] Byte; 
            for (int i = 0; i < (InString.Length / 2); i++)
            {
                ByteStrings[i] = InString.Substring(i * 2, 2);
            }

            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length];
            for (int i = 0; i < ByteStrings.Length; i++)
            {
                //  ByteStrings.to 
                ByteOut[i] = System.Convert.ToByte(ByteStrings[i], 16);

            }
            return ByteOut;

        }
    }
}

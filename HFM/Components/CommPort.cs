/**
 * ________________________________________________________________________________ 
 *
 *  ��������װ�˶Դ��ڵĲ�����������ô������á��򿪶˿ڡ��رն˿ڡ�����д
 *  ���ߣ���۾�
 *  �汾��V1.0
 *  ����ʱ�䣺2020-02-11
 *  ������CommPort
 *  ���¼�¼��02-19�����˶�ȡ�������ļ���ȡ�������÷���
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Runtime.InteropServices ;
using System.Data;
using System.Data.SqlClient;
namespace HFM.Components
{
	/// <summary>
	/// CommPort ��ժҪ˵����
	/// </summary>
	public class CommPort
	{
		public CommPort()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
        ~CommPort()
        {
            
        }
		public int PortNum;  
		public int BaudRate; 
		public byte ByteSize; 
		public byte Parity; // 0-4=no,odd,even,mark,space  
		public byte StopBits; // 0,1,2 = 1, 1.5, 2  
		public int ReadTimeout;
        public bool IsEnabled=true;
        #region ���ò�����windowsϵͳ�ļ������api
        //comm port win32 file handle 
        public int hComm = -1; 
		public bool hCommRead = false; 
       
		public bool Opened = false; 
  
		//win32 api constants 
		private const uint GENERIC_READ = 0x80000000; 
		private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
		
		private const int OPEN_EXISTING = 3;       
		private const int INVALID_HANDLE_value = -1; 
       
		[StructLayout(LayoutKind.Sequential)] 
		public struct DCB  
		{ 
			//taken from c struct in platform sdk  
			public int DCBlength;           // sizeof(DCB)  
			public int BaudRate;            // current baud rate  
			//	public int fBinary;          // binary mode, no EOF check  
			public int fParity;          // enable parity checking  
			//			public int fOutxCtsFlow;      // CTS output flow control  
			//			public int fOutxDsrFlow;      // DSR output flow control  
			//			public int fDtrControl;       // DTR flow control type  
			//			public int fDsrSensitivity;   // DSR sensitivity  
			//			public int fTXContinueOnXoff; // XOFF continues Tx  
			//			public int fOutX;          // XON/XOFF out flow control  
			//			public int fInX;           // XON/XOFF in flow control  
			//			public int fErrorChar;     // enable error replacement  
			//			public int fNull;          // enable null stripping  
			//			public int fRtsControl;     // RTS flow control  
			//			public int fAbortonError;   // abort on error  
			//			public int fDummy2;        // reserved  
			public ushort wReserved;          // not currently used  
			public ushort XonLim;             // transmit XON threshold  
			public ushort XoffLim;            // transmit XOFF threshold  
			public byte ByteSize;           // number of bits/byte, 4-8  
			public byte Parity;             // 0-4=no,odd,even,mark,space  
			public byte StopBits;           // 0,1,2 = 1, 1.5, 2  
			public char XonChar;            // Tx and Rx XON character  
			public char XoffChar;           // Tx and Rx XOFF character  
			public char ErrorChar;          // error replacement character  
			public char EofChar;            // end of input character  
			public char EvtChar;            // received event character  
			public ushort wReserved1;         // reserved; do not use             
		} 

		[StructLayout(LayoutKind.Sequential)] 
		private struct COMMTIMEOUTS  
		{   
			public int ReadIntervalTimeout;  
			public int ReadTotalTimeoutMultiplier;  
			public int ReadTotalTimeoutConstant;  
			public int WriteTotalTimeoutMultiplier;  
			public int WriteTotalTimeoutConstant;  
		}     

		[StructLayout(LayoutKind.Sequential)]    
		private struct OVERLAPPED  
		{  
			public int  Internal;  
			public int  InternalHigh;  
			public int  Offset;  
			public int  OffsetHigh;  
			public int hEvent;  
		}   
       
		[DllImport("kernel32")] 
		private static extern int CreateFile( 
			string lpFileName,                         // file name 
			uint dwDesiredAccess,                      // access mode 
			uint dwShareMode,                          // share mode 
			int lpSecurityAttributes, // SD 
			int dwCreationDisposition,                // how to create 
			uint dwFlagsAndAttributes,                 // file attributes 
			uint hTemplateFile                        // handle to template file 
			); 
		[DllImport("kernel32")] 
		private static extern bool GetCommState( 
			int hFile,  // handle to communications device 
			ref DCB lpDCB    // device-control block 
			);    
		[DllImport("kernel32")] 
		private static extern bool BuildCommDCB( 
			string lpDef,  // device-control string 
			ref DCB lpDCB     // device-control block 
			); 
		[DllImport("kernel32")] 
		private static extern bool SetCommState( 
			int hFile,  // handle to communications device 
			ref DCB lpDCB    // device-control block 
			); 
		[DllImport("kernel32")] 
		private static extern bool GetCommTimeouts( 
			int hFile,                  // handle to comm device 
			ref COMMTIMEOUTS lpCommTimeouts  // time-out values 
			);    
		[DllImport("kernel32")]    
		private static extern bool SetCommTimeouts( 
			int hFile,                  // handle to comm device 
			ref COMMTIMEOUTS lpCommTimeouts  // time-out values 
			); 
		[DllImport( "kernel32")] 
		private static extern bool ReadFile( 
			int hFile,                // handle to file 
			byte[] lpBuffer,             // data buffer 
			int nNumberOfBytesToRead,  // number of bytes to read 
			ref int lpNumberOfBytesRead, // number of bytes read 
			ref OVERLAPPED lpOverlapped    // overlapped buffer 
			); 
		[DllImport("kernel32")] 
		private static extern bool WriteFile( 
			int hFile,                    // handle to file 
			byte[] lpBuffer,                // data buffer 
			int nNumberOfBytesToWrite,     // number of bytes to write 
			ref int lpNumberOfBytesWritten,  // number of bytes written 
			ref OVERLAPPED lpOverlapped        // overlapped buffer 
			); 
		[DllImport("kernel32")] 
		private static extern bool CloseHandle( 
			int hObject   // handle to object 
			);

        [DllImport("kernel32")]
        private static extern bool PurgeComm(int hFile, uint dwFlags);
        #endregion

        #region �򿪴���
        /// <summary>
        /// �򿪴��ڲ���
        /// </summary>
        public void Open()  
		{ 
			if(Opened)
				return;
			else
			{
        
				DCB dcbCommPort = new DCB(); 
				COMMTIMEOUTS ctoCommPort = new COMMTIMEOUTS();


                // OPEN THE COMM PORT. 
                if (PortNum < 10)
                {
                    hComm = CreateFile("COM" + PortNum, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
                }
                else
                {
                    hComm = CreateFile("\\\\.\\COM" + PortNum, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
                }
       
				// IF THE PORT CANNOT BE OPENED, BAIL OUT. 
				if(hComm == INVALID_HANDLE_value)  
				{ 
					throw(new ApplicationException("Comm Port Can Not Be Opened")); 
				} 
       
				// SET THE COMM TIMEOUTS. 
          
				GetCommTimeouts(hComm,ref ctoCommPort); 
				ctoCommPort.ReadTotalTimeoutConstant = ReadTimeout; 
				ctoCommPort.ReadTotalTimeoutMultiplier = 0; 
				ctoCommPort.WriteTotalTimeoutMultiplier = 0; 
				ctoCommPort.WriteTotalTimeoutConstant = 0;   
				SetCommTimeouts(hComm,ref ctoCommPort); 
       
				// SET BAUD RATE, PARITY, WORD SIZE, AND STOP BITS. 
				// THERE ARE OTHER WAYS OF DOING SETTING THESE BUT THIS IS THE EASIEST. 
				// IF YOU WANT TO LATER ADD CODE FOR OTHER BAUD RATES, REMEMBER 
				// THAT THE ARGUMENT FOR BuildCommDCB MUST BE A POINTER TO A STRING. 
				// ALSO NOTE THAT BuildCommDCB() DEFAULTS TO NO HANDSHAKING. 
       
				dcbCommPort.DCBlength = Marshal.SizeOf(dcbCommPort); 
				GetCommState(hComm, ref dcbCommPort); 
				dcbCommPort.BaudRate=BaudRate; 
				dcbCommPort.Parity=Parity; 
				dcbCommPort.ByteSize=ByteSize; 
				dcbCommPort.StopBits=StopBits; 
				if(!SetCommState(hComm, ref dcbCommPort))
				{
					throw(new ApplicationException("Comm Port Set Failed!")); 
				}
         
				Opened = true; 
			}
         
		}
        #endregion

        #region �رմ���
        /// <summary>
        /// �رմ��ڲ���
        /// </summary>
        public void Close()  
		{ 
			if(!Opened)
				return;
			else
			{
				if (hComm!=INVALID_HANDLE_value)  
				{ 
					if(CloseHandle(hComm)==true)
						Opened=false;
					else
						throw(new ApplicationException("Comm Port Can Not Be Closed")); 
				}
			}
		}
        #endregion

        #region ������
        /// <summary>
        /// �����ڲ���
        /// </summary>
        /// <param name="NumBytes">��ȡ�����ݳ���</param>
        /// <returns>�Ӵ��ڻ�õ��ֽ�����������</returns>
        public byte[] Read(int NumBytes)  
		{ 
			byte[] BufBytes; 
			byte[] OutBytes; 
			BufBytes = new byte[NumBytes]; 
			if (hComm!=INVALID_HANDLE_value)  
			{ 
				OVERLAPPED ovlCommPort = new OVERLAPPED(); 
				int BytesRead=0; 
				hCommRead=ReadFile(hComm,BufBytes,NumBytes,ref BytesRead,ref ovlCommPort); 
				OutBytes = new byte[BytesRead]; 
				Array.Copy(BufBytes,OutBytes,BytesRead); 
             
			}  
			else  
			{ 
				throw(new ApplicationException("Comm Port Not Open")); 
			} 
			return OutBytes; 
		}
        #endregion

        #region д����
        /// <summary>
        /// д���ڲ���
        /// </summary>
        /// <param name="WriteBytes">�򴮿ڷ��͵��ֽ�����������</param>
        public void Write(byte[] WriteBytes)  
		{ 
			if (hComm!=INVALID_HANDLE_value)  
			{ 
				OVERLAPPED ovlCommPort = new OVERLAPPED(); 
				int BytesWritten = 0; 
				WriteFile(hComm,WriteBytes,WriteBytes.Length,ref BytesWritten,ref ovlCommPort);
			} 
			else  
			{ 
				throw(new ApplicationException("Comm Port Not Open")); 
			}       
		}
        #endregion

        #region ��ȡ�����ļ��е�ǰ�˿�����
        /// <summary>
        /// ��ȡ�����ļ��е�ǰ�˿�����
        /// </summary>
        /// <param></param>
        /// <returns>��ǰ�˿�����</returns>
        public CommPort GetCommPortSet(string key)
        {
            string portNum = "COM1";
            string parity ="��";
            string stopBits = "1";
            string portSetString = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            string[] portSetArray=portSetString.Split(';');
            HexCon hexCon = new HexCon();
            for(int i=0;i<portSetArray.Length;i++)
            {
                if(portSetArray[i].Length!=0)
                {                   
                    switch (portSetArray[i].Split('=')[0].ToString())
                    {
                        case "PortNum":
                            portNum = portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "BaudRate":
                            this.BaudRate=Convert.ToInt32(portSetArray[i].Split('=')[1]);
                            break;
                        case "DataBits":
                            this.ByteSize = hexCon.StringToByte("0" + portSetArray[i].Split('=')[1].ToString())[0];
                            break;
                        case "Parity":
                            parity= portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "StopBits":
                            stopBits= portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "IsEnabled":
                            this.IsEnabled = portSetArray[i].Split('=')[1].ToString().ToLower() == "true" ? true : false; 
                            break;
                    }
                    
                    this.PortNum = int.Parse(portNum.Substring(3));
                    //switch (portNum.ToUpper())
                    //{
                    //    case "COM1":
                    //        this.PortNum = 1;
                    //        break;
                    //    case "COM2":
                    //        this.PortNum = 2;
                    //        break;
                    //    case "COM3":
                    //        this.PortNum = 3;
                    //        break;
                    //    case "COM4":
                    //        this.PortNum = 4;
                    //        break;
                    //}
                    switch (parity)
                    {
                        case "��":
                            this.Parity = 0;
                            break;
                        case "��":
                            this.Parity = 1;
                            break;
                        case "ż":
                            this.Parity = 2;
                            break;
                        case "��־":
                            this.Parity = 3;
                            break;
                        case "�ո�":
                            this.Parity = 4;
                            break;

                    }
                    switch (stopBits)
                    {
                        case "1":
                            this.StopBits = 0;
                            break;
                        case "1.5":
                            this.StopBits = 1;
                            break;
                        case "2":
                            this.StopBits = 2;
                            break;
                    }
                }
            }
            this.ReadTimeout = 100;
            return this;
        }


        public string[] GetCommPortSetForParameter(string key)
        {
            string portSetString = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            string[] portSetArray = portSetString.Split(';');
            HexCon hexCon = new HexCon();
            for (int i = 0; i < portSetArray.Length; i++)
            {
                if (portSetArray[i].Length != 0)
                {
                    switch (portSetArray[i].Split('=')[0].ToString())
                    {
                        case "PortNum":
                            portSetArray[i] = portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "BaudRate":
                            portSetArray[i] = portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "DataBits":
                            portSetArray[i] = portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "Parity":
                            portSetArray[i] = portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "StopBits":
                            portSetArray[i] = portSetArray[i].Split('=')[1].ToString();
                            break;
                        case "IsEnabled":
                            portSetArray[i] = portSetArray[i].Split('=')[1].ToString();
                            break;
                    }
                   
                }
            }
            this.ReadTimeout = 100;
            return portSetArray;
        }

        #endregion

        #region ��մ��ڻ���������
        public bool ClearPortData()
        {
            bool result = false;
            if(hComm!=INVALID_HANDLE_value)
            {
                result = PurgeComm(hComm, 0);
            }
            return result;
        }
        #endregion

        #region ��õ�ǰ����״̬
        //public bool GetCommState()
        //{
        //    if (hComm != INVALID_HANDLE_value)
        //    {
        //        return GetCommState(hComm);
        //    }
        //    else
        //    {
        //        return false;
        //    }           
        //}
        #endregion
    }
}

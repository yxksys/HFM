/**
 * ________________________________________________________________________________ 
 *
 *  描述：生成报文，发送报文，接受报文，解析报文
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：报文类
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    public class Message
    {
        #region 报文公共字段属性
        //发送的数据报文
        byte[] MySendMessageData = new byte[62];
        #endregion

        #region 生成下发下位机的读参数指令P和C  2020年2月12日 10:34:01
        /// <summary>
        /// 生成发下位机的读参数指令P和C
        /// </summary>
        /// <param name="read">read:C/P大写</param>
        /// <returns>返回相应报文</returns>
        public byte[] BuildMessage(char read)
        {
            
            switch (read)
            {
                case 'C':
                    MySendMessageData[0] = Convert.ToByte('C');
                    MySendMessageData[61] =?????;
                    break;
                case 'P':
                    MySendMessageData[0] = Convert.ToByte('P');
                    break;
                default:
                    break;
            }
            return MySendMessageData;
        }
        #endregion

        #region 生成下发下位机的Alpha和Beta自检报文 2020年2月12日 10:34:26
        /// <summary>
        /// 生成下发下位机的Alpha和Beta自检报文
        /// </summary>
        /// <param name="checkType">1:Alpha通讯协议,2:beta通讯协议</param>
        /// <returns>返回选择的报文协议</returns>
        public byte[] BuildMessage(int checkType)
        {
            int i = 0;
            //初始化报文
            for (i = 0; i < 62; i++)
            {
                MySendMessageData[i] = 0;
            }
            switch (checkType)
            {
                case 1:
                    i = 0;
                    MySendMessageData[i++] = 0x5A;
                    MySendMessageData[i++] = 0x00;
                    MySendMessageData[i++] = 0x0B;
                    MySendMessageData[i++] = 0x03;
                    MySendMessageData[i++] = 0xE8;
                    MySendMessageData[i++] = 0x00;
                    MySendMessageData[i++] = 0x01;//01是alpha,00是beta
                    MySendMessageData[i++] = 0x00;
                    MySendMessageData[i++] = 0x03;
                    break;
                case 0:
                    i = 0;
                    MySendMessageData[i++] = 0x5A;
                    MySendMessageData[i++] = 0x00;
                    MySendMessageData[i++] = 0x0B;
                    MySendMessageData[i++] = 0x03;
                    MySendMessageData[i++] = 0xE8;
                    MySendMessageData[i++] = 0x00;
                    MySendMessageData[i++] = 0x00;//01是alpha,00是beta
                    MySendMessageData[i++] = 0x00;
                    MySendMessageData[i++] = 0x03;
                    break;
                default:

                    break;
            }
            return MySendMessageData;
        }
        #endregion

        #region 生成下发下位机的自检报文 2020年2月12日 10:34:57 
        /// <summary>
        /// 生成下发下位机的自检报文
        /// </summary>
        /// <param name="pulsNumber">脉冲数</param>
        /// <param name="pulsHV">脉冲高低压电平</param>
        /// <param name="pulsWidth">控制信号</param>
        /// <param name="ctrlSignal">脉冲宽度</param>
        /// <returns>返回自检报文</returns>
        public byte[] BuildMessage(int pulsNumber,int pulsHV,int pulsWidth,int ctrlSignal)
        {
            int i = 0;
            MySendMessageData[i++] = 0x5A;
            MySendMessageData[i++] = 0x00;
            MySendMessageData[i++] = 0x0B;
            //脉冲数
            MySendMessageData[i++] = Convert.ToByte(pulsNumber/256);
            MySendMessageData[i++] = Convert.ToByte(pulsNumber%256);
            //脉冲高低压电平
            MySendMessageData[i++] = Convert.ToByte(pulsHV);
            //控制信号
            MySendMessageData[i++] = Convert.ToByte(pulsWidth);
            //脉冲宽度
            MySendMessageData[i++] = Convert.ToByte(ctrlSignal/256);
            MySendMessageData[i++] = Convert.ToByte(ctrlSignal%256);
            return MySendMessageData;
        }
        #endregion 

        #region 生成下发下位机的写道盒参数报文（P写参数命令码）
        public byte[] BuildMessage(ChannelParameter channelParameter)
        {
            return null;
        }
        #endregion     

        #region 解析从下位机读回的报文（P上传参数命令码和C上传测量值命令码）
        /// <summary>
        /// 解析从下位机读回的报文
        /// P参数命令码返回ChannelParameter对象列表
        /// C测量值命令码返回MeasureData对象列表
        /// </summary>
        /// <typeparam name="T">返回对象泛型列表，P参数命令码：ChannelParameter对象列表； C测量值命令码：MeasureData对象列表</typeparam>
        /// <param name="message">下位机上传的报文信息</param>
        /// <returns>解析后的报文对象列表</returns>
        public IList<T> ExplainMessage<T>(string message)
        {
            //解析报文头
            //P参数命令码：返回ChannelParameter对象列表；
            //C测量值命令码：返回MeasureData对象列表
            return null;
        }
        #endregion
        
        #region 发送报文信息
        /// <summary>
        ///  通过串口向下位机发送报文信息（串口已经打开）
        ///  </summary>
        /// <param name="BuffMessage">存储报文信息缓冲区</param>
        /// <param name="commport">传输报文端口</param>
        /// <returns>true  发送成功
        ///          false 发送失败  </returns>
        public bool SendMessage(byte[] BuffMessage, CommPort commport)
            {
                //串口已打开

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
                            return false;
                        }
                    }
                }
            }

        #endregion

        #region 接收报文信息
        /// <summary>
        ///  通过串口接收下位机上传的采集数据报文（串口已经打开）		
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
                        return "";
                    }

                }
            }
        }
        #endregion
    }
}

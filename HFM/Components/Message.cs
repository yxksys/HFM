/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：
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
    abstract class Message
    {
        #region 生成下发下位机的Alpha和Beta自检报文
        /// <summary>
        /// 生成下发下位机的Alpha和Beta自检报文
        /// 报文格式：Alpha自检：0x5A,0x00,0x0B,0x03,0xEB,0x00,0x01,0x00,0x03
        ///           Beta自检： 0x5A,0x00,0x0B,0x03,0xEB,0x00,0x00,0x00,0x03
        /// </summary>
        /// <param name="checkType">自检类型，0：Alpha1：Beta</param>
        /// <returns>自检报文数组</returns>
        public static byte[] BuildMessage(int checkType)
        {
            byte[] messageData = new byte[62];
            switch (checkType)
            {
                case 0://Alpha自检
                    messageData[0] = 0x5A;
                    messageData[1] = 0x00;
                    messageData[2] = 0x0B;
                    messageData[3] = 0x03;
                    messageData[4] = 0xEB;
                    messageData[5] = 0x00;
                    messageData[6] = 0x01;
                    messageData[7] = 0x00;
                    messageData[8] = 0x03;
                    break;
                case 1://Beat自检
                    messageData[0] = 0x5A;
                    messageData[1] = 0x00;
                    messageData[2] = 0x0B;
                    messageData[3] = 0x03;
                    messageData[4] = 0xEB;
                    messageData[5] = 0x00;
                    messageData[6] = 0x00;
                    messageData[7] = 0x00;
                    messageData[8] = 0x03;
                    break; 
            }

            return messageData;
        }
        #endregion

        #region 生成下发下位机的自检报文
        /// <summary>
        /// 生成下发下位机的自检报文
        /// 报文格式：0x5A,0x00,0x0B,脉冲数（2字节）,脉冲高低电平（1字节）,控制信号（1字节）,脉冲宽度（2字节）
        /// </summary>
        /// <param name="pulsNumber">脉冲数</param>
        /// <param name="pulsHV">脉冲高低电平0:低电平，1：高电平</param>
        /// <param name="pulsWidth">脉冲宽度</param>
        /// <param name="ctrlSignal">控制信号0：低，1：高</param>
        /// <returns>自检报文数组</returns>
        public static byte[] BuildMessage(int pulsNumber,int pulsHV,int ctrlSignal,int pulsWidth)
        {
            byte[] messageData = new byte[62];
            messageData[0] = 0x5A;
            messageData[1] = 0x00;
            messageData[2] = 0x0B;
            //脉冲数
            messageData[3] = Convert.ToByte(pulsNumber / 256);
            messageData[4] = Convert.ToByte(pulsNumber % 256);
            //脉冲高低电平
            if(pulsHV==0)
            {
                messageData[5] = 0x00;
            }
            else
            {
                messageData[5] = 0x01;
            }       
            //控制信号
            if(ctrlSignal==0)
            {
                messageData[6] = 0x00;
            }
            else
            {
                messageData[6] = 0x01;
            }
            //脉冲宽度
            messageData[7] = Convert.ToByte(pulsWidth / 256);
            messageData[8] = Convert.ToByte(pulsWidth % 256);
            return messageData;
        }
        #endregion

        #region 生成下发下位机的写道盒参数报文（P写参数命令码）
        /// <summary>
        /// 生成下发下位机的写道盒参数报文（P写参数命令码）
        /// 报文格式:'P',通道（1~7），Alpha阈值（1字节），Beta阈值（1字节），高压（2字节），AD因子（2字节），
        /// DA因子（2字节），高压因子（2字节），工作时间（2字节），高压倍数（2字节）...。共四个通道参数值，'P'（第62个字节）
        /// 特别注意：通道ID1-7，但是一个数据包最多封装4个通道，因此7个通道需封装成两个报文数据包分别进行下发。
        ///           第1次，封装通道1-4并下发，第二次，封装通道5-7并下发
        /// </summary>
        /// <param name="channelParameter">道盒对象</param>
        /// <returns>道盒参数（P写参数命令码）报文数组</returns>
        public static byte[] BuildMessage(IList<ChannelParameter> channelParameterS)
        {
            byte[] messageData = new byte[62];
            int j = 1;
            //报文头，1字节
            messageData[0] =Convert.ToByte('p');
            //循环生成4个通道的报文，每个通道15个字节
            for(int i=0; i<4; i++)
            {               
                //通道ID，1字节
                messageData[j] = Convert.ToByte(channelParameterS[i].Channel.ChannelID);
                //Alpha阈值，1字节
                messageData[j + 1] = Convert.ToByte(channelParameterS[i].AlphaThreshold/10);
                //Beta阈值，1字节
                messageData[j + 2] = Convert.ToByte(channelParameterS[i].BetaThreshold/10);
                //高压值，2字节
                messageData[j + 3] = Convert.ToByte(channelParameterS[i].PresetHV/256);
                messageData[j + 4] = Convert.ToByte(channelParameterS[i].PresetHV%256);
                //AD因子，2字节
                messageData[j + 5] = Convert.ToByte(channelParameterS[i].ADCFactor/256);
                messageData[j + 6] = Convert.ToByte(channelParameterS[i].ADCFactor % 256);
                //DA因子，2字节
                messageData[j + 7] = Convert.ToByte(channelParameterS[i].DACFactor / 256);
                messageData[j + 8] = Convert.ToByte(channelParameterS[i].DACFactor % 256);
                //高压因子，2字节
                messageData[j + 9] = Convert.ToByte(channelParameterS[i].HVFactor / 256);
                messageData[j + 10] = Convert.ToByte(channelParameterS[i].HVFactor % 256);
                //工作时间，2字节
                messageData[j + 11] = Convert.ToByte(channelParameterS[i].WorkTime / 256);
                messageData[j + 12] = Convert.ToByte(channelParameterS[i].WorkTime % 256);
                //高压倍数，2字节
                messageData[j + 13] = Convert.ToByte(channelParameterS[i].HVRatio / 256);
                messageData[j + 14] = Convert.ToByte(channelParameterS[i].HVRatio % 256);
                j = j + 15;
            }
            //报文结束标志，1字节
            messageData[61] = Convert.ToByte('p');
            return messageData;
        }
        #endregion     

        #region 解析从下位机读回的报文（P上传参数命令码和C上传测量值命令码）
        /// <summary>
        /// 解析从下位机读回的报文
        /// P参数命令码返回ChannelParameter对象列表
        /// 报文格式：'p'，通道编号（1字节），Alph阈值（1字节），Beta阈值（1字节），高压值（2字节），AD因子（2字节），DA因子（2字节），高压因子（2字节），工作时间（2字节），高压倍数（2字节）。。。。共四个通参数量值
        /// C测量值命令码返回MeasureData对象列表
        /// 报文格式：‘c’,通道编号（1字节），Alpha计数值（4字节），Beta计数值（4字节），模拟电压值（2字节），数字电压值（2字节），高压值（2字节）。。。。共四个通道测量值，红外状态(第62个字节)
        /// 特别注意：通道编号1-7，但是一个数据包最多封装4个通道，因此7个通道下位机封装成两个报文数据包一次性上传到上位机。
        ///           下位机上传报文：'报文头p/c'，通道1-4上传数据，'报文头p/c',通道5-7并上传数据
        /// </summary>
        /// <typeparam name="T">返回对象泛型列表，P参数命令码：ChannelParameter对象列表； C测量值命令码：MeasureData对象列表</typeparam>
        /// <param name="message">下位机上传的报文信息</param>
        /// <returns>解析后的报文对象列表</returns>
        public IList<T> ExplainMessage<T>(string message)
        {
            //初始化返回P命令码道盒参数对象列表
            IList<ChannelParameter> channelParameterS = new List<ChannelParameter>();
            //初始化返回C命令码测试数据对象列表
            IList<MeasureData> measureDataS = new List<MeasureData>();           
            //报文类型 0:p 1:c
            int messageType = 0;
            //报文信息不够则返回null
            if(message.Length<=62)
            {
                return null;
            }
            //一次读取数据包含两个报文数据包，分别为通道1-4和通道5-7
            for (int packageIndex = 0; packageIndex < message.Length; packageIndex += 62)
            {
                //定义指向通道第一个数据的索引值，为报文头之后下一个数据
                int channelHeadIndex = packageIndex + 1;
                //解析报文头
                switch (message.ElementAt<char>(packageIndex))
                {
                    case 'p':  //'p'上传参数
                        //设置当前报文类型
                        messageType = 0;
                        //解析当前数据包四个通道数据报文
                        for (int j = 0; j < 4; j++)
                        {
                            //当前通道第一个数据索引大于报文长度，数据全部解析完成停止解析
                            if (channelHeadIndex>=message.Length) 
                            {
                                break;
                            }
                            //按照报文格式，分别取出本通道相关道盒参数值，从当前报文第一个数据索引开始，连续读取15个字节数据进行解析
                            int channelID = Convert.ToInt32(message.ElementAt<char>(channelHeadIndex).ToString());//通道ID一个字节
                            float alphaThreshold = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 1).ToString())*10;//Alpha值一个字节
                            float betaThreshold = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 2).ToString())*10;//Beta值一个字节
                            float presetHV = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 3).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 4).ToString());//高压值两个字节
                            float aDCFactor = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 5).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 6).ToString());//AD因子两个字节
                            float dACFactor = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 7).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 8).ToString());//DA因子两个字节
                            float hVFactor = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 9).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 10).ToString());//高压值两个字节
                            float workTime = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 11).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 12).ToString());//工作时间两个字节
                            float hVRatio = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 13).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 14).ToString());//高压值两个字节
                                                                                                                                                                                                            //按照解析的道盒参数构造道盒参数对象                                                                               
                            ChannelParameter channelParameter = new ChannelParameter(channelID, alphaThreshold, betaThreshold, presetHV, aDCFactor, dACFactor, hVFactor, workTime, hVRatio);
                            //将构造的道盒参数对象添加到列表中
                            channelParameterS.Add(channelParameter);
                            //更新报文指针
                            channelHeadIndex = channelHeadIndex + 15;
                        }
                        break;
                    case 'c': //'c'上传测量值
                        messageType = 1;
                        //解析当前数据包四个通道数据报文
                        for (int j = 0; j < 4; j++)
                        {
                            //当前通道第一个数据索引大于报文长度，数据全部解析完成停止解析
                            if (channelHeadIndex >= message.Length)
                            {
                                break;
                            }
                            //按照报文格式，分别取出本通道测量数值，从当前报文第一个数据索引开始，连续读取15个字节数据进行解析
                            int channelID = Convert.ToInt32(message.ElementAt<char>(channelHeadIndex).ToString());//通道ID一个字节

                            float alpha = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 1).ToString())*256*256*256+ Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 2).ToString())*256*256
                                          +Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 3).ToString())*256+ Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 4).ToString());//Alpha计数值四个字节

                            float beta = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 5).ToString())*256*256*256+ Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 6).ToString())*256*256 
                                         +Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 7).ToString())*256+ Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 8).ToString());//Beta计数值四个字节

                            float analogV = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 9).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 10).ToString());//模拟电压值两个字节

                            float digitalV = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 11).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 12).ToString());//数字电压值两个字节

                            float hV = Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 13).ToString()) * 256 + Convert.ToSingle(message.ElementAt<char>(channelHeadIndex + 14).ToString());//高压值两个字节                            
                            
                            //按照解析的测量数据构造测量数据对象
                            //其它参数给默认值
                            MeasureData measureData = new MeasureData(channelID,DateTime.Now,alpha,beta,analogV,digitalV,hV);
                            //将构造的道盒参数对象添加到列表中
                            measureDataS.Add(measureData);
                            //更新报文指针
                            channelHeadIndex = channelHeadIndex + 15;
                        }
                        //报文最后一个字节为红外状态                       
                        int infraredStatus = Convert.ToInt32(message.ElementAt<char>(channelHeadIndex).ToString());
                        //infraredStatus报文格式：衣物探头状态数据位（1bit）、右手状态数据位（1bit）、左手状态数据位（1bit）
                        //数据的值 0：手部到位/衣物探头未拿起 1：手部不到位/衣物探头拿起
                        if (packageIndex == 0) //第一个数据包1-4通道为手部探头
                        {
                            //左手到位
                            if (infraredStatus == 0 || infraredStatus == 2 || infraredStatus == 4 || infraredStatus == 6)
                            {
                                measureDataS[0].InfraredStatus = 1;
                                measureDataS[1].InfraredStatus = 1;
                            }
                            //右手到位
                            if (infraredStatus == 0 || infraredStatus == 1 || infraredStatus == 4 || infraredStatus == 5)
                            {
                                measureDataS[2].InfraredStatus = 1;
                                measureDataS[3].InfraredStatus = 1;
                            }
                        }
                        else//第二个数据包为5-7为脚步探头和衣物探头
                        {
                            //衣物探头拿起
                            if (infraredStatus == 4 || infraredStatus == 5 || infraredStatus == 6 || infraredStatus == 7)
                            {
                                measureDataS[6].InfraredStatus = 1;
                            }
                        }
                        break;
                }
            }
            //根据当前解析报文类型返回对应数据对象列表
            if(messageType==0) //p命令码报文,返回ChannelParameter对象列表
            {                
                return (List<T>)channelParameterS;
            }
            else //c命令码报文，返回MeasureData对象列表
            {
                return (List<T>) measureDataS;
            }        
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

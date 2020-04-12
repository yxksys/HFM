/**
 * ________________________________________________________________________________ 
 *
 *  描述：完成上位机与下位机报文传输与解析
 *  作者：杨慧炯
 *  版本：V1.0
 *  创建时间：2020-02-11
 *  类名：Message
 *  更新记录：2020-02-14修正了Alpha和Beta自检报文中两个错误报文数据；
 *            2020-02-21修正了解析报文中Alpha和Beta计数值四个字节信息的解析方式
 *            2020-03-07增加了生成“向管理机上报监测状态”报文方法
 *                      增加了解析管理机下发报文方法（上报监测状态指令码和时间同步指令码）
 *          杨旭锴2020年3月23日修改了道盒下发指令,从大写P改为小写p,以及把下发数字的除数改为int型,浮点型会对下发后的数字造成数字增加256.
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
        /// <param name="checkType">自检类型，0：Alpha 1：Beta</param>
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
                    messageData[4] = 0xE8;
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
                    messageData[4] = 0xE8;
                    messageData[5] = 0x00;
                    messageData[6] = 0x00;
                    messageData[7] = 0x00;
                    messageData[8] = 0x03;
                    break; 
            }

            return messageData;
        }

        /// <summary>
        ///  /// 生成下发下位机的Alpha和Beta自检报文
        /// 报文格式：Alpha自检：0x5A,checkTime/256，checkTime%256,0x03,0xEB,0x00,0x01,0x00,0x03
        ///           Beta自检： 0x5A,checkTime/256，checkTime%256,0x03,0xEB,0x00,0x00,0x00,0x03
        /// </summary>
        /// <param name="checkType">自检类型，0：Alpha1：Beta</param>
        /// <param name="checkTime">自检报文数组</param>
        /// <returns></returns>
        public static byte[] BuildMessage(int checkType,int checkTime)
        {
            byte[] messageData = new byte[62];
            switch (checkType)
            {
                case 0://Alpha自检
                    messageData[0] = 0x5A;
                    messageData[1] = Convert.ToByte(checkTime/256);
                    messageData[2] = Convert.ToByte (checkTime%256);
                    messageData[3] = 0x03;
                    messageData[4] = 0xE8;
                    messageData[5] = 0x00;
                    messageData[6] = 0x01;
                    messageData[7] = 0x00;
                    messageData[8] = 0x03;
                    break;
                case 1://Beat自检
                    messageData[0] = 0x5A;
                    messageData[1] = Convert.ToByte(checkTime / 256);
                    messageData[2] = Convert.ToByte(checkTime % 256);
                    messageData[3] = 0x03;
                    messageData[4] = 0xE8;
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
        /// 报文格式:'p',通道（1~7），Alpha阈值（1字节），Beta阈值（1字节），高压（2字节），AD因子（2字节），
        /// DA因子（2字节），高压因子（2字节），工作时间（2字节），高压倍数（2字节）...。共四个通道参数值，'p'（第62个字节）
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
                messageData[j + 3] = Convert.ToByte((int)(channelParameterS[i].PresetHV / 256));
                messageData[j + 4] = Convert.ToByte(channelParameterS[i].PresetHV%256);
                //AD因子，2字节
                messageData[j + 5] = Convert.ToByte((int)(channelParameterS[i].ADCFactor / 256));
                messageData[j + 6] = Convert.ToByte(channelParameterS[i].ADCFactor % 256);
                //DA因子，2字节
                messageData[j + 7] = Convert.ToByte((int)(channelParameterS[i].DACFactor / 256));
                messageData[j + 8] = Convert.ToByte(channelParameterS[i].DACFactor % 256);
                //高压因子，2字节
                messageData[j + 9] = Convert.ToByte((int)(channelParameterS[i].HVFactor / 256));
                messageData[j + 10] = Convert.ToByte(channelParameterS[i].HVFactor % 256);
                //工作时间，2字节
                messageData[j + 11] = Convert.ToByte((int)(channelParameterS[i].WorkTime / 256));
                messageData[j + 12] = Convert.ToByte(channelParameterS[i].WorkTime % 256);
                //高压倍数，2字节
                messageData[j + 13] = Convert.ToByte((int) (channelParameterS[i].HVRatio / 256));
                messageData[j + 14] = Convert.ToByte(channelParameterS[i].HVRatio % 256);
                j = j + 15;
            }
            //报文结束标志，1字节
            messageData[61] = Convert.ToByte('p');
            return messageData;
        }
        #endregion

        #region 生成上报管理机监测状态报文
        /// <summary>
        /// 生成上报管理机监测状态报文
        /// 报文格式：监测仪地址（0~255）、功能码0x03、数据长度0x0A、
        ///           年（2字节：0x1413表示2019年）、月日（2字节：0x0C0F表示12月15日）
        ///           时分（2字节：0x1012表示16时18分）、妙微妙（2字节：0x3B00表示59秒）、
        ///           监测仪状态（2字节：0x0001表示正常，0x0002表示故障，0x0004表示污染）、
        ///           16位CRC校验（2字节：校验码高位-校验码低位）                
        /// </summary>
        /// <param name="deviceAddress">设备地址</param>
        /// <param name="submitTime">上报时间</param>
        /// <param name="deviceStatus">设备状态</param>
        /// <returns></returns>
        public static byte[] BuildMessage(int deviceAddress,DateTime submitTime,int deviceStatus)
        {
            byte[] messageData = new byte[16];
            messageData[0] = Convert.ToByte(deviceAddress);
            messageData[1] = 0x03;
            messageData[2] = 0x0A;
            messageData[3] = Convert.ToByte(submitTime.Year / 100);
            messageData[4] = Convert.ToByte(submitTime.Year % 100);
            messageData[5] = Convert.ToByte(submitTime.Month);
            messageData[6] = Convert.ToByte(submitTime.Day);
            messageData[7] = Convert.ToByte(submitTime.Hour);
            messageData[8] = Convert.ToByte(submitTime.Minute);
            messageData[9] = Convert.ToByte(submitTime.Second);
            messageData[10] = Convert.ToByte(submitTime.Millisecond);
            //监测状态，2字节
            messageData[11] = 0x00;
            switch (deviceStatus)//控制板中16、32、64分别表示正常、故障、污染。上报管理机时1、2、4分别表示正常、故障、污染
            {
                case 16:
                    deviceStatus = 0x01;
                    break;
                case 32:
                    deviceStatus = 0x02;
                    break;
                case 64:
                    deviceStatus = 0x04;
                    break;
            }
            messageData[12] = Convert.ToByte(deviceStatus);
            //求CRC校验值
            byte[] crc16 = new byte[2];
            crc16=Tools.CRC16(messageData, messageData.Length - 1);
            messageData[13] = crc16[0];
            messageData[14] = crc16[1];
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
        public static IList<T> ExplainMessage<T>(byte[]  message)
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
                
                switch (Convert.ToChar(message[packageIndex]))
                {
                    case 'P':  //'P'上传参数
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
                            int channelID = Convert.ToInt32(message[channelHeadIndex]);//通道ID一个字节
                            float alphaThreshold = Convert.ToSingle(message[channelHeadIndex + 1])*10;//Alpha值一个字节,数值放大10倍
                            float betaThreshold = Convert.ToSingle(message[channelHeadIndex + 2])*10;//Beta值一个字节,数值放大10倍
                            float presetHV = Convert.ToSingle(message[channelHeadIndex + 3]) * 256;
                            presetHV += Convert.ToSingle(message[channelHeadIndex + 4]);//高压值两个字节
                            float aDCFactor = Convert.ToSingle(message[channelHeadIndex + 5]) * 256;
                            aDCFactor += Convert.ToSingle(message[channelHeadIndex + 6]);//AD因子两个字节
                            float dACFactor = Convert.ToSingle(message[channelHeadIndex + 7]) * 256;
                                dACFactor +=Convert.ToSingle(message[channelHeadIndex + 8]);//DA因子两个字节
                            float hVFactor = Convert.ToSingle(message[channelHeadIndex + 9]) * 256;
                            hVFactor += Convert.ToSingle(message[channelHeadIndex + 10]);//高压值两个字节
                            float workTime = Convert.ToSingle(message[channelHeadIndex + 11]) * 256;
                            workTime += Convert.ToSingle(message[channelHeadIndex + 12]);//工作时间两个字节
                            float hVRatio = Convert.ToSingle(message[channelHeadIndex + 13]) * 256;
                            hVRatio += Convert.ToSingle(message[channelHeadIndex + 14]);//高压值两个字节
                            //按照解析的道盒参数构造道盒参数对象                                                                               
                            ChannelParameter channelParameter = new ChannelParameter(channelID, alphaThreshold, betaThreshold, presetHV, aDCFactor, dACFactor, hVFactor, workTime, hVRatio);
                            //将构造的道盒参数对象添加到列表中
                            channelParameterS.Add(channelParameter);                            
                            //更新报文指针
                            channelHeadIndex = channelHeadIndex + 15;
                        }
                        break;
                    case 'C': //'C'上传测量值
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
                            int channelID = Convert.ToInt32(message[channelHeadIndex]);//通道ID一个字节

                            float beta = Convert.ToSingle(message[channelHeadIndex + 1]);
                            beta += Convert.ToSingle(message[channelHeadIndex + 2]) * 256;
                            beta += Convert.ToSingle(message[channelHeadIndex + 3]) * 256 * 256;
                            beta += Convert.ToSingle(message[channelHeadIndex + 4]) * 256 * 256 * 256;//Alpha计数值四个字节,由低位向高位

                            float alpha = Convert.ToSingle(message[channelHeadIndex + 5]);
                            alpha += Convert.ToSingle(message[channelHeadIndex + 6]) * 256;
                            alpha += Convert.ToSingle(message[channelHeadIndex + 7]) * 256 * 256;
                            alpha += Convert.ToSingle(message[channelHeadIndex + 8]) * 256 * 256 * 256;//Beta计数值四个字节,由低位向高位

                            float analogV = Convert.ToSingle(message[channelHeadIndex + 9]) * 256;
                            analogV += Convert.ToSingle(message[channelHeadIndex + 10]);//模拟电压值两个字节

                            float digitalV = Convert.ToSingle(message[channelHeadIndex + 11]) * 256;
                            digitalV += Convert.ToSingle(message[channelHeadIndex + 12]);//数字电压值两个字节

                            float hV = Convert.ToSingle(message[channelHeadIndex + 13]) * 256;
                            hV += Convert.ToSingle(message[channelHeadIndex + 14]);//高压值两个字节                            

                            //按照解析的测量数据构造测量数据对象
                            //其它参数给默认值                            
                            //channel.GetChannel(channelID); 
                            Channel channel = new Channel();
                            MeasureData measureData = new MeasureData();
                            //channel.GetChannel(channelID);
                            channel.ChannelID = channelID;                           
                            measureData.Channel = channel;
                            measureData.Alpha = alpha;
                            measureData.Beta = beta;
                            measureData.AnalogV = analogV;
                            measureData.DigitalV = digitalV;
                            measureData.HV = hV;
                            //(channelID,DateTime.Now,alpha,beta,analogV,digitalV,hV);
                            //将构造的道盒参数对象添加到列表中
                            measureDataS.Add(measureData);
                            //更新报文指针
                            channelHeadIndex = channelHeadIndex + 15;
                        }
                        //报文最后一个字节为红外状态                       
                        int infraredStatus = Convert.ToInt32(message[channelHeadIndex]);
                        //infraredStatus报文格式：衣物探头状态数据位（1bit）、右手状态数据位（1bit）、左手状态数据位（1bit）
                        //数据的值 0：手部到位/衣物探头未拿起 1：手部不到位/衣物探头拿起
                        if (packageIndex == 0) //第一个数据包1-4通道为手部探头
                        {
                            //左手到位
                            if ((infraredStatus & 1)==0)
                            {
                                measureDataS[0].InfraredStatus = 1;
                                measureDataS[1].InfraredStatus = 1;
                            }
                            ////右手到位
                            if ((infraredStatus & 2)==0)
                            {
                                measureDataS[2].InfraredStatus = 1;
                                measureDataS[3].InfraredStatus = 1;
                            }
                        }
                        else//第二个数据包为5-7为脚步探头和衣物探头
                        {
                            //衣物探头拿起
                            if ((infraredStatus & 4)==4)
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

        #region 解析从管理机下发的报文（上报监测状态/时间同步）
        /// <summary>
        /// 解析从管理机下发的报文（上报监测状态/时间同步）
        /// </summary>
        /// <param name="message">管理机下发的报文信息</param>
        /// <returns>解析后的报文数据：上报监测状态返回监测仪地址数组（长度为1），时间同步返回标准时间数组（长度为7：年月日时分秒毫秒）</returns>
        public static int[] ExplainMessage(byte[] message)
        {
            int[] messageData=null;            
            //报文长度不能小于8字节
            if (message.Length<8)
            {
                return null;
            }
            //进行CRC校验
            byte[] crc16 = new byte[2];
            crc16 = Tools.CRC16(message, message.Length - 2);
            //校验失败返回
            if(message[message.Length-2]!=crc16[1] || message[message.Length-1]!=crc16[0])
            {
                return null;
            }
            //校验成功
            switch(message[1])
            {                
                case 0x03://向管理机上报监测状态报文
                    messageData = new int[1];
                    messageData[0] = message[0];
                    break;                   
                case 0x10://进行时间同步报文
                    if (message.Length >= 17)//报文长度满足要求
                    {
                        messageData = new int[7];
                        messageData[0] = message[7] * 100 + message[8];//年
                        for(int i=1;i<messageData.Length;i++)
                        {
                            //messageData[1]到messageData[6]分别存储：月、日、时、分、妙、毫秒
                            messageData[i] = message[i + 8];
                        }                        
                    }
                    break;
            }
            return messageData;
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
        public static bool SendMessage(byte[] BuffMessage, CommPort commport)
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
        ///                 接收失败：返回null  </returns>
        public static byte[] ReceiveMessage(CommPort commport)
        {
            //串口已打开            
            int NumBytes;
            HexCon hexcon = new HexCon();
            NumBytes = 200;
            byte[] RecBuf = new byte[200];
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
                    return null;
                else
                {
                    //读串口数据到RecBuf
                    try
                    {
                        //接收下位机上传的采集数据报文，将其从byte型转换为string类型(十六进制)并返回
                        RecBuf = commport.Read(NumBytes);                        
                        return RecBuf;
                    }
                    catch
                    {
                        return null;                        
                    }

                }
            }
        }
        #endregion
    }
}

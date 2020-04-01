/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：杨旭锴
 *  版本：
 *  创建时间：2020年2月16日 16:58:28
 *  类名：通道类Channel
 *  修改:2020年2月24日 10:59:58
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace HFM.Components
{ 
    class Channel
    {
        #region 数据库查询语言
        //获得全部通道列表
        private const string SQL_SELECT_CHANNEL = "SELECT ChannelID, ChannelName, ChannelName_English," +
                                                                " ProbeArea, Status, IsEnabled " +
                                                                "FROM HFM_DIC_Channel ";
        //根据启用状态查询通道列表
        private const string SQL_SELECT_CHANNEL_BY_ISENABLED = "SELECT ChannelID, ChannelName, ChannelName_English," +
                                                                " ProbeArea, Status, IsEnabled " +
                                                                "FROM HFM_DIC_Channel " +
                                                                "WHERE IsEnabled = @IsEnabled";
        //按通道ChannelID查询
        private const string SQL_SELECT_CHANNEL_BY_CHANNELID = "SELECT ChannelID, ChannelName, ChannelName_English," +
                                                                " ProbeArea, Status, IsEnabled " +
                                                                "FROM HFM_DIC_Channel " +
                                                                "WHERE ChannelID = @ChannelID";
        //按通道名称ChannelName查询
        private const string SQL_SELECT_CHANNEL_BY_CHANNELNAME = "SELECT ChannelID, ChannelName, ChannelName_English," +
                                                                " ProbeArea, Status, IsEnabled " +
                                                                "FROM HFM_DIC_Channel " +
                                                                "WHERE ChannelName = @ChannelName";
        //按通道ChannelID更新通道开启状态IsEnabled
        private const string SQL_UPDATE_CHANNEL_BY_CHANNELID = "UPDATE  HFM_DIC_Channel " +
                                                               "SET IsEnabled = @IsEnabled " +
                                                               "WHERE (ChannelID = @ChannelID)";
        //按通道ChannelID更新探测面积
        private const string SQL_UPDATE_PROBEAREA_BY_CHANNELID = "UPDATE HFM_DIC_Channel SET ProbeArea = @ProbeArea" +
                                                                 " WHERE ChannelID = @ChannelID";
        #endregion

        #region 属性
        private int _channelID;
        private string _channelName;
        private string _channelName_English;
        private float _probeArea;
        private string _status;
        private bool _isEnabled;

        /// <summary>
        /// 通道ID
        /// </summary>
        public int ChannelID { get => _channelID; set => _channelID = value; }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get => _channelName; set => _channelName = value; }
        /// <summary>
        /// 通道英文名称
        /// </summary>
        public string ChannelName_English { get => _channelName_English; set => _channelName_English = value; }
        /// <summary>
        /// 测量面积
        /// </summary>
        public float ProbeArea { get => _probeArea; set => _probeArea = value; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get => _status; set => _status = value; }
        /// <summary>
        /// 通道是否启用
        /// </summary>
        public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }

        #endregion

        #region 构造函数
        public Channel()
        { }
        /// <summary>
        /// 构造函数（有参)
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <param name="channelName">通道名称</param>
        /// <param name="channelName_English">通道英文名称</param>
        /// <param name="probeArea">测量面积</param>
        /// <param name="status">状态</param>
        /// <param name="isEnabled">通道是否启用</param>
        public Channel(int channelID, string channelName, string channelName_English, float probeArea, string status, bool isEnabled)
        {
            this._channelID = channelID;
            this._channelName = channelName;
            this._channelName_English = channelName_English;
            this._probeArea = probeArea;
            this._status = status;
            this._isEnabled = isEnabled;

        }
        #endregion

        #region 获得全部通道列表
        /// <summary>
        /// 获得全部通道列表
        /// </summary>
        /// <returns>全部通道列表</returns>
        public IList<Channel> GetChannel()
        {
            //实例化列表
            IList<Channel> Ichannels = new List<Channel>();
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CHANNEL))
            {
                while (reader.Read())
                {
                    Channel channel = new Channel();
                    channel.ChannelID = Convert.ToInt32(reader["ChannelID"].ToString());
                    channel.ChannelName = Convert.ToString(reader["ChannelName"].ToString());
                    channel.ChannelName_English = Convert.ToString(reader["ChannelName_English"].ToString());
                    channel.ProbeArea = Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString());
                    channel.Status = Convert.ToString(reader["Status"].ToString());
                    channel.IsEnabled = Convert.ToBoolean(reader["IsEnabled"].ToString());
                    Ichannels.Add(channel);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return Ichannels;
        }
        #endregion

        #region 根据启用状态查询通道列表
        /// <summary>
        /// 根据启用状态查询通道列表
        /// </summary>
        /// <param name="isEnabled">false:通道不启用查询，true：通道启用查询</param>
        /// <returns></returns>
        public IList<Channel> GetChannel(bool isEnabled)
        {
            //实例化列表
            IList<Channel> Ichannels = new List<Channel>();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@IsEnabled",OleDbType.Boolean)
            };
            parms[0].Value = isEnabled;
            //查询语句
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CHANNEL_BY_ISENABLED, parms))
            {
                //读取数据赋值列表
                while (reader.Read())
                {
                    Channel channel = new Channel
                    {
                        ChannelID = Convert.ToInt32(reader["ChannelID"].ToString()),
                        ChannelName = Convert.ToString(reader["ChannelName"].ToString()),
                        ChannelName_English = Convert.ToString(reader["ChannelName_English"].ToString()),
                        ProbeArea = Convert.ToSingle(reader["ProbeArea"].ToString() == ""
                            ? "0"
                            : reader["ProbeArea"].ToString()),
                        Status = Convert.ToString(reader["Status"].ToString()),
                        IsEnabled = Convert.ToBoolean(reader["IsEnabled"].ToString())
                    };
                    Ichannels.Add(channel);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return Ichannels;
        }
        #endregion

        #region 根据通道ID查询通道信息   
        /// <summary>
        /// 根据通道ID查询通道信息
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <returns>通道ID对应的通道信息</returns>
        public Channel GetChannel(int channelID)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
            };
            parms[0].Value = channelID;
            //根据通道ID查询通道信息
            //Channel channel = new Channel();            
            //OleDbConnection conn=new OleDbConnection();
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CHANNEL_BY_CHANNELID,parms))
            {
                while (reader.Read())
                {
                    this.ChannelID = Convert.ToInt32(reader["ChannelID"].ToString());
                    this.ChannelName = Convert.ToString(reader["ChannelName"].ToString());
                    this.ChannelName_English = Convert.ToString(reader["ChannelName_English"].ToString());
                    this.ProbeArea = Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString());
                    this.Status = Convert.ToString(reader["Status"].ToString());
                    this.IsEnabled = Convert.ToBoolean(reader["IsEnabled"].ToString());
                }
                reader.Close();
                DbHelperAccess.Close();                
            }
            return this;
        }
        #endregion

        #region 根据通道名称查询通道信息
        /// <summary>
        /// 根据通道名称查询通道信息
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <returns></returns>
        public Channel GetChannel(string channelName)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ChannelName",OleDbType.VarChar,10)
            };
            parms[0].Value = channelName;
            //根据通道ID查询通道信息
            //Channel channel = new Channel();
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CHANNEL_BY_CHANNELNAME, parms))
            {               
                while (reader.Read())
                {
                    this.ChannelID = Convert.ToInt32(reader["ChannelID"].ToString());
                    this.ChannelName = Convert.ToString(reader["ChannelName"].ToString());
                    this.ChannelName_English = Convert.ToString(reader["ChannelName_English"].ToString());
                    this.ProbeArea = Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString());
                    this.Status = Convert.ToString(reader["Status"].ToString());
                    this.IsEnabled = Convert.ToBoolean(reader["IsEnabled"].ToString());
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return this;
        }
        #endregion

        #region 对某一类型通道的启用状态进行设置
        /// <summary>
        /// 对某一类型通道的启用状态进行设置
        /// </summary>
        /// <param name="channelType">channelType值可选0：手部（包含左手心、左手背、右手心、右手背）1：脚部（包含左脚、右脚）2：衣物</param>
        /// <param name="isEnabled">false:通道不启用，true：通道启用</param>
        /// <returns>设置成功/失败</returns>
        public bool SetEnabledByType(int channelType,bool isEnabled)
        {
            Channel channel = new Channel();
            channel.IsEnabled = isEnabled;
            channel.ChannelID = 0;
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@IsEnabled",OleDbType.Boolean),
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
                
            };
            parms[0].Value = channel.IsEnabled;
            parms[1].Value = channel.ChannelID;
            bool isSuccess = false;//是否成功
            int success = 0;//返回记录数
            switch (channelType)
            {
                case 0://手部（包含左手心、左手背、右手心、右手背）

                    
                    for(int i = 1; i < 5; i++)
                    {
                        channel.ChannelID = i;
                        
                        parms[1].Value = channel.ChannelID;//给通道ChannelID赋值
                        if (DbHelperAccess.ExecuteSql(SQL_UPDATE_CHANNEL_BY_CHANNELID, parms)!= 0)                    
                        {
                            success++;
                        }                    
                    }
                    if(success==4)
                    {
                        return isSuccess = true;
                    }
                    else
                    {
                        return isSuccess = false;
                    }
                    
                case 1://脚部（包含左脚、右脚）
                    success = 0;
                    for (int i = 5; i < 7; i++)
                    {
                        channel.ChannelID = i;
                        parms[1].Value = channel.ChannelID;//给通道ChannelID赋值
                        if (DbHelperAccess.ExecuteSql(SQL_UPDATE_CHANNEL_BY_CHANNELID, parms) != 0)
                        {
                            success++;
                        }
                    }
                    if (success == 2)
                    {
                        return isSuccess = true;
                    }
                    else
                    {
                        return isSuccess = false;
                    }
                case 2://衣物

                    channel.ChannelID = 7;
                    parms[1].Value = channel.ChannelID;//给通道ChannelID赋值
                    if (DbHelperAccess.ExecuteSql(SQL_UPDATE_CHANNEL_BY_CHANNELID, parms) != 0)
                    {
                        return isSuccess = true;
                    }
                    else
                    {
                        return isSuccess = false;
                    }
                default:
                    break;
            }
            return isSuccess;


        }
        #endregion

        #region 根据通道ID设置该通道的启用状态
        /// <summary>
        /// 根据通道ID设置该通道的启用状态
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <param name="isEnabled">false:通道不启用，true：通道启用</param>
        /// <returns></returns>
        public bool SetEnabledByID(int channelID,bool isEnabled)
        {
            Channel channel = new Channel();
            channel.IsEnabled = isEnabled;
            channel.ChannelID = channelID;
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@IsEnabled",OleDbType.Boolean),
                new OleDbParameter("@ChannelID",OleDbType.Integer)

            };
            parms[0].Value = channel.IsEnabled;
            parms[1].Value = channel.ChannelID;
            
            
                if (DbHelperAccess.ExecuteSql(SQL_UPDATE_CHANNEL_BY_CHANNELID, parms) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            
        }
        #endregion

        #region 根据通道ID设置探测面积
        /// <summary>
        /// 根据通道ID设置探测面积
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="ProbeArea"></param>
        /// <returns></returns>
        public bool SetProbeAreaByID(int channelID, float ProbeArea)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ProbeArea",OleDbType.Single),
                new OleDbParameter("@ChannelID",OleDbType.Integer)
            };
            parms[0].Value = ProbeArea;
            parms[1].Value = channelID;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_PROBEAREA_BY_CHANNELID, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}

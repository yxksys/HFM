/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：刻度类Calibration
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.OleDb;

namespace HFM.Components
{
    class Calibration
    {
        private const string SQL_SELECT_CALIBRATION = "SELECT CalibrationID,CalibrationTime,a.ChannelID,HighVoltage," +
                                                      "Threshold,Efficiency,MDA,AlphaBetaPercent," +
                                                      "ChannelName,ChannelName_English,ProbeArea,Status,IsEnabled " +
                                                      "FROM HFM_Calibration a " +
                                                      "INNER JOIN HFM_DIC_Channel b ON a.ChannelID=b.ChannelID";
        private const string SQL_SELECT_CALIBRATION_BY_CHANNELID = "SELECT CalibrationID,CalibrationTime,a.ChannelID,HighVoltage," +
                                                       "Threshold,Efficiency,MDA,AlphaBetaPercent," +
                                                       "ChannelName,ChannelName_English,ProbeArea,Status,IsEnabled " +
                                                       "FROM HFM_Calibration a " +
                                                       "INNER JOIN HFM_DIC_Channel b ON a.ChannelID=b.ChannelID " +
                                                       "WHERE a.ChannelID=@ChannelID";

        private const string SQL_INSERT_CALIBRATION = "INSERT INTO HFM_Calibration(CalibrationTime,ChannelID,HighVoltage," +
                                                      "Threshold,Efficiency,MDA,AlphaBetaPercent)" +
                                                      " VALUES(@CalibrationTime,@ChannelID,@HighVoltage,@Threshold,@Efficiency,@MDA,@AlphaBetaPercent)";

        private const string SQL_DELETE_CALIBRATION = "DELETE FROM HFM_Calibration";
        #region 构造函数
        public Calibration()
        { }
        public Calibration(DateTime calibrationTime, int channelID, float highVoltage, string threshold, float efficiency, float mDA, float alphaBetaPercent)
        {
            this._calibrationTime = calibrationTime;
            //根据
            this._channel = (new Channel()).GetChannel(channelID);
            this._highVoltage = highVoltage;
            this._threshold = threshold;
            this._efficiency = efficiency;
            this._mDA = mDA;
            this._alphaBetaPercent = alphaBetaPercent;
        }
        #endregion
        #region 属性

        private int _calibrationID;//刻度ID
        private DateTime _calibrationTime;//刻度时间
        private Channel _channel;//刻度通道
        private float _highVoltage;//高压值
        private string _threshold;//阈值
        private float _efficiency;//效率
        private float _mDA;//探测下限值
        private float _alphaBetaPercent;//串道比

        /// <summary>
        /// 刻度ID
        /// </summary>
        public int CalibrationID
        {
            get => _calibrationID;
            set => _calibrationID = value;
        }
        /// <summary>
        /// 刻度时间
        /// </summary>
        public DateTime CalibrationTime
        {
            get => _calibrationTime;
            set => _calibrationTime = value;
        }
        /// <summary>
        /// 高压值
        /// </summary>
        public float HighVoltage
        {
            get => _highVoltage;
            set => _highVoltage = value;
        }
        /// <summary>
        /// 阈值
        /// </summary>
        public string Threshold
        {
            get => _threshold;
            set => _threshold = value;
        }
        /// <summary>
        /// 效率
        /// </summary>
        public float Efficiency
        {
            get => _efficiency;
            set => _efficiency = value;
        }
        /// <summary>
        /// 探测下限值
        /// </summary>
        public float MDA
        {
            get => _mDA;
            set => _mDA = value;
        }
        /// <summary>
        /// 串道比
        /// </summary>
        public float AlphaBetaPercent
        {
            get => _alphaBetaPercent;
            set => _alphaBetaPercent = value;
        }
        internal Channel Channel
        {
            get => _channel;
            set => _channel = value;
        }
        #endregion
        /// <summary>
        /// 从数据库中查询全部刻度操作记录
        /// </summary>
        /// <returns></returns>
        public IList<Calibration> GetData()
        {
            IList<Calibration> ICalibrationS = new List<Calibration>();
            //从数据库中查询全部刻度操作记录并赋值给ICalibrationS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CALIBRATION))
            {
                while (reader.Read())//读查询结果
                {
                    //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                    Channel channel = new Channel(reader.GetInt32(0), reader["ChannelName"].ToString(), reader["ChannelName_English"].ToString(),
                                                  Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString()), reader["Status"].ToString(), reader.GetBoolean(12));
                    //根据读出的查询结构构造Calibration对象
                    Calibration calibraion = new Calibration();
                    calibraion.CalibrationID = Convert.ToInt32(reader["CalibrationID"].ToString());
                    calibraion.CalibrationTime = Convert.ToDateTime(reader["CalibrationTime"].ToString());
                    calibraion.Channel = channel;
                    calibraion.HighVoltage = Convert.ToSingle(reader["HighVoltage"].ToString());
                    calibraion.Threshold = (reader["Threshold"].ToString());
                    calibraion.Efficiency = Convert.ToSingle(reader["Efficiency"].ToString());
                    calibraion.MDA = Convert.ToSingle(reader["MDA"].ToString());
                    calibraion.AlphaBetaPercent = Convert.ToSingle(reader["AlphaBetaPercent"].ToString());
                    //从reader读出并构造的查询结果对象添加到List中
                    ICalibrationS.Add(calibraion);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return ICalibrationS;
        }
        /// <summary>
        /// 根据通道ID查询刻度信息
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <returns>该通道的刻度记录信息</returns>
        public IList<Calibration> GetData(int channelID)
        {
            IList<Calibration> ICalibrationS = new List<Calibration>();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
            };
            parms[0].Value = channelID;
            //从数据库中查询全部刻度操作记录并赋值给ICalibrationS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CALIBRATION_BY_CHANNELID, parms))
            {
                while (reader.Read())//读查询结果
                {
                    //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                    Channel channel = new Channel(reader.GetInt32(0), reader["ChannelName"].ToString(), reader["ChannelName_English"].ToString(),
                                                  Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString()), reader["Status"].ToString(), reader.GetBoolean(12));
                    //根据读出的查询结构构造Calibration对象
                    Calibration calibraion = new Calibration();
                    calibraion.CalibrationID = Convert.ToInt32(reader["CalibrationID"].ToString());
                    calibraion.CalibrationTime = Convert.ToDateTime(reader["CalibrationTime"].ToString());
                    calibraion.Channel = channel;
                    calibraion.HighVoltage = Convert.ToSingle(reader["HighVoltage"].ToString());
                    calibraion.Threshold = (reader["Threshold"].ToString());
                    calibraion.Efficiency = Convert.ToSingle(reader["Efficiency"].ToString());
                    calibraion.MDA = Convert.ToSingle(reader["MDA"].ToString());
                    calibraion.AlphaBetaPercent = Convert.ToSingle(reader["AlphaBetaPercent"].ToString());
                    //从reader读出并构造的查询结果对象添加到List中
                    ICalibrationS.Add(calibraion);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return ICalibrationS;
        }
        /// <summary>
        /// 向数据库中添加刻度信息
        /// </summary>
        /// <param name="calibration">要添加的刻度对象</param>
        /// <returns>是否添加成功</returns>
        public bool AddData(Calibration calibration)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@CalibrationTime",OleDbType.Date,8),
                new OleDbParameter("@ChannelID",OleDbType.Integer,4),
                new OleDbParameter("@HighVoltage",OleDbType.VarChar,255),
                new OleDbParameter("@Threshold",OleDbType.VarChar,25),
                new OleDbParameter("@Efficiency",OleDbType.VarChar,255),
                new OleDbParameter("@MDA",OleDbType.VarChar,255),
                new OleDbParameter("@AlphaBetaPercent",OleDbType.VarChar,255)
            };
            parms[0].Value = calibration.CalibrationTime;
            parms[1].Value = calibration.Channel.ChannelID;
            parms[2].Value = calibration.HighVoltage.ToString();
            parms[3].Value = calibration.Threshold.ToString();
            parms[4].Value = calibration.Efficiency.ToString();
            parms[5].Value = calibration.MDA.ToString();
            parms[6].Value = calibration.AlphaBetaPercent.ToString();
            if (DbHelperAccess.ExecuteSql(SQL_INSERT_CALIBRATION, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 删除表中数据信息
        /// </summary>
        /// <returns></returns>
        public int DeleteData()
        {
            return DbHelperAccess.ExecuteSql(SQL_DELETE_CALIBRATION);
        }
    }
}

﻿/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：2020/2/13
 *  类名：探测器类型警报及效率类
 *  
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
    class ProbeParameter
    {
        #region 常量
        private const string SQL_SELECT_PROBEPARAMETER = "SELECT PreferenceID,ProbeType,NuclideType,a.ChannelID," +
                                                         "HBackground,LBackground,Alarm_1,Alarm_2,Efficiency," +
                                                         "ChannelName,ChannelName_English,ProbeArea,Status,IsEnabled " +
                                                         "FROM HFM_Preference a" +
                                                         "INNER JOIN HFM_DIC_Channel b ON a.ChannelID=b.ChannelID";
        private const string SQL_SELECT_PROBEPARAMETER_BY_NUCLIDETYPE = "SELECT PreferenceID,ProbeType,NuclideType,a.ChannelID," +
                                                         "HBackground,LBackground,Alarm_1,Alarm_2,Efficiency," +
                                                         "ChannelName,ChannelName_English,ProbeArea,Status,IsEnabled " +
                                                         "FROM HFM_Preference a" +
                                                         "INNER JOIN HFM_DIC_Channel b ON a.ChannelID=b.ChannelID" +
                                                         "WHERE NuclideType = @NuclideType";
        private const string SQL_UPDATE_PROBEPARAMETER = "UPDATE HFM_Preference SET ProbeType = @ProbeType,NuclideType = @NuclideType," +
                                                         "ChannelID = @ChannelID,HBackground = @HBackground,LBackground = @LBackground," +
                                                         "Alarm_1 = @Alarm_1,Alarm_2 = @Alarm_2,Efficiency = @Efficiency WHERE PreferenceID = @PreferenceID";

        #endregion

        #region 属性

        private int _preferenceID;//探测参数编号
        private string _probeType;//探测器类型
        private string _nuclideType;//核素类型可选值α、β、C
        private Channel _probeChannel;//探测通道
        private float _hBackground;//本底上限
        private float _lBackground;//本底下限
        private float _alarm_1;//一级警报
        private float _alarm_2;//二级警报
        private float _efficiency;//当前选择探测效率

        /// <summary>
        /// 探测参数编号
        /// </summary>
        public int PreferenceID { get => _preferenceID; set => _preferenceID = value; }
        /// <summary>
        /// 探测器类型
        /// </summary>
        public string ProbeType { get => _probeType; set => _probeType = value; }
        /// <summary>
        /// 核素类型可选值α、β、C
        /// </summary>
        public string NuclideType { get => _nuclideType; set => _nuclideType = value; }
        /// <summary>
        /// 本底上限
        /// </summary>
        public float HBackground { get => _hBackground; set => _hBackground = value; }
        /// <summary>
        /// 本底下限
        /// </summary>
        public float LBackground { get => _lBackground; set => _lBackground = value; }
        /// <summary>
        /// 一级警报
        /// </summary>
        public float Alarm_1 { get => _alarm_1; set => _alarm_1 = value; }
        /// <summary>
        /// 二级警报
        /// </summary>
        public float Alarm_2 { get => _alarm_2; set => _alarm_2 = value; }
        /// <summary>
        /// 当前选择探测效率
        /// </summary>
        public float Efficiency { get => _efficiency; set => _efficiency = value; }
        /// <summary>
        /// 探测通道
        /// </summary>
        internal Channel ProbeChannel { get => _probeChannel; set => _probeChannel = value; }
        #endregion

        #region 方法

        #region 获得数据
        /// <summary>
        /// 获得所有探测参数
        /// </summary>
        /// <returns></returns>
        public IList<ProbeParameter> GetParameter()
        {
            IList<ProbeParameter> ICalibrationS = new List<ProbeParameter>();
            //从数据库中查询全部刻度操作记录并赋值给ICalibrationS
            OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_PROBEPARAMETER);
            while (reader.Read())//读查询结果
            {
                //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                Channel channel = new Channel(reader.GetInt32(0), reader["ChannelName"].ToString(), reader["ChannelName_English"].ToString(),
                                              Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString()),
                                              reader["Status"].ToString(), reader.GetBoolean(12));
                //根据读出的查询结构构造ProbeParameter对象
                ProbeParameter probeParameter = new ProbeParameter();
                probeParameter.PreferenceID = Convert.ToInt32(reader["Preference"].ToString());
                probeParameter.ProbeType = Convert.ToString(reader["ProbeType"].ToString());
                probeParameter.NuclideType = Convert.ToString(reader["NuclideType"].ToString());
                probeParameter.ProbeChannel = channel;
                probeParameter.HBackground = Convert.ToSingle(reader["HBackground"].ToString());
                probeParameter.LBackground = Convert.ToSingle(reader["LBackground"].ToString());
                probeParameter.Alarm_1 = Convert.ToSingle(reader["Alarm_1"].ToString());
                probeParameter.Alarm_2 = Convert.ToSingle(reader["Alarm_2"].ToString());
                probeParameter.Efficiency = Convert.ToSingle(reader["Efficiency"]);
                //从reader读出并构造的查询结果对象添加到List中
                ICalibrationS.Add(probeParameter);
            }
            return ICalibrationS;
        }
        /// <summary>
        /// 根据核素类型（α、β、C）查询其探测参数
        /// </summary>
        /// <param name="nuclideType"></param>
        /// <returns></returns>
        public IList<ProbeParameter> GetParameter(string nuclideType)
        {
            IList<ProbeParameter> ICalibrationS = new List<ProbeParameter>();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@NuclideType",OleDbType.VarChar,255)
            };
            parms[0].Value = nuclideType;
            //从数据库中查询全部刻度操作记录并赋值给ICalibrationS
            OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_PROBEPARAMETER_BY_NUCLIDETYPE, parms);
            while (reader.Read())//读查询结果
            {
                //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                Channel channel = new Channel(reader.GetInt32(0), reader["ChannelName"].ToString(), reader["ChannelName_English"].ToString(),
                                              Convert.ToSingle(reader["ProbeArea"].ToString() == "" ? "0" : reader["ProbeArea"].ToString()),
                                              reader["Status"].ToString(), reader.GetBoolean(12));
                //根据读出的查询结构构造ProbeParameter对象
                ProbeParameter probeParameter = new ProbeParameter();
                probeParameter.PreferenceID = Convert.ToInt32(reader["Preference"].ToString());
                probeParameter.ProbeType = Convert.ToString(reader["ProbeType"].ToString());
                probeParameter.NuclideType = Convert.ToString(reader["NuclideType"].ToString());
                probeParameter.ProbeChannel = channel;
                probeParameter.HBackground = Convert.ToSingle(reader["HBackground"].ToString());
                probeParameter.LBackground = Convert.ToSingle(reader["LBackground"].ToString());
                probeParameter.Alarm_1 = Convert.ToSingle(reader["Alarm_1"].ToString());
                probeParameter.Alarm_2 = Convert.ToSingle(reader["Alarm_2"].ToString());
                probeParameter.Efficiency = Convert.ToSingle(reader["Efficiency"]);
                //从reader读出并构造的查询结果对象添加到List中
                ICalibrationS.Add(probeParameter);
            }
            return ICalibrationS;
        }
        #endregion

        #region 更新数据
        /// <summary>
        /// 根据参数对象probeParameter的核素类型NuclideType和探测通道ProbeChannel
        /// 更新其HBackground、LBackground、Alarm_1、Alarm_2、Efficiency参数。
        /// 注意：当更新了本表中的Efficiency参数时，
        /// 同时应通过调用EfficiencyParameter对象SetParameter(EfficiencyParameter efficiencyParameter)
        /// 方法更新探测效率参数表HFM_EfficiencyParameter中对应的效率参数
        /// </summary>
        /// <param name="probeParameter"></param>
        /// <returns></returns>
        public bool SetParameter(ProbeParameter probeParameter)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ProbeType",OleDbType.VarChar,255),
                new OleDbParameter("@NuclideType",OleDbType.VarChar,255),
                new OleDbParameter("@ChannelID",OleDbType.Integer,4),
                new OleDbParameter("@HBackground",OleDbType.VarChar,255),
                new OleDbParameter("@LBackground",OleDbType.VarChar,255),
                new OleDbParameter("@Alarm_1",OleDbType.VarChar,255),
                new OleDbParameter("@Alarm_2",OleDbType.VarChar,255),
                new OleDbParameter("@Efficiency",OleDbType.VarChar,255)
            };
            parms[0].Value = probeParameter.ProbeType.ToString();
            parms[1].Value = probeParameter.NuclideType.ToString();
            parms[2].Value = probeParameter.ProbeChannel.ChannelID;
            parms[3].Value = probeParameter.HBackground.ToString();
            parms[4].Value = probeParameter.LBackground.ToString();
            parms[5].Value = probeParameter.Alarm_1.ToString();
            parms[6].Value = probeParameter.Alarm_2.ToString();
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_PROBEPARAMETER, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        
        #endregion
    }
}

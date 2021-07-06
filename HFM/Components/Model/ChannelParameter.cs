/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：
 *  类名：道盒类 ChannelParameter
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
    class ChannelParameter
    {
        #region 常量
        private const string SQL_SELECT_CHANNELPARAMETER = "SELECT CheckingID,AlphaThreshold,BetaThreshold,PresetHV,ADCFactor," +
                                                           "DACFactor,HVFactor,HVRatio,WorkTime,a.ChannelID,ChannelName,ChannelName_English," +
                                                           "ProbeArea,Status,IsEnabled FROM HFM_ChannelParameter a INNER JOIN" +
                                                           " HFM_DIC_Channel b ON a.ChannelID = b.ChannelID";
        private const string SQL_UPDATE_CHANNELPARAMETER_BY_CHANNELID = "UPDATE HFM_ChannelParameter SET AlphaThreshold = @AlphaThreshold," +
                                                           "BetaThreshold = @BetaThreshold,PresetHV = @PresetHV,ADCFactor = @ADCFactor," +
                                                           "DACFactor = @DACFactor,HVFactor = @HVFactor,HVRatio = @HVRatio,WorkTime = @WorkTime " +
                                                           "WHERE ChannelID = @ChannelID";
        private const string SQL_SELECT_CHANNELPARAMETER_BY_CHANNELID = "SELECT AlphaThreshold,BetaThreshold,PresetHV,ADCFactor,DACFactor," +
                                                           "HVFactor,HVRatio,WorkTime,CheckingID FROM HFM_ChannelParameter WHERE ChannelID = @ChannelID";
        #endregion

        #region 属性
        private int _checkingID;// 道盒ID
        private Channel _channel;// 所属通道
        private float _alphaThreshold;// Alpha阈值
        private float _betaThreshold;// Beta阈值
        private float _presetHV;// 高压值
        private float _aDCFactor;// AD因子
        private float _dACFactor;// DA因子
        private float _hVFactor;// 高压因子
        private float _hVRatio;// 高压倍数
        private float _workTime;// 工作时间


        /// <summary>
        /// 道盒ID
        /// </summary>
        public int CheckingID { get => _checkingID; set => _checkingID = value; }
        /// <summary>
        /// Alpha阈值
        /// </summary>
        public float AlphaThreshold { get => _alphaThreshold; set => _alphaThreshold = value; }
        /// <summary>
        /// Beta阈值
        /// </summary>
        public float BetaThreshold { get => _betaThreshold; set => _betaThreshold = value; }
        /// <summary>
        /// 高压值
        /// </summary>
        public float PresetHV { get => _presetHV; set => _presetHV = value; }
        /// <summary>
        /// AD因子
        /// </summary>
        public float ADCFactor { get => _aDCFactor; set => _aDCFactor = value; }
        /// <summary>
        /// DA因子
        /// </summary>
        public float DACFactor { get => _dACFactor; set => _dACFactor = value; }
        /// <summary>
        /// 高压因子
        /// </summary>
        public float HVFactor { get => _hVFactor; set => _hVFactor = value; }
        /// <summary>
        /// 高压倍数
        /// </summary>
        public float HVRatio { get => _hVRatio; set => _hVRatio = value; }
        /// <summary>
        /// 工作时间
        /// </summary>
        public float WorkTime { get => _workTime; set => _workTime = value; }
        /// <summary>
        /// 所属通道
        /// </summary>
        internal Channel Channel { get => _channel; set => _channel = value; }

        public string ChannelName
        {
            get { return _channel.ChannelName; }
        }
        #endregion

        #region 构造函数
        public ChannelParameter()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="_checkingID"></param>
        /// <param name="_channel"></param>
        /// <param name="_alphaThreshold"></param>
        /// <param name="_betaThreshold"></param>
        /// <param name="_presetHV"></param>
        /// <param name="_aDCFactor"></param>
        /// <param name="_dACFactor"></param>
        /// <param name="_hVFactor"></param>
        /// <param name="_hVRatio"></param>
        /// <param name="_workTime"></param>
        public ChannelParameter(int _checkingID, float _alphaThreshold, float _betaThreshold, float _presetHV, float _aDCFactor, float _dACFactor, float _hVFactor, float _workTime, float _hVRatio )
        {
            this._checkingID = _checkingID;
            this._channel = (new Channel()).GetChannel(_checkingID);
            this._alphaThreshold = _alphaThreshold;
            this._betaThreshold = _betaThreshold;
            this._presetHV = _presetHV;
            this._aDCFactor = _aDCFactor;
            this._dACFactor = _dACFactor;
            this._hVFactor = _hVFactor;
            this._workTime = _workTime;
            this._hVRatio = _hVRatio;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获得全部道盒参数
        /// </summary>
        /// <returns></returns>
        public IList<ChannelParameter> GetParameter()
        {
            IList<ChannelParameter> ICalibrationS = new List<ChannelParameter>();
            //从数据库中查询道盒数据并赋值给ICalibrationS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CHANNELPARAMETER))
            {
                while (reader.Read())//读查询结果
                {
                    //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                    //解决ProbeArea类型转换问题，若为空则不能直接转换为float
                    string ProbeArea = Convert.ToString(reader["ProbeArea"]);
                    float probeArea;
                    if (ProbeArea == "")
                    {
                        probeArea = 0.0f;
                    }
                    else
                    {
                        probeArea = float.Parse(ProbeArea);
                    }
                    Channel channel = new Channel(Convert.ToInt32(reader["ChannelID"]), Convert.ToString(reader["ChannelName"]),
                                                   Convert.ToString(reader["ChannelName_English"]), probeArea,
                                                   Convert.ToString(reader["Status"]), Convert.ToBoolean(reader["IsEnabled"]));
                    //根据读出的查询结构构造ChannelParameter对象
                    ChannelParameter channelParameter = new ChannelParameter();
                    channelParameter.CheckingID = Convert.ToInt32(reader["CheckingID"]);
                    channelParameter.AlphaThreshold = Convert.ToSingle(reader["AlphaThreshold"]);
                    channelParameter.BetaThreshold = Convert.ToSingle(reader["BetaThreshold"]);
                    channelParameter.PresetHV = Convert.ToSingle(reader["PresetHV"]);
                    channelParameter.ADCFactor = Convert.ToSingle(reader["ADCFactor"]);
                    channelParameter.DACFactor = Convert.ToSingle(reader["DACFactor"]);
                    channelParameter.HVFactor = Convert.ToSingle(reader["HVFactor"]);
                    channelParameter.HVRatio = Convert.ToSingle(reader["HVRatio"]);
                    channelParameter.WorkTime = Convert.ToSingle(reader["WorkTime"]);
                    channelParameter.Channel = channel;
                    ICalibrationS.Add(channelParameter);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return ICalibrationS;

        }
        /// <summary>
        /// 根据通道ID查询该通道的道盒参数
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <returns>道盒参数</returns>
        public ChannelParameter GetParameter(int channelID)
        {
            ChannelParameter channelParameter = new ChannelParameter();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
            };
            parms[0].Value = channelID;

            //从数据库中查询全部刻度操作记录并赋值给channelParameter
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_CHANNELPARAMETER_BY_CHANNELID, parms))
            {
                while (reader.Read())//读查询结果
                {
                    //根据读出的查询结构构造channelParameter对象

                    this.AlphaThreshold = Convert.ToSingle(reader["AlphaThreshold"]);
                    this.BetaThreshold = Convert.ToSingle(reader["BetaThreshold"]);
                    this.PresetHV = Convert.ToSingle(reader["PresetHV"]);
                    this.ADCFactor = Convert.ToSingle(reader["ADCFactor"]);
                    this.DACFactor = Convert.ToSingle(reader["DACFactor"]);
                    this.HVFactor = Convert.ToSingle(reader["HVFactor"]);
                    this.HVRatio = Convert.ToSingle(reader["HVRatio"]);
                    this.WorkTime = Convert.ToSingle(reader["HVRatio"]);
                    this.CheckingID = Convert.ToInt32(reader["CheckingID"]);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            this.Channel = new Channel().GetChannel(channelID);//根据channelID获得channel信息
            return this;

        }
        /// <summary>
        /// 根据参数对象channelParameter的通道ID，设置道盒参数
        /// </summary>
        /// <param name="channelParameter"></param>
        /// <returns></returns>
        public bool SetParameter(ChannelParameter channelParameter)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@AlphaThreshold",OleDbType.VarChar,255),
                new OleDbParameter("@BetaThreshold",OleDbType.VarChar,255),
                new OleDbParameter("@PresetHV",OleDbType.VarChar,255),
                new OleDbParameter("@ADCFactor",OleDbType.VarChar,255),
                new OleDbParameter("@DACFactor",OleDbType.VarChar,255),
                new OleDbParameter("@HVFactor",OleDbType.VarChar,255),
                new OleDbParameter("@HVRatio",OleDbType.VarChar,255),
                new OleDbParameter("@WorkTime",OleDbType.VarChar,255),
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
            };
            parms[0].Value = channelParameter.AlphaThreshold.ToString();
            parms[1].Value = channelParameter.BetaThreshold.ToString();
            parms[2].Value = channelParameter.PresetHV.ToString();
            parms[3].Value = channelParameter.ADCFactor.ToString();
            parms[4].Value = channelParameter.DACFactor.ToString();
            parms[5].Value = channelParameter.HVFactor.ToString();
            parms[6].Value = channelParameter.HVRatio.ToString();
            parms[7].Value = channelParameter.WorkTime.ToString();
            parms[8].Value = channelParameter.Channel.ChannelID;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_CHANNELPARAMETER_BY_CHANNELID, parms) != 0)
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

/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：2020/2/14
 *  类名：探测效率参数类 EfficiencyParameter
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
    class EfficiencyParameter
    {

        private const string SQL_SELECT_EFFICIENCYPARAMETER = "SELECT EfficiencyParamID,Efficiency,a.ChannelID,NuclideType," +
                                                              "NuclideName,ChannelName,ChannelName_English,ProbeArea,Status," +
                                                              "IsEnabled FROM HFM_EfficiencyParameter a " +
                                                              "INNER JOIN HFM_DIC_Channel b ON a.ChannelID = b.ChannelID";
        private const string SQL_SELECT_EFFICIENCY_BY_NUCLIDETYPE_AND_NUCLIDENAME = "SELECT EfficiencyParamID,a.ChannelID," +
                                                              "Efficiency,NuclideType,NuclideName,ChannelName,ChannelName_English," +
                                                              "ProbeArea,Status,IsEnabled FROM HFM_EfficiencyParameter a, " +
                                                              "HFM_DIC_Channel b WHERE NuclideType = @NuclideType AND " +
                                                              "NuclideName = @NuclideName AND a.ChannelID = b.ChannelID";
        private const string SQL_SELECT_EFFICIENCY_BY_NUCLIDETYPE_AND_CHANNEL_AND_NUCLIDENAME = "SELECT EfficiencyParamID,a.ChannelID,Efficiency, NuclideType," +
                                                              "NuclideName,ChannelName,ChannelName_English,ProbeArea,Status,IsEnabled FROM HFM_EfficiencyParameter" +
                                                              " a,HFM_DIC_Channel b WHERE NuclideType = @NuclideType AND NuclideName = @NuclideName " +
                                                              " AND a.ChannelID = @ChannelID AND a.ChannelID = b.ChannelID";
        private const string SQL_UPDATE_EFFICIENCY_BY_NUCLIDETYPE_AND_NUCLIDENAME_AND_CHANNELID = "UPDATE HFM_EfficiencyParameter" +
                                                              " SET Efficiency = @Efficiency WHERE NuclideType = @NuclideType AND " +
                                                              "NuclideName = @NuclideName AND ChannelID = @ChannelID";

        #region 属性
        private int _efficiencyParamID;//核素参数编号
        private Channel _channel;//通道
        private string _nuclideType;//核素类型可选值为：α、β、C
        private string _nuclideName;//核素名称：U_235、Pu_238、Pu_239
        private float _efficiency;//效率值
        /// <summary>
        /// EfficiencyParamID
        /// </summary>
        public int EfficiencyParamID { get => _efficiencyParamID; set => _efficiencyParamID = value; }
        /// <summary>
        /// 核素类型可选值为：α、β、C
        /// </summary>
        public string NuclideType { get => _nuclideType; set => _nuclideType = value; }
        /// <summary>
        /// 核素名称：U_235、Pu_238、Pu_239
        /// </summary>
        public string NuclideName { get => _nuclideName; set => _nuclideName = value; }
        /// <summary>
        /// 效率值
        /// </summary>
        public float Efficiency { get => _efficiency; set => _efficiency = value; }
        /// <summary>
        /// 通道
        /// </summary>
        internal Channel Channel { get => _channel; set => _channel = value; }

        #endregion

        #region 构造函数
        public EfficiencyParameter()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="_efficiencyParamID"></param>
        /// <param name="_channel"></param>
        /// <param name="_nuclideType"></param>
        /// <param name="_nuclideNama"></param>
        /// <param name="_efficiency"></param>
        public EfficiencyParameter(int _efficiencyParamID, Channel _channel, string _nuclideType,
                                   string _nuclideNama, float _efficiency)
        {
            this._efficiencyParamID = _efficiencyParamID;
            this._channel = _channel;
            this._nuclideType = _nuclideType;
            this._nuclideName = _nuclideNama;
            this._efficiency = _efficiency;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获得所有效率参数
        /// </summary>
        /// <returns></returns>
        public IList<EfficiencyParameter> GetParameter()
        {
            IList<EfficiencyParameter> ICalibrationS = new List<EfficiencyParameter>();
            //从数据库中查询全部核素并赋值给ICalibrationS
            OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_EFFICIENCYPARAMETER);
            while(reader.Read())//读查询结果
            {
                //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                Channel channel = new Channel(Convert.ToInt32(reader["ChannelID"]), Convert.ToString(reader["ChannelName"]),
                                               Convert.ToString(reader["ChannelName_English"]), Convert.ToSingle(reader["ProbeArea"]),
                                               Convert.ToString(reader["Status"]), Convert.ToBoolean(reader["IsEnabled"]));

                //根据读出的查询结构构造EfficiencyParameter对象
                EfficiencyParameter efficiencyParameter = new EfficiencyParameter();
                efficiencyParameter.EfficiencyParamID = Convert.ToInt32(reader["EfficiencyParamID"].ToString());
                efficiencyParameter.Channel = channel;
                efficiencyParameter.Efficiency = Convert.ToSingle(reader["Efficiency"].ToString());
                efficiencyParameter.NuclideType = Convert.ToString(reader["NuclideType"].ToString());
                efficiencyParameter.NuclideName = Convert.ToString(reader["NuclideName"].ToString());
                //从reader读出并构造的查询结果对象添加到List中
                ICalibrationS.Add(efficiencyParameter);
            }
            return ICalibrationS;
        }
        /// <summary>
        /// 根据核素类型和核素名称查询效率参数
        /// </summary>
        /// <param name="nuclideType"></param>
        /// <param name="nuclideName"></param>
        /// <returns></returns>
        public IList<EfficiencyParameter> GetParameter(string nuclideType,string nuclideName)
        {
            IList<EfficiencyParameter> ICalibrationS = new List<EfficiencyParameter>();
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@NuclideType",OleDbType.VarChar,255),
                new OleDbParameter("@NuclideName",OleDbType.VarChar,255)
            };
            parms[0].Value = nuclideType;
            parms[1].Value = nuclideName;
            //从数据库中查询全部核素效率并赋值给ICalibrationS
            OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_EFFICIENCY_BY_NUCLIDETYPE_AND_NUCLIDENAME, parms);
            while (reader.Read())//读查询结果
            {
                //根据查询结果即ChannelID对应的Channel信息，构造Channel对象
                Channel channel = new Channel(Convert.ToInt32(reader["ChannelID"]), Convert.ToString(reader["ChannelName"]),
                                               Convert.ToString(reader["ChannelName_English"]), Convert.ToSingle(reader["ProbeArea"]),
                                               Convert.ToString(reader["Status"]), Convert.ToBoolean(reader["IsEnabled"]));
                //根据读出的查询结构构造EffciencyParameter对象
                EfficiencyParameter efficiencyParameter = new EfficiencyParameter();
                efficiencyParameter.EfficiencyParamID = Convert.ToInt32(reader["EfficiencyParamID"].ToString());
                efficiencyParameter.Channel = channel;
                efficiencyParameter.Efficiency = Convert.ToSingle(reader["Efficiency"].ToString());
                efficiencyParameter.NuclideType = Convert.ToString(reader["NuclideType"].ToString());
                efficiencyParameter.NuclideName = Convert.ToString(reader["NuclideName"].ToString());
                //从reader读出并构造的查询结果对象添加到List中
                ICalibrationS.Add(efficiencyParameter);
            }
            return ICalibrationS;
        }
        /// <summary>
        /// 根据核素类型、通道和核素名称查询效率参数
        /// </summary>
        /// <param name="nuclideType"></param>
        /// <param name="channelID"></param>
        /// <param name="nuclideName"></param>
        /// <returns></returns>
        public EfficiencyParameter GetParameter(string nuclideType,string nuclideName ,int channelID)
        {
            EfficiencyParameter efficiencyParameter = new EfficiencyParameter();
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@NuclideType",OleDbType.VarChar,255),
                new OleDbParameter("@NuclideName",OleDbType.VarChar,255),
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
            };
            parms[0].Value = nuclideType;
            parms[1].Value = nuclideName;
            parms[2].Value = channelID;
            //从数据库中查询全部核素效率并赋值给ICalibrationS
            OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_EFFICIENCY_BY_NUCLIDETYPE_AND_CHANNEL_AND_NUCLIDENAME, parms);
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
                //根据读出的查询结构构造EffciencyParameter对象
                efficiencyParameter.EfficiencyParamID = Convert.ToInt32(reader["EfficiencyParamID"].ToString());
                efficiencyParameter.Channel = channel;
                efficiencyParameter.Efficiency = Convert.ToSingle(reader["Efficiency"].ToString());
                efficiencyParameter.NuclideType = Convert.ToString(reader["NuclideType"].ToString());
                efficiencyParameter.NuclideName = Convert.ToString(reader["NuclideName"].ToString());
            }
            return efficiencyParameter;
        }
        /// <summary>
        /// 根据参数对象efficiencyParameter的ChannelID、NuclideType，NuclideName更新其Efficiency值
        /// </summary>
        /// <param name="efficiencyParameter"></param>
        /// <returns></returns>
        public bool SetParameter(EfficiencyParameter efficiencyParameter)
        {

            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@Efficiency",OleDbType.VarChar,255),
                new OleDbParameter("@NuclideType",OleDbType.VarChar,255),
                new OleDbParameter("@NuclideName",OleDbType.VarChar,255),
                new OleDbParameter("@ChannelID",OleDbType.Integer,4)
            };
            parms[0].Value = efficiencyParameter.Efficiency.ToString();
            parms[1].Value = efficiencyParameter.NuclideType.ToString();
            parms[2].Value = efficiencyParameter.NuclideName.ToString();
            parms[3].Value = efficiencyParameter.Channel.ChannelID;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_EFFICIENCY_BY_NUCLIDETYPE_AND_NUCLIDENAME_AND_CHANNELID, parms) != 0)
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

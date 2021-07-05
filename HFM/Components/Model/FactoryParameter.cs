/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁 杨旭锴
 *  版本：
 *  创建时间：2020/2/14 修改：2020年2月17日 09:40:34
 *  类名：工厂参数类  FactoryParmeter
 *  更新：杨旭锴，2020年3月7日，新增字段“DeviceAddress”=“设备地址”，方法相应修改
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
    public class FactoryParameter
    {
        #region 数据库查询语句
        /// <summary>
        /// 查询字段：仪器编号、软件名称、IP地址、通信端口、是否自动连接、探测类型、平滑因子、手部是否双探测器
        /// </summary>
        private const string SQL_SELECT_MAINPREFERENCE = "SELECT   InstrumentNum, SoftName, IPAddress, PortNumber," +
                                                        " IsConnectedAuto, MeasureType, SmoothingFactor, IsDoubleProbe, DeviceAddress,ReportingTime,IsFootInfrared,IsFriskerIndependent " +
                                                        " FROM HFM_MainPreference";
        /// <summary>
        /// 更新字段：仪器编号、软件名称、IP地址、通信端口、是否自动连接、探测类型、平滑因子、手部是否双探测器
        /// </summary>
        private const string SQL_UPDATE_MAINPREFERENCE = "UPDATE HFM_MainPreference " +
                                                        "SET  InstrumentNum=@InstrumentNum, SoftName=@SoftName," +
                                                        " IPAddress=@IPAddress, PortNumber=@PortNumber, IsConnectedAuto=@IsConnectedAuto," +
                                                        " MeasureType=@MeasureType, SmoothingFactor=@SmoothingFactor, IsDoubleProbe=@IsDoubleProbe," +
                                                        "DeviceAddress=@DeviceAddress,ReportingTime=@ReportingTime,IsFootInfrared=@IsFootInfrared,IsFriskerIndependent=@IsFriskerIndependent";
        #endregion

        #region 属性
        private string _instrumentNum;//仪器编号
        private string _softName;//软件名称
        private string _ipAddress;//IP地址
        private string _portNumber;//通信端口
        private bool _isConnectedAuto;//是否自动连接
        private string _measureType;//探测类型
        private float _smoothingFactor;//平滑因子
        private bool _isDoubleProbe;//手部是否双探测器
        private string _deviceAddress;//设备地址
        private string _reportingTime;//上报时间间隔
        private bool _isFootInfrared=false;//脚步红外是否独立
        private bool _isFriskerIndependent = true;//衣物探头是否独立

        /// <summary>
        /// 仪器编号
        /// </summary>
        public string InstrumentNum { get => _instrumentNum; set => _instrumentNum = value; }
        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftName { get => _softName; set => _softName = value; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get => _ipAddress; set => _ipAddress = value; }
        /// <summary>
        /// 通信端口
        /// </summary>
        public string PortNumber { get => _portNumber; set => _portNumber = value; }
        /// <summary>
        /// 是否自动连接
        /// </summary>
        public bool IsConnectedAuto { get => _isConnectedAuto; set => _isConnectedAuto = value; }
        /// <summary>
        /// 探测类型
        /// </summary>
        public string MeasureType { get => _measureType; set => _measureType = value; }
        /// <summary>
        /// 平滑因子
        /// </summary>
        public float SmoothingFactor { get => _smoothingFactor; set => _smoothingFactor = value; }
        /// <summary>
        /// 手部是否双探测器
        /// </summary>
        public bool IsDoubleProbe { get => _isDoubleProbe; set => _isDoubleProbe = value; }
        /// <summary>
        /// 设备地址
        /// </summary>
        public string DeviceAddress
        {
            get => _deviceAddress;
            set => _deviceAddress = value;
        }
        /// <summary>
        /// 上报时间间隔
        /// </summary>
        public string ReportingTime { get=>_reportingTime; set=>_reportingTime=value; }
        /// <summary>
        /// 脚步红外是否独立
        /// </summary>
        public bool IsFootInfrared { get => _isFootInfrared; set => _isFootInfrared = value; }
        /// <summary>
        /// 衣物探头是否独立
        /// </summary>
        public bool IsFriskerIndependent { get => _isFriskerIndependent; set => _isFriskerIndependent = value; }
        #endregion
        #region 构造函数
        public FactoryParameter()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="instrumentNum">仪器编号</param>
        /// <param name="softName">软件名称</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="portNumber">通信端口</param>
        /// <param name="isConnectedAuto">是否自动连接：bool值</param>
        /// <param name="measureType">探测类型</param>
        /// <param name="smoothingFactor">平滑因子</param>
        /// <param name="isDoubleProbe">手部是否双探测器：bool值</param>
        /// <param name="deviceAddress">设备地址</param>
        public FactoryParameter(string instrumentNum, string softName, string ipAddress, string portNumber, bool isConnectedAuto, string measureType, float smoothingFactor, bool isDoubleProbe,string deviceAddress)
        {
            this._instrumentNum = instrumentNum;
            this._softName = softName;
            this._ipAddress = ipAddress;
            this._portNumber = portNumber;
            this._isConnectedAuto = isConnectedAuto;
            this._measureType = measureType;
            this._smoothingFactor = smoothingFactor;
            this._isDoubleProbe = isDoubleProbe;
            this.DeviceAddress = deviceAddress;
        }
        #endregion
        #region 方法
        /// <summary>
        /// 从数据库中查询当前工厂参数
        /// </summary>
        /// <returns>返回工厂参数对象</returns>
        public FactoryParameter GetParameter()
        {
            //FactoryParameter factoryParameter = new FactoryParameter();
            //查询表部分字段信息
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_MAINPREFERENCE))
            {
                while (reader.Read())
                {
                    this.InstrumentNum = Convert.ToString(reader["InstrumentNum"].ToString());
                    this.SoftName = Convert.ToString(reader["SoftName"].ToString());
                    this.IpAddress = Convert.ToString(reader["IPAddress"].ToString());
                    this.PortNumber = Convert.ToString(reader["PortNumber"].ToString());
                    this.IsConnectedAuto = Convert.ToBoolean(reader["IsConnectedAuto"].ToString());
                    this.MeasureType = Convert.ToString(reader["MeasureType"].ToString());
                    this.SmoothingFactor = Convert.ToSingle(reader["SmoothingFactor"].ToString() == "" ? "0" : reader["SmoothingFactor"].ToString());
                    this.IsDoubleProbe = Convert.ToBoolean(reader["IsDoubleProbe"].ToString());
                    this.DeviceAddress = Convert.ToString(reader["DeviceAddress"].ToString());
                    this.ReportingTime = Convert.ToString(reader["ReportingTime"].ToString());
                    this.IsFootInfrared = Convert.ToBoolean(reader["IsFootInfrared"]);
                    this.IsFriskerIndependent = Convert.ToBoolean(reader["IsFriskerIndependent"]);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return this;
        }
        /// <summary>
        /// 更新数据库中工厂设置
        /// </summary>
        /// <param name="factoryParameter"></param>
        /// <returns>true更新成功；false更新失败</returns>
        public bool SetParameter(FactoryParameter factoryParameter)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("InstrumentNum",OleDbType.VarChar,255),
                new OleDbParameter("SoftName",OleDbType.VarChar,255),
                new OleDbParameter("IPAddress",OleDbType.VarChar,255),
                new OleDbParameter("PortNumber",OleDbType.VarChar,255),
                new OleDbParameter("IsConnectedAuto",OleDbType.Boolean),
                new OleDbParameter("MeasureType",OleDbType.VarChar,255),
                new OleDbParameter("SmoothingFactor",OleDbType.VarChar,255),
                new OleDbParameter("IsDoubleProbe",OleDbType.Boolean),
                new OleDbParameter("DeviceAddress",OleDbType.VarChar,255),
                new OleDbParameter("ReportingTime",OleDbType.VarChar,255),//,
                new OleDbParameter("IsFootInfrared",OleDbType.Boolean),
                new OleDbParameter("IsFriskerIndependent",OleDbType.Boolean)
            };
            parms[0].Value = factoryParameter.InstrumentNum.ToString();
            parms[1].Value = factoryParameter.SoftName.ToString();
            parms[2].Value = factoryParameter.IpAddress.ToString();
            parms[3].Value = factoryParameter.PortNumber.ToString();
            parms[4].Value = factoryParameter.IsConnectedAuto;
            parms[5].Value = factoryParameter.MeasureType.ToString();
            parms[6].Value = factoryParameter.SmoothingFactor.ToString();
            parms[7].Value = factoryParameter.IsDoubleProbe;
            parms[8].Value = factoryParameter.DeviceAddress.ToString();
            parms[9].Value = factoryParameter.ReportingTime.ToString();
            parms[10].Value = factoryParameter.IsFootInfrared;
            parms[11].Value = factoryParameter.IsFriskerIndependent;
            //执行更新语句
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_MAINPREFERENCE,parms) != 0)
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

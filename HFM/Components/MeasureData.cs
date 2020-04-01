/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：白茹
 *  版本：
 *  创建时间：2020年2月17日
 *  类名：监测数据类
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
    class MeasureData
    {
        #region 数据库查询语句
        /// <summary>
        /// 查询所有监测数据
        /// </summary>
        private const string SQL_SELECT_MEASUREDATA = "SELECT MeasureID,MeasureDate,MeasureStatus,DetailedInfo,IsEnglish,IsReported" +
                                                     "FROM HFM_MeasureData";
        /// <summary>
        /// （2）按照语言查询监测数据
        /// </summary>
        private const string SQL_SELECT_MEASUREDATA_BY_ISENGLISH= "SELECT MeasureID,MeasureDate,MeasureStatus,DetailedInfo,IsEnglish,IsReported" +
                                                     "FROM HFM_MeasureData WHERE IsEnglish=@IsEnglish";
        /// <summary>
        /// 添加监测数据
        /// </summary>
        private const string SQL_INSERT_MEASUREDATA = "INSERT INTO HFM_MeasureData(MeasureDate,MeasureStatus,DetailedInfo,IsEnglish,IsReported)" +
                                                     "VALUES(@MeasureDate,@MeasureStatus,@DetailedInfo,@IsEnglish)";
        /// <summary>
        /// 查询最新一条监测记录
        /// </summary>
        private const string SQL_SELECT_MEASUREDATA_BY_NEWRECORD = "SELECT MAX(MeasureID),MeasureDate,MeasureStatus,DetailedInfo,IsEnglish ," +
                                                          "IsReported FROM HFM_MeasureData";
        /// <summary>
        /// 查询ID小于measureDataID的所有监测数据记录的IsReported值
        /// </summary>
        private const string SQL_SELECT_MEASUREDATA_BY_ISREPORTED = "SELECT IsReported FROM HFM_MeasureData WHERE MeasureID<@measureDataID";
        #endregion

        #region 字段属性
        private int _measureID;//ID
        private DateTime _measureDate;//测量时间
        private string _measureStatus="";//测量状态
        private string _detailedInfo="";//详细描述
        private Channel _channel;//测量通道
        private float _alpha;//Alpha计数值
        private float _beta;//Beta计数值
        private float _analogV;//模拟电压值
        private float _digitalV;//数字电压值
        private float _hV;//高压值
        private int _infraredStatus=0;//红外状态，0：手部不到位/衣物探头未拿起 1：手部到位/衣物探头拿起
        private bool _isEnglish=false;//是否英文
        private bool _isReported;//是否上报
        /// <summary>
        /// 测量数据ID
        /// </summary>
        public int MeasureID { get => _measureID; set => _measureID = value; }
        /// <summary>
        /// 测量时间
        /// </summary>
        public DateTime MeasureDate { get => _measureDate; set => _measureDate = value; }
        /// <summary>
        /// 测量状态
        /// </summary>
        public string MeasureStatus { get => _measureStatus; set => _measureStatus = value; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string DetailedInfo { get => _detailedInfo; set => _detailedInfo = value; }
        /// <summary>
        /// Alpha计数值
        /// </summary>
        public float Alpha { get => _alpha; set => _alpha = value; }
        /// <summary>
        /// Beta计数值
        /// </summary>
        public float Beta { get => _beta; set => _beta = value; }
        /// <summary>
        /// 模拟电压值
        /// </summary>
        public float AnalogV { get => _analogV; set => _analogV = value; }
        /// <summary>
        /// 数字电压值
        /// </summary>
        public float DigitalV { get => _digitalV; set => _digitalV = value; }
        /// <summary>
        /// 高压值
        /// </summary>
        public float HV { get => _hV; set => _hV = value; }
        /// <summary>
        /// 红外状态 1：手部到位/衣物探头拿起；0：手部不到位/衣物探头未拿起
        /// </summary>
        public int InfraredStatus { get => _infraredStatus; set => _infraredStatus = value; }
        /// <summary>
        /// 是否英文
        /// </summary>
        public bool IsEnglish { get => _isEnglish; set => _isEnglish = value; }
        /// <summary>
        /// 测量通道
        /// </summary>
        internal Channel Channel { get => _channel; set => _channel = value; }
        /// <summary>
        /// 是否上报
        /// </summary>
        public bool IsReported { get => _isReported; set => _isReported = value; }
        #endregion

        #region 构造函数
        public MeasureData()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <param name="measureDate">测量时间</param>
        /// <param name="alpha">Alpha计数值</param>
        /// <param name="beta">Beta计数值</param>
        /// <param name="analogV">模拟电压值</param>
        /// <param name="digitalV">数字电压值</param>
        /// <param name="hV">高压值</param>
        public MeasureData(int channelID,DateTime measureDate,float alpha,float beta,float analogV,float digitalV,float hV)
        {
            this._channel = (new Channel()).GetChannel(channelID);
            this._measureDate = measureDate;
            this._alpha = alpha;
            this._beta = beta;
            this._analogV = analogV;
            this._digitalV = digitalV;
            this._hV = hV;
        }
        #endregion

        #region 查询所有监测数据
        /// <summary>
        /// 查询所有监测数据
        /// </summary>
        /// <returns>监测数据</returns>
        public IList<MeasureData> GetData()
        {
            IList<MeasureData> IMeasureDateS = new List<MeasureData>();
            //从数据库中查询全部的监测数据记录并赋值给IMeasureDataS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_MEASUREDATA))
            {
                while (reader.Read())//读查询结果
                {
                    //构造MearsureDate对象
                    MeasureData measuredata = new MeasureData();
                    measuredata.MeasureID = Convert.ToInt32(reader["MeasureID"].ToString());
                    measuredata.MeasureDate = Convert.ToDateTime(reader["MeasureDate"].ToString());
                    measuredata.MeasureStatus = Convert.ToString(reader["MeasureStatus"].ToString());
                    measuredata.DetailedInfo = Convert.ToString(reader["DetailedInfo"].ToString());
                    measuredata.IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString());
                    measuredata.IsReported = Convert.ToBoolean(reader["IsReported"].ToString());
                    //从reader读出并将构造的查询结果添加到List中
                    IMeasureDateS.Add(measuredata);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return IMeasureDateS;
        }
        #endregion

        #region 按照语言查询监测数据
        /// <summary>
        /// 按照语言查询监测数据
        /// </summary>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        public IList<MeasureData> GetData(bool isEnglish)
        {
            IList<MeasureData> IMeasureDateS = new List<MeasureData>();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
                {
                    new OleDbParameter("@isEnglish",OleDbType.Boolean,2)
                };
            parms[0].Value = isEnglish;
            //根据语言从数据库中查询全部的监测数据记录并赋值给IMeasureDataS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_MEASUREDATA_BY_ISENGLISH, parms))
            {
                while (reader.Read())//读查询结果
                {
                    //根据查询结果构造MearsureDate对象
                    MeasureData measureData = new MeasureData();
                    measureData.MeasureID = Convert.ToInt32(reader["MeasureID"].ToString());
                    measureData.MeasureDate = Convert.ToDateTime(reader["MeasureDate"].ToString());
                    measureData.MeasureStatus = Convert.ToString(reader["MeasureStatus"].ToString());
                    measureData.DetailedInfo = Convert.ToString(reader["DetailedInfo"].ToString());
                    measureData.IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString());
                    measureData.IsReported = Convert.ToBoolean(reader["IsReported"].ToString());
                    //从reader读出并将构造的对象添加到List中
                    IMeasureDateS.Add(measureData);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return IMeasureDateS;
        }
        #endregion

        #region 添加监测数据
        /// <summary>
        /// 添加监测数据。
        /// </summary>
        /// <param name="measureData"></param>
        /// <returns></returns>
        public bool AddData(MeasureData measureData)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
                {
                    new OleDbParameter ("@MeasureDate",OleDbType.Date ,8),
                    new OleDbParameter ("@MeasureStatus",OleDbType.VarChar,50),
                    new OleDbParameter ("@DetailedInfo",OleDbType.LongVarChar),
                    new OleDbParameter ("@IsEnglish",OleDbType.Boolean ,2),
                    new OleDbParameter ("@IsReported",OleDbType.Boolean ,2)
                };
            parms[0].Value = measureData.MeasureDate;
            parms[1].Value = measureData.MeasureStatus.ToString();
            parms[2].Value = measureData.DetailedInfo.ToString();
            parms[3].Value = measureData.IsEnglish;
            parms[4].Value = measureData.IsReported;
            if (DbHelperAccess.ExecuteSql(SQL_INSERT_MEASUREDATA, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 查询最新一条监测记录
        public MeasureData GetData()
        {
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_MEASUREDATA_BY_NEWRECORD))
            {
                while (reader.Read())//读查询结果
                {
                    this.MeasureID = Convert.ToInt32(reader["MeasureID"].ToString());
                    this.MeasureDate = Convert.ToDateTime(reader["MeasureDate"].ToString());
                    this.MeasureStatus = Convert.ToString(reader["MeasureStatus"].ToString());
                    this.DetailedInfo = Convert.ToString(reader["DetailedInfo"].ToString());
                    this.IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString());
                    this.IsReported = Convert.ToBoolean(reader["IsReported"].ToString());
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return this;
        }
        #endregion

        #region 更新上报状态。
        public bool UpdataReported(bool isReported, int measureDataID)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@measureDataID",OleDbType.Integer,4)
            };
            parms[0].Value = measureDataID;
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_MEASUREDATA_BY_ISREPORTED, parms))
            {
                while (reader.Read())
                {
                    this.IsReported = isReported;                
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return true;
        }
        #endregion
    }
}

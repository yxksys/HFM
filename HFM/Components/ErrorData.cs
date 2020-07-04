/**
 * ________________________________________________________________________________ 
 *
 *  描述：故障数据类
 *  作者：白茹
 *  版本：
 *  创建时间：2020年2月19日
 *  类名：故障数据类
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
    public class ErrorData
    {
        #region 数据库查询语句
        /// <summary>
        /// 查询所有故障数据
        /// </summary>
        private const string SQL_SELECT_ERRORDATA = "SELECT ErrID,ErrTime,Record,IsEnglish,IsReported FROM HFM_ErrData";
        /// <summary>
        /// 按照语言查询故障数据
        /// </summary>
        private const string SQL_SELECT_ERRORDATA_BY_ISENGLISH = "SELECT ErrID,ErrTime,Record,IsEnglish,IsReported " +
                                                                 "FROM HFM_ErrData WHERE IsEnglish=@IsEnglish";
        /// <summary>
        /// /添加故障数据
        /// </summary>
        private const string SQL_INSERT_ERRORDATA = "INSERT INTO HFM_ErrData (ErrTime,Record,IsEnglish,IsReported) " +
                                                    "VALUES(@ErrTime,@Record,@IsEnglish,@IsReported)";
        /// <summary>
        /// 删除故障记录表中所有记录 
        /// </summary>
        private const string SQL_DELETE_ERRORDATA = "DELETE FROM HFM_ErrData";
        /// <summary>
        /// 查询最新一条监测记录
        /// </summary>
        private const string SQL_SELECT_ERRORDATA_BY_NEWRECORD = "SELECT TOP 1 ErrID,ErrTime,Record,IsEnglish,IsReported FROM HFM_ErrData ORDER BY ErrID DESC";
        /// <summary>
        /// 查询ID小于errorDataID的所有监测数据记录的IsReported值
        /// </summary>
        /// 
        private const string SQL_SELECT_ERRORDATA_BY_ISREPORTED = "SELECT IsReported FROM HFM_ErrData WHERE ErrID=@errorDataID";
        /// <summary>
        /// 更新上报状态标志
        /// </summary>
        private const string SQL_UPDATE_REPORTED_BY_ERRORID = "UPDATE HFM_ErrData SET IsReported=@IsReported WHERE ErrID=@errorDataID";
        #endregion

        #region 字段属性
        private int _errID;//故障ID
        private DateTime _errTime;//故障时间
        private string _record;//备注
        private bool _isEnglish;//是否英文
        private bool _isReported;//是否上报
        /// <summary>
        /// 故障ID
        /// </summary>
        public int ErrID { get => _errID; set => _errID = value; }
        /// <summary>
        /// 故障时间
        /// </summary>
        public DateTime ErrTime { get => _errTime; set => _errTime = value; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Record { get => _record; set => _record = value; }
        /// <summary>
        /// 是否英文
        /// </summary>
        public bool IsEnglish { get => _isEnglish; set => _isEnglish = value; }
        /// <summary>
        /// 是否上报
        /// </summary>
        public bool IsReported { get => _isReported; set => _isReported = value; }

        #endregion

        #region 查询所有故障数据
        /// <summary>
        /// 查询所有故障数据
        /// </summary>
        /// <returns></returns>
        public IList<ErrorData> GetData()
        {
            IList<ErrorData> IErrorDateS = new List<ErrorData>();
            //从数据库中查询所有故障数据并赋值给IErrorDataS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_ERRORDATA))
            {
                while (reader.Read())//读取查询结果
                {
                    //构造ErrorData对象
                    ErrorData errorData = new ErrorData
                    {
                        ErrID = Convert.ToInt32(reader["ErrID"].ToString()),
                        ErrTime = Convert.ToDateTime(reader["ErrTime"].ToString()),
                        Record = Convert.ToString(reader["Record"].ToString()),
                        IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString())
                    };
                    //将reader读出并构造的查询结果添加到List中
                    IErrorDateS.Add(errorData);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return IErrorDateS;
        }
        #endregion

        #region 按照语言查询故障数据
        /// <summary>
        /// 按照语言查询故障数据
        /// </summary>
        /// <param name="isEnglish">bool false:中文 ,true:English</param>
        /// <returns>故障数据列表</returns>
        public IList<ErrorData> GetData(bool isEnglish)
        {
            IList<ErrorData> IErrorDateS = new List<ErrorData>();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                    new OleDbParameter("@isEnglish",OleDbType.Boolean,2)
            };
            parms[0].Value = isEnglish;
            //从数据库中查询所有故障数据并赋值给IErrorDataS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_ERRORDATA_BY_ISENGLISH, parms))
            {
                while (reader.Read())//读取查询结果
                {
                    //构造ErrorData对象
                    ErrorData errorData = new ErrorData
                    {
                        ErrID = Convert.ToInt32(reader["ErrID"].ToString()),
                        ErrTime = Convert.ToDateTime(reader["ErrTime"].ToString()),
                        Record = Convert.ToString(reader["Record"].ToString()),
                        IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString())
                    };
                    //将reader读出并构造的查询结果添加到List中
                    IErrorDateS.Add(errorData);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return IErrorDateS;
        }
        #endregion

        #region 添加故障数据
        /// <summary>
        /// 添加故障数据
        /// </summary>
        /// <param name="errorData"></param>
        /// <returns>成功失败</returns>
        public bool AddData(ErrorData errorData)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
            new OleDbParameter("@ErrTime", OleDbType.Date, 8),
            new OleDbParameter("@Record", OleDbType.LongVarChar),
            new OleDbParameter("@IsEnglish", OleDbType.Boolean, 2),
            new OleDbParameter ("@IsReported",OleDbType.Boolean ,2)
            };
            parms[0].Value = errorData.ErrTime;
            parms[1].Value = errorData.Record.ToString();
            parms[2].Value = errorData.IsEnglish;
            parms[3].Value = errorData.IsReported;
            if (DbHelperAccess.ExecuteSql(SQL_INSERT_ERRORDATA, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region 删除表中所有记录

        public int DeleteData()
        {
            return DbHelperAccess.ExecuteSql(SQL_DELETE_ERRORDATA);
        }

        #endregion

        #region 查询最新一条监测记录
        public ErrorData GetLatestData()
        {
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_ERRORDATA_BY_NEWRECORD))
            {
                if(reader.HasRows!=true)
                {
                    reader.Close();
                    DbHelperAccess.Close();
                    return null;
                }
                while (reader.Read())//读取查询结果
                {
                    //构造ErrorData对象
                    //ErrorData errorData = new ErrorData();
                    this.ErrID = Convert.ToInt32(reader["ErrID"].ToString());
                    this.ErrTime = Convert.ToDateTime(reader["ErrTime"].ToString());
                    this.Record = Convert.ToString(reader["Record"].ToString());
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
        public bool UpdateReported(bool isReported, int errorDataID)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@IsReported",OleDbType.Boolean,2),
                new OleDbParameter("@errorDataID",OleDbType.Integer,4)
            };
            parms[0].Value = isReported;
            parms[1].Value = errorDataID;
            if(DbHelperAccess.ExecuteSql(SQL_UPDATE_REPORTED_BY_ERRORID, parms)>0)
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

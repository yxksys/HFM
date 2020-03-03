/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
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
        private const string SQL_SELECT_ERRORDATA = "SELECT ErrID,ErrTime,Record,IsEnglish FROM HFM_ErrData";
        private const string SQL_SELECT_ERRORDATA_BY_ISENGLISH = "SELECT ErrID,ErrTime,Record,IsEnglish" +
                                                                 "FROM HFM_ErrData WHRER IsEnglish=@IsEnglish";
        private const string SQL_INSERT_ERRORDATA = "INSERT INTO FROM HFM_ErrData (ErrTime,Record,IsEnglish)" +
                                                    "VALUES(@ErrTime,@Record,@IsEnglish )";
        #region 字段属性
        private int _errID;//故障ID
        private DateTime _errTime;//故障时间
        private string _record;//备注
        private bool _isEnglish;//是否英文
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
                    ErrorData errorData = new ErrorData();
                    errorData.ErrID = Convert.ToInt32(reader["ErrID"].ToString());
                    errorData.ErrTime = Convert.ToDateTime(reader["ErrTime"].ToString());
                    errorData.Record = Convert.ToString(reader["Record"].ToString());
                    errorData.IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString());
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
                    ErrorData errorData = new ErrorData();
                    errorData.ErrID = Convert.ToInt32(reader["ErrID"].ToString());
                    errorData.ErrTime = Convert.ToDateTime(reader["ErrTime"].ToString());
                    errorData.Record = Convert.ToString(reader["Record"].ToString());
                    errorData.IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString());
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
            new OleDbParameter("@IsEnglish", OleDbType.Boolean, 2)
            };
            parms[0].Value = errorData.ErrTime;
            parms[1].Value = errorData.Record.ToString();
            parms[2].Value = errorData.IsEnglish;
            if (DbHelperAccess.ExecuteSql(SQL_INSERT_ERRORDATA,parms) != 0)
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

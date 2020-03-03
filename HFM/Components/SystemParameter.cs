﻿/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁  杨旭锴
 *  版本：
 *  创建时间：2020年2月17日 09:41:19
 *  类名：系统参数类  SystemParameter
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
    class SystemParameter
    {
        #region 数据库查询语句
        /// <summary>
        /// 查询字段:测量单位、自检时间、平滑时间、报警时间、强制本底次数、衣物离线自检时间、当前是否英文版本
        /// </summary>
        private const string SQL_SELECT_MAINPREFERENCE = "SELECT MeasurementUnit, SelfCheckTime, SmoothingTime," +
                                                        " MeasuringTime, AlarmTime, BKGUpdate, ClothOfflineTime, IsEnglish " +
                                                        " FROM HFM_MainPreference";
        private const string SQL_UPDATE_MAINPREFERENCE = "UPDATE HFM_MainPreference " +
                                                        "SET  MeasurementUnit=@MeasurementUnit, SelfCheckTime=@SelfCheckTime," +
                                                        " SmoothingTime=@SmoothingTime, MeasuringTime=@MeasuringTime, AlarmTime=@AlarmTime," +
                                                        " BKGUpdate='@BKGUpdate', ClothOfflineTime=@ClothOfflineTime, IsEnglish=@IsEnglish";
        #endregion

        #region 属性
        private string _measurementUnit;//测量单位
        private int _selfCheckTime;//自检时间
        private int _smoothingTime;//平滑时间
        private int _measuringTime;//测量时间
        private int _alarmTime;//报警时间
        private int _bkgUpdate;//强制本底次数
        private int _clothOfflineTime;//衣物离线自检时间
        private bool _isEnglish;//当前是否英文版本
        private int _measuredCount;//已经完成检查次数

        /// <summary>
        /// 测量单位
        /// </summary>
        public string MeasurementUnit { get => _measurementUnit; set => _measurementUnit = value; }
        /// <summary>
        /// 自检时间
        /// </summary>
        public int SelfCheckTime { get => _selfCheckTime; set => _selfCheckTime = value; }
        /// <summary>
        /// 平滑时间
        /// </summary>
        public int SmoothingTime { get => _smoothingTime; set => _smoothingTime = value; }
        /// <summary>
        /// 测量时间
        /// </summary>
        public int MeasuringTime { get => _measuringTime; set => _measuringTime = value; }
        /// <summary>
        /// 报警时间
        /// </summary>
        public int AlarmTime { get => _alarmTime; set => _alarmTime = value; }
        /// <summary>
        /// 强制本底次数
        /// </summary>
        public int BkgUpdate { get => _bkgUpdate; set => _bkgUpdate = value; }
        /// <summary>
        /// 衣物离线自检时间
        /// </summary>
        public int ClothOfflineTime { get => _clothOfflineTime; set => _clothOfflineTime = value; }
        /// <summary>
        /// 当前是否英文版本
        /// </summary>
        public bool IsEnglish { get => _isEnglish; set => _isEnglish = value; }
        /// <summary>
        /// 已经完成检测人数
        /// </summary>       
        public int MeasuredCount { get => _measuredCount; set => _measuredCount = value; }

        #endregion

        #region 构造函数
        public SystemParameter()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="measurementUnit">测量单位</param>
        /// <param name="selfCheckTime">自检时间</param>
        /// <param name="smoothingTime">平滑时间</param>
        /// <param name="measuringTime">测量时间</param>
        /// <param name="alarmTime">报警时间</param>
        /// <param name="bkgUpdate">强制本底次数</param>
        /// <param name="clothOfflineTime">衣物离线自检时间</param>
        /// <param name="isEnglish">当前是否英文版本</param>
        public SystemParameter(string measurementUnit, int selfCheckTime, int smoothingTime, int measuringTime, int alarmTime, int bkgUpdate, int clothOfflineTime, bool isEnglish)
        {
            this._measurementUnit = measurementUnit;
            this._selfCheckTime = selfCheckTime;
            this._smoothingTime = smoothingTime;
            this._measuringTime = measuringTime;
            this._alarmTime = alarmTime;
            this._bkgUpdate = bkgUpdate;
            this._clothOfflineTime = clothOfflineTime;
            this._isEnglish = isEnglish;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 从数据库中查询当前系统参数并返回系统参数对象
        /// </summary>
        /// <returns>返回系统参数对象</returns>
        public SystemParameter GetParameter()
        {
            SystemParameter systemParameter = new SystemParameter();
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_MAINPREFERENCE))
            {
                while (reader.Read())
                {
                    systemParameter.MeasurementUnit = Convert.ToString(reader["MeasurementUnit"].ToString());
                    systemParameter.SelfCheckTime = Convert.ToInt32(reader["SelfCheckTime"].ToString() == "" ? "0" : reader["SelfCheckTime"].ToString());
                    systemParameter.SmoothingTime = Convert.ToInt32(reader["SmoothingTime"].ToString() == "" ? "0" : reader["SmoothingTime"].ToString());
                    systemParameter.MeasuringTime = Convert.ToInt32(reader["MeasuringTime"].ToString() == "" ? "0" : reader["MeasuringTime"].ToString());
                    systemParameter.AlarmTime = Convert.ToInt32(reader["AlarmTime"].ToString() == "" ? "0" : reader["AlarmTime"].ToString());
                    systemParameter.BkgUpdate = Convert.ToInt32(reader["BKGUpdate"].ToString() == "" ? "0" : reader["BKGUpdate"].ToString());
                    systemParameter.ClothOfflineTime = Convert.ToInt32(reader["ClothOfflineTime"].ToString() == "" ? "0" : reader["ClothOfflineTime"].ToString());
                    systemParameter.IsEnglish = Convert.ToBoolean(reader["IsEnglish"].ToString());
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return systemParameter;
        }
        /// <summary>
        /// 更新数据库中系统设置
        /// </summary>
        /// <param name="systemParameter">系统设置对象</param>
        /// <returns>是否成功</returns>
        public bool SetParameter(SystemParameter systemParameter)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("MeasurementUnit",OleDbType.VarChar,255),
                new OleDbParameter("SelfCheckTime",OleDbType.VarChar,255),
                new OleDbParameter("SmoothingTime",OleDbType.VarChar,255),
                new OleDbParameter("MeasuringTime",OleDbType.VarChar,255),
                new OleDbParameter("AlarmTime",OleDbType.Boolean),
                new OleDbParameter("BkgUpdate",OleDbType.VarChar,255),
                new OleDbParameter("ClothOfflineTime",OleDbType.VarChar,255),
                new OleDbParameter("IsEnglish",OleDbType.Boolean)
            };
            parms[0].Value = systemParameter.MeasurementUnit.ToString();
            parms[1].Value = systemParameter.SelfCheckTime.ToString();
            parms[2].Value = systemParameter.SmoothingTime.ToString();
            parms[3].Value = systemParameter.MeasuringTime.ToString();
            parms[4].Value = systemParameter.AlarmTime;
            parms[5].Value = systemParameter.BkgUpdate.ToString();
            parms[6].Value = systemParameter.ClothOfflineTime.ToString();
            parms[7].Value = systemParameter.IsEnglish;
            //执行更新语句
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_MAINPREFERENCE, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 更新检查次数
        /// 每次检测完成后检查次数+1
        /// </summary>
        /// <returns></returns>
        public void UpdateMeasuredCount()
        {
            
        }
        /// <summary>
        /// 已经完成检查次数清零
        /// 每次本底测量后需清零
        /// </summary>
        /// <returns></returns>
        public void ClearMeasuredCount()
        {
            
        }
        #endregion
    }
}

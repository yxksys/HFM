/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：
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

namespace HFM.Components
{
    class SystemParameter
    {
        #region 属性
        private string _measurementUnit;//测量单位
        private int _selfCheckTime;//自检时间
        private int _smoothingTime;//平滑时间
        private int _measuringTime;//测量时间
        private int _alarmTime;//报警时间
        private int _bkgUpdate;//强制本底次数
        private int _clothOfflineTime;//衣物离线自检时间
        private bool _isEnglish;//当前是否英文版本

        

        public string MeasurementUnit { get => _measurementUnit; set => _measurementUnit = value; }
        public int SelfCheckTime { get => _selfCheckTime; set => _selfCheckTime = value; }
        public int SmoothingTime { get => _smoothingTime; set => _smoothingTime = value; }
        public int MeasuringTime { get => _measuringTime; set => _measuringTime = value; }
        public int AlarmTime { get => _alarmTime; set => _alarmTime = value; }
        public int BkgUpdate { get => _bkgUpdate; set => _bkgUpdate = value; }
        public int ClothOfflineTime { get => _clothOfflineTime; set => _clothOfflineTime = value; }
        public bool IsEnglish { get => _isEnglish; set => _isEnglish = value; }

        #endregion
        #region 构造函数
        public SystemParameter()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="_measurementUnit"></param>
        /// <param name="_selfCheckTime"></param>
        /// <param name="_smoothingTime"></param>
        /// <param name="_measuringTime"></param>
        /// <param name="_alarmTime"></param>
        /// <param name="_bkgUpdate"></param>
        /// <param name="_clothOfflineTime"></param>
        /// <param name="_isEnglish"></param>
        public SystemParameter(string _measurementUnit, int _selfCheckTime, int _smoothingTime, int _measuringTime, int _alarmTime, int _bkgUpdate, int _clothOfflineTime, bool _isEnglish)
        {
            this._measurementUnit = _measurementUnit;
            this._selfCheckTime = _selfCheckTime;
            this._smoothingTime = _smoothingTime;
            this._measuringTime = _measuringTime;
            this._alarmTime = _alarmTime;
            this._bkgUpdate = _bkgUpdate;
            this._clothOfflineTime = _clothOfflineTime;
            this._isEnglish = _isEnglish;
        }
        #endregion
        #region 方法
        /// <summary>
        /// 从数据库中查询当前工厂参数并返回工厂参数对象
        /// </summary>
        /// <returns></returns>
        public SystemParameter GetParameter()
        {
            return null;
        }
        /// <summary>
        /// 更新数据库中工厂设置
        /// </summary>
        /// <param name="systemParameter"></param>
        /// <returns></returns>
        public bool SetParameter(SystemParameter systemParameter)
        {
            return true;
        }

        #endregion
    }
}

/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：2020/2/14
 *  类名：工厂参数类  FactoryParmeter
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
    class FactoryParameter
    {
        #region 属性
        private string _instrumentNum;//仪器编号
        private string _softName;//软件名称
        private string _ipAddress;//IP地址
        private string _portNumber;//通信端口
        private bool _isConnectedAuto;//是否自动连接
        private string _measureType;//探测类型
        private float _smoothingFactor;//平滑因子
        private bool _isDoubleProbe;//手部是否双探测器

        

        public string InstrumentNum { get => _instrumentNum; set => _instrumentNum = value; }
        public string SoftName { get => _softName; set => _softName = value; }
        public string IpAddress { get => _ipAddress; set => _ipAddress = value; }
        public string PortNumber { get => _portNumber; set => _portNumber = value; }
        public bool IsConnectedAuto { get => _isConnectedAuto; set => _isConnectedAuto = value; }
        public string MeasureType { get => _measureType; set => _measureType = value; }
        public float SmoothingFactor { get => _smoothingFactor; set => _smoothingFactor = value; }
        public bool IsDoubleProbe { get => _isDoubleProbe; set => _isDoubleProbe = value; }

        #endregion
        #region 构造函数
        public FactoryParameter()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="_instrumentNum"></param>
        /// <param name="_softName"></param>
        /// <param name="_ipAddress"></param>
        /// <param name="_portNumber"></param>
        /// <param name="_isConnectedAuto"></param>
        /// <param name="_measureType"></param>
        /// <param name="_smoothingFactor"></param>
        /// <param name="_isDoubleProbe"></param>
        public FactoryParameter(string _instrumentNum, string _softName, string _ipAddress, string _portNumber, bool _isConnectedAuto, string _measureType, float _smoothingFactor, bool _isDoubleProbe)
        {
            this._instrumentNum = _instrumentNum;
            this._softName = _softName;
            this._ipAddress = _ipAddress;
            this._portNumber = _portNumber;
            this._isConnectedAuto = _isConnectedAuto;
            this._measureType = _measureType;
            this._smoothingFactor = _smoothingFactor;
            this._isDoubleProbe = _isDoubleProbe;
        }
        #endregion
        #region 方法
        /// <summary>
        /// 从数据库中查询当前工厂参数并返回工厂参数对象
        /// </summary>
        /// <returns></returns>
        public FactoryParameter GetParameter()
        {
            return null;
        }
        /// <summary>
        /// 更新数据库中工厂设置
        /// </summary>
        /// <param name="factoryParameter"></param>
        /// <returns></returns>
        public bool SetParameter(FactoryParameter factoryParameter)
        {
            return true;
        }

        #endregion
    }
}

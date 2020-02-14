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

namespace HFM.Components
{
    class ChannelParameter
    {
        #region 属性
        private int _checkingID;
        private Channel _channel;
        private float _alphaThreshold;
        private float _betaThreshold;
        private float _presetHV;
        private float _aDCFactor;
        private float _dACFactor;
        private float _hVFactor;
        private float _hVRatio;
        private float _workTime;

        
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
        public ChannelParameter(int _checkingID, Channel _channel, float _alphaThreshold, float _betaThreshold, float _presetHV, float _aDCFactor, float _dACFactor, float _hVFactor, float _hVRatio, float _workTime)
        {
            this._checkingID = _checkingID;
            this._channel = _channel;
            this._alphaThreshold = _alphaThreshold;
            this._betaThreshold = _betaThreshold;
            this._presetHV = _presetHV;
            this._aDCFactor = _aDCFactor;
            this._dACFactor = _dACFactor;
            this._hVFactor = _hVFactor;
            this._hVRatio = _hVRatio;
            this._workTime = _workTime;
        }
        #endregion
        #region 方法
        /// <summary>
        /// 获得全部道盒参数
        /// </summary>
        /// <returns></returns>
        public IList<ChannelParameter> GetParameter()
        {
            return null;
        }
        /// <summary>
        /// 根据参数对象channelParameter的通道ID，设置道盒参数
        /// </summary>
        /// <param name="channelParameter"></param>
        /// <returns></returns>
        public bool SetParameter(ChannelParameter channelParameter)
        {
            return true;
        }

        #endregion
    }
}

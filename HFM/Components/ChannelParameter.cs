/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：
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
        public ChannelParameter(int channelID,float alphaThreshold,float betaThreshold,float presetHV,float aDCFactor,float dACFactor,float hVFactor,float workTime,float hVRatio)
        {
            this._channel = (new Channel()).GetChannel(channelID);
            this._alphaThreshold = alphaThreshold;
            this._betaThreshold = betaThreshold;
            this._presetHV = presetHV;
            this._aDCFactor = aDCFactor;
            this._dACFactor = dACFactor;
            this._hVFactor = hVFactor;
            this._workTime = workTime;
            this._hVRatio = hVRatio;
        }
        #endregion
    }
}

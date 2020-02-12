/**
 * ________________________________________________________________________________ 
 *
 *  ������
 *  ���ߣ�
 *  �汾��
 *  ����ʱ�䣺
 *  ������
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
        #region ����
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
        /// ����ID
        /// </summary>
        public int CheckingID { get => _checkingID; set => _checkingID = value; }
        /// <summary>
        /// Alpha��ֵ
        /// </summary>
        public float AlphaThreshold { get => _alphaThreshold; set => _alphaThreshold = value; }
        /// <summary>
        /// Beta��ֵ
        /// </summary>
        public float BetaThreshold { get => _betaThreshold; set => _betaThreshold = value; }
        /// <summary>
        /// ��ѹֵ
        /// </summary>
        public float PresetHV { get => _presetHV; set => _presetHV = value; }
        /// <summary>
        /// AD����
        /// </summary>
        public float ADCFactor { get => _aDCFactor; set => _aDCFactor = value; }
        /// <summary>
        /// DA����
        /// </summary>
        public float DACFactor { get => _dACFactor; set => _dACFactor = value; }
        /// <summary>
        /// ��ѹ����
        /// </summary>
        public float HVFactor { get => _hVFactor; set => _hVFactor = value; }
        /// <summary>
        /// ��ѹ����
        /// </summary>
        public float HVRatio { get => _hVRatio; set => _hVRatio = value; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public float WorkTime { get => _workTime; set => _workTime = value; }
        /// <summary>
        /// ����ͨ��
        /// </summary>
        internal Channel Channel { get => _channel; set => _channel = value; }
        #endregion

        #region ���캯��
        public ChannelParameter()
        { }
        public ChannelParameter(int channelID, float alphaThreshold, float betaThreshold, float presetHV, float aDCFactor, float dACFactor, float hVFactor, float workTime, float hVRatio)
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

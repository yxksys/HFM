/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：通道类Channel
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
    class Channel
    {
        #region 属性
        private int _channelID;
        private string _channelName;
        private string _channelName_English;
        private float _probeArea;
        private string _status;
        private bool _isEnabled;

        /// <summary>
        /// 通道ID
        /// </summary>
        public int ChannelID
        {
            get => _channelID;
            set => _channelID = value;
        }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get => _channelName; set => _channelName = value; }
        /// <summary>
        /// 通道英文名称
        /// </summary>
        public string ChannelName_English { get => _channelName_English; set => _channelName_English = value; }
        /// <summary>
        /// 测量面积
        /// </summary>
        public float ProbeArea { get => _probeArea; set => _probeArea = value; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get => _status; set => _status = value; }
        /// <summary>
        /// 通道是否启用
        /// </summary>
        public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }
        #endregion

        #region 构造函数
        public Channel()
        { }
        public Channel(int channelID, string channelName, string channelName_English, float probeArea, string status, bool isEnabled)
        {
            this._channelID = channelID;
            this._channelName = channelName;
            this._channelName_English = channelName_English;
            this._probeArea = probeArea;
            this._status = status;
            this._isEnabled = isEnabled;
        }
        #endregion
        /// <summary>
        /// 获得全部通道列表
        /// </summary>
        /// <returns>通道列表</returns>
        public IList<Channel> GetChannel()
        {

            return null;
        }

        /// <summary>
        /// 根据通道ID查询通道信息
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <returns>通道ID对应的通道信息</returns>
        public Channel GetChannel(int channelID)
        {
            //添加根据ID查询通道信息代码
            return this;
        }

    }
}

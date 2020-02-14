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
        public int ChannelID { get => _channelID; set => _channelID = value; }
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
        /// <summary>
        /// 构造函数（有参)
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <param name="channelName">通道名称</param>
        /// <param name="channelName_English">通道英文名称</param>
        /// <param name="probeArea">测量面积</param>
        /// <param name="status">状态</param>
        /// <param name="isEnabled">通道是否启用</param>
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

        #region 获得全部通道列表
        /// <summary>
        /// 获得全部通道列表
        /// </summary>
        /// <returns>全部通道列表</returns>
        public IList<Channel> GetChannel()
        {
            return null;
        }
        #endregion

        #region 根据启用状态查询通道列表
        /// <summary>
        /// 根据启用状态查询通道列表
        /// </summary>
        /// <param name="isEnabled">是否英文</param>
        /// <returns></returns>
        public IList<Channel> GetChannel(bool isEnabled)
        {
            return null;
        }
        #endregion

        #region 根据通道ID查询通道信息
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
        #endregion

        #region 根据通道名称查询通道信息
        /// <summary>
        /// 根据通道名称查询通道信息
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <returns></returns>
        public Channel GetChannel(string channelName)
        {
            return null;
        }
        #endregion

        #region 对某一类型通道的启用状态进行设置
        /// <summary>
        /// 对某一类型通道的启用状态进行设置
        /// </summary>
        /// <param name="channelType">channelType值可选0：手部（包含左手心、左手背、右手心、右手背）1：脚部（包含左脚、右脚）2：衣物</param>
        /// <param name="isEnabled">true开启英文</param>
        /// <returns>设置成功/失败</returns>
        public bool SetEnabledByType(int channelType, bool isEnabled)
        {
            return false;
        }
        #endregion

        #region 根据通道ID设置该通道的启用状态
        /// <summary>
        /// 根据通道ID设置该通道的启用状态
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <param name="isEnabled">true开启英文</param>
        /// <returns></returns>
        public bool SetEnabledByID(int channelID, bool isEnabled)
        {
            return true;
        }
        #endregion
    }
}

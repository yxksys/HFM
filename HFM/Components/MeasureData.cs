/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：监测数据类
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
    class MeasureData
    {
        #region 字段属性
        private int _measureID;//ID
        private DateTime _measureDate;//测量时间
        private string _measureStatus="";//测量状态
        private string _detailedInfo="";//详细描述
        private Channel _channel;//测量通道
        private float _alpha;//Alpha计数值
        private float _beta;//Beta计数值
        private float _analogV;//模拟电压值
        private float _digitalV;//数字电压值
        private float _hV;//高压值
        private int _infraredStatus=0;//红外状态，0：手部不到位/衣物探头未拿起 1：手部到位/衣物探头拿起
        private bool _isEnglish=false;//是否英文
        /// <summary>
        /// 测量数据ID
        /// </summary>
        public int MeasureID { get => _measureID; set => _measureID = value; }
        /// <summary>
        /// 测量时间
        /// </summary>
        public DateTime MeasureDate { get => _measureDate; set => _measureDate = value; }
        /// <summary>
        /// 测量状态
        /// </summary>
        public string MeasureStatus { get => _measureStatus; set => _measureStatus = value; }
        /// <summary>
        /// 详细描述
        /// </summary>
        public string DetailedInfo { get => _detailedInfo; set => _detailedInfo = value; }
        /// <summary>
        /// Alpha计数值
        /// </summary>
        public float Alpha { get => _alpha; set => _alpha = value; }
        /// <summary>
        /// Beta计数值
        /// </summary>
        public float Beta { get => _beta; set => _beta = value; }
        /// <summary>
        /// 模拟电压值
        /// </summary>
        public float AnalogV { get => _analogV; set => _analogV = value; }
        /// <summary>
        /// 数字电压值
        /// </summary>
        public float DigitalV { get => _digitalV; set => _digitalV = value; }
        /// <summary>
        /// 高压值
        /// </summary>
        public float HV { get => _hV; set => _hV = value; }
        /// <summary>
        /// 红外状态 1：手部到位/衣物探头拿起；0：手部不到位/衣物探头未拿起
        /// </summary>
        public int InfraredStatus { get => _infraredStatus; set => _infraredStatus = value; }
        /// <summary>
        /// 是否英文
        /// </summary>
        public bool IsEnglish { get => _isEnglish; set => _isEnglish = value; }
        /// <summary>
        /// 测量通道
        /// </summary>
        internal Channel Channel { get => _channel; set => _channel = value; }
        #endregion

        #region 构造函数
        public MeasureData()
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelID">通道ID</param>
        /// <param name="measureDate">测量时间</param>
        /// <param name="alpha">Alpha计数值</param>
        /// <param name="beta">Beta计数值</param>
        /// <param name="analogV">模拟电压值</param>
        /// <param name="digitalV">数字电压值</param>
        /// <param name="hV">高压值</param>
        public MeasureData(int channelID,DateTime measureDate,float alpha,float beta,float analogV,float digitalV,float hV)
        {
            this._channel = (new Channel()).GetChannel(channelID);
            this._measureDate = measureDate;
            this._alpha = alpha;
            this._beta = beta;
            this._analogV = analogV;
            this._digitalV = digitalV;
            this._hV = hV;
        }
        #endregion

        #region 查询所有监测数据
        /// <summary>
        /// 查询所有监测数据
        /// </summary>
        /// <returns>监测数据</returns>
        public IList<MeasureData> GetData()
        {
            return null;
        }
        #endregion

        #region 按照语言查询监测数据
        /// <summary>
        /// 按照语言查询监测数据
        /// </summary>
        /// <param name="isEnglish"></param>
        /// <returns></returns>
        public IList<MeasureData> GetData(bool isEnglish)
        {
            return null;
        }
        #endregion

        #region 添加监测数据
        /// <summary>
        /// 添加监测数据。
        /// </summary>
        /// <param name="measureData"></param>
        /// <returns></returns>
        public bool AddData(MeasureData measureData)
        {
            return false;
        }
        #endregion
    }
}

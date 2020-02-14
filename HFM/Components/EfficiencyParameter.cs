/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：2020/2/14
 *  类名：探测效率参数类 EfficiencyParameter
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
    class EfficiencyParameter
    {
        #region 属性
        private int _efficiencyParamID;
        private Channel _channel;//通道
        private string _nuclideType;//核素类型可选值为：α、β、C
        private string _nuclideName;//核素名称：U_235、Pu_238、Pu_239
        private float _efficiency;//效率值
        /// <summary>
        /// EfficiencyParamID
        /// </summary>
        public int EfficiencyParamID { get => _efficiencyParamID; set => _efficiencyParamID = value; }
        /// <summary>
        /// 核素类型可选值为：α、β、C
        /// </summary>
        public string NuclideType { get => _nuclideType; set => _nuclideType = value; }
        /// <summary>
        /// 核素名称：U_235、Pu_238、Pu_239
        /// </summary>
        public string NuclideName { get => _nuclideName; set => _nuclideName = value; }
        /// <summary>
        /// 效率值
        /// </summary>
        public float Efficiency { get => _efficiency; set => _efficiency = value; }
        /// <summary>
        /// 通道
        /// </summary>
        internal Channel Channel { get => _channel; set => _channel = value; }

        #endregion
        #region 构造函数
        public EfficiencyParameter()
        { }
        #endregion
        #region 方法
        /// <summary>
        /// 获得所有效率参数
        /// </summary>
        /// <returns></returns>
        public IList<EfficiencyParameter> GetParameter()
        {
            return null;
        }
        /// <summary>
        /// 根据核素类型和核素名称查询效率参数
        /// </summary>
        /// <param name="nuclideType"></param>
        /// <param name="nuclideName"></param>
        /// <returns></returns>
        public IList<EfficiencyParameter> GetParameter(string nuclideType,string nuclideName)
        {
            return null;
        }
        /// <summary>
        /// 根据核素类型、通道和核素名称查询效率参数
        /// </summary>
        /// <param name="nuclideType"></param>
        /// <param name="channelID"></param>
        /// <param name="nuclideName"></param>
        /// <returns></returns>
        public EfficiencyParameter GetParameter(string nuclideType, int channelID, string nuclideName)
        {
            return null;
        }
        /// <summary>
        /// 根据参数对象efficiencyParameter的ChannelID、NuclideType，NuclideName更新其Efficiency值
        /// </summary>
        /// <param name="efficiencyParameter"></param>
        /// <returns></returns>
        public bool SetParameter(EfficiencyParameter efficiencyParameter)
        {
            return true;
        }

        #endregion
    }
}

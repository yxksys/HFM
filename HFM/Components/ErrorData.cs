/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：故障数据类
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
    public class ErrorData
    {
        #region 属性
        private int _errID;//故障ID
        private DateTime _errTime;//故障时间
        private string _record;//备注
        private bool _isEnglish;//是否英文
        /// <summary>
        /// 故障ID
        /// </summary>
        public int ErrID { get => _errID; set => _errID = value; }
        /// <summary>
        /// 故障时间
        /// </summary>
        public DateTime ErrTime { get => _errTime; set => _errTime = value; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Record { get => _record; set => _record = value; }
        /// <summary>
        /// 是否英文
        /// </summary>
        public bool IsEnglish { get => _isEnglish; set => _isEnglish = value; }
        #endregion

        /// <summary>
        /// 查询所有故障数据
        /// </summary>
        /// <returns></returns>
        public IList<ErrorData> GetData()
        {
            return null;
        }
        /// <summary>
        /// 按照语言查询故障数据
        /// </summary>
        /// <param name="isEnglish">bool false:中文 ,true:English</param>
        /// <returns>故障数据列表</returns>
        public IList<ErrorData> GetData(bool isEnglish)
        {
            
            return null;
        }

        /// <summary>
        /// 添加故障数据
        /// </summary>
        /// <param name="errorData"></param>
        /// <returns>是否成功</returns>
        public bool AddData(ErrorData errorData)
        {
            return true;
        }
           
    }
}

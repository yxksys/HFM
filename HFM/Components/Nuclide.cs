/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：邢家宁
 *  版本：
 *  创建时间：2020/2/14
 *  类名：核素类 Nuclide
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
    class Nuclide
    {
        #region 属性
        private int _alphaNuclide;//α核素可用
        private int _betaNuclide;//β核素可用
        private int _clothesNuclide;//C（衣物探头）核素可用
        private string _alphaNuclideUser;//α核素选择
        private string _betaNuclideUser;//β核素选择
        private string _clothesNucliderUser;//C（衣物探头）核素选择
        

        public int AlphaNuclide { get => _alphaNuclide; set => _alphaNuclide = value; }
        public int BetaNuclide { get => _betaNuclide; set => _betaNuclide = value; }
        public int ClothesNuclide { get => _clothesNuclide; set => _clothesNuclide = value; }
        public string AlphaNuclideUser { get => _alphaNuclideUser; set => _alphaNuclideUser = value; }
        public string BetaNuclideUser { get => _betaNuclideUser; set => _betaNuclideUser = value; }
        public string ClothesNucliderUser { get => _clothesNucliderUser; set => _clothesNucliderUser = value; }
        #endregion
        #region 构造函数
        public Nuclide()
        { }
        /// <summary>
        /// 参数构造
        /// </summary>
        /// <param name="_alphaNuclide"></param>
        /// <param name="_betaNuclide"></param>
        /// <param name="_clothesNuclide"></param>
        /// <param name="_alphaNuclideUser"></param>
        /// <param name="_betaNuclideUser"></param>
        /// <param name="_clothesNucliderUser"></param>
        public Nuclide(int _alphaNuclide, int _betaNuclide, int _clothesNuclide, string _alphaNuclideUser, string _betaNuclideUser, string _clothesNucliderUser)
        {
            this._alphaNuclide = _alphaNuclide;
            this._betaNuclide = _betaNuclide;
            this._clothesNuclide = _clothesNuclide;
            this._alphaNuclideUser = _alphaNuclideUser;
            this._betaNuclideUser = _betaNuclideUser;
            this._clothesNucliderUser = _clothesNucliderUser;
        }

        #endregion
        #region 方法
        /// <summary>
        /// 从数据库中查询当前核素信息
        /// </summary>
        /// <returns></returns>
        public Nuclide GetNuclide()
        {
            return null;
        }
        /// <summary>
        /// 获得当前α核素选择
        /// </summary>
        /// <returns></returns>
        public string GetAlphaNuclideUser()
        {
            return null;
        }
        /// <summary>
        /// 获得当前β核素选择
        /// </summary>
        /// <returns></returns>
        public string GetBetaNuclideUser()
        {
            return null;
        }
        /// <summary>
        /// 获得当前C核素选择
        /// </summary>
        /// <returns></returns>
        public string GetClothesNuclideUser()
        {
            return null;
        }
        /// <summary>
        /// 更新数据库中的核素信息
        /// </summary>
        /// <param name="nuclide"></param>
        /// <returns></returns>
        public bool SetNuclide(Nuclide nuclide)
        {
            return true;
        }
        /// <summary>
        /// 设置当前α核素选择，将数据库HFM_MainPreference中的
        /// AlphaNuclideUser字段按照参数alphaNuclideUser进行设置
        /// </summary>
        /// <param name="alphaNuclideUser"></param>
        /// <returns></returns>
        public bool SetAlphaNuclideUser(int alphaNuclideUser)
        {
            return true;
        }
        /// <summary>
        /// 设置当前β核素选择，将数据库HFM_MainPreference中的
        /// BetaNuclideUser字段按照参数betaNuclideUser 进行设置
        /// </summary>
        /// <param name="betaNuclideUser"></param>
        /// <returns></returns>
        public bool SetBetaNuclideUser(int betaNuclideUser)
        {
            return true;
        }
        /// <summary>
        /// //设置当前C核素选择，将数据库HFM_MainPreference中的
        /// ClothesNuclideUser字段按照参数clothesNuclideUser进行设置
        /// </summary>
        /// <param name="clothesNuclideUser"></param>
        /// <returns></returns>
        public bool SetClothesNuclideUser(int clothesNuclideUser)
        {
            return true;
        }
        #endregion
    }
}

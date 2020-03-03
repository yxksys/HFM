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
using System.Data.OleDb;

namespace HFM.Components
{
    class Nuclide
    {
        #region 常量
        private const string SQL_SELECT_NUCLIDE = "SELECT AlphaNuclide,BetaNuclide,ClothesNuclide,AlphaNuclideUser," +
                                                  "BetaNuclideUser,ClothesNuclideUser FROM HFM_MainPreference";
        private const string SQL_SELECT_NUCLIDE_ALPHA = "SELECT AlphaNuclideUser FROM HFM_MainPreference";
        private const string SQL_SELECT_NUCLIDE_BETA = "SELECT BetaNuclideUser FROM HFM_MainPreference";
        private const string SQL_SELECT_NUCLIDE_COLTHES = "SELECT ClothesNuclideUser FROM HFM_MainPreference";
        private const string SQL_UPDATE_NUCLIDE = "UPDATE HFM_MainPreference SET AlphaNuclide = @AlphaNuclide," +
                                                  "BetaNuclide = @BetaNuclide,ClothesNuclide = @ClothesNuclide," +
                                                  "AlphaNuclideUser = @AlphaNuclideUser,BetaNuclideUser = @BetaNuclideUser," +
                                                  "ClothesNuclideUser = @ClothesNuclideUser";
        private const string SQL_UPDATE_NUCLIDE_ALPHA = "UPDATE HFM_MainPreference SET AlphaNuclideUser = @AlphaNuclideUser";
        private const string SQL_UPDATE_NUCLIDE_BETA = "UPDATE HFM_MainPreference SET BetaNuclideUser = @BetaNuclideUser";
        private const string SQL_UPDATE_NUCLIDE_COLTHES = "UPDATE HFM_MainPreference SET ClothesNuclideUser = @ClothesNuclideUser";
        #endregion

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
            Nuclide nuclide = new Nuclide();
            //从数据库中查询当前核素信息数据并赋值给nuclide
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_NUCLIDE))
            {
                while (reader.Read())//读查询结果
                {
                    //根据读出的查询结构构造Nuclide对象
                    nuclide.AlphaNuclide = Convert.ToInt32(reader["AlphaNuclide"]);
                    nuclide.BetaNuclide = Convert.ToInt32(reader["BetaNuclide"]);
                    nuclide.ClothesNuclide = Convert.ToInt32(reader["ClothesNuclide"]);
                    nuclide.AlphaNuclideUser = Convert.ToString(reader["AlphaNuclideUser"]);
                    nuclide.BetaNuclideUser = Convert.ToString(reader["BetaNuclideUser"]);
                    nuclide.ClothesNucliderUser = Convert.ToString(reader["ClothesNuclideUser"]);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return nuclide;
        }
        /// <summary>
        /// 获得当前α核素选择
        /// </summary>
        /// <returns></returns>
        public string GetAlphaNuclideUser()
        {
            string nuclide = "";
            //从数据库中查询当前α核素信息数据
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_NUCLIDE))
            {
                while (reader.Read())
                {
                    nuclide = Convert.ToString(reader["AlphaNuclideUser"]);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return nuclide;
        }
        /// <summary>
        /// 获得当前β核素选择
        /// </summary>
        /// <returns></returns>
        public string GetBetaNuclideUser()
        {
            string nuclide = "";
            //从数据库中查询当前β核素信息数据
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_NUCLIDE))
            {                
                while (reader.Read())
                {
                    nuclide = Convert.ToString(reader["BetaNuclideUser"]);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return nuclide;
        }
        /// <summary>
        /// 获得当前C核素选择
        /// </summary>
        /// <returns></returns>
        public string GetClothesNuclideUser()
        {
            string nuclide = "";
            //从数据库中查询当前C核素信息数据
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_NUCLIDE))
            {
                while (reader.Read())
                {
                    nuclide = Convert.ToString(reader["ClothesNuclideUser"]);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return nuclide;
        }
        
        /// <summary>
        /// 设置当前α核素选择，将数据库HFM_MainPreference中的
        /// AlphaNuclideUser字段按照参数alphaNuclideUser进行设置
        /// </summary>
        /// <param name="alphaNuclideUser"></param>
        /// <returns></returns>
        public bool SetAlphaNuclideUser(string alphaNuclideUser)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@AlphaNuclideUser",OleDbType.VarChar,255)
            };
            parms[0].Value = alphaNuclideUser;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_NUCLIDE_ALPHA, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 设置当前β核素选择，将数据库HFM_MainPreference中的
        /// BetaNuclideUser字段按照参数betaNuclideUser 进行设置
        /// </summary>
        /// <param name="betaNuclideUser"></param>
        /// <returns></returns>
        public bool SetBetaNuclideUser(string betaNuclideUser)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@BetaNuclideUser",OleDbType.VarChar,255)
            };
            parms[0].Value = betaNuclideUser;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_NUCLIDE_BETA, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// //设置当前C核素选择，将数据库HFM_MainPreference中的
        /// ClothesNuclideUser字段按照参数clothesNuclideUser进行设置
        /// </summary>
        /// <param name="clothesNuclideUser"></param>
        /// <returns></returns>
        public bool SetClothesNuclideUser(string clothesNuclideUser)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@ClothesNuclideUser",OleDbType.VarChar,255)
            };
            parms[0].Value = clothesNuclideUser;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_NUCLIDE_COLTHES, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新数据库中的核素信息
        /// </summary>
        /// <param name="nuclide"></param>
        /// <returns></returns>
        public bool SetNuclide(Nuclide nuclide)
        {
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@AlphaNuclide",OleDbType.VarChar,255),
                new OleDbParameter("@BetaNuclide",OleDbType.VarChar,255),
                new OleDbParameter("@ClothesNuclide",OleDbType.VarChar,255),
                new OleDbParameter("@AlphaNuclideUser",OleDbType.VarChar,255),
                new OleDbParameter("@BetaNuclideUser",OleDbType.VarChar,255),
                new OleDbParameter("@ClothesNuclideUser",OleDbType.VarChar,255)

            };
            parms[0].Value = nuclide.AlphaNuclide.ToString();
            parms[1].Value = nuclide.BetaNuclide.ToString();
            parms[2].Value = nuclide.ClothesNuclide.ToString();
            parms[3].Value = nuclide.AlphaNuclideUser.ToString();
            parms[4].Value = nuclide.BetaNuclideUser.ToString();
            parms[5].Value = nuclide.ClothesNucliderUser.ToString();
            if (DbHelperAccess.ExecuteSql(SQL_UPDATE_NUCLIDE, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }



        }
        #endregion
    }
}

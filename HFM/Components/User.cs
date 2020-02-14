/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：框架搭建时间 2020年2月14日 20:49:45
 *  类名：用户类
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
    class User
    {
        #region 字段属性
        private int _userId;
        private string _userName;
        private string _passWord;
        private int _role;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get => _userId; set => _userId = value; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get => _userName; set => _userName = value; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get => _passWord; set => _passWord = value; }
        /// <summary>
        /// 角色
        /// </summary>
        public int Role { get => _role; set => _role = value; }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public User()
        {

        }

        #endregion

        #region 密码验证登录
        /// <summary>
        /// 密码验证登录
        /// </summary>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        public User Login(string passWord)
        {
            return null;
        }
        #endregion

        #region 根据用户ID修改密码
        /// <summary>
        /// 根据用户ID修改密码
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="passWord">密码</param>
        /// <returns>返回成功
        ///          或失败   </returns>
        public bool ChangePassWord(int userID, string passWord)
        {
            return false;
        }
        #endregion

        #region 根据用户ID修改密码
        /// <summary>
        /// 从数据库中获得所有用户
        /// </summary>
        /// <returns></returns>
        public IList<User> GetUser()
        {
            return null;
        }
        #endregion

        #region 从数据库中按照用户ID查询用户信息
        /// <summary>
        /// 从数据库中按照用户ID查询用户信息
        /// </summary>
        /// <param name="userID">用户Id</param>
        /// <returns></returns>
        public User GetUser(int userID)
        {
            return null;
        }
        #endregion
    }
}

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
using System.Data.OleDb;

namespace HFM.Components
{
    class User
    {
        private const string SQL_SELECT_USER_BY_LOGIN = "SELECT UserID FROM HFM_UserInfo WHERE PassWord=@PassWord";
        private const string SQL_UPDATA_USER = "UPDATA HFM_UserInfo SET passWord=@passWord WHERE userID=@userID";
        private const string SQL_SELECT_USER = "SELECT UserID,UserName,PassWord,Role FROM HFM_UserInfo";
        private const string SQL_SELECT_USER_BY_USERID = "SELECT UserID,UserName,PassWord,Role FROM HFM_UserInfo WHERE UserID=@UserID";
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
            User user = new User();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@PassWord",OleDbType.VarChar,255)
            };
            parms[0].Value = passWord;
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_USER_BY_LOGIN, parms))
            {
                while (reader.Read())
                {
                    //构造User对象
                    user.UserId = Convert.ToInt32(reader["UserID"].ToString());
                    user.UserName = Convert.ToString(reader["UserName"].ToString());
                    user.PassWord = Convert.ToString(reader["PassWord"].ToString());
                    user.Role = Convert.ToInt32(reader["Role"].ToString());
                    //从reader读出的查询对象添加到List中
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return user;
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
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                new OleDbParameter("@userID",OleDbType.Integer),
                new OleDbParameter("@PassWord",OleDbType.VarChar,255)
            };
            parms[0].Value = userID;
            parms[1].Value = passWord;
            if (DbHelperAccess.ExecuteSql(SQL_UPDATA_USER, parms) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 从数据库中获得所有用户
        /// <summary>
        /// 从数据库中获得所有用户
        /// </summary>
        /// <returns></returns>
        public IList<User> GetUser()
        {
            IList<User> IUserS = new List<User>();
            //从数据库中查询全部刻度操作记录并赋值给IUserS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_USER))
            {
                while (reader.Read()) //读查询结果
                {
                    //构造User对象
                    User user = new User();
                    user.UserId = Convert.ToInt32(reader["UserID"].ToString());
                    user.UserName = Convert.ToString(reader["UserName"].ToString());
                    user.PassWord = Convert.ToString(reader["PassWord"].ToString());
                    user.Role = Convert.ToInt32(reader["Role"].ToString());
                    //从reader读出的查询对象添加到List中
                    IUserS.Add(user);
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            return IUserS;
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
            User user = new User();
            //构造查询参数
            OleDbParameter[] parms = new OleDbParameter[]
            {
                    new OleDbParameter("@userID",OleDbType.Boolean,2)
            };
            parms[0].Value = userID;
            //从数据库中查询全部刻度操作记录并赋值给IUserS
            using (OleDbDataReader reader = DbHelperAccess.ExecuteReader(SQL_SELECT_USER_BY_USERID, parms))
            {
                while (reader.Read())
                {
                    //构造User对象                
                    user.UserId = Convert.ToInt32(reader["UserID"].ToString());
                    user.UserName = Convert.ToString(reader["UserName"].ToString());
                    user.PassWord = Convert.ToString(reader["PassWord"].ToString());
                    user.Role = Convert.ToInt32(reader["Role"].ToString());
                }
                reader.Close();
                DbHelperAccess.Close();
            }
            //从reader读出的查询对象添加到List中
            return user;
        }
        #endregion
    }
}

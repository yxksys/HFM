/**
 * ________________________________________________________________________________ 
 *
 *  描述：
 *  作者：
 *  版本：
 *  创建时间：
 *  类名：平台类
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HFM.Components
{
    class PlatForm
    {
        public struct ColorStatus
        {
            public static Color CORLOR_NORMAL = Color.FromArgb(128, 255, 128);//绿色
            public static Color COLOC_PINGBI = Color.FromArgb(64, 64, 64);
            public static Color COLOR_STATUS = Color.Lime;
            public static Color COLOR_STOP = Color.Gray;
            public static Color CORLOR_ERROR = Color.Orange;
            public static Color COLOR_ALARM_1 = Color.Orange;
            public static Color COLOR_ALARM_2 = Color.Red;
            public static Color COLOR_SYSTEM = SystemColors.Control;
        }
        public struct ErrorRange
        {
            public static double HV_ERROR = 0.2;
            public static double BASE_ERROR = 0.3;
        }

        #region 启动监测
        /// <summary>
        /// 启动监测
        /// </summary>
        public void Run()
        {

        }
        #endregion

    }
}

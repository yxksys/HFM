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
            public static Color CORLOR_BKINPLACE = Color.FromArgb(1, 90, 170);//红外到位背景色蓝色
            public static Color CORLOR_FRNORMAL = Color.White;//通用前景色白色
            public static Color CORLOR_BKNOTINPLACE = Color.FromArgb(161,185,196);//红外不到位背景色
            public static Color CORLOR_FRNOTINPLACE = Color.FromArgb(246,127,27);//红外不到位前景色
            public static Color COLOC_PINGBI = Color.FromArgb(64,64,64);
            public static Color COLOR_STATUS = Color.LightBlue;
            public static Color COLOR_STOP = Color.Gray;
            public static Color CORLOR_ERROR = Color.Orange;
            public static Color COLOR_ALARM_1 = Color.FromArgb(200,120,95);
            public static Color COLOR_ALARM_2 = Color.Red;
            public static Color COLOR_SYSTEM = SystemColors.Control;
            public static Color COLOR_BKENABLED = Color.FromArgb(1, 90, 170);//通道启用背景色蓝色
            public static Color COLOR_BKDISABLED = Color.DarkGray; //Color.FromArgb(142, 167, 178);//通道未启用背景色
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

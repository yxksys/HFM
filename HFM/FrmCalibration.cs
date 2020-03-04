/**
 * ________________________________________________________________________________ 
 *
 *  描述：刻度窗体
 *  作者：杨旭锴
 *  版本：
 *  创建时间：2020年2月25日 16:58:28
 *  类名：刻度窗体
 *  
 *  Copyright (C) 2020 TIT All rights reserved.
 *_________________________________________________________________________________
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmCalibration : Form
    {
        #region 字段、方法、实例
        /// <summary>
        /// 开启串口封装的方法
        /// </summary>
        private void OpenPort()
        {
            CommPort commPort = new CommPort();
            //从配置文件获得当前串口配置
            if (commPort.Opened == true)
            {
                commPort.Close();
            }
            commPort.GetCommPortSet();
            //打开串口
            try
            {
                commPort.Open();
            }
            catch
            {
                MessageBox.Show(@"端口打开错误！请检查通讯是否正常。");
            }
        }
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        /// <summary>
        /// 核数
        /// </summary>
        private int nuclideId = 0;
        /// <summary>
        /// 获取所有“通道参数”
        /// </summary>
        List<Channel> channelList = new Channel().GetChannel(true).ToList();
        /// <summary>
        /// 获取所有“效率参数”
        /// </summary>
        List<EfficiencyParameter> efficiencyList = new EfficiencyParameter().GetParameter().ToList();
        /// <summary>
        /// 
        /// </summary>
        /// 
        #endregion

        #region 初始化加载
        public FrmCalibration()
        {
            InitializeComponent();
            bkWorkerReceiveData.WorkerReportsProgress = true;
        }

        private void FrmCalibration_Load(object sender, EventArgs e)
        {
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
            //开启异步线程
            bkWorkerReceiveData.RunWorkerAsync();
            #region 获得全部启用的通道添加到下拉列表中，更具系统中英文状态选择中英文
            
            
            //根据系统语言填充通道下拉列表
            if (isEnglish == true)
            {
                //英文通道名称
                foreach (var listChannel in channelList)
                {
                    CmbChannelSelection.Items.Add(listChannel.ChannelName_English);
                }
            }
            else
            {
                //中文通道名称
                foreach (var listChannel in channelList)
                {
                    CmbChannelSelection.Items.Add(listChannel.ChannelName);
                }
            }
            #endregion

            #region 获得全部核数添加到下拉列表
            //根据所有“效率参数”去除重复核数
            var listEfficiency = efficiencyList.GroupBy(r => r.NuclideName).ToArray();
            //向核数下拉列表添加核数
            foreach (var item in listEfficiency)
            {
                CmbNuclideSelect.Items.Add(item.Key);
            }
            #endregion

        }
        #endregion

        /// <summary>
        /// 通道下拉列表选择后（触发事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbChannelSelection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //if(CmbChannelSelection.Created.ToString())
        }
    }
}

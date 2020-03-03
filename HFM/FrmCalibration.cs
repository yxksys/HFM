using System;
using System.Collections.Generic;
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

        #endregion

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
            #region 获得全部通道添加到下拉列表中，更具系统中英文状态选择中英文
            var channelList = new HFM.Components.Channel().GetChannel();
            if (isEnglish == true)
            {
                foreach (var listChannel in channelList)
                {
                    CmbChannelSelection.Items.Add(listChannel.ChannelName_English);
                }
            }
            else
            {
                foreach (var listChannel in channelList)
                {
                    CmbChannelSelection.Items.Add(listChannel.ChannelName);
                }
            }
            #endregion

            #region 获得全部核数添加到下拉列表

            

            #endregion

        }
    }
}

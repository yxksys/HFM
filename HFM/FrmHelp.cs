using HFM.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HFM
{
    public partial class FrmHelp : Form
    {
        private CommPort _commPort = new CommPort();
        public FrmHelp()
        {
            InitializeComponent();
            //系统参数对象
            Components.SystemParameter systemParameter = new Components.SystemParameter().GetParameter();
            //工厂参数对象(包含软件仪器名称)
            FactoryParameter factoryParameter = new FactoryParameter().GetParameter();

            //从配置文件获得当前串口配置
            if (_commPort.Opened == true)
            {
                _commPort.Close();
            }
            //Tools tools = new Tools();//实例化工具类：仪器名称中英文转换使用使用
            if (systemParameter.IsEnglish==false)
            {
                //仪器名称
                LblName.Text = "仪器名称:" + factoryParameter.SoftName;
                //软件版本
                LblVersions.Text = "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();//"V3.0";
                //联系方式
                LblNumber.Text = "联系方式:" + "0351-2202181  2203549";
                //版权所有
                LblCopyright.Text = "版权所有（C） 山西中辐核仪器有限责任公司";
            }
            else//英文
            {
                //仪器名称
                LblName.Text = "Model:" + Tools.EnSoftName(factoryParameter.SoftName);
                //LblName.Text = "Model:" + factoryParameter.SoftName;
                //软件版本
                LblVersions.Text = "Version:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();//"3.0.4";
                //联系方式
                LblNumber.Text = "Tel:" + "0351-2202181  2203549";
                //版权所有
                LblCopyright.Text = "Copyright (c) Shanxi Zhongfu Nuclear Instruments Co., Ltd";
            }
        }
        
        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmHelp_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }
    }
}

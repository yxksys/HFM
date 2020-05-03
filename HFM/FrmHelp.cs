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
        public FrmHelp()
        {
            InitializeComponent();
            //系统参数对象
            Components.SystemParameter systemParameter = new Components.SystemParameter().GetParameter();
            //工厂参数对象(包含软件仪器名称)
            FactoryParameter factoryParameter = new FactoryParameter().GetParameter();

            if (systemParameter.IsEnglish==false)
            {
                //仪器名称
                LblName.Text = "仪器名称:" + factoryParameter.SoftName;
                //软件版本
                LblVersions.Text = "软件版本:" + "V1.0";
                //联系方式
                LblNumber.Text = "联系方式:" + "0351-2202150  2203549";
                //版权所有
                LblCopyright.Text = "版权所有（C） 山西中辐核仪器有限责任公司";
            }
            else//英文
            {
                //仪器名称
                LblName.Text = "Model:" + factoryParameter.SoftName;
                //软件版本
                LblVersions.Text = "Version:" + "V1.0";
                //联系方式
                LblNumber.Text = "Tel:" + "0351-2202150  2203549";
                //版权所有
                LblCopyright.Text = "Copyright (c) Shanxi Zhongfu Nuclear Instruments Co., Ltd";
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFM.Components;

namespace HFM
{
    public partial class FrmWelcome : Form
    {
        public FrmWelcome()
        {
            InitializeComponent();
        }

        private void BtnTestRorW_Click(object sender, EventArgs e)
        {
            //测试CalibrationGetData方法
            //Calibration calibration = new Calibration();
            //不带参数查询测试
            //IList<Calibration> Icalibrations = calibration.GetData();
            //带参数查询测试
            //Icalibrations = calibration.GetData(1);
            //if (calibration.AddData(new Calibration(DateTime.Now, 2, 3, 4, 5, 6, 7)) == true)
            //{
            //    MessageBox.Show("添加成功");
            //}    

            ////测试通道类查询    
            //Channel channel = new Channel();    
            //channel.GetChannel(1);    
            //channel.GetChannel("左手心");
            //channel.SetEnabledByType(0, false);
            //if (channel.SetEnabledByType(0, false) == true)
            //{
            //    MessageBox.Show("更新成功");
            //}
            //else
            //{
            //    MessageBox.Show("更新失败");
            //}
            //测试工厂参数类查询
            //FactoryParameter factory = new FactoryParameter();    
            //factory.GetParameter();
            //if(factory.SetParameter(new FactoryParameter("1","2","3","4",true,"6",7,true)) == true)
            //{
            //    MessageBox.Show("更新成功");
            //}
            //else
            //{
            //    MessageBox.Show("更新失败");
            //}
            //测试系统参数查询
            //Components.SystemParameter system = new Components.SystemParameter();
            //system.GetParameter();
            //if(system.SetParameter(new Components.SystemParameter("1", 1, 1, 1, 1, 1, 1, false)) == true)
            //{
            //    MessageBox.Show("更新成功");
            //}
            //测试系统参数查询(新增)
            //ProbeParameter probeParameter = new ProbeParameter();
            //probeParameter=probeParameter.GetParameter(2,"α");
            //if (probeParameter != null)
            //{
            //    MessageBox.Show("ok");
            //}
            //else
            //{
            //    MessageBox.Show("no");
            //}
            Nuclide nuclide = new Nuclide();
            string a = nuclide.GetAlphaNuclideUser();
            IList<EfficiencyParameter> p = new List<EfficiencyParameter>();
            EfficiencyParameter efficiencyParameter = new EfficiencyParameter();
            p = efficiencyParameter.GetParameter("α", a);
            for (int i = 0; i < p.Count && p[i].NuclideType == "α"; i++)
            {
                if (!efficiencyParameter.SetParameter(p[i]))
                {
                    MessageBox.Show("数据库存取失败");
                    return;
                }
            }
        }

        private void BtnCommport_Click(object sender, EventArgs e)
        {
            //实例化相关对象
            Calibration calibration = new Calibration();
            CommPort commPort = new CommPort();
            
            //打开串口
            if (commPort.Opened == false)
            {
                try
                {
                    commPort.Open();
                }
                catch
                {
                    MessageBox.Show("端口打开错误，请检查通讯是否正常！");
                }
            }
           
        }
    }
}

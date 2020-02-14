using System;
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
            Calibration calibration = new Calibration();
            //不带参数查询测试
            IList<Calibration> Icalibrations = calibration.GetData();
            //带参数查询测试
            Icalibrations = calibration.GetData(2);
            if(calibration.AddData(new Calibration(DateTime.Now,2,3,4,5,6,7))==true)
            {
                MessageBox.Show("添加成功");
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using HFM.Components;

namespace HFM
{
    public partial class FrmCalibration : Form
    {
        public FrmCalibration()
        {
            InitializeComponent();
            
        }
        CommPort commPort = new CommPort();
        private void FrmCalibration_Load(object sender, EventArgs e)
        {
            //线程支持异步取消
            bkWorkerReceiveData.WorkerSupportsCancellation = true;
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
                MessageBox.Show("端口打开错误！请检查通讯是否正常。");
                return;
            }
        }
    }
}


using System;
using System.Windows.Forms;

namespace HFM
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 
        public static FrmWelcome  frmWelcome;
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            int x = System.Diagnostics.Process.GetProcessesByName("HFM").Length;
            if (System.Diagnostics.Process.GetProcessesByName("HFM").Length<=1)
            {
                FrmWelcome frmWelcome = new FrmWelcome();
                frmWelcome.Show();
                Application.Run();
            }
            else
            {
                MessageBox.Show("监测程序已经运行");
            }
        }
        
    }
}

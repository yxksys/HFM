
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
            FrmWelcome frmWelcome = new FrmWelcome();
            frmWelcome.Show();
            Application.Run();
        }
        
    }
}

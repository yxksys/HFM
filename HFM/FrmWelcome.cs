using System.Threading;
using System.Windows.Forms;

namespace HFM
{
    public partial class FrmWelcome : Form
    {
        public FrmMeasureMain frmMeasureMain;
        public FrmWelcome()
        {
            InitializeComponent();
        }

        private void FrmWelcome_Load(object sender, System.EventArgs e)
        {
            Time_Welcome_cloes.Start();
        }

        private void Time_Welcome_cloes_Tick(object sender, System.EventArgs e)
        {
            Time_Welcome_cloes.Enabled = false;
            Thread.Sleep(500);            
            frmMeasureMain = new FrmMeasureMain();
            frmMeasureMain.Show();
            this.Dispose();
            
        }

        private void FrmWelcome_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}

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
    public partial class FrmClothes : Form
    {
        public FrmClothes()
        {
            InitializeComponent();
        }

        private void FrmClothes_Load(object sender, EventArgs e)
        {
            loadingCircle.OuterCircleRadius = 20;
            loadingCircle.InnerCircleRadius = 14;
            loadingCircle.NumberSpoke = 20;
            loadingCircle.SpokeThickness = 3;
            loadingCircle.Active = true;
        }
    }
}

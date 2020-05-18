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
using System.Threading;
using System.Globalization;

namespace HFM
{
    public partial class FrmClothes : Form
    {
        public bool isEnglish = false;
        public FrmClothes()
        {
            InitializeComponent();
        }
        public FrmClothes(bool isEnglish)
        {
            this.isEnglish = isEnglish;                   
            InitializeComponent();
        }
        private void FrmClothes_Load(object sender, EventArgs e)
        {
            loadingCircle.OuterCircleRadius = 20;
            loadingCircle.InnerCircleRadius = 14;
            loadingCircle.NumberSpoke = 20;
            loadingCircle.SpokeThickness = 3;
            loadingCircle.Active = true;
            if (isEnglish)
            {
                try
                {
                    isEnglish = true;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
                    Tools.ApplyLanguageResource(this);
                    Tools.controls.Clear();
                }
                catch
                {
                    return;
                }
            }
            else
            {
                try
                {
                    isEnglish = false;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
                    Tools.ApplyLanguageResource(this);
                    Tools.controls.Clear();
                }
                catch
                {
                    return;
                }
            }
        }
    }
}

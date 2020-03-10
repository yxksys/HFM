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

namespace HFM
{
    public partial class FrmKeyIn : Form
    {
        public FrmKeyIn()
        {
            Code = "";
            buttonNum[0] = BtnDot;
            buttonNum[1] = BtnOne;   buttonNum[2] = BtnTwo;   buttonNum[3] = BtnThree;
            buttonNum[4] = BtnFour;  buttonNum[5] = BtnFive;  buttonNum[6] = BtnSix;
            buttonNum[7] = BtnSeven; buttonNum[8] = BtnEight; buttonNum[9] = BtnNine; 
            for(int i=0;i<9;i++)
            {
                buttonNum[i].BackColor=
            }
            InitializeComponent();
        }
        #region 字段 数组
        private string _code;
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        private bool isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);

        public string Code { get => _code; set => _code = value; }

        private Button[] buttonNum = new Button[9];
        #endregion


        //输入完成
        private void BtnEnter_Click(object sender, EventArgs e)
        {
            
        }
        //退格
        private void BtnBackspace_Click(object sender, EventArgs e)
        {

        }
        //窗体加载
        private void FrmKeyIn_Load(object sender, EventArgs e)
        {
            if (isEnglish == true){
                this.Text = "数字输入";
                BtnBackspace.Text  = "退格";
                BtnEnter.Text = "确认";
            }
            else {
                this.Text = "Keyboard";
                BtnBackspace.Text = "Backspace";
                BtnEnter.Text = "Enter";
            }
        }
        //键盘按下
        private void KeyboardInput(object sender, KeyPressEventArgs e)
        {
            //临时变量记录按键
            char tempChar = e.KeyChar;
            if ((tempChar >= 48 && tempChar <= 57) || (tempChar >= 65 && tempChar <= 90) ||
                (tempChar >= 97 && tempChar <= 122) || tempChar == 46)
            {
                Code += tempChar;
            }
            else if(tempChar ==8)
            {
                if (tempChar != 0)
                {
                    Code = Code.Substring(0, Code.Length - 1);
                }
            }
            if ((tempChar >= 65 && tempChar <= 90) ||(tempChar >= 97 && tempChar <= 122))
            {
                LblLetter.Text = tempChar.ToString();
                LblLetter.Visible = true;
                Thread.Sleep(200);
                Code += tempChar;
            }
            else if ((tempChar >= 48 && tempChar <= 57) || tempChar == 46)
            {
                //buttonNum [tempChar -48]=
            }
            else
            {
                MessageBox.Show("请输入字母、数字或小数点内的字符");
            }

        }
    }
}

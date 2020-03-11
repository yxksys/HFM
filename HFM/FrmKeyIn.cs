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
            TempButton.BackColor = Color.Black;
            TempButton.Visible = false;
            buttonNum[0] = BtnDot;
            buttonNum[1] = BtnOne;   buttonNum[2] = BtnTwo;   buttonNum[3] = BtnThree;
            buttonNum[4] = BtnFour;  buttonNum[5] = BtnFive;  buttonNum[6] = BtnSix;
            buttonNum[7] = BtnSeven; buttonNum[8] = BtnEight; buttonNum[9] = BtnNine; 
            InitializeComponent();
        }
        #region 字段 数组
        private string _code;
        /// <summary>
        /// 系统数据库中读取是否开启英文
        /// </summary>
        /// 
        private Button _tempButton;

        private bool isEnglish = (new HFM.Components.SystemParameter().GetParameter().IsEnglish);
        public string Code { get => _code; set => _code = value; }
        public Button TempButton { get => _tempButton; set => _tempButton = value; }

        private Button[] buttonNum = new Button[9];
        #endregion

        #region
        //输入完成
        private void BtnEnter_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //退格
        private void BtnBackspace_Click(object sender, EventArgs e)
        {
            if (Code != "")
            {
                Code = Code.Substring(0, Code.Length - 1);
            }
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
            //如果按键在可输入范围内
            if ((tempChar >= 48 && tempChar <= 57) || (tempChar >= 65 && tempChar <= 90) ||
                (tempChar >= 97 && tempChar <= 122) || tempChar == 46)
            {
                Code += tempChar;//记录输入的数值
                //若输入字母字符
                if ((tempChar >= 65 && tempChar <= 90) || (tempChar >= 97 && tempChar <= 122))
                {
                    LblLetter.Text = tempChar.ToString();
                    LblLetter.Visible = true;
                    Thread.Sleep(100);
                    LblLetter.Visible = false;

                }
                //若输入数字或者小数点
                if ((tempChar >= 48 && tempChar <= 57))
                {
                    Button.Location  = buttonNum[tempChar - 48].Location;
                    Button.Visible = true;
                    Thread.Sleep(200);
                    Button.Visible = false;
                }
                if (tempChar == 46)
                {
                    Button = buttonNum[0];
                    Button.Visible = true;
                    Thread.Sleep(200);
                    Button.Visible = false;
                }
            }
            //如果按下了返回键
            if(tempChar ==8)
            {
                if (Code != "")
                {
                    Code = Code.Substring(0, Code.Length - 1);
                }
            }          
        }
        //按钮按下

        private void Button_Click(object sender, EventArgs e)
        {
            //TempButton = ;
        }
        #endregion
    }
}

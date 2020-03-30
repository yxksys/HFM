﻿using System;
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
    #region 委托函数
    public delegate void SendValue(string str);
    #endregion
    public partial class FrmKeyIn : Form
    {
        #region 字段 数组
        /// <summary>
        /// 字符串 用于记录密码字符
        /// </summary>
        private string _code="";
        /// <summary>
        /// 临时按钮 在键盘输入时用来变黑显示
        /// </summary>
        private Button _tempButton;
        /// <summary>
        /// 委托变量 
        /// </summary>
        public event SendValue sendValue;
        /// <summary>
        /// 判断系统数据库中读取是否开启英文
        /// </summary>
        private bool _isEnglish = false;
        /// <summary>
        /// 数组按钮
        /// </summary>
        private Button[] buttonNum = new Button[9];
        #endregion
        #region 封装
        public string Code { get => _code; set => _code = value; }

        public Button TempButton { get => _tempButton; set => _tempButton = value; }
        #endregion
        #region 方法
         //初始化数据
        public FrmKeyIn(SendValue _sendValue, string _value)
        {
            sendValue = _sendValue;
            Code = _value;
            //使得临时按钮的大小与数字键盘上按钮的大小相同，但颜色为黑色，不可视
            TempButton.BackColor = Color.Black;
            TempButton.Size = new System.Drawing.Size(292, 349); 
            TempButton.Visible = false;
            //给按钮数组赋值
            buttonNum[0] = BtnDot;
            buttonNum[1] = BtnOne;   buttonNum[2] = BtnTwo;   buttonNum[3] = BtnThree;
            buttonNum[4] = BtnFour;  buttonNum[5] = BtnFive;  buttonNum[6] = BtnSix;
            buttonNum[7] = BtnSeven; buttonNum[8] = BtnEight; buttonNum[9] = BtnNine;
            //高亮集中在“确认”按钮上
            BtnEnter.Focus();
            InitializeComponent();
        }        
        #endregion

        #region 界面初始加载
        private void FrmKeyIn_Load(object sender, EventArgs e)
        {
            if (_isEnglish == true)
            {
                this.Text = "数字输入";
                BtnBackspace.Text = "退格";
                BtnEnter.Text = "确认";
            }
            else
            {
                this.Text = "Keyboard";
                BtnBackspace.Text = "Backspace";
                BtnEnter.Text = "Enter";
            }
        }
        #endregion

        #region 点击确认按钮
        private void BtnEnter_Click(object sender, EventArgs e)
        {
            //执行委托
            sendValue.Invoke(Code);
            //关闭窗口
            this.Close();
        }
        #endregion

        #region 点击返回按钮
        private void BtnBackspace_Click(object sender, EventArgs e)
        {
            if (Code != "")
            {
                //去掉密码字符串的最后一位
                Code = Code.Substring(0, Code.Length - 1);
            }
        }
        #endregion

        #region 键盘按下字母、数字或者小数点
        private void KeyboardInput(object sender, KeyPressEventArgs e)
        {
            //获取发送者所代表的字符
            char tempChar = e.KeyChar;
            //如果按键在可输入范围内
            if ((tempChar >= 48 && tempChar <= 57) || (tempChar >= 65 && tempChar <= 90) ||
                (tempChar >= 97 && tempChar <= 122) || tempChar == 46)
            {
                Code += tempChar;//把字符添加到密码中
                //若输入字母字符
                if ((tempChar >= 65 && tempChar <= 90) || (tempChar >= 97 && tempChar <= 122))
                {
                    //获得该字母值
                    LblLetter.Text = tempChar.ToString();
                    //字母可视
                    LblLetter.Visible = true;
                    Thread.Sleep(100);
                    LblLetter.Visible = false;

                }
                //若输入数字或者小数点
                if ((tempChar >= 48 && tempChar <= 57))
                {
                    //获得发送者按钮的位置
                    TempButton.Location  = buttonNum[tempChar - 48].Location;
                    //临时按钮可视化
                    TempButton.Visible = true;
                    Thread.Sleep(200);
                    TempButton.Visible = false;
                }
                //若输入小数点
                if (tempChar == 46)
                {
                    TempButton = buttonNum[0];
                    TempButton.Visible = true;
                    Thread.Sleep(200);
                    TempButton.Visible = false;
                }
            }
            //如果按下了返回键
            if(tempChar ==8)
            {
                if (Code != "")
                {
                    //去掉密码字符串的最后一位
                    Code = Code.Substring(0, Code.Length - 1);
                }
            }          
        }
        #endregion

        #region 鼠标按下数字按钮
        private void Button_Click(object sender, EventArgs e)
        {
            //找到请求的发送者
            Button btn = (Button)sender;
            //添加数字键盘上输入的数字
            Code += btn.Text;
            //临时按钮获取该发送者的位置坐标和内容
            TempButton.Location = btn.Location;
            //使得临时按钮可视
            TempButton.Visible = true;
            //延时
            Thread.Sleep(200);
            //使得临时按钮不可视
            TempButton.Visible = false;
        }
        #endregion
    }
}

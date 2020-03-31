using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HFM.Components
{
    class MeasureIO
    {
        /// <summary>
        /// 手脚检测交互
        /// </summary>
        /// <param name="content">交互内容</param>
        public void HFMeasureIO(object content)
        {
            string ioContent = content as string;
            //创建音频播放对象
            string appPath = Application.StartupPath;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            switch (ioContent)
            {
                case "SelfTest":
                    player.SoundLocation = appPath + "\\Audio\\Chinese_Self_checking.wav";
                    break;
                case "SelfTestFault":
                    break;
                case "BackGrouneMeasure":
                    break;
            }
            player.Play();
        }
    }
}
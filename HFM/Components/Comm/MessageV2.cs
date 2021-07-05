using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFM.Components
{
    abstract class MessageV2:Message
    {
        /// <summary>
        /// 更新红外状态
        /// 由于解析报文方法中，脚步和手部红外状态一致，
        /// 所以新增脚部、躯干红外状态后，需在解析上传测量数据报文后调用此方法更新脚部和躯干红外状态）
        /// </summary>
        /// <param name="measureDataS">解析报文后的测量数据</param>
        /// <param name="message"></param>
        /// <returns>更新红外状态后的测量数据</returns>
        public static IList<MeasureData> UpdateInfraredStatus(IList<MeasureData> measureDataS, byte[] message)
        {
            //报文信息不够则返回null
            if (message.Length <= 62)
            {
                return null;
            }
            //报文最后一个字节（C指令报文长度为124）为红外状态                       
            int infraredStatus = Convert.ToInt32(message[message.Length-1]);
            //infraredStatus报文格式(1字节由高位到低位)：保留（1bit）-躯干红外状态（1bit）-右脚红外状态（1bit）-左脚红外状态（1bit）-保留（1bit）-衣物探头开关状态（1bit）-右手红外状态（1bit）-左手红外状态（1bit）
            //数据的值 0：红外到位/衣物探头未拿起 1：红外不到位/衣物探头拿起
            //更新左脚红外状态
            if ((infraredStatus & 16) == 0)
            {
                measureDataS[4].InfraredStatus = 1;
            }
            else
            {
                measureDataS[4].InfraredStatus = 0;
            }
            //更新右脚红外状态
            if ((infraredStatus & 32) == 0)
            {
                measureDataS[5].InfraredStatus = 1;
            }
            else
            {
                measureDataS[5].InfraredStatus = 0;
            }
            //设置躯干红外状态
            if ((infraredStatus & 64) == 0)
            {
                measureDataS[6].InfraredStatus = 0;
            }
            else
            {
                measureDataS[6].InfraredStatus = 1;
            }
            return measureDataS;
        }
    }
}

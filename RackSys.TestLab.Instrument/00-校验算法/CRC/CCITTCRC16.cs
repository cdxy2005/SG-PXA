using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class CCITTCRC16
    {
        #region CRC校验的初始值
        private ushort m_InitialValue = 0;
        /// <summary>
        /// CRC校验的初始值
        /// </summary>
        public ushort InitialValue
        {
            get { return m_InitialValue; }
            set { m_InitialValue = value; }
        }
        #endregion

        #region 高位字节
        /// <summary>
        /// 校验结果的高位字节
        /// </summary>
        public byte HighByte { get; set; }
        #endregion

        #region 低位字节
        /// <summary>
        /// 校验结果的地位字节
        /// </summary>
        public byte LowByte { get; set; }
        #endregion

        #region 执行CRC校验
        /// <summary>
        /// 执行CRC校验，返回结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ushort ExecuteCheck(byte[] data)
        {
            int tmpValue = InitialValue;
            for (int i = 0; i < data.Length; i++)
            {
                tmpValue = tmpValue ^ (data[i]<<8);
                for (int j = 0; j < 8; j++)
                {
                    if ((tmpValue & 0x8000) != 0)   // 只测试最高位
                    {
                        tmpValue = (tmpValue << 1) ^ 0x1021; // 最高位为1，移位和异或处理}
                    }
                    else
                    {
                        tmpValue = tmpValue<<1; // 否则只移位（乘2）
                    }
                }
            }
            //高字节
            HighByte = (byte)(tmpValue & 0xff00);
            //低字节
            LowByte = (byte)(tmpValue & 0xff);
            return (ushort)tmpValue;
        }
        #endregion
    }
}

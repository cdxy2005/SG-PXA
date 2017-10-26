using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class Sum16
    {
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

        #region 执行校验
        /// <summary>
        /// 执行CRC校验，返回结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ushort ExecuteCheck(byte[] data)
        {
            int tmpValue = 0;
            for (int i = 0; i < data.Length; i++)
            {
                tmpValue +=  data[i];
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

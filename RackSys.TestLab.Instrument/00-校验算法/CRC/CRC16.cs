/* =============================================
* Copyright @ 2013 北京中创锐科技术有限公司 
* 名    称：CRC16Util
* 功    能：16位的CRC校验
* 作    者：wwg Administrator
* 添加时间：2014/5/15 11:43:58
* 使用介绍：
* =============================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class CRC16
    {

        #region CRC校验的初始值
        private ushort m_InitialValue = 0xffff;
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
                tmpValue = tmpValue ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    if (1 == (tmpValue & 0x01))
                    {
                        tmpValue = tmpValue >> 1;
                        tmpValue = tmpValue ^ 0xA001;
                    }
                    else
                    {
                        tmpValue = tmpValue >> 1;
                    }
                }
            }
            //高字节
            HighByte = (byte)((tmpValue & 0xff00)>>8);
            //低字节
            LowByte = (byte)(tmpValue & 0xff);
            return (ushort)tmpValue;
        }
        #endregion
        
    }
}

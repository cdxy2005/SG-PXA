/* =============================================
* Copyright @ 2013 北京中创锐科技术有限公司 
* 名    称：CheckAlgorithmLib
* 功    能：校验算法库
* 作    者：wwg Administrator
* 添加时间：2014/5/15 15:35:00
* 使用介绍：
* =============================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class CheckAlgorithmLib
    {
        #region 16位CRC
        private static CRC16 m_CRC16;
        /// <summary>
        /// 16位CRC
        /// </summary>
        public static CRC16 CRC16
        {
            get
            {
                if (object.ReferenceEquals(null, m_CRC16))
                {
                    m_CRC16 = new CRC16();
                }
                return m_CRC16;
            }
        }
        #endregion

        #region 16位 CCITTCRC
        private static CCITTCRC16 m_CCITTCRC16;
        /// <summary>
        /// 16位 CCITTCRC
        /// </summary>
        public static CCITTCRC16 CCITTCRC16
        {
            get
            {
                if (object.ReferenceEquals(null, m_CCITTCRC16))
                {
                    m_CCITTCRC16 = new CCITTCRC16();
                }
                return m_CCITTCRC16;
            }
        }
        #endregion

        #region 和校验16
        private static Sum16 m_Sum16;
        /// <summary>
        /// 16位和校验
        /// </summary>
        public static Sum16 Sum16
        {
            get 
            {
                if (object.ReferenceEquals(null, m_Sum16))
                {
                    m_Sum16 = new Sum16();
                }
                return m_Sum16;
            }
        }

        #endregion

        #region 16位异或校验
        private static XOR16 m_XOR16;
        /// <summary>
        /// 16位和校验
        /// </summary>
        public static XOR16 XOR16
        {
            get
            {
                if (object.ReferenceEquals(null, m_XOR16))
                {
                    m_XOR16 = new XOR16();
                }
                return m_XOR16;
            }
        }
        #endregion 
    }
}

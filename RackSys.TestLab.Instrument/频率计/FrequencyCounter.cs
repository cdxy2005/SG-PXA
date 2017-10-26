/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：FrequencyCounter
 * 功    能：FrequencyCounter
 * 作    者：Chen xf Administrator
 * 添加时间：2014-10-21 14:30:11
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    /// <summary>
    /// 需要根据53150的代码把53130的子类写出来
    /// </summary>
    public abstract class FrequencyCounter : ScpiInstrument
    {
        public FrequencyCounter(string inAddress)
            : base(inAddress)
        {
            this.IO.Clear();
            base.Send("*RST");
            Thread.Sleep(100);
        }

        public abstract double MeasureFrequency(int Channel);

        /// <summary>
        /// 设置输入阻抗
        /// </summary>
        /// <param name="Channel">1或2</param>
        /// <param name="impedance">50Ω or 1MΩ</param>
        public abstract void SetInputImpedance(int Channel, double impedance);

        /// <summary>
        /// 设置读数的门时间
        /// </summary>
        /// <param name="gateInSeconds">门时间，单位S</param>
        public abstract void SetGateTime(double gateInSeconds);

        public virtual void Reset()
        {
            base.Send("*RST");
        }

        public static FrequencyCounter Connect(string currentAddress)
        {
            FrequencyCounter ARFOutputMatrix = null;
            string str = (currentAddress != null ? currentAddress : "GPIB0::18::INSTR");
            try
            {
                if (FrequencyCounter.DetermineSupport(str))
                {
                    ARFOutputMatrix = FrequencyCounter.CreateDetectedDCPowerSupply(str);
                }
            }
            catch
            {
                throw;
            }
            if (ARFOutputMatrix != null)
            {
                ARFOutputMatrix.Connected = true;
            }
            return ARFOutputMatrix;
        }

        /// <summary>
        /// 判断是否可以支持对应型号的频率测量模块
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static bool DetermineSupport(string address)
        {
            FrequencyCounter AFreqCounter = null;
            try
            {
                AFreqCounter = FrequencyCounter.CreateDetectedDCPowerSupply(address);
            }
            catch
            {
                throw;
            }

            return AFreqCounter != null;
        }
        /// <summary>
        /// 创建对应的频率测量对象。
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static FrequencyCounter CreateDetectedDCPowerSupply(string address)
        {
            FrequencyCounter AFreqCounter = null;
            try
            {
                string ModelNo = ScpiInstrument.DetermineModel(address);
                if (ModelNo.IndexOf("53130") >= 0 || ModelNo.IndexOf("53132") >= 0)
                {
                    AFreqCounter = new Agilent53130(address);
                }
                else if (ModelNo.IndexOf("SR620") >= 0)
                {
                    //AFreqCounter = new SR620(address);
                }
                else
                {
                    throw new Exception(string.Concat(ModelNo, " 不是一个可以支持的频率测量模块"));
                }
            }
            catch (Exception exception)
            {
                throw new Exception(string.Concat("连接频率测量模块错误: ", exception.Message));
            }
            return AFreqCounter;
        }
    }
}
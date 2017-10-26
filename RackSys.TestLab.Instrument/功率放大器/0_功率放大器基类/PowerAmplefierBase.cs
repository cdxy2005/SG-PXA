using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public abstract class PowerAmplefierBase : ScpiInstrument
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="inAddress"></param>
        public PowerAmplefierBase(string inAddress)
            : base(inAddress)
        {
        }
        protected override void DetermineOptions()
        {
            base.m_options = "";
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="currentAddress"></param>
        /// <returns></returns>
        public static PowerAmplefierBase Connect(string currentAddress)
        {
            //读取型号创建相应的设备类

            return new PowerAmplefier_BONN_BLMA_1018_10D(currentAddress);
        }

        /// <summary>
        /// 开关
        /// </summary>
        /// <param name="isOn"></param>
        public abstract void RFOnOff(bool isOn);

        public abstract bool RFOutputEnabled
        {
            get;
        }

        /// <summary>
        /// 设置band
        /// </summary>
        /// <param name="bandId"></param>
        public abstract void SetBand(int bandId);

        /// <summary>
        /// 获取Band模式个数
        /// </summary>
        /// <returns></returns>
        public abstract int GetBands();

        /// <summary>
        /// 通过id获取band的起始值
        /// </summary>
        /// <param name="bandId">band id</param>
        /// <returns></returns>
        public abstract double GetBandStartById(int bandId);

        /// <summary>
        /// 通过id获取band的结束值
        /// </summary>
        /// <param name="bandId">band id</param>
        /// <returns></returns>
        public abstract double GetBandEndById(int bandId);


        public abstract int GetBandByFre(double Freq);

        //public abstract string QueryRFStatus();
    }
}

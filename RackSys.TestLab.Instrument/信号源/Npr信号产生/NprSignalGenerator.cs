using System;
using System.Collections.Generic;
using System.Text;
using RackSys.RFLib.Npr;

namespace RackSys.TestLab.Instrument
{
   
    /// <summary>
    /// Npr信号源对象
    /// </summary>
    public class NprSignalGenerator
    {
        /// <summary>
        /// Npr信号参数
        /// </summary>
        public class NprParameter
        {
            /// <summary>
            /// 多音数量：默认为801；
            /// </summary>
            public int m_ToneCount =  801;

            /// <summary>
            /// 信号带宽：默认为80MHz
            /// </summary>
            public double m_SignalBW = 80e6;

            /// <summary>
            /// 多音间隔
            /// </summary>
            public double  m_ToneSpacing
            {
                get
                {
                    return this.m_SignalBW / (m_ToneCount - 1);
                }
            }

            /// <summary>
            /// 缺口宽度：缺省为10%
            /// </summary>
            public double m_NotchRelativeWidth = 10;

            public bool m_CalculateNoiseOffsetAutomatically = true;

            /// <summary>
            /// 噪声偏移量
            /// </summary>
            //public double m_NoiseOffset = 0;

            public double m_CenterFreq = 1e9;

            public double m_Amplitude = -30;

            /// <summary>
            /// ALC 控制
            /// </summary>
            public bool m_AlcState = true;

            /// <summary>
            /// 相位分布
            /// </summary>
            public PhaseDistribution m_PhaseDistribution = PhaseDistribution.Random;

            /// <summary>
            /// 相位种子
            /// </summary>
            public int m_PhaseSeeds = 1;
        }
        

        /// <summary>
        /// 多音失真控制对象
        /// </summary>
        private MultitoneDistortion md;

        /// <summary>
        /// 系统设置信息 
        /// </summary>
        private SystemInfo systeminf;

        /// <summary>
        /// 内部任意波形系统对象
        /// </summary>
        private InternalArbHwSystem m_InternalArbHwSys;

        /// <summary>
        /// Npr信号对象
        /// </summary>
        private NprSignal m_NprSignal;

        /// <summary>
        /// 设备连接地址
        /// </summary>
        private string m_InstrAddress = "GPIB1::19::INSTR";

        /// <summary>
        /// 构造过程，
        /// </summary>
        /// <param name="inInstrAddress"></param>
        public NprSignalGenerator(string inInstrAddress)
        {
            this.m_InstrAddress = inInstrAddress;
        }

        /// <summary>
        /// NPR信号中心频率和信号源载波间的差距，大小等于：Npr信号中心频率 - 信号源载波频率 ，
        /// 单位：Hz；该数值在每次下载时自动更新
        /// </summary>
        private double m_CenterFreqOffsetFromSGCarrier;

        /// <summary>
        /// NPR信号中心频率和信号源载波间的差距，大小等于：Npr信号中心频率 - 信号源载波频率 ，单位：Hz
        /// </summary>
        public double CenterFreqOffsetFromSGCarrier
        {
            get { return m_CenterFreqOffsetFromSGCarrier; }
            set { m_CenterFreqOffsetFromSGCarrier = value; }
        }


        /// <summary>
        /// 仪表连接
        /// </summary>
        /// <param name="inInstrAddress"></param>
        public void Connect(string inInstrAddress = "")
        {
            if ( inInstrAddress != "")
            {
                this.m_InstrAddress = inInstrAddress;
            }

            try
            {
                //创建多音失真控制对象.
                md = new MultitoneDistortion();

                //设置工作模式为npr模式
                md.CurrentMeasurementMode = MeasurementMode.NoisePowerRatio;

                //配置系统对象
                systeminf = new SystemInfo();

                //设置信号源的地址
                systeminf.SignalGeneratorAddress = this.m_InstrAddress;

                //使用内部任意波形信号发生器
                systeminf.IQInputs = HwSystemType.InternalArb;

                //设置硬件系统信息
                md.SetHardwareSystem(systeminf);

                //获得硬件系统
                m_InternalArbHwSys = (InternalArbHwSystem)md.GetHardwareSystem();

                // 测试IO连接
                md.TestIOConnections();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 下载并回放
        /// </summary>
        public void DownLoadAndPlay(NprParameter inNprSignalParam)
        {
            //获得NPR信号对象
            m_NprSignal = (NprSignal)md.GetCurrentSignal();

            //设置npr多音数目
            m_NprSignal.ToneCount = inNprSignalParam.m_ToneCount;

            // 设置多音间隔
            m_NprSignal.ToneSpacing = inNprSignalParam.m_ToneSpacing;

            // 缺口相对位置
            m_NprSignal.NotchRelativeCenter = 0;
            ///缺口相对宽度
            m_NprSignal.NotchRelativeWidth = inNprSignalParam.m_NotchRelativeWidth;

            ///载频
            m_NprSignal.Frequency = inNprSignalParam.m_CenterFreq;
            ///幅度
            m_NprSignal.Amplitude = inNprSignalParam.m_Amplitude;

            ///alc的开关
            m_NprSignal.AlcState = inNprSignalParam.m_AlcState;

            ///波形名称
            m_NprSignal.WaveformName = "RackNprSig";

            ///
            m_NprSignal.CalculateNoiseOffsetAutomatically = inNprSignalParam.m_CalculateNoiseOffsetAutomatically;
            m_NprSignal.PhaseDistribution = inNprSignalParam.m_PhaseDistribution;
            m_NprSignal.PhaseSeed = inNprSignalParam.m_PhaseSeeds;

            //Enable the Correction
            m_NprSignal.CorrectionsEnabled = false;

            //Apply the amplitude flatness
            //ns.ApplyAmplitudeAccuracyAdjustment=true;
            m_NprSignal.IgnoreCarrier = false;

            md.TurnRFOffOnStop = true;

            ///信号下载
            md.Download();

            md.Play();

            ///载波Offset数据
            m_CenterFreqOffsetFromSGCarrier = m_NprSignal.NoiseOffset;
        }

        /// <summary>
        /// 回放
        /// </summary>
        public void Play()
        {
            if (this.m_NprSignal == null)
            {
                throw new Exception("请先调用下载过程下载对应的Npr信号");
            }

            this.md.Play();
        }

        /// <summary>
        /// 清除ARb中的内容
        /// </summary>
        public void ClearArbMemory()
        {
            this.md.ClearArbMemory();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            this.md.TurnRFOffOnStop = true;
            this.md.Stop();
        }

        /// <summary>
        /// 载频
        /// </summary>
        public double CenterFreq
        {
            get
            {
                return this.m_InternalArbHwSys.GetSignalGenerator().Frequency;
            }
            set
            {
                this.m_InternalArbHwSys.GetSignalGenerator().Frequency = value;
            }
        }

        /// <summary>
        /// 幅度
        /// </summary>
        public double Amplitude
        {
            get
            {
                return this.m_InternalArbHwSys.GetSignalGenerator().Amplitude;
            }
            set
            {
                this.m_InternalArbHwSys.GetSignalGenerator().Amplitude = value;
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RackSys.TestLab.Instrument
{
    public class SMF100a: SignalGenerator
    {
        public SMF100a(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }

        /// <summary>
        /// RF输出使能
        /// </summary>
        public override bool RFOutputEnabled
        {
            get
            {
                return base.QueryNumber("OUTP:ALL?") > 0;
            }
            set
            {
                this.Send(string.Concat("OUTP:ALL ", (value ? "ON" : "OFF")));
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// RF输出功率大小
        /// </summary>
        public override double RFPower
        {
            get
            {
                return base.QueryNumber("SOUR:POW:LEV:IMM:AMPL?");
            }
            set
            {
                ///限制最大值低于某一阀值
                if (value > this.RFPowerLimitMax)
                {
                    throw new Exception(
                        string.Format("所要设置的信号源功率电平超出了被测物激励信号保护电平，要设置的电平：{0}dBm，被测物保护电平：{1}dBm",
                        value, this.RFPowerLimitMax));
                    value = this.RFPowerLimitMax;
                }

                if (!IsSafeCall)
                {
                    throw new Exception("为保护被测物起见，不允许直接调用射频输出功率属性（SignalGenerator.RFPower），请使用DUTProtection.Sg_RFPower，实现对被测物的保护。");
                }
                else
                {
                    IsSafeCall = false;
                }

                if (value != this.RFPower)
                {
                    base.SendNumber("SOUR:POW:LEV:IMM:AMPL ", value);
                }
            }
        }



        /// <summary>
        /// 幅度偏移量
        /// </summary>
        public override double AmplOffset
        {
            get
            {
                return base.QueryNumber("SOURce:POWer:LEVel:IMMediate:OFFSet?");
            }
            set
            {
                base.SendNumber("SOURce:POWer:LEVel:IMMediate:OFFSet ", value);
            }
        }


        public override double RFFrequency
        {
            get
            {
                return base.QueryNumber("FREQ?");
            }
            set
            {
                if (value != this.RFFrequency)
                {
                    base.SendNumber("FREQ ", value);
                }
            }
        }

        /// <summary>
        /// 将模拟信号源的各设置参数实现到仪表
        /// </summary>
        /// <param name="SGsetting"></param>
        /// <returns></returns>
        public bool SignalGeratorConfigure(SignalGeneratorSetting SGsetting)
        {
            this.RFFrequency = SGsetting.Freq;
            this.WaitOpc();
            this.AmplOffset = SGsetting.Offset;
            this.WaitOpc();
            this.RFPower = SGsetting.RFPower;
            this.WaitOpc();
            this.RFOutputEnabled = SGsetting.RFOn;

            return true;
        }


        public bool SignalGeratorConfigurationRead(out SignalGeneratorSetting SGsetting)
        {
            SGsetting = new SignalGeneratorSetting();
            SGsetting.Freq = this.RFFrequency;
            this.WaitOpc();
            SGsetting.Offset = this.AmplOffset;
            this.WaitOpc();
            SGsetting.RFPower = this.RFPower;
            this.WaitOpc();
            SGsetting.RFOn = this.RFOutputEnabled;

            return true;
        }
    }
}

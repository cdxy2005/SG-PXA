using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public class Incubator: ScpiInstrument
    {
        public Incubator(string inAddress)
            : base(inAddress)
        {
        }

        public override string Identity
        {
            get { return "温箱"; }
        }

        #region 温箱连接
        /// <summary>
        /// 温箱连接
        /// </summary>
        /// <param name="currentAddress"></param>
        /// <returns></returns>
        public Incubator Connect(string currentAddress)
        {
            switch (IncubatorConfig.CurIncubatorConfig.CurIncubatorType) 
            {
                case IncubatorType.ACS_WS110C_XIAN504:
                    return new ACSIncubator(currentAddress);
                case IncubatorType.VOTSCH_7011_5XIAN504:
                    return new FUQIIncubator(currentAddress);
            }
            throw new Exception("不被支持的温箱！");
        }
        #endregion



        protected override void DetermineIdentity()
        {            
            return ;
        }

        protected override void DetermineOptions()
        {
            base.m_options = "";
        }

        public virtual bool StopChamber() 
        {
            return false;
        }

        /// <summary>
        /// 几个温箱的默认保护参数值更新程序
        /// </summary>
        /// <param name="mSafetyTemp"></param>
        /// <param name="mSafetyHumidity"></param>
        /// <param name="mDeadTimeInHour"></param>
        public virtual void SetDefaultVauleForProtection(float mSafetyTemp, float mDeadTimeInHour) 
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool StartIncubator(float ExpectedTemp) 
        {
            return false;
        }
        public virtual bool TurnoffChamber()
        {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual double Temprature
        {
            get
            {
                return -30;
            }
            set
            {

            }
        }

    }
}

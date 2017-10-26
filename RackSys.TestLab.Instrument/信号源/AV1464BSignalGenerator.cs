using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    class AV1464BSignalGenerator : SignalGenerator
    {
        public AV1464BSignalGenerator(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }

        protected override void DetermineOptions()
        {
            this.m_options = "";
        }

    }
}

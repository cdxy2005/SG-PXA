using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    class AgilentE8257D: SignalGenerator
    {
        public AgilentE8257D(string address)
            : base(address)
        {
            string str = ScpiInstrument.DetermineModel(address);
            base.GetErrorQueue();
        }

    }
}

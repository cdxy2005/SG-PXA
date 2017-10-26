using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;

namespace RackSys.TestLab.Instrument
{
    public abstract class MutiFunctionUnit : ScpiInstrument
    {
        public MutiFunctionUnit(string address)
            : base(address)
        {
        }

        public virtual double GetTunnelVoltage(int m_TunnelCode)
        { return 0; }
    }
}

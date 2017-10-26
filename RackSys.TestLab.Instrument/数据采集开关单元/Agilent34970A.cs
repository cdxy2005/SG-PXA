using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace RackSys.TestLab.Instrument
{
    public  class Agilent34970A : MutiFunctionUnit
    {
        public Agilent34970A(string inAddress)
            : base(inAddress)
        {
        }

        public override double GetTunnelVoltage(int m_TunnelCode)
        {
            return base.QueryNumber("");
        }

        
       

    }
}

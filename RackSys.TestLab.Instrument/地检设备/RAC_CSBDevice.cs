#region Author & Version
/* =============================================
 * Copyright @ 2013 北京中创锐科技术有限公司 
 * 名    称：RAC_CSBDevice 
 * 功    能：RAC_CSBDevice 
 * 作    者：Administrator 
 * 添加时间：2015-08-11 9:58:59 
 * =============================================*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RackSys.TestLab.Instrument
{
    public  class RAC_CSBDevice:CSBDevice
    {
        public RAC_CSBDevice(string inAddress)
            : base(inAddress)
        {
        }
    }
}

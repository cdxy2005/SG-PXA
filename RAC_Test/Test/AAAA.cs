/* =============================================
 * Copyright @ 2017 北京中创锐科技术有限公司 
 * 名    称：AAAA  
 * 功    能：AAAA  
 * 作    者：CHEN XF Administrator
 * 添加时间：2017/8/3 15:24:20
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAC_Test.Test
{
    class AAAA:BasicAAA
    {
        public AAAA():base()
        {
            if (IsScpiInstrument())
            {

            }
        }

        public override bool IsScpiInstrument()
        {
            return true;
        }
    }
}

/* =============================================
 * Copyright @ 2017 北京中创锐科技术有限公司 
 * 名    称：CCCC  
 * 功    能：CCCC  
 * 作    者：CHEN XF Administrator
 * 添加时间：2017/8/3 15:28:30
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAC_Test.Test
{
    class CCCC:AAAA
    {
        public CCCC():base()
        {
            if (IsScpiInstrument())
            {

            }
        }
        public override bool IsScpiInstrument()
        {
            return false;
        }
    }
}

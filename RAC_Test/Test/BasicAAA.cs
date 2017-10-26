/* =============================================
 * Copyright @ 2017 北京中创锐科技术有限公司 
 * 名    称：BasicAAA  
 * 功    能：BasicAAA  
 * 作    者：CHEN XF Administrator
 * 添加时间：2017/8/3 15:21:48
 * =============================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAC_Test.Test
{
    class BasicAAA
    {
        public BasicAAA()
        {
            if (IsScpiInstrument())
            {

            }
        }
        public virtual bool IsScpiInstrument()
        {
            return false;
        }
    }
}

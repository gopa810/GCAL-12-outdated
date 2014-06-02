using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventRelative: GPEvent
    {
        public int nSpecRef = 0;

        public override string getShortDesc()
        {
            return "Event related to other event";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventSankranti: GPEvent
    {
        public int nSankranti = 0;

        public override string getShortDesc()
        {
            return "Event based on sankranti";
        }
    }
}

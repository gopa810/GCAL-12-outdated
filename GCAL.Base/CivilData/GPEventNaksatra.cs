using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventNaksatra: GPEvent
    {
        public int nNaksatra = 0;

        public override string getShortDesc()
        {
            return "Event based on naksatra";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventTithi: GPEvent
    {
        public int nTithi;
        public int nMasa;

        public override string getShortDesc()
        {
            return "Event based on tithi and masa";
        }
    }
}

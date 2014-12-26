using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPEventYoga: GPEvent
    {
        // number of yoga
        public int nYoga;

        public override string getShortDesc()
        {
            return "Event based on yoga";
        }
    }
}

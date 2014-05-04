using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMasaDays
    {
        public int masa;
        public int year;
        public GPGregorianTime vc_start = new GPGregorianTime((GPLocation)null);
        public GPGregorianTime vc_end = new GPGregorianTime((GPLocation)null);
    }
}

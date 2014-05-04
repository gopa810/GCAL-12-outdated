using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPVSOPEntry
    {
        public double A, B, C;

        public GPVSOPEntry()
        {
        }

        public GPVSOPEntry(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
    }
}

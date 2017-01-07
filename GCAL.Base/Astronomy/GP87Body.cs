using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base.Astronomy
{
    public class GP87Body
    {
        public virtual double getLongitude(double t)
        {
            return 0;
        }

        public virtual double getLatitude(double t)
        {
            return 0;
        }

        public virtual double getRadius(double t)
        {
            return 0;
        }
    }
}

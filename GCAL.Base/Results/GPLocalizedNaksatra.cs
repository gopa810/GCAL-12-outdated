using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPLocalizedNaksatra
    {
        private int p_naksatra = 0;
        private GPGregorianTime p_initDate = null;
        private GPLocationProvider p_location = null;
        private GPGregorianTime p_startTime = null;
        private GPGregorianTime p_endTime = null;

        public GPLocalizedNaksatra(GPLocationProvider loc, GPGregorianTime start, int nNaks)
        {
            p_naksatra = nNaks;
            p_initDate = new GPGregorianTime(start);
            p_location = loc;
        }

        public GPGregorianTime getStartTime()
        {
            if (p_startTime == null)
            {
                GPNaksatra.GetPrevNaksatra(p_initDate, out p_startTime);
            }
            return p_startTime;
        }

        public void setStartTime(GPGregorianTime value)
        {
            p_startTime = new GPGregorianTime(value);
        }

        public GPGregorianTime getEndTime()
        {
            if (p_endTime == null)
            {
                GPNaksatra.GetNextNaksatra(p_initDate, out p_endTime);
            }
            return p_endTime;
        }

        public void setEndTime(GPGregorianTime value)
        {
            p_endTime = new GPGregorianTime(value);
        }

        public string getName()
        {
            return GPNaksatra.getName(p_naksatra);
        }

        public GPLocalizedNaksatra getPreviousNaksatra()
        {
            GPLocalizedNaksatra lt = new GPLocalizedNaksatra(p_location, getStartTime().TimeByAddingHours(-12.0), GPTithi.PREV_TITHI(p_naksatra));
            lt.setEndTime(getStartTime());
            return lt;
        }

        public GPLocalizedNaksatra getNextNaksatra()
        {
            GPLocalizedNaksatra lt = new GPLocalizedNaksatra(p_location, getEndTime().TimeByAddingHours(12.0), GPTithi.NEXT_TITHI(p_naksatra));
            lt.setStartTime(getEndTime());
            return lt;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPLocalizedTithi
    {
        private int p_tithi = 0;
        private GPGregorianTime p_initDate = null;
        private GPLocationProvider p_location = null;
        private GPGregorianTime p_startTime = null;
        private GPGregorianTime p_endTime = null;

        public GPLocalizedTithi(GPLocationProvider loc, GPGregorianTime start, int nTithi)
        {
            p_tithi = nTithi;
            p_initDate = new GPGregorianTime(start);
            p_location = loc;
        }

        public GPGregorianTime getStartTime()
        {
            if (p_startTime == null)
            {
                GPTithi.GetPrevTithiStart(p_initDate, out p_startTime);
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
                GPTithi.GetNextTithiStart(p_initDate, out p_endTime);
            }
            return p_endTime;
        }

        public void setEndTime(GPGregorianTime value)
        {
            p_endTime = new GPGregorianTime(value);
        }

        public string getName()
        {
            return GPTithi.getName(p_tithi);
        }

        public GPLocalizedTithi getPreviousTithi()
        {
            GPLocalizedTithi lt = new GPLocalizedTithi(p_location, getStartTime().TimeByAddingHours(-12.0), GPTithi.PREV_TITHI(p_tithi));
            lt.setEndTime(getStartTime());
            return lt;
        }

        public GPLocalizedTithi getNextTithi()
        {
            GPLocalizedTithi lt = new GPLocalizedTithi(p_location, getEndTime().TimeByAddingHours(12.0), GPTithi.NEXT_TITHI(p_tithi));
            lt.setStartTime(getEndTime());
            return lt;
        }
    }
}

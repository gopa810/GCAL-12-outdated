using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCoreEventResults
    {
        public GPGregorianTime m_vcStart;
        public GPGregorianTime m_vcEnd;
        public UInt32 m_options;
        public GPLocationProvider m_location;
        private void clear()
        {
            b_sorted = true;
            p_events.Clear();
            e_ptr = null;
            m_options = 0;
        }

        private GPCoreEvent[] e_ptr = null;
        List<GPCoreEvent> p_events = new List<GPCoreEvent>();
        public bool b_sorted;
        public IReportProgress progressReport = null;


        public List<GPStringPair> ExtractRecordsForDate(GPGregorianTime vc)
        {
            List<GPStringPair> recs = new List<GPStringPair>();

            foreach (GPCoreEvent dnr in e_ptr)
            {
                if (dnr.Time.CompareYMD(vc) == 0)
                {
                    GPStringPair rec = new GPStringPair();
                    rec.Value = dnr.Time.getLongTimeString();
                    rec.Name = dnr.getEventTitle();
                    recs.Add(rec);
                }
            }

            return recs;
        }

        private bool AddEvent(GPGregorianTime inTime, int inType, int inData)
        {
            GPCoreEvent eve = new GPCoreEvent();
            eve.Time = inTime;
            eve.nData = inData;
            eve.nType = inType;

            p_events.Add(eve);
            return true;
        }

        private bool AddEvent(GPGregorianTime inTime, int inType, string inData)
        {
            GPCoreEvent eve = new GPCoreEvent();
            eve.Time = inTime;
            eve.strData = inData;
            eve.nType = inType;

            p_events.Add(eve);
            return true;
        }

        public void Sort(bool inSort)
        {
            int i, j;
            GPCoreEvent p;

            b_sorted = inSort;
            if (inSort == false)
                return;

            int n_count = p_events.Count;
            e_ptr = new GPCoreEvent[p_events.Count];

            for (i = 0; i < n_count; i++)
                e_ptr[i] = p_events[i];

            for (i = 0; i < n_count - 1; i++)
            {
                for (j = i + 1; j < n_count; j++)
                {
                    if ((e_ptr[i].Time.getDayInteger() + e_ptr[i].Time.getDayHours() + e_ptr[i].getDstFlag() / 24.0)
                        > (e_ptr[j].Time.getDayInteger() + e_ptr[j].Time.getDayHours() + e_ptr[j].getDstFlag() / 24.0))
                    {
                        p = e_ptr[i];
                        e_ptr[i] = e_ptr[j];
                        e_ptr[j] = p;
                    }
                }
            }
        }

        public int getCount()
        {
            return p_events.Count;
        }

        public GPCoreEvent get(int i)
        {
            if (b_sorted)
            {
                if (e_ptr == null)
                    Sort(true);
                return e_ptr[i];
            }
            else
            {
                return p_events[i];
            }
        }

        public GPCoreEventResults()
        {
            clear();
        }

        public void CalculateEvents(GPLocationProvider loc, GPGregorianTime vcStart, GPGregorianTime vcEnd)
        {
            GPCoreEventResults inEvents = this;
            GPLocationProvider earth = loc;
            GPGregorianTime vc = new GPGregorianTime(loc);
            GPSun sun = new GPSun();
            //int ndst = 0;
            int nData;

            inEvents.clear();
            inEvents.m_location = loc;
            inEvents.m_vcStart = vcStart;
            inEvents.m_vcEnd = vcEnd;

            GPGregorianTime vcAdd = new GPGregorianTime(loc);
            GPGregorianTime vcTemp = null;
            GPGregorianTime vcNext = new GPGregorianTime(loc);

            vc.Copy(vcStart);

            if (GPDisplays.CoreEvents.Sunrise())
            {
                vcAdd.Copy(vc);
                while (vcAdd.IsBeforeThis(vcEnd))
                {
                    sun.SunCalc(vcAdd, earth);

                    vcTemp = new GPGregorianTime(sun.arunodaya);
                    inEvents.AddEvent(vcTemp, GPConstants.CCTYPE_S_ARUN, 0);

                    //GPJulianTime tr, tt, ts;
                    //GPAstroEngine.CalculateTimeSun(vcTemp, vcTemp.getLocation(), out tr, out tt, out ts);
                    vcTemp = new GPGregorianTime(sun.rise);
                    //vcTemp = new GPGregorianTime(vcTemp.getLocation(), tr);
                    inEvents.AddEvent(vcTemp, GPConstants.CCTYPE_S_RISE, 0);

                    vcTemp = new GPGregorianTime(sun.noon);
                    //vcTemp = new GPGregorianTime(vcTemp.getLocation(), tt);
                    inEvents.AddEvent(vcTemp, GPConstants.CCTYPE_S_NOON, 0);

                    vcTemp = new GPGregorianTime(sun.set);
                    //vcTemp = new GPGregorianTime(vcTemp.getLocation(), ts);
                    inEvents.AddEvent(vcTemp, GPConstants.CCTYPE_S_SET, 0);

                    vcAdd.NextDay();
                }
            }

            if (GPDisplays.CoreEvents.Tithi())
            {
                GPTithi te = new GPTithi();
                te.setStartDate(vc);
                vcAdd = te.getNext();
                while (vcAdd.IsBeforeThis(vcEnd))
                {
                    nData = te.getCurrentPosition();
                    //ndst = loc.getTimeZone().GetDaylightChangeType(vcNext);
                    inEvents.AddEvent(vcAdd, GPConstants.CCTYPE_TITHI, nData);
                    vcAdd = te.getNext();
                }
            }

            if (GPDisplays.CoreEvents.Naksatra())
            {
                GPNaksatra te = new GPNaksatra();
                te.setStartDate(vc);
                vcAdd = te.getNext();
                while (vcAdd.IsBeforeThis(vcEnd))
                {
                    nData = te.getCurrentNaksatra();
                    //ndst = loc.getTimeZone().GetDaylightChangeType(vcNext);
                    inEvents.AddEvent(vcAdd, GPConstants.CCTYPE_NAKS, nData);
                    vcAdd = te.getNext();
                }
            }

            if (GPDisplays.CoreEvents.Sankranti())
            {
                GPSankranti te = new GPSankranti();
                te.setStartDate(vc);
                vcAdd = te.getNext();
                while (vcAdd.IsBeforeThis(vcEnd))
                {
                    nData = te.getCurrentPosition();
                    //ndst = loc.getTimeZone().GetDaylightChangeType(vcNext);
                    inEvents.AddEvent(vcAdd, GPConstants.CCTYPE_SANK, nData);
                    vcAdd = te.getNext();
                }
            }

            if (GPDisplays.CoreEvents.Conjunction())
            {
                double[] times = null;
                GPConjunction te = new GPConjunction();
                te.setStartDate(vc);
                vcAdd = te.getNext();
                while (vcAdd.IsBeforeThis(vcEnd))
                {
                    nData = te.getCurrentPosition();
                    //ndst = loc.getTimeZone().GetDaylightChangeType(vcNext);
                    inEvents.AddEvent(vcAdd, GPConstants.CCTYPE_CONJ, nData);

                    if (GPDisplays.CoreEvents.SunEclipse())
                    {
                        GPAstroEngine.CalculateTimesSunEclipse(vcAdd.getJulianGreenwichTime(), vcAdd.getLocation(), out times);
                        if (times != null && times[2] > 0)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (times[i] > 0)
                                {
                                    GPGregorianTime gt = new GPGregorianTime(vcAdd.getLocation());
                                    gt.setJulianGreenwichTime(new GPJulianTime(times[i], 0.0));
                                    inEvents.AddEvent(gt, GPConstants.SUNECLIPSE_CONSTS[i], 0);
                                }
                            }
                        }
                    }

                    vcAdd = te.getNext();
                }
            }


            // moon eclipses
            if (GPDisplays.CoreEvents.MoonEclipse())
            {
                double[] times = null;
                GPConjunction te = new GPConjunction();
                te.setOpositeConjunction(true);
                te.setStartDate(vc);
                vcAdd = te.getNext();
                while (vcAdd.IsBeforeThis(vcEnd))
                {
                    GPAstroEngine.CalculateTimesMoonEclipse(vcAdd.getJulianGreenwichTime(), vcAdd.getLocation(), out times);
                    if (times != null && times[4] > 0)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (times[i] > 0 && GPConstants.MOONECLIPSE_CONSTS[i] > 0)
                            {
                                GPGregorianTime gt = new GPGregorianTime(vcAdd.getLocation());
                                gt.setJulianGreenwichTime(new GPJulianTime(times[i], 0.0));
                                inEvents.AddEvent(gt, GPConstants.MOONECLIPSE_CONSTS[i], 0);
                            }
                        }
                    }

                    vcAdd = te.getNext();
                }
            }

            // rise and set of the moon
            if (GPDisplays.CoreEvents.Moonrise())
            {
                GPJulianTime julian = vc.getJulian();
                GPJulianTime julianEnd = vcEnd.getJulian();
                GPJulianTime nextJulian;
                TRiseSet kind;

                while (julian.getGreenwichJulianDay() < julianEnd.getGreenwichJulianDay())
                {
                    nextJulian = GPAstroEngine.GetNextMoonEvent(julian, vc.getLocationProvider(), out kind);
                    if (kind == TRiseSet.RISE)
                    {
                        inEvents.AddEvent(new GPGregorianTime(loc, nextJulian), GPConstants.CoreEventMoonRise, 0);
                    }
                    else if (kind == TRiseSet.SET)
                    {
                        inEvents.AddEvent(new GPGregorianTime(loc, nextJulian), GPConstants.CoreEventMoonSet, 0);
                    }
                    julian.setGreenwichJulianDay(nextJulian.getGreenwichJulianDay() + 10.0 / 1440.0);
                }
            }

            // travellings
            {
                GPJulianTime julian = vc.getJulian();
                GPJulianTime julianEnd = vcEnd.getJulian();
                double start, end;

                start = julian.getGreenwichJulianDay();
                end = julianEnd.getGreenwichJulianDay();

                for(int i = 0; i < loc.getChangeCount(); i++)
                {
                    GPLocationChange chn = loc.getChangeAtIndex(i);
                    if ((chn.julianStart >= start && chn.julianStart <= end) 
                        || (chn.julianStart >= start && chn.julianEnd <= end))
                    {
                        GPGregorianTime startTime = new GPGregorianTime(chn.LocationA);
                        startTime.setJulianGreenwichTime(new GPJulianTime(chn.julianStart, 0));
                        GPGregorianTime endTime = new GPGregorianTime(chn.LocationB);
                        endTime.setJulianGreenwichTime(new GPJulianTime(chn.julianEnd, 0));
                        inEvents.AddEvent(startTime, GPConstants.CCTYPE_TRAVELLING_START, 0);
                        inEvents.AddEvent(endTime, GPConstants.CCTYPE_TRAVELLING_END, 0);
                    }
                }
            }

            // eventual sorting
            inEvents.Sort(GPDisplays.CoreEvents.Sort());
        }

        public List<GPLocation> getLocationList()
        {
            List<GPLocation> locList = new List<GPLocation>();

            double start = m_vcStart.getJulianGreenwichTime();
            double end = m_vcEnd.getJulianGreenwichTime();

            if (m_location.getChangeCount() > 0)
            {
                for (int i = 0; i < m_location.getChangeCount(); i++)
                {
                    GPLocationChange lc = m_location.getChangeAtIndex(i);

                    if ((lc.julianStart >= start && lc.julianStart <= end) &&
                        (lc.julianEnd >= start && lc.julianEnd <= end))
                    {
                        addLocationToList(lc.LocationA, locList);
                        addLocationToList(lc.LocationB, locList);
                    }
                }
            }
            else
            {
                addLocationToList(m_location.getLocationAtIndex(0), locList);
            }

            return locList;
        }

        private void addLocationToList(GPLocation lc, List<GPLocation> list)
        {
            if (list.IndexOf(lc) < 0)
                list.Add(lc);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCalendarResults
    {
        public GPCalendarDay[] m_pData = null;
        public int m_nCount;
        public int m_PureCount;
        public GPLocationProvider m_Location = null;
        public GPGregorianTime m_vcStart;
        public int m_vcCount;
        public IReportProgress progressReport = null;

        public void ResolveFestivalsFasting(int nIndex)
        {
            GPCalendarDay previousDay = m_pData[nIndex - 1];
            GPCalendarDay currentDay = m_pData[nIndex];
            GPCalendarDay nextDay = m_pData[nIndex + 1];

            int fastItemIndex = 0;
            int fastTypeOfEvent;
            GPCalendarDay.Festival fastingItem = null;
            String str;
            String subject;
            int fastTypeOfDay = 0;
            string ch;

            if (currentDay.nMahadvadasiType != GPConstants.EV_NULL)
            {
                // begin of inserting "total fast even from water..."
                // in case of pandava nirjala and old style fasting
                if (currentDay.sEkadasiVrataName == GPStrings.getSharedStrings().getString(563)
                    && GPDisplays.General.OldStyleFasting())
                    currentDay.Festivals.Insert(0, new GPCalendarDay.Festival(11, GPStrings.getSharedStrings().getString(173)));
                // end of inserting "total fast even from water..." text

                str = string.Format(GPStrings.getSharedStrings().getString(87), currentDay.sEkadasiVrataName);
                currentDay.Festivals.Insert(0, new GPCalendarDay.Festival(10, str));
            }

            ch = GPAppHelper.GetMahadvadasiName(currentDay.nMahadvadasiType);
            if (ch != null)
            {
                currentDay.Festivals.Insert(0, new GPCalendarDay.Festival(8, ch));
            }

            // analyze for fasting
            fastItemIndex = currentDay.GetFastingItemIndex();
            while (fastItemIndex >= 0)
            {
                fastingItem = currentDay.Festivals[fastItemIndex];
                // getting the type of fast
                fastTypeOfEvent = fastingItem.getFastType();
                subject = (fastingItem.FastSubject != null ? fastingItem.FastSubject : string.Empty);

                // resolving fast
                if (fastTypeOfEvent > GPConstants.FAST_NULL)
                {
                    if (previousDay.nFastType == GPConstants.FAST_EKADASI)
                    {

                        if (!GPDisplays.General.OldStyleFasting())
                        {
                            previousDay.Festivals.Add(new GPCalendarDay.Festival(100, string.Format(GPStrings.getSharedStrings().getString(960), subject)));
                            currentDay.Festivals.Insert(fastItemIndex + 1, new GPCalendarDay.Festival(fastingItem.SortKey + 1, GPStrings.getSharedStrings().getString(860)));
                        }
                        else
                        {
                            previousDay.Festivals.Add(new GPCalendarDay.Festival(100, string.Format(GPStrings.getSharedStrings().getString(961), subject)));
                            currentDay.Festivals.Insert(fastItemIndex + 1, new GPCalendarDay.Festival(fastingItem.SortKey + 1, GPStrings.getSharedStrings().getString(861)));
                        }
                        fastingItem.setFastType(GPConstants.FAST_NULL);
                    }
                    else if (currentDay.nFastType == GPConstants.FAST_EKADASI)
                    {
                        if (GPDisplays.General.OldStyleFasting())
                            currentDay.Festivals.Insert(fastItemIndex + 1, new GPCalendarDay.Festival(fastingItem.SortKey + 1, GPStrings.getSharedStrings().getString(862)));//"(Fasting till noon, with feast tomorrow)";
                        else
                            currentDay.Festivals.Insert(fastItemIndex + 1, new GPCalendarDay.Festival(fastingItem.SortKey + 1, GPStrings.getSharedStrings().getString(936)));//"(Fast today)"
                        fastingItem.setFastType(GPConstants.FAST_NULL);
                    }
                    else
                    {
                        if (!GPDisplays.General.OldStyleFasting())
                        {
                            if (fastTypeOfEvent > GPConstants.FAST_NOON)
                                fastTypeOfEvent = GPConstants.FAST_DAY;
                            else fastTypeOfEvent = GPConstants.FAST_NULL;
                        }
                        if (fastTypeOfEvent != GPConstants.FAST_NULL)
                            currentDay.Festivals.Insert(fastItemIndex + 1, new GPCalendarDay.Festival(fastingItem.SortKey + 1, GPAppHelper.GetFastingName(fastTypeOfEvent)));
                        fastingItem.setFastType(GPConstants.FAST_NULL);
                    }
                }
                if (fastTypeOfDay < fastTypeOfEvent)
                    fastTypeOfDay = fastTypeOfEvent;

                // get next fasting item
                fastItemIndex = currentDay.GetFastingItemIndex();
            }

            // set fasting flag if:
            //   - new flag is not NULL
            //   - today is not Ekadasi (when Ekadasi, this is primary fasting type)
            //   - yesterday is not Ekadashi (we can't do fasting day after Ekadasi)
            if (fastTypeOfDay != GPConstants.FAST_NULL
                && previousDay.nFastType != GPConstants.FAST_EKADASI
                && currentDay.nFastType != GPConstants.FAST_EKADASI)
            {
                currentDay.nFastType = fastTypeOfDay;
            }

        }

        public bool AddSpecFestival(GPCalendarDay day, int nSpecialFestival, int nClass)
        {
            GPEvent pevent = GPEventList.getShared().GetSpecialEvent(nSpecialFestival);
            if (pevent != null)
            {
                day.AddFestivalCopy(pevent);
            }

            return pevent != null;
        }

        protected double GcGetNaksatraEndHour(GPLocationProvider earth, GPGregorianTime yesterday, GPGregorianTime today)
        {
            GPGregorianTime nend;
            GPGregorianTime snd = yesterday;
            snd.setDayHours(0.5);
            GPNaksatra.GetNextNaksatra(snd, out nend);
            return nend.getJulianLocalNoon() - today.getJulianLocalNoon() + nend.getDayHours();
        }

        /* Function is writen accoring this algorithm:


        1. Normal - fasting day has ekadasi at sunrise and dvadasi at next sunrise.

        2. Viddha - fasting day has dvadasi at sunrise and trayodasi at next
        sunrise, and it is not a naksatra mahadvadasi

        3. Unmilani - fasting day has ekadasi at both sunrises

        4. Vyanjuli - fasting day has dvadasi at both sunrises, and it is not a
        naksatra mahadvadasi

        5. Trisprsa - fasting day has ekadasi at sunrise and trayodasi at next
        sunrise.

        6. Jayanti/Vijaya - fasting day has gaura dvadasi and specified naksatra at
        sunrise and same naksatra at next sunrise

        7. Jaya/Papanasini - fasting day has gaura dvadasi and specified naksatra at
        sunrise and same naksatra at next sunrise

        ==============================================
        Case 1 Normal (no change)

        If dvadasi tithi ends before 1/3 of daylight
           then PARANA END = TIME OF END OF TITHI
        but if dvadasi TITHI ends after 1/3 of daylight
           then PARANA END = TIME OF 1/3 OF DAYLIGHT

        if 1/4 of dvadasi tithi is before sunrise
           then PARANA BEGIN is sunrise time
        but if 1/4 of dvadasi tithi is after sunrise
           then PARANA BEGIN is time of 1/4 of dvadasi tithi

        if PARANA BEGIN is before PARANA END
           then we will write "BREAK FAST FROM xx TO yy
        but if PARANA BEGIN is after PARANA END
           then we will write "BREAK FAST AFTER xx"

        ==============================================
        Case 2 Viddha

        If trayodasi tithi ends before 1/3 of daylight
           then PARANA END = TIME OF END OF TITHI
        but if trayodasi TITHI ends after 1/3 of daylight
           then PARANA END = TIME OF 1/3 OF DAYLIGHT

        PARANA BEGIN is sunrise time

        we will write "BREAK FAST FROM xx TO yy

        ==============================================
        Case 3 Unmilani

        PARANA END = TIME OF 1/3 OF DAYLIGHT

        PARANA BEGIN is end of Ekadasi tithi

        if PARANA BEGIN is before PARANA END
           then we will write "BREAK FAST FROM xx TO yy
        but if PARANA BEGIN is after PARANA END
           then we will write "BREAK FAST AFTER xx"

        ==============================================
        Case 4 Vyanjuli

        PARANA BEGIN = Sunrise

        PARANA END is end of Dvadasi tithi

        we will write "BREAK FAST FROM xx TO yy

        ==============================================
        Case 5 Trisprsa

        PARANA BEGIN = Sunrise

        PARANA END = 1/3 of daylight hours

        we will write "BREAK FAST FROM xx TO yy

        ==============================================
        Case 6 Jayanti/Vijaya

        PARANA BEGIN = Sunrise

        PARANA END1 = end of dvadasi tithi or sunrise, whichever is later
        PARANA END2 = end of naksatra

        PARANA END is earlier of END1 and END2

        we will write "BREAK FAST FROM xx TO yy

        ==============================================
        Case 7 Jaya/Papanasini

        PARANA BEGIN = end of naksatra

        PARANA END = 1/3 of Daylight hours

        if PARANA BEGIN is before PARANA END
           then we will write "BREAK FAST FROM xx TO yy
        but if PARANA BEGIN is after PARANA END
           then we will write "BREAK FAST AFTER xx"



          */

        public void CalculateEParana(GPCalendarDay s, GPCalendarDay t, GPLocationProvider earth)
        {
            t.nMahadvadasiType = GPConstants.EV_NULL;
            t.nFastType = GPConstants.FAST_NULL;

            double startOfTithi, endOfTithi, theOneFourthOfTithi;
            double sunRise, theOneThirdOfDaylight, endOfNaksatra;
            double startOfParana = -1.0, endOfParana = -1.0;
            double lengthOfTithi;

            sunRise = t.astrodata.sun.getSunriseDayHours();
            theOneThirdOfDaylight = sunRise + (t.astrodata.sun.set.getJulianGreenwichTime() - t.astrodata.sun.rise.getJulianGreenwichTime())/3;
            lengthOfTithi = GPTithi.GetTithiTimes(t.date, out startOfTithi, out endOfTithi, sunRise);
            theOneFourthOfTithi = lengthOfTithi / 4.0 + startOfTithi;

            switch (s.nMahadvadasiType)
            {
                case GPConstants.EV_UNMILANI:
                    endOfParana = endOfTithi;
                    t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                    if (endOfParana > theOneThirdOfDaylight)
                    {
                        endOfParana = theOneThirdOfDaylight;
                        t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                    }
                    startOfParana = sunRise;
                    t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                    break;
                case GPConstants.EV_VYANJULI:
                    startOfParana = sunRise;
                    t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                    endOfParana = Math.Min(endOfTithi, theOneThirdOfDaylight);
                    if (endOfParana == endOfTithi)
                        t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                    else
                        t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                    break;
                case GPConstants.EV_TRISPRSA:
                    startOfParana = sunRise;
                    endOfParana = theOneThirdOfDaylight;
                    t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                    t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                    break;
                case GPConstants.EV_JAYANTI:
                case GPConstants.EV_VIJAYA:

                    endOfNaksatra = GcGetNaksatraEndHour(earth, s.date, t.date); //GetNextNaksatra(earth, snd, nend);
                    if (GPTithi.TITHI_DVADASI(t.astrodata.nTithi))
                    {
                        if (endOfNaksatra < endOfTithi)
                        {
                            if (endOfNaksatra < theOneThirdOfDaylight)
                            {
                                startOfParana = endOfNaksatra;
                                t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_NAKEND;
                                endOfParana = Math.Min(endOfTithi, theOneThirdOfDaylight);
                                if (endOfParana == endOfTithi)
                                    t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                                else
                                    t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                            }
                            else
                            {
                                startOfParana = endOfNaksatra;
                                t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_NAKEND;
                                endOfParana = endOfTithi;
                                t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                            }
                        }
                        else
                        {
                            startOfParana = sunRise;
                            t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                            endOfParana = Math.Min(endOfTithi, theOneThirdOfDaylight);
                            if (endOfParana == endOfTithi)
                                t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                            else
                                t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                        }
                    }
                    else
                    {
                        startOfParana = sunRise;
                        t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                        endOfParana = Math.Min(endOfNaksatra, theOneThirdOfDaylight);
                        if (endOfParana == endOfNaksatra)
                            t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_NAKEND;
                        else
                            t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                    }

                    break;
                case GPConstants.EV_JAYA:
                case GPConstants.EV_PAPA_NASINI:

                    endOfNaksatra = GcGetNaksatraEndHour(earth, s.date, t.date); //GetNextNaksatra(earth, snd, nend);

                    if (GPTithi.TITHI_DVADASI(t.astrodata.nTithi))
                    {
                        if (endOfNaksatra < endOfTithi)
                        {
                            if (endOfNaksatra < theOneThirdOfDaylight)
                            {
                                startOfParana = endOfNaksatra;
                                t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_NAKEND;
                                endOfParana = Math.Min(endOfTithi, theOneThirdOfDaylight);
                                if (endOfParana == endOfTithi)
                                    t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                                else
                                    t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                            }
                            else
                            {
                                startOfParana = endOfNaksatra;
                                t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_NAKEND;
                                endOfParana = endOfTithi;
                                t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                            }
                        }
                        else
                        {
                            startOfParana = sunRise;
                            t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                            endOfParana = Math.Min(endOfTithi, theOneThirdOfDaylight);
                            if (endOfParana == endOfTithi)
                                t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                            else
                                t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                        }
                    }
                    else
                    {
                        if (endOfNaksatra < theOneThirdOfDaylight)
                        {
                            startOfParana = endOfNaksatra;
                            t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_NAKEND;
                            endOfParana = theOneThirdOfDaylight;
                            t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                        }
                        else
                        {
                            startOfParana = endOfNaksatra;
                            t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_NAKEND;
                            endOfParana = -1.0;
                            t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_NULL;
                        }
                    }

                    break;
                default:
                    // first initial
                    endOfParana = Math.Min(endOfTithi, theOneThirdOfDaylight);
                    if (endOfParana == endOfTithi)
                        t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_TEND;
                    else
                        t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_3DAY;
                    startOfParana = Math.Max(sunRise, theOneFourthOfTithi);
                    if (startOfParana == sunRise)
                        t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                    else
                        t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_4TITHI;

                    if (GPTithi.TITHI_DVADASI(s.astrodata.nTithi))
                    {
                        startOfParana = sunRise;
                        t.EkadasiParanaReasonStart = GPConstants.EP_TYPE_SUNRISE;
                    }

                    //if (parBeg > third_day)
                    if (startOfParana > endOfParana)
                    {
                        //			parBeg = sunRise;
                        endOfParana = -1.0;
                        t.EkadasiParanaReasonEnd = GPConstants.EP_TYPE_NULL;
                    }
                    break;
            }


            //begin = parBeg;
            //end = parEnd;

            if (startOfParana > 0.0)
            {
                t.ekadasiParanaStart = new GPGregorianTime(t.date);
                t.ekadasiParanaStart.setDayHours(startOfParana);
            }
            if (endOfParana > 0.0)
            {
                t.ekadasiParanaEnd = new GPGregorianTime(t.date);
                t.ekadasiParanaEnd.setDayHours(endOfParana);
            }
        }

        public int FindDate(GPGregorianTime vc)
        {
            int i;
            for (i = BEFORE_DAYS; i < m_nCount; i++)
            {
                if ((m_pData[i].date.getDay() == vc.getDay()) && (m_pData[i].date.getMonth() == vc.getMonth()) && (m_pData[i].date.getYear() == vc.getYear()))
                    return (i - BEFORE_DAYS);
            }

            return -1;
        }

        /******************************************************************************************/
        /*                                                                                        */
        /*  TEST if today is given festival tithi                                                 */
        /*                                                                                        */
        /*  if today is given tithi and yesterday is not this tithi                               */
        /*  then it is festival day (it is first day of this tithi, when vriddhi)                 */
        /*                                                                                        */
        /*  if yesterday is previous tithi to the given one and today is next to the given one    */
        /*  then today is day after ksaya tithi which is given                                    */
        /*                                                                                        */
        /*                                                                                        */
        /******************************************************************************************/

        public bool IsFestivalDay(GPCalendarDay yesterday, GPCalendarDay today, int nTithi)
        {
            return ((today.astrodata.nTithi == nTithi) && GPTithi.TITHI_LESS_THAN(yesterday.astrodata.nTithi, nTithi))
                    || (GPTithi.TITHI_LESS_THAN(yesterday.astrodata.nTithi, nTithi) && GPTithi.TITHI_GREAT_THAN(today.astrodata.nTithi, nTithi));
        }

        public GPCalendarDay get(int nIndex)
        {
            int nReturn = nIndex + BEFORE_DAYS;

            if (nReturn >= m_nCount)
                return null;

            return m_pData[nReturn];
        }

        public int getCount()
        {
            return m_PureCount;
        }

        public int MahadvadasiCalc(int nIndex, GPLocationProvider earth)
        {
            int nMahaType = 0;
            int nMhdDay = -1;

            GPCalendarDay s = m_pData[nIndex - 1];
            GPCalendarDay t = m_pData[nIndex];
            GPCalendarDay u = m_pData[nIndex + 1];

            // if yesterday is dvadasi
            // then we skip this day
            if (GPTithi.TITHI_DVADASI(s.astrodata.nTithi))
                return 1;

            if (GPTithi.TITHI_GAURA_DVADASI == t.astrodata.nTithi && GPTithi.TITHI_GAURA_DVADASI == t.astrodata.getTithiAtSunset() && IsMhd58(nIndex, out nMahaType))
            {
                t.nMahadvadasiType = nMahaType;
                nMhdDay = nIndex;
            }
            else if (GPTithi.TITHI_DVADASI(t.astrodata.nTithi))
            {
                if (GPTithi.TITHI_DVADASI(u.astrodata.nTithi) && GPTithi.TITHI_EKADASI(s.astrodata.nTithi) && GPTithi.TITHI_EKADASI(s.astrodata.getTithiAtArunodaya()))
                {
                    t.nMahadvadasiType = GPConstants.EV_VYANJULI;
                    nMhdDay = nIndex;
                }
                else if (NextNewFullIsVriddhi(nIndex, earth))
                {
                    t.nMahadvadasiType = GPConstants.EV_PAKSAVARDHINI;
                    nMhdDay = nIndex;
                }
                else if (GPTithi.TITHI_LESS_EKADASI(s.astrodata.getTithiAtArunodaya()))
                {
                    t.nMahadvadasiType = GPConstants.EV_SUDDHA;
                    nMhdDay = nIndex;
                }
            }

            if (nMhdDay >= 0)
            {
                // fasting day
                m_pData[nMhdDay].nFastType = GPConstants.FAST_EKADASI;
                m_pData[nMhdDay].sEkadasiVrataName = GPAppHelper.GetEkadasiName(t.astrodata.nMasa, t.astrodata.nPaksa);
                m_pData[nMhdDay].ekadasiParanaStart = null;
                m_pData[nMhdDay].ekadasiParanaEnd = null;

                // parana day
                m_pData[nMhdDay + 1].nFastType = GPConstants.FAST_NULL;
                m_pData[nMhdDay + 1].ekadasiParanaStart = null;
                m_pData[nMhdDay + 1].ekadasiParanaEnd = null;
            }

            return 1;
        }

        public int CompleteCalc(int nIndex, GPLocationProvider earth)
        {
            GPCalendarDay s = m_pData[nIndex - 1];
            GPCalendarDay t = m_pData[nIndex];
            GPCalendarDay u = m_pData[nIndex + 1];
            GPCalendarDay v = m_pData[nIndex + 2];
            GPCalendarDay.Festival newFestival = null;

            // test for Govardhan-puja
            if (t.astrodata.nMasa == GPMasa.DAMODARA_MASA)
            {
                if (t.astrodata.nTithi == GPTithi.TITHI_GAURA_PRATIPAT)
                {
                    GPMoon.CalcMoonTimes(m_Location, u.date, out s.moonrise, out s.moonset);
                    GPMoon.CalcMoonTimes(m_Location, t.date, out t.moonrise, out t.moonset);
                    if (s.astrodata.nTithi == GPTithi.TITHI_GAURA_PRATIPAT)
                    {
                    }
                    else if (u.astrodata.nTithi == GPTithi.TITHI_GAURA_PRATIPAT)
                    {
                        if (t.moonrise != null)
                        {
                            if (t.moonrise.getDayHours() > t.astrodata.sun.rise.getDayHours())
                                // today is GOVARDHANA PUJA
                                AddSpecFestival(t, GPConstants.SPEC_GOVARDHANPUJA, 1);
                            else
                                AddSpecFestival(u, GPConstants.SPEC_GOVARDHANPUJA, 1);
                        }
                        else if (u.moonrise != null)
                        {
                            if (u.moonrise.getDayHours() < u.astrodata.sun.rise.getDayHours())
                                // today is GOVARDHANA PUJA
                                AddSpecFestival(t, GPConstants.SPEC_GOVARDHANPUJA, 1);
                            else
                                AddSpecFestival(u, GPConstants.SPEC_GOVARDHANPUJA, 1);
                        }
                        else
                        {
                            AddSpecFestival(t, GPConstants.SPEC_GOVARDHANPUJA, 1);
                        }
                    }
                    else
                    {
                        // today is GOVARDHANA PUJA
                        AddSpecFestival(t, GPConstants.SPEC_GOVARDHANPUJA, 1);
                    }

                }
                else if ((t.astrodata.nTithi == GPTithi.TITHI_GAURA_DVITIYA) && (s.astrodata.nTithi == GPTithi.TITHI_AMAVASYA))
                {
                    // today is GOVARDHANA PUJA
                    AddSpecFestival(t, GPConstants.SPEC_GOVARDHANPUJA, 1);
                }
            }

            int mid_nak_t, mid_nak_u;

            if (t.astrodata.nMasa == GPMasa.HRSIKESA_MASA)
            {
                // test for Janmasthami
                if (IsFestivalDay(s, t, GPTithi.TITHI_KRSNA_ASTAMI))
                {
                    // if next day is not astami, so that means that astami is not vriddhi
                    // then today is SKJ
                    if (u.astrodata.nTithi != GPTithi.TITHI_KRSNA_ASTAMI)
                    {
                        // today is Sri Krsna Janmasthami
                        AddSpecFestival(t, GPConstants.SPEC_JANMASTAMI, 0);
                        //AddSpecFestival(u, GPConstants.SPEC_NANDAUTSAVA, 1);
                        //AddSpecFestival(u, GPConstants.SPEC_PRABHAPP, 2);
                        //				t.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                    }
                    else // tithi is vriddhi and we have to test both days
                    {
                        // test when both days have ROHINI
                        if ((t.astrodata.nNaksatra == GPNaksatra.ROHINI_NAKSATRA) && (u.astrodata.nNaksatra == GPNaksatra.ROHINI_NAKSATRA))
                        {
                            mid_nak_t = GPAstroData.calculateNaksatraAtMidnight(t.date, earth);
                            mid_nak_u = GPAstroData.calculateNaksatraAtMidnight(u.date, earth);

                            // test when both days have modnight naksatra ROHINI
                            if ((GPNaksatra.ROHINI_NAKSATRA == mid_nak_u) && (mid_nak_t == GPNaksatra.ROHINI_NAKSATRA))
                            {
                                // choice day which is monday or wednesday
                                if ((u.date.getDayOfWeek() == GPConstants.DW_MONDAY) || (u.date.getDayOfWeek() == GPConstants.DW_WEDNESDAY))
                                {
                                    AddSpecFestival(u, GPConstants.SPEC_JANMASTAMI, 0);
                                    //AddSpecFestival(v, GPConstants.SPEC_NANDAUTSAVA, 1);
                                    //AddSpecFestival(v, GPConstants.SPEC_PRABHAPP, 2);
                                    //							u.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                                }
                                else
                                {
                                    // today is Sri Krsna Janmasthami
                                    AddSpecFestival(t, GPConstants.SPEC_JANMASTAMI, 0);
                                    //AddSpecFestival(u, GPConstants.SPEC_NANDAUTSAVA, 1);
                                    //AddSpecFestival(u, GPConstants.SPEC_PRABHAPP, 2);
                                    //							t.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                                }
                            }
                            else if (mid_nak_t == GPNaksatra.ROHINI_NAKSATRA)
                            {
                                // today is Sri Krsna Janmasthami
                                AddSpecFestival(t, GPConstants.SPEC_JANMASTAMI, 0);
                                //AddSpecFestival(u, GPConstants.SPEC_NANDAUTSAVA, 1);
                                //						t.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                                //AddSpecFestival(u, GPConstants.SPEC_PRABHAPP, 2);
                            }
                            else if (mid_nak_u == GPNaksatra.ROHINI_NAKSATRA)
                            {
                                AddSpecFestival(u, GPConstants.SPEC_JANMASTAMI, 0);
                                //AddSpecFestival(v, GPConstants.SPEC_NANDAUTSAVA, 1);
                                //AddSpecFestival(v, GPConstants.SPEC_PRABHAPP, 2);
                                //						u.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                            }
                            else
                            {
                                if ((u.date.getDayOfWeek() == GPConstants.DW_MONDAY) || (u.date.getDayOfWeek() == GPConstants.DW_WEDNESDAY))
                                {
                                    AddSpecFestival(u, GPConstants.SPEC_JANMASTAMI, 0);
                                    //AddSpecFestival(v, GPConstants.SPEC_NANDAUTSAVA, 1);
                                    //AddSpecFestival(v, GPConstants.SPEC_PRABHAPP, 2);
                                    //							u.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                                }
                                else
                                {
                                    // today is Sri Krsna Janmasthami
                                    AddSpecFestival(t, GPConstants.SPEC_JANMASTAMI, 0);
                                    //AddSpecFestival(u, GPConstants.SPEC_NANDAUTSAVA, 1);
                                    //AddSpecFestival(u, GPConstants.SPEC_PRABHAPP, 2);
                                    //							t.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                                }
                            }
                        }
                        else if (t.astrodata.nNaksatra == GPNaksatra.ROHINI_NAKSATRA)
                        {
                            // today is Sri Krsna Janmasthami
                            AddSpecFestival(t, GPConstants.SPEC_JANMASTAMI, 0);
                            //AddSpecFestival(u, GPConstants.SPEC_NANDAUTSAVA, 1);
                            //AddSpecFestival(u, GPConstants.SPEC_PRABHAPP, 2);
                            //					t.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                        }
                        else if (u.astrodata.nNaksatra == GPNaksatra.ROHINI_NAKSATRA)
                        {
                            AddSpecFestival(u, GPConstants.SPEC_JANMASTAMI, 0);
                            //AddSpecFestival(v, GPConstants.SPEC_NANDAUTSAVA, 1);
                            //AddSpecFestival(v, GPConstants.SPEC_PRABHAPP, 2);
                            //					u.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                        }
                        else
                        {
                            if ((u.date.getDayOfWeek() == GPConstants.DW_MONDAY) || (u.date.getDayOfWeek() == GPConstants.DW_WEDNESDAY))
                            {
                                AddSpecFestival(u, GPConstants.SPEC_JANMASTAMI, 0);
                                //AddSpecFestival(v, GPConstants.SPEC_NANDAUTSAVA, 1);
                                //AddSpecFestival(v, GPConstants.SPEC_PRABHAPP, 2);
                                //						u.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                            }
                            else
                            {
                                // today is Sri Krsna Janmasthami
                                AddSpecFestival(t, GPConstants.SPEC_JANMASTAMI, 0);
                                //AddSpecFestival(u, GPConstants.SPEC_NANDAUTSAVA, 1);
                                //AddSpecFestival(u, GPConstants.SPEC_PRABHAPP, 2);
                                //						t.nFastType = (GetShowSetVal(42) ? FAST_MIDNIGHT : FAST_TODAY);
                            }
                        }
                    }
                }
            }



            // test for RathaYatra
            if (t.astrodata.nMasa == GPMasa.VAMANA_MASA)
            {
                if (IsFestivalDay(s, t, GPTithi.TITHI_GAURA_DVITIYA))
                {
                    AddSpecFestival(t, GPConstants.SPEC_RATHAYATRA, 1);
                }

                /*if (nIndex > 4)
                {
                    if (IsFestivalDay(m_pData[nIndex - 5], m_pData[nIndex - 4], GPTithi.TITHI_GAURA_DVITIYA))
                    {
                        AddSpecFestival(t, GPConstants.SPEC_HERAPANCAMI, 1);
                    }
                }

                if (nIndex > 8)
                {
                    if (IsFestivalDay(m_pData[nIndex - 9], m_pData[nIndex - 8], GPTithi.TITHI_GAURA_DVITIYA))
                    {
                        AddSpecFestival(t, GPConstants.SPEC_RETURNRATHA, 1);
                    }
                }

                if (IsFestivalDay(m_pData[nIndex], m_pData[nIndex + 1], GPTithi.TITHI_GAURA_DVITIYA))
                {
                    AddSpecFestival(t, GPConstants.SPEC_GUNDICAMARJANA, 1);
                }*/

            }

            // test for Gaura Purnima
            if (s.astrodata.nMasa == GPMasa.GOVINDA_MASA)
            {
                if (IsFestivalDay(s, t, GPTithi.TITHI_PURNIMA))
                {
                    AddSpecFestival(t, GPConstants.SPEC_GAURAPURNIMA, 0);
                    //			t.nFastType = FAST_MOONRISE;
                }
            }

            // test for Jagannatha Misra festival
            /*if (m_pData[nIndex - 2].astrodata.nMasa == GPMasa.GOVINDA_MASA)
            {
                if (IsFestivalDay(m_pData[nIndex - 2], s, GPTithi.TITHI_PURNIMA))
                {
                    AddSpecFestival(t, GPConstants.SPEC_MISRAFESTIVAL, 1);
                }
            }*/


            // ------------------------
            // test for other festivals
            // ------------------------

            int n, n2;
            int _masa_from = 0, _masa_to;
            int _tithi_from = 0, _tithi_to;
            //GPEventTithi pEvx = null;

            bool s1 = true, s2 = false;

            if (t.astrodata.nMasa > 11)
                goto other_fest;

            n = t.astrodata.nMasa * 30 + t.astrodata.nTithi;
            _tithi_to = t.astrodata.nTithi;
            _masa_to = t.astrodata.nMasa;

            if (s.astrodata.nTithi == t.astrodata.nTithi)
                s1 = false;

            // if ksaya tithi, then s2 is true
            if ((t.astrodata.nTithi != s.astrodata.nTithi) &&
                (t.astrodata.nTithi != (s.astrodata.nTithi + 1) % 30))
            {
                n2 = (n + 359) % 360; // this is index into table of festivals for previous tithi
                _tithi_from = n2 % 30;
                _masa_from = n2 / 30;
                s2 = true;
            }

            if (s2)
            {
                foreach (GPEventTithi pEvx in GPEventList.getShared().tithiEvents)
                {
                    if ((pEvx.nMasa == _masa_from) && (pEvx.nTithi == _tithi_from) && (pEvx.nUsed != 0) && (pEvx.nVisible != 0))
                    {
                        t.AddFestivalCopy(pEvx);
                    }
                }
            }

            if (s1)
            {
                foreach (GPEventTithi pEvx in GPEventList.getShared().tithiEvents)
                {
                    if (pEvx.nMasa == _masa_to && pEvx.nTithi == _tithi_to && pEvx.nUsed != 0 && pEvx.nVisible != 0)
                    {
                        t.AddFestivalCopy(pEvx);
                    }
                }
            }

        other_fest:
            // ---------------------------
            // bhisma pancaka test
            // ---------------------------

            if (t.astrodata.nMasa == GPMasa.DAMODARA_MASA)
            {
                if ((t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA) && (t.nFastType == GPConstants.FAST_EKADASI))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(448, GPStrings.getSharedStrings().getString(81)));
                }
            }

            // ---------------------------
            // caturmasya tests
            // ---------------------------

            // first month for punima and ekadasi systems
            if (t.astrodata.nMasa == GPMasa.VAMANA_MASA)
            {
                // purnima system
                if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_GAURA_CATURDASI, GPTithi.TITHI_PURNIMA))
                {
                    u.Festivals.Add(new GPCalendarDay.Festival(400, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(112) + " " + GPStrings.getSharedStrings().getString(965)));
                    u.Festivals.Add(new GPCalendarDay.Festival(401, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(114)));
                }

                // ekadasi system
                //if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_GAURA_DASAMI, GPTithi.TITHI_GAURA_EKADASI))
                if ((t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA) && (t.nMahadvadasiType != GPConstants.EV_NULL))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(402, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(112) + " " + GPStrings.getSharedStrings().getString(967)));
                    t.Festivals.Add(new GPCalendarDay.Festival(403, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(114)));
                }
            }

            // first month for pratipat system
            // month transit for purnima and ekadasi systems
            if (t.astrodata.nMasa == GPMasa.SRIDHARA_MASA)
            {
                if (s.astrodata.nMasa == GPMasa.ADHIKA_MASA)
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(404, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(115)));
                    t.Festivals.Add(new GPCalendarDay.Festival(405, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(115)));
                    t.Festivals.Add(new GPCalendarDay.Festival(406, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(115)));
                }

                // pratipat system
                if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_PURNIMA, GPTithi.TITHI_KRSNA_PRATIPAT))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(407, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(112) + " " + GPStrings.getSharedStrings().getString(966)));
                    t.Festivals.Add(new GPCalendarDay.Festival(408, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(114)));
                }

                // first day of particular month for PURNIMA system, when purnima is not KSAYA
                if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_GAURA_CATURDASI, GPTithi.TITHI_PURNIMA))
                {
                    u.Festivals.Add(new GPCalendarDay.Festival(409, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(116) + " " + GPStrings.getSharedStrings().getString(965)));
                    u.Festivals.Add(new GPCalendarDay.Festival(410, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(118)));
                    t.Festivals.Add(new GPCalendarDay.Festival(411, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(113) + " " + GPStrings.getSharedStrings().getString(965)));
                }

                // ekadasi system
                //if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_GAURA_DASAMI, GPTithi.TITHI_GAURA_EKADASI))
                if ((t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA) && (t.nMahadvadasiType != GPConstants.EV_NULL))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(412, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(116) + " " + GPStrings.getSharedStrings().getString(967)));
                    t.Festivals.Add(new GPCalendarDay.Festival(413, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(118)));
                    s.Festivals.Add(new GPCalendarDay.Festival(414, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(113) + " " + GPStrings.getSharedStrings().getString(967)));
                }
            }

            // second month for pratipat system
            // month transit for purnima and ekadasi systems
            if (t.astrodata.nMasa == GPMasa.HRSIKESA_MASA)
            {
                if (s.astrodata.nMasa == GPMasa.ADHIKA_MASA)
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(415, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(119)));
                    t.Festivals.Add(new GPCalendarDay.Festival(416, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(119)));
                    t.Festivals.Add(new GPCalendarDay.Festival(417, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(119)));
                }

                // pratipat system
                if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_PURNIMA, GPTithi.TITHI_KRSNA_PRATIPAT))
                //		if (s.astrodata.nMasa == SRIDHARA_MASA)
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(418, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(116) + " " + GPStrings.getSharedStrings().getString(966)));
                    t.Festivals.Add(new GPCalendarDay.Festival(419, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(118)));
                    s.Festivals.Add(new GPCalendarDay.Festival(420, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(113) + " " + GPStrings.getSharedStrings().getString(966)));
                }

                // first day of particular month for PURNIMA system, when purnima is not KSAYA
                if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_GAURA_CATURDASI, GPTithi.TITHI_PURNIMA))
                {
                    u.Festivals.Add(new GPCalendarDay.Festival(421, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(120) + " " + GPStrings.getSharedStrings().getString(965)));
                    u.Festivals.Add(new GPCalendarDay.Festival(422, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(122)));
                    t.Festivals.Add(new GPCalendarDay.Festival(423, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(117) + " " + GPStrings.getSharedStrings().getString(965)));
                }
                // ekadasi system
                if ((t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA) && (t.nMahadvadasiType != GPConstants.EV_NULL))
                //if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_GAURA_DASAMI, GPTithi.TITHI_GAURA_EKADASI))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(424, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(120) + " " + GPStrings.getSharedStrings().getString(967)));
                    t.Festivals.Add(new GPCalendarDay.Festival(425, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(122)));
                    s.Festivals.Add(new GPCalendarDay.Festival(426, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(117) + " " + GPStrings.getSharedStrings().getString(967)));
                }
            }

            // third month for pratipat
            // month transit for purnima and ekadasi systems
            if (t.astrodata.nMasa == GPMasa.PADMANABHA_MASA)
            {
                if (s.astrodata.nMasa == GPMasa.ADHIKA_MASA)
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(427, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(123)));
                    t.Festivals.Add(new GPCalendarDay.Festival(428, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(123)));
                    t.Festivals.Add(new GPCalendarDay.Festival(429, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(123)));
                }
                // pratipat system
                if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_PURNIMA, GPTithi.TITHI_KRSNA_PRATIPAT))
                //		if (s.astrodata.nMasa == HRSIKESA_MASA)
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(430, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(120) + " " + GPStrings.getSharedStrings().getString(966)));
                    t.Festivals.Add(new GPCalendarDay.Festival(431, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(122)));
                    s.Festivals.Add(new GPCalendarDay.Festival(432, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(117) + " " + GPStrings.getSharedStrings().getString(966)));
                }

                // first day of particular month for PURNIMA system, when purnima is not KSAYA
                if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_GAURA_CATURDASI, GPTithi.TITHI_PURNIMA))
                {
                    u.Festivals.Add(new GPCalendarDay.Festival(433, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(124) + " " + GPStrings.getSharedStrings().getString(965)));
                    u.Festivals.Add(new GPCalendarDay.Festival(434, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(126)));
                    t.Festivals.Add(new GPCalendarDay.Festival(435, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(121) + " " + GPStrings.getSharedStrings().getString(965)));
                }

                // ekadasi system
                if ((t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA) && (t.nMahadvadasiType != GPConstants.EV_NULL))
                //if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_GAURA_DASAMI, GPTithi.TITHI_GAURA_EKADASI))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(436, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(124) + " " + GPStrings.getSharedStrings().getString(967)));
                    t.Festivals.Add(new GPCalendarDay.Festival(437, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(126)));
                    s.Festivals.Add(new GPCalendarDay.Festival(438, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(121) + " " + GPStrings.getSharedStrings().getString(967)));
                }
            }

            // fourth month for pratipat system
            // month transit for purnima and ekadasi systems
            if (t.astrodata.nMasa == GPMasa.DAMODARA_MASA)
            {
                if (s.astrodata.nMasa == GPMasa.ADHIKA_MASA)
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(439, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(127)));
                    t.Festivals.Add(new GPCalendarDay.Festival(440, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(127)));
                    t.Festivals.Add(new GPCalendarDay.Festival(441, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(127)));
                }
                // pratipat system
                if (GPTithi.TITHI_TRANSIT(s.astrodata.nTithi, t.astrodata.nTithi, GPTithi.TITHI_PURNIMA, GPTithi.TITHI_KRSNA_PRATIPAT))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(442, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(124) + " " + GPStrings.getSharedStrings().getString(966)));
                    t.Festivals.Add(new GPCalendarDay.Festival(443, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(126)));
                    s.Festivals.Add(new GPCalendarDay.Festival(444, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(121) + " " + GPStrings.getSharedStrings().getString(966)));
                }

                // last day for punima system
                if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_GAURA_CATURDASI, GPTithi.TITHI_PURNIMA))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(445, GPDisplays.Keys.CaturmasyaPurnima, GPStrings.getSharedStrings().getString(125) + " " + GPStrings.getSharedStrings().getString(965)));
                }

                // ekadasi system
                //if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_GAURA_DASAMI, GPTithi.TITHI_GAURA_EKADASI))
                if ((t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA) && (t.nMahadvadasiType != GPConstants.EV_NULL))
                {
                    s.Festivals.Add(new GPCalendarDay.Festival(446, GPDisplays.Keys.CaturmasyaEkadasi, GPStrings.getSharedStrings().getString(125) + " " + GPStrings.getSharedStrings().getString(967)));
                }

                if (GPTithi.TITHI_TRANSIT(t.astrodata.nTithi, u.astrodata.nTithi, GPTithi.TITHI_PURNIMA, GPTithi.TITHI_KRSNA_PRATIPAT))
                {
                    t.Festivals.Add(new GPCalendarDay.Festival(447, GPDisplays.Keys.CaturmasyaPratipat, GPStrings.getSharedStrings().getString(125) + " " + GPStrings.getSharedStrings().getString(966)));

                    // on last day of Caturmasya pratipat system is Bhisma Pancaka ending
                    t.Festivals.Add(new GPCalendarDay.Festival(448, GPStrings.getSharedStrings().getString(82)));
                }
            }

            return 1;
        }

        public int EkadasiCalc(int nIndex, GPLocationProvider earth)
        {
            GPCalendarDay s = m_pData[nIndex - 1];
            GPCalendarDay t = m_pData[nIndex];
            GPCalendarDay u = m_pData[nIndex + 1];

            if (GPTithi.TITHI_EKADASI(t.astrodata.nTithi))
            {
                // if TAT < 11 then NOT_EKADASI
                if (GPTithi.TITHI_LESS_EKADASI(t.astrodata.getTithiAtArunodaya()))
                {
                    t.nMahadvadasiType = GPConstants.EV_NULL;
                    t.sEkadasiVrataName = string.Empty;
                    t.nFastType = GPConstants.FAST_NULL;
                }
                else
                {
                    // else ak MD13 then MHD1 and/or 3
                    if (GPTithi.TITHI_EKADASI(s.astrodata.nTithi) && GPTithi.TITHI_EKADASI(s.astrodata.getTithiAtArunodaya()))
                    {
                        if (GPTithi.TITHI_TRAYODASI(u.astrodata.nTithi))
                        {
                            t.nMahadvadasiType = GPConstants.EV_UNMILANI_TRISPRSA;
                            t.sEkadasiVrataName = GPAppHelper.GetEkadasiName(t.astrodata.nMasa, t.astrodata.nPaksa);
                            t.nFastType = GPConstants.FAST_EKADASI;
                        }
                        else
                        {
                            t.nMahadvadasiType = GPConstants.EV_UNMILANI;
                            t.sEkadasiVrataName = GPAppHelper.GetEkadasiName(t.astrodata.nMasa, t.astrodata.nPaksa);
                            t.nFastType = GPConstants.FAST_EKADASI;
                        }
                    }
                    else
                    {
                        if (GPTithi.TITHI_TRAYODASI(u.astrodata.nTithi))
                        {
                            t.nMahadvadasiType = GPConstants.EV_TRISPRSA;
                            t.sEkadasiVrataName = GPAppHelper.GetEkadasiName(t.astrodata.nMasa, t.astrodata.nPaksa);
                            t.nFastType = GPConstants.FAST_EKADASI;
                        }
                        else
                        {
                            // else ak U je MAHADVADASI then NOT_EKADASI
                            if (GPTithi.TITHI_EKADASI(u.astrodata.nTithi) || (u.nMahadvadasiType >= GPConstants.EV_SUDDHA))
                            {
                                t.nMahadvadasiType = GPConstants.EV_NULL;
                                t.sEkadasiVrataName = string.Empty;
                                t.nFastType = GPConstants.FAST_NULL;
                            }
                            else if (u.nMahadvadasiType == GPConstants.EV_NULL)
                            {
                                // else suddha ekadasi
                                t.nMahadvadasiType = GPConstants.EV_SUDDHA;
                                t.sEkadasiVrataName = GPAppHelper.GetEkadasiName(t.astrodata.nMasa, t.astrodata.nPaksa);
                                t.nFastType = GPConstants.FAST_EKADASI;
                            }
                        }
                    }
                }
            }
            // test for break fast

            if (s.nFastType == GPConstants.FAST_EKADASI)
            {
                CalculateEParana(s, t, earth);
            }

            return 1;
        }

        public int ExtendedCalc(int nIndex, GPLocationProvider earth)
        {
            GPCalendarDay s = m_pData[nIndex - 1];
            GPCalendarDay t = m_pData[nIndex];
            GPCalendarDay u = m_pData[nIndex + 1];
            GPCalendarDay v = m_pData[nIndex + 2];

            // test for Rama Navami
            if ((t.astrodata.nMasa == GPMasa.VISNU_MASA) && (t.astrodata.nPaksa == GPPaksa.GAURA_PAKSA))
            {
                if (IsFestivalDay(s, t, GPTithi.TITHI_GAURA_NAVAMI))
                {
                    if (u.nFastType >= GPConstants.FAST_EKADASI)
                    {
                        // yesterday was Rama Navami
                        AddSpecFestival(s, GPConstants.SPEC_RAMANAVAMI, 0);
                        //s.nFastType = GPConstants.FAST_SUNSET;
                    }
                    else
                    {
                        // today is Rama Navami
                        AddSpecFestival(t, GPConstants.SPEC_RAMANAVAMI, 0);
                        //t.nFastType = GPConstants.FAST_SUNSET;
                    }
                }
            }

            return 1;
        }

        public int CalculateCalendar(GPGregorianTime begDate, int iCount)
        {
            int i, m = 0, weekday;
            int nTotalCount = BEFORE_DAYS + iCount + BEFORE_DAYS;
            GPGregorianTime date;
            GPLocationProvider loc = begDate.getLocationProvider();
            int nYear = 0;
            int prev_paksa = 0;
            bool bCalcMoon = (GPDisplays.Calendar.TimeMoonriseVisible() || GPDisplays.Calendar.TimeMoonsetVisible());

            m_nCount = 0;
            m_Location = begDate.getLocationProvider();
            m_vcStart = new GPGregorianTime(begDate);
            m_vcCount = iCount;

            // alokacia pola
            m_pData = new GPCalendarDay[nTotalCount + 1];

            // inicializacia poctovych premennych
            m_nCount = nTotalCount;
            m_PureCount = iCount;

            date = new GPGregorianTime(begDate);
            date.setDayHours(0.5);

            date.SubDays(BEFORE_DAYS);

            weekday = (Convert.ToInt32(date.getJulianLocalNoon()) + 1) % 7;


            for (i = 0; i <= nTotalCount; i++)
            {
                m_pData[i] = new GPCalendarDay(begDate.getLocationProvider(), this, i - BEFORE_DAYS);
            }

            // 1
            // initialization of days
            foreach (GPCalendarDay vd in m_pData)
            {
                vd.date = new GPGregorianTime(date);
                date.NextDay();
            }

            // 3
            if (bCalcMoon)
            {
                foreach (GPCalendarDay vd in m_pData)
                {
                    GPMoon.CalcMoonTimes(loc, vd.date, out vd.moonrise, out vd.moonset);
                }
            }

            // 4
            // init of astro data
            foreach (GPCalendarDay vd in m_pData)
            {
                vd.astrodata.calculateDayData(vd.date, m_Location);
            }

            bool calc_masa = true;

            // 5
            // init of masa
            prev_paksa = -1;
            foreach (GPCalendarDay vd in m_pData)
            {
                calc_masa = (prev_paksa != -1 ? vd.astrodata.nPaksa != prev_paksa : calc_masa);
                prev_paksa = vd.astrodata.nPaksa;

                if (calc_masa)
                {
                    m = vd.astrodata.determineMasa(vd.date, out nYear);
                }
                vd.astrodata.nMasa = m;
                vd.astrodata.nGaurabdaYear = nYear;
            }

            // 6
            // init of mahadvadasis
            for (i = 2; i < m_PureCount + BEFORE_DAYS + 3; i++)
            {
                //m_pData[i].Clear();
                MahadvadasiCalc(i, loc);
            }

            // 6,5
            // init for Ekadasis
            for (i = 3; i < m_PureCount + BEFORE_DAYS + 3; i++)
            {
                EkadasiCalc(i, loc);
            }

            // 7
            // init of festivals
            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS + 3; i++)
            {
                CompleteCalc(i, loc);
            }

            // 8
            // init of festivals
            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS; i++)
            {
                ExtendedCalc(i, loc);
            }

            // resolve festivals fasting
            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS; i++)
            {
                ResolveFestivalsFasting(i);
            }

            // init for sankranti
            date = new GPGregorianTime(m_pData[0].date);
            i = 0;
            bool bFoundSan;
            int zodiac;
            int i_target;
            do
            {
                date = GPSankranti.GetNextSankranti(date, out zodiac);
                //date.AddHours(loc.getTimeZone().BiasHoursForDate(date));
                //date.normalizeValues();

                bFoundSan = false;
                for (i = 0; i < m_nCount - 1; i++)
                {
                    i_target = -1;

                    switch (GPSankranti.getCurrentSankrantiMethod())
                    {
                        case 0:
                            if (date.CompareYMD(m_pData[i].date) == 0)
                            {
                                i_target = i;
                            }
                            break;
                        case 1:
                            if (date.CompareYMD(m_pData[i].date) == 0)
                            {
                                if (date.getDayHours() < m_pData[i].astrodata.sun.rise.getDayHours())
                                {
                                    i_target = i - 1;
                                }
                                else
                                {
                                    i_target = i;
                                }
                            }
                            break;
                        case 2:
                            if (date.CompareYMD(m_pData[i].date) == 0)
                            {
                                if (date.getDayHours() > m_pData[i].astrodata.sun.noon.getDayHours())
                                {
                                    i_target = i + 1;
                                }
                                else
                                {
                                    i_target = i;
                                }
                            }
                            break;
                        case 3:
                            if (date.CompareYMD(m_pData[i].date) == 0)
                            {
                                if (date.getDayHours() > m_pData[i].astrodata.sun.set.getDayHours())
                                {
                                    i_target = i + 1;
                                }
                                else
                                {
                                    i_target = i;
                                }
                            }
                            break;
                    }

                    if (i_target >= 0)
                    {
                        m_pData[i_target].sankranti_zodiac = zodiac;
                        m_pData[i_target].sankranti_day.Copy(date);
                        bFoundSan = true;
                        break;
                    }
                }
                date.NextDay();
                date.NextDay();
            }
            while (bFoundSan == true);

            // 9
            // init for festivals dependent on sankranti
            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS; i++)
            {
                foreach (GPEventSankranti eve in GPEventList.getShared().sankrantiEvents)
                {
                    if (m_pData[i].sankranti_zodiac == eve.nSankranti)
                    {
                        m_pData[i + eve.nOffset].Festivals.Add(new GPCalendarDay.Festival(80, GPDisplays.Keys.FestivalClass(eve.nClass), eve.strText));
                    }
                }
                /*if (m_pData[i].sankranti_zodiac == GPSankranti.MAKARA_SANKRANTI)
                {
                    m_pData[i].Festivals.Add(new VAISNAVADAY.Festival(GPDisplays.Keys.FestivalClass(5), GPStrings.SharedStrings[78]));
                }
                else if (m_pData[i].sankranti_zodiac == GPSankranti.MESHA_SANKRANTI)
                {
                    m_pData[i].Festivals.Add(new VAISNAVADAY.Festival(GPDisplays.Keys.FestivalClass(5), GPStrings.SharedStrings[79]));
                }
                else if (m_pData[i+1].sankranti_zodiac == GPSankranti.VRSABHA_SANKRANTI)
                {
                    m_pData[i].Festivals.Add(new VAISNAVADAY.Festival(GPDisplays.Keys.FestivalClass(5), GPStrings.SharedStrings[80]));
                }*/
            }

            // 10
            // init ksaya data
            // init of second day of vriddhi
            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS; i++)
            {
                if (m_pData[i].astrodata.nTithi == m_pData[i - 1].astrodata.nTithi)
                    m_pData[i].IsSecondDayTithi = true;
                else if (m_pData[i].astrodata.nTithi != GPTithi.NEXT_TITHI(m_pData[i - 1].astrodata.nTithi))
                {
                    GPLocalizedTithi prevTithi = m_pData[i].getCurrentTithi().getPreviousTithi();
                    m_pData[i].ksayaTithi = prevTithi;
                }
            }

            // travellings insert into data array
            int currIndex = BEFORE_DAYS;
            for (int ci = 0; ci < loc.getChangeCount(); ci++)
            {
                GPLocationChange lc = loc.getChangeAtIndex(ci);

                GPGregorianTime timeStart = new GPGregorianTime(lc.LocationA, new GPJulianTime(lc.julianStart, 0));
                GPGregorianTime timeEnd = new GPGregorianTime(lc.LocationB, new GPJulianTime(lc.julianEnd, 0));

                currIndex = IndexOf(timeStart, currIndex);
                if (currIndex == -2)
                {
                    currIndex = BEFORE_DAYS;
                }
                else if (currIndex == -1)
                {
                    break;
                }
                else
                {
                    if (m_pData[currIndex].Travelling == null)
                        m_pData[currIndex].Travelling = new List<GPLocationChange>();
                    m_pData[currIndex].Travelling.Add(lc);
                }

                currIndex = IndexOf(timeEnd, currIndex);
                if (currIndex == -2)
                {
                    currIndex = BEFORE_DAYS;
                }
                else if (currIndex == -1)
                {
                    break;
                }
                else
                {
                    if (m_pData[currIndex].Travelling == null)
                        m_pData[currIndex].Travelling = new List<GPLocationChange>();
                    m_pData[currIndex].Travelling.Add(lc);
                }
            }

            // adjust travellings list and newlocation flag
            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS; i++)
            {
                if (m_pData[i].Travelling != null)
                {
                    for (int mi = i + 1; mi < m_PureCount + BEFORE_DAYS; mi++)
                    {
                        if (m_pData[mi].Travelling != null)
                        {
                            m_pData[i].Travelling.AddRange(m_pData[mi].Travelling);
                            m_pData[mi].Travelling = null;
                        }
                        else
                        {
                            m_pData[mi].FlagNewLocation = true;
                            i = mi;
                            break;
                        }
                    }
                }
            }

            for (i = BEFORE_DAYS; i < m_PureCount + BEFORE_DAYS; i++)
            {
                m_pData[i].SortFestivals();
            }

            return 1;

        }

        public int IndexOf(GPGregorianTime time)
        {
            return IndexOf(time, BEFORE_DAYS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startIndex"></param>
        /// <returns>Returns -1 in case date is not found and returns -2 in case when 
        /// first tested date was greater than input date. Otherwise returned valid index
        /// in array m_pData.</returns>
        public int IndexOf(GPGregorianTime time, int startIndex)
        {
            int res = 0;
            for (int i = startIndex; i < BEFORE_DAYS + m_PureCount; i++)
            {
                res = m_pData[i].date.CompareYMD(time);
                if (res == 0)
                    return i;
                if (res > 0)
                    return -2;
            }
            return -1;
        }


        public bool IsMhd58(int nIndex, out int nMahaType)
        {
            GPCalendarDay t = m_pData[nIndex];
            GPCalendarDay u = m_pData[nIndex + 1];

            nMahaType = 0;

            if (t.astrodata.nNaksatra != u.astrodata.nNaksatra)
                return false;

            if (t.astrodata.nPaksa != 1)
                return false;

            if (t.astrodata.nTithi == t.astrodata.getTithiAtSunset())
            {
                if (t.astrodata.nNaksatra == GPNaksatra.PUNARVASU_NAKSATRA) // punarvasu
                {
                    nMahaType = GPConstants.EV_JAYA;
                    return true;
                }
                else if (t.astrodata.nNaksatra == GPNaksatra.ROHINI_NAKSATRA) // rohini
                {
                    nMahaType = GPConstants.EV_JAYANTI;
                    return true;
                }
                else if (t.astrodata.nNaksatra == GPNaksatra.PUSYAMI_NAKSATRA) // pusyami
                {
                    nMahaType = GPConstants.EV_PAPA_NASINI;
                    return true;
                }
                else if (t.astrodata.nNaksatra == GPNaksatra.SRAVANA_NAKSATRA) // sravana
                {
                    nMahaType = GPConstants.EV_VIJAYA;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (t.astrodata.nNaksatra == GPNaksatra.SRAVANA_NAKSATRA) // sravana
                {
                    nMahaType = GPConstants.EV_VIJAYA;
                    return true;
                }
            }

            return false;
        }


        public bool NextNewFullIsVriddhi(int nIndex, GPLocationProvider earth)
        {
            int i = 0;
            int nTithi;
            int nPrevTithi = 100;

            for (i = 0; i < BEFORE_DAYS && nIndex < m_pData.Length; i++)
            {
                nTithi = m_pData[nIndex].astrodata.nTithi;
                if ((nTithi == nPrevTithi) && GPTithi.TITHI_FULLNEW_MOON(nTithi))
                {
                    return true;
                }
                nPrevTithi = nTithi;
                nIndex++;
            }

            return false;
        }
        public GPCalendarResults()
        {
            m_PureCount = 0;
            m_nCount = 0;
        }

        public const int BEFORE_DAYS = 8;
    }
}

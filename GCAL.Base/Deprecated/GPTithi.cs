using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public class GPTithi: GPAstroIterator
    {
        public int tithi = 0;
        public bool fullName = false;

        public static int getCount()
        {
            return 30;
        }

        public static string getName(int i)
        {
            return GPStrings.getString(600 + i);
        }

        public static string getFullName(int i)
        {
            return string.Format("{0} {1} / {2} {3}", GPStrings.getString(600 + i), GPStrings.getString(13), GPStrings.getString(i / 15 + 712), 
                GPStrings.getString(20));
        }

        public GPTithi()
        {
        }

        public GPTithi(int i)
        {
            tithi = i;
        }

        public GPTithi(int i, bool fn)
        {
            tithi = i;
            fullName = fn;
        }

        public string getName()
        {
            return GPTithi.getName(tithi);
        }

        public string getFullName()
        {
            return GPTithi.getFullName(tithi);
        }

        public override string ToString()
        {
            return fullName ? getFullName() : getName();
        }


        public const int TITHI_KRSNA_PRATIPAT = 0;
        public const int TITHI_KRSNA_DVITIYA = 1;
        public const int TITHI_KRSNA_TRITIYA = 2;
        public const int TITHI_KRSNA_CATURTI = 3;
        public const int TITHI_KRSNA_PANCAMI = 4;
        public const int TITHI_KRSNA_SASTI = 5;
        public const int TITHI_KRSNA_SAPTAMI = 6;
        public const int TITHI_KRSNA_ASTAMI = 7;
        public const int TITHI_KRSNA_NAVAMI = 8;
        public const int TITHI_KRSNA_DASAMI = 9;
        public const int TITHI_KRSNA_EKADASI = 10;
        public const int TITHI_KRSNA_DVADASI = 11;
        public const int TITHI_KRSNA_TRAYODASI = 12;
        public const int TITHI_KRSNA_CATURDASI = 13;
        public const int TITHI_AMAVASYA = 14;
        public const int TITHI_GAURA_PRATIPAT = 15;
        public const int TITHI_GAURA_DVITIYA = 16;
        public const int TITHI_GAURA_TRITIYA = 17;
        public const int TITHI_GAURA_CATURTI = 18;
        public const int TITHI_GAURA_PANCAMI = 19;
        public const int TITHI_GAURA_SASTI = 20;
        public const int TITHI_GAURA_SAPTAMI = 21;
        public const int TITHI_GAURA_ASTAMI = 22;
        public const int TITHI_GAURA_NAVAMI = 23;
        public const int TITHI_GAURA_DASAMI = 24;
        public const int TITHI_GAURA_EKADASI = 25;
        public const int TITHI_GAURA_DVADASI = 26;
        public const int TITHI_GAURA_TRAYODASI = 27;
        public const int TITHI_GAURA_CATURDASI = 28;
        public const int TITHI_PURNIMA = 29;

        public static bool TITHI_EKADASI(int a)
        {
            return a == 10 || a == 25;
        }

        public static bool TITHI_DVADASI(int a)
        {
            return a == 11 || a == 26;
        }

        public static bool TITHI_TRAYODASI(int a)
        {
            return a == 12 || a == 27;
        }

        public static bool TITHI_LESS_EKADASI(int a)
        {
            return ((a == 9) || (a == 24) || (a == 8) || (a == 23));
        }

        public static bool TITHI_LESS_DVADASI(int a)
        {
            return (((a) == 10) || ((a) == 25) || ((a) == 9) || ((a) == 24));
        }

        public static bool TITHI_LESS_TRAYODASI(int a)
        {
            return (((a) == 11) || ((a) == 26) || ((a) == 10) || ((a) == 25));
        }
        public static bool TITHI_FULLNEW_MOON(int a)
        {
            return (((a) == 14) || ((a) == 29));
        }

        public static int PREV_PREV_TITHI(int a)
        {
            return (((a) + 28) % 30);
        }
        public static int PREV_TITHI(int a)
        {
            return (((a) + 29) % 30);
        }
        public static int NEXT_TITHI(int a)
        {
            return (((a) + 1) % 30);
        }
        public static int NEXT_NEXT_TITHI(int a)
        {
            return (((a) + 1) % 30);
        }

        public static bool TITHI_LESS_THAN(int a, int b)
        {
            return (((a) == PREV_TITHI(b)) || ((a) == PREV_PREV_TITHI(b)));
        }
        public static bool TITHI_GREAT_THAN(int a, int b)
        {
            return (((a) == NEXT_TITHI(b)) || ((a) == NEXT_NEXT_TITHI(b)));
        }

        // TRUE when transit from A to B is between T and U
        public static bool TITHI_TRANSIT(int t, int u, int a, int b)
        {
            return (((t) == (a)) && ((u) == (b))) || (((t) == (a)) && ((u) == NEXT_TITHI(b))) || (((t) == PREV_TITHI(a)) && ((u) == (b)));
        }


        public static double GetTithiTimes(GPGregorianTime vc, out double titBeg, out double titEnd, double sunRise)
        {
            GPGregorianTime d1, d2;

            vc.setDayHours(sunRise);
            GetPrevTithiStart(vc, out d1);
            GetNextTithiStart(vc, out d2);

            titBeg = d1.getDayHours() + d1.getJulianLocalNoon() - vc.getJulianLocalNoon();
            titEnd = d2.getDayHours() + d2.getJulianLocalNoon() - vc.getJulianLocalNoon();

            return (titEnd - titBeg);
        }


        public int getCurrentTithi()
        {
            return getCurrentPosition();
        }

        public override double getUnitAverageLength()
        {
            return 0.9;
        }

        public override int getUnitCount()
        {
            return 30;
        }

        public override double calculatePosition()
        {
            pMoon.MoonCalc(pJulianDate);
            //pSun.calculateCoordinates(pStartDate, pStartDate.getLocation(), pStartDate.getDayHours());
            //	SunPosition(d, ed, sun, d.shour - 0.5 + d.tzone/24.0);
            double l1 = GPMath.putIn360(pMoon.longitude_deg - /*pSun.eclipticalLongitude*/
                GPSun.GetSunLongitude(pJulianDate) - 180.0);
            return l1 / 12.0;
        }


        /*********************************************************************/
        /*                                                                   */
        /*   finds next time when starts next tithi                          */
        /*                                                                   */
        /*   timezone is not changed                                         */
        /*                                                                   */
        /*   return value: index of tithi 0..29                              */
        /*                 or -1 if failed                                   */
        /*********************************************************************/

        public static int GetNextTithiStart(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double phi = 12.0;
            double l1, l2, sunl;
            double jday = startDate.getJulianGreenwichTime();
            double xj;
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocation());
            double scan_step = 0.5;
            int prev_tit = 0;
            int new_tit = -1;

            //Debugger.Log(0, "", "---------- GetNextTithiStart -------------------\n");
            moon.MoonCalc(jday);
            sunl = GPSun.GetSunLongitude(jday);
            //	SunPosition(d, ed, sun, d.shour - 0.5 + d.tzone/24.0);
            l1 = GPMath.putIn360(moon.longitude_deg - sunl - 180.0);
            prev_tit = Convert.ToInt32(Math.Floor(l1 / phi));

            int counter = 0;
            while (counter < 20)
            {
                xj = jday;
                xd.Copy(d);

                jday += scan_step;
                d.setDayHours( d.getDayHours() + scan_step);
                if (d.getDayHours() > 1.0)
                {
                    d.setDayHours( d.getDayHours() - 1.0);
                    d.NextDay();
                }

                moon.MoonCalc(jday);
                //SunPosition(d, ed, sun, d.shour - 0.5 + d.tzone/24.0);
                //l2 = put_in_360(moon.longitude_deg - sun.longitude_deg - 180.0);
                sunl = GPSun.GetSunLongitude(jday);
                l2 = GPMath.putIn360(moon.longitude_deg - sunl - 180.0);
                //Debugger.Log(0, "", "Current position: " + l2/12.0 + "   date: " + jday + "\n");
                new_tit = Convert.ToInt32(Math.Floor(l2 / phi));

                if (prev_tit != new_tit)
                {
                    jday = xj;
                    d.Copy(xd);
                    scan_step *= 0.5;
                    counter++;
                    continue;
                }
                else
                {
                    l1 = l2;
                }
            }
            nextDate = d;
            //	nextDate.shour += startDate.tzone / 24.0;
            //	nextDate.NormalizeValues();
            return new_tit;
        }

        /*********************************************************************/
        /*                                                                   */
        /*   finds previous time when starts next tithi                      */
        /*                                                                   */
        /*   timezone is not changed                                         */
        /*                                                                   */
        /*   return value: index of tithi 0..29                              */
        /*                 or -1 if failed                                   */
        /*********************************************************************/

        public static int GetPrevTithiStart(GPGregorianTime startDate, out GPGregorianTime nextDate)
        {
            double phi = 12.0;
            double l1, l2, sunl;
            double jday = startDate.getJulianGreenwichTime();
            double xj;
            GPMoon moon = new GPMoon();
            GPGregorianTime d = new GPGregorianTime(startDate);
            GPGregorianTime xd = new GPGregorianTime(startDate.getLocation());
            double scan_step = 0.5;
            int prev_tit = 0;
            int new_tit = -1;

            moon.MoonCalc(jday);
            sunl = GPSun.GetSunLongitude(jday);
            l1 = GPMath.putIn360(moon.longitude_deg - sunl - 180.0);
            prev_tit = Convert.ToInt32(Math.Floor(l1 / phi));

            int counter = 0;
            while (counter < 20)
            {
                xj = jday;
                xd.Copy(d);

                jday -= scan_step;
                d.setDayHours( d.getDayHours() - scan_step);
                if (d.getDayHours() < 0.0)
                {
                    d.setDayHours( d.getDayHours() + 1.0);
                    d.PreviousDay();
                }

                moon.MoonCalc(jday);
                sunl = GPSun.GetSunLongitude(jday);
                l2 = GPMath.putIn360(moon.longitude_deg - sunl - 180.0);
                new_tit = Convert.ToInt32(Math.Floor(l2 / phi));

                if (prev_tit != new_tit)
                {
                    jday = xj;
                    d.Copy(xd);
                    scan_step *= 0.5;
                    counter++;
                    continue;
                }
                else
                {
                    l1 = l2;
                }
            }
            nextDate = d;
            //	nextDate.shour += startDate.tzone / 24.0;
            //	nextDate.NormalizeValues();
            return new_tit;
        }





        /*********************************************************************/
        /*  Calculates Date of given Tithi                                   */
        /*********************************************************************/

        public static GPGregorianTime CalcTithiDate(int nGYear, int nMasa, int nPaksa, int nTithi, GPLocation earth)
        {
            int i = 0, gy = 0;
            GPGregorianTime d = new GPGregorianTime(earth);
            GPAstroData day = new GPAstroData();
            int tithi = 0;
            int counter = 0;
            uint tmp = 0;

            if (nGYear >= 464 && nGYear < 572)
            {
                tmp = GPGaurabdaYear.gGaurBeg[(nGYear - 464) * 26 + nMasa * 2 + nPaksa];
                d.setDate(Convert.ToInt32(tmp & 0xfffc00) >> 10,
                          Convert.ToInt32(tmp & 0x3e0) >> 5,
                          Convert.ToInt32(tmp & 0x1f));
                d.NextDay();

                day.calculateDayData(d, earth);
                day.nMasa = day.determineMasa(d, out gy);
                day.nGaurabdaYear = gy;
            }
            else
            {
                //d = GetFirstDayOfYear(earth, nGYear + 1486);
                d.setDate(nGYear + 1486, 2 + nMasa, 15);
                if (d.getMonth() > 12)
                {
                    d.setDate(d.getYear() + 1, d.getMonth() - 12, 0);
                }
                d.setDayHours(0.5);

                i = 0;
                do
                {
                    d.AddDays(13);
                    day.calculateDayData(d, earth);
                    day.nMasa = day.determineMasa(d, out gy);
                    day.nGaurabdaYear = gy;
                    i++;
                }
                while (((day.nPaksa != nPaksa) || (day.nMasa != nMasa)) && (i <= 30));
            }

            if (i >= 30)
            {
                d.Clear();
                return d;
            }

            // we found masa and paksa
            // now we have to find tithi
            tithi = nTithi + nPaksa * 15;

            if (day.nTithi == tithi)
            {
                // loc1
                // find tithi juncts in this day and according to that times,
                // look in previous or next day for end and start of this tithi
                d.PreviousDay();
                day.calculateDayData(d, earth);
                if ((day.nTithi > tithi) && (day.nPaksa != nPaksa))
                {
                    d.NextDay();
                }
                return d;
            }

            if (day.nTithi < tithi)
            {
                // do increment of date until nTithi == tithi
                //   but if nTithi > tithi
                //       then do decrement of date
                counter = 0;
                while (counter < 16)
                {
                    d.NextDay();
                    day.calculateDayData(d, earth);
                    if (day.nTithi == tithi)
                        return d;
                    if ((day.nTithi < tithi) && (day.nPaksa != nPaksa))
                        return d;
                    if (day.nTithi > tithi)
                        return d;
                    counter++;
                }
                // somewhere is error
                d.Clear();
                return d;
            }
            else
            {
                // do decrement of date until nTithi <= tithi
                counter = 0;
                while (counter < 16)
                {
                    d.PreviousDay();
                    day.calculateDayData(d, earth);
                    if (day.nTithi == tithi)
                        return d;
                    if ((day.nTithi > tithi) && (day.nPaksa != nPaksa))
                    {
                        d.NextDay();
                        return d;
                    }
                    if (day.nTithi < tithi)
                    {
                        d.NextDay();
                        return d;
                    }
                    counter++;
                }
                // somewhere is error
                d.Clear();
                return d;
            }

            // now we know the type of day-accurancy
            // nType = 0 means, that we dont found a day
            // nType = 1 means, we find day, when tithi was present at sunrise
            // nType = 2 means, we found day, when tithi started after sunrise
            //                  but ended before next sunrise
            //

        }



        /*********************************************************************/
        /* Finds starting and ending time for given tithi                    */
        /*                                                                   */
        /* tithi is specified by Gaurabda year, masa, paksa and tithi number */
        /*      nGYear - 0..9999                                             */
        /*       nMasa - 0..12, 0-Madhusudana, 1-Trivikrama, 2-Vamana        */
        /*                      3-Sridhara, 4-Hrsikesa, 5-Padmanabha         */
        /*                      6-Damodara, 7-Kesava, 8-narayana, 9-Madhava  */
        /*                      10-Govinda, 11-Visnu, 12-PurusottamaAdhika   */
        /*       nPaksa -       0-Krsna, 1-Gaura                             */
        /*       nTithi - 0..14                                              */
        /*       earth  - used timezone                                      */
        /*                                                                   */
        /*********************************************************************/

        public static GPGregorianTime CalcTithiEnd(int nGYear, int nMasa, int nPaksa, int nTithi, GPLocation earth, out GPGregorianTime endTithi)
        {
            GPGregorianTime d;

            d = GPGaurabdaYear.getFirstDayOfYear(earth, nGYear + 1486);
            d.setDayHours(0.5);
            d.setLocation(earth);

            return CalcTithiEndEx(d, nGYear, nMasa, nPaksa, nTithi, earth, out endTithi);
        }



        public static GPGregorianTime CalcTithiEndEx(GPGregorianTime vcStart, int GYear, int nMasa, int nPaksa, int nTithi, GPLocation earth, out GPGregorianTime endTithi)
        {
            int i, gy, nType;
            GPGregorianTime d = new GPGregorianTime(earth);
            GPGregorianTime dtemp = new GPGregorianTime(earth);
            GPAstroData day = new GPAstroData();
            int tithi;
            int counter;
            GPGregorianTime start = new GPGregorianTime(earth), end = new GPGregorianTime(earth);
            //	SUNDATA sun;
            //	MOONDATA moon;
            double sunrise;
            start.setDayHours(-1.0);
            end.setDayHours(-1.0);
            start.Clear();
            end.Clear();

            /*	d = GetFirstDayOfYear(earth, nGYear + 1486);
                d.shour = 0.5;
                d.TimeZone = earth.tzone;
            */
            d.Copy(vcStart);

            i = 0;
            do
            {
                d.AddDays(13);
                day.calculateDayData(d, earth);
                day.nMasa = day.determineMasa(d, out gy);
                day.nGaurabdaYear = gy;
                i++;
            }
            while (((day.nPaksa != nPaksa) || (day.nMasa != nMasa)) && (i <= 30));

            if (i >= 30)
            {
                d.Clear();
                endTithi = d;
                return d;
            }

            // we found masa and paksa
            // now we have to find tithi
            tithi = nTithi + nPaksa * 15;

            if (day.nTithi == tithi)
            {
                // loc1
                // find tithi juncts in this day and according to that times,
                // look in previous or next day for end and start of this tithi
                nType = 1;
            }
            else
            {
                if (day.nTithi < tithi)
                {
                    // do increment of date until nTithi == tithi
                    //   but if nTithi > tithi
                    //       then do decrement of date
                    counter = 0;
                    while (counter < 30)
                    {
                        d.NextDay();
                        day.calculateDayData(d, earth);
                        if (day.nTithi == tithi)
                            goto cont_2;
                        if ((day.nTithi < tithi) && (day.nPaksa != nPaksa))
                        {
                            d.PreviousDay();
                            goto cont_2;
                        }
                        if (day.nTithi > tithi)
                        {
                            d.PreviousDay();
                            goto cont_2;
                        }
                        counter++;
                    }
                    // somewhere is error
                    d.Clear();
                    nType = 0;
                }
                else
                {
                    // do decrement of date until nTithi <= tithi
                    counter = 0;
                    while (counter < 30)
                    {
                        d.PreviousDay();
                        day.calculateDayData(d, earth);
                        if (day.nTithi == tithi)
                            goto cont_2;
                        if ((day.nTithi > tithi) && (day.nPaksa != nPaksa))
                        {
                            goto cont_2;
                        }
                        if (day.nTithi < tithi)
                        {
                            goto cont_2;
                        }
                        counter++;
                    }
                    // somewhere is error
                    d.Clear();
                    nType = 0;

                }
            cont_2:
                if (day.nTithi == tithi)
                {
                    // do the same as in loc1
                    nType = 1;
                }
                else
                {
                    // nTithi != tithi and nTithi < tithi
                    // but on next day is nTithi > tithi
                    // that means we will find start and the end of tithi
                    // in this very day or on next day before sunrise
                    nType = 2;
                }

            }

            // now we know the type of day-accurancy
            // nType = 0 means, that we dont found a day
            // nType = 1 means, we find day, when tithi was present at sunrise
            // nType = 2 means, we found day, when tithi started after sunrise
            //                  but ended before next sunrise
            //
            sunrise = day.sun.getSunriseDayHours() / 360 + day.sun.rise.getLocation().getTimeZoneOffsetHours() / 24;

            if (nType == 1)
            {
                GPGregorianTime d1, d2;
                d.setDayHours(sunrise);
                GetPrevTithiStart(d, out d1);
                //d = d1;
                //d.shour += 0.02;
                GetNextTithiStart(d, out d2);

                endTithi = d2;
                return d1;
            }
            else if (nType == 2)
            {
                GPGregorianTime d1, d2;
                d.setDayHours( sunrise);
                GetNextTithiStart(d, out d1);
                d.Copy(d1);
                d.addDayHours(0.1);
                GetNextTithiStart(d, out d2);

                endTithi = d2;
                return d1;
            }

            // if nType == 0, then this algoritmus has some failure
            if (nType == 0)
            {
                d.Clear();
                d.setDayHours(0.0);
                endTithi = d;
            }
            else
            {
                d.Copy(start);
                endTithi = end;
            }

            return d;
        }


    }
}




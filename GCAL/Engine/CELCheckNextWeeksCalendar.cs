using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using GCAL.Base;

namespace GCAL.Engine
{
    public class CELCheckNextWeeksCalendar: CELBase
    {
        public List<GPStringPair> lines = new List<GPStringPair>();
        private GPCalendarResults p_cal = new GPCalendarResults();
        private GPGregorianTime p_today = null;

        protected override void Execute()
        {

            GPLocationProvider loc = GPAppHelper.getMyLocation();

            //Testing.Report(loc, "gcal13");

            //loc1.setLatitudeNorthPositive(48.16);
            //loc1.setLongitudeEastPositive(17.09);
            //loc1.setTimeZoneName("Europe/Bratislava");
            p_today = new GPGregorianTime(loc);
            p_today.Today();
            /*
            Debugger.Log(0,"", String.Format("Location: {0} {1} {2}\n", loc.getFullName(), loc1.getLongitudeString(), loc1.getLatitudeString()));

            double date = p_today.getJulianLocalNoon();
            TEclipse te;
            for (int i = 0; i < 30; i++)
            {
                te = MA.Engine.NextEclipse(ref date, true);
                int y, m, d;
                MA.Engine.DecodeDateCorrect(date, out y, out m, out d);
                Debugger.Log(0, "", String.Format("eclipse:{0}  date: {1} {2} {3}\n", te, y, m, d));
                date += 20;
            }

            //double a1 = MA.GPMeeusEngine.star_time(2456708.3200) ;
            //a1 = MA.GPMeeusEngine.star_time(2452083) - a1;


            double lat1 = 48.16, lat2 = 120;
            double lon1 = 17.09, lon2 = 123;
            double dStart = 0.287399999999998;
            double dEnd = 0.287799999999998;
            GPObserver obs = new GPObserver();
            obs.setLatitudeNorthPositive(lat1).setLongitudeEastPositive(lon1).SetAltitude(0.2);

            GPJulianTime rise, trans, set;


            GPJulianTime time = new GPJulianTime();

            time.setLocalJulianDay(2456708.5);
            time.setLocalTimezoneOffset(1);
            for (int l = 0; l < 40; l++)
            {
                Log("Julian  {0}     = {1} / {2} / {3}   {4}:{5}:{6}\n", time.GetJulianDay(), time.GetYear(),
                    time.GetMonth(), time.GetDay(), time.GetHour(), time.GetMinute(), time.GetSecond());
                time.AddSeconds(79367.6);
            }

            //GPGregorianTime gt = new GPGregorianTime(loc);
            //gt.setJulianGreenwichTime(time);
            //double rise = MA.GPMeeusEngine.Sun_Rise(p_today.GetJulianDetailed() - 0.5, loc.Latitude, -loc.Longitude);
            //GPAstroEngine.CalculateTimeSun(gt, obs, out rise, out trans, out set);

            //Log("Rise: {0}, \nTrans:{1}, \nSet:{2}", rise, trans, set);

            //MA.Testing.TestSunCoordinates();
            //MA.Testing.TestSiderealTime();
            //MA.Testing.TestMoonEvents();

            //GPSun sun = new GPSun();
            //sun.SunCalc(p_today, loc);
            TRiseSet kind;
            double deltaphi, epsilon, srt;
            GPJulianTime dp = new GPJulianTime();
            dp.setLocalJulianDay(2456710.500000);

            obs.setLongitudeEastPositive(-25.858).setLatitudeNorthPositive(-23.983);
            srt = 2452081.000000;

            //MA.Testing.TestMoonEclipse();

            Testing.TestConjunctions();

            //MA.GPCelestialBodyCoordinates coord = MA.GPMeeusEngine.moon_coordinate(2448724.5);
            //double phi, eps;
            //MA.GPMeeusEngine.calc_epsilon_phi(2446895.5, out phi, out eps);
            //Log("Sidereal time: {0}", MA.GPMeeusEngine.star_time(2446895.5));
            //Log("Epsilon: {0}, phi: {1}", eps, phi);
            //Log("Sunrise {0}", sun.rise.LongTimeString);

            return;
            */
            int maxCount = GPUserDefaults.IntForKey("nextfest.days", 16);
            if (maxCount < 3)
            {
                maxCount = 16;
                GPUserDefaults.SetIntForKey("nextfest.days", maxCount);
            }
            bool onlyFast = GPUserDefaults.BoolForKey("nextfest.onlyfast", true);
            p_cal.CalculateCalendar(p_today, maxCount);
            List<string> temp = new List<string>();
            for (int i = 0; i < p_cal.getCount(); i++)
            {
                temp.Clear();
                GPCalendarDay vd = p_cal.get(i);
                if (onlyFast)
                {
                    if (vd.sEkadasiVrataName.Length > 0)
                    {
                        temp.Add(string.Format(GPStrings.getString(87), vd.sEkadasiVrataName));
                    }
                    else if (vd.hasEkadasiParana())
                    {
                        temp.Add(vd.getEkadasiParanaString());
                    }
                }
                else
                {
                    if (vd.hasEkadasiParana())
                    {
                        temp.Add(vd.getEkadasiParanaString());
                    }
                    /*foreach (GPCalendarDay.Festival fest in vd.Festivals)
                    {
                        //if (onlyFast == false || fest.getPreviousFastType() != GPConstants.FAST_NULL)
                        {
                            temp.Add(fest.Text);
                        }
                    }*/
                    foreach (GPCalendarDay.Festival fest in vd.Festivals)
                    {
                        if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                        {
                            temp.Add(fest.Text);
                        }
                    }
                }

                if (temp.Count > 0)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        GPStringPair dr = new GPStringPair();
                        if (j == 0)
                            dr.Name = vd.date.ToString() + " " + GPStrings.getString(150 + vd.date.getDayOfWeek());
                        dr.Value = temp[j];
                        lines.Add(dr);
                    }
                }
            }
        }

        public void Log(String format, params object[] args)
        {
            Debugger.Log(0, "", String.Format(format, args));
            Debugger.Log(0, "", "\n");
        }

        public string TodayString
        {
            get
            {
                if (p_today == null)
                    return string.Empty;

                StringBuilder sb = new StringBuilder();
                FormaterHtml.WriteTodayInfoHTML(p_today, p_cal, sb, GPUserDefaults.IntForKey("FontSize", 10));
                return sb.ToString();
            }
        }

        public string getNextFestDaysString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table cellpadding=0 cellspacing=0 border=0 width=95%>");
            int ic = 0;
            bool rowStarted = false;
            foreach (GPStringPair dr in lines)
            {
                if (dr.Name.Length > 0)
                {
                    if (rowStarted)
                        sb.AppendLine("</td></tr>");

                    if (ic % 2 == 0)
                        sb.Append("<tr style='background:#cceeee'>");
                    else
                        sb.Append("<tr>");
                    ic++;
                    sb.AppendFormat("<td><span style='font-weight:bold'>{0}</span><br>", dr.Name);
                    rowStarted = true;
                }
                sb.AppendFormat(" &nbsp;&nbsp;{0}<br>", dr.Value);
            }
            if (rowStarted)
                sb.AppendLine("</td></tr>");
            sb.Append("</table>");
            return sb.ToString();
        }

    }
}

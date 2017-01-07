using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using GCAL.Base.Astronomy;

namespace GCAL.Base
{
    public class Testing
    {
        public static double defLatitude = 48.10;
        public static double defLongitude = 17.08;
        public static double defJulian = 2456710.500000;

        public static void Log(String format, params object[] args)
        {
            Debugger.Log(0, "", String.Format(format, args));
            Debugger.Log(0, "", "\n");
        }

        public static void TestAlg()
        {
            GPLocation location = new GPLocation();
            location.setCity("Vrakun");
            location.setCountryCode("SK");
            location.setLatitudeNorthPositive(47.93922);
            location.setLongitudeEastPositive(17.59145);
            location.setTimeZoneName("Europe/Bratislava");


            GPGregorianTime time = new GPGregorianTime(location);
            time.setDate(2016, 8, 7);
            time.setDayHours(8, 12, 0);

            GPCelestialBodyCoordinates coord = GPAstroEngine.sun_coordinate(time.getJulianGreenwichTime());

            GPAstroEngine.calcHorizontal(coord, location);

            Log("Sun Coordinates: Azimut: {0}, Elevation: {1}", coord.azimuth, coord.elevation);

        }

        public static void TestMoonEvents()
        {
            TRiseSet kind;
            GPJulianTime dp = new GPJulianTime();
            dp.setLocalJulianDay(defJulian);
            GPLocation obs = new GPLocation();
            obs.setLatitudeNorthPositive(defLatitude).setLongitudeEastPositive(defLongitude).SetAltitude(0.2);
            GPLocation prov = obs;

            for (int k = 0; k < 26; k++)
            {
                dp = GPAstroEngine.GetNextMoonEvent(dp, prov, out kind);
                Log("next event = {0}, {1}\n", dp, kind);
                dp.AddHours(1);
            }
        }

        public static void TestSunCoordinates()
        {
            GPObserver obs = new GPObserver();
            obs.setLatitudeNorthPositive(defLatitude).setLongitudeEastPositive(defLongitude);
            GPJulianTime dp = new GPJulianTime();
            dp.setLocalJulianDay(2456710.500000);
            for (int k = 0; k < 26; k++)
            {
                //dp = MA.GPMeeusEngine.GetNextMoonEvent(dp, obs, out kind);
                GPCelestialBodyCoordinates crd = GPAstroEngine.sun_coordinate(dp.getGreenwichJulianEphemerisDay());
                //srt = MA.GPMeeusEngine.GetSiderealTime(dp.GetJulianDay(), out deltaphi, out epsilon);
                //crd = MA.GPMeeusEngine.moon_coordinate(2448724.5);
                crd.makeTopocentric(obs);
                GPAstroEngine.calcHorizontal(crd, obs);
                //Log("time {0}   deltaphi {1}  epsilon {2} sidereal {3}", dp, deltaphi, epsilon, srt/15);
                //Log("time {0}   altitude {1}  azimuth {2}", dp, crd.elevation, crd.azimuth);
                //Log("time {0}   ra {1}  dec {2}", dp, crd.right_ascession, crd.declination);
                //Log("RA={0} DEC={1}", crd.right_ascession, crd.elevation);
                //Log("AZ={0} EL={1} RA", crd.azimuth, crd.elevation);
                Log("{0}", crd.elevation);
                //                Log("next event = {0}, {1}, {2}\n", dp, kind, crd.elevation);
                dp.AddHours(1);
            }
        }
        
        public static void TestSiderealTime()
        {
            GPObserver obs = new GPObserver();
            obs.setLatitudeNorthPositive(defLatitude).setLongitudeEastPositive(defLongitude);
            GPJulianTime dp = new GPJulianTime();
            dp.setLocalJulianDay(2456710.500000);
            for (int k = 0; k < 26; k++)
            {
                GPCelestialBodyCoordinates crd = GPAstroEngine.sun_coordinate(dp.getGreenwichJulianEphemerisDay());
                Log("{0}", crd.apparent_sidereal_time/15);
                dp.AddHours(1);
            }
        }
        public static void TestMoonCoordinates()
        {
            GPObserver obs = new GPObserver();
            obs.setLatitudeNorthPositive(defLatitude).setLongitudeEastPositive(defLongitude).SetAltitude(0.2);
            GPJulianTime dp = new GPJulianTime();
            dp.setLocalJulianDay(2456710.500000);
            for (int k = 0; k < 26; k++)
            {
                //dp = MA.GPMeeusEngine.GetNextMoonEvent(dp, obs, out kind);
                GPCelestialBodyCoordinates crd = GPAstroEngine.moon_coordinate(dp.getGreenwichJulianEphemerisDay());
                //srt = MA.GPMeeusEngine.GetSiderealTime(dp.GetJulianDay(), out deltaphi, out epsilon);
                //crd = MA.GPMeeusEngine.moon_coordinate(2448724.5);
                crd.makeTopocentric(obs);
                GPAstroEngine.calcHorizontal(crd, obs);
                //Log("time {0}   deltaphi {1}  epsilon {2} sidereal {3}", dp, deltaphi, epsilon, srt/15);
                //Log("time {0}   altitude {1}  azimuth {2}", dp, crd.elevation, crd.azimuth);
                //Log("time {0}   ra {1}  dec {2}", dp, crd.right_ascession, crd.declination);
                Log("{0}", crd.azimuth);
                //                Log("next event = {0}, {1}, {2}\n", dp, kind, crd.elevation);
                dp.AddHours(1);
            }
        }

        public static void TestSunEclipse()
        {
            Log("=== start sun eclipse ====");
            GPObserver obs = new GPObserver();
            double srt;
            obs.setLongitudeEastPositive(-25.858).setLatitudeNorthPositive(-23.983);
            srt = 2452081.000000;
            GPAstroEngine.FindNextEclipse(ref srt, true);
            Log("Next eclipse = {0}", srt);
            double[] times = null;
            GPAstroEngine.CalculateTimesSunEclipse(srt, obs, out times);
            for (int i = 0; i < times.Length; i++)
            {
                Log("times[{0}] = {1}", times[i]);
            }
            Log("=== end sun eclipse ====");
        }

        public static void TestMoonEclipse()
        {
            Log("=== start moon eclipse ====");
            GPObserver obs = new GPObserver();
            double srt;
            obs.setLongitudeEastPositive(-25.858).setLatitudeNorthPositive(-23.983);
            srt = 2451919.500000;
            //GPMeeusEngine.NextEclipse(ref srt, false);
            Log("Next eclipse = {0}", srt);
            double[] times = null;
            GPAstroEngine.CalculateTimesMoonEclipse(srt, obs, out times);

            for (int i = 0; i < times.Length; i++)
            {
                Log("times[{0}] = {1}", i, times[i]);
            }
            Log("=== end moon eclipse ====");
        }

        public static void TestConjunctions()
        {
            Log("== start test conjunctions ==");

            double date = 2451919.500000;

            for (int i = 0; i < 30; i++)
            {
                date = GPAstroEngine.FindNextConjunction(date);
                Log("Conjunction at: {0}", date);
                if (date < 0)
                    break;
                date += 20;
            }
            Log("== end test conjunctions ==");
        }

        public static void Report(GPLocation loc, string prefix)
        {
            GPGregorianTime start = new GPGregorianTime(loc);
            GPGregorianTime end = new GPGregorianTime(loc);
            start.setDate(2010, 1, 1);
            end.setDate(2015, 1, 1);

            GPCalendarResults calendar = new GPCalendarResults();
            calendar.CalculateCalendar(start, Convert.ToInt32(end.getJulianLocalNoon() - start.getJulianLocalNoon()));

            GPCoreEventResults coreEvents = new GPCoreEventResults();
            coreEvents.Sort(false);
            coreEvents.CalculateEvents(loc, start, end);

            string path = "d:\\gcal\\reports\\";

            if (Directory.Exists(path))
            {
                File.WriteAllText(Path.Combine(path, prefix + "-cal-" + loc.getCity() + ".txt"),
                    FormaterInternal.getInternalDalendarData(calendar));
                File.WriteAllText(Path.Combine(path, prefix + "-eve-" + loc.getCity() + ".txt"),
                    FormaterInternal.getInternalEventsText(coreEvents));
            }
        }
    }
}

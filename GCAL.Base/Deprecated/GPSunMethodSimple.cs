using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public class GPSunMethodSimple: GPSunMethod
    {
        public const double SUN_RADIUS = 695500;

        static double[] sun_long = new double[]{
		339.226,009.781,039.351,069.906,099.475,130.030,160.585,190.155,220.710,250.279,280.834,311.390,
		340.212,010.767,040.337,070.892,100.461,131.016,161.571,191.141,221.696,251.265,281.820,312.375,
		341.198,011.753,041.322,071.877,101.447,132.002,162.557,192.126,222.681,252.251,282.806,313.361,
		342.183,012.738,042.308,072.863,102.432,132.987,163.542,193.112,223.667,253.236,283.791,314.346,
		343.169,013.724,043.293,073.849,103.418,133.973,164.528,194.098,224.653,254.222,284.777,315.332,
		344.155,014.710,044.279,074.834,104.404,134.959,165.514,195.083,225.638,255.208,285.763,316.318,
		345.140,015.695,045.265,075.820,105.389,135.944,166.499,196.069,226.624,256.193,286.748,317.303,
		346.126,016.681,046.250,076.805,106.375,136.930,167.485,197.054,227.610,257.179,287.734,318.289,
		347.112,017.667,047.236,077.791,107.361,137.916,168.471,198.040,228.595,258.165,288.720,319.275,
		348.097,018.652,048.222,078.777,108.346,138.901,169.456,199.026,229.581,259.150,289.705,320.260,
		349.083,019.638,049.207,079.762,109.332,139.887,170.442,200.011,230.566,260.136,290.691,321.246,
		350.068,020.624,050.193,080.748,110.317,140.873,171.428,200.997,231.552,261.122,291.677,322.232,
		351.054,021.609,051.179,081.734,111.303,141.858,172.413,201.983,232.538,262.107,292.662,323.217,
		352.040,022.595,052.164,082.719,112.289,142.844,173.399,202.968,233.523,263.093,293.648,324.203,
		353.025,023.581,053.150,083.705,113.274,143.829,174.385,203.954,234.509,264.078,294.634,325.189,
		354.011,024.566,054.136,084.691,114.260,144.815,175.370,204.940,235.495,265.064,295.619,326.174,
		354.997,025.552,055.121,085.676,115.246,145.801,176.356,205.925,236.480,266.050,296.605,327.160,
		355.982,026.537,056.107,086.662,116.231,146.786,177.341,206.911,237.466,267.035,297.590,328.146,
		356.968,027.523,057.093,087.648,117.217,147.772,178.327,207.897,238.452,268.021,298.576,329.131,
		357.954,028.509,058.078,088.633,118.203,148.758,179.313,208.882,239.437,269.007,299.562,330.117,
		358.939,029.494,059.064,089.619,119.188,149.743,180.298,209.868,240.423,269.992,300.547,331.102,
		359.925,030.480,060.049,090.605,120.174,150.729,181.284,210.854,241.409,270.978,301.533,332.088,
		000.911,031.466,061.035,091.590,121.160,151.715,182.270,211.839,242.394,271.964,302.519,333.074,
		001.896,032.451,062.021,092.576,122.145,152.700,183.255,212.825,243.380,272.949,303.504,334.059,
		002.882,033.437,063.006,093.561,123.131,153.686,184.241,213.810,244.366,273.935,304.490,335.045,
		003.868,034.423,063.992,094.547,124.117,154.672,185.227,214.796,245.351,274.921,305.476,336.031,
		004.853,035.408,064.978,095.533,125.102,155.657,186.212,215.782,246.337,275.906,306.461,337.016,
		005.839,036.394,065.963,096.518,126.088,156.643,187.198,216.767,247.322,276.892,307.447,338.002,
		006.824,037.380,066.949,097.504,127.073,157.629,188.184,217.753,248.308,277.878,308.433,338.988,
		007.810,038.365,067.935,098.490,128.059,158.614,189.169,218.739,249.294,278.863,309.418,339.100,
		008.796,038.365,068.920,098.490,129.045,159.600,189.169,219.724,249.294,279.849,310.404,339.226,
		};
        static double[] sun_1_col = new double[] { -001.157, -000.386, 000.386, 001.157 };
        static double[] sun_1_row = new double[]
        {
            -001.070, 002.015, 005.101, 008.186,
            011.271, 014.356, 017.441, 020.526, 023.611, 026.697
        };

        static double[] sun_2_col = new double[] { 000.322, 000.107, -000.107, -000.322 };
        static double[] sun_2_row = new double[]
        {
            -000.577,-000.449,-000.320,-000.192,
            -000.064, 000.064, 000.192, 000.320, 000.449, 000.577
        };
        static double[] sun_3_row = new double[]{
		-000.370,-000.339,-000.309,-000.278,-000.247,-000.216,-000.185,-000.154,
		-000.123,-000.093,-000.062,-000.031,+000.000,+000.031,+000.062,+000.093,
		+000.123,+000.154,+000.185,+000.216,+000.247,+000.278,+000.309,+000.339,
		+000.370 };
        static double[] sun_3_col = new double[] { +000.358, +000.119, -000.119, -000.358 };
        static double[] sun_4_row = new double[] {251.97,258.85,265.73,272.61,279.49
		,286.37,293.25,300.14,307.02,313.90};
        static double[] sun_4_col = new double[] { -002.58, -000.86, 000.86, 002.58 };
        static double[] sun_5_row = new double[] {-000.83,-000.76,-000.69,-000.62,
		-000.55,-000.48,-000.41,-000.34,
		-000.28,-000.21,-000.14,-000.07,
		+000.00,+000.07,+000.14,+000.21,
		+000.28,+000.34,+000.41,+000.48,
		+000.55,+000.62,+000.69,+000.76,
		+000.83 };
        static double[] sun_5_col = new double[] { -000.03, -000.01, 000.01, +000.03 };





        public GPSunMethodSimple()
        {
        }

        public GPSunMethodSimple(GPSunMethodSimple sd)
            : base(sd)
        {
        }

        public override string ToString()
        {
            if (rise != null)
                return rise.ToString();
            return "";
        }



        // find mean ecliptic longitude of the sun for your chosen day
        //

        public double SunGetMeanLong(int year, int month, int day)
        {


            double mel = 0.0;

            if ((month > 12) || (month < 1) || (day < 1) || (day > 31))
                return -1.0;
            mel = sun_long[(day - 1) * 12 + (month + 9) % 12];

            int y, yy;

            if (month < 3)
            {
                y = (year - 1) / 100;
                yy = (year - 1) % 100;
            }
            else
            {
                y = year / 100;
                yy = year % 100;
            }

            if (y <= 15)
            {
                mel += sun_1_col[y % 4] + sun_1_row[y / 4];
            }
            else if (y < 40)
            {
                mel += sun_2_col[y % 4] + sun_2_row[y / 4];
            }


            mel += sun_3_col[yy % 4] + sun_3_row[yy / 4];

            return mel;
        }


        // finds ecliptic longitude of perigee of the sun 
        // for the mean summer solstice of your chosen year 
        // (and effectively for the entire year)

        public double SunGetPerigee(int year, int month, int day)
        {

            double per = 0.0;

            if ((month > 12) || (month < 1) || (day < 1) || (day > 31))
                return -1.0;

            int y, yy;

            if (month < 3)
            {
                y = (year - 1) / 100;
                yy = (year - 1) % 100;
            }
            else
            {
                y = year / 100;
                yy = year % 100;
            }

            per = sun_4_row[y / 4] + sun_4_col[y % 4];
            per += sun_5_row[yy / 4] + sun_5_col[yy % 4];

            return per;
        }


        public double SetDegTime(double d)
        {
            return d / 360.0;
        }

        public double AngleRadiusDeg
        {
            get
            {
                return GPMath.rad2deg(2*Math.Asin(GPSunMethodSimple.SUN_RADIUS / rise.distance));
            }
        }


        public override double getLongitude(int year, int month, int day, int hour, int minute, int second)
        {
            // mean ecliptic longitude of the sun 
            double mel = SunGetMeanLong(year, month, day) + (hour*15.0 + minute/4.0 + second/240.0) / 365.25;
            // ecliptic longitude of perigee 
            double elp = SunGetPerigee(year, month, day);

            // mean anomaly of the sun
            double M = mel - elp;

            // equation of center
            double C = 1.9148 * GPMath.sinDeg(M)
                     + 0.02 * GPMath.sinDeg(2 * M)
                     + 0.0003 * GPMath.sinDeg(3 * M);

            // ecliptic longitude of the sun
            return mel + C;
        }

        public void calculateCoordinatesMethodC(GPGregorianTime vct, double DayHours, SunCoords sun)
        {
            double DG = GPMath.pi / 180;
            double RAD = 180 / GPMath.pi;

            // mean ecliptic longitude of the sun 
            double mel = SunGetMeanLong(vct.getYear(), vct.getMonth(), vct.getDay()) + (360 / 365.25) * DayHours / 360.0;
            // ecliptic longitude of perigee 
            double elp = SunGetPerigee(vct.getYear(), vct.getMonth(), vct.getDay());

            // mean anomaly of the sun
            double M = mel - elp;

            // equation of center
            double C = 1.9148 * GPMath.sinDeg(M)
                     + 0.02 * GPMath.sinDeg(2 * M)
                     + 0.0003 * GPMath.sinDeg(3 * M);

            // ecliptic longitude of the sun
            double els = 0;
            sun.eclipticalLongitude = els = mel + C;

            // declination of the sun
            sun.declination = GPMath.arcsinDeg(0.397948 * GPMath.sinDeg(sun.eclipticalLongitude));

            // right ascension of the sun
            sun.rightAscession = els - RAD * Math.Atan2(Math.Sin(2 * els * DG), 23.2377 + Math.Cos(2 * DG * els));

            // equation of time
            sun.equationOfTime = sun.rightAscession - mel;
            //equationOfTime = GPAstroEngine.getEquationOfTime(julianDay, right_asc_deg);
            //Debugger.Log(0,"", String.Format("{1}: EoTdiff = {0}\n", vct.getShortDateString(), equationOfTime - (right_asc_deg - mel)));
        }

        private void calculateRiseSetMethodA(GPGregorianTime vct, GPLocation ed, double DayHours, SunCoords sun)
        {
            double DG = GPMath.pi / 180;
            double RAD = 180 / GPMath.pi;


            double time = vct.getJulianLocalNoon() - 0.5 + DayHours / 360 - vct.getTimeZoneOffsetHours() / 24.0;
            double dLatitude = ed.GetLatitudeNorthPositive();
            double dLongitude = ed.GetLongitudeEastPositive();
            //Debugger.Log(0,"",String.Format("{0}     {1} {2}\n", vct.getLongDateString(), dLatitude, dLongitude));
            // definition of event
            // eventdef = 0.0;
            // civil twilight eventdef = 0.10453;
            // nautical twilight eventdef = 0.20791;
            // astronomical twilight eventdef = 0.30902;
            // center of the sun on the horizont eventdef = 0.01454;
            double eventdef = 0.01454;

            double x = GPMath.tanDeg(dLatitude) * GPMath.tanDeg(sun.declination) + eventdef / (GPMath.cosDeg(dLatitude) * GPMath.cosDeg(sun.declination));

            if (x < -1.0 || x > 1.0)
            {
                // initial values for the case
                // that no rise no set for that day
                sun.sunrise_deg = -360;
                sun.noon_deg = -360;
                sun.sunset_deg = -360;
                return;
            }

            double hourAngle = GPMath.arcsinDeg(x);

            // time of sunrise
            sun.sunrise_deg = 90.0 - dLongitude - hourAngle + sun.equationOfTime;
            // time of noon
            sun.noon_deg = 180.0 - dLongitude + sun.equationOfTime;
            // time of sunset
            sun.sunset_deg = 270.0 - dLongitude + hourAngle + sun.equationOfTime;

        }


        public void calculateRiseSet(GPGregorianTime vct, GPLocation ed, double DayHours, SunCoords coords)
        {
            calculateCoordinatesMethodC(vct, DayHours, coords);
            calculateRiseSetMethodA(vct, ed, DayHours, coords);
        }

        public SunCoords calculateRise(GPGregorianTime vct, GPLocation ed)
        {
            SunCoords coords = new SunCoords();
            calculateRiseSet(vct, ed, 0, coords);
            calculateRiseSet(vct, ed, coords.sunrise_deg - 180, coords);
            calculateRiseSet(vct, ed, coords.sunrise_deg - 180, coords);
            return coords;
        }

        public SunCoords calculateSet(GPGregorianTime vct, GPLocation ed)
        {
            SunCoords coords = new SunCoords();
            calculateRiseSet(vct, ed, 0, coords);
            calculateRiseSet(vct, ed, coords.sunset_deg - 180, coords);
            calculateRiseSet(vct, ed, coords.sunset_deg - 180, coords);
            return coords;
        }

        public override void SunCalc(GPGregorianTime vct, GPLocation earth)
        {
            arunodaya = new GPBodyData(vct);
            rise = new GPBodyData(vct);
            noon = new GPBodyData(vct);
            set = new GPBodyData(vct);


            // first calculation
            // for 12:00 universal time
            GPSun s_rise = new GPSun();
            SunCoords s_risec = s_rise.calculateRise(vct, earth);

            // first calculation
            // for 12:00 universal time
            GPSun s_set = new GPSun();
            SunCoords s_setc = s_set.calculateSet(vct, earth);

            // calculate times
            arunodaya.eclipticalLongitude = s_risec.eclipticalLongitude - (24.0 / 365.25);
            arunodaya.setDayHours(SetDegTime(s_risec.sunrise_deg - 24.0 + earth.getTimeZoneOffsetHours() * 15.0));
            
            rise.eclipticalLongitude = s_risec.eclipticalLongitude;
            rise.rightAscession = s_risec.rightAscession;
            rise.setDayHours(SetDegTime(s_risec.sunrise_deg + earth.getTimeZoneOffsetHours() * 15.0));

            noon.setDayHours(SetDegTime((s_setc.sunset_deg + s_risec.sunrise_deg) / 2 + earth.getTimeZoneOffsetHours() * 15.0));

            set.setDayHours(SetDegTime(s_setc.sunset_deg + earth.getTimeZoneOffsetHours() * 15.0));
            set.eclipticalLongitude = s_setc.eclipticalLongitude;

            // finally calculate length of the daylight
            DayLength = (s_setc.sunset_deg - s_risec.sunrise_deg) * 24.0 / 360.0;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPMath
    {
        // input value: arc in degrees
        public static double pi = Math.PI;
        public static double pi2 = Math.PI * 2.0;
        public static double rads = Math.PI / 180.0;

        public static double frac(double x)
        {
            return x - Math.Floor(x);
        }

        public static double cosDeg(double x)
        {
            return Math.Cos(x * rads);
        }
 
        // input value: arc in degrees
 
        public static double sinDeg(double x)
        {
            return Math.Sin(x * rads);
        }
 
        // input value: arc in degrees
        // it is calculating arctan(x/y)
        // with testing values
 
        public static double arctan2Deg(double x, double y)
        {
            return Math.Atan2(x, y) / rads;
        }
 
        // input value: arc in degrees
        // output value: tangens
 
        public static double tanDeg(double x)
        {
            return Math.Tan(x * rads);
        }
 
        // input value: -1.0 .. 1.0
        // output value: -180 deg .. 180 deg
 
        public static double arcsinDeg(double x)
        {
            return Math.Asin(x) / rads;
        }
 
        public static double arccosDeg(double x)
        {
            return Math.Acos(x) / rads;
        }
 
        public static double arctanDeg(double x)
        {
            return Math.Atan(x) / rads;
        }
 

        // modulo 360
 
        public static double putIn360(double id)
        {
            return (id / 360 - Math.Floor(id / 360)) * 360;
            /*double d = id;
            while (d >= 360.0)
                d -= 360.0;
            while (d < 0.0)
                d += 360.0;
            return d;*/
        }
 
        // modulo 360 but in range -180deg .. 180deg
        // used for comparision values around 0deg
        // so difference between 359deg and 1 deg
        // is not 358 degrees, but 2 degrees (because 359deg = -1 deg)
 
        public static double putIn180(double in_d)
        {
            double d = putIn360(in_d);

            if (d > 180)
                d -= 360;
 
            return d;
        }
 
        // sign of the number
        // -1: number is less than zero
        // 0: number is zero
        // +1: number is greater than zero
 
        public static int getSign(double d)
        {
            if (d > 0.0)
                return 1;
            if (d < 0.0)
                return -1;
            return 0;
        }
 
        public static double deg2rad(double x)
        {
           return x * rads;
        }

        public static double rad2deg(double x)
        {
            return x / rads;
        }

        public static double arcDistance(double lon1, double lat1, double lon2, double lat2)
        {
            lat1 = Math.PI / 2 - lat1;
            lat2 = Math.PI / 2 - lat2;
            return Math.Acos(Math.Cos(lat1) * Math.Cos(lat2) + Math.Sin(lat1) * Math.Sin(lat2) * Math.Cos(lon1 - lon2));
        }

        public static double arcDistanceDeg(double lon1, double lat1, double lon2, double lat2)
        {
            return rad2deg(arcDistance(deg2rad(lon1), deg2rad(lat1), deg2rad(lon2), deg2rad(lat2)));
        }

        public static double putIn1(double d)
        {
            return d - Math.Floor(d);
        }
    }
}



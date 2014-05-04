using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public delegate GPVSOPEntry TVSOPCalcFunc(int nr, int index);

    /// <summary>
    /// Based on VSOP87 (Variations Seculaires des Orbites Planetaires)
    /// </summary>
    public class GPMeeusVSOP
    {
        public const double J2000 = 2451545.0;

        protected double FDate;
        public virtual GPVSOPEntry LongitudeFactor(int nr, int index) { return null; }

        public virtual GPVSOPEntry LatitudeFactor(int nr, int index) { return null; }


        public virtual GPVSOPEntry RadiusFactor(int nr, int index) { return null; }

        protected double CalcLongitude()
        {
            return Calc(LongitudeFactor);
        }

        protected double CalcLatitude()
        {
            return Calc(LatitudeFactor);
        }

        protected double CalcRadius()
        {
            return Calc(RadiusFactor);
        }
        protected double Calc(TVSOPCalcFunc factor)
        {
            double t;
            double current;
            double[] r = new double[6];
            int i, j;

            t = Tau();
            for (j = 0; j <= 5; j++)
            {
                r[j] = 0;
                i = 0;
                GPVSOPEntry e;
                do
                {
                    e = factor(j, i);
                    if (e != null)
                    {
                        current = e.A * Math.Cos(e.B + e.C * t);
                        r[j] += current;
                        i++;
                    }
                } while (e != null);
            }
            return (r[0] + t * (r[1] + t * (r[2] + t * (r[3] + t * (r[4] + t * r[5]))))) * 1e-8;
        }

        protected void SetDate(double value)
        {
            FDate = value;
        }
        

        protected double Tau()
        {
            return (FDate - J2000 /*- 0.5*/) / 365250.0;
        }

        public void DynamicToFK5(ref double longitude, ref double latitude)
        {
            double lprime, t;
            double delta_l, delta_b;
            t = 10 * Tau();
            lprime = longitude + GPMath.deg2rad(-1.397 - 0.00031 * t) * t;
            delta_l = -GPMath.deg2rad(0.09033 / 3600) + GPMath.deg2rad(0.03916 / 3600) * (Math.Cos(lprime) + Math.Sin(lprime)) * Math.Tan(latitude);
            delta_b = GPMath.deg2rad(0.03916 / 3600) * (Math.Cos(lprime) - Math.Sin(lprime));
            longitude = longitude + delta_l;
            latitude = latitude + delta_b;
        }

        public double Longitude
        {
            get { return CalcLongitude(); }
        }

        public double Latitude
        {
            get { return CalcLatitude(); }
        }

        public double Radius
        {
            get { return CalcRadius(); }
        }

        public double Date
        {
            set { SetDate(value); }
        }

    }
}

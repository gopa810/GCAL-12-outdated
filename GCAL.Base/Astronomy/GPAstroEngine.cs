using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GCAL.Base
{
    public enum TMoonPhase
    {
        Newmoon = 0,
        WaxingCrescrent = 1,
        FirstQuarter = 2,
        WaxingGibbous = 3,
        Fullmoon = 4,
        WaningGibbous = 5,
        LastQuarter = 6,
        WaningCrescent = 7
    }

    public enum TRiseSet
    {
        RISE,
        TRANSIT,
        SET,
        NONE
    }

    public enum TSeason
    {
        Winter, Spring, Summer, Autumn
    }

    public enum TEclipse
    {
        none = 0, partial, noncentral, circular, circulartotal, total, halfshadow
    }

    public enum TSolarTerm
    {
        st_z2 = 0, st_j3, st_z3, st_j4, st_z4, st_j5, st_z5, st_j6, st_z6,
        st_j7, st_z7, st_j8, st_z8, st_j9, st_z9, st_j10, st_z10,
        st_j11, st_z11, st_j12, st_z12, st_j1, st_z1, st_j2
    }

    public class GPAstroEngine
    {
        public static double AU = 149597869; // astronomical unit in km 
        public static double mean_lunation = 29.530589;  // Mean length of a month 
        public static double tropic_year = 365.242190;   // Tropic year length 
        public const double EARTH_RADIUS = 6378.15;     // Radius of the earth 
        public const double MOON_RADIUS = 1737.4;
        public const double SUN_RADIUS = 695500;
        public static bool LowPrecision = false;
        public static double J1999 = 2451180.0;
        public static double J2000 = 2451545.0;
        public const double JFirstLunation = 2423436.0;

        private static int[,] cep_arg_mul = new int[,] 
        {
            { 0, 0, 0, 0, 1},
            {-2, 0, 0, 2, 2},
            { 0, 0, 0, 2, 2},
            { 0, 0, 0, 0, 2},
            { 0, 1, 0, 0, 0},
            { 0, 0, 1, 0, 0},
            {-2, 1, 0, 2, 2},
            { 0, 0, 0, 2, 1},
            { 0, 0, 1, 2, 2},
            {-2,-1, 0, 2, 2},
            {-2, 0, 1, 0, 0},
            {-2, 0, 0, 2, 1},
            { 0, 0,-1, 2, 2},
            { 2, 0, 0, 0, 0},
            { 0, 0, 1, 0, 1},
            { 2, 0,-1, 2, 2},
            { 0, 0,-1, 0, 1},
            { 0, 0, 1, 2, 1},
            {-2, 0, 2, 0, 0},
            { 0, 0,-2, 2, 1},
            { 2, 0, 0, 2, 2},
            { 0, 0, 2, 2, 2},
            { 0, 0, 2, 0, 0},
            {-2, 0, 1, 2, 2},
            { 0, 0, 0, 2, 0},
            {-2, 0, 0, 2, 0},
            { 0, 0,-1, 2, 1},
            { 0, 2, 0, 0, 0},
            { 2, 0,-1, 0, 1},
            {-2, 2, 0, 2, 2},
            { 0, 1, 0, 0, 1},
            {-2, 0, 1, 0, 1},
            { 0,-1, 0, 0, 1},
            { 0, 0, 2,-2, 0},
            { 2, 0,-1, 2, 1},
            { 2, 0, 1, 2, 2},
            { 0, 1, 0, 2, 2},
            {-2, 1, 1, 0, 0},
            { 0,-1, 0, 2, 2},
            { 2, 0, 0, 2, 1},
            { 2, 0, 1, 0, 0},
            {-2, 0, 2, 2, 2},
            {-2, 0, 1, 2, 1},
            { 2, 0,-2, 0, 1},
            { 2, 0, 0, 0, 1},
            { 0,-1, 1, 0, 0},
            {-2,-1, 0, 2, 1},
            {-2, 0, 0, 0, 1},
            { 0, 0, 2, 2, 1},
            {-2, 0, 2, 0, 1},
            {-2, 1, 0, 2, 1},
            { 0, 0, 1,-2, 0},
            {-1, 0, 1, 0, 0},
            {-2, 1, 0, 0, 0},
            { 1, 0, 0, 0, 0},
            { 0, 0, 1, 2, 0},
            { 0, 0,-2, 2, 2},
            {-1,-1, 1, 0, 0},
            { 0, 1, 1, 0, 0},
            { 0,-1, 1, 2, 2},
            { 2,-1,-1, 2, 2},
            { 0, 0, 3, 2, 2},
            { 2,-1, 0, 2, 2}
		};

        private static int[,] cep_arg_phi = new int[,] {
		 {-171996,-1742},
		 { -13187,  -16},
		 {  -2274,   -2},
		 {   2062,    2},
		 {   1426,  -34},
		 {    712,    1},
		 {   -517,   12},
		 {   -386,   -4},
		 {   -301,    0},
		 {    217,   -5},
		 {   -158,    0},
		 {    129,    1},
		 {    123,    0},
		 {     63,    0},
		 {     63,    1},
		 {    -59,    0},
		 {    -58,   -1},
		 {    -51,    0},
		 {     48,    0},
		 {     46,    0},
		 {    -38,    0},
		 {    -31,    0},
		 {     29,    0},
		 {     29,    0},
		 {     26,    0},
		 {    -22,    0},
		 {     21,    0},
		 {     17,   -1},
		 {     16,    0},
		 {    -16,    1},
		 {    -15,    0},
         {    -13,    0},
         {    -12,    0},
         {     11,    0},
         {    -10,    0},
         {     -8,    0},
         {      7,    0},
         {     -7,    0},
         {     -7,    0},
         {     -7,    0},
         {      6,    0},
         {      6,    0},
         {      6,    0},
         {     -6,    0},
         {     -6,    0},
         {      5,    0},
         {     -5,    0},
         {     -5,    0},
         {     -5,    0},
         {      4,    0},
         {      4,    0},
         {      4,    0},
         {     -4,    0},
         {     -4,    0},
         {     -4,    0},
         {      3,    0},
         {     -3,    0},
         {     -3,    0},
         {     -3,    0},
         {     -3,    0},
         {     -3,    0},
         {     -3,    0},
         {     -3,    0}
		};
        private static int[,] cep_arg_eps = new int[,] {
		{ 92025,   89},
		{  5736,  -31},
		{   977,   -5},
		{  -895,    5},
		{    54,   -1},
		{    -7,    0},
		{   224,   -6},
		{   200,    0},
		{   129,   -1},
		{   -95,    3},
		{     0,    0},
		{   -70,    0},
		{   -53,    0},
		{     0,    0},
		{   -33,    0},
		{    26,    0},
		{    32,    0},
		{    27,    0},
		{     0,    0},
		{   -24,    0},
		{    16,    0},
		{    13,    0},
		{     0,    0},
		{   -12,    0},
		{     0,    0},
		{     0,    0},
		{   -10,    0},
		{     0,    0},
		{    -8,    0},
		{     7,    0},
		{     9,    0},
        {     7,    0},
        {     6,    0},
        {     0,    0},
        {     5,    0},
        {     3,    0},
        {    -3,    0},
        {     0,    0},
        {     3,    0},
        {     3,    0},
        {     0,    0},
        {    -3,    0},
        {    -3,    0},
        {     3,    0},
        {     3,    0},
        {     0,    0},
        {     3,    0},
        {     3,    0},
        {     3,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0},
        {     0,    0}
	};

        private static int[,] arg_lr = new int[,] {
		 { 0, 0, 1, 0},		 { 2, 0,-1, 0},		 { 2, 0, 0, 0},		 { 0, 0, 2, 0},
		 { 0, 1, 0, 0},		 { 0, 0, 0, 2},		 { 2, 0,-2, 0},		 { 2,-1,-1, 0},
		 { 2, 0, 1, 0},		 { 2,-1, 0, 0},		 { 0, 1,-1, 0},		 { 1, 0, 0, 0},
		 { 0, 1, 1, 0},		 { 2, 0, 0,-2},		 { 0, 0, 1, 2},		 { 0, 0, 1,-2},
		 { 4, 0,-1, 0},		 { 0, 0, 3, 0},		 { 4, 0,-2, 0},		 { 2, 1,-1, 0},
		 { 2, 1, 0, 0},		 { 1, 0,-1, 0},		 { 1, 1, 0, 0},		 { 2,-1, 1, 0},
		 { 2, 0, 2, 0},		 { 4, 0, 0, 0},		 { 2, 0,-3, 0},		 { 0, 1,-2, 0},
		 { 2, 0,-1, 2},		 { 2,-1,-2, 0},		 { 1, 0, 1, 0},		 { 2,-2, 0, 0},
		 { 0, 1, 2, 0},		 { 0, 2, 0, 0},		 { 2,-2,-1, 0},		 { 2, 0, 1,-2},
		 { 2, 0, 0, 2},		 { 4,-1,-1, 0},		 { 0, 0, 2, 2},		 { 3, 0,-1, 0},
		 { 2, 1, 1, 0},		 { 4,-1,-2, 0},		 { 0, 2,-1, 0},		 { 2, 2,-1, 0},
		 { 2, 1,-2, 0},		 { 2,-1, 0,-2},		 { 4, 0, 1, 0},		 { 0, 0, 4, 0},
		 { 4,-1, 0, 0},		 { 1, 0,-2, 0},		 { 2, 1, 0,-2},		 { 0, 0, 2,-2},
		 { 1, 1, 1, 0},		 { 3, 0,-2, 0},		 { 4, 0,-3, 0},		 { 2,-1, 2, 0},
		 { 0, 2, 1, 0},		 { 1, 1,-1, 0},		 { 2, 0, 3, 0},		 { 2, 0,-1,-2}
	   };

        private static int[,] arg_b = new int[,] {
         { 0, 0, 0, 1},		 { 0, 0, 1, 1},		 { 0, 0, 1,-1},		 { 2, 0, 0,-1},
		 { 2, 0,-1, 1},		 { 2, 0,-1,-1},		 { 2, 0, 0, 1},		 { 0, 0, 2, 1},
		 { 2, 0, 1,-1},		 { 0, 0, 2,-1},
		 { 2,-1, 0,-1},		 { 2, 0,-2,-1},		 { 2, 0, 1, 1},		 { 2, 1, 0,-1},
		 { 2,-1,-1, 1},		 { 2,-1, 0, 1},		 { 2,-1,-1,-1},		 { 0, 1,-1,-1},
		 { 4, 0,-1,-1},		 { 0, 1, 0, 1},		 { 0, 0, 0, 3},		 { 0, 1,-1, 1},
		 { 1, 0, 0, 1},		 { 0, 1, 1, 1},		 { 0, 1, 1,-1},		 { 0, 1, 0,-1},
		 { 1, 0, 0,-1},		 { 0, 0, 3, 1},		 { 4, 0, 0,-1},		 { 4, 0,-1, 1},
		 { 0, 0, 1,-3},		 { 4, 0,-2, 1},		 { 2, 0, 0,-3},		 { 2, 0, 2,-1},
		 { 2,-1, 1,-1},		 { 2, 0,-2, 1},		 { 0, 0, 3,-1},		 { 2, 0, 2, 1},
		 { 2, 0,-3,-1},		 { 2, 1,-1, 1},		 { 2, 1, 0, 1},		 { 4, 0, 0, 1},
		 { 2,-1, 1, 1},		 { 2,-2, 0,-1},		 { 0, 0, 1, 3},		 { 2, 1, 1,-1},
		 { 1, 1, 0,-1},		 { 1, 1, 0, 1},		 { 0, 1,-2,-1},		 { 2, 1,-1,-1},
		 { 1, 0, 1, 1},		 { 2,-1,-2,-1},		 { 0, 1, 2, 1},		 { 4, 0,-2,-1},
		 { 4,-1,-1,-1},		 { 1, 0, 1,-1},		 { 4, 0, 1,-1},		 { 1, 0,-1,-1},
		 { 4,-1, 0,-1},		 { 2,-2, 0, 1}
		};
        private static int[] sigma_r = new int[] {
	   -20905355,   		-3699111,     		-2955968,     		 -569925,
		   48888,     		   -3149,     		  246158,     		 -152138,
		 -170733,     		 -204586,     		 -129620,     		  108743,
		  104755,     		   10321,     			   0,       		   79661,
		  -34782,     		  -23210,     		  -21636,     		   24208,
		   30824,     		   -8379,     		  -16675,     		  -12831,
		  -10445,     		  -11650,     		   14403,     		   -7003,
			   0,       		   10056,     			6322,       		   -9884,
			5751,       			   0,       		   -4950,     			4130,
			   0,       		   -3958,     			   0,       			3258,
			2616,       		   -1897,     		   -2117,     			2354,
			   0,       			   0,       		   -1423,     		   -1117,
		   -1571,     		   -1739,     			   0,       		   -4421,
			   0,       			   0,       			   0,       			   0,
			1165,       			   0,       			   0,       			8752
				  };

        private static int[] sigma_l = new int[] {
		6288774,  		1274027,    	 658314,  		 213618,
		-185116,  		-114332,    		  58793,  		  57066,
		  53322,  		  45758,    		 -40923,  		 -34720,
		 -30383,  		  15327,    		 -12528,  		  10980,
		  10675,  		  10034,    		   8548,  		  -7888,
		  -6766,  		  -5163,    		   4987,  		   4036,
		   3994,  		   3861,    		   3665,  		  -2689,
		  -2602,  		   2390,    		  -2348,  		   2236,
		  -2120,  		  -2069,    		   2048,  		  -1773,
		  -1595,  		   1215,    		  -1110,  		   -892,
		   -810,  			759,      		   -713,  		   -700,
			691,    			596,      			549,    			537,
			520,    		   -487,    		   -399,  		   -381,
			351,    		   -340,    			330,    			327,
		   -323,  			299,      			294,    			  0
		};
        private static int[] sigma_b = new int[] {
		5128122,   		 280602,   		 277693,   		 173237,
		  55413,   		  46271,   		  32573,   		  17198,
		   9266,   		   8822,   		   8216,   		   4324,
		   4200,   		  -3359,   		   2463,   		   2211,
		   2065,   		  -1870,   		   1828,   		  -1794,
		  -1749,   		  -1565,   		  -1491,   		  -1475,
		  -1410,   		  -1344,   		  -1335,   		   1107,
		   1021,   			833,     			777,     			671,
			607,     			596,     			491,     		   -451,
			439,     			422,     			421,     		   -366,
		   -351,   			331,     			315,     			302,
		   -283,   		   -229,   			223,     			223,
		   -220,   		   -220,   		   -185,   			181,
		   -177,   			176,     			166,     		   -164,
			132,     		   -119,   			115,     			107
		};

        public static void calc_coord(double jd, GPMeeusVSOP obj, out double l, out double b, out double r)
        {
            r = l = b = 0.0;
            try
            {
                obj.Date = jd;
                r = obj.Radius;
                l = obj.Longitude;
                b = obj.Latitude;
                obj.DynamicToFK5(ref l, ref b);
            }
            finally
            {
            }

            l = GPMath.putIn360(GPMath.rad2deg(l));
            b = GPMath.rad2deg(b);
        }

        public static void earth_coord(double jd, out double l, out double b, out double r)
        {
            calc_coord(jd, new GPMeeusEarth(), out l, out b, out r);
        }
        public static void jupiter_coord(double jd, out double l, out double b, out double r)
        {
            calc_coord(jd, new GPMeeusJupiter(), out l, out b, out r);
        }

        /// <summary>
        /// Nutation and Obliquity of the ecliptic.
        /// Based on Chapter 21.
        /// </summary>
        /// <param name="date">Julian Ephemeris Date</param>
        /// <param name="delta_phi">Output: Nutation in longitude</param>
        /// <param name="epsilon">Output: Obliquity of ecliptic</param>
        public static void calc_epsilon_phi(double date, out double delta_phi, out double epsilon)
        {

            double t, omega;
            double d, m, ms, f, s;
            int i;
            double epsilon_0, delta_epsilon;

            // date is supposed to be JDE (Julian Ephemeris Day)
            // 21.1
            t = (date - J2000) / 36525;

            // longitude of rising knot
            omega = GPMath.putIn360(125.04452 + (-1934.136261 + (0.0020708 + t / 450000) * t) * t);

            if (LowPrecision)
            {
                double l, ls;
                // (*@/// delta_phi and delta_epsilon - low accuracy *)
                // mean longitude of sun (l) and moon (ls)
                l = 280.4665 + 36000.7698 * t;
                ls = 218.3165 + 481267.8813 * t;

                //(* correction due to nutation *)
                delta_epsilon = (9.20 * GPMath.cosDeg(omega) + 0.57 * GPMath.cosDeg(2 * l) + 0.10 * GPMath.cosDeg(2 * ls) - 0.09 * GPMath.cosDeg(2 * omega))/3600;

                // longitude correction due to nutation
                delta_phi = (-17.20 * GPMath.sinDeg(omega) - 1.32 * GPMath.sinDeg(2 * l) - 0.23 * GPMath.sinDeg(2 * ls) + 0.21 * GPMath.sinDeg(2 * omega))/3600;
            }
            else
            {
                // mean elongation of moon to sun
                d = GPMath.putIn360(297.85036 + (445267.111480 + (-0.0019142 + t / 189474) * t) * t);

                // mean anomaly of the sun
                m = GPMath.putIn360(357.52772 + (35999.050340 + (-0.0001603 - t / 300000) * t) * t);

                // mean anomaly of the moon
                ms = GPMath.putIn360(134.96298 + (477198.867398 + (0.0086972 + t / 56250) * t) * t);

                // argument of the latitude of the moon
                f = GPMath.putIn360(93.27191 + (483202.017538 + (-0.0036825 + t / 327270) * t) * t);

                delta_phi = 0;
                delta_epsilon = 0;

                for (i = 0; i < cep_arg_mul.GetLength(0); i++)
                {
                    s = cep_arg_mul[i, 0] * d
                       + cep_arg_mul[i, 1] * m
                       + cep_arg_mul[i, 2] * ms
                       + cep_arg_mul[i, 3] * f
                       + cep_arg_mul[i, 4] * omega;
                    delta_phi += (cep_arg_phi[i, 0] + cep_arg_phi[i, 1] * t * 0.1) * GPMath.sinDeg(s);
                    delta_epsilon += (cep_arg_eps[i, 0] + cep_arg_eps[i, 1] * t * 0.1) * GPMath.cosDeg(s);
                }

                delta_phi = delta_phi * 0.0001 / 3600;
                delta_epsilon = delta_epsilon * 0.0001 / 3600;
            }
            // angle of ecliptic
            epsilon_0 = 84381.448 + (-46.8150 + (-0.00059 + 0.001813 * t) * t) * t;

            epsilon = epsilon_0/3600 + delta_epsilon;

        }

        /// <summary>
        /// Transformation from ecliptical to equatorial coordinates
        /// Based on Chapter 12
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="date">Julian Ephemeris day</param>
        public static void CalculateEquatorial(GPCelestialBodyCoordinates coord, double date)
        {
            //var
            double obliquity; //: extended;
            double nutation; //: extended;
            double alpha, delta; //: extended;

            nutation = coord.delta_phi;
            obliquity = coord.epsilon;
            //calc_epsilon_phi(date, out nutation, out obliquity);
            coord.eclipticalLongitude = GPMath.putIn360(coord.eclipticalLongitude + nutation);

            // 12.3
            alpha = GPMath.arctan2Deg(GPMath.sinDeg(coord.eclipticalLongitude) * GPMath.cosDeg(obliquity) - GPMath.tanDeg(coord.eclipticalLatitude) * GPMath.sinDeg(obliquity), GPMath.cosDeg(coord.eclipticalLongitude));
            // 12.4
            delta = GPMath.arcsinDeg(GPMath.sinDeg(coord.eclipticalLatitude) * GPMath.cosDeg(obliquity) + GPMath.cosDeg(coord.eclipticalLatitude) * GPMath.sinDeg(obliquity) * GPMath.sinDeg(coord.eclipticalLongitude));

            coord.right_ascession = GPMath.putIn360(alpha);
            coord.declination = GPMath.putIn180(delta);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="date"></param>
        /// <param name="longitude">+ east, - west</param>
        /// <param name="latitude"></param>
        public static void calcHorizontal(GPCelestialBodyCoordinates coord, GPObserver obs)
        {
            double h;

            h = GPMath.putIn360(coord.apparent_sidereal_time - obs.GetLongitudeWestPositive() - coord.right_ascession);
            //h = GPMath.put_in_360(coord.hour_angle - obs.GetLongitudeWestPositive());

            // 12.5
            coord.azimuth = GPMath.rad2deg(Math.Atan2(GPMath.sinDeg(h),
                                   GPMath.cosDeg(h) * GPMath.sinDeg(obs.GetLatitudeNorthPositive()) -
                                   GPMath.tanDeg(coord.declination) * GPMath.cosDeg(obs.GetLatitudeNorthPositive())));

            // 12.6
            coord.elevation = GPMath.rad2deg(Math.Asin(GPMath.sinDeg(obs.GetLatitudeNorthPositive()) * GPMath.sinDeg(coord.declination) +
                                    GPMath.cosDeg(obs.GetLatitudeNorthPositive()) * GPMath.cosDeg(coord.declination) * GPMath.cosDeg(h)));

        }


        /// <summary>
        /// Calculates sidereal time at Greenwich. 
        /// Based on Chapter 11 of Astronomical Algorithms.
        /// </summary>
        /// <param name="date">Julian Ephemeris Day</param>
        /// <returns>Sidereal time in degrees.</returns>
        public static double GetSiderealTime(double date)
        {
            double delta_phi, epsilon;

            return GetSiderealTime(date, out delta_phi, out epsilon);
        }

        /// <summary>
        /// Calculates sidereal time at Greenwich. 
        /// Based on Chapter 11 of Astronomical Algorithms.
        /// </summary>
        /// <param name="date">Julian Ephemeris Day</param>
        /// <returns>Sidereal time in degrees.</returns>
        public static double GetSiderealTime(double date, out double delta_phi, out double epsilon)
        {
            double t;
            //date = 2446896.30625;
            //jd = date;
            t = (date - GPAstroEngine.J2000) / 36525.0;
            GPAstroEngine.calc_epsilon_phi(date, out delta_phi, out epsilon);

            // 11.2
            double mean_sidereal_time = GPMath.putIn360(280.46061837 + 360.98564736629 * (date - GPAstroEngine.J2000) +
                             t * t * (0.000387933 - t / 38710000));

            return GPMath.putIn360(mean_sidereal_time + delta_phi * GPMath.cosDeg(epsilon));
        }

        /// <summary>
        /// Calculation of Sun coordinates
        /// </summary>
        /// <param name="date">Julian Ephemeris Day (Dynamic Time)</param>
        /// <returns></returns>
        public static GPCelestialBodyCoordinates sun_coordinate(double date)
        {
            double l, b, r;

            // calculate ecliptical coordinates
            earth_coord(date, out l, out b, out r);

            // convert earth coordinate to sun coordinate 
            l = l + 180;
            b = -b;

            // abberation
            // according Meeus Chapter 24.10
            l -= (20.4898 / 3600) / r ;

            GPCelestialBodyCoordinates result = new GPCelestialBodyCoordinates();
            result.eclipticalLongitude = GPMath.putIn360(l);
            result.eclipticalLatitude = b;
            result.distanceFromEarth = r * AU;
            result.SetSiderealTime(date);
            //result.SetJulianEphemerisDay(date);

            // initialize equatorial coordinates
            CalculateEquatorial(result, date);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jdate">Julian Ephemeris day value. Differs from Julian Day for DeltaT value. JDE is Dynamic Time.</param>
        /// <returns></returns>
        public static GPCelestialBodyCoordinates moon_coordinate(double jdate)
        {


            double t, d, m, ms, f, e, ls;// : extended;
            double sr, sl, sb, temp;// : extended;
            double a1, a2, a3; // : extended;
            double lambda, beta, delta; //: extended;
            int i; //: integer;


            t = (jdate - J2000) / 36525.0;

            //(* mean elongation of the moon  
            d = 297.8502042 + (445267.1115168 + (-0.0016300 + (1.0 / 545868 - 1.0 / 113065000 * t) * t) * t) * t;

            //(* mean anomaly of the sun  
            m = 357.5291092 + (35999.0502909 + (-0.0001536 + 1.0 / 24490000 * t) * t) * t;

            //(* mean anomaly of the moon  
            ms = 134.9634114 + (477198.8676313 + (0.0089970 + (1.0 / 69699 - 1.0 / 1471200 * t) * t) * t) * t;

            //(* argument of the longitude of the moon  
            f = 93.2720993 + (483202.0175273 + (-0.0034029 + (-1.0 / 3526000 + 1.0 / 863310000 * t) * t) * t) * t;

            //(* correction term due to excentricity of the earth orbit  
            e = 1.0 + (-0.002516 - 0.0000074 * t) * t;

            //(* mean longitude of the moon  
            ls = 218.3164591 + (481267.88134236 + (-0.0013268 + (1.0 / 538841 - 1.0 / 65194000 * t) * t) * t) * t;

            //(* arguments of correction terms  
            a1 = 119.75 + 131.849 * t;
            a2 = 53.09 + 479264.290 * t;
            a3 = 313.45 + 481266.484 * t;

            sr = 0;
            for (i = 0; i < 60; i++)
            {
                temp = sigma_r[i] * GPMath.cosDeg(arg_lr[i, 0] * d
                                       + arg_lr[i, 1] * m
                                       + arg_lr[i, 2] * ms
                                       + arg_lr[i, 3] * f);
                if (Math.Abs(arg_lr[i, 1]) == 1) temp = temp * e;
                if (Math.Abs(arg_lr[i, 1]) == 2) temp = temp * e * e;
                sr = sr + temp;
            }

            sl = 0;
            for (i = 0; i < 60; i++)
            {
                temp = sigma_l[i] * GPMath.sinDeg(arg_lr[i, 0] * d
                                       + arg_lr[i, 1] * m
                                       + arg_lr[i, 2] * ms
                                       + arg_lr[i, 3] * f);
                if (Math.Abs(arg_lr[i, 1]) == 1) temp = temp * e;
                if (Math.Abs(arg_lr[i, 1]) == 2) temp = temp * e * e;
                sl = sl + temp;
            }

            //(* correction terms  
            sl = sl + 3958 * GPMath.sinDeg(a1)
                + 1962 * GPMath.sinDeg(ls - f)
                + 318 * GPMath.sinDeg(a2);
            sb = 0;
            for (i = 0; i < 60; i++)
            {
                temp = sigma_b[i] * GPMath.sinDeg(arg_b[i, 0] * d
                                       + arg_b[i, 1] * m
                                       + arg_b[i, 2] * ms
                                       + arg_b[i, 3] * f);
                if (Math.Abs(arg_b[i, 1]) == 1) temp = temp * e;
                if (Math.Abs(arg_b[i, 1]) == 2) temp = temp * e * e;
                sb = sb + temp;
            }

            //(* correction terms  
            sb = sb - 2235 * GPMath.sinDeg(ls)
                  + 382 * GPMath.sinDeg(a3)
                  + 175 * GPMath.sinDeg(a1 - f)
                  + 175 * GPMath.sinDeg(a1 + f)
                  + 127 * GPMath.sinDeg(ls - ms)
                  - 115 * GPMath.sinDeg(ls + ms);

            lambda = ls + sl / 1000000;
            beta = sb / 1000000;
            delta = 385000.56 + sr / 1000;

            GPCelestialBodyCoordinates coord = new GPCelestialBodyCoordinates();
            coord.distanceFromEarth = delta;
            coord.eclipticalLongitude = GPMath.putIn360(lambda);
            coord.eclipticalLatitude = beta;
            coord.SetSiderealTime(jdate);
            //coord.SetJulianEphemerisDay(jdate);

            CalculateEquatorial(coord, jdate);
            //Debugger.Log(0, "", "Longitude: " + coord.longitude + "\n");
            return coord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="jdate"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude">+ eastwards, - westwards</param>
        /// <param name="height"></param>
        public static void correct_position(GPCelestialBodyCoordinates position, GPObserver obs)
        {
            position.makeTopocentric(obs);
/*            double u, h, delta_alpha;
            double rho_sin, rho_cos;
            const double b_a = 0.99664719;

            // geocentric position of observer on the earth surface
            u = GPMath.arctan_d(b_a * b_a * GPMath.tan_d(obs.GetLatitudeNorthPositive()));
            rho_sin = b_a * GPMath.sin_d(u) + obs.GetAltitude() / 6378140.0 * GPMath.sin_d(obs.GetLatitudeNorthPositive());
            rho_cos = GPMath.cos_d(u) + obs.GetAltitude() / 6378140.0 * GPMath.cos_d(obs.GetLatitudeNorthPositive());

            // equatorial horizontal paralax
            position.parallax = GPMath.arcsin_d(GPMath.sin_d(8.794 / 3600) / (position.distanceFromEarth / AU));

            // geocentric hour angle of the body
            h = position.GetHourAngle(obs);


            delta_alpha = GPMath.arctan_d(
                        (-rho_cos * GPMath.sin_d(position.parallax) * GPMath.sin_d(h)) /
                        (GPMath.cos_d(position.declination) - rho_cos * GPMath.sin_d(position.parallax) * GPMath.cos_d(h)));

            position.right_ascession += delta_alpha;
            position.declination = GPMath.arctan_d(
              ((GPMath.sin_d(position.declination) - rho_sin * GPMath.sin_d(position.parallax)) * GPMath.cos_d(delta_alpha)) /
              (GPMath.cos_d(position.declination) - rho_cos * GPMath.sin_d(position.parallax) * GPMath.cos_d(h)));*/
        }

        public double sun_distance(double jdate)
        {
            return sun_coordinate(jdate).distanceFromEarth / AU;
        }

        public double sun_diameter(double jd)
        {
            return sun_diameter(sun_coordinate(jd));
        }

        public double sun_diameter(GPCelestialBodyCoordinates coord)
        {
            return 959.63 / (coord.distanceFromEarth / AU) * 2;
        }

        public double moon_diameter(double jd)
        {
            return moon_diameter(moon_coordinate(jd));
        }

        public double moon_diameter(GPCelestialBodyCoordinates coord)
        {
            return 358473400 / coord.distanceFromEarth * 2;
        }

        public static void calc_phase_data(double date, TMoonPhase phase, out double jde, out double kk,
                out double m, out double ms, out double f, out double o, out double e)
        {
            int phases = 8;
            double t, ts;
            long k;

            k = Convert.ToInt64(Math.Round((date - J2000) / 36525.0 * 1236.85));
            ts = (date - J2000) / 36525.0;
            kk = Convert.ToInt32(k) + Convert.ToDouble(Convert.ToInt32(phase)) / phases;
            t = kk / 1236.85;
            jde = 2451550.09765 + 29.530588853 * kk
                 + t * t * (0.0001337 - t * (0.000000150 - 0.00000000073 * t));
            m = 2.5534 + 29.10535669 * kk - t * t * (0.0000218 + 0.00000011 * t);
            ms = 201.5643 + 385.81693528 * kk + t * t * (0.1017438 + t * (0.00001239 - t * 0.000000058));
            f = 160.7108 + 390.67050274 * kk - t * t * (0.0016341 + t * (0.00000227 - t * 0.000000011));
            o = 124.7746 - 1.56375580 * kk + t * t * (0.0020691 + t * 0.00000215);
            e = 1 - ts * (0.002516 + ts * 0.0000074);

        }

        public double age_of_moon(double jd)
        {
            GPCelestialBodyCoordinates sun_coord;
            GPCelestialBodyCoordinates moon_coord;

            sun_coord = sun_coordinate(jd);
            moon_coord = moon_coordinate(jd);

            return GPMath.putIn360(moon_coord.eclipticalLongitude - sun_coord.eclipticalLongitude) / 360 * mean_lunation;
        }

        public double nextphase_approx(double date, TMoonPhase phase)
        {
            double epsilon = 1e-7;
            int phases = 8;
            double target_age;
            double h;

            target_age = Convert.ToInt32(phase) * mean_lunation / phases;
            double result = date;
            do
            {
                h = age_of_moon(result) - target_age;
                if (h > mean_lunation / 2)
                {
                    h = h - mean_lunation;
                }
                result = result - h;
            } while (Math.Abs(h) >= epsilon);

            return result;
        }

        public double nextphase_49(double date, TMoonPhase phase)
        {
            double result;
            double t, kk, jde, m, ms, f, o, e, korr, w, akorr;
            double[] a = new double[15];

            calc_phase_data(date, phase, out jde, out kk, out m, out ms, out f, out o, out e);
            t = kk / 1236.85;

            switch (phase)
            {
                case TMoonPhase.Newmoon:
                    {
                        korr = -0.40720 * GPMath.sinDeg(ms)
                           + 0.17241 * e * GPMath.sinDeg(m)
                           + 0.01608 * GPMath.sinDeg(2 * ms)
                           + 0.01039 * GPMath.sinDeg(2 * f)
                           + 0.00739 * e * GPMath.sinDeg(ms - m)
                           - 0.00514 * e * GPMath.sinDeg(ms + m)
                           + 0.00208 * e * e * GPMath.sinDeg(2 * m)
                           - 0.00111 * GPMath.sinDeg(ms - 2 * f)
                           - 0.00057 * GPMath.sinDeg(ms + 2 * f)
                           + 0.00056 * e * GPMath.sinDeg(2 * ms + m)
                           - 0.00042 * GPMath.sinDeg(3 * ms)
                           + 0.00042 * e * GPMath.sinDeg(m + 2 * f)
                           + 0.00038 * e * GPMath.sinDeg(m - 2 * f)
                           - 0.00024 * e * GPMath.sinDeg(2 * ms - m)
                           - 0.00017 * GPMath.sinDeg(o)
                           - 0.00007 * GPMath.sinDeg(ms + 2 * m)
                           + 0.00004 * GPMath.sinDeg(2 * ms - 2 * f)
                           + 0.00004 * GPMath.sinDeg(3 * m)
                           + 0.00003 * GPMath.sinDeg(ms + m - 2 * f)
                           + 0.00003 * GPMath.sinDeg(2 * ms + 2 * f)
                           - 0.00003 * GPMath.sinDeg(ms + m + 2 * f)
                           + 0.00003 * GPMath.sinDeg(ms - m + 2 * f)
                           - 0.00002 * GPMath.sinDeg(ms - m - 2 * f)
                           - 0.00002 * GPMath.sinDeg(3 * ms + m)
                           + 0.00002 * GPMath.sinDeg(4 * ms);

                    }
                    break;
                case TMoonPhase.FirstQuarter:
                case TMoonPhase.LastQuarter:
                    {
                        korr = -0.62801 * GPMath.sinDeg(ms)
                               + 0.17172 * e * GPMath.sinDeg(m)
                               - 0.01183 * e * GPMath.sinDeg(ms + m)
                               + 0.00862 * GPMath.sinDeg(2 * ms)
                               + 0.00804 * GPMath.sinDeg(2 * f)
                               + 0.00454 * e * GPMath.sinDeg(ms - m)
                               + 0.00204 * e * e * GPMath.sinDeg(2 * m)
                               - 0.00180 * GPMath.sinDeg(ms - 2 * f)
                               - 0.00070 * GPMath.sinDeg(ms + 2 * f)
                               - 0.00040 * GPMath.sinDeg(3 * ms)
                               - 0.00034 * e * GPMath.sinDeg(2 * ms - m)
                               + 0.00032 * e * GPMath.sinDeg(m + 2 * f)
                               + 0.00032 * e * GPMath.sinDeg(m - 2 * f)
                               - 0.00028 * e * e * GPMath.sinDeg(ms + 2 * m)
                               + 0.00027 * e * GPMath.sinDeg(2 * ms + m)
                               - 0.00017 * GPMath.sinDeg(o)
                               - 0.00005 * GPMath.sinDeg(ms - m - 2 * f)
                               + 0.00004 * GPMath.sinDeg(2 * ms + 2 * f)
                               - 0.00004 * GPMath.sinDeg(ms + m + 2 * f)
                               + 0.00004 * GPMath.sinDeg(ms - 2 * m)
                               + 0.00003 * GPMath.sinDeg(ms + m - 2 * f)
                               + 0.00003 * GPMath.sinDeg(3 * m)
                               + 0.00002 * GPMath.sinDeg(2 * ms - 2 * f)
                               + 0.00002 * GPMath.sinDeg(ms - m + 2 * f)
                               - 0.00002 * GPMath.sinDeg(3 * ms + m);
                        w = 0.00306 - 0.00038 * e * GPMath.cosDeg(m)
                                  + 0.00026 * GPMath.cosDeg(ms)
                                  - 0.00002 * GPMath.cosDeg(ms - m)
                                  + 0.00002 * GPMath.cosDeg(ms + m)
                                  + 0.00002 * GPMath.cosDeg(2 * f);
                        if (phase == TMoonPhase.FirstQuarter)
                        {
                            korr = korr + w;

                        }
                        else
                        {
                            korr = korr - w;
                        }

                    }
                    break;
                case TMoonPhase.Fullmoon:
                    {
                        korr = -0.40614 * GPMath.sinDeg(ms)
                           + 0.17302 * e * GPMath.sinDeg(m)
                           + 0.01614 * GPMath.sinDeg(2 * ms)
                           + 0.01043 * GPMath.sinDeg(2 * f)
                           + 0.00734 * e * GPMath.sinDeg(ms - m)
                           - 0.00515 * e * GPMath.sinDeg(ms + m)
                           + 0.00209 * e * e * GPMath.sinDeg(2 * m)
                           - 0.00111 * GPMath.sinDeg(ms - 2 * f)
                           - 0.00057 * GPMath.sinDeg(ms + 2 * f)
                           + 0.00056 * e * GPMath.sinDeg(2 * ms + m)
                           - 0.00042 * GPMath.sinDeg(3 * ms)
                           + 0.00042 * e * GPMath.sinDeg(m + 2 * f)
                           + 0.00038 * e * GPMath.sinDeg(m - 2 * f)
                           - 0.00024 * e * GPMath.sinDeg(2 * ms - m)
                           - 0.00017 * GPMath.sinDeg(o)
                           - 0.00007 * GPMath.sinDeg(ms + 2 * m)
                           + 0.00004 * GPMath.sinDeg(2 * ms - 2 * f)
                           + 0.00004 * GPMath.sinDeg(3 * m)
                           + 0.00003 * GPMath.sinDeg(ms + m - 2 * f)
                           + 0.00003 * GPMath.sinDeg(2 * ms + 2 * f)
                           - 0.00003 * GPMath.sinDeg(ms + m + 2 * f)
                           + 0.00003 * GPMath.sinDeg(ms - m + 2 * f)
                           - 0.00002 * GPMath.sinDeg(ms - m - 2 * f)
                           - 0.00002 * GPMath.sinDeg(3 * ms + m)
                           + 0.00002 * GPMath.sinDeg(4 * ms);

                    }
                    break;
                default:
                    korr = 0;
                    break;
            }


            // Additional Corrections due to planets
            a[1] = 299.77 + 0.107408 * kk - 0.009173 * t * t;
            a[2] = 251.88 + 0.016321 * kk;
            a[3] = 251.83 + 26.651886 * kk;
            a[4] = 349.42 + 36.412478 * kk;
            a[5] = 84.66 + 18.206239 * kk;
            a[6] = 141.74 + 53.303771 * kk;
            a[7] = 207.14 + 2.453732 * kk;
            a[8] = 154.84 + 7.306860 * kk;
            a[9] = 34.52 + 27.261239 * kk;
            a[10] = 207.19 + 0.121824 * kk;
            a[11] = 291.34 + 1.844379 * kk;
            a[12] = 161.72 + 24.198154 * kk;
            a[13] = 239.56 + 25.513099 * kk;
            a[14] = 331.55 + 3.592518 * kk;
            akorr = +0.000325 * GPMath.sinDeg(a[1])
                      + 0.000165 * GPMath.sinDeg(a[2])
                      + 0.000164 * GPMath.sinDeg(a[3])
                      + 0.000126 * GPMath.sinDeg(a[4])
                      + 0.000110 * GPMath.sinDeg(a[5])
                      + 0.000062 * GPMath.sinDeg(a[6])
                      + 0.000060 * GPMath.sinDeg(a[7])
                      + 0.000056 * GPMath.sinDeg(a[8])
                      + 0.000047 * GPMath.sinDeg(a[9])
                      + 0.000042 * GPMath.sinDeg(a[10])
                      + 0.000040 * GPMath.sinDeg(a[11])
                      + 0.000037 * GPMath.sinDeg(a[12])
                      + 0.000035 * GPMath.sinDeg(a[13])
                      + 0.000023 * GPMath.sinDeg(a[14]);
            korr = korr + akorr;
            //
            result = jde + korr;

            return result;
        }

        public double nextphase(double date, TMoonPhase phase)
        {
            switch (phase)
            {
                case TMoonPhase.Newmoon:
                case TMoonPhase.FirstQuarter:
                case TMoonPhase.Fullmoon:
                case TMoonPhase.LastQuarter:
                    return nextphase_49(date, phase);
                case TMoonPhase.WaningCrescent:
                case TMoonPhase.WaningGibbous:
                case TMoonPhase.WaxingCrescrent:
                case TMoonPhase.WaxingGibbous:
                    return nextphase_approx(date, phase);
                default:
                    return 0;
            }
        }

        public double last_phase(double date, TMoonPhase phase)
        {
            double temp_date = date + 28;
            double result = temp_date;
            while (result > date)
            {
                result = nextphase(temp_date, phase);
                if (result < 0.1)
                    throw new Exception("No date time possible");
                temp_date = temp_date - 28;
            }

            return result;
        }

        public double next_phase(double date, TMoonPhase phase)
        {
            double temp_date = date - 28;
            double result = temp_date;
            while (result < date)
            {
                result = nextphase(temp_date, phase);
                if (result < 0.1)
                    throw new Exception("No date time possible");
                temp_date += 28;
            }
            return result;
        }

        public int lunation(double date)
        {
            return Convert.ToInt32(Math.Round((last_phase(date, TMoonPhase.Newmoon) - JFirstLunation) / mean_lunation) + 1);
        }


        private static short[,] arg_apo = new short[,] {
                 { 2, 0, 0},
     { 4, 0, 0},
     { 0, 0, 1},
     { 2, 0,-1},
     { 0, 2, 0},
     { 1, 0, 0},
     { 6, 0, 0},
     { 4, 0,-1},
     { 2, 2, 0},
     { 1, 0, 1},
     { 8, 0, 0},
     { 6, 0,-1},
     { 2,-2, 0},
     { 2, 0,-2},
     { 3, 0, 0},
     { 4, 2, 0},
     { 8, 0,-1},
     { 4, 0,-2},
     {10, 0, 0},
     { 3, 0, 1},
     { 0, 0, 2},
     { 2, 0, 1},
     { 2, 0, 2},
     { 6, 2, 0},
     { 6, 0,-2},
     {10, 0,-1},
     { 5, 0, 0},
     { 4,-2, 0},
     { 0, 2, 1},
     {12, 0, 0},
     { 2, 2,-1},
     { 1, 0,-1}

        };
        private static short[,] arg_per = new short[,] {
                 { 2, 0, 0},
     { 4, 0, 0},
     { 6, 0, 0},
     { 8, 0, 0},
     { 2, 0,-1},
     { 0, 0, 1},
     {10, 0, 0},
     { 4, 0,-1},
     { 6, 0,-1},
     {12, 0, 0},
     { 1, 0, 0},
     { 8, 0,-1},
     {14, 0, 0},
     { 0, 2, 0},
     { 3, 0, 0},
     {10, 0,-1},
     {16, 0, 0},
     {12, 0,-1},
     { 5, 0, 0},
     { 2, 2, 0},
     {18, 0, 0},
     {14, 0,-1},
     { 7, 0, 0},
     { 2, 1, 0},
     {20, 0, 0},
     { 1, 0, 1},
     {16, 0,-1},
     { 4, 0, 1},
     { 2, 0,-2},
     { 4, 0,-2},
     { 6, 0,-2},
     {22, 0, 0},
     {18, 0,-1},
     { 6, 0, 1},
     {11, 0, 0},
     { 8, 0, 1},
     { 4,-2, 0},
     { 6, 2, 0},
     { 3, 0, 1},
     { 5, 0, 1},
     {13, 0, 0},
     {20, 0,-1},
     { 3, 0, 2},
     { 4, 2,-2},
     { 1, 0, 2},
     {22, 0,-1},
     { 0, 4, 0},
     { 6,-2, 0},
     { 2,-2, 1},
     { 0, 0, 2},
     { 0, 2,-1},
     { 2, 4, 0},
     { 0, 2,-2},
     { 2,-2, 2},
     {24, 0, 0},
     { 4,-4, 0},
     { 9, 0, 0},
     { 4, 2, 0},
     { 2, 0, 2},
     { 1, 0,-1}

};

        private static short[,] koe_apo = new short[,] {
                 { 4392,  0},
     {  684,  0},
     {  456,-11},
     {  426,-11},
     {  212,  0},
     { -189,  0},
     {  144,  0},
     {  113,  0},
     {   47,  0},
     {   36,  0},
     {   35,  0},
     {   34,  0},
     {  -34,  0},
     {   22,  0},
     {  -17,  0},
     {   13,  0},
     {   11,  0},
     {   10,  0},
     {    9,  0},
     {    7,  0},
     {    6,  0},
     {    5,  0},
     {    5,  0},
     {    4,  0},
     {    4,  0},
     {    4,  0},
     {   -4,  0},
     {   -4,  0},
     {    3,  0},
     {    3,  0},
     {    3,  0},
     {   -3,  0}

        };

        private static short[,] koe_per = new short[,] {
             {-16769,  0},
             {  4589,  0},
             { -1856,  0},
             {   883,  0},
             {  -773, 19},
             {   502,-13},
             {  -460,  0},
             {   422,-11},
             {  -256,  0},
             {   253,  0},
             {   237,  0},
             {   162,  0},
             {  -145,  0},
             {   129,  0},
             {  -112,  0},
             {  -104,  0},
             {    86,  0},
             {    69,  0},
             {    66,  0},
             {   -53,  0},
             {   -52,  0},
             {   -46,  0},
             {   -41,  0},
             {    40,  0},
             {    32,  0},
             {   -32,  0},
             {    31,  0},
             {   -29,  0},
             {   -27,  0},
             {    24,  0},
             {   -21,  0},
             {   -21,  0},
             {   -21,  0},
             {    19,  0},
             {   -18,  0},
             {   -14,  0},
             {   -14,  0},
             {   -14,  0},
             {    14,  0},
             {   -14,  0},
             {    13,  0},
             {    13,  0},
             {    11,  0},
             {   -11,  0},
             {   -10,  0},
             {    -9,  0},
             {    -8,  0},
             {     8,  0},
             {     8,  0},
             {     7,  0},
             {     7,  0},
             {     7,  0},
             {    -6,  0},
             {    -6,  0},
             {     6,  0},
             {     5,  0},
             {    27,  0},
             {    27,  0},
             {     5,  0},
             {    -4,  0}
        };
        private double nextXXXgee(double date, bool apo)
        {
            double k, jde, t;
            double d, m, f, v;
            int i;

            k = Math.Round(((date - J1999) / 365.25 - 0.97) * 13.2555);
            if (apo) k = k + 0.5;
            t = k / 1325.55;
            jde = 2451534.6698 + 27.55454988 * k + (-0.0006886 +
                 (-0.000001098 + 0.0000000052 * t) * t) * t * t;
            d = 171.9179 + 335.9106046 * k + (-0.0100250 + (-0.00001156 + 0.000000055 * t) * t) * t * t;
            m = 347.3477 + 27.1577721 * k + (-0.0008323 - 0.0000010 * t) * t * t;
            f = 316.6109 + 364.5287911 * k + (-0.0125131 - 0.0000148 * t) * t * t;
            v = 0;
            if (apo)
            {
                for (i = 0; i <= 31; i++)
                {
                    v = v + GPMath.sinDeg(arg_apo[i, 0] * d + arg_apo[i, 1] * f + arg_apo[i, 2] * m) *
                       (koe_apo[i, 0] * 0.0001 + koe_apo[i, 1] * 0.00001 * t);
                }
            }
            else
            {
                for (i = 0; i < 60; i++)
                {
                    v = v + GPMath.sinDeg(arg_per[i, 0] * d + arg_per[i, 1] * f + arg_per[i, 2] * m) *
                       (koe_per[i, 0] * 0.0001 + koe_per[i, 1] * 0.00001 * t);
                }
            }
            return jde + v;
        }

        public double nextperigee(double date)
        {
            double temp_date = date - 28;

            double result = temp_date;

            while (result < date)
            {
                result = nextXXXgee(temp_date, false);
                temp_date += 28;
            }

            return result;
        }
        public double nextapogee(double date)
        {
            double temp_date = date - 28;

            double result = temp_date;

            while (result < date)
            {
                result = nextXXXgee(temp_date, true);
                temp_date += 28;
            }

            return result;
        }

        private double nextXXXhel(double date, bool apo)
        {
            double k, jde;

            k = Math.Round(((date - J2000) / 365.25 - 0.01) * 0.99997);
            if (apo) k = k + 0.5;
            jde = 2451547.507 + (365.2596358 + 0.0000000158 * k) * k;
            return jde;
        }

        public double nextperihel(double date)
        {
            double temp_date = date - 365.25;
            double result = temp_date;

            while (result < date)
            {
                result = nextXXXhel(temp_date, false);
                temp_date += 365.25;
            }

            return result;
        }
        public double nextaphel(double date)
        {
            double temp_date = date - 365.25;
            double result = temp_date;

            while (result < date)
            {
                result = nextXXXhel(temp_date, true);
                temp_date += 365.25;
            }

            return result;
        }


        public double StartSeason(int year, TSeason season)
        {
            double y;
            double jde0;
            double t, w, dl, s;
            int i;
            /* /// a: array[0..23] of int = (..);  */
            int[] a = new int[] {
                485, 203, 199, 182, 156, 136, 77, 74, 70, 58, 52, 50,
                45, 44, 29, 18, 17, 16, 14, 12, 12, 12, 9, 8 };
            /* \\\ */
            /* /// bc:array[0..23,1..2] of double = (..);  */
            double[,] bc = new double[,] {
                 { 324.96,   1934.136 },
                 { 337.23,  32964.467 },
                 { 342.08,     20.186 },
                 {  27.85, 445267.112 },
                 {  73.14,  45036.886 },
                 { 171.52,  22518.443 },
                 { 222.54,  65928.934 },
                 { 296.72,   3034.906 },
                 { 243.58,   9037.513 },
                 { 119.81,  33718.147 },
                 { 297.17,    150.678 },
                 {  21.02,   2281.226 },
                 { 247.54,  29929.562 },
                 { 325.15,  31555.956 },
                 {  60.93,   4443.417 },
                 { 155.12,  67555.328 },
                 { 288.79,   4562.452 },
                 { 198.04,  62894.029 },
                 { 199.76,  31436.921 },
                 {  95.39,  14577.848 },
                 { 287.11,  31931.756 },
                 { 320.81,  34777.259 },
                 { 227.73,   1222.114 },
                 {  15.45,  16859.074 }
              };
            /* \\\ */

            if (year >= -1000 && year <= 999)
            {
                y = year / 1000;
                switch (season)
                {
                    case TSeason.Spring:
                        jde0 = 1721139.29189 + (365242.13740 + (0.06134 + (0.00111 - 0.00071 * y) * y) * y) * y;
                        break;
                    case TSeason.Summer:
                        jde0 = 1721233.25401 + (365241.72562 + (-0.05323 + (0.00907 + 0.00025 * y) * y) * y) * y;
                        break;
                    case TSeason.Autumn:
                        jde0 = 1721325.70455 + (365242.49558 + (-0.11677 + (-0.00297 + 0.00074 * y) * y) * y) * y;
                        break;
                    case TSeason.Winter:
                        jde0 = 1721414.39987 + (365242.88257 + (-0.00769 + (-0.00933 - 0.00006 * y) * y) * y) * y;
                        break;
                    default:
                        jde0 = 0;   /*  this can't happen  */
                        break;
                }
            }
            /* \\\ */
            /* /// +1000..+3000:  */
            else if (year >= 1000 && year <= 3000)
            {
                y = (year - 2000) / 1000;
                switch (season)
                {
                    case TSeason.Spring:
                        jde0 = 2451623.80984 + (365242.37404 + (0.05169 + (-0.00411 - 0.00057 * y) * y) * y) * y;
                        break;
                    case TSeason.Summer:
                        jde0 = 2451716.56767 + (365241.62603 + (0.00325 + (0.00888 - 0.00030 * y) * y) * y) * y;
                        break;
                    case TSeason.Autumn:
                        jde0 = 2451810.21715 + (365242.01767 + (-0.11575 + (0.00337 + 0.00078 * y) * y) * y) * y;
                        break;
                    case TSeason.Winter:
                        jde0 = 2451900.05952 + (365242.74049 + (-0.06223 + (-0.00823 + 0.00032 * y) * y) * y) * y;
                        break;
                    default:
                        jde0 = 0;   /*  this can't happen  */
                        break;
                }
            }
            /* \\\ */
            else
            {
                throw new Exception("Out of range of the algorithm");
            };
            t = (jde0 - J2000) / 36525;
            w = 35999.373 * t - 2.47;
            dl = 1 + 0.0334 * GPMath.cosDeg(w) + 0.0007 * GPMath.cosDeg(2 * w);
            /* /// s  =  ä a cos(b+c*t)  */
            s = 0;
            for (i = 0; i <= 23; i++)
                s = s + a[i] * GPMath.cosDeg(bc[i, 0] + bc[i, 1] * t);
            /* \\\ */
            return jde0 + (0.00001 * s) / dl;
        }

        /// <summary>
        /// Parabolic interpolation of 3 values.
        /// </summary>
        /// <param name="y1">Value of Y for X = -1</param>
        /// <param name="y2">Value of Y for X = 0</param>
        /// <param name="y3">Value of Y for X = +1</param>
        /// <param name="n">Input value of X</param>
        /// <returns>Interpolated value of Y for given X</returns>
        private static double interpolation(double y1, double y2, double y3, double n)
        {
            double a, b, c;

            a = y2 - y1;
            b = y3 - y2;
            c = b - a;
            return y2 + 0.5 * n * (a + b + n * c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        /// <param name="theta0"></param>
        /// <param name="h0"></param>
        /// <param name="L">Westwards +, Eastwards -</param>
        /// <param name="phi">latitude</param>
        /// <param name="m"></param>
        /// <param name="transit"></param>
        /// <param name="jd"></param>
        /// <param name="deltaT">DeltaT</param>
        /// <returns></returns>
        private static double correction(GPCelestialBodyCoordinates pos1, GPCelestialBodyCoordinates pos2, GPCelestialBodyCoordinates pos3, 
            double theta0, double h0, GPObserver obs, double m, bool transit, double jd, double deltaT)
        {
            double alpha, delta, H, height;
            double phi = obs.GetLatitudeNorthPositive();
            double L = obs.GetLongitudeWestPositive();
            double theta = theta0 + 360.985647 * m;
            double dT = deltaT;
            //3.3
            alpha = interpolation(pos1.right_ascession, pos2.right_ascession, pos3.right_ascession,  m + dT/86400);

            // local hour angle
            H = GPMath.putIn180(theta - L - alpha);

            if (transit)
            {
                return -H / 360;
            }
            else
            {
                delta = interpolation(pos1.declination, pos2.declination, pos3.declination, m + dT / 86400);
                //12.6
                height = GPMath.arcsinDeg(GPMath.sinDeg(phi) * GPMath.sinDeg(delta)
                                 + GPMath.cosDeg(phi) * GPMath.cosDeg(delta) * GPMath.cosDeg(H));
                return (height - h0) / (360 * GPMath.cosDeg(delta) * GPMath.cosDeg(phi) * GPMath.sinDeg(H));
            }

        }

        /// <summary>
        /// Calculates time of sunrise, transit and sunset for given date and timezone.
        /// Times are calculated for given latitude and longitude.
        /// Timezone helps to determine julian date for noon at given earth position.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="timezone"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude">Longitude, + eastwards, - westwards</param>
        /// <param name="timeRise"></param>
        /// <param name="timeTransit"></param>
        /// <param name="timeSet"></param>
        /// <returns>Returns true if times were calculated. returns false if sunrise or sunset did not happen on given day, longitude
        /// and latitude</returns>
        public static bool CalculateTimeSun(GPGregorianTime date, 
            GPObserver obs, 
            out GPJulianTime timeRise, out GPJulianTime timeTransit, out GPJulianTime timeSet)
        {
            double h;
            GPCelestialBodyCoordinates pos1, pos2, pos3;
            GPJulianTime t1, t2, t3, ttheta;
            double h0, theta0, cos_H0, H0;
            double m0, m1, m2;

            // longitude positive westwards, negative eastwards
            double L = obs.GetLongitudeWestPositive();
            double phi = obs.GetLatitudeNorthPositive();
            double deltaT;

            h0 = -0.8333;
            // this is dynamic time of midnight at greenwich
            // UT = TD - deltaT
            h = date.getJulianGreenwichNoon() - 0.5;
            ttheta = new GPJulianTime();
            // set as universal time of midnight at greenwich
            ttheta.setLocalJulianDay(h);
            t1 = new GPJulianTime();
            t1.setLocalJulianEphemerisDay(h - 1);
            t2 = new GPJulianTime();
            t2.setLocalJulianEphemerisDay(h);
            t3 = new GPJulianTime();
            t3.setLocalJulianEphemerisDay(h + 1);
            deltaT = GPDynamicTime.GetDeltaT(h);
            theta0 = GetSiderealTime(ttheta.getGreenwichJulianEphemerisDay());

            pos1 = sun_coordinate(h - 1);
            pos2 = sun_coordinate(h);
            pos3 = sun_coordinate(h + 1);

            double T = (h - J2000) / 365250;
            double T2 = T * T;
            double T4 = T2 * T2;
            double equationOfTime = 0;/*280.4664567 + 360007.6982779 * T + 0.03032028 * T2 + T2 * T / 49931 - T2 * T2 / 15299 - T4 * T / 1988000
                - 0.0057183 - pos2.right_ascession + pos2.delta_phi * GPMath.cosDeg(pos2.epsilon);
            equationOfTime = GPMath.putIn180(equationOfTime)/360;*/

            cos_H0 = (GPMath.sinDeg(h0) - GPMath.sinDeg(phi) * GPMath.sinDeg(pos2.declination)) /
                    (GPMath.cosDeg(phi) * GPMath.cosDeg(pos2.declination));
            if ((cos_H0 < -1) || (cos_H0 > 1))
            {
                timeTransit = timeRise = timeSet = null;
                return false;
            }
            H0 = GPMath.arccosDeg(cos_H0);

            m0 = (pos2.right_ascession + L - theta0) / 360;
            m1 = m0 - H0 / 360;
            m2 = m0 + H0 / 360;

            double diffM;

            for (int k = 0; k < 2; k++)
            {
                m0 = GPMath.putIn1(m0);
                m1 = GPMath.putIn1(m1);
                m2 = GPMath.putIn1(m2);

                diffM = correction(pos1, pos2, pos3, theta0, h0, obs, m0, true, h, deltaT);
                m0 += diffM;
                diffM = correction(pos1, pos2, pos3, theta0, h0, obs, m1, false, h, deltaT);
                m1 += diffM;
                diffM = correction(pos1, pos2, pos3, theta0, h0, obs, m2, false, h, deltaT);
                m2 += diffM;
            }


            timeTransit = new GPJulianTime();
            timeTransit.setLocalTimezoneOffset(0);
            timeTransit.setLocalJulianEphemerisDay(h + m0 - equationOfTime);
            timeRise = new GPJulianTime();
            timeRise.setLocalTimezoneOffset(0);
            timeRise.setLocalJulianEphemerisDay(h + m1 - equationOfTime);
            timeSet = new GPJulianTime();
            timeSet.setLocalTimezoneOffset(0);
            timeSet.setLocalJulianEphemerisDay(h + m2 - equationOfTime);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="obs">Observer's coordinates on Earth's surface</param>
        /// <returns></returns>
        public static GPCelestialBodyCoordinates GetMoonTopocentric(GPJulianTime date, GPObserver obs)
        {
            //double DT = UniversalTimeToDynamicTime(date);
            GPCelestialBodyCoordinates coord = moon_coordinate(date.getGreenwichJulianEphemerisDay());
            coord.SetJulianDay(date.getGreenwichJulianDay());

            correct_position(coord, obs);

            calcHorizontal(coord, obs);

            return coord;
        }

        /// <summary>
        /// Search for next rise or set of the moon.
        /// </summary>
        /// <param name="startDate">Starting date of search</param>
        /// <param name="obs">Position of observer on Earth</param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static GPJulianTime GetNextMoonEvent(GPJulianTime startDate, GPLocationProvider obs, out TRiseSet kind)
        {
            kind = TRiseSet.RISE;
            double [] array = new double[2];
            GPJulianTime[] times = new GPJulianTime[2];
            GPJulianTime result = null;
            GPCelestialBodyCoordinates c;
            double time = 0;
            int i = 0;

            for (i = 0; i < 2; i++)
            {
                times[i] = new GPJulianTime();
                times[i].setLocalJulianDay(startDate.getLocalJulianDay() + i/24.0);
                c = GetMoonTopocentric(times[i], obs.getLocation(times[i].getGreenwichJulianDay()));
                array[i] = c.elevation;
                //Debugger.Log(0, "", String.Format("elev[{0}] = {1}\n", times[i], array[i]));

            }

            while ((kind = GetNextMoonEvent_DecideTime(array, times, obs, ref result)) == TRiseSet.NONE && i < 240)
            {
                times[0] = times[1];
                times[1] = new GPJulianTime();
                times[1].setLocalJulianDay(startDate.getLocalJulianDay() + i / 24.0);
                i++;
                array[0] = array[1];
                array[1] = GetMoonTopocentric(times[1], obs.getLocation(times[1].getGreenwichJulianDay())).elevation;
                //Debugger.Log(0, "", String.Format("elev[{0}] = {1}\n", times[2], array[2]));
            }

            // if we have gone through 10 days without finding time of transition
            // we return -1
            // otherwise time variable contains relative offset in hours for time[1]
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Returns true if rise/set time was found, 
        /// false if we need to move in time 1 hour forward</returns>
        private static TRiseSet GetNextMoonEvent_DecideTime(double[] array, GPJulianTime[] times, GPLocationProvider obs, ref GPJulianTime time)
        {
            double elevationLimit = -0.5;
            GPJulianTime nta, ntb, ntc;
            double ela, elb, elc;
            ela = array[0];
            elb = array[1];
            nta = times[0];
            ntb = times[1];
            ntc = null;
            if (ela < elevationLimit && elb > elevationLimit)
            {
                for (int i = 0; i < 10; i++)
                {
                    ntc = new GPJulianTime();
                    ntc.setLocalJulianDay((nta.getLocalJulianDay() + ntb.getLocalJulianDay()) / 2);
                    elc = GetMoonTopocentric(ntc, obs.getLocation(ntc.getGreenwichJulianDay())).elevation;
                    if (elc < elevationLimit)
                    {
                        ela = elc;
                        nta = ntc;
                    }
                    else
                    {
                        elb = elc;
                        ntb = ntc;
                    }
                }
                time = ntc;
                return TRiseSet.RISE;
            }
            else if (ela > elevationLimit && elb < elevationLimit)
            {
                for (int i = 0; i < 10; i++)
                {
                    ntc = new GPJulianTime();
                    ntc.setLocalJulianDay((nta.getLocalJulianDay() + ntb.getLocalJulianDay()) / 2);
                    elc = GetMoonTopocentric(ntc, obs.getLocation(ntc.getGreenwichJulianDay())).elevation;
                    if (elc > elevationLimit)
                    {
                        ela = elc;
                        nta = ntc;
                    }
                    else
                    {
                        elb = elc;
                        ntb = ntc;
                    }
                }
                time = ntc;
                return TRiseSet.SET;
            }


            return TRiseSet.NONE;
        }


        public static TEclipse Eclipse(ref double date, bool sun)
        {
            double jde, kk, m, ms, f, o, e;
            double t, f1, a1;
            double p, q, w, gamma, u;
            TEclipse result;

            if (sun)
                calc_phase_data(date, TMoonPhase.Newmoon, out jde, out kk, out m, out ms, out f, out o, out e);
            else
                calc_phase_data(date, TMoonPhase.Fullmoon, out jde, out kk, out m, out ms, out f, out o, out e);
            t = kk / 1236.85;
            if (Math.Abs(GPMath.sinDeg(f)) > 0.36)
                result = TEclipse.none;
            /* /// else  */
            else
            {
                f1 = f - 0.02665 * GPMath.sinDeg(o);
                a1 = 299.77 + 0.107408 * kk - 0.009173 * t * t;
                if (sun)
                    jde = jde - 0.4075 * GPMath.sinDeg(ms) + 0.1721 * e * GPMath.sinDeg(m);
                else
                    jde = jde - 0.4065 * GPMath.sinDeg(ms) + 0.1727 * e * GPMath.sinDeg(m);
                jde = jde + 0.0161 * GPMath.sinDeg(2 * ms)
                           - 0.0097 * GPMath.sinDeg(2 * f1)
                           + 0.0073 * e * GPMath.sinDeg(ms - m)
                           - 0.0050 * e * GPMath.sinDeg(ms + m)
                           - 0.0023 * GPMath.sinDeg(ms - 2 * f1)
                           + 0.0021 * e * GPMath.sinDeg(2 * m)
                           + 0.0012 * GPMath.sinDeg(ms + 2 * f1)
                           + 0.0006 * e * GPMath.sinDeg(2 * ms + m)
                           - 0.0004 * GPMath.sinDeg(3 * ms)
                           - 0.0003 * e * GPMath.sinDeg(m + 2 * f1)
                           + 0.0003 * GPMath.sinDeg(a1)
                           - 0.0002 * e * GPMath.sinDeg(m - 2 * f1)
                           - 0.0002 * e * GPMath.sinDeg(2 * ms - m)
                           - 0.0002 * GPMath.sinDeg(o);
                p = +0.2070 * e * GPMath.sinDeg(m)
                           + 0.0024 * e * GPMath.sinDeg(2 * m)
                           - 0.0392 * GPMath.sinDeg(ms)
                           + 0.0116 * GPMath.sinDeg(2 * ms)
                           - 0.0073 * e * GPMath.sinDeg(ms + m)
                           + 0.0067 * e * GPMath.sinDeg(ms - m)
                           + 0.0118 * GPMath.sinDeg(2 * f1);
                q = +5.2207
                           - 0.0048 * e * GPMath.cosDeg(m)
                           + 0.0020 * e * GPMath.cosDeg(2 * m)
                           - 0.3299 * GPMath.cosDeg(ms)
                           - 0.0060 * e * GPMath.cosDeg(ms + m)
                           + 0.0041 * e * GPMath.cosDeg(ms - m);
                w = Math.Abs(GPMath.cosDeg(f1));
                gamma = (p * GPMath.cosDeg(f1) + q * GPMath.sinDeg(f1)) * (1 - 0.0048 * w);
                u = +0.0059
                    + 0.0046 * e * GPMath.cosDeg(m)
                    - 0.0182 * GPMath.cosDeg(ms)
                    + 0.0004 * GPMath.cosDeg(2 * ms)
                    - 0.0005 * GPMath.cosDeg(m + ms);
                /* /// if sun then  */
                if (sun)
                {
                    if (Math.Abs(gamma) < 0.9972)
                    {
                        if (u < 0)
                            result = TEclipse.total;
                        else if (u > 0.0047)
                            result = TEclipse.circular;
                        else if (u < 0.00464 * Math.Sqrt(1 - gamma * gamma))
                            result = TEclipse.circulartotal;
                        else
                            result = TEclipse.circular;
                    }
                    else if (Math.Abs(gamma) > 1.5433 + u)
                        result = TEclipse.none;
                    else if (Math.Abs(gamma) < 0.9972 + Math.Abs(u))
                        result = TEclipse.noncentral;
                    else
                        result = TEclipse.partial;
                }

                else
                {
                    if ((1.0128 - u - Math.Abs(gamma)) / 0.5450 > 0)
                        result = TEclipse.total;
                    else if ((1.5573 + u - Math.Abs(gamma)) / 0.5450 > 0)
                        result = TEclipse.halfshadow;
                    else
                        result = TEclipse.none;
                };
            };
            date = jde;

            return result;
        }

        public static TEclipse FindNextEclipse(ref double date, bool sun)
        {
            double temp_date;

            TEclipse result = TEclipse.none;    /*  just to make Delphi 2/3 shut up, not needed really  */
            temp_date = date - 28 * 2;
            while (temp_date < date)
            {
                temp_date += 28;
                result = Eclipse(ref temp_date, sun);
            }
            date = temp_date;

            return result;
        }

        /// <summary>
        /// Calculates times relevant for sun eclipse.
        /// returned array of double values has this meaning:
        /// times[0] - start of partial eclipse or -1 if no eclipse
        /// times[1] - start of total eclipse or -1 if no total eclipse
        /// times[2] - center of eclipse or -1 if no eclipse
        /// times[3] - end of total eclipse or -1 if no total eclipse
        /// times[4] - end of partial eclipse or -1 if no eclipse
        /// 
        /// here are possible scenarios of eclipse
        /// 
        /// times[2] == -1 : no eclipse
        /// 
        /// times[2] != -1, times[1] == -1 : only partial eclipse
        /// 
        /// times[2] != -1, times[1] != -1 :  partial and total eclipse
        /// </summary>
        /// <param name="startDate">This is date returned by NextEclipse() function. 
        /// It is approximate date of center of the eclipse.</param>
        /// <param name="obs">Observer of eclipse</param>
        /// <param name="times">Output value. Times are in Universal Time</param>
        public static void CalculateTimesSunEclipse(double startDate, GPObserver obs, out double[] times)
        {
            double dStart = startDate - 0.5;
            double dEnd = startDate + 0.5;
            double jd = 0;
            double deltaT = GPDynamicTime.GetDeltaT(startDate);
            double sunMoonPartialDist;
            double sunMoonFullDist;
            double m1, m2;
            times = new double[5];
            double oneMin = 1 / 1440.0;

            GPFunctorSunMoon functor = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.AzimuthElevationDistance, obs);
            GPSearch sr = new GPSearch(functor);
            sr.setSilentErrorValue(true, -1);

            for (int i = 0; i < 5; i++)
            {
                times[i] = -1;
            }
            times[2] = sr.findMinimum(startDate - 0.5, startDate + 0.5);
            if (sr.wasError())
            {
                times[2] = -1;
                return;
            }
            m1 = functor.getMoonCoordinates().getVisibleAngle(MOON_RADIUS);
            m2 = functor.getSunCoordinates().getVisibleAngle(SUN_RADIUS);
            sunMoonFullDist = m1 - m2;
            sunMoonPartialDist = m1 + m2;

            if (times[2] > sunMoonPartialDist)
                times[2] = -1;

            if (times[2] > 0)
            {
                times[0] = sr.findSet(startDate - 0.5, times[2], sunMoonPartialDist);
                if (times[0] > 0)
                    times[1] = sr.findSet(times[0], times[2], sunMoonFullDist);
                if (Math.Abs(times[1] - times[2]) < oneMin)
                    times[1] = -1;
                times[4] = sr.findRise(times[2], startDate + 0.5, sunMoonPartialDist);
                if (times[4] > 0)
                    times[3] = sr.findRise(times[2], times[4], sunMoonFullDist);
                if (Math.Abs(times[3] - times[4]) < oneMin)
                    times[3] = -1;
            }

            for (int i = 0; i < 5; i++)
            {
                if (times[i] > 0)
                    times[i] -= deltaT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="obs"></param>
        /// <param name="times">Array of double values (Julian day - universal time)
        /// Meaning of items in array:
        /// index      meaning
        /// -----------------------------------
        /// 0        start of partial penumbral eclipse
        /// 1        start of total penumbral eclipse
        /// 2        start of partial full eclipse
        /// 3        start of total eclipse
        /// 4        center of eclipse (not necessary total)
        /// 5        end of total eclipse
        /// 6        end of partial full eclipse
        /// 7        end of total penumbral eclipse
        /// 8        end of partial penumbral eclipse
        /// 
        /// Every value: if value is less than 0, then
        /// time of this has not occured
        /// 
        /// if times[4] is less than  0, then no eclipse occured in given date
        /// </param>
        public static void CalculateTimesMoonEclipse(double startDate, GPObserver obs, out double[] times)
        {
            double dStart = startDate - 0.5;
            double dEnd = startDate + 0.5;
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double fullShadowOuter;
            double fullShadowInner;
            double penumbralShadowInner;
            double penumbralShadowOuter;
            double m1, m2, m3, fullShadowRadius;
            double penumShadowRadius;
            times = new double[9];

            GPFunctorSunMoon functor = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.RigthAscessionDeclinationOpositeDistance, obs);
            GPSearch sr = new GPSearch(functor);
            sr.setSilentErrorValue(true, -1);

            // nullify
            for (int i = 0; i < 9; i++)
            {
                times[i] = -1;
            }

            // get time of possible eclipse
            times[4] = sr.findMinimum(startDate - 0.5, startDate + 0.5);
            if (sr.wasError())
            {
                times[4] = -1;
                return;
            }
            double dSun, dMoon;
            double oneMin = 1/1440.0;

            // 
            // calculating shadow angles
            dSun = functor.getSunCoordinates().distanceFromEarth;
            dMoon = functor.getMoonCoordinates().distanceFromEarth;

            double sinalpha = (SUN_RADIUS - EARTH_RADIUS) / dSun;
            double dx = EARTH_RADIUS / sinalpha - dMoon;

            // diameter of full shadow in km in distance of moon
            fullShadowRadius = sinalpha * dx;
            m1 = functor.getMoonCoordinates().getVisibleAngle(MOON_RADIUS);
            m2 = functor.getMoonCoordinates().getVisibleAngle(fullShadowRadius);

            sinalpha = (SUN_RADIUS + EARTH_RADIUS) / dSun;
            dx = EARTH_RADIUS / sinalpha;
            penumShadowRadius = (dx + dMoon) * sinalpha;
            m3 = functor.getMoonCoordinates().getVisibleAngle(penumShadowRadius);

            fullShadowInner = m2 - m1;
            fullShadowOuter = m1 + m2;
            penumbralShadowOuter = m3 + m1;
            penumbralShadowInner = m3 - m1;

            dx = functor.getDoubleValue(times[4]);
            if (dx > penumbralShadowOuter)
                times[4] = -1;

            // start of penumbral
            if (times[4] > 0)
            {
                times[0] = sr.findSet(startDate - 0.5, times[4], penumbralShadowOuter);
                if (times[0] > 0 && penumbralShadowInner > fullShadowOuter)
                    times[1] = sr.findSet(times[0], times[4], penumbralShadowInner);
                if (Math.Abs(times[1] - times[4]) < oneMin)
                    times[1] = -1;

                // full start
                if (times[0] > 0)
                    times[2] = sr.findSet(times[0], times[4], fullShadowOuter);
                if (Math.Abs(times[2] - times[4]) < oneMin)
                    times[2] = -1;
                if (times[2] > 0)
                    times[3] = sr.findSet(times[2], times[4], fullShadowInner);
                if (Math.Abs(times[3] - times[4]) < oneMin)
                    times[3] = -1;

                // end of penumbral
                times[8] = sr.findRise(times[4], startDate + 0.5, penumbralShadowOuter);
                if (times[8] > 0 && penumbralShadowInner > fullShadowOuter)
                    times[7] = sr.findRise(times[4], times[8], penumbralShadowInner);

                // full end
                if (times[8] > 0)
                    times[6] = sr.findRise(times[4], times[8], fullShadowOuter);
                if (Math.Abs(times[6] - times[8]) < oneMin)
                    times[6] = -1;
                if (times[6] > 0)
                    times[5] = sr.findRise(times[4], times[6], fullShadowInner);
                if (Math.Abs(times[5] - times[6]) < oneMin)
                    times[5] = -1;
            }

            for (int i = 0; i < 9; i++)
            {
                if (times[i] > 0)
                    times[i] -= deltaT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate">This must be greenwich julian date</param>
        /// <returns></returns>
        public static double FindNextConjunction(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate)/86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSunMoon F = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.RightAscession);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, 0.99);

            if (next > 0)
            {
                return ConvertDynamicToUniversal(next);
            }
            return -1;
        }

        /// <summary>
        /// Finds previous sun/moon conjunction time
        /// </summary>
        /// <param name="startDate">Starting day of looking. This must be greenwich julian date</param>
        /// <returns>Universal time (Julian Day) of conjunction</returns>
        public static double FindPrevConjunction(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSunMoon F = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.RightAscession);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, -0.99);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindNextTithiStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSunMoon F = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.TithiDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, 0.3);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindPrevTithiStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSunMoon F = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.TithiDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, -0.3);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindNextSankrantiStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSun F = new GPFunctorSun(GPFunctorSun.ValueType.SankrantiDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, 12.0);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindNextNaksatraStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorMoon F = new GPFunctorMoon(GPFunctorMoon.ValueType.NaksatraDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, 0.3);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindPrevNaksatraStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorMoon F = new GPFunctorMoon(GPFunctorMoon.ValueType.NaksatraDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, -0.3);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindNextYogaStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSunMoon F = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.YogaDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, 0.3);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }

        public static double FindPrevYogaStart(double startDate)
        {
            double deltaT = GPDynamicTime.GetDeltaT(startDate) / 86400;
            double date = startDate + deltaT;
            double next = 0;

            GPFunctorSunMoon F = new GPFunctorSunMoon(GPFunctorSunMoon.ValueType.YogaDistance);
            GPSearch sr = new GPSearch(F);
            sr.setSilentErrorValue(true, -1);


            next = sr.findNextMinimum(startDate, -0.3);

            if (next > 0)
                return ConvertDynamicToUniversal(next);
            return -1;
        }


        public static double ConvertDynamicToUniversal(double next)
        {
            return next - GPDynamicTime.GetDeltaT(next) / 86400;
        }


        public static double ConvertUniversalToDynamic(double next)
        {
            return next + GPDynamicTime.GetDeltaT(next) / 86400;
        }
    }

}

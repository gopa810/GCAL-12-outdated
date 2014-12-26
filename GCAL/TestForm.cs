using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GCAL.Base;

namespace GCAL
{
    public partial class TestForm : Form, IReportProgress
    {
        public ContentServer content = null;

        public TestForm()
        {
            InitializeComponent();
        }

        public void ReportProgressBase(double d)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GPLocation loc;
            content.findLocations("Bratislava");
            loc = content.getLocation(0);
            GPLocationProvider provider = new GPLocationProvider(loc);
            int nCount = 365;

            GPGregorianTime startDateA = new GPGregorianTime(loc);
            GPCalendarResults calA = new GPCalendarResults();
            GPCalendarResults calB = new GPCalendarResults();

            calA.progressReport = this;
            calB.progressReport = this;

            if (startDateA != null)
            {
                GPSun.sunPosMethod = GPSun.SUNPOSMETHOD_CALCULATOR;
                calA.CalculateCalendar(startDateA, nCount);
                GPSun.sunPosMethod = GPSun.SUNPOSMETHOD_CALCULATOREX;
                calB.CalculateCalendar(startDateA, nCount);
            }
            GPSun.sunPosMethod = GPSun.SUNPOSMETHOD_CALCULATOR;

            StringBuilder sb = new StringBuilder();

            GPCalendarTwoLocResults cals = new GPCalendarTwoLocResults();
            cals.CalendarA = calA;
            cals.CalendarB = calB;

            FormaterHtml.WriteCompareCalendarHTML(cals, sb);

            string HtmlText = sb.ToString();

            webBrowser1.DocumentText = HtmlText;

            GPObserver obs = new GPObserver();
            obs = loc;

            GPStrings.showNumberOfString = false;
            StringBuilder sba = new StringBuilder();
            GPJulianTime sunRise, sunNoon, sunSet;
            for (int i = 0; i < calA.getCount(); i++)
            {
                GPCalendarDay cd = calA.get(i);
                GPCalendarDay cd2 = calB.get(i);
                GPAstroEngine.CalculateTimeSun(cd.date, loc, out sunRise, out sunNoon, out sunSet);
                GPGregorianTime gt = new GPGregorianTime(loc);

                GPCelestialBodyCoordinates pos = GPAstroEngine.sun_coordinate(GPDynamicTime.getUniversalTimeFromDynamicTime(2457012.82313));
                GPAstroEngine.calcHorizontal(pos, loc);

                sunRise.setLocalTimezoneOffset(loc.getTimeZoneOffsetHours());
                sba.AppendFormat("{0}     {1}    {2}  \n", cd.date.ToString(),
                    cd.getSunriseTime().getLongTimeString(),
                    cd2.getSunriseTime().getLongTimeString());
                gt.setDate(1992,10,13);
                //cd.astrodata.sun.calculateCoordinatesMethodM(gt, 360/24.0);
            }
            GPStrings.showNumberOfString = true;

            richTextBox1.Text = sba.ToString();


            GPGregorianTime t1 = new GPGregorianTime(loc);
            t1.setDate(2015, 4, 4);

            double jd = t1.getJulianLocalNoon();

            sba.Clear();
            for (double d = 0.3; d < 1.0; d += 0.01)
            {
                double ml = GPAstroEngine.moon_coordinate(jd + d).eclipticalLongitude;
                double sl1 = GPAstroEngine.sun_coordinate(jd + d).eclipticalLongitude;
                double sl2 = GPAstroEngine.sunLongitudeMethodM(jd + d);
                sba.AppendFormat("{0} : {1} {2} {3}\n", jd+d, ml, sl1, sl2);
            }
            richTextBox2.Text = sba.ToString();
        }
    }
}

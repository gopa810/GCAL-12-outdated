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
            UpdateSunPosValues();


        }

        public void ReportProgressBase(double d)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GPLocation loc;
            content.findLocations("Bratislava");
            loc = content.getLocation(0);
            GPLocation provider = new GPLocation(loc);
            int nCount = 365;

            GPGregorianTime startDateA = new GPGregorianTime(loc);
            GPCalendarResults calA = new GPCalendarResults();
            GPCalendarResults calB = new GPCalendarResults();

            calA.progressReport = this;
            calB.progressReport = this;

            if (startDateA != null)
            {
//                GPSun.sunPosMethod = GPSun.SUNPOSMETHOD_CALCULATOR;
                calA.CalculateCalendar(startDateA, nCount);
                //GPSun.sunPosMethod = GPSun.SUNPOSMETHOD_CALCULATOREX;
                calB.CalculateCalendar(startDateA, nCount);
            }
            //GPSun.sunPosMethod = GPSun.SUNPOSMETHOD_CALCULATOR;

            StringBuilder sb = new StringBuilder();

            GPCalendarTwoLocResults cals = new GPCalendarTwoLocResults();
            cals.CalendarA = calA;
            cals.CalendarB = calB;

            FormaterHtml.WriteCompareCalendarHTML(cals, sb);

            string HtmlText = sb.ToString();

            webBrowser1.DocumentText = HtmlText;

            GPObserver obs = new GPObserver();
            obs = loc;

            GPStrings.pushRich(false);

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

            GPStrings.popRich();

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

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            UpdateSunPosValues();
        }

        private void UpdateSunPosValues()
        {
            GPLocation location = new GPLocation();
            location.setCity("Vrakun");
            location.setCountryCode("SK");
            location.setLatitudeNorthPositive(47.93922);
            location.setLongitudeEastPositive(17.59145);
            location.setTimeZoneName("Europe/Bratislava");


            GPGregorianTime time = new GPGregorianTime(location);
            time.setDate((int)numericUpDown1.Value, (int)numericUpDown2.Value, (numericUpDown3.Value <= GPGregorianTime.GetMonthMaxDays((int)numericUpDown1.Value, (int)numericUpDown2.Value) ? (int)numericUpDown3.Value : GPGregorianTime.GetMonthMaxDays((int)numericUpDown1.Value, (int)numericUpDown2.Value)));
            time.setDayHours((int)numericUpDown4.Value, (int)numericUpDown5.Value, (int)numericUpDown6.Value);

            GPCelestialBodyCoordinates coord = GPAstroEngine.sun_coordinate(time.getJulianGreenwichTime());

            GPAstroEngine.calcHorizontal(coord, location);

            //Log("Sun Coordinates: Azimut: {0}, Elevation: {1}", coord.azimuth, coord.elevation);

            textBox1.Text = coord.eclipticalLongitude.ToString();
            textBox2.Text = coord.eclipticalLatitude.ToString();
            textBox3.Text = coord.right_ascession.ToString();
            textBox4.Text = coord.declination.ToString();
            textBox5.Text = GPMath.putIn360(coord.azimuth + 180).ToString();
            textBox6.Text = coord.elevation.ToString();

            double jd = time.getJulianGreenwichTime();

            textBox7.Text = time.getShortDateString() + " " + time.getLongTimeString() + " JD: " + jd.ToString();

            GPMoon moon = new GPMoon();
            moon.MoonCalc(time.getJulianGreenwichTime());
            textBox8.Text = moon.longitude_deg.ToString();
            textBox9.Text = moon.latitude_deg.ToString();

        }
    }
}

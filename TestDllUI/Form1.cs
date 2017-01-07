using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using GCAL.Base;

namespace TestDllUI
{
    public partial class Form1 : Form
    {
        GPLocation loc;
        DateTime startDate;
        DateTime endDate;
        int days;

        //
        // initialization of main window
        //
        public Form1()
        {
            InitializeComponent();

            //
            // initialization of combobox with list of timezones
            //
            GPTimeZoneList tzl = GPTimeZoneList.sharedTimeZones();
            List<GPTimeZone> tzlist =  tzl.getTimeZones();

            comboBox1.BeginUpdate();
            foreach (GPTimeZone tz in tzlist)
            {
                comboBox1.Items.Add(tz);
            }
            comboBox1.EndUpdate();

            comboBox1.Text = "Europe/Bratislava";
        }


        /// <summary>
        /// Getting values of location and time from user interface
        /// In GCAL.Base there are used classes:
        ///  - GPGregorianTime - for storing calendar date and time
        ///  - GPLocationProvider - for storing location
        /// In user interface, there are usually strings, double and DateTime
        /// This function is specific for this test application, but you 
        /// can see how the values are converted
        /// </summary>
        public void initVars()
        {
            GPLocation loca = new GPLocation();
            loca.setCity(textBox1.Text);
            double d;
            int l;
            if (double.TryParse(textBox2.Text, out d))
            {
                loca.setLongitudeEastPositive(d);
            }
            else
            {
                loca.setLongitudeEastPositive(0);
                textBox2.Text = "0.0";
            }
            if (double.TryParse(textBox3.Text, out d))
            {
                loca.setLatitudeNorthPositive(d);
            }
            else
            {
                loca.setLatitudeNorthPositive(0.0);
                textBox3.Text = "0.0";
            }

            loca.setTimeZoneName(comboBox1.Text);
            loc = loca;

            if (!int.TryParse(textBox4.Text, out l))
            {
                l = 1;
                textBox4.Text = "1";
            }

            if (l <= 0)
            {
                l = 1;
                textBox4.Text = "1";
            }

            days = l;

            //
            // converting dates
            //
            DateTime dd = dateTimePicker1.Value;
            DateTime dt = dateTimePicker2.Value;

            startDate = new DateTime(dd.Year, dd.Month, dd.Day, dt.Hour, dt.Minute, dt.Second);
            endDate = startDate.AddDays(days);

        }

        /// <summary>
        /// Helper function for showing Xml results in TextView
        /// </summary>
        /// <param name="doc"></param>
        private void SetXmlDocumentToTextView(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            doc.WriteTo(writer);
            writer.Flush();
            richTextBox1.Text = sb.ToString();
        }

        /// <summary>
        /// Calculate calendar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClick_CalculateCalendar(object sender, EventArgs e)
        {
            initVars();

            // getting XML document
            XmlDocument xdoc =  GCALBase.CalculateCalendar(loc, startDate, days);

            // pushing content to user interface
            SetXmlDocumentToTextView(xdoc);

        }

        /// <summary>
        /// Calculate Sankrantis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClick_CalculateSankranti(object sender, EventArgs e)
        {
            initVars();

            // getting XML document
            XmlDocument xdoc = GCALBase.CalculateSankrantis(loc, startDate, endDate);

            // pushing content to user interface
            SetXmlDocumentToTextView(xdoc);

        }

        private void buttonClick_CalculateAppDay(object sender, EventArgs e)
        {
            initVars();

            //
            // location is GCAL.Base.LocationProvider object made by method EncapsulateLocation
            // datetime is System.DateTime structure
            XmlDocument xdoc = GCALBase.CalculateAppearanceDay(loc, startDate);

            SetXmlDocumentToTextView(xdoc);
        }
    }
}

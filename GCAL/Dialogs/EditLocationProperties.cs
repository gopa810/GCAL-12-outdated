using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GCAL.Base;

namespace GCAL.Dialogs
{
    public partial class EditLocationProperties : Form
    {
        public EditLocationProperties()
        {
            InitializeComponent();

            GPCountryList list = GPCountryList.getShared();
            GPTimeZoneList tzones = GPTimeZoneList.sharedTimeZones();

            comboBox1.BeginUpdate();
            foreach (GPCountry country in list.countries)
            {
                comboBox1.Items.Add(country);
            }
            comboBox1.EndUpdate();

            comboBox2.BeginUpdate();
            foreach (GPTimeZone tzone in tzones.getTimeZones())
            {
                comboBox2.Items.Add(tzone);
            }
            comboBox2.EndUpdate();

            ValidateInfo();
        }

        public void SetUserInterfaceStrings()
        {
            this.button1.Text = GPStrings.getString(236);
            this.button2.Text = GPStrings.getString(237);
            this.label1.Text = GPStrings.getString(9);
            this.label2.Text = GPStrings.getString(10);
            this.label3.Text = GPStrings.getString(11);
            this.label4.Text = GPStrings.getString(252);
            this.labelMessage.Text = GPStrings.getString(270);
            this.label5.Text = GPStrings.getString(12);
            this.Text = GPStrings.getString(269);
        }


        public string LocationName { get { return textBox1.Text; } set { textBox1.Text = value; ValidateInfo(); } }
        public string Latitude { get { return textBox2.Text; } set { textBox2.Text = value; ValidateInfo(); } }
        public string Longitude { get { return textBox3.Text; } set { textBox3.Text = value; ValidateInfo(); } }
        public GPCountry Country { get { return comboBox1.SelectedItem as GPCountry; } set { comboBox1.SelectedItem = value; ValidateInfo(); } }
        public GPTimeZone TimeZone { get { return comboBox2.SelectedItem as GPTimeZone; } set { comboBox2.SelectedItem = value; ValidateInfo(); } }

        public void ValidateInfo()
        {
            double d;
            if (LocationName.Length == 0)
            {
                labelMessage.Text = GPStrings.getString(295);
                labelMessage.ForeColor = Color.Red;
                button1.Enabled = false;
            }
            else if (Latitude.Length == 0)
            {
                labelMessage.Text = GPStrings.getString(294);
                labelMessage.ForeColor = Color.Red;
                button1.Enabled = false;
            }
            else if (Longitude.Length == 0)
            {
                labelMessage.Text = GPStrings.getString(293);
                labelMessage.ForeColor = Color.Red;
                button1.Enabled = false;
            }
            else if (Country == null)
            {
                labelMessage.Text = GPStrings.getString(292);
                labelMessage.ForeColor = Color.Red;
                button1.Enabled = false;
            }
            else if (TimeZone == null)
            {
                labelMessage.Text = GPStrings.getString(291);
                labelMessage.ForeColor = Color.Red;
                button1.Enabled = false;
            }
            else if (!GPLocation.ConvertStringToCoordinate(Latitude, out d))
            {
                labelMessage.Text = GPStrings.getString(290);
                labelMessage.ForeColor = Color.Orange;
                button1.Enabled = false;
            }
            else if (!GPLocation.ConvertStringToCoordinate(Longitude, out d))
            {
                labelMessage.Text = GPStrings.getString(289);
                labelMessage.ForeColor = Color.Orange;
                button1.Enabled = false;
            }
            else
            {
                labelMessage.Text = GPStrings.getString(288);
                labelMessage.ForeColor = Color.Green;
                button1.Enabled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInfo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateInfo();
        }
    }
}

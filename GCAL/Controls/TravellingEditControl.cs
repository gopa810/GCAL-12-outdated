using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GCAL.Base;

namespace GCAL.Controls
{
    public partial class TravellingEditControl : UserControl
    {
        public LocationPickerControl LocationPicker { get; set; }
        public GPLocation LocationObj { get; set; }
        public LocationPickerControl LocationPicker2 { get; set; }
        public GPLocation LocationObj2 { get; set; }
        private bool lockedFeedback = false;
        private DateTime minimumUtcTime = new DateTime(2000, 1, 1);
        private int travellingTimezoneIndex = 0;

        public TravellingEditControl()
        {
            InitializeComponent();
            //groupBox1.Visible = false;
            UpdateUTCTimeLabel(0);
            UpdateUTCTimeLabel(1);
        }

        public DateTime getDateTime(int pos)
        {
            if (pos == 0)
                return dateTimePicker2.Value;
            return dateTimePicker3.Value;
        }

        public DateTime getUTCDateTime(int pos)
        {
            DateTime dt = getDateTime(pos);
            GPTimeZone tz = getTimeZone(travellingTimezoneIndex);
            if (tz != null)
            {
                double offset = tz.getOffsetHours();

                dt = dt.AddHours(-offset);
            }
            return dt;
        }

        public void setDateTime(DateTime dt, int pos)
        {
            if (pos == 0)
                dateTimePicker2.Value = dt;
            else
            {
                if (dt < dateTimePicker3.MinDate)
                    dateTimePicker3.Value = dateTimePicker3.MinDate;
                else if (dt > dateTimePicker3.MaxDate)
                    dateTimePicker3.Value = dateTimePicker3.MaxDate;
                else
                    dateTimePicker3.Value = dt;
            }
        }

        public void setUTCDateTime(DateTime dt, int pos)
        {
            GPTimeZone tz = getTimeZone(travellingTimezoneIndex);
            if (tz != null)
            {
                dt = dt.AddHours(tz.getOffsetHours());

                setDateTime(dt, pos);
            }
        }

        private void UpdateUTCTimeLabel()
        {
            UpdateUTCTimeLabel(0);
            UpdateUTCTimeLabel(1);
        }

        private void UpdateUTCTimeLabel(int pos)
        {
            DateTime dt = getUTCDateTime(pos);

            if (pos == 0)
                labelUTCEquivalent.Text = string.Format("Time equivalent to {0} UTC", dt.ToString("yyyy/MM/dd HH:mm:ss"));
            else
                labelUtcEqivalent2.Text = string.Format("Time equivalent to {0} UTC", dt.ToString("yyyy/MM/dd HH:mm:ss"));
        }
        
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            UpdateMinMaxDate();

            UpdateUTCTimeLabel(0);

            if (!lockedFeedback)
            {
                lockedFeedback = true;
                TimeSpan ts = dateTimePicker3.Value - dateTimePicker2.Value;
                textBox1.Text = ts.TotalHours.ToString();
                ValidateDurationTextBoxValue();
                lockedFeedback = false;
            }
        }

        private void UpdateMinMaxDate()
        {
            DateTime newDate = dateTimePicker2.Value;
            DateTime newDateMax = newDate.AddHours(24);
            DateTime newDateMin = newDate.AddMinutes(30);
            if (newDateMax < dateTimePicker3.MinDate)
            {
                dateTimePicker3.MinDate = newDateMin;
                dateTimePicker3.MaxDate = newDateMax;
            }
            else
            {
                dateTimePicker3.MaxDate = newDateMax;
                dateTimePicker3.MinDate = newDateMin;
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            UpdateUTCTimeLabel(1);

            if (!lockedFeedback)
            {
                lockedFeedback = true;
                TimeSpan ts = dateTimePicker3.Value - dateTimePicker2.Value;
                textBox1.Text = ts.TotalHours.ToString();
                ValidateDurationTextBoxValue();
                lockedFeedback = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!lockedFeedback)
            {
                lockedFeedback = true;
                //TimeSpan ts = getUTCDateTime(1) - getUTCDateTime(0);
                double d;
                if (!double.TryParse(textBox1.Text, out d))
                {
                    textBox1.Text = "0.5";
                    d = 0.5;
                }
                if (d < 0)
                {
                    textBox1.Text = "0.5";
                    d = 0.5;
                }
                else if (d > 24)
                {
                    textBox1.Text = "24";
                    d = 24;
                }
                if (d > 0)
                {
                    DateTime startTime = getUTCDateTime(0);
                    startTime = startTime.AddHours(d);
                    setUTCDateTime(startTime, 1);
                }
                lockedFeedback = false;
            }
            UpdateDurationTextBox();
        }

        private void UpdateDurationTextBox()
        {
            ValidateDurationTextBoxValue();
        }

        private void ValidateDurationTextBoxValue()
        {
            double d;
            if (double.TryParse(textBox1.Text, out d))
            {
                if (d < 0)
                {
                    textBox1.ForeColor = Color.Red;
                    labelWarning.Visible = true;
                }
                else
                {
                    textBox1.ForeColor = SystemColors.WindowText;
                    labelWarning.Visible = false;
                }
            }
            else
            {
                textBox1.Text = "0.5";
            }
        }

        private GPTimeZone getTimeZone(int index)
        {
            LocationPickerControl lpic = (index == 0) ? LocationPicker : LocationPicker2;
            if (lpic != null)
            {
                GPLocationProvider lp = lpic.SelectedLocation;
                if (lp != null)
                    return lp.getTimeZone();
            }

            GPLocation loc = (index == 0) ? LocationObj : LocationObj2;
            if (loc != null)
                return loc.getTimeZone();

            return null;
        }

        private void UpdateTimeZoneLabel()
        {
            UpdateTimeZoneLabel(0);
            UpdateTimeZoneLabel(1);
        }

        private void UpdateTimeZoneLabel(int i)
        {
            GPTimeZone tz = getTimeZone(travellingTimezoneIndex);
            Label label = ((i == 0) ? labelTimezone : labelTimezone2);

            if (tz != null)
            {
                label.Text = string.Format("Timezone: {0}", tz.getFullName());
                DateTime minDate = minimumUtcTime.AddHours(tz.getOffsetHours());
            }
            else
            {
                label.Text = string.Format("Timezone: {0}", "+00:00 UTC");
            }
        }

        private void TravellingEditControl_VisibleChanged(object sender, EventArgs e)
        {
            UpdateTimeZoneLabel();
            UpdateUTCTimeLabel();

            ValidateDurationTextBoxValue();
            GPTimeZone tz;

            tz = getTimeZone(0);
            if (tz != null)
                radioButton1.Text = tz.getFullName();

            tz = getTimeZone(1);
            if (tz != null)
                radioButton2.Text = tz.getFullName();
            UpdateMinMaxDate();
        }

        public int TransitionTimezoneIndex
        {
            get
            {
                if (radioButton1.Checked)
                    return 0;
                return 1;
            }
            set
            {
                if (value == 0)
                    radioButton1.Checked = true;
                else
                    radioButton2.Checked = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DateTime dt1 = getUTCDateTime(0);
            DateTime dt2 = getUTCDateTime(1);
            travellingTimezoneIndex = 0;
            UpdateTimeZoneLabel();
            setUTCDateTime(dt1, 0);
            setUTCDateTime(dt2, 1);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DateTime dt1 = getUTCDateTime(0);
            DateTime dt2 = getUTCDateTime(1);
            travellingTimezoneIndex = 1;
            UpdateTimeZoneLabel();
            setUTCDateTime(dt1, 0);
            setUTCDateTime(dt2, 1);
        }


        internal void SetTopStartDateLimit(DateTime dateTime)
        {
            dateTimePicker2.MinDate = new DateTime(1970, 1, 1);
            dateTimePicker2.MaxDate = dateTime.AddHours(-1);
        }

        internal void SetBottomEndDateLimit(DateTime dateTime)
        {
            dateTimePicker2.MinDate = dateTime.AddHours(1);
            dateTimePicker2.MaxDate = new DateTime(2300, 1, 1);
        }

    
    }
}

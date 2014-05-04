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
    public partial class EditTimezoneTransitionDlg : Form
    {
        public EditTimezoneTransitionDlg()
        {
            InitializeComponent();
        }

        public void SetUserInterfaceStrings()
        {
            this.button1.Text = GPStrings.getSharedStrings().getString(236);
            this.button2.Text = GPStrings.getSharedStrings().getString(237);
            this.label1.Text = GPStrings.getSharedStrings().getString(296);// "Date of transition";
            this.label2.Text = GPStrings.getSharedStrings().getString(297);// "Time for transition";
            this.label3.Text = GPStrings.getSharedStrings().getString(298);// "New timezone offset (in minutes)";
            this.label4.Text = GPStrings.getSharedStrings().getString(299);// "Abbreviation for given timezone offset";
            this.checkBox1.Text = GPStrings.getSharedStrings().getString(300);// "DST is in effect after this transition";
            this.Text = GPStrings.getSharedStrings().getString(301);// "Timezone Transition Details";
        }


        public long Timestamp
        {
            get
            {
                GPTimestamp tstamp = new GPTimestamp();
                tstamp.setDate(monthCalendar1.SelectionStart);
                tstamp.setTime(dateTimePicker1.Value);
                return tstamp.getValue() - TransOffsetSeconds;
            }
            set
            {
                GPTimestamp tstamp = new GPTimestamp(value);
                monthCalendar1.SelectionStart = tstamp.getDateTime();
                monthCalendar1.SelectionEnd = tstamp.getDateTime();
                dateTimePicker1.Value = tstamp.getDateTime();
            }
        }

        public void SetTimestampAndOffset(long tstamp, int offsetInSec)
        {
            Timestamp = tstamp + offsetInSec;
            TransOffsetSeconds = offsetInSec;
        }

        public int TransOffsetSeconds { get { return int.Parse(textBox1.Text)*60; } set { textBox1.Text = (value/60).ToString(); }}
        public string TransAbbr { get { return textBox2.Text; } set { textBox2.Text = value; }}
        public bool Dst { get { return checkBox1.Checked; } set { checkBox1.Checked = value; }}

        private void ValidateInfo()
        {
            bool enable = true;
            int i = 0;
            if (!int.TryParse(textBox1.Text, out i))
                enable = false;

            button1.Enabled = enable;
        }

    }
}

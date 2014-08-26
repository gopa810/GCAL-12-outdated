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
    public partial class GetDateTimeDlg : Form
    {
        private GPLocationProvider location = null;

        public GetDateTimeDlg(GPLocationProvider loc)
        {
            InitializeComponent();
            location = loc;

            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }

        public void SetUserInterfaceStrings()
        {
            this.button3.Text = GPStrings.getString(237);
            this.button2.Text = GPStrings.getString(239);
            this.button1.Text = GPStrings.getString(238);
            this.label1.Text = GPStrings.getString(302);
            this.label2.Text = GPStrings.getString(7);
            this.label3.Text = GPStrings.getString(8);
            this.Text = GPStrings.getString(302);

        }


        public GPGregorianTime SelectedDateTime
        {
            get
            {
                GPGregorianTime vc = new GPGregorianTime(location);
                DateTime date = dateTimePicker1.Value;
                DateTime time = dateTimePicker2.Value;

                vc.setDate(date.Year, date.Month, date.Day);
                vc.setDayHours(((time.Hour * 60.0 + time.Minute) * 60.0 + time.Second) / 86400.0);

                return vc;
            }
        }
    }
}

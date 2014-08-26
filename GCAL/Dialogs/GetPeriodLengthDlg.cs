using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GCAL.Base;
using GCAL.Controls;

namespace GCAL.Dialogs
{
    public partial class GetPeriodLengthDlg : Form
    {
        public GetPeriodLengthDlg(GPLocationProvider loc, GPGregorianTime startW, GPVedicTime startA)
        {
            InitializeComponent();

            timeRangeLengthControl1.SetData(loc, startW, startA);
        }

        public TimeRangeLengthControl Picker
        {
            get
            {
                return timeRangeLengthControl1;
            }
        }

        public void SetUserInterfaceStrings()
        {
            this.button3.Text = GPStrings.getString(237);
            this.button2.Text = GPStrings.getString(239);
            this.button1.Text = GPStrings.getString(238);
            this.label1.Text = GPStrings.getString(304);
            this.Text = GPStrings.getString(304);

        }



    }
}

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
    public partial class GetStartDateDlg : Form
    {
        public GetStartDateDlg(GPLocation location)
        {
            InitializeComponent();

            LocationPlace = location;
        }

        public void SetUserInterfaceStrings()
        {
            this.button3.Text = GPStrings.getString(237);
            this.button2.Text = GPStrings.getString(239);
            this.button1.Text = GPStrings.getString(238);
            this.label1.Text = GPStrings.getString(307);
            this.Text = GPStrings.getString(307);

        }


        /// <summary>
        /// location for recalutaion of dates
        /// </summary>
        public GPLocation LocationPlace
        {
            set
            {
                dateTimePickerControl1.LocationPlace = value;
            }
            get
            {
                return dateTimePickerControl1.LocationPlace;
            }
        }

        public DateTimePickerControl Picker
        {
            get
            {
                return dateTimePickerControl1;
            }
        }
    }
}

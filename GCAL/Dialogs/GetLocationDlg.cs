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
    public partial class GetLocationDlg : Form
    {
        private string prefix = "getlocation";

        public GetLocationDlg()
        {
            InitializeComponent();
            ActiveControl = locationPickerControl1;
        }

        public GetLocationDlg(string aPrefix)
        {
            InitializeComponent();
            ActiveControl = locationPickerControl1;

            prefix = aPrefix;
        }

        public void SetUserInterfaceStrings()
        {
            this.button3.Text = GPStrings.getString(237);
            this.button2.Text = GPStrings.getString(239);
            this.button1.Text = GPStrings.getString(238);
            this.label1.Text = GPStrings.getString(303);
            this.Text = GPStrings.getString(303);
        }


        private void locationPickerControl1_SelectedLocationChange(object sender, EventArgs data)
        {
            GPLocationProvider loc = locationPickerControl1.SelectedLocation;
            button2.Enabled = (loc != null);
        }

        public void LoadInterface()
        {
            locationPickerControl1.LoadInterfaceValues(prefix);
        }

        public void SaveInterface()
        {
            locationPickerControl1.SaveInterfaceValues(prefix);
        }

        public string NextButtonText
        {
            set
            {
                button2.Text = value;
            }
            get
            {
                return button2.Text;
            }
        }

        public bool PrevButtonVisible
        {
            set
            {
                button1.Visible = value;
            }
            get
            {
                return button1.Visible;
            }
        }

        public GPLocationProvider SelectedLocation
        {
            get
            {
                return locationPickerControl1.SelectedLocation;
            }
        }

    }
}

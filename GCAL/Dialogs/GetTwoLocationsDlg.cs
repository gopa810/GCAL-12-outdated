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
    public partial class GetTwoLocationsDlg : Form
    {
        public GetTwoLocationsDlg()
        {
            InitializeComponent();

            locationPickerControl1.LoadInterfaceValues("getlocationa");
            locationPickerControl2.LoadInterfaceValues("getlocationb");
        }

        public void SetUserInterfaceStrings()
        {
            this.label1.Text = GPStrings.getString(308);
            this.button3.Text = GPStrings.getString(237);
            this.button2.Text = GPStrings.getString(239);
            this.button1.Text = GPStrings.getString(238);
            this.label2.Text = GPStrings.getString(309);
            this.Text = GPStrings.getString(310);
        }


        public void SaveInterface()
        {
            locationPickerControl1.SaveInterfaceValues("getlocationa");
            locationPickerControl2.SaveInterfaceValues("getlocationb");
        }

        public GPLocationProvider SelectedLocationA
        {
            get
            {
                return locationPickerControl1.SelectedLocation;
            }
        }

        public GPLocationProvider SelectedLocationB
        {
            get
            {
                return locationPickerControl2.SelectedLocation;
            }
        }

    }
}

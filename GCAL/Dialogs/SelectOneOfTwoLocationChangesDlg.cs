using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GCAL.Dialogs
{
    public partial class SelectOneOfTwoLocationChangesDlg : Form
    {
        public SelectOneOfTwoLocationChangesDlg()
        {
            InitializeComponent();
        }

        public string TravellingNameA
        {
            set 
            {
                radioButton1.Text = value;
            }
        }

        public string TravellingNameB
        {
            set
            {
                radioButton2.Text = value;
            }
        }

        public bool SelectedA
        {
            get
            {
                return radioButton1.Checked;
            }
            set
            {
                radioButton1.Checked = value;
            }
        }

        public bool SelectedB
        {
            get
            {
                return radioButton2.Checked;
            }
            set
            {
                radioButton2.Checked = value;
            }
        }
    }
}

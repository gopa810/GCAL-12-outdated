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
    public partial class GetPeriodYearsDlg : Form
    {
        public GetPeriodYearsDlg()
        {
            InitializeComponent();

            DateTime dt = DateTime.Now;

            for (int i = 0; i < 20; i++)
            {
                comboBox1.Items.Add(dt.Year + i);
                comboBox2.Items.Add(i + 1);
            }

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

        }

        public void SetUserInterfaceStrings()
        {
            this.button3.Text = GPStrings.getString(237);
            this.button2.Text = GPStrings.getString(239);
            this.button1.Text = GPStrings.getString(238);
            this.label1.Text = GPStrings.getString(305);
            this.label2.Text = GPStrings.getString(259);
            this.label3.Text = GPStrings.getString(306);
            this.Text = GPStrings.getString(306);
        }


        public void LoadInterfaceValues(string prefix)
        {
            comboBox1.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".yp_year", 0);
            comboBox2.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".yp_count", 0);
        }

        public void SaveInterfaceValues(string prefix)
        {
            GPUserDefaults.SetIntForKey(prefix + ".yp_year", comboBox1.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".yp_count", comboBox2.SelectedIndex);
        }

        public int SelectedYear
        {
            get
            {
                return int.Parse(comboBox1.SelectedItem.ToString());
            }
        }

        public int SelectedCount
        {
            get
            {
                return int.Parse(comboBox2.SelectedItem.ToString());
            }
        }
    }
}

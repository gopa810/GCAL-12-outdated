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
    public partial class EditSankrantiBasedEventDlg : Form
    {
        public static int dayOffset = 8;

        public EditSankrantiBasedEventDlg()
        {
            InitializeComponent();

            for (int i = 0; i < GPSankranti.getCount(); i++)
            {
                comboBox1.Items.Add(GPSankranti.getName(i));
            }
            comboBox1.SelectedIndex = 0;

            for (int i = 0; i < (2*dayOffset+1); i++)
            {
                comboBox2.Items.Add(string.Format("{0}", i-dayOffset));
            }
            comboBox2.SelectedIndex = 0;

            for (int i = 0; i < GPFastType.count(); i++)
            {
                comboBox3.Items.Add(new GPFastType(i));
            }
            comboBox3.SelectedIndex = 0;

            for (int i = 0; i < GPEventClass.count(); i++)
            {
                comboBox4.Items.Add(new GPEventClass(i));
            }
            comboBox4.SelectedIndex = 0;
        }

        public string EventTitle { get { return textBox1.Text; } set { textBox1.Text = value; } }
        public string FastSubject { get { return textBox2.Text; } set { textBox2.Text = value; } }
        public string SinceYear { get { return textBox3.Text; } set { textBox3.Text = value; } }
        public int Sankranti { get { return comboBox1.SelectedIndex; } set { comboBox1.SelectedIndex = value; } }
        public int Offset { get { return comboBox2.SelectedIndex - dayOffset; } set { comboBox2.SelectedIndex = (value + dayOffset); } }
        public int FastType { get { return comboBox3.SelectedIndex; } set { comboBox3.SelectedIndex = value; } }
        public int Group { get { return comboBox4.SelectedIndex; } set { comboBox4.SelectedIndex = value; } }
        public bool EventVisible { get { return checkBox1.Checked; } set { checkBox1.Checked = value; } }

        public void SetUserInterfaceStrings()
        {
            this.checkBox1.Text = GPStrings.getSharedStrings().getString(279);
            this.label7.Text = GPStrings.getSharedStrings().getString(280);
            this.label6.Text = GPStrings.getSharedStrings().getString(277);
            this.label5.Text = GPStrings.getSharedStrings().getString(276);
            this.label4.Text = GPStrings.getSharedStrings().getString(275);
            this.label3.Text = GPStrings.getSharedStrings().getString(281);
            this.label2.Text = GPStrings.getSharedStrings().getString(56);
            this.label1.Text = GPStrings.getSharedStrings().getString(278);
            this.button2.Text = GPStrings.getSharedStrings().getString(237);
            this.button1.Text = GPStrings.getSharedStrings().getString(236);
            this.Text = GPStrings.getSharedStrings().getString(273);
        }


    }
}

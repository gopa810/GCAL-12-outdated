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
    public partial class EditTithiEvent : Form
    {
        private string previousSinceYear = string.Empty;

        public EditTithiEvent()
        {
            InitializeComponent();

            for (int i = 0; i < GPTithi.getCount(); i++)
            {
                comboBox1.Items.Add(new GPTithi(i,true));
            }
            comboBox1.SelectedIndex = 0;

            for (int i = 0; i < GPMasa.getCount(); i++)
            {
                comboBox2.Items.Add(new GPMasa(i));
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

        public void SetUserInterfaceStrings()
        {
            this.button1.Text = GPStrings.getSharedStrings().getString(236);
            this.button2.Text = GPStrings.getSharedStrings().getString(237);
            this.label1.Text = GPStrings.getSharedStrings().getString(278);
            this.label2.Text = GPStrings.getSharedStrings().getString(13);
            this.label3.Text = GPStrings.getSharedStrings().getString(22);
            this.label4.Text = GPStrings.getSharedStrings().getString(275);
            this.label5.Text = GPStrings.getSharedStrings().getString(276);
            this.label6.Text = GPStrings.getSharedStrings().getString(277);
            this.label7.Text = GPStrings.getSharedStrings().getString(280);
            this.checkBox1.Text = GPStrings.getSharedStrings().getString(279);
            this.Text = GPStrings.getSharedStrings().getString(274);
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (int.TryParse(textBox3.Text, out i))
            {
                previousSinceYear = textBox3.Text;
            }
            else
            {
                textBox3.Text = previousSinceYear;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = (textBox1.Text.Length > 0);
        }

        public string EventTitle { get { return textBox1.Text; } set { textBox1.Text = value; } }
        public string FastSubject { get { return textBox2.Text; } set { textBox2.Text = value; } }
        public string SinceYear { get { return textBox3.Text; } set { textBox3.Text = value; } }
        public int Tithi { get { return comboBox1.SelectedIndex; } set { comboBox1.SelectedIndex = value; } }
        public int Masa { get { return comboBox2.SelectedIndex; } set { comboBox2.SelectedIndex = value; } }
        public int FastType { get { return comboBox3.SelectedIndex; } set { comboBox3.SelectedIndex = value; } }
        public int Group { get { return comboBox4.SelectedIndex; } set { comboBox4.SelectedIndex = value; } }
        public bool EventVisible { get { return checkBox1.Checked; } set { checkBox1.Checked = value; } }
    }
}

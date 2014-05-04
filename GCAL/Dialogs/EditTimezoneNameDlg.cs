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
    public partial class EditTimezoneNameDlg : Form
    {
        public EditTimezoneNameDlg()
        {
            InitializeComponent();
            ValidateInfo();
        }

        public string TimezoneName { get { return textBox1.Text; } set { textBox1.Text = value; ValidateInfo();  } }
        public int TimezoneOffset { get { return int.Parse(textBox2.Text); } set { textBox2.Text = value.ToString(); ValidateInfo();  } }

        public void SetUserInterfaceStrings()
        {
            this.button1.Text = GPStrings.getSharedStrings().getString(236);
            this.button2.Text = GPStrings.getSharedStrings().getString(237);
            this.label1.Text = GPStrings.getSharedStrings().getString(283);
            this.label2.Text = GPStrings.getSharedStrings().getString(284);
            this.Text = GPStrings.getSharedStrings().getString(283);
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void ValidateInfo()
        {
            int i = 0;
            bool buttonOkEnable = true;

            if (textBox1.Text.Length == 0)
                buttonOkEnable = false;

            if (int.TryParse(textBox2.Text, out i) == false)
                buttonOkEnable = false;

            button1.Enabled = buttonOkEnable;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateInfo();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidateInfo();
        }

    }
}

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
    public partial class EditCountryDlg : Form
    {
        private string prevCode = "";
        public string ValidCode = "_";

        public EditCountryDlg()
        {
            InitializeComponent();
            ValidateInfo();
        }

        public void SetUserInterfaceStrings()
        {
            this.label1.Text = GPStrings.getSharedStrings().getString(253);
            this.label2.Text = GPStrings.getSharedStrings().getString(254);
            this.button1.Text = GPStrings.getSharedStrings().getString(236);
            this.button2.Text = GPStrings.getSharedStrings().getString(237);
            this.Text = GPStrings.getSharedStrings().getString(255);
        }


        public string CountryCode { get { return textBox1.Text; } set { textBox1.Text = value; ValidateInfo();  } }
        public string CountryName { get { return textBox2.Text; } set { textBox2.Text = value; ValidateInfo();  } }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            for(int i = 0; i < str.Length; i++)
            {
                if (!Char.IsLetter(str[i]))
                {
                    textBox1.Text = prevCode;
                    return;
                }
            }
            prevCode = textBox1.Text;
            ValidateInfo();
        }

        private void ValidateInfo()
        {
            if (textBox1.Text.Length == 0)
            {
                button1.Enabled = false;
                label3.Text = GPStrings.getSharedStrings().getString(285);
                label3.ForeColor = Color.Red;
            }
            else if (GPCountryList.getShared().ExistsCode(textBox1.Text) && textBox1.Text != ValidCode)
            {
                button1.Enabled = false;
                label3.Text = GPStrings.getSharedStrings().getString(286);
                label3.ForeColor = Color.Red;
            }
            else
            {
                button1.Enabled = true;
                label3.Text = GPStrings.getSharedStrings().getString(288);
                label3.ForeColor = Color.Green;
            }

            if (textBox2.Text.Length == 0)
            {
                button1.Enabled = false;
                label4.Text = GPStrings.getSharedStrings().getString(287);
                label4.ForeColor = Color.Red;
            }
            else
            {
                label4.Text = GPStrings.getSharedStrings().getString(288);
                label4.ForeColor = Color.Green;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidateInfo();
        }

    }
}

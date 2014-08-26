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
    public partial class EditLanguageNameDlg : Form
    {
        public EditLanguageNameDlg()
        {
            InitializeComponent();
        }

        public string LangName { get { return textBox1.Text; } set { textBox1.Text = value; } }

        public void SetUserInterfaceStrings()
        {
            this.button1.Text = GPStrings.getString(236);
            this.button2.Text = GPStrings.getString(237);
            this.label1.Text = GPStrings.getString(267);
            this.Text = GPStrings.getString(268);
        }


    }
}

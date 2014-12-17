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
    public partial class DialogEditString : Form
    {
        public DialogEditString()
        {
            InitializeComponent();
        }

        public DialogEditString(int index)
        {
            InitializeComponent();
            setStringIndex(index);
        }

        public void setStringIndex(int index)
        {
            label1.Text = String.Format("Original English Text [{0}]", index);
            label2.Text = GPStrings.getOriginalString(index);
            textBox1.Text = GPStrings.getPlainString(index);
        }

        public string getNewText()
        {
            return textBox1.Text;
        }
    }


}

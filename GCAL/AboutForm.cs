using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GCAL.Base;

namespace GCAL
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            this.Text = GPStrings.getSharedStrings().getString(462);
            label6.Text = GPAppHelper.getLongVersionText();
            label1.Text = GPStrings.getSharedStrings().getString(463);
            label2.Text = GPStrings.getSharedStrings().getString(464);
            label3.Text = GPStrings.getSharedStrings().getString(465);
            label4.Text = GPStrings.getSharedStrings().getString(466);
        }
    }
}

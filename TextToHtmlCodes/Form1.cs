using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TextToHtmlCodes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.SelectAll();
            richTextBox2.Copy();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string s = richTextBox1.Text;
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < s.Length; i++)
            {
                double d = Convert.ToInt32(s[i]);
                if (d > 128)
                {
                    sb.AppendFormat("&#{0};", Convert.ToInt32(d));
                }
                else
                {
                    sb.Append(s[i]);
                }
            }

            richTextBox2.Text = sb.ToString();
        }
    }
}

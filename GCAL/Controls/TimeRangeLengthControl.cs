using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GCAL.Base;

namespace GCAL.Controls
{
    public partial class TimeRangeLengthControl : UserControl
    {
        private string prevCount = "1";

        private GPLocationProvider location = null;
        private GPGregorianTime startWesternTime = null;
        private GPGregorianTime endWesternTime = null;
        private GPVedicTime startVedicTime = null;
        private GPVedicTime endVedicTime = null;
        private int nCount = 1;
        private int unitType = 4;

        public TimeRangeLengthControl()
        {
            InitializeComponent();
        }

        public void SetUserInterfaceStrings()
        {
            this.groupBox1.Text = GPStrings.getString(258);
            this.radioButton7.Text = GPStrings.getString(266);
            this.radioButton6.Text = GPStrings.getString(265);
            this.radioButton5.Text = GPStrings.getString(264);
            this.radioButton4.Text = GPStrings.getString(32);
            this.radioButton3.Text = GPStrings.getString(31);
            this.radioButton2.Text = GPStrings.getString(263);
            this.radioButton1.Text = GPStrings.getString(30);
            this.label1.Text = GPStrings.getString(259);
            this.groupBox2.Text = GPStrings.getString(260);
            this.label5.Text = GPStrings.getString(262);
            this.label2.Text = GPStrings.getString(261);

        }

        public void SetData(GPLocationProvider loca, GPGregorianTime startWesternDate, GPVedicTime startVedicDate)
        {
            location = loca;
            startWesternTime = startWesternDate;
            startVedicTime = startVedicDate;
        }

        public GPGregorianTime StartWesternDate
        {
            get
            {
                return startWesternTime;
            }
        }

        public GPVedicTime StartVedicDate
        {
            get
            {
                return startVedicTime;
            }
        }

        public GPGregorianTime EndWesternDate
        {
            get
            {
                return endWesternTime;
            }
        }

        public GPVedicTime EndVedicDate
        {
            get
            {
                return endVedicTime;
            }
        }

        public void LoadInterfaceValues(string prefix)
        {
            textBox1.Text = GPUserDefaults.StringForKey(prefix + ".gtr_count", "1");
            radioButton1.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_1", false);
            radioButton2.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_2", false);
            radioButton3.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_3", false);
            radioButton4.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_4", true);
            radioButton5.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_5", false);
            radioButton6.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_6", false);
            radioButton7.Checked = GPUserDefaults.BoolForKey(prefix + ".gtr_7", false);

            Recalculatedate();
        }

        public void SaveInterfaceValues(string prefix)
        {
            GPUserDefaults.SetStringForKey(prefix + ".gstr_count", textBox1.Text);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_1", radioButton1.Checked);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_2", radioButton2.Checked);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_3", radioButton3.Checked);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_4", radioButton4.Checked);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_5", radioButton5.Checked);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_6", radioButton6.Checked);
            GPUserDefaults.SetBoolForKey(prefix + ".gstr_7", radioButton7.Checked);
        }

        private static int[] maxCounts = new int[] {0, 30240, 4320, 1080, 90, 30240, 1080, 90 };

        private int CorrectedCount(int count)
        {
            return Math.Min(maxCounts[unitType], count);
        }

        private void Recalculatedate()
        {
            GPEngine.CalcEndDate(location, startWesternTime, startVedicTime, out endWesternTime, out endVedicTime, unitType, CorrectedCount(nCount));

            string template = "Start date: [start.western] - [start.vedic]\nEnddate: [end.western] - [end.vedic]";

            template = template.Replace("[start.western]", startWesternTime.ToString());
            template = template.Replace("[start.vedic]", startVedicTime.ToString());
            template = template.Replace("[end.western]", endWesternTime.ToString());
            template = template.Replace("[end.vedic]", endVedicTime.ToString());

            label3.Text = startWesternTime.ToString();
            label4.Text = startVedicTime.ToString();

            label6.Text = endWesternTime.ToString();
            label7.Text = endVedicTime.ToString();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int count;
            if (int.TryParse(textBox1.Text, out count))
            {
                nCount = count;
                prevCount = textBox1.Text;
                Recalculatedate();
            }
            else
            {
                //textBox1.Text = prevCount;
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 1;
            Recalculatedate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 2;
            Recalculatedate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 3;
            Recalculatedate();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 4;
            Recalculatedate();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 5;
            Recalculatedate();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 6;
            Recalculatedate();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            unitType = 7;
            Recalculatedate();
        }
    }
}

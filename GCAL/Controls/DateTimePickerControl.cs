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
    public partial class DateTimePickerControl : UserControl
    {
        private bool keepSycnhro = true;
        private string prevYear = string.Empty;
        private string prevGaurabda = string.Empty;
        private GPLocationProvider location = null;

        public void SetUserInterfaceStrings()
        {
            this.label1.Text = GPStrings.getSharedStrings().getString(240);
            this.label2.Text = GPStrings.getSharedStrings().getString(241);
            this.groupBox1.Text = GPStrings.getSharedStrings().getString(242);
            this.linkLabel5.Text = GPStrings.getSharedStrings().getString(243);
            this.linkLabel4.Text = GPStrings.getSharedStrings().getString(244);
            this.linkLabel3.Text = GPStrings.getSharedStrings().getString(245);
            this.linkLabel2.Text = GPStrings.getSharedStrings().getString(246);
            this.linkLabel1.Text = GPStrings.getSharedStrings().getString(43);
            this.label3.Text = string.Format("{0} / {1}", GPStrings.getSharedStrings().getString(13), GPStrings.getSharedStrings().getString(20));
            this.label4.Text = GPStrings.getSharedStrings().getString(22);
            this.label5.Text = GPStrings.getSharedStrings().getString(55);
            this.label6.Text = GPStrings.getSharedStrings().getString(247);
            this.label7.Text = GPStrings.getSharedStrings().getString(248);
            this.label8.Text = GPStrings.getSharedStrings().getString(55);
        }


        public GPLocationProvider LocationPlace
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }


        public DateTimePickerControl()
        {
            InitializeComponent();

            for (int i = 1; i <= 31; i++)
            {
                comboBox1.Items.Add(i);
            }

            for (int i = 0; i < 12; i++)
            {
                comboBox2.Items.Add(GPAppHelper.getMonthAbr(i + 1));
                comboBox4.Items.Add(new GPMasa(GPAppHelper.ComboMasaToMasa(i)));
            }

            for (int i = 0; i < 30; i++)
            {
                comboBox3.Items.Add(new GPTithi(i, true));
            }
        }

        public void AdjustDayCombo(int month, int year)
        {
            int sel = comboBox1.SelectedIndex;
            int max = GPGregorianTime.GetMonthMaxDays(year, month);

            while (comboBox1.Items.Count > max)
            {
                comboBox1.Items.RemoveAt(comboBox1.Items.Count - 1);
            }

            while (comboBox1.Items.Count < max)
            {
                comboBox1.Items.Add(comboBox1.Items.Count + 1);
            }
            if (sel > max - 1)
                comboBox1.SelectedIndex = (max - 1);
        }

        public void SetToday()
        {
            DateTime dt = DateTime.Now;
            SetWesternDate(dt.Year, dt.Month, dt.Day);
        }

        public GPGregorianTime GetWesternDate()
        {
            GPGregorianTime vc = new GPGregorianTime(location);

            int y;
            int.TryParse(textBox1.Text, out y);
            vc.setDate(y, comboBox2.SelectedIndex + 1, comboBox1.SelectedIndex + 1);

            return vc;
        }

        public GPVedicTime GetVedicDate()
        {
            GPVedicTime va = new GPVedicTime();

            va.tithi = comboBox3.SelectedIndex;
            va.masa = GPAppHelper.ComboMasaToMasa(comboBox4.SelectedIndex);
            int.TryParse(textBox2.Text, out va.gyear);

            return va;
        }

        public void SetWesternDate(int year, int month, int day)
        {
            keepSycnhro = false;
            comboBox2.SelectedIndex = month - 1;
            textBox1.Text = year.ToString();
            comboBox1.SelectedIndex = -1;
            keepSycnhro = true;
            comboBox1.SelectedIndex = day - 1;
        }

        public void SetVedicDate(int gyear, int masa, int tithi)
        {
            keepSycnhro = false;
            comboBox4.SelectedIndex = GPAppHelper.MasaToComboMasa(masa);
            textBox1.Text = gyear.ToString();
            comboBox3.SelectedIndex = -1;
            keepSycnhro = true;
            comboBox3.SelectedIndex = tithi;
        }

        private void WesternChanged()
        {
            if (keepSycnhro)
            {
                int year;
                keepSycnhro = false;
                if (location != null && int.TryParse(textBox1.Text, out year))
                {
                    GPVedicTime va = null;
                    GPGregorianTime vc = new GPGregorianTime(location);
                    vc.setDate(year, comboBox2.SelectedIndex + 1, comboBox1.SelectedIndex + 1);
                    vc.setDayHours(0.0);
                    GPEngine.VCTIMEtoVATIME(vc, out va, location);
                    comboBox3.SelectedIndex = va.tithi;
                    comboBox4.SelectedIndex = GPAppHelper.MasaToComboMasa(va.masa);
                    textBox2.Text = va.gyear.ToString();
                }
                keepSycnhro = true;
            }
        }

        private void VedicChanged()
        {
            if (keepSycnhro)
            {
                int gyear;
                keepSycnhro = false;
                if (location != null && int.TryParse(textBox2.Text, out gyear))
                {
                    GPGregorianTime vc = null;
                    GPVedicTime va = new GPVedicTime(comboBox3.SelectedIndex, GPAppHelper.ComboMasaToMasa(comboBox4.SelectedIndex), gyear);
                    GPEngine.VATIMEtoVCTIME(va, out vc, location);
                    comboBox2.SelectedIndex = vc.getMonth() - 1;
                    textBox1.Text = vc.getYear().ToString();
                    comboBox1.SelectedIndex = vc.getDay() - 1;
                }
                keepSycnhro = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WesternChanged();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int year;
            if (int.TryParse(textBox1.Text, out year))
            {
                AdjustDayCombo(comboBox2.SelectedIndex + 1, year);
            }
            WesternChanged();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int year;
            if (int.TryParse(textBox1.Text, out year))
            {
                if (textBox1.Text != prevYear)
                {
                    AdjustDayCombo(comboBox2.SelectedIndex + 1, year);
                    WesternChanged();
                    prevYear = textBox1.Text;
                }
            }
            else
            {
                textBox1.Text = prevYear;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            VedicChanged();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            VedicChanged();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int gyear;
            if (int.TryParse(textBox2.Text, out gyear))
            {
                if (textBox2.Text != prevGaurabda)
                {
                    VedicChanged();
                    prevGaurabda = textBox2.Text;
                }
            }
            else
            {
                textBox2.Text = prevGaurabda;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetToday();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            SetWesternDate(dt.Year + 1, 1, 1);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            SetWesternDate(dt.Year, 1, 1);
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetToday();
            int year;
            if (int.TryParse(textBox2.Text, out year))
            {
                keepSycnhro = false;
                textBox2.Text = (year + 1).ToString();
                comboBox3.SelectedIndex = 0;
                comboBox4.SelectedIndex = 1;
                keepSycnhro = true;
                comboBox4.SelectedIndex = 0;
            }
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SetToday();
            keepSycnhro = false;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 1;
            keepSycnhro = true;
            comboBox4.SelectedIndex = 0;
        }

        public void LoadInterfaceValues(string prefix)
        {
            DateTime dt = DateTime.Now;
            SetWesternDate(GPUserDefaults.IntForKey(prefix + ".gsd_year", dt.Year), GPUserDefaults.IntForKey(prefix + ".gsd_month", dt.Month), GPUserDefaults.IntForKey(prefix + ".gsd_day", dt.Day));
        }

        public void SaveInterfaceValues(string prefix)
        {
            int year;
            int.TryParse(textBox1.Text, out year);
            GPUserDefaults.SetIntForKey(prefix + ".gsd_year", year);
            GPUserDefaults.SetIntForKey(prefix + ".gsd_month", comboBox2.SelectedIndex + 1);
            GPUserDefaults.SetIntForKey(prefix + ".gsd_day", comboBox1.SelectedIndex + 1);
        }

    }
}

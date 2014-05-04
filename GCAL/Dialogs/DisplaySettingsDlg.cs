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
    public partial class DisplaySettingsDlg : Form
    {
        private List<TabPage> removedPages = new List<TabPage>();

        public DisplaySettingsDlg()
        {
            InitializeComponent();

            InitUserInterface();

            InitValues();
        }

        public DisplaySettingsDlg(params string[] tabsVisible)
        {
            InitializeComponent();

            InitUserInterface();

            InitValues();

            RemovePages(tabsVisible);
        }

        public void SetUserInterfaceStrings()
        {
            this.tabPage5.Text = GPStrings.getSharedStrings().getString(174);
            this.checkBox32.Text = GPStrings.getSharedStrings().getString(175);
            this.checkBox31.Text = GPStrings.getSharedStrings().getString(176);
            this.checkBox9.Text = GPStrings.getSharedStrings().getString(1006);
            this.checkBox8.Text = GPStrings.getSharedStrings().getString(991);
            this.checkBox7.Text = GPStrings.getSharedStrings().getString(177);
            this.checkBox6.Text = GPStrings.getSharedStrings().getString(178);
            this.checkBox5.Text = GPStrings.getSharedStrings().getString(179);
            this.checkBox4.Text = GPStrings.getSharedStrings().getString(180);
            this.checkBox3.Text = GPStrings.getSharedStrings().getString(181);
            this.checkBox2.Text = GPStrings.getSharedStrings().getString(182);
            this.tabPage1.Text = GPStrings.getSharedStrings().getString(45);
            this.label1.Text = GPStrings.getSharedStrings().getString(183);
            this.checkBox1.Text = GPStrings.getSharedStrings().getString(184);
            this.tabPage2.Text = GPStrings.getSharedStrings().getString(44);
            this.tabPage7.Text = GPStrings.getSharedStrings().getString(185);
            this.tabPage8.Text = GPStrings.getSharedStrings().getString(186);
            this.checkBox22.Text = GPStrings.getSharedStrings().getString(187);
            this.checkBox48.Text = GPStrings.getSharedStrings().getString(991);
            this.label2.Text = GPStrings.getSharedStrings().getString(188);// "Header Type";
            this.checkBox47.Text = GPStrings.getSharedStrings().getString(189);// "Paksa Info";
            this.label3.Text = GPStrings.getSharedStrings().getString(196);// "Anniversary Info";
            this.checkBox46.Text = GPStrings.getSharedStrings().getString(197);// "Fasting Flag";
            this.comboBox2.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(198),
            GPStrings.getSharedStrings().getString(199),
            GPStrings.getSharedStrings().getString(200)});
            this.checkBox45.Text = GPStrings.getSharedStrings().getString(190);// "Yoga Info";
            this.checkBox43.Text = GPStrings.getSharedStrings().getString(191);// "DST Change Info";
            this.checkBox44.Text = GPStrings.getSharedStrings().getString(192);//"Naksatra Info";
            this.checkBox34.Text = GPStrings.getSharedStrings().getString(193);// "Masa Start Info";
            this.checkBox30.Text = GPStrings.getSharedStrings().getString(194);// "Ekadashi Info";
            this.checkBox29.Text = GPStrings.getSharedStrings().getString(195);// "Sankranti Info";
            this.checkBox33.Text = GPStrings.getSharedStrings().getString(201);
            this.comboBox1.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(202),
            GPStrings.getSharedStrings().getString(203),
            GPStrings.getSharedStrings().getString(204)});
            this.checkBox49.Text = GPStrings.getSharedStrings().getString(205);
            this.tabPage9.Text = GPStrings.getSharedStrings().getString(206);
            this.checkBox16.Text = GPStrings.getSharedStrings().getString(207);
            this.checkBox17.Text = GPStrings.getSharedStrings().getString(208);
            this.checkBox18.Text = GPStrings.getSharedStrings().getString(51);
            this.checkBox19.Text = GPStrings.getSharedStrings().getString(52);
            this.checkBox20.Text = GPStrings.getSharedStrings().getString(53);
            this.checkBox21.Text = GPStrings.getSharedStrings().getString(54);
            this.checkBox23.Text = GPStrings.getSharedStrings().getString(209);
            this.checkBox24.Text = GPStrings.getSharedStrings().getString(210);
            this.checkBox25.Text = GPStrings.getSharedStrings().getString(100);
            this.checkBox26.Text = GPStrings.getSharedStrings().getString(101);
            this.checkBox27.Text = GPStrings.getSharedStrings().getString(102);
            this.checkBox42.Text = GPStrings.getSharedStrings().getString(181);
            this.checkBox28.Text = GPStrings.getSharedStrings().getString(103);
            this.tabPage3.Text = GPStrings.getSharedStrings().getString(46);
            this.checkBox15.Text = GPStrings.getSharedStrings().getString(211);
            this.checkBox14.Text = GPStrings.getSharedStrings().getString(999);
            this.checkBox13.Text = GPStrings.getSharedStrings().getString(1000);
            this.checkBox12.Text = GPStrings.getSharedStrings().getString(998);
            this.checkBox11.Text = GPStrings.getSharedStrings().getString(997);
            this.checkBox10.Text = GPStrings.getSharedStrings().getString(996);
            this.tabPage4.Text = GPStrings.getSharedStrings().getString(48);
            this.tabPage6.Text = GPStrings.getSharedStrings().getString(212);
            this.comboBox7.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(213),
            GPStrings.getSharedStrings().getString(214),
            GPStrings.getSharedStrings().getString(215),
            GPStrings.getSharedStrings().getString(216)});
            this.label8.Text = GPStrings.getSharedStrings().getString(217);
            this.comboBox6.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(218),
            GPStrings.getSharedStrings().getString(219),
            GPStrings.getSharedStrings().getString(220),
            GPStrings.getSharedStrings().getString(221)});
            this.label7.Text = GPStrings.getSharedStrings().getString(222);
            this.comboBox5.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(223),
            GPStrings.getSharedStrings().getString(224)});
            this.label6.Text = GPStrings.getSharedStrings().getString(225);
            this.label5.Text = GPStrings.getSharedStrings().getString(226);
            this.label4.Text = GPStrings.getSharedStrings().getString(227);
            this.comboBox3.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(228),
            GPStrings.getSharedStrings().getString(229),
            GPStrings.getSharedStrings().getString(230),
            GPStrings.getSharedStrings().getString(231)});
            this.button1.Text = GPStrings.getSharedStrings().getString(236);
            this.button2.Text = GPStrings.getSharedStrings().getString(237);
            this.label9.Text = GPStrings.getSharedStrings().getString(232);
            this.comboBox8.Items.AddRange(new object[] {
            GPStrings.getSharedStrings().getString(233),
            GPStrings.getSharedStrings().getString(234)});
            this.Text = GPStrings.getSharedStrings().getString(235);
    
        }



        private void RemovePages(string[] tabsVisible)
        {
            TabPage pageToRemove = tabControl1.TabPages[0];

            while (pageToRemove != null)
            {
                pageToRemove = null;
                foreach (TabPage page in tabControl1.TabPages)
                {
                    if (page.Tag is string)
                    {
                        if (!ContainsString(tabsVisible, page.Tag as string))
                        {
                            removedPages.Add(page);
                            pageToRemove = page;
                            break;
                        }
                    }
                }
                if (pageToRemove != null)
                    tabControl1.TabPages.Remove(pageToRemove);
            }
        }

        public bool ContainsString(string [] strings, string str)
        {
            foreach(string s in strings)
            {
                if (s == str)
                    return true;
            }
            return false;
        }

        private void InitUserInterface()
        {
            checkBox35.Text = GPEventClass.getName(0);
            checkBox36.Text = GPEventClass.getName(1);
            checkBox37.Text = GPEventClass.getName(2);
            checkBox38.Text = GPEventClass.getName(3);
            checkBox39.Text = GPEventClass.getName(4);
            checkBox40.Text = GPEventClass.getName(5);
            checkBox41.Text = GPEventClass.getName(6);

            for (int i = 0; i < 13; i++)
            {
                checkedListBox1.Items.Add(GPMasa.GetName(i), GPDisplays.MasaList.MasaVisible(i));
            }

            for (int i = 0; i < 7; i++)
            {
                comboBox4.Items.Add(GPStrings.getSharedStrings().getString(i));
            }
        }

        private void InitValues()
        {
            // appearance day
            checkBox1.Checked = GPDisplays.AppDay.childNameSuggestions();
            numericUpDown1.Value = GPDisplays.AppDay.celebrationCount();

            //today
            checkBox2.Checked = GPDisplays.Today.SunriseVisible();
            checkBox3.Checked = GPDisplays.Today.NoonVisible();
            checkBox4.Checked = GPDisplays.Today.SunsetVisible();
            checkBox5.Checked = GPDisplays.Today.SandhyaTimesVisible();
            checkBox6.Checked = GPDisplays.Today.SunriseInfo();
            checkBox7.Checked = GPDisplays.Today.BrahmaMuhurtaVisible();
            checkBox8.Checked = GPDisplays.Today.RasiOfMoonVisible();
            checkBox9.Checked = GPDisplays.Today.NaksatraPadaVisible();
            checkBox31.Checked = GPDisplays.Today.TithiList();
            checkBox32.Checked = GPDisplays.Today.NaksatraList();

            //core events
            checkBox10.Checked = GPDisplays.CoreEvents.Sunrise();
            checkBox11.Checked = GPDisplays.CoreEvents.Tithi();
            checkBox12.Checked = GPDisplays.CoreEvents.Naksatra();
            checkBox13.Checked = GPDisplays.CoreEvents.Sankranti();
            checkBox14.Checked = GPDisplays.CoreEvents.Conjunction();
            checkBox15.Checked = GPDisplays.CoreEvents.Sort();

            //calendar
            checkBox16.Checked = GPDisplays.Calendar.TithiArunodayaVisible();
            checkBox17.Checked = GPDisplays.Calendar.TimeArunodayaVisible();
            checkBox18.Checked = GPDisplays.Calendar.TimeSunriseVisible();
            checkBox19.Checked = GPDisplays.Calendar.TimeSunsetVisible();
            checkBox20.Checked = GPDisplays.Calendar.TimeMoonriseVisible();
            checkBox21.Checked = GPDisplays.Calendar.TimeMoonsetVisible();
            checkBox22.Checked = GPDisplays.Calendar.FestivalsVisible();
            checkBox23.Checked = GPDisplays.Calendar.KsayaTithiInfoVisible();
            checkBox24.Checked = GPDisplays.Calendar.VriddhiTithiInfoVisible();
            checkBox25.Checked = GPDisplays.Calendar.SunLongitudeVisible();
            checkBox26.Checked = GPDisplays.Calendar.MoonLongitudeVisible();
            checkBox27.Checked = GPDisplays.Calendar.AyanamsaValueVisible();
            checkBox28.Checked = GPDisplays.Calendar.JulianDayVisible();
            checkBox29.Checked = GPDisplays.Calendar.SankrantiInfoVisible();
            checkBox30.Checked = GPDisplays.Calendar.EkadasiInfoVisible();
            checkBox33.Checked = GPDisplays.Calendar.HideEmptyDays();
            checkBox34.Checked = GPDisplays.Calendar.StartMasaVisible();
            checkBox35.Checked = GPDisplays.Calendar.FestivalClass0();
            checkBox36.Checked = GPDisplays.Calendar.FestivalClass1();
            checkBox37.Checked = GPDisplays.Calendar.FestivalClass2();
            checkBox38.Checked = GPDisplays.Calendar.FestivalClass3();
            checkBox39.Checked = GPDisplays.Calendar.FestivalClass4();
            checkBox40.Checked = GPDisplays.Calendar.FestivalClass5();
            checkBox41.Checked = GPDisplays.Calendar.FestivalClass6();
            checkBox42.Checked = GPDisplays.Calendar.NoonTime();
            checkBox43.Checked = GPDisplays.Calendar.DSTNotice();
            checkBox44.Checked = GPDisplays.Calendar.NaksatraVisible();
            checkBox45.Checked = GPDisplays.Calendar.YogaVisible();
            checkBox46.Checked = GPDisplays.Calendar.FastingFlagVisible();
            checkBox47.Checked = GPDisplays.Calendar.PaksaInfoVisible();
            checkBox48.Checked = GPDisplays.Calendar.RasiVisible();
            checkBox49.Checked = GPDisplays.Calendar.EkadasiParanaDetails();
            comboBox1.SelectedIndex = GPDisplays.Calendar.AnniversaryType();
            comboBox2.SelectedIndex = GPDisplays.Calendar.MasaHeader() ? 1 : (GPDisplays.Calendar.MonthHeader() ? 2 : 0);

            // general
            comboBox3.SelectedIndex = GPDisplays.General.CaturmasyaSystem();
            comboBox4.SelectedIndex = GPDisplays.General.FirstDayOfWeek();
            comboBox5.SelectedIndex = GPDisplays.General.FastingNotation();
            comboBox6.SelectedIndex = GPDisplays.General.NameMasaFormat();
            comboBox7.SelectedIndex = GPDisplays.General.SankrantiNameFormat();
            comboBox8.SelectedIndex = (GPDisplays.General.TimeFormat24() ? 1 : 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // appearance day
            GPDisplays.AppDay.childNameSuggestions(checkBox1.Checked);
            GPDisplays.AppDay.celebrationCount(Convert.ToInt32(numericUpDown1.Value));

            //today
            GPDisplays.Today.SunriseVisible(checkBox2.Checked);
            GPDisplays.Today.NoonVisible(checkBox3.Checked);
            GPDisplays.Today.SunsetVisible(checkBox4.Checked);
            GPDisplays.Today.SandhyaTimesVisible(checkBox5.Checked);
            GPDisplays.Today.SunriseInfo(checkBox6.Checked);
            GPDisplays.Today.BrahmaMuhurtaVisible(checkBox7.Checked);
            GPDisplays.Today.RasiOfMoonVisible(checkBox8.Checked);
            GPDisplays.Today.NaksatraPadaVisible(checkBox9.Checked);
            GPDisplays.Today.TithiList(checkBox31.Checked);
            GPDisplays.Today.NaksatraList(checkBox32.Checked);

            // core events
            GPDisplays.CoreEvents.Sunrise(checkBox10.Checked);
            GPDisplays.CoreEvents.Tithi(checkBox11.Checked);
            GPDisplays.CoreEvents.Naksatra(checkBox12.Checked);
            GPDisplays.CoreEvents.Sankranti(checkBox13.Checked);
            GPDisplays.CoreEvents.Conjunction(checkBox14.Checked);
            GPDisplays.CoreEvents.Sort(checkBox15.Checked);

            // calendar
            GPDisplays.Calendar.TithiArunodayaVisible(checkBox16.Checked);
            GPDisplays.Calendar.TimeArunodayaVisible(checkBox17.Checked);
            GPDisplays.Calendar.TimeSunriseVisible(checkBox18.Checked);
            GPDisplays.Calendar.TimeSunsetVisible(checkBox19.Checked);
            GPDisplays.Calendar.TimeMoonriseVisible(checkBox20.Checked);
            GPDisplays.Calendar.TimeMoonsetVisible(checkBox21.Checked);
            GPDisplays.Calendar.FestivalsVisible(checkBox22.Checked);
            GPDisplays.Calendar.KsayaTithiInfoVisible(checkBox23.Checked);
            GPDisplays.Calendar.VriddhiTithiInfoVisible(checkBox24.Checked);
            GPDisplays.Calendar.SunLongitudeVisible(checkBox25.Checked);
            GPDisplays.Calendar.MoonLongitudeVisible(checkBox26.Checked);
            GPDisplays.Calendar.AyanamsaValueVisible(checkBox27.Checked);
            GPDisplays.Calendar.JulianDayVisible(checkBox28.Checked);
            GPDisplays.Calendar.SankrantiInfoVisible(checkBox29.Checked);
            GPDisplays.Calendar.EkadasiInfoVisible(checkBox30.Checked);
            GPDisplays.Calendar.MasaHeader((comboBox2.SelectedIndex == 1));
            GPDisplays.Calendar.MonthHeader((comboBox2.SelectedIndex == 2));
            GPDisplays.Calendar.HideEmptyDays(checkBox33.Checked);
            GPDisplays.Calendar.StartMasaVisible(checkBox34.Checked);
            GPDisplays.Calendar.FestivalClass0(checkBox35.Checked);
            GPDisplays.Calendar.FestivalClass1(checkBox36.Checked);
            GPDisplays.Calendar.FestivalClass2(checkBox37.Checked);
            GPDisplays.Calendar.FestivalClass3(checkBox38.Checked);
            GPDisplays.Calendar.FestivalClass4(checkBox39.Checked);
            GPDisplays.Calendar.FestivalClass5(checkBox40.Checked);
            GPDisplays.Calendar.FestivalClass6(checkBox41.Checked);
            GPDisplays.Calendar.NoonTime(checkBox42.Checked);
            GPDisplays.Calendar.DSTNotice(checkBox43.Checked);
            GPDisplays.Calendar.NaksatraVisible(checkBox44.Checked);
            GPDisplays.Calendar.YogaVisible(checkBox45.Checked);
            GPDisplays.Calendar.FastingFlagVisible(checkBox46.Checked);
            GPDisplays.Calendar.PaksaInfoVisible(checkBox47.Checked);
            GPDisplays.Calendar.RasiVisible(checkBox48.Checked);
            GPDisplays.Calendar.EkadasiParanaDetails(checkBox49.Checked);
            GPDisplays.Calendar.AnniversaryType(comboBox1.SelectedIndex);

            // general
            GPDisplays.General.CaturmasyaSystem(comboBox3.SelectedIndex);
            GPDisplays.General.FirstDayOfWeek(comboBox4.SelectedIndex);
            GPDisplays.General.FastingNotation(comboBox5.SelectedIndex);
            GPDisplays.General.NameMasaFormat(comboBox6.SelectedIndex);
            GPDisplays.General.SankrantiNameFormat(comboBox7.SelectedIndex);
            GPDisplays.General.TimeFormat24((comboBox8.SelectedIndex > 0));


            for (int i = 0; i < 13; i++)
            {
                GPDisplays.MasaList.SetMasaVisible(i, checkedListBox1.GetItemChecked(i));
            }

        }
    }
}

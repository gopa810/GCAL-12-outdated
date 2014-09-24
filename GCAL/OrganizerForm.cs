using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;

using GCAL.Base;
using GCAL.Dialogs;

namespace GCAL
{
    public partial class OrganizerForm : Form
    {
        public MainForm mainForm = null;
        private int editedStringIndex = -1;

        public OrganizerForm()
        {
            InitializeComponent();

            InitializeData();

            toolStripComboBox1.SelectedIndex = 0;
        }

        public void SetUserInterfaceStrings()
        {
            this.toolStripLabel1.Text = GPStrings.getString(170);
            this.toolStripComboBox1.Items.AddRange(new object[] {
            GPStrings.getString(171),
            GPStrings.getString(172)});
            this.tabPageCity.Text = GPStrings.getString(311);
            this.columnHeader1.Text = GPStrings.getString(251);
            this.columnHeader2.Text = GPStrings.getString(252);
            this.columnHeader3.Text = GPStrings.getString(10);
            this.columnHeader4.Text = GPStrings.getString(11);
            this.columnHeader5.Text = GPStrings.getString(12);
            this.tabPageCountry.Text = GPStrings.getString(312);
            this.columnHeader6.Text = GPStrings.getString(313);
            this.columnHeader7.Text = GPStrings.getString(252);
            this.tabPageEvent.Text = GPStrings.getString(984);
            this.tabPageTithi.Text = GPStrings.getString(314);
            this.columnHeader8.Text = GPStrings.getString(315);
            this.columnHeader9.Text = GPStrings.getString(13);
            this.columnHeader10.Text = GPStrings.getString(22);
            this.columnHeader11.Text = GPStrings.getString(20);
            this.columnHeader12.Text = GPStrings.getString(276);
            this.columnHeader13.Text = GPStrings.getString(275);
            this.columnHeader14.Text = GPStrings.getString(277);
            this.columnHeader15.Text = GPStrings.getString(305);
            this.label1.Text = GPStrings.getString(319);
            this.label2.Text = GPStrings.getString(320);
            this.tabPageRelative.Text = GPStrings.getString(317);
            this.columnHeader21.Text = GPStrings.getString(315);
            this.columnHeader23.Text = GPStrings.getString(282);
            this.columnHeader24.Text = GPStrings.getString(281);
            this.columnHeader25.Text = GPStrings.getString(276);
            this.columnHeader26.Text = GPStrings.getString(275);
            this.columnHeader27.Text = GPStrings.getString(277);
            this.columnHeader28.Text = GPStrings.getString(305);
            this.tabPageSankranti.Text = GPStrings.getString(316);
            this.columnHeader29.Text = GPStrings.getString(315);
            this.columnHeader30.Text = GPStrings.getString(318);
            this.columnHeader31.Text = GPStrings.getString(281);
            this.columnHeader33.Text = GPStrings.getString(276);
            this.columnHeader34.Text = GPStrings.getString(275);
            this.columnHeader35.Text = GPStrings.getString(277);
            this.columnHeader36.Text = GPStrings.getString(305);
            this.tabPageTimeZone.Text = GPStrings.getString(321);
            this.label4.Text = GPStrings.getString(322);
            this.columnHeader22.Text = GPStrings.getString(7);
            this.columnHeader32.Text = GPStrings.getString(325);
            this.columnHeader37.Text = GPStrings.getString(281);
            this.columnHeader38.Text = GPStrings.getString(323);
            this.columnHeader39.Text = GPStrings.getString(324);
            this.columnHeader16.Text = GPStrings.getString(12);
            this.columnHeader17.Text = GPStrings.getString(281);
            this.tabPageString.Text = GPStrings.getString(331);
            this.button1.Text = GPStrings.getString(335);
            this.label6.Text = GPStrings.getString(332);
            this.label5.Text = GPStrings.getString(333);
            this.columnHeader18.Text = GPStrings.getString(334);
            this.columnHeader19.Text = GPStrings.getString(333);
            this.label3.Text = GPStrings.getString(331);
            this.fileToolStripMenuItem.Text = GPStrings.getString(337);
            this.toolStripMenuItem1.Text = GPStrings.getString(361);
            this.newLocationToolStripMenuItem.Text = GPStrings.getString(344);
            this.editToolStripMenuItem.Text = GPStrings.getString(338);
            this.removeToolStripMenuItem.Text = GPStrings.getString(351);
            this.countriesToolStripMenuItem.Text = GPStrings.getString(312);
            this.newCountryToolStripMenuItem.Text = GPStrings.getString(345);
            this.editSelectedToolStripMenuItem.Text = GPStrings.getString(352);
            this.deleteSelectedToolStripMenuItem1.Text = GPStrings.getString(353);
            this.eventsToolStripMenuItem.Text = GPStrings.getString(984);
            this.newEventBasedOnTithiToolStripMenuItem.Text = GPStrings.getString(346);
            this.newEventBasedOnOtherEventToolStripMenuItem.Text = GPStrings.getString(347);
            this.newEventBasedOnSankrantiToolStripMenuItem.Text = GPStrings.getString(348);
            this.editToolStripMenuItem1.Text = GPStrings.getString(352);
            this.deleteSelectedToolStripMenuItem.Text = GPStrings.getString(353);
            this.timezonesToolStripMenuItem.Text = GPStrings.getString(321);
            this.newTimezoneToolStripMenuItem.Text = GPStrings.getString(349);
            this.editTimezoneNameToolStripMenuItem.Text = GPStrings.getString(354);
            this.deleteTimezoneToolStripMenuItem.Text = GPStrings.getString(355);
            this.newTimezoneTransitionToolStripMenuItem.Text = GPStrings.getString(350);
            this.editTimezoneTransitionToolStripMenuItem.Text = GPStrings.getString(356);
            this.deleteTimezoneTransitionToolStripMenuItem.Text = GPStrings.getString(357);
            this.languagesToolStripMenuItem.Text = GPStrings.getString(336);
            this.newToolStripMenuItem.Text = GPStrings.getString(340);
            this.importToolStripMenuItem.Text = GPStrings.getString(341);
            this.exportToolStripMenuItem.Text = GPStrings.getString(342);
            this.deleteToolStripMenuItem.Text = GPStrings.getString(343);
            this.Text = GPStrings.getString(360);
            
        }


        private void OrganizerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form form in fc)
            {
                if (form != this)
                {
                    form.BringToFront();
                }
            }
        }

        private void InitializeData()
        {
            comboBoxCountry.Items.Add("<all countries>");
            comboBoxCountry.SelectedIndex = 0;

            // cities
            GPLocationList locationList = GPLocationList.getShared();
            listView1.BeginUpdate();
            foreach (GPLocation location in locationList.locations)
            {
                ListViewItem lvi = new ListViewItem(location.getCity());
                lvi.SubItems.Add(location.getCountryCode());
                lvi.SubItems.Add(location.getLatitudeString());
                lvi.SubItems.Add(location.getLongitudeString());
                lvi.SubItems.Add(location.getTimeZoneName());
                lvi.Tag = location;
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();

            // countries
            GPCountryList countryList = GPCountryList.getShared();
            listView2.BeginUpdate();
            comboBoxCountry.BeginUpdate();
            foreach (GPCountry country in countryList.countries)
            {
                ListViewItem lvi = new ListViewItem(country.getCode());
                lvi.SubItems.Add(country.getName());
                lvi.Tag = country;
                listView2.Items.Add(lvi);

                // combo box in cities tab
                comboBoxCountry.Items.Add(country);
            }
            listView2.EndUpdate();
            comboBoxCountry.EndUpdate();



            //strings
            GPLanguageList langs = GPLanguageList.getShared();
            foreach (GPLanguage lang in langs.languages)
            {
                comboBox3.Items.Add(lang);
            }

            comboBox3.SelectedIndex = 0;

            // comboboxes of filter
            comboBox1.Items.Add(GPStrings.getString(915));
            for (int i = 0; i < GPEventClass.count(); i++)
            {
                comboBox1.Items.Add(new GPEventClass(i));
            }

            comboBox2.Items.Add(GPStrings.getString(915));
            for (int i = 0; i < GPMasa.getCount(); i++)
            {
                comboBox2.Items.Add(new GPMasa(i));
            }

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTimezoneTransitionsListView();
        }

        private void comboBoxCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = comboBoxCountry.SelectedIndex;
            if (selected >= 0 && selected < comboBoxCountry.Items.Count)
            {
                IComparer comp = listView1.ListViewItemSorter;
                listView1.ListViewItemSorter = null;
                GPLocationList locationList = GPLocationList.getShared();
                GPCountry country = null;
                if (comboBoxCountry.Items[selected] is GPCountry)
                {
                    country = comboBoxCountry.Items[selected] as GPCountry;
                }
                listView1.BeginUpdate();
                listView1.Items.Clear();
                foreach (GPLocation location in locationList.locations)
                {
                    if (country == null || location.getCountryCode() == country.getCode())
                    {
                        ListViewItem lvi = new ListViewItem(location.getCity());
                        lvi.SubItems.Add(location.getCountryCode());
                        lvi.SubItems.Add(location.getLatitudeString());
                        lvi.SubItems.Add(location.getLongitudeString());
                        lvi.SubItems.Add(location.getTimeZoneName());
                        lvi.Tag = location;
                        listView1.Items.Add(lvi);
                    }
                } 
                listView1.EndUpdate();
                listView1.ListViewItemSorter = comp;
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                GPLocation loc = listView1.SelectedItems[0].Tag as GPLocation;
                if (MessageBox.Show("Do you want to remove location '" + loc.getCity() + "'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    listView1.Items.Remove(listView1.SelectedItems[0]);
                }
            }
        }

        private void EditSelectedLocation()
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView1.SelectedItems[0];
                GPLocation loc = lvi.Tag as GPLocation;

                if (loc == null)
                    return;

                EditLocationProperties dlg = new EditLocationProperties();

                dlg.LocationName = loc.getCity();
                dlg.Longitude = loc.getLongitudeString();
                dlg.Latitude = loc.getLatitudeString();
                dlg.Country = GPCountryList.getShared().GetCountryByCode(loc.getCountryCode());
                dlg.TimeZone = GPTimeZoneList.sharedTimeZones().GetTimezoneByName(loc.getTimeZoneName());

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    loc.setCity(dlg.LocationName);
                    loc.setLongitudeString(dlg.Longitude);
                    loc.setLatitudeString(dlg.Latitude);
                    loc.setCountryCode(dlg.Country.getCode());
                    loc.setTimeZoneName(dlg.TimeZone.Name);

                    lvi.SubItems[0].Text = loc.getCity();
                    lvi.SubItems[1].Text = loc.getCountryCode();
                    lvi.SubItems[2].Text = loc.getLongitudeString();
                    lvi.SubItems[3].Text = loc.getLatitudeString();
                    lvi.SubItems[4].Text = loc.getTimeZoneName();

                    GPLocationList.getShared().Modified = true;
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedLocation();
        }

        private void newLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditLocationProperties dlg = new EditLocationProperties();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GPLocation loc = new GPLocation();
                loc.setCity(dlg.LocationName);
                loc.setLongitudeString(dlg.Longitude);
                loc.setLatitudeString(dlg.Latitude);
                loc.setCountryCode(dlg.Country.getCode());
                loc.setTimeZoneName(dlg.TimeZone.Name);

                ListViewItem lvi = new ListViewItem(loc.getCity());
                lvi.SubItems.Add(loc.getCountryCode());
                lvi.SubItems.Add(loc.getLongitudeString());
                lvi.SubItems.Add(loc.getLatitudeString());
                lvi.SubItems.Add(loc.getTimeZoneName());

                listView1.Items.Add(lvi);

                GPLocationList.getShared().locations.Add(loc);
                GPLocationList.getShared().Modified = true;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSelectedLocation();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sender is ListView)
            {
                (sender as ListView).ListViewItemSorter = new ListItemsComparer(e.Column);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTithiBasedEvents();
        }

        private void RefreshTithiBasedEvents()
        {
            int groupIndex = comboBox1.SelectedIndex;
            int masaIndex = comboBox2.SelectedIndex;

            if (groupIndex < 0 || masaIndex < 0)
                return;

            GPEventClass group = ((groupIndex > 0) ? comboBox1.Items[groupIndex] as GPEventClass : null);
            GPMasa masa = ((masaIndex > 0) ? comboBox2.Items[masaIndex] as GPMasa : null);

            GPEventList eventList = GPEventList.getShared();
            listView3.BeginUpdate();
            listView3.Items.Clear();
            foreach (GPEventTithi eve in eventList.tithiEvents)
            {
                if (masa != null && eve.nMasa != masa.getMasa())
                    continue;
                if (group != null && eve.nClass != group.group)
                    continue;
                ListViewItem lvi = new ListViewItem(eve.strText);
                lvi.SubItems.Add(GPTithi.getName(eve.nTithi));
                lvi.SubItems.Add(GPMasa.GetName(eve.nMasa));
                lvi.SubItems.Add(GPPaksa.getName(eve.nTithi / 15));
                lvi.SubItems.Add(GPFastType.getName(eve.getRawFastType()));
                lvi.SubItems.Add(eve.strFastSubject);
                lvi.SubItems.Add(GPEventClass.getName(eve.nClass));
                lvi.SubItems.Add(eve.nStartYear < -9999 ? "" : eve.nStartYear.ToString());
                lvi.Tag = eve;
                listView3.Items.Add(lvi);
            }
            listView3.EndUpdate();
        }


        private void RefreshRelativeEvents()
        {
            GPEventList eventList = GPEventList.getShared();
            listView6.BeginUpdate();
            listView6.Items.Clear();
            foreach (GPEventRelative eve in eventList.relativeEvents)
            {
                ListViewItem lvi = new ListViewItem(eve.strText);
                lvi.SubItems.Add(eventList.GetSpecialEvent(eve.nSpecRef).strText);
                lvi.SubItems.Add(eve.nOffset.ToString());
                lvi.SubItems.Add(GPFastType.getName(eve.getRawFastType()));
                lvi.SubItems.Add(eve.strFastSubject);
                lvi.SubItems.Add(GPEventClass.getName(eve.nClass));
                lvi.SubItems.Add(eve.nStartYear < -9999 ? "" : eve.nStartYear.ToString());
                lvi.Tag = eve;
                listView6.Items.Add(lvi);
            }
            listView6.EndUpdate();
        }

        private void RefreshSankrantiBasedEvents()
        {
            GPEventList eventList = GPEventList.getShared();
            listView7.BeginUpdate();
            listView7.Items.Clear();
            foreach (GPEventSankranti eve in eventList.sankrantiEvents)
            {
                ListViewItem lvi = new ListViewItem(eve.strText);
                lvi.SubItems.Add(GPSankranti.getName(eve.nSankranti));
                lvi.SubItems.Add(eve.nOffset.ToString());
                lvi.SubItems.Add(GPFastType.getName(eve.getRawFastType()));
                lvi.SubItems.Add(eve.strFastSubject);
                lvi.SubItems.Add(GPEventClass.getName(eve.nClass));
                lvi.SubItems.Add(eve.nStartYear < -9999 ? "" : eve.nStartYear.ToString());
                lvi.Tag = eve;
                listView7.Items.Add(lvi);
            }
            listView7.EndUpdate();
        }

        private void newEventBasedOnTithiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageEvent;
            tabControl2.SelectedTab = tabPageTithi;

            EditTithiEvent dlg = new EditTithiEvent();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                GPEventTithi eve = new GPEventTithi();

                eve.strText = dlg.EventTitle;
                eve.nClass = dlg.Group;
                eve.setRawFastType(dlg.FastType);
                eve.nMasa = dlg.Masa;
                eve.nStartYear = -10000;
                int.TryParse(dlg.SinceYear, out eve.nStartYear);
                eve.nTithi = dlg.Tithi;
                eve.nUsed = 1;
                eve.nVisible = dlg.EventVisible ? 1 : 0;
                eve.strFastSubject = dlg.FastSubject;

                GPEventList.getShared().tithiEvents.Add(eve);
                GPEventList.getShared().Modified = true;

                RefreshTithiBasedEvents();
            }
        }

        private void newEventBasedOnOtherEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageEvent;
            tabControl2.SelectedTab = tabPageRelative;

            EditRelativeEventDlg dlg = new EditRelativeEventDlg();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                GPEventRelative eve = new GPEventRelative();

                eve.strText = dlg.EventTitle;
                eve.nClass = dlg.Group;
                eve.setRawFastType(dlg.FastType);
                eve.nOffset = dlg.Offset;
                eve.nStartYear = -10000;
                int.TryParse(dlg.SinceYear, out eve.nStartYear);
                eve.nSpecRef = dlg.RefSpec;
                eve.nUsed = 1;
                eve.nVisible = dlg.EventVisible ? 1 : 0;
                eve.strFastSubject = dlg.FastSubject;

                GPEventList.getShared().relativeEvents.Add(eve);
                GPEventList.getShared().Modified = true;

                RefreshRelativeEvents();
            }
        }

        private void newEventBasedOnSankrantiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageEvent;
            tabControl2.SelectedTab = tabPageSankranti;
            EditSankrantiBasedEventDlg dlg = new EditSankrantiBasedEventDlg();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                GPEventSankranti eve = new GPEventSankranti();

                eve.strText = dlg.EventTitle;
                eve.nClass = dlg.Group;
                eve.setRawFastType(dlg.FastType);
                eve.nOffset = dlg.Offset;
                eve.nStartYear = -10000;
                int.TryParse(dlg.SinceYear, out eve.nStartYear);
                eve.nSankranti = dlg.Sankranti;
                eve.nUsed = 1;
                eve.nVisible = dlg.EventVisible ? 1 : 0;
                eve.strFastSubject = dlg.FastSubject;

                GPEventList.getShared().sankrantiEvents.Add(eve);
                GPEventList.getShared().Modified = true;

                RefreshSankrantiBasedEvents();
            }
        }

        public ListView GetSelectedEventListView()
        {
            if (tabControl2.SelectedTab == tabPageTithi)
            {
                return listView3;
            }
            else if (tabControl2.SelectedTab == tabPageRelative)
            {
                return listView6;
            }
            else if (tabControl2.SelectedTab == tabPageSankranti)
            {
                return listView7;
            }

            return null;
        }

        private ListViewItem GetSelectedListViewItem(ListView lv)
        {
            if (lv == null)
                return null;

            if (lv.SelectedItems.Count > 0)
            {
                return lv.SelectedItems[0];
            }

            return null;
        }

        private void EditCurrentEvent()
        {
            ListView selectedList = GetSelectedEventListView();
            ListViewItem selectedItem = GetSelectedListViewItem(selectedList);

            if (tabControl2.SelectedTab == tabPageTithi)
            {
                EditTithiBasedEvent(selectedItem, selectedItem.Tag as GPEventTithi);
            }
            else if (tabControl2.SelectedTab == tabPageRelative)
            {
                EditRelativeEvent(selectedItem, selectedItem.Tag as GPEventRelative);
            }
            else if (tabControl2.SelectedTab == tabPageSankranti)
            {
                EditSankrantiBasedEvent(selectedItem, selectedItem.Tag as GPEventSankranti);
            }
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditCurrentEvent();
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView selectedList = GetSelectedEventListView();
            ListViewItem selectedItem = GetSelectedListViewItem(selectedList);

            if (selectedItem != null)
            {
                if (MessageBox.Show("Do you want to delete event '" + selectedItem.Text + "' ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    GPEventList.getShared().RemoveEvent(selectedItem.Tag);
                    selectedList.Items.Remove(selectedItem);
                    RefreshCurrentEvents();
                }
            }

        }

        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            EditCurrentEvent();
        }

        public void RefreshCurrentEvents()
        {
            if (tabControl2.SelectedTab == tabPageTithi)
            {
                RefreshTithiBasedEvents();
            }
            else if (tabControl2.SelectedTab == tabPageRelative)
            {
                RefreshRelativeEvents();
            }
            else if (tabControl2.SelectedTab == tabPageSankranti)
            {
                RefreshSankrantiBasedEvents();
            }
        }

        public void EditTithiBasedEvent(ListViewItem lvi, GPEventTithi eve)
        {
            EditTithiEvent dlg = new EditTithiEvent();
            dlg.EventTitle = eve.strText;
            dlg.Group = eve.nClass;
            dlg.FastType = eve.getRawFastType();
            dlg.Masa = eve.nMasa;
            dlg.SinceYear = ((eve.nStartYear < -9999) ? "" : eve.nStartYear.ToString());
            dlg.Tithi = eve.nTithi;
            dlg.EventVisible = (eve.nVisible != 0);
            dlg.FastSubject = eve.strFastSubject;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                eve.strText = dlg.EventTitle;
                eve.nClass = dlg.Group;
                eve.setRawFastType(dlg.FastType);
                eve.nMasa = dlg.Masa;
                eve.nStartYear = -10000;
                int.TryParse(dlg.SinceYear, out eve.nStartYear);
                eve.nTithi = dlg.Tithi;
                eve.nVisible = dlg.EventVisible ? 1 : 0;
                eve.strFastSubject = dlg.FastSubject;

                GPEventList.getShared().Modified = true;

                RefreshTithiBasedEvents();
            }
        }

        public void EditRelativeEvent(ListViewItem lvi, GPEventRelative eve)
        {
            EditRelativeEventDlg dlg = new EditRelativeEventDlg();

            dlg.EventTitle = eve.strText;
            dlg.Group = eve.nClass;
            dlg.FastType = eve.getRawFastType();
            dlg.Offset = eve.nOffset;
            dlg.SinceYear = ((eve.nStartYear < -9999) ? "" : eve.nStartYear.ToString());
            dlg.RefSpec = eve.nSpecRef;
            dlg.EventVisible = (eve.nVisible != 0);
            dlg.FastSubject = eve.strFastSubject;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                eve.strText = dlg.EventTitle;
                eve.nClass = dlg.Group;
                eve.setRawFastType(dlg.FastType);
                eve.nOffset = dlg.Offset;
                eve.nStartYear = -10000;
                int.TryParse(dlg.SinceYear, out eve.nStartYear);
                eve.nSpecRef = dlg.RefSpec;
                eve.nUsed = 1;
                eve.nVisible = dlg.EventVisible ? 1 : 0;
                eve.strFastSubject = dlg.FastSubject;

                GPEventList.getShared().Modified = true;

                RefreshRelativeEvents();
            }
        }

        public void EditSankrantiBasedEvent(ListViewItem lvi, GPEventSankranti eve)
        {
            EditSankrantiBasedEventDlg dlg = new EditSankrantiBasedEventDlg();

            dlg.EventTitle = eve.strText;
            dlg.Group = eve.nClass;
            dlg.FastType = eve.getRawFastType();
            dlg.Offset = eve.nOffset;
            dlg.SinceYear = ((eve.nStartYear < -9999) ? "" : eve.nStartYear.ToString());
            dlg.Sankranti = eve.nSankranti;
            dlg.EventVisible = (eve.nVisible != 0);
            dlg.FastSubject = eve.strFastSubject;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                eve.strText = dlg.EventTitle;
                eve.nClass = dlg.Group;
                eve.setRawFastType(dlg.FastType);
                eve.nOffset = dlg.Offset;
                eve.nStartYear = -10000;
                int.TryParse(dlg.SinceYear, out eve.nStartYear);
                eve.nSankranti = dlg.Sankranti;
                eve.nUsed = 1;
                eve.nVisible = dlg.EventVisible ? 1 : 0;
                eve.strFastSubject = dlg.FastSubject;

                GPEventList.getShared().Modified = true;

                RefreshSankrantiBasedEvents();
            }
        }

        private void listView3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditCurrentEvent();
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSelectedCountry();
        }

        private void EditSelectedCountry()
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView2.SelectedItems[0];
                if (lvi.Tag is GPCountry)
                {
                    GPCountry country = lvi.Tag as GPCountry;

                    EditCountryDlg dlg = new EditCountryDlg();

                    dlg.ValidCode = country.getCode();
                    dlg.CountryCode = country.getCode();
                    dlg.CountryName = country.getName();

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (!country.getCode().Equals(dlg.CountryCode))
                        {
                            GPLocationList.getShared().ChangeCountryCode(country.getCode(), dlg.CountryCode);
                        }

                        country.setCode(dlg.CountryCode);
                        country.setName(dlg.CountryName);

                        lvi.SubItems[0].Text = country.getCode();
                        lvi.SubItems[1].Text = country.getName();

                        GPCountryList.getShared().Modified = true;
                    }
                }
            }
        }

        private void newCountryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCountryDlg dlg = new EditCountryDlg();
            GPCountry country = new GPCountry();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                country.setCode(dlg.CountryCode);
                country.setName(dlg.CountryName);

                ListViewItem lvi = new ListViewItem(country.getCode());
                lvi.SubItems.Add(country.getName());

                GPCountryList.getShared().countries.Add(country);
                GPCountryList.getShared().Modified = true;
            }

        }

        private void editSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSelectedCountry();
        }

        private void deleteSelectedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView2.SelectedItems[0];
                if (lvi.Tag is GPCountry)
                {
                    GPCountry country = lvi.Tag as GPCountry;

                    int count = GPLocationList.getShared().GetLocationCountForCountryCode(country.getCode());

                    if (count > 0)
                    {
                        MessageBox.Show("You cannot delete country '" + country.getName() + "', because there are " 
                            + count.ToString() + " locations associated with it.", "Cannot delete", 
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        if (MessageBox.Show("Do you want to delete country '" + country.getName() + "' ?",
                            "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            GPCountryList.getShared().countries.Remove(country);
                            GPCountryList.getShared().Modified = true;
                            listView2.Items.Remove(lvi);
                        }
                    }
                }
            }

        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView2.ListViewItemSorter = new ListItemsComparer(e.Column);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageEvent && listView3.Items.Count == 0)
            {
                // events
                GPEventList eventList = GPEventList.getShared();
                listView3.BeginUpdate();
                foreach (GPEventTithi eve in eventList.tithiEvents)
                {
                    ListViewItem lvi = new ListViewItem(eve.strText);
                    lvi.SubItems.Add(GPTithi.getName(eve.nTithi));
                    lvi.SubItems.Add(GPMasa.GetName(eve.nMasa));
                    lvi.SubItems.Add(GPPaksa.getName(eve.nTithi / 15));
                    lvi.SubItems.Add(GPFastType.getName(eve.getRawFastType()));
                    lvi.SubItems.Add(eve.strFastSubject);
                    lvi.SubItems.Add(GPEventClass.getName(eve.nClass));
                    lvi.SubItems.Add(eve.nStartYear < -9999 ? "" : eve.nStartYear.ToString());
                    lvi.Tag = eve;
                    listView3.Items.Add(lvi);
                }
                listView3.EndUpdate();

                RefreshRelativeEvents();

                RefreshSankrantiBasedEvents();
            }

            if (tabControl1.SelectedTab == tabPageTimeZone && listView4.Items.Count == 0)
            {
                // timezones
                RefreshTimezoneListView();
            }

            SearchTextInListviews();
        }

        private void RefreshTimezoneListView()
        {
            GPTimeZoneList zones = GPTimeZoneList.sharedTimeZones();
            listView4.BeginUpdate();
            listView4.Items.Clear();
            listView5.Items.Clear();
            foreach (GPTimeZone timezone in zones.getTimeZones())
            {
                ListViewItem lvi = new ListViewItem(timezone.Name);
                lvi.SubItems.Add(timezone.getOffsetString());
                lvi.Tag = timezone;
                listView4.Items.Add(lvi);
            }
            listView4.EndUpdate();
        }

        private void newTimezoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTimezoneNameDlg dlg = new EditTimezoneNameDlg();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GPTimeZone tzone = new GPTimeZone();

                tzone.Name = dlg.TimezoneName;
                tzone.OffsetSeconds = dlg.TimezoneOffset * 60;

                GPTimeZoneList.sharedTimeZones().getTimeZones().Add(tzone);
                GPTimeZoneList.sharedTimeZones().Modified = true;

                RefreshTimezoneListView();
            }
        }

        private void EditTimezoneName()
        {
            ListViewItem lvi = GetSelectedListViewItem(listView4);
            if (lvi != null)
            {
                if (lvi.Tag is GPTimeZone)
                {
                    GPTimeZone tz = lvi.Tag as GPTimeZone;
                    EditTimezoneNameDlg dlg = new EditTimezoneNameDlg();

                    dlg.TimezoneName = tz.Name;
                    dlg.TimezoneOffset = Convert.ToInt32(tz.OffsetSeconds / 60);

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        GPTimeZone tzone = new GPTimeZone();

                        tzone.Name = dlg.TimezoneName;
                        tzone.OffsetSeconds = dlg.TimezoneOffset * 60;

                        GPTimeZoneList.sharedTimeZones().Modified = true;

                        RefreshTimezoneListView();
                    }
                }
            }
        }

        private void editTimezoneNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTimezoneName();
        }

        private void deleteTimezoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvi = GetSelectedListViewItem(listView4);
            if (lvi != null)
            {
                if (lvi.Tag is GPTimeZone)
                {
                    GPTimeZone tz = lvi.Tag as GPTimeZone;

                    int count = GPLocationList.getShared().GetLocationCountForTimezone(tz.Name);
                    if (count > 0)
                    {
                        MessageBox.Show("You cannot delete timezone '" + tz.Name + "', because there are " + count.ToString() + " locations associated with it.", "Cannot delete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        if (MessageBox.Show("Do you want to delete timezone '" + tz.Name + "' ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            GPTimeZoneList.sharedTimeZones().getTimeZones().Remove(tz);
                            GPTimeZoneList.sharedTimeZones().Modified = true;
                            listView4.Items.Remove(lvi);
                            listView5.Items.Clear();
                        }
                    }
                }
            }
        }

        private void newTimezoneTransitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvt = GetSelectedListViewItem(listView4);
            ListViewItem lvi = GetSelectedListViewItem(listView8);
            if (lvi != null && lvt != null)
            {
                if (lvi.Tag is GPTimeZone.Transition && lvt.Tag is GPTimeZone)
                {
                    GPTimeZone tzone = lvt.Tag as GPTimeZone;
                    GPTimeZone.Transition tz = lvi.Tag as GPTimeZone.Transition;
                    EditTimezoneTransitionDlg dlg = new EditTimezoneTransitionDlg();

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        tz.OffsetInSeconds = dlg.TransOffsetSeconds;
                        tzone.AddTransition(tz);

                        RefreshTimezoneTransitionsListView();
                        GPTimeZoneList.sharedTimeZones().Modified = true;
                    }
                }
            }
        }

        private void editTimezoneTransitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTimezoneTransition();
        }

        private void deleteTimezoneTransitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteTimezoneTransition();

        }

        private void DeleteTimezoneTransition()
        {
            ListViewItem lvt = GetSelectedListViewItem(listView4);
            ListViewItem lvi = GetSelectedListViewItem(listView8);
            if (lvi != null && lvt != null)
            {
                if (lvi.Tag is GPTimeZone.Transition && lvt.Tag is GPTimeZone)
                {
                    GPTimeZone tzone = lvt.Tag as GPTimeZone;
                    GPTimeZone.Transition tz = lvi.Tag as GPTimeZone.Transition;
                    GPTimestamp tstamp = new GPTimestamp(0);
                    string message = "Do you want to delete transition which occurs on " + tstamp.getDateTime().ToShortDateString() + " for timezone " + tzone.Name + "?";
                    if (MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        tzone.Transitions.Remove(tz);
                        RefreshTimezoneTransitionsListView();
                    }
                }
            }
        }

        private void EditTimezoneTransition()
        {
            ListViewItem lvt = GetSelectedListViewItem(listView4);
            ListViewItem lvi = GetSelectedListViewItem(listView8);
            if (lvi != null && lvt != null)
            {
                if (lvi.Tag is GPTimeZone.Transition && lvt.Tag is GPTimeZone)
                {
                    GPTimeZone tzone = lvt.Tag as GPTimeZone;
                    GPTimeZone.Transition tz = lvi.Tag as GPTimeZone.Transition;
                    EditTimezoneTransitionDlg dlg = new EditTimezoneTransitionDlg();

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        tz.OffsetInSeconds = dlg.TransOffsetSeconds;
                        tzone.Transitions.Remove(tz);
                        tzone.AddTransition(tz);

                        RefreshTimezoneTransitionsListView();
                        GPTimeZoneList.sharedTimeZones().Modified = true;
                    }
                }
            }
        }

        private void RefreshTimezoneTransitionsListView()
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView4.SelectedItems[0];
                GPTimeZone tzone = lvi.Tag as GPTimeZone;
                if (tzone != null)
                {
                    listView8.BeginUpdate();
                    listView8.Items.Clear();
                    foreach (GPTimeZone.Transition trans in tzone.Transitions)
                    {
                        long y, m, d, h, min, sec;
                        sec = tzone.OffsetSeconds;
                        min = sec / 60;
                        sec = sec - min * 60;
                        h = min / 60;
                        min = min - h * 60;
                        d = h / 24;
                        h = h - d * 24;
                        m = d / 32;
                        d = d - m * 32;
                        y = m / 12;
                        m = m - y * 12;
                        ListViewItem lvin = new ListViewItem(string.Format("{0}-{1:00}-{2:00}", y, m, d));
                        lvin.SubItems.Add(string.Format("{0:00}:{1:00}", h, min));
                        lvin.SubItems.Add(trans.getOffsetString());
                        lvin.Tag = trans;
                        listView8.Items.Add(lvin);
                    }
                    listView8.EndUpdate();
                }
            }
        }

        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditTimezoneName();
        }

        private void listView8_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditTimezoneTransition();
        }

        private void listView8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                DeleteTimezoneTransition();
            }
        }

        private void RefreshStringsListView()
        {
            listView5.BeginUpdate();
            listView5.Items.Clear();
            //List<string> gstr = GPStrings.getSharedStrings().gstr;
            for (int i = 0; i < GPStrings.getCount(); i++)
            {
                string s = GPStrings.getString(i);
                if (s.Length > 0)
                {
                    ListViewItem lvi = new ListViewItem(i.ToString());
                    lvi.SubItems.Add(s);
                    lvi.Tag = i;
                    listView5.Items.Add(lvi);
                }
            }
            listView5.EndUpdate();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshStringsListView();
            if (comboBox3.SelectedItem is GPLanguage)
            {
                GPLanguage lang = comboBox3.SelectedItem as GPLanguage;
                if (GPLanguageList.getDefaultLanguage() == lang)
                {
                    textBox1.ReadOnly = true;
                }
                else
                {
                    textBox1.ReadOnly = false;
                }
            }
        }

        private void listView5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView5.SelectedItems.Count > 0)
            {
                ListViewItem lvi = listView5.SelectedItems[0];
                int.TryParse(lvi.Text, out editedStringIndex);
            }

            if (editedStringIndex >= 0)
            {
                textBox1.Text = GPStrings.getString(editedStringIndex);
                textBox2.Text = GPLanguageList.getDefaultLanguage().getStrings().getStringValue(editedStringIndex);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (editedStringIndex >= 0)
            {
                GPStrings.getSharedStrings().setString(editedStringIndex, textBox1.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to create new language from current one?", "Ask", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                EditLanguageNameDlg dlg = new EditLanguageNameDlg();

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    GPLanguage newLang = new GPLanguage();

                    newLang.LanguageName = dlg.LangName;
                    newLang.LanguageFile = GPFileHelper.UniqueFile(GPFileHelper.getLanguageDirectory(), "lang", ".txt");
                    newLang.setStrings(GPLanguageList.getCurrentLanguage().getStrings());
                    GPLanguageList.getShared().languages.Add(newLang);

                    comboBox3.Items.Add(newLang);
                    comboBox3.SelectedItem = newLang;
                }
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GPLanguage newLang = new GPLanguage();

                if (newLang.loadFile(ofd.FileName))
                {
                    GPLanguageList.getShared().languages.Add(newLang);
                    newLang.LanguageFile = GPFileHelper.UniqueFile(GPFileHelper.getLanguageDirectory(), "lang", ".txt");
                    newLang.setModified(true);

                    comboBox3.Items.Add(newLang);
                    comboBox3.SelectedItem = newLang;
                }
                else
                {
                    MessageBox.Show("Selected file does not contain valid GCAL language information.", "A Problem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    GPLanguageList.getCurrentLanguage().saveFile(sfd.FileName);
                }
                catch
                {
                    MessageBox.Show("Error during exporting language information.", "A Problem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GPStrings.getSharedStrings() != GPLanguageList.getDefaultLanguage().getStrings())
            {
                if (MessageBox.Show("Do you want to delete language '" + GPLanguageList.getCurrentLanguage().LanguageName + "' ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    GPLanguage removeLang = GPLanguageList.getCurrentLanguage();
                    GPLanguageList.setCurrentLanguage(GPLanguageList.getDefaultLanguage());
                    GPLanguageList.getShared().languages.Remove(removeLang);
                    File.Delete(removeLang.LanguageFile);
                }
            }
        }

        private ListView GetVisibleListBox(TabControl tabCtrl)
        {
            foreach (Control ctrl in tabCtrl.SelectedTab.Controls)
            {
                if (ctrl is ListView)
                {
                    if (ctrl.Tag is string && ((ctrl.Tag as string) == "no_search"))
                        continue;
                    return ctrl as ListView;
                }
                else if (ctrl is TabControl)
                {
                    return GetVisibleListBox(ctrl as TabControl);
                }
            }
            return null;
        }

        private void SearchTextInListviews()
        {
            string searchText = toolStripTextBox1.Text.ToLower();
            int scope = toolStripComboBox1.SelectedIndex;
            ListView listBox = GetVisibleListBox(tabControl1);
            listBox.SelectedItems.Clear();
            /*foreach (ListViewItem lvi in listBox.Items)
            {
                lvi.ForeColor = Color.Black;
            }*/
            if (searchText.Length == 0)
                return;

            int i = 0;
            bool found = false;
            foreach (ListViewItem lvi in listBox.Items)
            {
                i = 0;
                foreach (ListViewItem.ListViewSubItem subItem in lvi.SubItems)
                {
                    if (scope == 1 && i > 0)
                        break;
                    if (subItem.Text.ToLower().IndexOf(searchText) >= 0)
                    {
                        listBox.EnsureVisible(lvi.Index);
                        //subItem.ForeColor = Color.Red;
                        lvi.Selected = true;
                        found = true;
                        break;
                    }
                    i++;
                }
                if (found) break;
            }

            if (!found)
            {
                listBox.SelectedItems.Clear();
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            SearchTextInListviews();
        }

    }
}

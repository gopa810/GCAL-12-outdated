using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using GCAL.Base;
using GCAL.Engine;
using GCAL.Dialogs;

namespace GCAL.Controls
{
    public partial class LocationPickerControl : UserControl, CELAsyncResultReceiver, WizardChildDelegate
    {
        private string initPrefix = string.Empty;
        private bool supressLocationChangedEvent = false;
        private string findText = string.Empty;
        private bool findTextChanged = false;
        private bool supressTextChange = false;
        private bool modifiedFlagAdded = false;
        private CELFindCity findOperation = null;
        private WizardDialogDelegate wizardDelegate = null;
        private bool travellingChanged = true;

        public void setParent(WizardDialogDelegate aDelegate)
        {
            wizardDelegate = aDelegate;
        }

        public void SetUserInterfaceStrings()
        {
            this.tabPage1.Text = GPStrings.getSharedStrings().getString(249);
            this.label1.Text = GPStrings.getSharedStrings().getString(250);
            this.columnHeader1.Text = GPStrings.getSharedStrings().getString(251);
            this.columnHeader2.Text = GPStrings.getSharedStrings().getString(253);
            this.columnHeader3.Text = GPStrings.getSharedStrings().getString(10);
            this.columnHeader4.Text = GPStrings.getSharedStrings().getString(11);
            this.columnHeader5.Text = GPStrings.getSharedStrings().getString(12);
            this.tabPage2.Text = GPStrings.getSharedStrings().getString(256);
            this.label7.Text = GPStrings.getSharedStrings().getString(257);
            this.label6.Text = GPStrings.getSharedStrings().getString(12);
            this.label5.Text = GPStrings.getSharedStrings().getString(10);
            this.label4.Text = GPStrings.getSharedStrings().getString(11);
            this.label3.Text = GPStrings.getSharedStrings().getString(253);
            this.label2.Text = GPStrings.getSharedStrings().getString(9);

            if (wizardDelegate != null)
                wizardDelegate.EnableNext(false);
        }


        /// <summary>
        /// Property for synchronous set of searched text
        /// </summary>
        public string FindText
        {
            set
            {
                lock (findText)
                {
                    findText = value.ToLower();
                }
                if (findOperation != null)
                {
                    findOperation.Cancelled = true;
                }
                findOperation = new CELFindCity();
                findOperation.text = findText;
                findOperation.Invoke(this);
                findTextChanged = true;
            }
        }

        public double SelectedLongitude
        {
            get
            {
                return (Convert.ToDouble(comboBox2.SelectedIndex) + Convert.ToDouble(comboBox4.SelectedIndex) / 60.0) * (comboBox3.SelectedIndex == 0 ? 1.0 : -1.0);
            }
        }

        public double SelectedLatitude
        {
            get
            {
                return (Convert.ToDouble(comboBox7.SelectedIndex) + Convert.ToDouble(comboBox5.SelectedIndex) / 60.0) * (comboBox6.SelectedIndex == 0 ? 1.0 : -1.0);
            }
        }

        public GPLocationProvider SelectedLocation
        {
            get
            {
                switch(tabControl1.SelectedIndex)
                {
                    case 0:
                        if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Tag is GPLocation)
                        {
                            return new GPLocationProvider(listView1.SelectedItems[0].Tag as GPLocation);
                        }
                        return null;
                    case 1:
                        GPLocation loc = new GPLocation();
                        loc.setCity(textBox2.Text);
                        loc.setCountryCode(comboBox1.SelectedItem as string);
                        loc.setLatitudeNorthPositive(SelectedLatitude);
                        loc.setLongitudeEastPositive(SelectedLongitude);
                        loc.setTimeZoneName(comboBox8.SelectedItem as string);
                        return new GPLocationProvider(loc);
                    case 2:
                        return GPAppHelper.getMyLocation();
                    default:
                        return null;
                }
            }
        }

        private bool p_trav_visible = true;

        public bool TravellerVisible
        {
            get
            {
                return p_trav_visible;
            }
            set
            {
                p_trav_visible = value;
                if (value)
                {
                    tabControl1.TabPages.Add(tabPage3);
                }
                else
                {
                    tabControl1.TabPages.Remove(tabPage3);
                }
                ActiveControl = textBox1;
                textBox1.Select();
                textBox1.Focus();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LocationPickerControl()
        {
            InitializeComponent();
            ActiveControl = textBox1;

            foreach (GPCountry cn in GPCountryList.getShared().countries)
            {
                comboBox1.Items.Add(cn.getCode());
            }

            foreach (GPTimeZone tz in GPTimeZoneList.sharedTimeZones().getTimeZones())
            {
                comboBox8.Items.Add(tz.Name);
            }

            for (int i = 0; i <= 180; i++)
            {
                comboBox2.Items.Add(i.ToString());
            }
            for (int i = 0; i < 90; i++)
            {
                comboBox7.Items.Add(i.ToString());
            }
            for (int i = 0; i < 60; i++)
            {
                comboBox4.Items.Add(i.ToString());
                comboBox5.Items.Add(i.ToString());
            }

            comboBox3.Items.Add("East");
            comboBox3.Items.Add("West");
            comboBox6.Items.Add("North");
            comboBox6.Items.Add("South");

            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;

            UpdateEnabledButtons();

            Debugger.Log(0, "", "textBox1 focus start\n");
            textBox1.Select();
//            textBox1.Focus();
//            textBox1.Select(0, textBox1.Text.Length);
            Debugger.Log(0, "", "textBox1 focus end\n");
        }


        private void UpdateEnabledButtons()
        {
            bool bItems = (GPAppHelper.getMyLocation().getChangeCount() > 0);
            bool bSomeSelected = listBox1.SelectedIndex >= 0;
            bool bLocSelected = (listBox1.SelectedItem is GPLocation);
            bool bNotFirstItem = listBox1.SelectedIndex > 0;
            bool bNotLastItem = listBox1.SelectedIndex < (listBox1.Items.Count - 1);

            buttonAddTravelling.Enabled = !bItems;
            buttonEdit.Enabled = bSomeSelected & !bLocSelected;
            buttonRemoveAfter.Enabled = bSomeSelected & bItems & (listBox1.SelectedIndex < (listBox1.Items.Count - 2));
            buttonClearAll.Enabled = bItems;
            buttonRemoveBefore.Enabled = bSomeSelected & bItems & (listBox1.SelectedIndex > 1);
            buttonInsertBefore.Enabled = bSomeSelected & bItems;
            buttonInsertAfter.Enabled = bSomeSelected & bItems;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!supressTextChange)
            {
                FindText = textBox1.Text;
            }
        }

        public delegate void ClearLocationListDelegate();
        public delegate void AddLocationToViewDelegate(List<GPLocation> locs);
        public delegate void SetSelectionDelegate();

        public delegate void SelectedLocationChangeHandler (object sender, EventArgs data);

        public event SelectedLocationChangeHandler SelectedLocationChange;

        protected void OnSelectedLocationChanged(object sender, EventArgs data)
        {
            if (supressLocationChangedEvent)
                return;

            if (SelectedLocationChange != null)
            {
                SelectedLocationChange(this, data);
            }
        }

        public void ClearLocationList()
        {
            listView1.Items.Clear();
        }

        public void SetSelection()
        {
            listView1.SelectedItems.Clear();
            if (listView1.Items.Count > 0)
            {
                listView1.Items[0].Selected = true;
                OnSelectedLocationChanged(this, new EventArgs());
            }
        }

        public void AddLocationToView(List<GPLocation> locs)
        {
            listView1.BeginUpdate();
            foreach (GPLocation loc in locs)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = loc.getCity();
                lvi.SubItems.Add(loc.getCountryCode());
                lvi.SubItems.Add(loc.getLongitudeString());
                lvi.SubItems.Add(loc.getLatitudeString());
                lvi.SubItems.Add(loc.getTimeZoneName());
                lvi.Tag = loc;
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();

            CheckNextButtonInParent();

        }

        private void CheckNextButtonInParent()
        {
            if (wizardDelegate != null)
                wizardDelegate.EnableNext(listView1.Items.Count > 0);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (findTextChanged == false)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    List<GPLocation> locs = new List<GPLocation>();
                    bool interrupted = false;
                    bool finished = false;
                    string text;
                    while (!finished)
                    {
                        lock (findText)
                        {
                            text = findText;
                        }
                        listView1.Invoke(new ClearLocationListDelegate(ClearLocationList));
                        interrupted = false;
                        findTextChanged = false;
                        if (text.Length > 0)
                        {
                            foreach (GPLocation loc in GPLocationList.getShared().locations)
                            {
                                if (loc.getCity().IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0)
                                {
                                    locs.Add(loc);
                                }
                                if (findTextChanged)
                                {
                                    interrupted = true;
                                    break;
                                }
                            }
                        }
                        if (!interrupted)
                            finished = true;
                    }
                    listView1.Invoke(new AddLocationToViewDelegate(AddLocationToView), locs);
                    listView1.Invoke(new SetSelectionDelegate(SetSelection));
                }
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem lvi = listView1.SelectedItems[0];
            if ((lvi.Tag is GPLocation) == false)
                return;

            GPLocation loc = lvi.Tag as GPLocation;

            modifiedFlagAdded = true;
            int h1, m1;
            textBox2.Text = loc.getCity();
            GPAppHelper.hoursToParts(loc.GetLongitudeEastPositive(), out h1, out m1);
            comboBox2.SelectedIndex = h1;
            comboBox4.SelectedIndex = m1;
            comboBox3.SelectedIndex = (loc.GetLongitudeEastPositive() > 0.0 ? 0 : 1);
            GPAppHelper.hoursToParts(loc.GetLatitudeNorthPositive(), out h1, out m1);
            comboBox7.SelectedIndex = h1;
            comboBox6.SelectedIndex = (loc.GetLatitudeNorthPositive() > 0.0 ? 0 : 1);
            comboBox5.SelectedIndex = m1;
            comboBox8.SelectedItem = loc.getTimeZoneName();
            comboBox1.SelectedItem = loc.getCountryCode();
            modifiedFlagAdded = false;

            if (listView1.Focused)
            {
                supressTextChange = true;
                textBox1.Text = loc.getCity();
                supressTextChange = false;
            }

            modifiedFlagAdded = false;
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox8.SelectedIndex < 0 || comboBox8.SelectedIndex >= comboBox8.Items.Count)
                return;

            GPTimeZone tzone = GPTimeZoneList.sharedTimeZones().GetTimezoneByName(comboBox8.SelectedItem as string);
            if (tzone == null)
            {
                label8.Text = "-";
                return;
            }

            if (tzone.Transitions.Count == 0)
            {
                label8.Text = "-";
                return;
            }

            GPTimestamp ts = new GPTimestamp(DateTime.Now);
            GPTimeZone.Transition trans = tzone.GetNextTransition(ts);
            if (trans == null)
            {
                label8.Text = "-";
                return;
            }

            DateTime dt = trans.getDateTime();
            label8.Text = dt.ToLongDateString() + " - " + dt.ToShortTimeString();

            if (comboBox1.SelectedItem != null && comboBox8.SelectedItem != null && tabControl1.SelectedIndex == 1)
                OnSelectedLocationChanged(this, e);
            ModifyNameOfCity();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox8.SelectedItem != null && tabControl1.SelectedIndex == 1 && comboBox1.SelectedItem != null)
                OnSelectedLocationChanged(this, e);
            ModifyNameOfCity();
        }

        public void LoadInterfaceValues(string prefix)
        {
            initPrefix = prefix;
            tabControl1.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".tabctrl", 0);
            textBox1.Text = GPUserDefaults.StringForKey(prefix + ".findtext", "");
            textBox2.Text = GPUserDefaults.StringForKey(prefix + ".city", "");
            modifiedFlagAdded = true;
            comboBox1.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cc", -1);
            comboBox2.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cb2", 0);
            comboBox3.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cb3", 0);
            comboBox4.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cb4", 0);
            comboBox5.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cb5", 0);
            comboBox6.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cb6", 0);
            comboBox7.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".cb7", 0);
            comboBox8.SelectedIndex = GPUserDefaults.IntForKey(prefix + ".tzoneindex", -1);
            modifiedFlagAdded = GPUserDefaults.BoolForKey(prefix + ".modifyflag", false);

            LoadProvider();
        }

        public void SaveInterfaceValues(string prefix)
        {
            GPUserDefaults.SetIntForKey(prefix + ".tabctrl", tabControl1.SelectedIndex);
            GPUserDefaults.SetStringForKey(prefix + ".findtext", textBox1.Text);
            GPUserDefaults.SetStringForKey(prefix + ".city", textBox2.Text);
            GPUserDefaults.SetIntForKey(prefix + ".cc", comboBox1.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".cb2", comboBox2.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".cb3", comboBox3.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".cb4", comboBox4.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".cb5", comboBox5.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".cb6", comboBox6.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".cb7", comboBox7.SelectedIndex);
            GPUserDefaults.SetIntForKey(prefix + ".tzoneindex", comboBox8.SelectedIndex);
            GPUserDefaults.SetBoolForKey(prefix + ".modifyflag", modifiedFlagAdded);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyNameOfCity();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyNameOfCity();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyNameOfCity();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyNameOfCity();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyNameOfCity();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModifyNameOfCity();
        }

        private void ModifyNameOfCity()
        {
            if (!modifiedFlagAdded)
            {
                textBox2.Text += " (Adjusted)";
                modifiedFlagAdded = true;
            }
        }


        public void TaskStarted(CELBase task)
        {
        }

        public void TaskFinished(CELBase task)
        {
            if (task is CELFindCity)
            {
                CELFindCity find = task as CELFindCity;
                listView1.Items.Clear();
                AddLocationToView(find.Locations);
                findOperation = null;
                listView1.Invoke(new SetSelectionDelegate(SetSelection));
            }
        }

        public void TaskProgress(CELBase task, double prog)
        {
        }

        public void TaskFinishedSync(CELBase task)
        {
        }

        private void LoadProvider()
        {
            GPLocationProvider lp = GPAppHelper.getMyLocation();

            int i;
            int count = lp.getChangeCount();

            listBox1.Items.Clear();
            for (i = 0; i < count; i++)
            {
                listBox1.Items.Add(lp.getLocationAtIndex(i));
                listBox1.Items.Add(lp.getChangeAtIndex(i));
            }
            listBox1.Items.Add(lp.getLocationAtIndex(i));
            UpdateEnabledButtons();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabledButtons();
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Cast the sender object back to ListBox type.
            ListBox theListBox = (ListBox)sender;

            // Get the string contained in each item. 
            object item = theListBox.Items[e.Index];

            // If the item is the selected item, then draw the rectangle 
            // filled in blue. The item is selected when a bitwise And   
            // of the State property and the DrawItemState.Selected  
            // property is true. 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
            }
            else
            {
                // Otherwise, draw the rectangle filled in beige.
                e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, e.Bounds);
            }

            // Draw a rectangle in blue around each item.
            //e.Graphics.DrawRectangle(Pens.Blue, e.Bounds);

            if (item is GPLocation)
            {
                GPLocation loc = item as GPLocation;
                e.Graphics.DrawString(loc.getName(), labelCount.Font, Brushes.Black, e.Bounds.X + 4, e.Bounds.Y + 4);
                e.Graphics.DrawString("Timezone: " + loc.getTimeZone().getFullName(), label1.Font, Brushes.Black, e.Bounds.X + 4, e.Bounds.Y + 20);
            }
            else if (item is GPLocationChange)
            {
                GPLocationChange chan = item as GPLocationChange;
                GPGregorianTime gt = new GPGregorianTime(chan.LocationA);
                gt.setJulianGreenwichTime(new GPJulianTime(chan.julianStart, 0));
                string humanStart = string.Format("{0} {1}", gt.getLongTimeString(), chan.LocationA.getTimeZoneName());

                e.Graphics.DrawString(gt.getLongDateString() + " - travelling", labelCount.Font, Brushes.Black, e.Bounds.X + 48, e.Bounds.Y + 4);
                e.Graphics.DrawLine(Pens.Black, e.Bounds.X + 40, e.Bounds.Y + 4, e.Bounds.X + 40, e.Bounds.Bottom - 4);

                e.Graphics.DrawString("Start: " + chan.humanStart, label1.Font, Brushes.Black, e.Bounds.X + 48, e.Bounds.Y + 20);
                //e.Graphics.DrawString("End: " + chan.humanEnd, label1.Font, Brushes.Black, e.Bounds.X + 48, e.Bounds.Y + 36);
                e.Graphics.DrawString("Length: " + chan.humanLength, label1.Font, Brushes.Black, e.Bounds.X + 48, e.Bounds.Y + 36);
            }

            // Draw the focus rectangle around the selected item.
            e.DrawFocusRectangle();

        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            // Cast the sender object back to ListBox type.
            ListBox theListBox = (ListBox)sender;

            // Get the string contained in each item. 
            object item = theListBox.Items[e.Index];

            if (item is GPLocation)
            {
                e.ItemHeight = 40;
            }
            else if (item is GPLocationChange)
            {
                e.ItemHeight = 50;
            }
        }

        /// <summary>
        /// Add new travelling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            WizardDlg wizard = new WizardDlg();

            LocationPickerControl lpc = new LocationPickerControl();
            lpc.TravellerVisible = false;
            wizard.AddPage(lpc, "Start Location", "Select location, where you start your travelling");

            LocationPickerControl lpc2 = new LocationPickerControl();
            lpc2.TravellerVisible = false;
            wizard.AddPage(lpc2, "Target Location", "Select location, where you finish your traveling");

            TravellingEditControl tec = new TravellingEditControl();
            tec.LocationPicker = lpc;
            tec.LocationPicker2 = lpc2;
            tec.TransitionTimezoneIndex = 0;
            wizard.AddPage(tec, "Travelling time", "Specify travelling time");

            wizard.ResetPages();

            if (wizard.ShowDialog() == DialogResult.OK)
            {
                GPLocationChange newChange = new GPLocationChange();

                newChange.LocationA = lpc.SelectedLocation.getLocation(0);
                newChange.LocationB = lpc2.SelectedLocation.getLocation(0);

                GPGregorianTime start = new GPGregorianTime(newChange.LocationA);
                start.setDateTime(tec.getUTCDateTime(0).AddHours(newChange.LocationA.getTimeZoneOffsetHours()));
                newChange.julianStart = start.getJulianGreenwichTime();

                GPGregorianTime end = new GPGregorianTime(newChange.LocationB);
                end.setDateTime(tec.getUTCDateTime(1).AddHours(newChange.LocationB.getTimeZoneOffsetHours()));
                newChange.julianEnd = end.getJulianGreenwichTime();

                newChange.TimezoneStart = (tec.TransitionTimezoneIndex == 0);

                GPLocationProvider lp = GPAppHelper.getMyLocation();

                lp.addChange(newChange);

                RefreshList();
                travellingChanged = true;
            }
        }

        private void RefreshList()
        {
            int prevTop = listBox1.TopIndex;
            LoadProvider();
            listBox1.TopIndex = prevTop;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListBox lbox = listBox1;
            int index = listBox1.SelectedIndex;

            if (lbox.SelectedItem is GPLocation)
            {
                EditLocationInListbox(lbox, index);
            }
            else if (lbox.SelectedItem is GPLocationChange)
            {
                EditTravellingInListbox(lbox, index);
            }
            travellingChanged = true;


        }

        private void EditLocationInListbox(ListBox lbox, int index)
        {
        }

        private void EditTravellingInListbox(ListBox lbox, int index)
        {
            GPLocationChange newChange = (GPLocationChange)lbox.SelectedItem;

            if (newChange != null)
            {
                WizardDlg wizard = new WizardDlg();
                GPGregorianTime start, end;
                TravellingEditControl tec = new TravellingEditControl();
                tec.LocationObj = newChange.LocationA;
                tec.LocationObj2 = newChange.LocationB;
                wizard.AddPage(tec, "Travelling time", "Specify travelling time");

                tec.TransitionTimezoneIndex = (newChange.TimezoneStart ? 0 : 1);

                start = new GPGregorianTime(newChange.TimezoneStart ? newChange.LocationA : newChange.LocationB);
                start.setJulianGreenwichTime(new GPJulianTime(newChange.julianStart, 0));
                tec.setUTCDateTime(start.getLocalTime(), 0);

                start.setJulianGreenwichTime(new GPJulianTime(newChange.julianEnd, 0));
                tec.setUTCDateTime(start.getLocalTime(), 1);

                wizard.ResetPages();

                if (wizard.ShowDialog() == DialogResult.OK)
                {
                    start = new GPGregorianTime(newChange.LocationA);
                    start.setDateTime(tec.getUTCDateTime(0).AddHours(newChange.LocationA.getTimeZoneOffsetHours()));
                    newChange.julianStart = start.getJulianGreenwichTime();

                    end = new GPGregorianTime(newChange.LocationB);
                    end.setDateTime(tec.getUTCDateTime(1).AddHours(newChange.LocationB.getTimeZoneOffsetHours()));
                    newChange.julianEnd = end.getJulianGreenwichTime();

                    newChange.TimezoneStart = (tec.TransitionTimezoneIndex == 0);

                    listBox1.Invalidate();
                    listBox1.Update();
                }
            }
        }

        private int getChangeIndexFromListBoxIndex(int index)
        {
            return index / 2;
        }

        private int SelectedIndex
        {
            get
            {
                return listBox1.SelectedIndex;
            }
        }

        private bool IsSelectedLocation
        {
            get
            {
                return (SelectedIndex % 2) == 0;
            }
        }

        private bool IsSelectedTravelling
        {
            get
            {
                return (SelectedIndex % 2) == 1;
            }
        }

        // remove after
        private void button3_Click(object sender, EventArgs e)
        {
            ListBox lbox = listBox1;
            int index = lbox.SelectedIndex;
            int changeIndex = getChangeIndexFromListBoxIndex(index);
            GPLocationProvider provider = GPAppHelper.getMyLocation();

            while (lbox.Items.Count > changeIndex)
            {
                provider.removeChangeAtIndex(changeIndex);
            }
            RefreshList();
            travellingChanged = true;

        }

        // clear all
        private void button4_Click(object sender, EventArgs e)
        {
            GPLocationProvider prov = GPAppHelper.getMyLocation();

            prov.Clear();

            RefreshList();
            travellingChanged = true;

        }

        // remove before
        private void button5_Click(object sender, EventArgs e)
        {
            ListBox lbox = listBox1;
            int index = lbox.SelectedIndex;
            int changeIndex = getChangeIndexFromListBoxIndex(index);
            GPLocationProvider provider = GPAppHelper.getMyLocation();
            int i = 0;
            while (i < changeIndex)
            {
                //lbox.Items.RemoveAt(0);
                provider.removeChangeAtIndex(0);
                i++;
            }
            RefreshList();
            travellingChanged = true;

        }

        // insert before
        private void button6_Click(object sender, EventArgs e)
        {
            ListBox lbox = listBox1;
            int index = lbox.SelectedIndex;
            int changeIndex = getChangeIndexFromListBoxIndex(index);
            GPLocationChange chng = null;
            GPLocationProvider provider = GPAppHelper.getMyLocation();

            // ask for travelling time
            // ask for starting location
            WizardDlg wizard = new WizardDlg();

            LocationPickerControl lpc = new LocationPickerControl();
            lpc.TravellerVisible = false;
            wizard.AddPage(lpc, "Start Location", "Select location, where you start your travelling");

            //LocationPickerControl lpc2 = new LocationPickerControl();
            //lpc2.TravellerVisible = false;
            //wizard.AddPage(lpc2, "Target Location", "Select location, where you finish your traveling");

            TravellingEditControl tec = new TravellingEditControl();
            tec.LocationPicker = lpc;
            if (IsSelectedLocation)
            {
                tec.LocationObj2 = listBox1.SelectedItem as GPLocation;
                if ((listBox1.SelectedIndex + 1) < listBox1.Items.Count)
                {
                    chng = (listBox1.Items[listBox1.SelectedIndex + 1] as GPLocationChange);
                }
            }
            if (IsSelectedTravelling)
            {
                chng = (listBox1.SelectedItem as GPLocationChange);
                tec.LocationObj2 = chng.LocationA;
            }
            tec.TransitionTimezoneIndex = 0;
            if (chng != null)
            {
                GPGregorianTime start = new GPGregorianTime(tec.LocationObj2);
                start.setJulianGreenwichTime(new GPJulianTime(chng.julianStart, 0));
                tec.SetTopStartDateLimit(start.getLocalTime());
            }
            wizard.AddPage(tec, "Travelling time", "Specify travelling time");

            wizard.ResetPages();

            if (wizard.ShowDialog() == DialogResult.OK)
            {
                GPLocationChange newChange = new GPLocationChange();

                newChange.LocationA = lpc.SelectedLocation.getLocation(0);
                newChange.LocationB = tec.LocationObj2;

                GPGregorianTime start = new GPGregorianTime(newChange.LocationA);
                start.setDateTime(tec.getUTCDateTime(0).AddHours(newChange.LocationA.getTimeZoneOffsetHours()));
                newChange.julianStart = start.getJulianGreenwichTime();

                GPGregorianTime end = new GPGregorianTime(newChange.LocationB);
                end.setDateTime(tec.getUTCDateTime(1).AddHours(newChange.LocationB.getTimeZoneOffsetHours()));
                newChange.julianEnd = end.getJulianGreenwichTime();

                newChange.TimezoneStart = (tec.TransitionTimezoneIndex == 0);

                // remove travellings before changeIndex
                // insert new travelling at index 0
                provider.removeChangesBeforeIndex(changeIndex);
                provider.insertChangeAtIndex(0, newChange);

                RefreshList();
                travellingChanged = true;

            }

        }

        // insert after
        private void button7_Click(object sender, EventArgs e)
        {
            ListBox lbox = listBox1;
            int index = lbox.SelectedIndex;
            int changeIndex = getChangeIndexFromListBoxIndex(index);

            GPLocationChange chng = null;
            GPLocationProvider provider = GPAppHelper.getMyLocation();

            // ask for travelling time
            // ask for starting location
            WizardDlg wizard = new WizardDlg();

            LocationPickerControl lpc = new LocationPickerControl();
            lpc.TravellerVisible = false;
            wizard.AddPage(lpc, "Target Location", "Select location, where you end your travelling");

            //LocationPickerControl lpc2 = new LocationPickerControl();
            //lpc2.TravellerVisible = false;
            //wizard.AddPage(lpc2, "Target Location", "Select location, where you finish your traveling");

            TravellingEditControl tec = new TravellingEditControl();
            if (IsSelectedLocation)
            {
                tec.LocationObj = listBox1.SelectedItem as GPLocation;
                if ((listBox1.SelectedIndex - 1) > 0)
                {
                    chng = (listBox1.Items[listBox1.SelectedIndex - 1] as GPLocationChange);
                }
            }
            if (IsSelectedTravelling)
            {
                chng = (listBox1.SelectedItem as GPLocationChange);
                tec.LocationObj = chng.LocationB;
            }
            tec.LocationPicker2 = lpc;
            tec.TransitionTimezoneIndex = 0;
            if (chng != null)
            {
                GPGregorianTime start = new GPGregorianTime(tec.LocationObj);
                start.setJulianGreenwichTime(new GPJulianTime(chng.julianStart, 0));
                tec.SetBottomEndDateLimit(start.getLocalTime());
            }
            wizard.AddPage(tec, "Travelling time", "Specify travelling time");

            wizard.ResetPages();

            if (wizard.ShowDialog() == DialogResult.OK)
            {
                GPLocationChange newChange = new GPLocationChange();

                newChange.LocationA = tec.LocationObj;
                newChange.LocationB = lpc.SelectedLocation.getLocation(0);

                GPGregorianTime start = new GPGregorianTime(newChange.LocationA);
                start.setDateTime(tec.getUTCDateTime(0).AddHours(newChange.LocationA.getTimeZoneOffsetHours()));
                newChange.julianStart = start.getJulianGreenwichTime();

                GPGregorianTime end = new GPGregorianTime(newChange.LocationB);
                end.setDateTime(tec.getUTCDateTime(1).AddHours(newChange.LocationB.getTimeZoneOffsetHours()));
                newChange.julianEnd = end.getJulianGreenwichTime();

                newChange.TimezoneStart = (tec.TransitionTimezoneIndex == 0);

                // remove travellings before changeIndex
                // insert new travelling at index 0
                provider.removeChangesAfterIndex(changeIndex);
                provider.addChange(newChange);

                RefreshList();
                travellingChanged = true;

            }

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
        }

        private void LocationPickerControl_Enter(object sender, EventArgs e)
        {
            textBox1.Select();
            CheckNextButtonInParent();
        }

        private void LocationPickerControl_Leave(object sender, EventArgs e)
        {
            if (TravellerVisible && travellingChanged)
            {
                GPAppHelper.saveMyLocation();
            }
        }

    }
}

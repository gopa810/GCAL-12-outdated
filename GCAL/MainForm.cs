using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using GCAL.Base;
using GCAL.Dialogs;
using GCAL.Engine;
using GCAL.Controls;

namespace GCAL
{
    public partial class MainForm : Form, CELAsyncResultReceiver
    {
        public CELSearch searchAgent = null;
        public object CurrentCalculatedObject = null;
        public string newVersion = string.Empty;
        public string newVersionLink = string.Empty;
        private string p_newVersionFile = string.Empty;
        private bool p_newVersionProgress = false;

        public MainForm()
        {
            InitializeComponent();

            SupervisorForm.WindowOpened(this);

            splitContainer1.Panel2Collapsed = true;
            webBrowser1.Focus();

            webBrowser1.DocumentText = GPAppHelper.GenerateStartupPage();

            CELStatupTask task = new CELStatupTask();
            task.Window = this;
            task.Invoke(this);

            this.Text = GPAppHelper.getLongVersionText();
            //cnwc.Invoke(this);
        }

        public void SetUserInterfaceStrings()
        {
            this.fileToolStripMenuItem.Text = GPStrings.getString(337);
            this.saveContentToolStripMenuItem.Text = GPStrings.getString(362);
            this.printToolStripMenuItem.Text = GPStrings.getString(363);
            this.exitToolStripMenuItem.Text = GPStrings.getString(378);
            this.editToolStripMenuItem.Text = GPStrings.getString(338);
            this.copyToolStripMenuItem.Text = GPStrings.getString(365);
            this.selectAllToolStripMenuItem.Text = GPStrings.getString(364);
            this.clearSelectionToolStripMenuItem.Text = GPStrings.getString(379);
            this.viewToolStripMenuItem.Text = GPStrings.getString(339);
            this.organizerToolStripMenuItem.Text = GPStrings.getString(360);
            this.todayScreenToolStripMenuItem.Text = GPStrings.getString(174);
            this.plainTextToolStripMenuItem.Text = GPStrings.getString(380);
            this.richTextToolStripMenuItem.Text = GPStrings.getString(381);
            this.textSize10ToolStripMenuItem1.Text = GPStrings.getString(382);
            this.textSize11ToolStripMenuItem1.Text = GPStrings.getString(383);
            this.textSize12ToolStripMenuItem1.Text = GPStrings.getString(384);
            this.textSize13ToolStripMenuItem1.Text = GPStrings.getString(385);
            this.textSize14ToolStripMenuItem1.Text = GPStrings.getString(386);
            this.calculateToolStripMenuItem.Text = GPStrings.getString(387);
            this.calendarToolStripMenuItem.Text = GPStrings.getString(44);
            this.calendarCoreEventsToolStripMenuItem.Text = GPStrings.getString(388);
            this.calendarFor2LocationsToolStripMenuItem.Text = GPStrings.getString(389);
            this.appearanceDayToolStripMenuItem.Text = GPStrings.getString(45);
            this.coreEventsToolStripMenuItem.Text = GPStrings.getString(46);
            this.masaListToolStripMenuItem.Text = GPStrings.getString(48);
            this.settingsToolStripMenuItem.Text = GPStrings.getString(372);
            this.appearanceDaySettingsToolStripMenuItem.Text = GPStrings.getString(970);
            this.calendarDisplayToolStripMenuItem.Text = GPStrings.getString(971);
            this.coreEventsSettingsToolStripMenuItem.Text = GPStrings.getString(972);
            this.masaListSettingsToolStripMenuItem.Text = GPStrings.getString(973);
            this.generalSettingsToolStripMenuItem.Text = GPStrings.getString(390);
            this.windowToolStripMenuItem.Text = GPStrings.getString(391);
            this.newWindowToolStripMenuItem.Text = GPStrings.getString(392);
            this.closeToolStripMenuItem.Text = GPStrings.getString(393);
            this.helpToolStripMenuItem.Text = GPStrings.getString(394);
            this.aboutToolStripMenuItem.Text = GPStrings.getString(395);
            this.showStartupTipsToolStripMenuItem.Text = GPStrings.getString(396);
            this.helpToolStripMenuItem1.Text = GPStrings.getString(394);
            this.toolStripTextBox1.Text = GPStrings.getString(397);
            this.toolStripStatusLabel1.Text = GPStrings.getString(398);
            this.tableToolStripMenuItem.Text = GPStrings.getString(450);
            this.Text = GPStrings.getString(399);
        }

        //public void SetUserInterfaceStrings()
        //{
        //}

        public enum DialogNames
        {
            GetLocation,
            GetTwoLocations,
            GetStartDate,
            GetPeriodLength,
            GetPeriodYears,
            GetDateTime
        }

        private GPCalculationOperation p_current_result_type = GPCalculationOperation.None;
        Dictionary<GPCalculationParameters, object> p_results = new Dictionary<GPCalculationParameters, object>();


        /// <summary>
        /// Executes command from foreign window.
        /// </summary>
        /// <param name="command">Command to execute</param>
        public void Execute(string command)
        {
            if (command == "organizerform.closed")
            {
                this.BringToFront();
                this.Activate();
            }
        }

        /// <summary>
        /// Shows Organizer window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void organizerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCollection forms = Application.OpenForms;
            OrganizerForm organizer = null;

            foreach (Form form in forms)
            {
                if (form is OrganizerForm)
                {
                    organizer = form as OrganizerForm;
                    break;
                }
            }

            if (organizer != null)
            {
                organizer.BringToFront();
                organizer.Activate();
            }
            else
            {
                organizer = new OrganizerForm();
                organizer.Show();
            }
        }

        /// <summary>
        /// Open new main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm newMainForm = new MainForm();
            newMainForm.Show();
        }

        /// <summary>
        /// close this form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Copy", true, null);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("SelectAll", true, null);
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Unselect", true, null);
        }


        private Dictionary<GPCalculationParameters, object> RunDialogs(DialogNames[] dialogNames)
        {
            //Dictionary<DialogNames, Form> dialogs = new Dictionary<DialogNames, Form>();
            Dictionary<GPCalculationParameters, object> results = new Dictionary<GPCalculationParameters, object>();
            int state = 0;
            DialogResult result;

            while (state >= 0 && state < dialogNames.Length)
            {
                // get location dialog
                if (dialogNames[state] == DialogNames.GetLocation)
                {
                    GetLocationDlg dlg = new GetLocationDlg();
                    dlg.LoadInterface();
                    result = dlg.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return null;
                    }
                    else if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        dlg.SaveInterface();
                        results[GPCalculationParameters.LocationProvider] = dlg.SelectedLocation;
                        state++;
                    }
                    else if (result == System.Windows.Forms.DialogResult.No)
                    {
                        state--;
                    }
                }
                else if (dialogNames[state] == DialogNames.GetTwoLocations)
                {
                    GetTwoLocationsDlg dlg1 = new GetTwoLocationsDlg();
                    result = dlg1.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        return null;
                    }
                    else if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        dlg1.SaveInterface();
                        results[GPCalculationParameters.LocationProvider] = dlg1.SelectedLocationA;
                        results[GPCalculationParameters.LocationA] = dlg1.SelectedLocationA;
                        results[GPCalculationParameters.LocationB] = dlg1.SelectedLocationB;
                        state++;
                    }
                    else if (result == System.Windows.Forms.DialogResult.No)
                    {
                        state--;
                    }
                }
                // get time range dialog
                else if (dialogNames[state] == DialogNames.GetStartDate)
                {
                    if (results.ContainsKey(GPCalculationParameters.LocationProvider))
                    {
                        GetStartDateDlg dlg2 = new GetStartDateDlg(results[GPCalculationParameters.LocationProvider] as GPLocationProvider);
                        dlg2.Picker.LoadInterfaceValues("getstartdate");
                        result = dlg2.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.Cancel)
                            return null;
                        else if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            dlg2.Picker.SaveInterfaceValues("getstartdate");
                            results[GPCalculationParameters.StartWesternDate] = dlg2.Picker.GetWesternDate();
                            results[GPCalculationParameters.StartVedicDate] = dlg2.Picker.GetVedicDate();
                            state++;
                        }
                        else if (result == System.Windows.Forms.DialogResult.No)
                        {
                            state--;
                        }
                    }
                    else
                        return null;
                }
                else if (dialogNames[state] == DialogNames.GetPeriodLength)
                {
                    if (results.ContainsKey(GPCalculationParameters.LocationProvider) && results.ContainsKey(GPCalculationParameters.StartVedicDate) && results.ContainsKey(GPCalculationParameters.StartWesternDate))
                    {
                        GetPeriodLengthDlg dlg3 = new GetPeriodLengthDlg(results[GPCalculationParameters.LocationProvider] as GPLocationProvider,
                            results[GPCalculationParameters.StartWesternDate] as GPGregorianTime,
                            results[GPCalculationParameters.StartVedicDate] as GPVedicTime);
                        dlg3.Picker.LoadInterfaceValues("getperiod");
                        result = dlg3.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.Cancel)
                            return null;
                        else if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            dlg3.Picker.SaveInterfaceValues("getperiod");
                            results[GPCalculationParameters.EndWesternDate] = dlg3.Picker.EndWesternDate;
                            results[GPCalculationParameters.EndVedicDate] = dlg3.Picker.EndVedicDate;
                            state++;
                        }
                        else if (result == System.Windows.Forms.DialogResult.No)
                        {
                            state--;
                        }
                    }
                    else
                        return null;
                }
                else if (dialogNames[state] == DialogNames.GetPeriodYears)
                {
                    GetPeriodYearsDlg dlg4 = new GetPeriodYearsDlg();
                    dlg4.LoadInterfaceValues("yearsdlg");
                    result = dlg4.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                        return null;
                    else if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        dlg4.SaveInterfaceValues("yearsdlg");
                        results[GPCalculationParameters.StartYear] = dlg4.SelectedYear;
                        results[GPCalculationParameters.CountYear] = dlg4.SelectedCount;
                        state++;
                    }
                    else if (result == System.Windows.Forms.DialogResult.No)
                    {
                        state--;
                    }
                }
                else if (dialogNames[state] == DialogNames.GetDateTime)
                {
                    if (results.ContainsKey(GPCalculationParameters.LocationProvider))
                    {
                        GetDateTimeDlg dlg5 = new GetDateTimeDlg(results[GPCalculationParameters.LocationProvider] as GPLocationProvider);
                        result = dlg5.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.Cancel)
                            return null;
                        else if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            results[GPCalculationParameters.WesternDateTime] = dlg5.SelectedDateTime;
                            state++;
                        }
                        else if (result == System.Windows.Forms.DialogResult.No)
                        {
                            state--;
                        }
                    }
                    else
                        return null;

                }
            }

            if (state < 0)
                return null;

            return results;
        }

        private bool CheckResults(Dictionary<GPCalculationParameters, object> results, GPCalculationParameters[] keys)
        {
            foreach (GPCalculationParameters key in keys)
            {
                if (!results.ContainsKey(key))
                    return false;
            }
            return true;
        }

        private void calendarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<GPCalculationParameters, object> results;
            results = RunDialogs(new DialogNames[] { DialogNames.GetLocation, DialogNames.GetStartDate, DialogNames.GetPeriodLength });

            if (results != null && CheckResults(results, new GPCalculationParameters[] { GPCalculationParameters.LocationProvider, GPCalculationParameters.StartWesternDate, GPCalculationParameters.EndWesternDate }))
            {
                p_results = results;
                p_current_result_type = GPCalculationOperation.Calendar;
                RecalculateResults();
            }
        }

        private void appearanceDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<GPCalculationParameters, object> results;

            results = RunDialogs(new DialogNames[] { DialogNames.GetLocation, DialogNames.GetDateTime });

            if (results != null && CheckResults(results, new GPCalculationParameters[] { GPCalculationParameters.LocationProvider, GPCalculationParameters.WesternDateTime }))
            {
                p_results = results;
                p_current_result_type = GPCalculationOperation.AppearanceDay;
                RecalculateResults();
            }
        }

        private void coreEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<GPCalculationParameters, object> results;

            results = RunDialogs(new DialogNames[] { DialogNames.GetLocation, DialogNames.GetStartDate, DialogNames.GetPeriodLength });

            if (results != null && CheckResults(results, new GPCalculationParameters[] { GPCalculationParameters.LocationProvider, GPCalculationParameters.StartWesternDate, GPCalculationParameters.EndWesternDate }))
            {
                p_results = results;
                p_current_result_type = GPCalculationOperation.CoreEvents;
                RecalculateResults();
            }
        }

        private void masaListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<GPCalculationParameters, object> results;

            results = RunDialogs(new DialogNames[] { DialogNames.GetLocation, DialogNames.GetPeriodYears });

            if (results != null && CheckResults(results, new GPCalculationParameters[] { GPCalculationParameters.LocationProvider, GPCalculationParameters.StartYear, GPCalculationParameters.CountYear }))
            {
                p_results = results;
                p_current_result_type = GPCalculationOperation.MasaList;
                RecalculateResults();
            }
        }

        private void calendarCoreEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<GPCalculationParameters, object> results;

            results = RunDialogs(new DialogNames[] { DialogNames.GetLocation, DialogNames.GetStartDate, DialogNames.GetPeriodLength });

            if (results != null && CheckResults(results, new GPCalculationParameters[] { GPCalculationParameters.LocationProvider, GPCalculationParameters.StartWesternDate, GPCalculationParameters.EndWesternDate }))
            {
                p_results = results;
                p_current_result_type = GPCalculationOperation.CalendarPlusCore;
                RecalculateResults();
            }
        }

        private void calendarFor2LocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<GPCalculationParameters, object> results;

            results = RunDialogs(new DialogNames[] { DialogNames.GetTwoLocations, DialogNames.GetStartDate, DialogNames.GetPeriodLength });

            if (results != null && CheckResults(results, new GPCalculationParameters[] { GPCalculationParameters.LocationA, GPCalculationParameters.LocationB, GPCalculationParameters.StartWesternDate, GPCalculationParameters.EndWesternDate }))
            {
                p_results = results;
                p_current_result_type = GPCalculationOperation.CalendarForTwoLocations;
                RecalculateResults();
            }
        }

        #region async task execution handlers

        public delegate void TaskStatusDelegate(CELBase task);
        public delegate void TaskStatusProgressDelegate(CELBase task, double progress);

        public void TaskStarted(CELBase task)
        {
            TaskStatusDelegate del = new TaskStatusDelegate(TaskStartedMain);
            if (this.IsHandleCreated)
                this.Invoke(del, task);
        }

        public void TaskFinished(CELBase task)
        {
            toolStripStatusLabel1.Visible = false;
            toolStripProgressBar1.Visible = false;
            if (task is CELGenerateHtml)
            {
                webBrowser1.DocumentText = (task as CELGenerateHtml).HtmlText;
                webBrowser1.Select();
                CurrentCalculatedObject = (task as CELGenerateHtml).CalculatedObject;
            }
            else if (task is CELCheckNextWeeksCalendar)
            {
                CELCheckNextWeeksCalendar checktask = task as CELCheckNextWeeksCalendar;
                HtmlElement htme = webBrowser1.Document.GetElementById("nefid");
                if (htme != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table cellpadding=0 cellspacing=0 border=0 width=95%>");
                    int ic = 0;
                    foreach (GPStringPair dr in (task as CELCheckNextWeeksCalendar).lines)
                    {
                        if (ic % 2 == 0)
                            sb.Append("<tr style='background:#cceeee'>");
                        else
                            sb.Append("<tr>");
                        sb.AppendFormat("<td><span style='font-weight:bold'>{0}</span><br>&nbsp;&nbsp;{1}</td></tr>", dr.Name, dr.Value);
                        ic++;
                    }
                    sb.Append("</table>");
                    htme.InnerHtml = sb.ToString();

                    SetDisplayProperty("nefidMain", "block");
                }

                htme = webBrowser1.Document.GetElementById("todaypart");
                if (htme != null)
                {
                    htme.InnerHtml = checktask.TodayString;
                    SetDisplayProperty("todaypartMain", "block");
                }
            }
            else if (task is CELCheckUpdates)
            {
                HtmlElement htme = webBrowser1.Document.GetElementById("nvid");
                CELCheckUpdates cup = task as CELCheckUpdates;
                if (cup.Success && htme != null)
                {
                    // getting version of this assembly
                    if (GPFileHelper.FileVersion != cup.Version)
                    {
                        newVersion = cup.Version;
                        newVersionLink = cup.VersionLink;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendFormat(GPStrings.getString(458), cup.Version);
                        sb.AppendFormat("<ul>");
                        sb.AppendFormat("<li id='nvdown' style='display:block'><a href=\"http://gcal.app/download\">{0}</a>", GPStrings.getString(455));
                        sb.AppendFormat("<li id='nvdowninst' style='display:block'><a href=\"http://gcal.app/downinst\">{0}</a>", GPStrings.getString(456));
                        sb.AppendFormat("<li id='nvinst' style='display:none'><a href=\"http://gcal.app/install\">{0}</a>", GPStrings.getString(457));
                        sb.AppendFormat("<li id='nvdownprog' style='display:none'>{0}", GPStrings.getString(459));
                        sb.AppendFormat("<li id='nvdownerr' style='display:none'>{0}", GPStrings.getString(460));
                        sb.Append("</ul>");
                        htme.InnerHtml = sb.ToString();
                        SetDisplayProperty("nvidMain", "block");
                    }
                }
            }
            else if (task is CELDownloadFile)
            {
                CELDownloadFile dft = task as CELDownloadFile;
                if (dft.Tag is string)
                {
                    string stringTag = dft.Tag as string;
                    if (stringTag == "DownloadNewVersion" || stringTag == "DownloadNewVersionAndInstall")
                    {
                        if (dft.Success)
                        {
                            newVersionFile = dft.TargetFileName;
                        }
                        else
                        {
                            newVersionFile = string.Empty;
                        }
                    }

                    if (stringTag == "DownloadNewVersionAndInstall")
                    {
                        StartInstallation();
                    }
                }
            }
            //TaskStatusDelegate del = new TaskStatusDelegate(TaskFinishedMain);
            //this.Invoke(del, task);
        }

        public string newVersionFile
        {
            get
            {
                return p_newVersionFile;
            }
            set
            {
                newVersionProgress = false;
                p_newVersionFile = value;
                if (value != null && value.Length > 0)
                {
                    SetDisplayProperty("nvdown", "none");
                    SetDisplayProperty("nvdowninst", "none");
                    SetDisplayProperty("nvinst", "block");
                    SetDisplayProperty("nvdownerr", "none");
                }
                else
                {
                    SetDisplayProperty("nvdown", "block");
                    SetDisplayProperty("nvdowninst", "block");
                    SetDisplayProperty("nvinst", "none");
                    SetDisplayProperty("nvdownerr", "block");
                }
            }
        }

        public bool newVersionProgress
        {
            get
            {
                return p_newVersionProgress;
            }
            set
            {
                p_newVersionProgress = value;
                if (value)
                {
                    SetDisplayProperty("nvdown", "none");
                    SetDisplayProperty("nvdowninst", "none");
                    SetDisplayProperty("nvinst", "none");
                    SetDisplayProperty("nvdownprog", "block");
                    SetDisplayProperty("nvdownerr", "none");
                }
                else
                {
                    SetDisplayProperty("nvdownprog", "none");
                }
            }
        }

        public void SetDisplayProperty(string elementId, string value)
        {
            webBrowser1.Document.InvokeScript("ShowElement", new object[] { elementId, value });
        }

        public void TaskProgress(CELBase task, double progress)
        {
            TaskStatusProgressDelegate del = new TaskStatusProgressDelegate(TaskProgressMain);
            this.Invoke(del, task, progress);
        }

        public void TaskStartedMain(CELBase task)
        {
            toolStripStatusLabel1.Visible = true;
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Value = 0;
        }

        public void TaskFinishedMain(CELBase task)
        {
            toolStripStatusLabel1.Visible = false;
            toolStripProgressBar1.Visible = false;
            if (task is CELGenerateHtml)
            {
                CELGenerateHtml taskHtml = task as CELGenerateHtml;
                webBrowser1.DocumentText = taskHtml.HtmlText;
                CurrentCalculatedObject = taskHtml.CalculatedObject;
            }
            else if (task is CELCheckNextWeeksCalendar)
            {
                Debugger.Log(0, "", "calendar received....");
            }
        }

        public void TaskProgressMain(CELBase task, double progress)
        {
            toolStripProgressBar1.Value = Convert.ToInt32(progress * 100);
        }


        #endregion

        private void calendarDisplayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettingsDlg dlg = new DisplaySettingsDlg("calendar");
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK && p_current_result_type == GPCalculationOperation.Calendar)
                RecalculateResults();
        }

        private void appearanceDaySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettingsDlg dlg = new DisplaySettingsDlg("appearanceday");
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && p_current_result_type == GPCalculationOperation.AppearanceDay)
                RecalculateResults();
        }

        private void coreEventsSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettingsDlg dlg = new DisplaySettingsDlg("coreevents");
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && p_current_result_type == GPCalculationOperation.CoreEvents)
                RecalculateResults();
        }

        private void masaListSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettingsDlg dlg = new DisplaySettingsDlg("masalist");
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && p_current_result_type == GPCalculationOperation.MasaList)
                RecalculateResults();
        }

        private void todayScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TodayForm.ShowForm();
        }

        private void generalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplaySettingsDlg dlg = new DisplaySettingsDlg("general");
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                RecalculateResults();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SupervisorForm.WindowClosed(this);
        }

        public void RecalculateResults()
        {
            RecalculateResults(p_current_result_type, p_results, null);
        }

        public void RecalculateResults(GPCalculationOperation aResultType, Dictionary<GPCalculationParameters, object> aResultParams, string searchText)
        {
            if (aResultParams == null || aResultParams.Count == 0)
                return;
            if (aResultType == GPCalculationOperation.Calendar)
            {
                CELGenerateCalendar task = new CELGenerateCalendar();
                task.SearchText = searchText;
                GPGregorianTime start = aResultParams[GPCalculationParameters.StartWesternDate] as GPGregorianTime;
                GPGregorianTime end = aResultParams[GPCalculationParameters.EndWesternDate] as GPGregorianTime;

                task.SetData(aResultParams[GPCalculationParameters.LocationProvider] as GPLocationProvider,
                    start, Convert.ToInt32(end.getJulianLocalNoon() - start.getJulianLocalNoon()));

                task.Invoke(this);
            }
            else if (aResultType == GPCalculationOperation.CalendarPlusCore)
            {
                CELGenerateCalendarPlusCore task = new CELGenerateCalendarPlusCore();
                task.SearchText = searchText;

                task.SetData(aResultParams[GPCalculationParameters.LocationProvider] as GPLocationProvider,
                    aResultParams[GPCalculationParameters.StartWesternDate] as GPGregorianTime,
                    aResultParams[GPCalculationParameters.EndWesternDate] as GPGregorianTime);
                task.Invoke(this);
            }
            else if (aResultType == GPCalculationOperation.CalendarForTwoLocations)
            {
                CELGenerateCalendarTwoLocs task = new CELGenerateCalendarTwoLocs();
                task.SearchText = searchText;

                GPGregorianTime start = aResultParams[GPCalculationParameters.StartWesternDate] as GPGregorianTime;
                GPGregorianTime end = aResultParams[GPCalculationParameters.EndWesternDate] as GPGregorianTime;

                task.SetData(aResultParams[GPCalculationParameters.LocationA] as GPLocationProvider,
                    aResultParams[GPCalculationParameters.LocationB] as GPLocationProvider,
                    start, Convert.ToInt32(end.getJulianLocalNoon() - start.getJulianLocalNoon()));

                task.Invoke(this);
            }
            else if (aResultType == GPCalculationOperation.CoreEvents)
            {
                CELGenerateCoreEvents task = new CELGenerateCoreEvents();
                task.SearchText = searchText;

                task.SetData(aResultParams[GPCalculationParameters.LocationProvider] as GPLocationProvider,
                    aResultParams[GPCalculationParameters.StartWesternDate] as GPGregorianTime,
                    aResultParams[GPCalculationParameters.EndWesternDate] as GPGregorianTime);

                task.Invoke(this);
            }
            else if (aResultType == GPCalculationOperation.MasaList)
            {
                CELGenerateMasaList task = new CELGenerateMasaList();
                task.SearchText = searchText;
                task.SetData(aResultParams[GPCalculationParameters.LocationProvider] as GPLocationProvider,
                    (int)aResultParams[GPCalculationParameters.StartYear],
                    (int)aResultParams[GPCalculationParameters.CountYear]);
                task.Invoke(this);
            }
            else if (aResultType == GPCalculationOperation.AppearanceDay)
            {
                CELGenerateAppearanceDay task = new CELGenerateAppearanceDay();
                task.SearchText = searchText;
                task.SetData(aResultParams[GPCalculationParameters.LocationProvider] as GPLocationProvider,
                    aResultParams[GPCalculationParameters.WesternDateTime] as GPGregorianTime);
                task.Invoke(this);
            }
            else if (aResultType == GPCalculationOperation.Today)
            {
                TodayForm.ShowForm(aResultParams[GPCalculationParameters.StartWesternDate] as GPGregorianTime);
            }
        }

        private void textSize10ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GPUserDefaults.SetIntForKey("FontSize", 9);
            RecalculateResults();
        }

        private void textSize11ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GPUserDefaults.SetIntForKey("FontSize", 10);
            RecalculateResults();
        }

        private void textSize12ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GPUserDefaults.SetIntForKey("FontSize", 11);
            RecalculateResults();
        }

        private void textSize13ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GPUserDefaults.SetIntForKey("FontSize", 12);
            RecalculateResults();
        }

        private void textSize14ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GPUserDefaults.SetIntForKey("FontSize", 13);
            RecalculateResults();
        }

        private void toolStripTextBox1_GotFocus(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text == GPStrings.getString(397))
            {
                toolStripTextBox1.Text = "";
                toolStripTextBox1.BackColor = SystemColors.Window;
            }
            else if (splitContainer1.Panel2Collapsed == true)
            {
                splitContainer1.Panel2Collapsed = false;
            }
        }

        private void toolStripTextBox1_LostFocus(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text == "")
            {
                toolStripTextBox1.Text = GPStrings.getString(397);
                toolStripTextBox1.BackColor = Color.LightYellow;
                splitContainer1.Panel2Collapsed = true;
            }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Length > 1 && toolStripTextBox1.Text != GPStrings.getString(397))
            {
                splitContainer1.Panel2Collapsed = false;

                StartSearch(GPAppHelper.getMyLocation(), toolStripTextBox1.Text);
            }
        }


        public void StartSearch(GPLocationProvider loc, string text)
        {
            CELSearch search = new CELSearch(text);
            search.Location = loc;
            search.listControl = searchResultsList1;

            search.Invoke(this);

            if (searchAgent != null)
                searchAgent.ShouldCancel = true;
            searchAgent = search;
        }

        private void buttonHideSearchPane_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = true;
        }

        private void searchResultsList1_SelectedItemChanged(object sender, EventArgs e)
        {
            if (searchResultsList1.SelectedItems.Count > 0)
            {
                CELSearch.Results curr = searchResultsList1.SelectedItems[0] as CELSearch.Results;

                RecalculateResults(curr.Operation, curr.Parameters, curr.SearchText);
            }
        }

        private void plainTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CELGenerateHtml.Template = "default:plain";
            RecalculateResults();
        }

        private void richTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CELGenerateHtml.Template = "default:html";
            RecalculateResults();
        }

        private void tableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CELGenerateHtml.Template = "default:table";
            RecalculateResults();
        }

        private void saveContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentCalculatedObject is GPCalendarResults)
            {
                OnSaveCalendar();
            }
            else if (CurrentCalculatedObject is GPCoreEventResults)
            {
                OnSaveEvents();
            }
            else if (CurrentCalculatedObject is GPAppDayResults)
            {
                OnSaveAppday();
            }
            else if (CurrentCalculatedObject is GPMasaListResults)
            {
                OnSaveMasaList();
            }
            else if (CurrentCalculatedObject is GPCalendarPlusEventsResults)
            {
                OnSaveCalendarPlusEvents();
            }
            else if (CurrentCalculatedObject is GPCalendarTwoLocResults)
            {
                OnSaveTwoCalendars();
            }
        }



        private void OnSaveCalendar()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText,
                FileFormatType.HtmlText, FileFormatType.HtmlTable, FileFormatType.Csv,
                FileFormatType.Ical, FileFormatType.Vcal, FileFormatType.Xml);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCalendarResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatCalendarPlain((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatCalendarRtf((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteCalendarHTML((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 4)
                    {
                        FormaterHtml.WriteCalendarHtmlTable((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 5)
                    {
                        FormaterCSV.FormatCalendarCSV((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 6)
                    {
                        FormaterICAL.FormatCalendarICAL((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 7)
                    {
                        FormaterVCAL.FormatCalendarVCAL((CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    else if (sfd.FilterIndex == 8)
                    {
                        FormaterXml.WriteXml(FormaterXml.GetCalendarXmlDocument(CurrentCalculatedObject as GPCalendarResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }

        private void OnSaveCalendarPlusEvents()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.HtmlText);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCalendarPlusEventsResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatCalendarPlusCorePlain((CurrentCalculatedObject as GPCalendarPlusEventsResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterHtml.WriteCalendarPlusCoreHTML((CurrentCalculatedObject as GPCalendarPlusEventsResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }
        private void OnSaveEvents()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText, 
                FileFormatType.HtmlText, FileFormatType.Xml);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCoreEventResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatEventsText((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatEventsRtf((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteEventsHTML((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    else if (sfd.FilterIndex == 4)
                    {
                        FormaterXml.FormatEventsXML((CurrentCalculatedObject as GPCoreEventResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }
        private void OnSaveTwoCalendars()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.HtmlText);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPCalendarTwoLocResults)
                {
                    if (sfd.FilterIndex == 1)
                    {
                        StringBuilder sb = new StringBuilder();
                        FormaterHtml.WriteCompareCalendarHTML((CurrentCalculatedObject as GPCalendarTwoLocResults), sb);
                        File.WriteAllText(sfd.FileName, sb.ToString());
                    }
                }
            }
        }
        private void OnSaveMasaList()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText, FileFormatType.HtmlText);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPMasaListResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatMasaListText((CurrentCalculatedObject as GPMasaListResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatMasaListRtf((CurrentCalculatedObject as GPMasaListResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteMasaListHTML((CurrentCalculatedObject as GPMasaListResults), sb);
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }
        private void OnSaveAppday()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = GPAppHelper.MakeFilterString(FileFormatType.PlainText, FileFormatType.RichText, FileFormatType.HtmlText, FileFormatType.Xml);
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CurrentCalculatedObject is GPAppDayResults)
                {
                    StringBuilder sb = new StringBuilder();
                    if (sfd.FilterIndex == 1)
                    {
                        FormaterPlain.FormatAppDayText((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    else if (sfd.FilterIndex == 2)
                    {
                        FormaterRtf.FormatAppDayRtf((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    else if (sfd.FilterIndex == 3)
                    {
                        FormaterHtml.WriteAppDayHTML((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    else if (sfd.FilterIndex == 4)
                    {
                        FormaterXml.FormatAppDayXML((CurrentCalculatedObject as GPAppDayResults), sb);
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                }
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Document.ExecCommand("Print", true, null);
        }

        private void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool isCalendar = (CurrentCalculatedObject is GPCalendarResults);
            if (isCalendar || CurrentCalculatedObject == null)
            {
                if (CELGenerateHtml.Template == "default:plain")
                {
                    plainTextToolStripMenuItem.Checked = true;
                    richTextToolStripMenuItem.Checked = false;
                    tableToolStripMenuItem.Checked = false;
                }
                else if (CELGenerateHtml.Template == "default:table")
                {
                    plainTextToolStripMenuItem.Checked = false;
                    richTextToolStripMenuItem.Checked = false;
                    tableToolStripMenuItem.Checked = true;
                }
                else
                {
                    plainTextToolStripMenuItem.Checked = false;
                    richTextToolStripMenuItem.Checked = true;
                    tableToolStripMenuItem.Checked = false;
                }
            }
            else
            {
                if (CELGenerateHtml.Template == "default:plain")
                {
                    plainTextToolStripMenuItem.Checked = true;
                    richTextToolStripMenuItem.Checked = false;
                    tableToolStripMenuItem.Checked = false;
                }
                else
                {
                    plainTextToolStripMenuItem.Checked = false;
                    richTextToolStripMenuItem.Checked = true;
                    tableToolStripMenuItem.Checked = false;
                }
            }

            int fsize = GPUserDefaults.IntForKey("FontSize", 9);
            textSize10ToolStripMenuItem1.Checked = (fsize == 9);
            textSize11ToolStripMenuItem1.Checked = (fsize == 10);
            textSize12ToolStripMenuItem1.Checked = (fsize == 11);
            textSize13ToolStripMenuItem1.Checked = (fsize == 12);
            textSize14ToolStripMenuItem1.Checked = (fsize == 13);

        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.AbsoluteUri;

            if (url == "about:blank")
            {
            }
            else if (url.StartsWith("http://gcal.app/"))
            {
                e.Cancel = true;
                url = url.Substring("http://gcal.app/".Length);
                if (url == "download")
                {
                    CELDownloadFile dft = new CELDownloadFile();
                    dft.Address = newVersionLink;
                    dft.Tag = "DownloadNewVersion";
                    dft.Invoke(this);
                    newVersionProgress = true;
                }
                else if (url == "downinst")
                {
                    CELDownloadFile dft = new CELDownloadFile();
                    dft.Address = newVersionLink;
                    dft.Tag = "DownloadNewVersionAndInstall";
                    dft.Invoke(this);
                    newVersionProgress = true;
                }
                else if (url == "install")
                {
                    StartInstallation();
                }
                else if (url == "nexttip")
                {
                    ShowNextTip();
                }
                else if (url == "mylocation")
                {
                    if (TodayForm.EditMyLocation())
                    {
                        CELCheckNextWeeksCalendar cew = new CELCheckNextWeeksCalendar();
                        cew.Invoke(this);
                    }
                }
            }
        }


        public void StartInstallation()
        {
            if (File.Exists(newVersionFile))
            {
                if (MessageBox.Show(GPStrings.getString(461), GPAppHelper.getShortVersionText(), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(newVersionFile);
                    Application.Exit();
                }
            }
        }

        public void ShowNextTip()
        {
            string s = GPAppHelper.NextStartupTip();
            HtmlElement elem = webBrowser1.Document.GetElementById("startuptip");
            elem.InnerHtml = s;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void showStartupTipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool curr = !GPUserDefaults.BoolForKey("app.startup.tips", true);
            GPUserDefaults.SetBoolForKey("app.startup.tips", curr);
            SetDisplayProperty("startuptipMain", (curr ? "block" : "none"));
            if (curr)
            {
                ShowNextTip();
            }
        }

        private void helpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            showStartupTipsToolStripMenuItem.Checked = GPUserDefaults.BoolForKey("app.startup.tips", true);
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        public void ShowHelp()
        {
            Help.ShowHelp(this, String.Format("file://{0}", GPFileHelper.getAppDataFile("gcal.chm")));
        }

    }
}

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
    public partial class EditRelativeEventDlg : Form
    {
        public EditRelativeEventDlg()
        {
            InitializeComponent();

            comboBox1.BeginUpdate();
            List<GPEventTithi> tithiEvents = GPEventList.getShared().tithiEvents;
            for (int i = 0; i < tithiEvents.Count; i++)
            {
                comboBox1.Items.Add(tithiEvents[i]);
            }
            List<GPEventRelative> relativeEvents = GPEventList.getShared().relativeEvents;
            for (int i = 0; i < relativeEvents.Count; i++)
            {
                comboBox1.Items.Add(relativeEvents[i]);
            }
            List<GPEventSankranti> sankrantiEvents = GPEventList.getShared().sankrantiEvents;
            for (int i = 0; i < sankrantiEvents.Count; i++)
            {
                comboBox1.Items.Add(sankrantiEvents[i]);
            }
            comboBox1.SelectedIndex = 0;
            comboBox1.EndUpdate();

            for (int i = 0; i < 17; i++)
            {
                comboBox2.Items.Add(string.Format("{0}", i - 8));
            }
            comboBox2.SelectedIndex = 0;

            for (int i = 0; i < GPFastType.count(); i++)
            {
                comboBox3.Items.Add(new GPFastType(i));
            }
            comboBox3.SelectedIndex = 0;

            for (int i = 0; i < GPEventClass.count(); i++)
            {
                comboBox4.Items.Add(new GPEventClass(i));
            }
            comboBox4.SelectedIndex = 0;

        }

        public void SetUserInterfaceStrings()
        {
            this.checkBox1.Text = GPStrings.getString(279);
            this.label7.Text = GPStrings.getString(280);
            this.label6.Text = GPStrings.getString(277);
            this.label5.Text = GPStrings.getString(276);
            this.label4.Text = GPStrings.getString(275);
            this.label3.Text = GPStrings.getString(281);
            this.label2.Text = GPStrings.getString(282);
            this.label1.Text = GPStrings.getString(278);
            this.button2.Text = GPStrings.getString(237);
            this.button1.Text = GPStrings.getString(236);
            this.Text = GPStrings.getString(271);
        }


        public string EventTitle { get { return textBox1.Text; } set { textBox1.Text = value; } }
        public string FastSubject { get { return textBox2.Text; } set { textBox2.Text = value; } }
        public string SinceYear { get { return textBox3.Text; } set { textBox3.Text = value; } }
        public int RefSpec 
        {
            get 
            {
                if (comboBox1.SelectedIndex < 0)
                    return 0;
                if (comboBox1.SelectedItem is GPEvent)
                    return (comboBox1.SelectedItem as GPEvent).nSpec;
                return 0;
            }
            set
            {
                foreach (GPEvent eve in comboBox1.Items)
                {
                    if (eve.nSpec == value)
                    {
                        comboBox1.SelectedItem = eve;
                        return;
                    }
                }
            }
        }
        public int Offset { get { return comboBox2.SelectedIndex - 8; } set { comboBox2.SelectedIndex = (value + 8); } }
        public int FastType { get { return comboBox3.SelectedIndex; } set { comboBox3.SelectedIndex = value; } }
        public int Group { get { return comboBox4.SelectedIndex; } set { comboBox4.SelectedIndex = value; } }
        public bool EventVisible { get { return checkBox1.Checked; } set { checkBox1.Checked = value; } }

    }
}

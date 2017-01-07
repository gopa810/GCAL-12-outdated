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
    public partial class SupervisorForm : Form
    {
        private static SupervisorForm sharedSupervisor = null;

        public List<Form> openForms = new List<Form>();

        public SupervisorForm()
        {
            InitializeComponent();

            sharedSupervisor = this;

            MainForm form = new MainForm();

            form.Show();

            /*if (GPDisplays.Today.VisibleAtLaunch())
            {
                TodayForm.ShowForm();
            }*/

            /*StartForm form2 = new StartForm();
            form2.Show();*/
        }

        public static void WindowOpened(Form form)
        {
            sharedSupervisor.openForms.Add(form);
        }

        public static void WindowClosed(Form form)
        {
            sharedSupervisor.openForms.Remove(form);
            if (sharedSupervisor.openForms.Count == 0)
            {
                sharedSupervisor.Close();
                Application.Exit();
            }
            else
            {
                sharedSupervisor.openForms[0].BringToFront();
                sharedSupervisor.openForms[0].Focus();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Hide();
            timer1.Stop();
            if (sharedSupervisor.openForms.Count > 0)
            {
                sharedSupervisor.openForms[0].BringToFront();
                sharedSupervisor.openForms[0].Focus();
            }
        }
    }
}

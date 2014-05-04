using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GCAL
{
    public class Supervisor
    {
        private static Supervisor p_shared = null;
        public List<Form> openForms = new List<Form>();

        public static Supervisor sharedSupervisor
        {
            get
            {
                if (p_shared == null)
                    p_shared = new Supervisor();
                return p_shared;
            }
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
                Application.Exit();
            }
            else
            {
                sharedSupervisor.openForms[0].BringToFront();
                sharedSupervisor.openForms[0].Focus();
            }
        }
    }
}

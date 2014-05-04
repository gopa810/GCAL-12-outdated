using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GCAL
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SupervisorForm svf = new SupervisorForm();
            Application.Run();
        }

    }
}

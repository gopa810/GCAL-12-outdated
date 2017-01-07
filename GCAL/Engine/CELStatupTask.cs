using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace GCAL.Engine
{
    public class CELStatupTask: CELBase
    {
        public Form Window = null;

        protected override void Execute()
        {
            if (Window == null)
                return;
            int i = 0;
            while (!Window.IsHandleCreated && i < 100)
            {
                Debugger.Log(0, "", string.Format("-{0}- step \n", i));
                System.Threading.Thread.Sleep(200);
                i++;
            }

            if (Window.IsHandleCreated)
            {
                // starting check next festivals & fasts
                CELCheckNextWeeksCalendar c1 = new CELCheckNextWeeksCalendar();
                c1.Invoke((CELAsyncResultReceiver)Window);

                // starting check new version availability
                /*CELCheckUpdates c2 = new CELCheckUpdates();
                c2.Invoke((CELAsyncResultReceiver)Window);*/

            }
        }
    }
}

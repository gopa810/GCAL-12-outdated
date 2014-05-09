using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Dialogs
{
    public interface WizardChildDelegate
    {
        void setParent(WizardDialogDelegate aDelegate);
    }
}

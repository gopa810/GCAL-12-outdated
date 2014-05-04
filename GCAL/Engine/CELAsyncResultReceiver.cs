using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Engine
{
    public delegate void CELAsyncResultReceiverTaskMethod(CELBase task);
    public delegate void CELAsyncResultReceiverTaskMethodProgress(CELBase task, double d);

    public interface CELAsyncResultReceiver
    {

        /// <summary>
        /// Called before task execution
        /// </summary>
        /// <param name="task"></param>
        void TaskStarted(CELBase task);

        /// <summary>
        /// Called after finishing task execution.
        /// </summary>
        /// <param name="task"></param>
        void TaskFinished(CELBase task);
        
        /// <summary>
        /// Reporting progress of execution
        /// </summary>
        /// <param name="task">caller task</param>
        /// <param name="progress">double value from range 0.0 - 1.0</param>
        void TaskProgress(CELBase task, double progress);

    }
}

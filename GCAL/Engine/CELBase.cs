using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;

namespace GCAL.Engine
{
    public class CELBase
    {
        private CELAsyncResultReceiver target = null;
        public delegate void ExecuteFuncDelegate();
        private object p_tag = null;


        public CELAsyncResultReceiver Target
        {
            get { return target; }
            set { target = value; }
        }

        public object Tag
        {
            get { return p_tag; }
            set { p_tag = value; }
        }

        /// <summary>
        /// This function starts async execution of task
        /// </summary>
        public void Invoke()
        {
            ExecuteFuncDelegate deleg = new ExecuteFuncDelegate(SyncExecute);

            deleg.BeginInvoke(new AsyncCallback(CallbackMethod), this);
        }

        /// <summary>
        /// This function starts async execution of task.
        /// </summary>
        /// <param name="targ">target for receiving message after completing task</param>
        public void Invoke(CELAsyncResultReceiver targ)
        {
            Target = targ;

            ExecuteFuncDelegate deleg = new ExecuteFuncDelegate(SyncExecute);

            if (targ != null)
            {
                targ.TaskStarted(this);
            }

            deleg.BeginInvoke(new AsyncCallback(CallbackMethod), this);
        }

        static void CallbackMethod(IAsyncResult result)
        {
            AsyncResult ar = (AsyncResult)result;
            ExecuteFuncDelegate caller = (ExecuteFuncDelegate)ar.AsyncDelegate;

            CELBase task = (CELBase)ar.AsyncState;

            if (task.target != null)
            {
                if (task.target is Control)
                {
                    CELAsyncResultReceiverTaskMethod method = new CELAsyncResultReceiverTaskMethod(task.target.TaskFinished);
                    Control ctrl = task.target as Control;
                    while (!ctrl.IsHandleCreated)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    ctrl.Invoke(method, task);
                }
                else
                {
                    task.target.TaskFinished(task);
                }
            }
        }

        public void SyncExecute()
        {
            Execute();
        }

        /// <summary>
        /// This method should be overriden in subclass.
        /// Performs main work of this task
        /// </summary>
        protected virtual void Execute()
        {
            throw new Exception("Method execute must be implemented in subclass.");
        }

        protected void ReportProgress(double d)
        {
            if (target != null)
            {
                if (target is Control)
                {
                    CELAsyncResultReceiverTaskMethodProgress method = new CELAsyncResultReceiverTaskMethodProgress(target.TaskProgress);
                    (target as Control).Invoke(method, this, d);
                }
                else
                {
                    target.TaskProgress(this, d);
                }
            }
        }
    }
}

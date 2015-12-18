using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;



namespace InfloCommon
{
    public abstract class BaseProcWorker : IProcessWorker
    {
        //public IUBIDiagnostics Diagnostics { get; set; }
        public bool StopProcessing { get; set; }
        public int SecondsBetweenIterations { get; set; }

        protected BaseProcWorker()
        {
            StopProcessing = false;
            SecondsBetweenIterations = 20;
        }

        public void Run()
        {
            ////If Diagnostics is still null, let's go ahead and look for the default
            //if (Diagnostics == null)
            //{
            //    Diagnostics = WorkerRole.Kernel.Get<IUBIDiagnostics>();
            //}

            while (!StopProcessing)
            {
                try
                {
                    //Do some setup
                    this.Setup();

                    //Do some work
                    this.PerformWork();

                    //Sleep for SecondsBetweenIterations seconds.
                    Thread.Sleep(1000 * SecondsBetweenIterations);
                }
                catch (Exception ex)
                {
                    string errMsg = string.Format("Error occured.  Message = {0}<br>Stack Trace = {1} ", ex.Message, ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        errMsg += string.Format("<br> InnerException Message = {0} ", ex.InnerException.Message);
                    }
                    Trace.TraceError( errMsg);
                    //Sleep a bit longer
                    Thread.Sleep(1000 * 60 * 5);
                }
            }
        }


        /// <summary>
        /// Optional setup called before each PerformWork method call
        /// </summary>
        public virtual void Setup() {}

        /// <summary>
        /// Called periodically, continuously until the process is stopped.  Iteration timing is based on the value of TimeBetweenIterations.
        /// </summary>
        public abstract void PerformWork();
    }
}

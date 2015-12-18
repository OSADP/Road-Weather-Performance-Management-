using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;
using InfloCommon.Repositories;
using InfloCommon;

namespace DataProcessorWorkerRole
{
    public class TSSProcWorker : BaseProcWorker
    {
      

        public override void PerformWork()
        {
          

            Trace.TraceInformation("TSSProcWorker called");
            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

            DataProcessing proc = WorkerRole.Kernel.Get<DataProcessing>();
            proc.tmrTSSData_Tick();

            

        }

       

       
    }
}

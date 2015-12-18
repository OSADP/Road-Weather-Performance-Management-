using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
//using Microsoft.WindowsAzure.StorageClient;
using Ninject;
using InfloCommon;
using InfloCommon.Repositories;

namespace DataProcessorWorkerRole
{
    public class CVProcWorker : BaseProcWorker
    {

        public override void PerformWork()
        {
            Trace.TraceInformation("CVProcWorker called");
            IUnitOfWork uow = WorkerRole.Kernel.Get<IUnitOfWork>();

            DataProcessing proc = WorkerRole.Kernel.Get<DataProcessing>();
            proc.tmrCVData_Tick();

            //IAzureQueue<ModuleLogQueueMessage> moduleLogQueue = WorkerRole.Kernel.Get<IAzureQueue<ModuleLogQueueMessage>>();
            //IAzureBlobContainer<string> mdfBlobContainer = WorkerRole.Kernel.Get<IAzureBlobContainer<string>>();

            //moduleLogQueue.EnsureExist();

            //CheckForAndProcessMessages(uow, moduleLogQueue, mdfBlobContainer);
        }

        

        

 

      
    }

    
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Ninject;
using InfloCommon.Repositories;

namespace DataProcessorWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        const int DefaultWorkerSleepTimeSec = 10;
        private static Microsoft.WindowsAzure.Storage.CloudStorageAccount srStorageAccount;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private CVProcManager _CVProcMgr;
        private TSSProcManager _TSSProcMgr;

        public static IKernel Kernel = new StandardKernel();

        public override void Run()
        {
            Trace.TraceInformation("DataProcessorWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            SetupIoCBindings();
            Trace.TraceInformation("DataProcessorWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("DataProcessorWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("DataProcessorWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("DataProcessorWorkerRole RunAsync called");


            bool runCVProcessor = Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("RunCVProcessor"));
            bool runTSSProcessor = Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("RunTSSProcessor"));

            if (runCVProcessor)
            {
                //Diagnostics.WriteMainDiagnosticInfo(TraceEventType.Information, TraceEventId.TraceGeneral, "Starting Module Log Processor Manager");
                _CVProcMgr = new CVProcManager();
                _CVProcMgr.ProcessWorker.SecondsBetweenIterations = GetSleepTimeForWorker("CVProcessor");
                _CVProcMgr.Start();
            }

            if (runTSSProcessor)
            {
                //Diagnostics.WriteMainDiagnosticInfo(TraceEventType.Information, TraceEventId.TraceGeneral, "Starting Trip Detail Processor Manager");
                _TSSProcMgr = new TSSProcManager();
                _TSSProcMgr.ProcessWorker.SecondsBetweenIterations = GetSleepTimeForWorker("TSSProcessor");
                _TSSProcMgr.Start();
            }


            while (!cancellationToken.IsCancellationRequested)
            {
                if (runCVProcessor && !_CVProcMgr.IsRunning())
                {
                    _CVProcMgr.Start();
                }

                if (runTSSProcessor && !_TSSProcMgr.IsRunning())
                {
                    _TSSProcMgr.Start();
                }

                await Task.Delay(10000);
            }
        }
        private int GetSleepTimeForWorker(string workerName)
        {
            try
            {
                string sleepTimeInSecondsAsAString = RoleEnvironment.GetConfigurationSettingValue(workerName + "SleepTimeSec");

                int sleepTimeInSeconds;
                if (int.TryParse(sleepTimeInSecondsAsAString, out sleepTimeInSeconds))
                {
                    return sleepTimeInSeconds;
                }
            }
            catch (Exception)
            {
                Trace.TraceError("Unable to retreive SleepTime value for worker " + workerName);
            }
            return DefaultWorkerSleepTimeSec;
        }

        private static void SetupIoCBindings()
        {
            // retrieve a reference to the messages queue
            var strStorageAccountConnectionString = CloudConfigurationManager.GetSetting("StorageAccountConnectionString");
            var LoggerLevelString = CloudConfigurationManager.GetSetting("LoggerLevel");
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(setting);
            if (strStorageAccountConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve storage account connection string");
            }
            else if (strStorageAccountConnectionString.Length <= 0)
            {
                Trace.TraceError("Storage account connection string empty");
            }
            else  //connect to the cloud storage account
            {
                Trace.TraceInformation("[INFO] Retrieved StorageAccountConnectionString for BsmWorkerRole\n{0}",
                    strStorageAccountConnectionString);

                try
                {
                    srStorageAccount = CloudStorageAccount.Parse(strStorageAccountConnectionString);
                }
                catch (Exception e)
                {
                    Trace.TraceError("Exception occurred when parsing storage account connection string\n{0}\n{1}",
                        strStorageAccountConnectionString, e.Message);
                }

                if (srStorageAccount != null)
                {


                    TableLogger.Initialize(strStorageAccountConnectionString, TableLogger.TableName, LoggerLevelString);
                }
            }
            //Get connection string
            string connectionString = RoleEnvironment.GetConfigurationSettingValue("InfloDatabaseConnectionString");

            BindUnitOfWork(connectionString);

            //Get inflo configuration xml file 
            string configXML = RoleEnvironment.GetConfigurationSettingValue("INFLOConfigFile");

            //Create processing unit as singleton
            
            //Kernel.Bind<DataProcessing>().To<DataProcessing>().InSingletonScope();
               // .WithConstructorArgument("infloConfigXml", configXML);
            //InSingletonScope doesn't actually give me just a single instance. Use ToContant instead.
            Kernel.Bind<DataProcessing>().ToConstant(new DataProcessing(configXML));

            //Kernel.Bind<DataProcessing>().ToSelf().InSingletonScope()
            // .WithConstructorArgument("infloConfigXml", configXML);

      
            

            //TimeSpan timeoutValue = new TimeSpan(0, 5, 0);

            ////This binding means that whenever Ninject encounters a dependency on IAzureQueue, it will resolve an instance of AzureQueue and inject it using the constructor arguments. 
            //Kernel.Bind<IAzureQueue<ModuleLogQueueMessage>>().To<AzureQueue<ModuleLogQueueMessage>>()
            //      .WithConstructorArgument("account", storageAccount)
            //      .WithConstructorArgument("queueName", queueName)
            //      .WithConstructorArgument("visibilityTimeOut", timeoutValue);

            //string containerName = RoleEnvironment.GetConfigurationSettingValue("ModuleLogQueueContainerName");

            ////This binding means that whenever Ninject encounters a dependency on IAzureBlobContainer, it will resolve an instance of FilesBlobContainer and inject it using the constructor arguments. 
            //Kernel.Bind<IAzureBlobContainer<string>>().To<VitalDataFileBlobContainer>()
            //      .WithConstructorArgument("account", storageAccount)
            //      .WithConstructorArgument("containerName", containerName)
            //      .WithConstructorArgument("contentType", "text/plain");
        }

        public static void BindUnitOfWork(string connectionString)
        {
            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
            Kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("connectionString", connectionString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Text;
using Ninject;
using InfloCommon.Models;
using InfloCommon.Repositories;
using RoadSegmentMapping;
using InfloCommon;
using System.Globalization;

namespace PikalertDataWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        const int DefaultWorkerSleepTimeSec = 10;

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        public static IKernel Kernel = new StandardKernel();
        private PikAlertManager _PikAlertMgr;
        private DetectorDataManager _DetectDataMgr;

        private static string osmMapDbConnectionString;
        
        IUnitOfWork uow;

        public override void Run()
        {
            Trace.TraceInformation("PikalertDataWorkerRole is running");

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

            // Load SQL Server Types for RoadSegmentMatter Distance functions.
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            uow = WorkerRole.Kernel.Get<IUnitOfWork>();
            Trace.TraceInformation("PikalertDataWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("PikalertDataWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("PikalertDataWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("DataProcessorWorkerRole RunAsync called");


            bool runPikAlertProcessor = Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("RunPikAlertProcessor"));
            bool runDetectorDataProcessor = Convert.ToBoolean(RoleEnvironment.GetConfigurationSettingValue("RunDetectorDataProcessor"));

            if (runPikAlertProcessor)
            {
                //Diagnostics.WriteMainDiagnosticInfo(TraceEventType.Information, TraceEventId.TraceGeneral, "Starting Module Log Processor Manager");
                _PikAlertMgr = new PikAlertManager();
                _PikAlertMgr.ProcessWorker.SecondsBetweenIterations = GetSleepTimeForWorker("PikAlert");
                _PikAlertMgr.Start();
            }

            if (runDetectorDataProcessor)
            {
                //Diagnostics.WriteMainDiagnosticInfo(TraceEventType.Information, TraceEventId.TraceGeneral, "Starting Trip Detail Processor Manager");
                _DetectDataMgr = new DetectorDataManager();
                _DetectDataMgr.ProcessWorker.SecondsBetweenIterations = GetSleepTimeForWorker("DetectorData");
                _DetectDataMgr.Start();
            }


            while (!cancellationToken.IsCancellationRequested)
            {
                if (runPikAlertProcessor && !_PikAlertMgr.IsRunning())
                {
                    _PikAlertMgr.Start();
                }

                if (runDetectorDataProcessor && !_DetectDataMgr.IsRunning())
                {
                    _DetectDataMgr.Start();
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

            //Get connection string
            string connectionString = RoleEnvironment.GetConfigurationSettingValue("InfloDatabaseConnectionString");

            BindUnitOfWork(connectionString);

            osmMapDbConnectionString =
   RoleEnvironment.GetConfigurationSettingValue("OsmMapModelDbConnectionString");

            if (osmMapDbConnectionString == null)
            {
                Trace.TraceError("Unable to retrieve OsmMapDbConnectionString");
                throw new Exception("PikAlert Worker Role Configuration Failure: Unable to retrieve OsmMapDbConnectionString");
            }
            else if (osmMapDbConnectionString.Length <= 0)
            {
                Trace.TraceError("OsmMapDbConnectionString empty");
                throw new Exception("PikAlert Worker Role Configuration Failure: OsmMapDbConnectionString empty");
            }
            Kernel.Bind<RoadSegmentMapper>().To<RoadSegmentMapper>().WithConstructorArgument("osmMapConnectionString", osmMapDbConnectionString);

          //  rsMapper = new RoadSegmentMapper(osmMapDbConnectionString);
        }

        public static void BindUnitOfWork(string connectionString)
        {
            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
            Kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("connectionString", connectionString);


        }
    }
}

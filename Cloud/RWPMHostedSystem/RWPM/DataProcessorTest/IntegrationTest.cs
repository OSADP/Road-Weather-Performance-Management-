using System;
using NUnit.Framework;
using DataProcessorWorkerRole;
using System.IO;
using Ninject;
using InfloCommon.Repositories;
using InfloCommon;
using INFLOClassLib;
using System.Data;
using System.Collections.Generic;
using System.Linq;


namespace DataProcessorTest
{
    [TestFixture]
    public class IntegrationTest
    {
        string strStorageAccountConnectionString = "DefaultEndpointsProtocol=http;AccountName=inflostorage;AccountKey=m3F1+Kg0N5UiaRZG8Sr6GysssCna/IxQDRXu1MuY7CbPbsuTPA54pgJu75eBd7CXQM6dg7O/5E7g/LhvthApIQ==";
        string dbConnectionString = "metadata=res://*/InfloDb.csdl|res://*/InfloDb.ssdl|res://*/InfloDb.msl;provider=System.Data.SqlClient;provider connection string='data source=hguz1d8jjq.database.windows.net,1433;initial catalog=inflo;user id=inflo;password=B@ttelle;MultipleActiveResultSets=True;App=EntityFramework'";
        //  public static IKernel Kernel = new StandardKernel();


        private void SetupDataCVI35()
        {
            UnitOfWork uow = new UnitOfWork(dbConnectionString);

            //Mimic data being sent by BSM from the cars.
           
            TME_CVData_Input input = new TME_CVData_Input();
            input.NomadicDeviceID = "5";//id of car
            input.MMLocation = 41;// 157;
            input.CVQueuedState = true;
            input.Speed = 10;
            input.DateGenerated = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
            input.RoadwayId = "I35W_N";//should match input config file
            uow.TME_CVData_Inputs.Add(input);
            uow.Commit();
        }
        private void SetupDataTSSI35()
        {
            IUnitOfWork uow = new UnitOfWork(dbConnectionString);

            uow.TME_TSSData_Inputs.RemoveRange(uow.TME_TSSData_Inputs.Where(n => n.RoadwayId == "I35W_N"));

            DateTime currLocalTime = DateTime.Now;
            DateTime DateReceived = TimeZoneInfo.ConvertTimeToUtc(currLocalTime, TimeZoneInfo.Local);


            TME_TSSData_Input input = new TME_TSSData_Input();

            input.RoadwayId = "I35W_N";//should match input config file
            input.DZId = "2132";//id of detector zone
            input.StartInterval = 0;
            input.EndInterval = 0;
            input.EndTime = DateReceived;
            input.BeginTime = DateReceived.AddSeconds(-20);
            input.IntervalLength = (short)clsGlobalVars.TSSDataLoadingFrequency;
            input.DateReceived = DateReceived;
            //Volume – Used to measure the quantity of traffic. Volume is defined as the number of vehicles observed or predicted to pass over a given point or section of a lane or roadway during a given time. Volume is typically used to track historical trends and to predict the future occurrence of congestion on specified freeway sections.
            input.Volume = 10;
            //Speed – An important measurement in determining the quality of traffic operations. Speed is frequently used to describe traffic operations because it is easy to explain and understand. Speed measurements are typically taken for individual vehicles and averaged to characterize the traffic stream as a whole. Measured speeds can be compared to optimum values to estimate the level of operations for a freeway or to detect incidents. For example, an alarm for an incident detection system might be triggered if average speeds fall below a target value.
            input.AvgSpeed = 15;
            //Occupancy – Defined as the percent of time a given section of roadway is occupied by a vehicle and can be used as a surrogate for density. Occupancy is measured using presence detectors and is easier to measure than density. Occupancy is measured on a lane-by-lane basis, with values ranging from 0 percent (no vehicles passing over a section of roadway) to 100 percent (vehicles stopped over a section of roadway).
            input.Occupancy = 10;
            input.Queued = false;
            input.Congested = false;
            if (input.AvgSpeed <= clsGlobalVars.LinkQueuedSpeedThreshold)
            {
                input.Queued = true;
            }
            else if (input.AvgSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold)
            {
                input.Congested = true;
            }

            uow.TME_TSSData_Inputs.Add(input);
            uow.Commit();
        }
        /// <summary>
        /// This Test provides a means to debug through the code, with a data input point added so that the main code pathways are exercised,
        /// and outputs generated to the database.
        /// </summary>
        [Test]
        public void TestCVI35()
        {
            string xmlConfig = "";
            using (StreamReader sr = new StreamReader(@"..\..\RWPM_InfloConfigFile.xml"))
            {
                xmlConfig = sr.ReadToEnd();
            }
            //   CloudStorageAccount  srStorageAccount = CloudStorageAccount.Parse(strStorageAccountConnectionString);
            TableLogger.Initialize(strStorageAccountConnectionString, TableLogger.TableName, "Debug");


            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
            //Kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("connectionString", connectionString);
            WorkerRole.BindUnitOfWork(dbConnectionString);

            SetupDataCVI35();

            DataProcessing dp = new DataProcessing(xmlConfig);
            dp.tmrCVData_Tick();

            Assert.AreEqual(41, clsGlobalVars.BOQMMLocation);
            Assert.AreEqual(10, clsGlobalVars.QueueSpeed);// added to db as 0 , but changes to 10 at end of DetermineBOQ function: clsGlobalVars.QueueSpeed = TotalQuedSublinksCVSpeed / TotalQuedSublinksVolume;
            Assert.AreEqual(1.0, clsGlobalVars.QueueLength);

            List<double> harmonizedspeeds = new List<double>();
            harmonizedspeeds.AddRange(dp.RSLList.Select(d => d.HarmonizedSpeed).ToList());
            //todoAssert.AreEqual(35, dp.RSLList[5].HarmonizedSpeed);
            Assert.AreEqual(0, dp.RSLList[411].HarmonizedSpeed);
            Assert.AreEqual(40, dp.RSLList[404].HarmonizedSpeed);
            Assert.AreEqual(45, dp.RSLList[400].HarmonizedSpeed);
        }
        /// <summary>
        /// This Test provides a means to debug through the code, with a data input point added so that the main code pathways are exercised,
        /// and outputs generated to the database.
        /// </summary>
        [Test]
        public void TestTSSI35()
        {
            string xmlConfig = "";
            using (StreamReader sr = new StreamReader(@"..\..\RWPM_InfloConfigFile.xml"))
            {
                xmlConfig = sr.ReadToEnd();
            }
            //   CloudStorageAccount  srStorageAccount = CloudStorageAccount.Parse(strStorageAccountConnectionString);
            TableLogger.Initialize(strStorageAccountConnectionString, TableLogger.TableName, "Debug");


            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
            //Kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("connectionString", connectionString);
            WorkerRole.BindUnitOfWork(dbConnectionString);



            DataProcessing dp = new DataProcessing(xmlConfig);
            dp.tmrCVData_Tick();//set NumberNoCVDataIntervals

            SetupDataTSSI35();
            dp.tmrTSSData_Tick();

            Assert.AreEqual(0.0, clsGlobalVars.QueueLength);

            Assert.AreEqual(17.0, clsGlobalVars.InfrastructureBOQMMLocation);
            Assert.AreEqual(15, clsGlobalVars.InfrastructureBOQLinkSpeed);

            //todoAssert.AreEqual(35, dp.RSLList[5].HarmonizedSpeed);
            Assert.AreEqual(30, dp.RSLList[8].HarmonizedSpeed);
            Assert.AreEqual(35, dp.RSLList[11].HarmonizedSpeed);
            Assert.AreEqual(30, dp.RSLList[12].HarmonizedSpeed);
        }

        #region Roadway 1000

        private void SetupDataCV1000()
        {
            UnitOfWork uow = new UnitOfWork(dbConnectionString);

            //Mimic data being sent by BSM from the cars.
            double mmLoc = 157;
            TME_CVData_Input input = new TME_CVData_Input();
            input.NomadicDeviceID = "5";//id of car
            input.MMLocation = 165;// 157;
            input.CVQueuedState = true;
            input.Speed = 10;
            input.DateGenerated = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
            input.RoadwayId = "1000";//should match input config file
            uow.TME_CVData_Inputs.Add(input);
            uow.Commit();
        }
        private void SetupDataTSS1000()
        {
            IUnitOfWork uow = new UnitOfWork(dbConnectionString);

    //List<TME_TSSData_Input> toremove = uow.TME_TSSData_Inputs.Where(n => n.RoadwayId == "1000").ToList();
    //foreach (var t in toremove)
    //{
    //    uow.TME_TSSData_Inputs.Remove(t);
    //}
    uow.TME_TSSData_Inputs.RemoveRange(uow.TME_TSSData_Inputs.Where(n => n.RoadwayId == "1000"));

            DateTime currLocalTime = DateTime.Now;
            DateTime DateReceived = TimeZoneInfo.ConvertTimeToUtc(currLocalTime, TimeZoneInfo.Local);

       
            TME_TSSData_Input input = new TME_TSSData_Input();
            
            input.RoadwayId = "1000";//should match input config file
            input.DZId = "005es15920:_MN_Stn";//id of detector zone
            input.StartInterval = 0;
            input.EndInterval = 0;
            input.EndTime = DateReceived;
            input.BeginTime = DateReceived.AddSeconds(-20);
            input.IntervalLength = (short)clsGlobalVars.TSSDataLoadingFrequency;
            input.DateReceived = DateReceived;
            //Volume – Used to measure the quantity of traffic. Volume is defined as the number of vehicles observed or predicted to pass over a given point or section of a lane or roadway during a given time. Volume is typically used to track historical trends and to predict the future occurrence of congestion on specified freeway sections.
            input.Volume = 10;
            //Speed – An important measurement in determining the quality of traffic operations. Speed is frequently used to describe traffic operations because it is easy to explain and understand. Speed measurements are typically taken for individual vehicles and averaged to characterize the traffic stream as a whole. Measured speeds can be compared to optimum values to estimate the level of operations for a freeway or to detect incidents. For example, an alarm for an incident detection system might be triggered if average speeds fall below a target value.
            input.AvgSpeed = 15;
            //Occupancy – Defined as the percent of time a given section of roadway is occupied by a vehicle and can be used as a surrogate for density. Occupancy is measured using presence detectors and is easier to measure than density. Occupancy is measured on a lane-by-lane basis, with values ranging from 0 percent (no vehicles passing over a section of roadway) to 100 percent (vehicles stopped over a section of roadway).
            input.Occupancy = 10;
            input.Queued = false;
            input.Congested = false;
            if (input.AvgSpeed <= clsGlobalVars.LinkQueuedSpeedThreshold)
            {
                input.Queued = true;
            }
            else if (input.AvgSpeed <= clsGlobalVars.LinkCongestedSpeedThreshold)
            {
                input.Congested = true;
            }
            
            uow.TME_TSSData_Inputs.Add(input);
            uow.Commit();
        }
        /// <summary>
        /// This Test provides a means to debug through the code, with a data input point added so that the main code pathways are exercised,
        /// and outputs generated to the database.
        /// </summary>
        [Test]
        public void TestCV1000()
        {
            string xmlConfig = "";
            using (StreamReader sr = new StreamReader(@"..\..\Config1.xml"))
            {
                xmlConfig = sr.ReadToEnd();
            }
            //   CloudStorageAccount  srStorageAccount = CloudStorageAccount.Parse(strStorageAccountConnectionString);
            TableLogger.Initialize(strStorageAccountConnectionString, TableLogger.TableName, "Debug");


            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
            //Kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("connectionString", connectionString);
            WorkerRole.BindUnitOfWork(dbConnectionString);

            SetupDataCV1000();

            DataProcessing dp = new DataProcessing(xmlConfig);
            dp.tmrCVData_Tick();

            Assert.AreEqual(165, clsGlobalVars.BOQMMLocation);
            Assert.AreEqual(10, clsGlobalVars.QueueSpeed);// added to db as 0 , but changes to 10 at end of DetermineBOQ function: clsGlobalVars.QueueSpeed = TotalQuedSublinksCVSpeed / TotalQuedSublinksVolume;
            Assert.AreEqual(1.0, clsGlobalVars.QueueLength);

            Assert.AreEqual(35, dp.RSLList[5].HarmonizedSpeed);
            Assert.AreEqual(30, dp.RSLList[8].HarmonizedSpeed);
            Assert.AreEqual(35, dp.RSLList[11].HarmonizedSpeed);
            Assert.AreEqual(30, dp.RSLList[12].HarmonizedSpeed);
        }
        /// <summary>
        /// This Test provides a means to debug through the code, with a data input point added so that the main code pathways are exercised,
        /// and outputs generated to the database.
        /// </summary>
        [Test]
        public void TestTSS1000()
        {
            string xmlConfig = "";
            using (StreamReader sr = new StreamReader(@"..\..\Config1.xml"))
            {
                xmlConfig = sr.ReadToEnd();
            }
            //   CloudStorageAccount  srStorageAccount = CloudStorageAccount.Parse(strStorageAccountConnectionString);
            TableLogger.Initialize(strStorageAccountConnectionString, TableLogger.TableName, "Debug");


            //This binding means that whenever Ninject encounters a dependency on IUnitOfWork, it will resolve an instance of UnitOfWork and inject it using the connection string as an arg. 
            //Kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("connectionString", connectionString);
            WorkerRole.BindUnitOfWork(dbConnectionString);

           

            DataProcessing dp = new DataProcessing(xmlConfig);
            dp.tmrCVData_Tick();//set NumberNoCVDataIntervals

            SetupDataTSS1000();
            dp.tmrTSSData_Tick();

            Assert.AreEqual(0.0, clsGlobalVars.QueueLength);

            Assert.AreEqual(159.2, clsGlobalVars.InfrastructureBOQMMLocation);
            Assert.AreEqual(15, clsGlobalVars.InfrastructureBOQLinkSpeed);

            Assert.AreEqual(35, dp.RSLList[5].HarmonizedSpeed);
            Assert.AreEqual(30, dp.RSLList[8].HarmonizedSpeed);
            Assert.AreEqual(35, dp.RSLList[11].HarmonizedSpeed);
            Assert.AreEqual(30, dp.RSLList[12].HarmonizedSpeed);
        }
        #endregion
    }
}

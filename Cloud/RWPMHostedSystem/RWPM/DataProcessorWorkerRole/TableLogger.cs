
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataProcessorWorkerRole
{
    public class TableLogger
    {
        public static string TableName = "DataProcessorLog";
        private static CloudTableClient cloudTableClient;
        private static CloudTable cloudTable = null;

        private static LoggingTableEntity tableEntry = new LoggingTableEntity();

        private static LoggerLevel selectedLoggingLevel;
        public static Microsoft.WindowsAzure.Storage.CloudStorageAccount srStorageAccount;

        public static void Initialize(string storageAccountString, string tableName, string loggerLevel)
        {

            try
            {
                srStorageAccount = CloudStorageAccount.Parse(storageAccountString);
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception occurred when parsing storage account connection string\n{0}\n{1}",
                    storageAccountString, e.Message);
                return;
            }


            cloudTableClient = srStorageAccount.CreateCloudTableClient();
            cloudTable = cloudTableClient.GetTableReference(tableName);

            try
            {
                if (cloudTable.CreateIfNotExists())
                {
                    Trace.TraceInformation("Created Azure Table '{0}'", tableName);
                }
                else
                {
                    Trace.TraceInformation("Got reference to existing Table '{0}'", tableName);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Exception occurred when creating Table\n{0}",
                    e.Message);

                cloudTable = null;
            }

            if(!Enum.TryParse(loggerLevel,true, out selectedLoggingLevel))
            {
                Trace.TraceError("Logging level specified in config invalid ("+loggerLevel+"). Defaulting to Debug");
                //Setting from config file failed, default to all
                selectedLoggingLevel = LoggerLevel.Debug;
            }
            
        }

 

        public static void SubmitLogEntry(LoggingTableEntity entry)
        {
            if (cloudTable != null)
            {
                //Only log messages greater than or equal to the selected level of detail.
                if (entry.Level >= selectedLoggingLevel)
                {
                    cloudTable.ExecuteAsync(TableOperation.Insert(entry));
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessorWorkerRole
{

    /// <summary>
    /// Consolidated diagnostics written to the GUI and to text log files.
    /// </summary>
    public enum LoggerType
    {
        Other = 0,   //previously txtINFLOLog  . plus used for functions that are called by both TSS and CV.
        Init = 1,  //previously txtINFLOLog
        CV,        //previously txtCVDataLog
        TSS,       //previously txtTSSDataLog
        SpdHarm,   //previously txtSpdHarmLog
        Config,     //txtINFLOConfigLog
        SublinkDataLog,  //text file SublinkDataLog
        QueueLog, //text file QueueLog
        FillDataSetLog,//text file FillDataSetLog
        CVDataLog  //text file CVDataLog  \\CVDataFile-" + CVDataFileCounter.ToString();  File per time the CV timer runs?

    }
    public enum LoggerLevel
    {
        Debug = 0, //ported from hassan's code, Debug lines only printed to the GUI display.
        Info = 1,  //ported from hassan's code, Info lines printed to the GUI display and the TXT log file in the exe location.
        Warning = 2,
        Error = 3,
        Off=4

    }
    public class LoggingTableEntity : Microsoft.WindowsAzure.Storage.Table.TableEntity
    {
    //    public DateTime Date { get;  set; }
        public LoggerType Type { get; set; }//enum type wont show up in azure.
        public LoggerLevel Level { get; set; }//enum type wont show up in azure.
        public string Severity { get; set; }
        public string Message { get; set; }
        public  LoggingTableEntity()
        {
        }
        public LoggingTableEntity(DateTime date, LoggerType type,LoggerLevel level,string message)
        {
          //  Date = date.ToUniversalTime(); //Timestamp automatically logged by Azure table
          //  Type = type;
            Level = level;
            Severity = Enum.GetName(typeof(LoggerLevel), level);
            Message = message;

            this.PartitionKey = Enum.GetName(typeof(LoggerType),Type);
            this.RowKey = Guid.NewGuid().ToString();
        }
    }
}

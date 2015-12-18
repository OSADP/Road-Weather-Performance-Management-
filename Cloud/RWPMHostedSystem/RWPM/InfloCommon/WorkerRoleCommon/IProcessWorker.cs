using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace InfloCommon
{
    public interface IProcessWorker
    {
        //IUBIDiagnostics Diagnostics { get; set; } 

        bool StopProcessing { get; set; }
        int SecondsBetweenIterations { get; set; }

        void Run();


    }
}

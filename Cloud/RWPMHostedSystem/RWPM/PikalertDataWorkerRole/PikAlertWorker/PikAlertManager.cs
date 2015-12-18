using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InfloCommon;


namespace PikalertDataWorkerRole
{
    public class PikAlertManager : BaseProcManager
    {
        public PikAlertManager()
            : base(new PikAlertWorker())
        {}
    }
}

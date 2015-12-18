using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InfloCommon;

namespace DataProcessorWorkerRole
{
    class TSSProcManager : BaseProcManager
    {
        public TSSProcManager()
            : base(new TSSProcWorker())
        {

        }
    }
}

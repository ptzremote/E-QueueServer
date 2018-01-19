using System;
using System.Collections.Generic;

namespace EQueueLib
{
    [Serializable]
    public class QueueState
    {
        public List<OperatorInfo> OperatorList { get; set; }
        public List<ServiceInfo> ServiceList { get; set; }
        public List<QueueClientInfo> ClientInQueue { get; set; }
    }
}

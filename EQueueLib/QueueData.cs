using System;
using System.Collections.Generic;

namespace EQueueLib
{
    [Serializable]
    public class QueueData
    {
        public QueueData()
        {

        }
        public Command Cmd { get; set; }
        public QueueClientInfo ClientInfo { get; set; }
        public int WindowNumber { get; set; }
        public ServiceInfo SelectedService { get; set; }
        public OperatorInfo SelectedOperator { get; set; }
        public List<ServiceInfo> ServiceList { get; set; }
        public QueueState QueueState { get; set; }
    }
}

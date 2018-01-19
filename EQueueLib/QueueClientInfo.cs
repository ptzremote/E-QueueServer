using System;

namespace EQueueLib
{
    [Serializable]
    public class QueueClientInfo : IEquatable<QueueClientInfo>
    {
        //TODO: change to private, remember about JSON serializer
        public QueueClientInfo()
        {

        }
        public int Id { get;  set; }
        public DateTime EnqueueTime { get;  set; }
        public int ClientNumber { get;  set; }
        public DateTime? DequeueTime { get;  set; }
        public DateTime? CompleteServiceTime { get;  set; }
        public int WindowNumber { get;  set; }
        public ServiceInfo ServiceInfo { get;  set; }
        public OperatorInfo OperatorInfo { get;  set; }

        bool IEquatable<QueueClientInfo>.Equals(QueueClientInfo other)
        {
            if (Id == other.Id)
                return true;
            return false;
        }

        public static QueueClientInfo CreateQueueClientInfo(int clientNumber, ServiceInfo serviceInfo)
        {
            return new QueueClientInfo
            {
                ClientNumber = clientNumber,
                ServiceInfo = serviceInfo,
                EnqueueTime = DateTime.Now
            };
        }
    }
}

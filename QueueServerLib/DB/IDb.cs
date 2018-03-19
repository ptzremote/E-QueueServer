using EQueueLib;
using System.Collections.Generic;

namespace QueueServerLib
{
    public interface IDb
    {
        List<OperatorInfo> GetOperatorList();
        List<ServiceInfo> GetServiceList();
        List<QueueClientInfo> GetClientInQueue();
        QueueClientInfo GetClientById(int id);
        ServiceInfo GetServiceById(int id);
        OperatorInfo GetOperatorById(int id);
        QueueClientInfo GetLastClient();
        QueueClientInfo GetLastDequeueClient();
        void AddNewClient(QueueClientInfo newClient);
        void SaveChanges();
        QueueClientInfo SetCompleteServiceInfo(int clientId, int operatorId);
    }
}

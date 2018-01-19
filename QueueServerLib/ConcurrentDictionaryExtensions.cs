using EQueueLib;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QueueServerLib
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void AddOrUpdateNewClient(this ConcurrentDictionary<ServiceInfo, List<QueueClientInfo>> clientInQueue, QueueClientInfo clientInfo)
        {

            clientInQueue.AddOrUpdate(clientInfo.ServiceInfo, AddFactory, UpdateFactory);

            List<QueueClientInfo> AddFactory(ServiceInfo service)
            {
                return new List<QueueClientInfo> { clientInfo };
            }

            List<QueueClientInfo> UpdateFactory(ServiceInfo service, List<QueueClientInfo> clients)
            {
                clients.Add(clientInfo);
                return clients;
            }
        }
    }
}

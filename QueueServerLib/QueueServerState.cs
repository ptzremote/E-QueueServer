using CoreNetLib;
using Microsoft.Extensions.Logging;
using EQueueLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace QueueServerLib
{
    class QueueServerState
    {
        ILogger Logger { get; } =
            CoreNetLogging.LoggerFactory.CreateLogger<QueueServerState>();
        public int MaxQueueClientCount { get; set; } = 100;
        public int CurentClientNumber { get; private set; }
        public ConcurrentDictionary<ServiceInfo, List<QueueClientInfo>> ClientInQueue { get; set; } = new ConcurrentDictionary<ServiceInfo, List<QueueClientInfo>>();
        public List<KeyValuePair<int, List<ServiceInfo>>> FreeWindow { get; set; } = new List<KeyValuePair<int, List<ServiceInfo>>>();

        Func<QueueClientInfo> lastClientFromDb;

        public QueueServerState(Func<QueueClientInfo> lastClient)
        {
            lastClientFromDb = lastClient;
            if (lastClientFromDb() != null && lastClientFromDb().EnqueueTime.Day <= DateTime.Now.Day)
            {
                CurentClientNumber = lastClientFromDb().ClientNumber;
            }
            else
                CurentClientNumber = 0;
        }
        public bool TryGetNextClient(out QueueClientInfo nextClient)
        {
            nextClient = null;
            if (FreeWindow.Count == 0)
                return false;

            foreach (var curentWindow in FreeWindow)
            {

                var result = from windowService in curentWindow.Value
                             join service in ClientInQueue on windowService equals service.Key
                             select new { curentWindow.Key, service.Value };

                if (result.Count() <= 0)
                {
                    continue;
                }
                else
                {
                    List<QueueClientInfo> clients = new List<QueueClientInfo>();
                    foreach (var item in result)
                    {
                        clients.AddRange(item.Value);
                    }

                    if (clients.Count <= 0)
                        continue;
                    clients = new List<QueueClientInfo>
                        (clients.OrderBy(t => t.EnqueueTime));

                    nextClient = NextClientHandler(clients.First(),curentWindow.Key);
                    return true;
                }
            }
            return false;
        }

        QueueClientInfo NextClientHandler(QueueClientInfo nextClient, int window)
        {
            Logger.LogInformation($"Next client number {nextClient.ClientNumber}" +
                $" window number: {window}");

            FreeWindow.Remove(FreeWindow.Where(kp => kp.Key == window).FirstOrDefault());

            ClientInQueue.Where(x => x.Key.Id == nextClient.ServiceInfo.Id).
                FirstOrDefault().Value.Remove(nextClient);

            nextClient.DequeueTime = DateTime.Now;
            nextClient.WindowNumber = window;

            return nextClient;
        }

        public void UpdateCurrentClientNumber()
        {
                if (CurentClientNumberCanBeIncrement())
                    CurentClientNumber++;
                else
                    CurentClientNumber = 1;

            bool CurentClientNumberCanBeIncrement()
            {
                return CurentClientNumber != 0 &&
                       CurentClientNumber < MaxQueueClientCount &&
                       lastClientFromDb() != null &&
                       lastClientFromDb().EnqueueTime.Day <= DateTime.Now.Day;
            }
        }
    }
}

using EQueueLib;

namespace QueueServerLib
{
    static class QueueDataFactory
    {
        internal static QueueData GetServerStateData()
        {
            return new QueueData
            {
                Cmd = Command.ServerState,
            };
        }

        internal static QueueData GetCompleteClientData(QueueClientInfo clientInfo)
        {
            return new QueueData
            {
                Cmd = Command.CompleteClientService,
                ClientInfo = clientInfo,
            };
        }

        internal static QueueData GetNewClientData(QueueClientInfo clientNInfo)
        {
            return new QueueData
            {
                Cmd = Command.NewClient,
                ClientInfo = clientNInfo
            };
        }

        internal static QueueData GetNextClientData(QueueClientInfo client)
        {
            return new QueueData
            {
                Cmd = Command.NextClient,
                ClientInfo = client,
                WindowNumber = client.WindowNumber,
                SelectedService = client.ServiceInfo,
            };
        }
    }
}

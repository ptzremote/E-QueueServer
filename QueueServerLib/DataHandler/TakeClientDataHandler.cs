using CoreNetLib;
using Microsoft.Extensions.Logging;
using EQueueLib;
using System;

namespace QueueServerLib
{
    class TakeClientDataHandler : DataHandler
    {
        QueueData qData;

        ILogger Logger { get; } =
    CoreNetLogging.LoggerFactory.CreateLogger<QueueServer>();
        public TakeClientDataHandler(QueueData qData)
        {
            this.qData = qData;
        }

        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
            QueueClientInfo takenClient = null;

            foreach (var clientsList in qsState.ClientInQueue.Values)
            {
                takenClient = clientsList.
                    Find(x => x.Id == qData.ClientInfo.Id);

                if (takenClient != null)
                {
                    clientsList.Remove(takenClient);
                    break;
                }
            }

            if (takenClient == null)
            {
                Logger.LogInformation($"Can`t take client with number {qData.ClientInfo.ClientNumber}. " +
                    $"No such client number in queue.");
                return null;
            }
            else
            {
                var takenClientInDb = db.GetClientById(qData.ClientInfo.Id);

                if (takenClientInDb != null)
                {
                    takenClientInDb.WindowNumber = qData.WindowNumber;
                    takenClientInDb.DequeueTime = DateTime.Now;
                }

                Logger.LogInformation($"Client number {takenClientInDb.ClientNumber} taken out of order");

                return QueueDataFactory.GetNextClientData(takenClientInDb);
            }
        }
    }
}

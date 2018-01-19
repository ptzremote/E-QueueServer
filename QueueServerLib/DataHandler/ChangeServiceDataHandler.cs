using EQueueLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueueServerLib
{
    class ChangeServiceDataHandler : DataHandler
    {
        QueueData qData;
        public ChangeServiceDataHandler(QueueData qData)
        {
            this.qData = qData;
        }
        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
            QueueClientInfo client = QueueClientInfo.CreateQueueClientInfo(qData.ClientInfo.ClientNumber, qData.SelectedService);

            //save enqueue time for this client, it must dequeue as soon as possible
            client.EnqueueTime = qData.ClientInfo.EnqueueTime;

            if (qsState.ClientInQueue.ContainsKey(qData.SelectedService))
            {
                qsState.ClientInQueue[qData.SelectedService].Insert(0, client);
            }
            else
            {
                qsState.ClientInQueue.
                    AddOrUpdate(
                    qData.SelectedService,
                    new List<QueueClientInfo> { client },
                    (key, value) => new List<QueueClientInfo> { client }
                    );
            }

            db.AddNewClient(client);

            return null;
        }
    }
}

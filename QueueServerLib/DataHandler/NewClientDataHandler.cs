using EQueueLib;
using System;
using System.Threading;

namespace QueueServerLib
{
    class NewClientDataHandler : DataHandler
    {
        QueueData qData;
        public NewClientDataHandler(QueueData qData)
        {
            this.qData = qData;
        }
        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
            qsState.UpdateCurrentClientNumber();

            var newClient = QueueClientInfo.CreateQueueClientInfo(qsState.CurentClientNumber, qData.SelectedService);

            db.AddNewClient(newClient);

            qsState.ClientInQueue.AddOrUpdateNewClient(newClient);

            return QueueDataFactory.GetNewClientData(newClient);


        }
    }
}

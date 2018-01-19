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
            SetCurrentClientNumber();

            var newClient = QueueClientInfo.CreateQueueClientInfo(qsState.CurentClientNumber, qData.SelectedService);

            db.AddNewClient(newClient);

            qsState.ClientInQueue.AddOrUpdateNewClient(newClient);

            return QueueDataFactory.GetNewClientData(newClient);

            void SetCurrentClientNumber()
            {
                if (CurentClientNumberCanBeIncrement())
                    qsState.CurentClientNumber++;
                else
                    qsState.CurentClientNumber = 1;
            }

            bool CurentClientNumberCanBeIncrement()
            {
                return qsState.CurentClientNumber != 0 &&
                       qsState.CurentClientNumber < qsState.MaxQueueClientCount &&
                       db.GetLastClient().EnqueueTime.Day <= DateTime.Now.Day;
            }
        }
    }
}

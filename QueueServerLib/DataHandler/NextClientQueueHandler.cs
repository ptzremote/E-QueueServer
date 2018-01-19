using EQueueLib;

namespace QueueServerLib
{
    internal class NextClientQueueHandler : DataHandler
    {
        QueueData qData;
        public NextClientQueueHandler(QueueData qData)
        {
            this.qData = qData;
        }
        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
            if (qData.ClientInfo != null)
            {
                QueueClientInfo client = db.GetClientById(qData.ClientInfo.Id);
                return QueueDataFactory.GetNextClientData(client);
            }
            return null;
        }
    }
}
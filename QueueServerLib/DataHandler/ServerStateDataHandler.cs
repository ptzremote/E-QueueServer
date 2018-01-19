using EQueueLib;

namespace QueueServerLib
{
    class ServerStateDataHandler : DataHandler
    {
        QueueData qData;
        public ServerStateDataHandler(QueueData qData)
        {
            this.qData = qData;
        }
        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
           return QueueDataFactory.GetServerStateData();
        }
    }
}

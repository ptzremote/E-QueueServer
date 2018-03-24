namespace QueueServerLib
{
    public class QueueServerHandler
    {
        IDb db;
        QueueServerState qsState;

        internal QueueServerHandler(IDb db, QueueServerState qsState)
        {
            this.db = db;
            this.qsState = qsState;
        }
        public void DeleteClientInfo(int clientId)
        {
            db.DeleteClientInfo(clientId);

            foreach (var clientList in qsState.ClientInQueue.Values)
            {
                var client = clientList.Find(c => c.Id == clientId);
                if (client != null)
                {
                    clientList.Remove(client);
                    break;
                }
            } 
        }
    }
}
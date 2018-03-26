using EQueueLib;
using System;

namespace QueueServerLib
{
    public class QueueServerHandler
    {
        IDb db;
        internal QueueServerState qsState;

        public event EventHandler OnStateChange;

        internal QueueServerHandler(IDb db)
        {
            this.db = db;
            qsState = new QueueServerState(db.GetLastClient);
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

        public void StateChanged()
        {
            OnStateChange?.Invoke(null, EventArgs.Empty);
        }

        internal QueueState GetQueueState()
        {
            return new QueueState
            {
                ServiceList = db.GetServiceList(),
                OperatorList = db.GetOperatorList(),
                ClientInQueue = db.GetClientInQueue()
            };
        }
    }
}
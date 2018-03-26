using System;

namespace QueueServerLib
{
    internal interface IQueueConnectionAdapter
    {
        void SendDataToAll(object data);

        event QueueConnectionHandler OnDataReceived;

        void HandleData(IDb db, EventArgs e, IQueueConnectionAdapter conn);

        void Start();
    }

    internal delegate void QueueConnectionHandler(IQueueConnectionAdapter sender, EventArgs e);
}
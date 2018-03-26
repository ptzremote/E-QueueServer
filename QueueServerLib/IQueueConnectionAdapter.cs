using System;

namespace QueueServerLib
{
    internal interface IQueueConnectionAdapter
    {
        void SendDataToAll(object data);

        event QueueConnectionHandler OnDataReceived;

        void HandleData(IDb db, EventArgs e);

        void Start();
    }

    internal delegate void QueueConnectionHandler(IQueueConnectionAdapter sender, EventArgs e);
}
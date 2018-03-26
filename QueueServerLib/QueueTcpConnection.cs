using CoreNetLib;
using EQueueLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueueServerLib
{
    class QueueTcpConnection : IQueueConnectionAdapter
    {
        internal IHub hub;
        internal QueueServerHandler qsHandler;
        ILogger Logger { get; } =
            CoreNetLogging.LoggerFactory.CreateLogger<QueueTcpConnection>();

        public event QueueConnectionHandler OnDataReceived;

        public QueueTcpConnection(QueueServerHandler qsHandler)
            : this()
        {
            this.qsHandler = qsHandler;
        }
        private QueueTcpConnection()
        {
            var netSettings = new NetSettings
            {
                serializer = JSONSerializerHelper.Serialize,
                deserializer = JSONSerializerHelper.Deserialize
            };

            hub = new TcpHub(netSettings);
            hub.OnDataReceived += Hub_OnDataReceived;
        }

        private void Hub_OnDataReceived(object sender, ReceivedDataEventArgs e)
        {
            OnDataReceived?.Invoke(this, e);
        }

        public void Start()
        {
            hub.Start();
        }
        public void HandleData(IDb db, EventArgs e, IQueueConnectionAdapter conn)
        {
            if (!(conn is QueueTcpConnection))
                return;

            var receivedDataArgs = (ReceivedDataEventArgs)e;
            var receivedQueueData = (QueueData)receivedDataArgs.Data;

            //TODO: change to interlock
            lock (this)
            {
                Logger.LogTrace($"Command {receivedQueueData.Cmd} received.");
                var handler = DataHandlerFactory.MakeHandler(receivedQueueData);
                var handledData = handler.Handle(qsHandler.qsState, db);

                db.SaveChanges();

                if (qsHandler.qsState.TryGetNextClient(out QueueClientInfo nextClient))
                {
                    var nextClientData = QueueDataFactory.GetNextClientData(nextClient);
                    using (var qDb = new QueueDBContext())
                    {
                        var cClient = qDb.QueueClientInfo.Find(nextClient.Id);
                        cClient.DequeueTime = nextClient.DequeueTime;
                        cClient.WindowNumber = nextClient.WindowNumber;
                        qDb.SaveChanges();
                    }
                    db.SaveChanges();

                    SendDataToAll(nextClientData);
                }

                if (handledData != null)
                {
                    SendDataToAll(handledData);
                }
            }
        }

        void SendDataToAll(object data)
        {
            var qData = (QueueData)data;
            if (qData != null)
            {
                qData.QueueState = qsHandler.GetQueueState();
                hub.SendToAll(qData);
                qsHandler.StateChanged();
            }
        }

        void IQueueConnectionAdapter.SendDataToAll(object data)
        {
            SendDataToAll(data);
        }
    }
}

using CoreNetLib;
using EQueueLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace QueueServerLib
{
    public class QueueServer
    {
        internal IDb db;
        public QueueServerHandler qsHandler;
        internal List<IQueueConnectionAdapter> connAdapter;

        public QueueServer()
            : this(new QueueDB())
        {

        }

        public QueueServer(IDb db)
        {
            this.db = db;
            qsHandler = new QueueServerHandler(db);
            connAdapter = new List<IQueueConnectionAdapter>();
        }

        public void UseTcp()
        {
            connAdapter.Add(new QueueTcpConnection(qsHandler));
        }

        private void Conn_OnDataReceived(IQueueConnectionAdapter sender, EventArgs e)
        {
            sender.HandleData(db, e);
        }

        public void Run()
        {
            foreach (var conn in connAdapter)
            {
                conn.Start();
                conn.OnDataReceived += Conn_OnDataReceived;
            }
        }

        public void Restart()
        {
            qsHandler.qsState.ClientInQueue.Clear();
            qsHandler.qsState.FreeWindow.Clear();
            QueueInitialize();
            foreach (var conn in connAdapter)
            {
                conn.SendDataToAll(new QueueData { Cmd = Command.ServerState });
            }
        }
        public void QueueInitialize()
        {
            foreach (var clientInfo in db.GetClientInQueue())
            {
                qsHandler.qsState.ClientInQueue.AddOrUpdateNewClient(clientInfo);
            }

            var lastClient = db.GetLastClient();

            if (lastClient != null)
            {
                qsHandler.qsState.CurentClientNumber = lastClient.ClientNumber;
            }
            else
                qsHandler.qsState.CurentClientNumber = 0;
        }
    }
}

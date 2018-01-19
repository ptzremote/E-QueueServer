using System;
using EQueueLib;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CoreNetLib;

namespace QueueClientLib
{
    public class QueueClient
    {
        NetClient client;

        QueueState serverState;
        QueueClientInfo clientInfo;
        ConcurrentQueue<QueueClientInfo> nextClientList;

        public event EventHandler OnNextClient;
        public event EventHandler OnCompleteClient;
        public event EventHandler<DisconnectEventArgs> OnDisconnected;
        public event EventHandler<QueueStateEventArgs> OnQueueStateChanged;

        public QueueClient()
        {
            client = new NetClient(new NetSettings
            {
                serializer = JSONSerializerHelper.Serialize,
                deserializer = JSONSerializerHelper.Deserialize
            });
            nextClientList = new ConcurrentQueue<QueueClientInfo>();

            client.OnDataReceived += Client_OnDataReceived;
            client.OnDisconnected += Client_OnDisconnected;
        }

        public void Connect(string host, int port)
        {
            client.Connect(host, port);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        private void Client_OnDisconnected(object sender, DisconnectEventArgs e)
        {
            OnDisconnected?.Invoke(null, e);
        }

        private void Client_OnDataReceived(object sender, ReceivedDataEventArgs e)
        {
            var receivedQueueData = (QueueData)e.Data;

            //QueueServer always send actual queue state
            OnQueueStateChanged?.Invoke(null, new QueueStateEventArgs { QueueState = receivedQueueData.QueueState });

            switch (receivedQueueData.Cmd)
            {
                case Command.NewClient:
                    clientInfo = receivedQueueData.ClientInfo;
                    break;
                case Command.NextClient:
                    NextClientHandler(receivedQueueData);
                    break;
                case Command.ServerState:
                    serverState = receivedQueueData.QueueState;
                    break;
                case Command.CompleteClientService:
                    CompleteServiceHandler(receivedQueueData);
                    break;
                default:
                    break;
            }
        }

        private void CompleteServiceHandler(QueueData qData)
        {
            OnCompleteClient?.Invoke(null, new ClientInfoEventArgs { ClientInfo = qData.ClientInfo });
        }

        private void NextClientHandler(QueueData qData)
        {
            nextClientList.Enqueue(qData.ClientInfo);

            OnNextClient?.Invoke(null, null);
        }

        #region QueueClient API
        public bool TryGetNextClient(out QueueClientInfo nextClientInfo)
        {
            if (nextClientList.Count > 0)
            {
                nextClientList.TryDequeue(out QueueClientInfo clientInfo);
                nextClientInfo = clientInfo;
                return true;
            }
            nextClientInfo = null;
            return false;
        }

        public void FreeWindow(int windowNumber, List<ServiceInfo> serviceList)
        {
            QueueData qData = new QueueData
            {
                Cmd = Command.FreeWindow,
                WindowNumber = windowNumber,
                ServiceList = serviceList
            };
            client.SendAsync(qData);
        }

        public void FreeWindowForClient(int windowNumber, QueueClientInfo clientInfo)
        {
            QueueData qData = new QueueData
            {
                Cmd = Command.TakeClient,
                WindowNumber = windowNumber,
                ClientInfo = clientInfo
            };
            client.SendAsync(qData);
        }

        public void BusyWindow(int windowNumber)
        {
            QueueData qData = new QueueData
            {
                Cmd = Command.FreeWindow,
                WindowNumber = windowNumber,
            };
            client.SendAsync(qData);
        }

        public void ChangeService(QueueClientInfo clientInfo, ServiceInfo service)
        {
            QueueData qData = new QueueData
            {
                Cmd = Command.ChangeService,
                ClientInfo = clientInfo,
                SelectedService = service
            };
            client.SendAsync(qData);
        }

        public void RepeateNextClient(QueueClientInfo clientInfo)
        {
            if (!client.Connected)
                throw new Exception("client not connected!");

            QueueData qData = new QueueData
            {
                Cmd = Command.NextClient,
                ClientInfo = clientInfo,
            };
            client.SendAsync(qData);
        }

        public void CompleteServiceForClient(QueueClientInfo clientInfo, OperatorInfo operatorInfo)
        {
            QueueData qData = new QueueData
            {
                Cmd = Command.CompleteClientService,
                ClientInfo = clientInfo,
                SelectedOperator = operatorInfo
            };
            client.SendAsync(qData);
        }

        public Task<QueueClientInfo> NewClientAsync(ServiceInfo serviceInfo)
        {
            var cts = new CancellationTokenSource();
            clientInfo = null;

            cts.CancelAfter(2000);
            client.SendAsync(
                new QueueData
                {
                    Cmd = Command.NewClient,
                    SelectedService = serviceInfo
                });

            return Task.Factory.StartNew(() =>
            {
                while (clientInfo == null)
                {
                    Task.Delay(30);
                    if (cts.Token.IsCancellationRequested)
                        break;
                }
                return clientInfo;
            }, cts.Token);
        }
        public Task<QueueState> GetServerStateAsync()
        {
            var cts = new CancellationTokenSource();
            serverState = null;

            cts.CancelAfter(2000);
            client.SendAsync(
                new QueueData
                {
                    Cmd = Command.ServerState
                });

            return Task.Factory.StartNew(() =>
                {
                    while (serverState == null)
                    {
                        Task.Delay(30);
                        if (cts.Token.IsCancellationRequested)
                            break;
                    }
                    return serverState;
                }, cts.Token);
        }
    }

    #endregion
    public class ClientInfoEventArgs : EventArgs
    {
        public QueueClientInfo ClientInfo { get; set; }
    }
}

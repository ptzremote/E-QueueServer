using EQueueLib;
using QueueClientLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleQueueClient
{
    public class QueueClientDebug
    {
        QueueClient qClient;
        QueueState state;
        QueueClientInfo curentClient;

        public QueueClientDebug()
        {
            qClient = new QueueClient();
            qClient.OnNextClient += Client_OnNextClient;
            qClient.OnCompleteClient += Client_OnCompleteClient;
            qClient.OnQueueStateChanged += Client_OnQueueStateChanged;
        }

        #region Event Handlers
        private void Client_OnQueueStateChanged(object sender, QueueStateEventArgs e)
        {
            state = e.QueueState;
        }

        private void Client_OnCompleteClient(object sender, EventArgs e)
        {
            var clientInfo = (e as ClientInfoEventArgs).ClientInfo;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Client number: {clientInfo.ClientNumber} complete service time {clientInfo.CompleteServiceTime}");
            Console.ResetColor();
        }

        private void Client_OnNextClient(object sender, EventArgs e)
        {
            if (qClient.TryGetNextClient(out QueueClientInfo nextClient))
            {
                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Next client number: {nextClient.ClientNumber} " +
                    $"Window number: {nextClient.WindowNumber}");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();

                curentClient = nextClient;
            }
        }
        #endregion
        public void Connect(string host, int port)
        {
            qClient.Connect(host, port);
        }

        public (string, int) GetHostAndPort()
        {
            Console.Write("Enter QServer host: ");
            var host = Console.ReadLine();
            Console.Write("Enter QServer port: ");
            var inputPort = Console.ReadLine();

            int port = 0;
            if (host.Length > 0 && !Int32.TryParse(inputPort, out port))
            {
                throw new Exception($"{inputPort} can not be parse as integer, try again");
            }

            if (host.Length == 0)
                host = "localhost";
            if (port == 0)
                port = 11000;
            return (host, port);
        }

        public async void NewClient()
        {
            Console.WriteLine();
            Console.WriteLine("Which service do you want to select?");
            int n = 1;
            foreach (var service in PrintServiceList())
            {
                Console.WriteLine($"{n} - {service}");
                n++;
            }

            var inputServiceNumber = Console.ReadLine();

            Int32.TryParse(inputServiceNumber, out int serviceNumber);

            curentClient = await qClient.NewClientAsync(state.ServiceList[serviceNumber - 1]);

            Console.WriteLine($"OK! Client number is {curentClient.ClientNumber}");
        }

        private void FreeWindow()
        {
            Console.WriteLine();
            Console.WriteLine($"Enter window number: ");
            var inputWindow = Console.ReadLine();

            Int32.TryParse(inputWindow, out int window);

            Console.WriteLine("Which service?");
            List<ServiceInfo> services = new List<ServiceInfo>();
            while (true)
            {
                int n = 1;
                foreach (var service in PrintServiceList())
                {
                    Console.WriteLine($"{n} - {service}");
                    n++;
                }

                Console.WriteLine($"{n} - All done");

                Int32.TryParse(Console.ReadLine(), out int select);
                if (select == state.ServiceList.Count + 1)
                {
                    break;
                }
                else
                {
                    services.Add(state.ServiceList[select - 1]);
                }
            }

            qClient.FreeWindow(window, services);
        }

        public void PrintOptions()
        {
            Console.WriteLine();
            Console.WriteLine("Select require operation: ");
            Console.WriteLine("1 - new client");
            Console.WriteLine("2 - free window");
            Console.WriteLine("3 - take client by number");
            Console.WriteLine("4 - change service for client");
            Console.WriteLine("5 - complete client service");
            Console.WriteLine("6 - get server state");
            Console.WriteLine("7 - add many clients");

            Int32.TryParse(Console.ReadLine(), out int select);

            switch (select)
            {
                case 1:
                    NewClient();
                    break;
                case 2:
                    FreeWindow();
                    break;
                case 3:
                    TakeClient();
                    break;
                case 4:
                    ChangeService();
                    break;
                case 5:
                    CompleteClientService();
                    break;
                case 6:
                    GetAndPrintState();
                    break;
                case 7:
                    AddManyClients();
                    break;

                default:
                    break;
            }
        }

        private async void AddManyClients()
        {
            Console.WriteLine($"Enter client count: ");
            var inputClientCount = Console.ReadLine();

            Int32.TryParse(inputClientCount, out int ClientCount);
            Random rnd = new Random();
            for (int i = 1; i <= ClientCount; i++)
            {
                var client = await qClient.NewClientAsync(state.ServiceList[rnd.Next(0,state.ServiceList.Count)]);
                Console.WriteLine($"Client {i} added. Client number {client.ClientNumber}");
            }
        }

        private void CompleteClientService()
        {
            Console.WriteLine();
            Console.WriteLine($"Enter client number: ");
            var inputClient = Console.ReadLine();
            Int32.TryParse(inputClient, out int clientNumber);

            qClient.CompleteServiceForClient(curentClient, state.OperatorList[0]);
        }

        private void ChangeService()
        {
            Console.WriteLine();
            Console.WriteLine($"Enter client number: ");
            var inputClient = Console.ReadLine();
            Int32.TryParse(inputClient, out int clientNumber);

            Console.WriteLine("Which service?");

            int n = 1;
            foreach (var service in PrintServiceList())
            {
                Console.WriteLine($"{n} - {service}");
                n++;
            }

            Int32.TryParse(Console.ReadLine(), out int select);

            qClient.ChangeService(state.ClientInQueue.Where(c => c.ClientNumber == clientNumber).FirstOrDefault(), state.ServiceList[select -1]);
        }

        private void TakeClient()
        {
            Console.WriteLine();
            Console.WriteLine($"Enter client number: ");
            var inputClient = Console.ReadLine();
            Int32.TryParse(inputClient, out int clientNumber);


            Console.WriteLine();
            Console.WriteLine($"Enter window number: ");
            var inputWindow = Console.ReadLine();
            Int32.TryParse(inputWindow, out int window);

            qClient.FreeWindowForClient(window, state.ClientInQueue.Where(c => c.ClientNumber == clientNumber).FirstOrDefault());
        }

        public async void GetAndPrintState()
        {
            state = await qClient.GetServerStateAsync();

            if (state == null) return;

            Console.WriteLine();
            Console.WriteLine("Operators list: ");
            foreach (var oper in state.OperatorList)
            {
                Console.WriteLine($"Operator name: {oper.OperatorFIO}");
            }

            Console.WriteLine();
            foreach (var service in PrintServiceList())
            {
                Console.WriteLine(service);
            }

            Console.WriteLine();
            Console.WriteLine($"Client in queue: {state.ClientInQueue.Count}");
            foreach (var client in state.ClientInQueue)
            {
                Console.WriteLine($"number: {client.ClientNumber} " +
                    $"service: {client.ServiceInfo.ServiceName} ");
            }
        }

        IEnumerable<string> PrintServiceList()
        {
            Console.WriteLine("Services list: ");
            foreach (var service in state.ServiceList)
            {
                yield return service.ServiceName;
            }
        }


    }

}

using CoreNetLib;
using NLog.Extensions.Logging;

namespace ConsoleQueueClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreNetLogging.LoggerFactory.AddNLog();
            CoreNetLogging.LoggerFactory.ConfigureNLog("nlog.config");

            QueueClientDebug client = new QueueClientDebug();

            var server = client.GetHostAndPort();

            client.Connect(server.Item1, server.Item2);
            client.GetAndPrintState();
            while (true)
            {
                client.PrintOptions();
            }
        }
    }
}

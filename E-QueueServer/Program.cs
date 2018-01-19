using CoreNetLib;
using EQueueLib;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using QueueServerLib;
using System;

namespace EQueueServer
{
    class Program
    {
        static ILogger Logger { get; } =
    CoreNetLogging.LoggerFactory.CreateLogger<Program>();
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //configure logger
            CoreNetLogging.LoggerFactory.AddNLog();
            CoreNetLogging.LoggerFactory.ConfigureNLog("nlog.config");

            //initialize sqlite db (create db, seed data, migrate)
            QueueDB.Init();


            //using JSON serializer in tcp hub
            var netSettings = new NetSettings
            {
                serializer = JSONSerializerHelper.Serialize,
                deserializer = JSONSerializerHelper.Deserialize
            };

            //configure and create qServer instance
            QueueServer server = new QueueServer(new TcpHub(netSettings), new QueueDB());

            //initialize qServer with data from db
            server.QueueInitialize();

            //all prereq done, Run!
            server.Run();

            //delay
            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Logger.LogWarning("Unhadled domain exception:\n\n" + ex.Message);
            }
            catch
            {
                Logger.LogWarning("Fatal exception happend inside UnhadledExceptionHandler");
            }
            
        }
    }
}

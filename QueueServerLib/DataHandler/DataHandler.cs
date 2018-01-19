using CoreNetLib;
using Microsoft.Extensions.Logging;
using EQueueLib;
using System.Diagnostics;

namespace QueueServerLib
{
    abstract class DataHandler
    {
        ILogger Logger { get; } =
    CoreNetLogging.LoggerFactory.CreateLogger<DataHandler>();
        internal abstract QueueData HandleData(QueueServerState qsState, IDb db);
        internal QueueData Handle(QueueServerState qsState, IDb db)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            var qData = HandleData(qsState, db);
            Logger.LogTrace($"Handle received data time elapsed: {sw.ElapsedMilliseconds} ms");
            sw.Stop();

            return qData;
        }
    }
}

using CoreNetLib;
using Microsoft.Extensions.Logging;
using EQueueLib;
using System.Collections.Generic;
using System.Linq;

namespace QueueServerLib
{
    class FreeWindowDataHandler : DataHandler
    {
        ILogger Logger { get; } =
    CoreNetLogging.LoggerFactory.CreateLogger<FreeWindowDataHandler>();
        QueueData qData;
        public FreeWindowDataHandler(QueueData qData)
        {
            this.qData = qData;
        }
        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
            var serviceList = new List<ServiceInfo>();

            //service list will be null for BusyWindow method
            if (qData.ServiceList == null)
            {
                Logger.LogInformation($"Busy window: {qData.WindowNumber}");
                qsState.FreeWindow.Remove(qsState.FreeWindow.Where(kp => kp.Key == qData.WindowNumber).FirstOrDefault());
                return null;
            }

            //so if ServiceList not null, it`s a real free window
            Logger.LogInformation($"Free window: {qData.WindowNumber}");
            foreach (var service in qData.ServiceList)
            {
                serviceList.Add(db.GetServiceById(service.Id));
            }

            if (qData.WindowNumber > 0)
            {
                AddWindow(qData.WindowNumber, serviceList);
            }

            void AddWindow(int window, List<ServiceInfo> windowList)
            {
                var curentKeyValue = new KeyValuePair<int, List<ServiceInfo>>(window, windowList);
                foreach (var w in qsState.FreeWindow)
                {
                    if (w.Key == window)
                    {
                        qsState.FreeWindow.Remove(w);
                        break;
                    }
                }
                qsState.FreeWindow.Add(curentKeyValue);
            }
            return null;
        }
    }
}

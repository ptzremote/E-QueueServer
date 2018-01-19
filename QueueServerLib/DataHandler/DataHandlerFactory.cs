using EQueueLib;
using System;

namespace QueueServerLib
{
    static class DataHandlerFactory
    {
        public static DataHandler MakeHandler(QueueData qData)
        {
            switch (qData.Cmd)
            {
                case Command.NewClient:
                    return new NewClientDataHandler(qData);
                case Command.ServerState:
                    return new ServerStateDataHandler(qData);
                case Command.FreeWindow:
                    return new FreeWindowDataHandler(qData);
                case Command.TakeClient:
                    return new TakeClientDataHandler(qData);
                case Command.ChangeService:
                    return new ChangeServiceDataHandler(qData);
                case Command.NextClient:
                    return new NextClientQueueHandler(qData);
                case Command.CompleteClientService:
                    return new CompleteClientServiceHandler(qData);
                default:
                    throw new Exception("Not supported command recieved from client");
            }
        }
    }
}

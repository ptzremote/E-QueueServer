using CoreNetLib;
using EQueueLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueueServerLib
{
    class CompleteClientServiceHandler : DataHandler
    {
        QueueData qData;
        public CompleteClientServiceHandler(QueueData qData)
        {
            this.qData = qData;
        }

        internal override QueueData HandleData(QueueServerState qsState, IDb db)
        {
            var client = db.GetClientById(qData.ClientInfo.Id);

            if (client == null)
            {
                return null;
            }
                
            client.CompleteServiceTime = DateTime.Now;
            client.OperatorInfo = db.GetOperatorById(qData.SelectedOperator.Id);

            return QueueDataFactory.GetCompleteClientData(client);
        }
    }
}

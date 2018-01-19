using EQueueLib;
using System;

namespace QueueClientLib
{
    public class QueueStateEventArgs : EventArgs
    {
        public QueueState QueueState { get; set; }
    }
}
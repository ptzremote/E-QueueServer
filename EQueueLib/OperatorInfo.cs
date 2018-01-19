using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EQueueLib
{
    [Serializable]
    public class OperatorInfo : IEquatable<OperatorInfo>
    {
        public int Id { get; set; }
        public string OperatorFIO { get; set; }

        [NonSerialized]
        ICollection<QueueClientInfo> queueClientInfos;

        [JsonIgnore]
        public virtual ICollection<QueueClientInfo> QueueClientInfos
        {
            get
            {
                return queueClientInfos;
            }
            set
            {
                queueClientInfos = value;
            }
        }

        bool IEquatable<OperatorInfo>.Equals(OperatorInfo other)
        {
            if (Id == other.Id)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Id + OperatorFIO.GetHashCode();
        }
    }
}

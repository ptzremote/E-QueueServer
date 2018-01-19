using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EQueueLib
{
    [Serializable]
    public class ServiceInfo : IEquatable<ServiceInfo>
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }

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

        bool IEquatable<ServiceInfo>.Equals(ServiceInfo other)
        {
            if (Id == other.Id)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Id + ServiceName.GetHashCode();
        }
    }
}

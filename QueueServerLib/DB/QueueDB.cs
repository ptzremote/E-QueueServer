using Microsoft.EntityFrameworkCore;
using EQueueLib;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace QueueServerLib
{
    public class QueueDB : IDb
    {
        public void Init()
        {
            using (var qDb = new QueueDBContext())
            {
                qDb.Database.Migrate();
            }

            Initialize();
        }

        public List<QueueClientInfo> GetClientInQueue()
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.QueueClientInfo.
                Include(c => c.ServiceInfo).
                Include(o => o.OperatorInfo).
                Where(c => c.DequeueTime == null).ToList();
            }
        }

        public QueueClientInfo GetLastClient()
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.QueueClientInfo.LastOrDefault();
            }
        }

        public async void AddNewClient(QueueClientInfo newClient)
        {
            using (var qDb = new QueueDBContext())
            {
                var selectedService = qDb.ServiceInfo.Find(newClient.ServiceInfo.Id);
                newClient.ServiceInfo = selectedService;
                qDb.QueueClientInfo.Add(newClient);
                await qDb.SaveChangesAsync();
            }
        }

        public List<ServiceInfo> GetServiceList()
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.ServiceInfo.ToList();
            }
        }

        public List<OperatorInfo> GetOperatorList()
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.OperatorInfo.ToList();
            }
        }

        async void IDb.SaveChanges()
        {

            //var modifiedOrAddedEntities = qDb.ChangeTracker.Entries()
            //                                    .Where(x => x.State == EntityState.Modified
            //                                    || x.State == EntityState.Added)
            //                                    .Count();
            //if (isInitialize && modifiedOrAddedEntities > 100)
            using (var qDb = new QueueDBContext())
            {
                await qDb.SaveChangesAsync();
            }
        }

        public ServiceInfo GetServiceById(int id)
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.ServiceInfo.Find(id);
            }
        }

        public OperatorInfo GetOperatorById(int id)
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.OperatorInfo.Find(id);
            }
        }

        public QueueClientInfo GetClientById(int id)
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.QueueClientInfo.
                    Include(s => s.ServiceInfo).
                    Include(o => o.OperatorInfo).
                    Where(c => c.Id == id).
                    LastOrDefault();
            }
        }

        public QueueClientInfo GetLastDequeueClient()
        {
            using (var qDb = new QueueDBContext())
            {
                return qDb.QueueClientInfo.Where(c => c.DequeueTime != null).First();
            }
        }
        void Initialize()
        {
            using (var qDb = new QueueDBContext())
            {

                if (qDb.ServiceInfo.Any())
                {
                    return; // DB has been seeded
                }

                qDb.ServiceInfo.AddRange(
                    new ServiceInfo
                    {
                        ServiceName = "Запись на прием к врачу"
                    },
                    new ServiceInfo
                    {
                        ServiceName = "Запись на восстановительное лечение"
                    },
                    new ServiceInfo
                    {
                        ServiceName = "Справочная информация"
                    },
                    new ServiceInfo
                    {
                        ServiceName = "Прикрепление к поликлинике"
                    },
                    new ServiceInfo
                    {
                        ServiceName = "Поставить печать"
                    },
                    new ServiceInfo
                    {
                        ServiceName = "Прочее"
                    }
                    );

                qDb.OperatorInfo.AddRange(
                    new OperatorInfo
                    {
                        OperatorFIO = "Тимофеева Наталья"
                    },
                    new OperatorInfo
                    {
                        OperatorFIO = "Бусько Наталья"
                    },
                    new OperatorInfo
                    {
                        OperatorFIO = "Дуброва Гузилья"
                    },
                    new OperatorInfo
                    {
                        OperatorFIO = "Внукова Антонина"
                    },
                    new OperatorInfo
                    {
                        OperatorFIO = "Герасимова Светлана"
                    },
                    new OperatorInfo
                    {
                        OperatorFIO = "Шкурикова Диана"
                    }
                    );
                qDb.SaveChanges();
            }
        }

        public QueueClientInfo SetCompleteServiceInfo(int clientId, int operatorId)
        {
            using (var qDb = new QueueDBContext())
            {
                var cClient = qDb.QueueClientInfo.Find(clientId);
                cClient.CompleteServiceTime = DateTime.Now;
                cClient.OperatorInfo = qDb.OperatorInfo.Find(operatorId);
                qDb.SaveChanges();
                return cClient;
            }
        }

        public async void DeleteClientInfo(int clientId)
        {
            using (var qDb = new QueueDBContext())
            {
                var clientInfo = await qDb.QueueClientInfo.FindAsync(clientId);
                if (clientInfo != null)
                {
                    qDb.QueueClientInfo.Remove(clientInfo);
                    await qDb.SaveChangesAsync();
                }
            }
        }
    }
}

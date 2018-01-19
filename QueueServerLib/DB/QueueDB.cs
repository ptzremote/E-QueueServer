using Microsoft.EntityFrameworkCore;
using EQueueLib;
using System.Collections.Generic;
using System.Linq;

namespace QueueServerLib
{
    public class QueueDB : IDb
    {
        static QueueDBContext qDb;

        public static void Init()
        {
            qDb = new QueueDBContext();
            qDb.Database.Migrate();
            Initialize();
        }

        public List<QueueClientInfo> GetClientInQueue()
        {
                return qDb.QueueClientInfo.
                Include(c => c.ServiceInfo).
                Include(o => o.OperatorInfo).
                Where(c => c.DequeueTime == null).ToList();
        }

        public QueueClientInfo GetLastClient()
        {
                return qDb.QueueClientInfo.LastOrDefault();
        }

        public void AddNewClient(QueueClientInfo newClient)
        {
                qDb.QueueClientInfo.Add(newClient);
        }

        public List<ServiceInfo> GetServiceList()
        {
                return qDb.ServiceInfo.ToList();
        }

        public List<OperatorInfo> GetOperatorList()
        {
                return qDb.OperatorInfo.ToList();
        }

        void IDb.SaveChanges()
        {
           
                //var modifiedOrAddedEntities = qDb.ChangeTracker.Entries()
                //                                    .Where(x => x.State == EntityState.Modified
                //                                    || x.State == EntityState.Added)
                //                                    .Count();
                //if (isInitialize && modifiedOrAddedEntities > 100)
                qDb.SaveChanges();
           
        }

        public ServiceInfo GetServiceById(int Id)
        {
                return qDb.ServiceInfo.Where(c => c.Id == Id).FirstOrDefault();
        }

        public OperatorInfo GetOperatorById(int Id)
        {
                return qDb.OperatorInfo.Where(c => c.Id == Id).FirstOrDefault();
        }

        public QueueClientInfo GetClientById(int id)
        {
                return qDb.QueueClientInfo.
                Where(c => c.Id == id).
                LastOrDefault();
        }

        public QueueClientInfo GetLastDequeueClient()
        {
                return qDb.QueueClientInfo.Where(c => c.DequeueTime != null).First();
        }
        static async void Initialize()
        {

            if (await qDb.ServiceInfo.AnyAsync())
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

            await qDb.SaveChangesAsync();
        }
    }
}

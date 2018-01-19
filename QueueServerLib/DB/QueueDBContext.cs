using Microsoft.EntityFrameworkCore;
using EQueueLib;

namespace QueueServerLib
{
    public class QueueDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=equeue.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        internal DbSet<QueueClientInfo> QueueClientInfo { get; set; }
        internal DbSet<ServiceInfo> ServiceInfo { get; set; }
        internal DbSet<OperatorInfo> OperatorInfo { get; set; }
    }
}

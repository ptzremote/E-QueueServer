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

        public DbSet<QueueClientInfo> QueueClientInfo { get; set; }
        public DbSet<ServiceInfo> ServiceInfo { get; set; }
        public DbSet<OperatorInfo> OperatorInfo { get; set; }
    }
}

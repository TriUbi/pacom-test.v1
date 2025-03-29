using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Models;

namespace DeviceStatusApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DeviceStatus> Status => Set<DeviceStatus>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}

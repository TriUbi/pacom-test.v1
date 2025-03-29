using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Models;

namespace DeviceStatusApi.Data
{
    public class AppDbContext : DbContext
    {
       public DbSet<DeviceStatus> Status { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}

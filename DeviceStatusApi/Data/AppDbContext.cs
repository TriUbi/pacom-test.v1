using Microsoft.EntityFrameworkCore;
using DeviceStatusApi.Models;

namespace DeviceStatusApi.Data
{
    /// <summary>
    /// Klass som hanterar anslutning till min db
    /// Här kopplas modellen DeviceStatus till tabellen "Status"
    /// </summary>
    public class AppDbContext : DbContext
    {
         /// <summary>
        /// Representerar tabellen i db som innehåller alla enheters status
        /// </summary>
       public DbSet<DeviceStatus> Status { get; set; }

        /// <summary>
        /// Konstruktor som tar emot inställningar för databasen.
        /// </summary>
        /// <param name="options">Inställningar för databasen</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}
